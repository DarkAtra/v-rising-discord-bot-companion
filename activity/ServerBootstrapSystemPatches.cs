using System.Collections.Generic;
using HarmonyLib;
using ProjectM;
using Stunlock.Network;
using v_rising_discord_bot_companion.game;
using DateTime = System.DateTime;

namespace v_rising_discord_bot_companion.activity;

[HarmonyPatch(typeof(ServerBootstrapSystem))]
public class ServerBootstrapSystemPatches {

    private static readonly List<PlayerActivity> _playerActivities = [];

    public static List<PlayerActivity> getPlayerActivities() {
        removeExpiredPlayerActivities();
        return _playerActivities;
    }

    private static void removeExpiredPlayerActivities() {
        _playerActivities.RemoveAll(playerActivity => DateTime.UtcNow > playerActivity.Occurred.AddMinutes(10));
    }

    [HarmonyPrefix]
    [HarmonyPatch("OnUserConnected")]
    public static void OnUserConnected(
        ServerBootstrapSystem __instance,
        NetConnectionId netConnectionId
    ) {

        var userIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
        var serverClient = __instance._ApprovedUsersLookup[userIndex];
        var userEntity = serverClient.UserEntity;
        var vPlayer = VPlayer.from(userEntity);

        // no character exists yet, wait until one is created via SpawnCharacter
        if (vPlayer.VUser.User.CharacterName.IsEmpty) {
            return;
        }

        Plugin.Logger.LogDebug($"OnUserConnected: {vPlayer.VUser.User.CharacterName.ToString()}");

        _playerActivities.Add(
            new PlayerActivity(
                Type: ActivityType.CONNECTED,
                PlayerName: vPlayer.VUser.User.CharacterName.ToString(),
                Occurred: DateTime.UtcNow
            )
        );

        removeExpiredPlayerActivities();
    }

    [HarmonyPrefix]
    [HarmonyPatch("OnUserDisconnected")]
    public static void OnUserDisconnected(
        ServerBootstrapSystem __instance,
        NetConnectionId netConnectionId,
        ConnectionStatusChangeReason connectionStatusReason,
        string extraData
    ) {

        var userIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
        var serverClient = __instance._ApprovedUsersLookup[userIndex];
        var userEntity = serverClient.UserEntity;
        var vPlayer = VPlayer.from(userEntity);

        // no character exists yet, wait until one is created via SpawnCharacter
        if (vPlayer.VUser.User.CharacterName.IsEmpty) {
            return;
        }

        Plugin.Logger.LogDebug($"OnUserDisconnected: {vPlayer.VUser.User.CharacterName.ToString()}");

        _playerActivities.Add(
            new PlayerActivity(
                Type: ActivityType.DISCONNECTED,
                PlayerName: vPlayer.VUser.User.CharacterName.ToString(),
                Occurred: DateTime.UtcNow
            )
        );

        removeExpiredPlayerActivities();
    }

    // [HarmonyPrefix]
    // [HarmonyPatch("SpawnCharacter")]
    // public static void SpawnCharacter(
    //     ServerBootstrapSystem __instance,
    //     EntityCommandBuffer commandBuffer,
    //     Entity prefab,
    //     Entity user,
    //     Nullable_Unboxed<float3> customSpawnPosition
    // ) {
    //
    //     var vPlayer = VPlayer.from(user);
    //
    //     Plugin.Logger.LogDebug($"SpawnCharacter: {vPlayer.VUser.User.CharacterName.ToString()}");
    //
    //     _playerActivities.Add(
    //         new PlayerActivity(
    //             Type: ActivityType.CONNECTED,
    //             PlayerName: vPlayer.VUser.User.CharacterName.ToString(),
    //             Occurred: DateTime.UtcNow
    //         )
    //     );
    //
    //     removeExpiredPlayerActivities();
    // }
}
