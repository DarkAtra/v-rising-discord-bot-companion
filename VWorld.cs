using System;
using System.Collections.Generic;
using System.Linq;
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

    public static List<Player> GetAllPlayerCharacters() {
        return ListUtils.Convert(
                Server.EntityManager
                    .CreateEntityQuery(ComponentType.ReadOnly<PlayerCharacter>())
                    .ToEntityArray(Allocator.Temp)
            ).Select(entity =>
                new Player(entity, Server.EntityManager.GetComponentData<PlayerCharacter>(entity))
            )
            .ToList();
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

public class Player {
    public Entity Entity { get; private set; }
    public PlayerCharacter Character { get; private set; }

    public Player(Entity entity, PlayerCharacter character) {
        Entity = entity;
        Character = character;
    }
}
