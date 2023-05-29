// using HarmonyLib;
// using ProjectM.Auth;
// using Steamworks;
//
// namespace v_rising_server_mod_test;
//
// [HarmonyPatch(typeof(SteamPlatformSystem))]
// public class QueryPatches {
//     [HarmonyPatch("_PlayersResponse", MethodType.Getter)]
//     public static void Prefix(SteamPlatformSystem __instance, ref ISteamMatchmakingPlayersResponse _PlayersResponse) {
//         Plugin.Logger.LogInfo("Players: " + _PlayersResponse);
//     }
// }
