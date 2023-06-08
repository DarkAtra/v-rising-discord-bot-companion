using System;
using System.Collections.Generic;
using System.Linq;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace v_rising_discord_bot_companion.game;

public static class VWorld {

    private readonly static Lazy<World> _server = new(() => {
        foreach (var world in World.s_AllWorlds) {
            if (world.Name == "Server") {
                return world;
            }
        }
        throw new Exception("There is no Server world (yet). Did you install a server mod on the client?");
    });

    public readonly static World Server = _server.Value;

    public static EntityManager EntityManager = Server.EntityManager;
    public static bool IsServer => Application.productName == "VRisingServer";

    public static List<VPlayer> GetAllPlayerCharacters() {
        return ListUtils.Convert(
                EntityManager
                    .CreateEntityQuery(ComponentType.ReadOnly<User>())
                    .ToEntityArray(Allocator.Temp)
            )
            .Where(userEntity => EntityManager.GetComponentData<User>(userEntity).LocalCharacter._Entity != Entity.Null)
            .Select(userEntity => {
                var user = EntityManager.GetComponentData<User>(userEntity);
                return new VPlayer(user,
                    userEntity,
                    EntityManager.GetComponentData<PlayerCharacter>(user.LocalCharacter._Entity),
                    user.LocalCharacter._Entity);
            })
            .ToList();
    }
}
