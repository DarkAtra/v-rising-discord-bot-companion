using System;
using System.Collections.Generic;
using v_rising_discord_bot_companion.common;

namespace v_rising_discord_bot_companion.killfeed;

public readonly record struct VBloodKill(
    List<Player> Killers,
    VBlood VBlood,
    DateTime Occurred
);
