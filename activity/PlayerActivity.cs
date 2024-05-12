using System;

namespace v_rising_discord_bot_companion.activity;

public readonly record struct PlayerActivity(
    ActivityType Type,
    string PlayerName,
    DateTime Occurred
);
