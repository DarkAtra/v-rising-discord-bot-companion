using System;
using System.Collections.Generic;
using HarmonyLib;
using ProjectM;
using v_rising_discord_bot_companion.common;
using v_rising_discord_bot_companion.game;

namespace v_rising_discord_bot_companion.killfeed;

[HarmonyPatch(typeof(VBloodSystem))]
public class VBloodSystemPatches {

    private static readonly List<VBloodKill> _vBloodKills = new();

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

            var killersPerVBlood = new Dictionary<VBlood, List<Player>>();
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
                        killersPerVBlood,
                        vBlood,
                        new Player(
                            Name: ((VCharacter) vPlayer.VCharacter!).Character.Name.ToString(),
                            GearLevel: ((VCharacter) vPlayer.VCharacter!).getGearLevel()
                        )
                    );
                }
            }

            foreach (var (vBlood, killers) in killersPerVBlood) {
                _vBloodKills.Add(
                    new VBloodKill(
                        Killers: killers,
                        VBlood: vBlood,
                        Occurred: DateTime.UtcNow
                    )
                );
            }
        }
    }

    private static void addKiller(Dictionary<VBlood, List<Player>> killersPerVBlood, VBlood vBlood, Player player) {
        if (!killersPerVBlood.ContainsKey(vBlood)) {
            killersPerVBlood[vBlood] = new List<Player>();
        }
        killersPerVBlood[vBlood].Add(player);
    }
}
