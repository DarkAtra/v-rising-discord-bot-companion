using System;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppSystem.IO;
using Il2CppSystem.Net;
using Il2CppSystem.Text.RegularExpressions;
using ProjectM;
using ProjectM.Network;
using v_rising_discord_bot_companion.character;
using v_rising_discord_bot_companion.query;

namespace v_rising_discord_bot_companion.command;

[HarmonyPatch(typeof(ServerWebAPISystem))]
public class ServerWebAPISystemPatches {

    private readonly static JsonSerializerOptions _serializeOptions = new() {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [HarmonyPostfix]
    [HarmonyPatch("OnCreate")]
    public static void OnCreate(ServerWebAPISystem __instance) {

        if (!SettingsManager.ServerHostSettings.API.Enabled) {
            return;
        }

        __instance._HttpReceiveService.AddRoute(new HttpServiceReceiveThread.Route(
            new Regex("/v-rising-discord-bot/characters"),
            "GET",
            BuildAdapter(_ => CharacterInfoCommand.GetCharacters())
        ));
    }

    private static HttpServiceReceiveThread.RequestHandler BuildAdapter(Func<HttpListenerContext, object> commandHandler) {
        return DelegateSupport.ConvertDelegate<HttpServiceReceiveThread.RequestHandler>(
            new Action<HttpListenerContext>(context => {

                var commandResponse = QueryDispatcher.Instance.Dispatch(() => commandHandler(context));
                while (commandResponse.Status == Status.PENDING) {
                    Thread.Sleep(25);
                }

                object responseData;
                if (commandResponse.Status is Status.FAILED or Status.PENDING) {
                    Plugin.Logger.LogInfo($"Request with url '{context.Request.Url.ToString()}' failed with message: {commandResponse.Exception?.Message}");
                    context.Response.StatusCode = 500;
                    responseData = new Problem(
                        Type: "about:blank",
                        Title: "Internal Server Error"
                    );
                } else {
                    responseData = commandResponse.Data!;
                }

                context.Response.ContentType = MediaTypeNames.Application.Json;

                var responseWriter = new StreamWriter(context.Response.OutputStream);
                responseWriter.Write(JsonSerializer.Serialize(responseData, _serializeOptions));
                responseWriter.Flush();
            })
        )!;
    }
}
