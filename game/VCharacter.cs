using System;
using ProjectM;
using Unity.Entities;

namespace v_rising_discord_bot_companion.game;

public readonly record struct VCharacter(
    PlayerCharacter Character,
    Entity CharacterEntity
) {

    public static VCharacter from(VUser vUser) {
        return from(vUser.User.LocalCharacter._Entity);
    }

    public static VCharacter from(Entity characterEntity) {
        return new VCharacter(
            Character: VWorld.Server.EntityManager.GetComponentData<PlayerCharacter>(characterEntity),
            CharacterEntity: characterEntity
        );
    }

    public int getGearLevel() {
        return Convert.ToInt32(VWorld.Server.EntityManager.GetComponentData<Equipment>(CharacterEntity).GetFullLevel());
    }
}
