using System;
using System.Text.Json;
using System.Threading;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppSystem.IO;
using Il2CppSystem.Net;
using Il2CppSystem.Text.RegularExpressions;
using ProjectM;
using ProjectM.Network;
using v_rising_server_mod_test.gearlevel;
using v_rising_server_mod_test.query;

namespace v_rising_server_mod_test.command;

[HarmonyPatch(typeof(ServerWebAPISystem))]
public class ServerWebAPISystemPatches {
    
    private readonly static JsonSerializerOptions _serializeOptions = new() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

    [HarmonyPostfix]
    [HarmonyPatch("OnCreate")]
    public static void OnCreate(ServerWebAPISystem __instance) {

        if (!SettingsManager.ServerHostSettings.API.Enabled) {
            return;
        }

        var gearLevelRouteRegex = new Regex("/v-rising-discord-bot/characters/(?<characterName>[a-zA-Z0-9]+)/gear-level");
        __instance._HttpReceiveService.AddRoute(new HttpServiceReceiveThread.Route(
            gearLevelRouteRegex,
            "GET",
            BuildAdapter(context => {

                var requestUrl = context.Request.Url.ToString();
                var match = gearLevelRouteRegex.Match(requestUrl);

                var characterName = match.Groups["characterName"].Value!;
                return CharacterGearLevelCommand.HandleGetPlayerScore(characterName);
            })
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
                    // TODO: proper error handling
                    Plugin.Logger.LogInfo($"Request with url '{context.Request.Url.ToString()}' failed with message: {commandResponse.Exception?.Message}");
                    responseData = new Problem("about:blank", "Internal Server Error");
                } else {
                    responseData = commandResponse.Data!;
                }

                var responseWriter = new StreamWriter(context.Response.OutputStream);
                responseWriter.Write(JsonSerializer.Serialize(responseData, _serializeOptions));
                responseWriter.Flush();
            })
        )!;
    }
}
