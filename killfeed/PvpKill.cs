using System;

namespace v_rising_discord_bot_companion.killfeed;

public readonly record struct PvpKill(
    Player Killer,
    Player Victim,
    DateTime Occurred
);

public readonly record struct Player(
    string Name,
    int GearLevel
);
