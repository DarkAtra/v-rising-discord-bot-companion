using System;
using System.Collections.Generic;
using System.Linq;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace v_rising_discord_bot_companion;

public static class VWorld {

    private readonly static Lazy<World> _server = new(() =>
        GetWorld("Server") ?? throw new Exception("There is no Server world (yet). Did you install a server mod on the client?")
    );

    public readonly static World Server = _server.Value;

    public static EntityManager EntityManager = Server.EntityManager;
    public static bool IsServer => Application.productName == "VRisingServer";

    public static List<Player> GetAllPlayerCharacters() {
        return ListUtils.Convert(
                EntityManager
                    .CreateEntityQuery(ComponentType.ReadOnly<User>())
                    .ToEntityArray(Allocator.Temp)
            )
            .Where(userEntity => EntityManager.GetComponentData<User>(userEntity).LocalCharacter._Entity != Entity.Null)
            .Select(userEntity => {
                var user = EntityManager.GetComponentData<User>(userEntity);
                return new Player(user,
                    userEntity,
                    EntityManager.GetComponentData<PlayerCharacter>(user.LocalCharacter._Entity),
                    user.LocalCharacter._Entity);
            })
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

public readonly record struct Player(
    User User,
    Entity UserEntity,
    PlayerCharacter Character,
    Entity CharacterEntity
);
