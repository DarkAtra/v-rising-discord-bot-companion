using ProjectM;
using ProjectM.Network;
using Unity.Entities;

namespace v_rising_discord_bot_companion.game;

public readonly record struct VPlayer(
    User User,
    Entity UserEntity,
    PlayerCharacter Character,
    Entity CharacterEntity
);
