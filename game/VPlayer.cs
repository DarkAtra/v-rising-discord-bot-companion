using System.Collections.Generic;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace v_rising_discord_bot_companion.game;

public readonly record struct VPlayer(
    VUser VUser,
    VCharacter? VCharacter
) {

    public static VPlayer from(Entity userEntity) {

        var vUser = VUser.from(userEntity);
        if (VWorld.Server.EntityManager.GetComponentData<User>(userEntity).LocalCharacter._Entity == Entity.Null) {
            return new VPlayer(
                VUser: vUser,
                VCharacter: null
            );
        }

        return new VPlayer(
            VUser: vUser,
            VCharacter: game.VCharacter.from(vUser)
        );
    }

    public static List<VPlayer> GetAllPlayers() {

        var players = VWorld.Server.EntityManager
            .CreateEntityQuery(ComponentType.ReadOnly<User>())
            .ToEntityArray(Allocator.Temp);

        var vPlayers = new List<VPlayer>();
        foreach (var player in players) {
            vPlayers.Add(from(player));
        }

        players.Dispose();
        return vPlayers;
    }

    public ulong GetId() {
        return VUser.User.PlatformId;
    }

    public bool HasCharacter() {
        return VCharacter != null;
    }
}
