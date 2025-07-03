using System;
using System.Collections.Generic;

namespace v_rising_discord_bot_companion.raid;

public record struct Raid(
    Guid Id,
    List<Player> Attackers,
    List<Player> Defenders,
    DateTime Occurred,
    DateTime Updated
);
