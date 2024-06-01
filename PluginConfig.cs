using System.Collections.Generic;

namespace v_rising_discord_bot_companion;

public readonly record struct PluginConfig(
    List<BasicAuthUser> BasicAuthUsers
);

public readonly record struct BasicAuthUser(
    string Username,
    string Password
);
