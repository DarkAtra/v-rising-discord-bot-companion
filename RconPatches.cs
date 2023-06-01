using System;
using System.Collections.Generic;
using System.Threading;
using HarmonyLib;
using Il2CppInterop.Runtime;
using ProjectM;
using RCONServerLib.Utils;

namespace v_rising_server_mod_test;

[HarmonyPatch(typeof(RconListenerSystem))]
public class RconPatches {

    [HarmonyPostfix]
    [HarmonyPatch("OnCreate")]
    public static void OnCreate(RconListenerSystem __instance) {

        if (!SettingsManager.ServerHostSettings.Rcon.Enabled) {
            return;
        }

        __instance._Server.CommandManager.Add(
            "atra_get_player_score",
            "atra_get_player_score name",
            "Get the gear level for a player by name.",
            BuildAdapter(GetPlayerScoreCommand.HandleGetPlayerScore)
        );
    }

    private static CommandHandler BuildAdapter(Func<string, List<string>, string> commandHandler) {
        return DelegateSupport.ConvertDelegate<CommandHandler>(
            new Func<string, Il2CppSystem.Collections.Generic.IList<string>, string>((command, args) => {

                var commandResponse = RconCommandDispatcher.Instance.Dispatch(command, ListUtils.Convert(args), commandHandler);
                while (commandResponse.Status == Status.PENDING) {
                    Thread.Sleep(25);
                }

                if (commandResponse.Status == Status.FAILED) {
                    Plugin.Logger.LogInfo($"Exception handling command '{command}': {commandResponse.Exception?.Message}");
                    return "error";
                }

                return commandResponse.Data ?? "error";
            })
        )!;
    }
}
