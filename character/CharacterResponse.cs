using System.Collections.Generic;
using v_rising_discord_bot_companion.common;

namespace v_rising_discord_bot_companion.character;

public readonly record struct CharacterResponse(
    string Name,
    int GearLevel,
    string? Clan,
    List<VBlood> KilledVBloods
);
