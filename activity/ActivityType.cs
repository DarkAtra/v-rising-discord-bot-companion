using System.Text.Json.Serialization;

namespace v_rising_discord_bot_companion.activity;

[JsonConverter(typeof(ActivityTypeConverter))]
public enum ActivityType {
    CONNECTED,
    DISCONNECTED
}
