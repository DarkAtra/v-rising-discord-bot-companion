using System;
using System.Collections.Generic;
using System.Linq;
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
            DelegateSupport.ConvertDelegate<CommandHandler>(
                new Func<string, Il2CppSystem.Collections.Generic.IList<string>, string>((command, _args) => {
                    var args = ListUtils.convert(_args);
                    return HandleGetPlayerScore(command, args);
                })
            )
        );
    }

    private static string HandleGetPlayerScore(string command, List<string> args) {

        if (args.Count != 1) {
            throw new ArgumentException("Error: this command expects exactly one argument <name>");
        }

        var characterName = args[0];
        Plugin.Logger.LogInfo($"Using character name: {characterName}");

        var character = VWorld.GetAllOnlinePlayerCharacters()
                            .FirstOrDefault(character => character.Name.ToString() == characterName) as PlayerCharacter?
                        ?? throw new ArgumentException($"Error: no character with name '{characterName}' exists");

        Plugin.Logger.LogInfo($"Found character for that name. UserEntity: {character.UserEntity}");

        var equipment = VWorld.Server.EntityManager.GetComponentData<Equipment>(character.UserEntity);
        return $"{equipment.GetFullLevel()}";
    }
}
