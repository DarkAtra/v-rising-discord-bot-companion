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
    static void Postfix(DeathEventListenerSystem __instance) {

        var deathEvents = __instance._DeathEventQuery.ToComponentDataArray<DeathEvent>(Allocator.Temp);
        try {
            foreach (DeathEvent deathEvent in deathEvents) {

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

        var raidId = defendingPlayer.VUser.User.PlatformId; // might be better to use the castle hearth as id here
        var now = DateTime.UtcNow;

        if (!_raids.TryGetValue(raidId, out var raid)) {
            _raids.Add(
                raidId,
                new Raid(
                    Id: Guid.NewGuid(),
                    Attackers: attackers.Select(it => map(it, now)).ToList(),
                    Defenders: defenders.Select(it => map(it, now)).ToList(),
                    Occurred: now,
                    Updated: now
                )
            );
            return;
        }

        foreach (var vPlayer in attackers) {
            var mapped = map(vPlayer, now);
            if (raid.Attackers.All(it => it.Id != mapped.Id)) {
                raid.Attackers.Add(mapped);
                raid.Updated = now; // FIXME: this does not work for some reason
            }
        }
        foreach (var vPlayer in defenders) {
            var mapped = map(vPlayer, now);
            if (raid.Defenders.All(it => it.Id != mapped.Id)) {
                raid.Defenders.Add(mapped);
                raid.Updated = now; // FIXME: this does not work for some reason
            }
        }
    }

    private static Player map(VPlayer player, DateTime now) {
        return new Player(
            Id: player.VUser.User.PlatformId,
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
