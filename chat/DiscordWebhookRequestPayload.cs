namespace v_rising_discord_bot_companion.chat;

public readonly record struct DiscordWebhookRequestPayload(
    string Username,
    string Content,
    string AvatarUrl
);
