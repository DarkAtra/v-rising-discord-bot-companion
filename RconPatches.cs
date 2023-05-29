// using HarmonyLib;
// using ProjectM;
//
// namespace v_rising_server_mod_test;
//
// [HarmonyPatch(typeof(RconListenerSystem))]
// public class RconPatches {
//     [HarmonyPostfix]
//     [HarmonyPatch("OnCreate")]
//     public static void OnCreate(RconListenerSystem __instance) {
//         if (!SettingsManager.ServerHostSettings.Rcon.Enabled) {
//             return;
//         }
//     
//         var handler = new Func<string, IList<string>, string>((cmd, _args) => { return ""; });
//     
//         __instance._Server.CommandManager.Add(
//             "atra_get_player_score",
//             "atra_get_player_score",
//             "Get the gear level for the current player.",
//             // new CommandHandler( ?? )
//         );
//     }
// }


