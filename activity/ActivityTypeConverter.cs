using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace v_rising_discord_bot_companion.activity;

public class ActivityTypeConverter : JsonConverter<ActivityType> {

    public override ActivityType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, ActivityType value, JsonSerializerOptions options) {
        writer.WriteStringValue(Enum.GetName(typeof(ActivityType), value));
    }
}
