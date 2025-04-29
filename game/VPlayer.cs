using System.Collections.Generic;
using System.Linq;
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
        return ListUtils.Convert(
                VWorld.Server.EntityManager
                    .CreateEntityQuery(ComponentType.ReadOnly<User>())
                    .ToEntityArray(Allocator.Temp)
            )
            .Select(from)
            .ToList();
    }

    public bool HasCharacter() {
        return VCharacter != null;
    }
}
