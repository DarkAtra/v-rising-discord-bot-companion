using System.Collections.Generic;

namespace v_rising_discord_bot_companion;

public readonly record struct PluginConfig(
    List<BasicAuthUser> BasicAuthUsers,
    string? DiscordWebhookUrl,
    string DiscordWebhookUsername,
    string DiscordWebhookAvatarUrl
);

public readonly record struct BasicAuthUser(
    string Username,
    string Password
);
