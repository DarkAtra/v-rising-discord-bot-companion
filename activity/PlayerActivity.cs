using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace v_rising_discord_bot_companion.activity;

public readonly record struct PlayerActivity(
    ActivityType Type,
    string PlayerName,
    DateTime Occurred
);

[JsonConverter(typeof(ActivityTypeConverter))]
public enum ActivityType {
    CONNECTED,
    DISCONNECTED
}

public class ActivityTypeConverter : JsonConverter<ActivityType> {

    public override ActivityType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, ActivityType value, JsonSerializerOptions options) {
        writer.WriteStringValue(Enum.GetName(typeof(ActivityType), value));
    }
}
