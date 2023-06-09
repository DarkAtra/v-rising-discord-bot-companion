using ProjectM;
using ProjectM.Network;
using Unity.Entities;

namespace v_rising_discord_bot_companion.game;

public readonly record struct VPlayer(
    VUser VUser,
    VCharacter? VCharacter
) {

    public static VPlayer from(Entity userEntity) {

        var vUser = VUser.from(userEntity);
        if (VWorld.EntityManager.GetComponentData<User>(userEntity).LocalCharacter._Entity == Entity.Null) {
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

    public bool HasCharacter() {
        return VCharacter != null;
    }
}

public readonly record struct VUser(
    User User,
    Entity UserEntity
) {

    public static VUser from(Entity userEntity) {
        var user = VWorld.EntityManager.GetComponentData<User>(userEntity);
        return new VUser(
            User: user,
            UserEntity: userEntity
        );
    }
}

public readonly record struct VCharacter(
    PlayerCharacter Character,
    Entity CharacterEntity
) {

    public static VCharacter from(VUser vUser) {
        var characterEntity = vUser.User.LocalCharacter._Entity;
        return new VCharacter(
            Character: VWorld.EntityManager.GetComponentData<PlayerCharacter>(characterEntity),
            CharacterEntity: characterEntity
        );
    }
}
