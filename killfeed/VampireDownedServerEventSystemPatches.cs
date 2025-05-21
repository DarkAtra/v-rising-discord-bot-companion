using System;
using System.Collections.Generic;
using HarmonyLib;
using ProjectM;
using Unity.Collections;
using Unity.Entities;
using v_rising_discord_bot_companion.game;

namespace v_rising_discord_bot_companion.killfeed;

[HarmonyPatch(typeof(VampireDownedServerEventSystem))]
public class VampireDownedServerEventSystemPatches {

    private static readonly List<PvpKill> _pvpKills = [];

    public static List<PvpKill> getPvpKills() {
        removeExpiredPvpKills();
        return _pvpKills;
    }

    private static void removeExpiredPvpKills() {
        _pvpKills.RemoveAll(pvpKill => DateTime.UtcNow > pvpKill.Occurred.AddMinutes(10));
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(VampireDownedServerEventSystem.OnUpdate))]
    public static void Postfix(VampireDownedServerEventSystem __instance) {

        if (__instance.__query_1174204813_0 == null) {
            return;
        }

        var entities = __instance.__query_1174204813_0.ToEntityArray(Allocator.Temp);
        foreach (var entity in entities) {
            handleDownedEntity(entity);
        }

        removeExpiredPvpKills();
    }

    private static void handleDownedEntity(Entity entity) {

        VampireDownedServerEventSystem.TryFindRootOwner(entity, 1, VWorld.Server.EntityManager, out var victimEntity);
        VWorld.Server.EntityManager.TryGetComponentData<VampireDownedBuff>(entity, out var buff);
        VampireDownedServerEventSystem.TryFindRootOwner(buff.Source, 1, VWorld.Server.EntityManager, out var killerEntity);

        if (!VWorld.Server.EntityManager.HasComponent<PlayerCharacter>(killerEntity)
            || !VWorld.Server.EntityManager.HasComponent<PlayerCharacter>(victimEntity)
            || victimEntity.Equals(killerEntity)) {
            return;
        }

        var killer = VCharacter.from(killerEntity);
        var victim = VCharacter.from(victimEntity);

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
