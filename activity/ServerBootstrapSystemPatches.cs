using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using Stunlock.Network;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using v_rising_discord_bot_companion.game;
using DateTime = System.DateTime;

namespace v_rising_discord_bot_companion.activity;

[HarmonyPatch]
public class ServerBootstrapSystemPatches {

    private static readonly List<PlayerActivity> _playerActivities = [];
    private static readonly WaitForSeconds newCharacterDelay = new(2.5f);

    public static List<PlayerActivity> getPlayerActivities() {
        removeExpiredPlayerActivities();
        return _playerActivities;
    }

    private static void removeExpiredPlayerActivities() {
        _playerActivities.RemoveAll(playerActivity => DateTime.UtcNow > playerActivity.Occurred.AddMinutes(10));
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserConnected))]
    public static void OnUserConnected(
        ServerBootstrapSystem __instance,
        NetConnectionId netConnectionId
    ) {

        var userIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
        var serverClient = __instance._ApprovedUsersLookup[userIndex]!;
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
    [HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserDisconnected))]
    public static void OnUserDisconnected(
        ServerBootstrapSystem __instance,
        NetConnectionId netConnectionId,
        ConnectionStatusChangeReason connectionStatusReason,
        string extraData
    ) {

        if (!__instance._NetEndPointToApprovedUserIndex.TryGetValue(netConnectionId, out var userIndex)
            || userIndex >= __instance._ApprovedUsersLookup.Count) {
            return;
        }

        var serverClient = __instance._ApprovedUsersLookup[userIndex];
        if (serverClient == null) {
            return;
        }

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

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HandleCreateCharacterEventSystem), nameof(HandleCreateCharacterEventSystem.OnUpdate))]
    public static void OnCharacterCreated(HandleCreateCharacterEventSystem __instance) {

        var fromCharacterEvents = __instance._CreateCharacterEventQuery.ToComponentDataArray<FromCharacter>(Allocator.Temp);

        foreach (var fromCharacter in fromCharacterEvents) {

            var userEntity = fromCharacter.User;

            Plugin.Instance.StartCoroutine(HandleCharacterCreatedRoutine(userEntity));
        }

        fromCharacterEvents.Dispose();
    }

    private static IEnumerator HandleCharacterCreatedRoutine(Entity userEntity) {

        yield return newCharacterDelay;

        var vPlayer = VPlayer.from(userEntity);

        Plugin.Logger.LogDebug($"OnCharacterCreated: {vPlayer.VUser.User.CharacterName.ToString()}");

        _playerActivities.Add(
            new PlayerActivity(
                Type: ActivityType.CONNECTED,
                PlayerName: vPlayer.VUser.User.CharacterName.ToString(),
                Occurred: DateTime.UtcNow
            )
        );

        removeExpiredPlayerActivities();
    }
}
