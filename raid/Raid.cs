using System;
using System.Collections.Generic;

namespace v_rising_discord_bot_companion.raid;

public readonly record struct Raid(
    List<Player> Attackers,
    List<Player> Defenders,
    DateTime Occurred
);
