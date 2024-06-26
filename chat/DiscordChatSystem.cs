using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bloodstone.Hooks;
using ProjectM.Network;

namespace v_rising_discord_bot_companion.chat;

public class DiscordChatSystem {

    private static readonly int MAX_MESSAGE_LENGTH = 2000;

    private static readonly HttpClient _httpClient = new();
    private static readonly JsonSerializerOptions _serializeOptions = new() {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public static void HandleChatEvent(VChatEvent vChatEvent) {

        var pluginConfig = Plugin.Instance.GetPluginConfig();
        var discordWebhookUrl = pluginConfig.DiscordWebhookUrl;
        if (discordWebhookUrl == null || vChatEvent.Type != ChatMessageType.Global) {
            return;
        }

        try {

            var webhookRequestPayload = new DiscordWebhookRequestPayload(
                Username: pluginConfig.DiscordWebhookUsername,
                AvatarUrl: pluginConfig.DiscordWebhookAvatarUrl,
                Content: $"{vChatEvent.User.CharacterName}: {vChatEvent.Message.Substring(0, Math.Min(vChatEvent.Message.Length, MAX_MESSAGE_LENGTH - vChatEvent.User.CharacterName.Length))}"
            );

            var webhookRequest = new HttpRequestMessage(HttpMethod.Post, discordWebhookUrl) {
                Content = new StringContent(
                    JsonSerializer.Serialize(webhookRequestPayload, _serializeOptions),
                    Encoding.UTF8,
                    "application/json"
                )
            };

            var response = _httpClient.Send(webhookRequest);
            if (response.StatusCode != HttpStatusCode.NoContent) {
                Plugin.Logger.LogError($"Discord webhook responded with unexpected status code '{response.StatusCode}' - please check your configuration");
            }
        } catch (Exception e) {
            Plugin.Logger.LogError($"Exception calling discord webhook: {e.Message}");
        }
    }
}
