using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ProjectM;
using ProjectM.CastleBuilding;
using ProjectM.Network;
using ProjectM.Scripting;
using Stunlock.Core;
using Unity.Collections;
using v_rising_discord_bot_companion.game;

namespace v_rising_discord_bot_companion.raid;

[HarmonyPatch(typeof(DeathEventListenerSystem))]
public class DeathEventListenerSystemPatches {

    private static readonly PrefabGUID SIEGE_GOLEM_PREFAB_ID = new(914043867);
    private static ServerGameManager? serverGameManager;

    private static readonly Dictionary<ulong, Raid> _raids = new();

    public static List<Raid> getRaids() {
        removeExpiredRaids();
        return _raids.Select(it => it.Value).ToList();
    }

    private static void removeExpiredRaids() {
        List<ulong> expiredRaids = new();
        foreach (var keyValuePair in _raids) {
            if (DateTime.UtcNow > keyValuePair.Value.Updated.AddMinutes(10)) {
                expiredRaids.Add(keyValuePair.Key);
            }
        }
        foreach (var expiredRaid in expiredRaids) {
            _raids.Remove(expiredRaid);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(DeathEventListenerSystem.OnUpdate))]
    public static void Postfix(DeathEventListenerSystem __instance) {

        var deathEvents = __instance._DeathEventQuery.ToComponentDataArray<DeathEvent>(Allocator.Temp);
        try {
            foreach (var deathEvent in deathEvents) {

                if (VWorld.Server.EntityManager.HasComponent<AnnounceCastleBreached>(deathEvent.Died)
                    && deathEvent.StatChangeReason.Equals(StatChangeReason.StatChangeSystem_0)) {

                    // if a player with siege golem buff breached the castle
                    if (getServerGameManager().TryGetBuff(deathEvent.Killer, SIEGE_GOLEM_PREFAB_ID.ToIdentifier(), out _)) {
                        onCastleBreached(deathEvent);
                    }
                }
            }
        } catch (Exception e) {
            Plugin.Logger.LogError($"Exception in DeathEventListenerSystem#OnUpdate: {e.Message}");
        } finally {
            deathEvents.Dispose();
        }

        removeExpiredRaids();
    }

    private static void onCastleBreached(DeathEvent deathEvent) {

        var attackingPlayer = VPlayer.from(VWorld.Server.EntityManager.GetComponentData<PlayerCharacter>(deathEvent.Killer).UserEntity);
        var castleHeartEntity = VWorld.Server.EntityManager.GetComponentData<CastleHeartConnection>(deathEvent.Died).CastleHeartEntity._Entity;
        var defendingPlayer = VPlayer.from(VWorld.Server.EntityManager.GetComponentData<UserOwner>(castleHeartEntity).Owner._Entity);

        var attackers = new List<VPlayer>();
        attackers.Add(attackingPlayer);
        attackers.AddRange(getClanMembers(attackingPlayer));

        var defenders = new List<VPlayer>();
        defenders.Add(defendingPlayer);
        defenders.AddRange(getClanMembers(defendingPlayer));

        var castleOwnerId = defendingPlayer.GetId(); // might be better to use the castle hearth as id here
        var now = DateTime.UtcNow;

        if (!_raids.TryGetValue(castleOwnerId, out var raid)) {

            var raidId = Guid.NewGuid();

            Plugin.Logger.LogDebug($"New raid with id '{raidId}' started for castle owned by player with id '{castleOwnerId}'...");

            _raids.Add(
                castleOwnerId,
                new Raid(
                    Id: raidId,
                    Attackers: attackers.Select(it => map(it, now)).ToList(),
                    Defenders: defenders.Select(it => map(it, now)).ToList(),
                    Occurred: now,
                    Updated: now
                )
            );

            Plugin.Logger.LogDebug($"Added new raid with id '{raid.Id}'.");
            return;
        }

        Plugin.Logger.LogDebug($"Updating raid with id '{raid.Id}' if necessary...");

        foreach (var vPlayer in attackers) {
            if (raid.Attackers.All(it => it.Id != vPlayer.GetId())) {
                Plugin.Logger.LogDebug($"New attacker joined the raid with id '{raid.Id}'. Setting raid.Updated to: {now}");
                raid.Attackers.Add(map(vPlayer, now));
                raid.Updated = now; // FIXME: this does not work for some reason
            }
        }
        foreach (var vPlayer in defenders) {
            if (raid.Defenders.All(it => it.Id != vPlayer.GetId())) {
                Plugin.Logger.LogDebug($"New defender joined the raid with id '{raid.Id}'. Setting raid.Updated to: {now}");
                raid.Defenders.Add(map(vPlayer, now));
                raid.Updated = now; // FIXME: this does not work for some reason
            }
        }

        Plugin.Logger.LogDebug($"Finished updating raid with id '{raid.Id}'.");
    }

    private static Player map(VPlayer player, DateTime now) {
        return new Player(
            Id: player.GetId(),
            Name: ((VCharacter) player.VCharacter!).Character.Name.ToString(),
            GearLevel: ((VCharacter) player.VCharacter!).getGearLevel(),
            Clan: player.VUser.GetClanName(),
            JoinedAt: now
        );
    }

    private static List<VPlayer> getClanMembers(VPlayer player) {

        var clanEntity = player.VUser.User.ClanEntity._Entity;
        var clanMembers = new List<VPlayer>();

        if (VWorld.Server.EntityManager.HasComponent<ClanTeam>(clanEntity)) {
            var users = VWorld.Server.EntityManager.GetBuffer<SyncToUserBuffer>(clanEntity);
            foreach (var user in users) {
                if (user.UserEntity != player.VUser.UserEntity) {
                    clanMembers.Add(VPlayer.from(user.UserEntity));
                }
            }
        }

        return clanMembers;
    }

    private static ServerGameManager getServerGameManager() {
        if (serverGameManager == null) {
            serverGameManager = VWorld.Server.GetExistingSystemManaged<ServerScriptMapper>().GetServerGameManager();
        }
        return (ServerGameManager) serverGameManager;
    }
}
