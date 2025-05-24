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

    private static readonly List<Raid> _raids = [];

    public static List<Raid> getRaids() {
        removeExpiredPvpKills();
        return _raids;
    }

    private static void removeExpiredPvpKills() {
        _raids.RemoveAll(raid => DateTime.UtcNow > raid.Occurred.AddMinutes(10));
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

                        var attackingPlayer = VPlayer.from(VWorld.Server.EntityManager.GetComponentData<PlayerCharacter>(deathEvent.Killer).UserEntity);

                        var attackers = new List<VPlayer>();
                        attackers.Add(attackingPlayer);
                        attackers.AddRange(getClanMembers(attackingPlayer));

                        var castleHeartEntity = VWorld.Server.EntityManager.GetComponentData<CastleHeartConnection>(deathEvent.Died).CastleHeartEntity._Entity;
                        var defendingPlayer = VPlayer.from(VWorld.Server.EntityManager.GetComponentData<UserOwner>(castleHeartEntity).Owner._Entity);

                        var defenders = new List<VPlayer>();
                        defenders.Add(defendingPlayer);
                        defenders.AddRange(getClanMembers(defendingPlayer));

                        _raids.Add(
                            new Raid(
                                Attackers: attackers.Select(map).ToList(),
                                Defenders: defenders.Select(map).ToList(),
                                Occurred: DateTime.UtcNow
                            )
                        );
                    }
                }
            }
        } catch (Exception e) {
            Plugin.Logger.LogError($"Exception in DeathEventListenerSystem#OnUpdate: {e.Message}");
        } finally {
            deathEvents.Dispose();
        }

        removeExpiredPvpKills();
    }

    private static Player map(VPlayer player) {
        return new Player(
            Name: ((VCharacter) player.VCharacter!).Character.Name.ToString(),
            GearLevel: ((VCharacter) player.VCharacter!).getGearLevel(),
            Clan: player.VUser.GetClanName()
        );
    }

    private static List<VPlayer> getClanMembers(VPlayer player) {

        var clanEntity = player.VUser.User.ClanEntity._Entity;
        var clanMembers = new List<VPlayer>();

        if (VWorld.Server.EntityManager.HasComponent<ClanTeam>(clanEntity)) {
            var users = VWorld.Server.EntityManager.GetBuffer<SyncToUserBuffer>(clanEntity);
            foreach (SyncToUserBuffer user in users) {
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
