using System;
using System.Collections.Generic;
using HarmonyLib;
using ProjectM;
using v_rising_discord_bot_companion.common;
using v_rising_discord_bot_companion.game;

namespace v_rising_discord_bot_companion.killfeed;

[HarmonyPatch(typeof(VBloodSystem))]
public class VBloodSystemPatches {

    private const double EVENT_DELAY = 2;

    private static readonly List<VBloodKill> _vBloodKills = new();
    private static readonly Dictionary<VBlood, List<Player>> killersPerVBlood = new();
    private static readonly Dictionary<VBlood, DateTime> lastKillerUpdates = new();

    public static List<VBloodKill> getVBloodKills() {
        removeExpiredVBloodKills();
        return _vBloodKills;
    }

    private static void removeExpiredVBloodKills() {
        _vBloodKills.RemoveAll(vBloodKill => DateTime.UtcNow > vBloodKill.Occurred.AddMinutes(10));
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(VBloodSystem.OnUpdate))]
    private static void Prefix(VBloodSystem __instance) {

        var vBloodConsumedEvents = __instance.EventList;

        if (vBloodConsumedEvents.Length > 0) {

            foreach (var vBloodConsumedEvent in vBloodConsumedEvents) {
                if (VWorld.Server.EntityManager.HasComponent<PlayerCharacter>(vBloodConsumedEvent.Target)) {

                    var player = VWorld.Server.EntityManager.GetComponentData<PlayerCharacter>(vBloodConsumedEvent.Target);
                    var vPlayer = VPlayer.from(player.UserEntity);

                    if (!Enum.IsDefined(typeof(VBlood), vBloodConsumedEvent.Source.GuidHash)) {
                        Plugin.Logger.LogWarning($"An unknown VBlood was killed: {vBloodConsumedEvent.Source.GuidHash}");
                        continue;
                    }

                    var vBlood = (VBlood) vBloodConsumedEvent.Source.GuidHash;
                    addKiller(
                        vBlood,
                        new Player(
                            Name: ((VCharacter) vPlayer.VCharacter!).Character.Name.ToString(),
                            GearLevel: ((VCharacter) vPlayer.VCharacter!).getGearLevel()
                        )
                    );
                    lastKillerUpdates[vBlood] = DateTime.UtcNow;
                }
            }
        } else if (lastKillerUpdates.Count > 0) {

            var readyForEvent = new List<VBlood>();
            foreach (var (vBlood, lastUpdateTime) in lastKillerUpdates) {
                if (DateTime.UtcNow - lastUpdateTime < TimeSpan.FromSeconds(EVENT_DELAY)) {
                    continue;
                }
                readyForEvent.Add(vBlood);
            }

            foreach (var vBlood in readyForEvent) {
                lastKillerUpdates.Remove(vBlood);
                _vBloodKills.Add(
                    new VBloodKill(
                        Killers: killersPerVBlood[vBlood],
                        VBlood: vBlood,
                        Occurred: DateTime.UtcNow
                    )
                );
            }
        }
    }

    private static void addKiller(VBlood vBlood, Player player) {
        if (!killersPerVBlood.ContainsKey(vBlood)) {
            killersPerVBlood[vBlood] = new List<Player>();
        }
        killersPerVBlood[vBlood].Add(player);
    }
}
