using System;

namespace v_rising_discord_bot_companion.raid;

public readonly record struct Player(
    ulong Id,
    string Name,
    int GearLevel,
    string? Clan,
    DateTime JoinedAt
);
