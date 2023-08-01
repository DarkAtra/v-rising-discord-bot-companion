using System;
using System.Collections.Generic;
using Bloodstone.API;
using HarmonyLib;
using ProjectM;
using Unity.Collections;
using v_rising_discord_bot_companion.game;

namespace v_rising_discord_bot_companion.killfeed;

[HarmonyPatch(typeof(DeathEventListenerSystem))]
public class DeathEventListenerSystemPatches {

    private readonly static List<PvpKill> _pvpKills = new();

    public static List<PvpKill> getPvpKills() {
        removeExpiredPvpKills();
        return _pvpKills;
    }

    private static void removeExpiredPvpKills() {
        _pvpKills.RemoveAll(pvpKill => DateTime.UtcNow > pvpKill.Occurred.AddMinutes(10));
    }

    [HarmonyPostfix]
    [HarmonyPatch("OnUpdate")]
    public static void Postfix(DeathEventListenerSystem __instance) {

        if (__instance._DeathEventQuery == null) {
            return;
        }

        var events = __instance._DeathEventQuery.ToComponentDataArray<DeathEvent>(Allocator.Temp);
        foreach (var deathEvent in events) {
            handleDeathEvent(deathEvent);
        }

        removeExpiredPvpKills();
    }

    private static void handleDeathEvent(DeathEvent deathEvent) {

        if (!VWorld.Server.EntityManager.HasComponent<PlayerCharacter>(deathEvent.Killer)
            || !VWorld.Server.EntityManager.HasComponent<PlayerCharacter>(deathEvent.Died)) {
            return;
        }

        if (!VWorld.Server.EntityManager.HasComponent<PlayerCharacter>(deathEvent.Killer) || !VWorld.Server.EntityManager.HasComponent<PlayerCharacter>(deathEvent.Died)) {
            return;
        }

        var killer = VCharacter.from(deathEvent.Killer);
        var victim = VCharacter.from(deathEvent.Died);

        _pvpKills.Add(new PvpKill(
            Killer: new Player(
                Name: killer.Character.Name.ToString(),
                GearLevel: killer.getGearLevel()
            ),
            Victim: new Player(
                Name: victim.Character.Name.ToString(),
                GearLevel: victim.getGearLevel()
            ),
            Occurred: DateTime.UtcNow
        ));
    }
}
