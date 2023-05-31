using System;
using System.Collections.Generic;
using ProjectM;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace v_rising_server_mod_test;

public static class VWorld {
    private static World? _serverWorld;

    public static World Server {
        get {
            if (_serverWorld != null) {
                return _serverWorld;
            }

            _serverWorld = GetWorld("Server") ?? throw new Exception(
                "There is no Server world (yet). Did you install a server mod on the client?"
            );
            return _serverWorld;
        }
    }

    public static bool IsServer => Application.productName == "VRisingServer";

    public static List<PlayerCharacter> GetAllOnlinePlayerCharacters() {
        return ListUtils.convert(Server.EntityManager
            .CreateEntityQuery(ComponentType.ReadOnly<PlayerCharacter>())
            .ToComponentDataArray<PlayerCharacter>(Allocator.Temp));
    }

    private static World? GetWorld(string name) {
        foreach (var world in World.s_AllWorlds) {
            if (world.Name == name) {
                return world;
            }
        }
        return null;
    }
}
