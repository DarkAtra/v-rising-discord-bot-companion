using System;
using Unity.Entities;
using UnityEngine;

namespace v_rising_discord_bot_companion;

public static class VWorld {

    private static World? _serverWorld;

    public static World Server {
        get {
            if (_serverWorld != null && _serverWorld.IsCreated)
                return _serverWorld;

            _serverWorld = GetWorld("Server")
                           ?? throw new Exception("There is no Server world (yet). Did you install a server mod on the client?");
            return _serverWorld;
        }
    }

    public static bool IsServer => Application.productName == "VRisingServer";

    private static World? GetWorld(string name) {
        foreach (var world in World.s_AllWorlds) {
            if (world.Name == name) {
                _serverWorld = world;
                return world;
            }
        }

        return null;
    }
}
