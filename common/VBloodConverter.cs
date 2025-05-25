using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace v_rising_discord_bot_companion.common;

public class VBloodConverter : JsonConverter<VBlood> {

    public override VBlood Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, VBlood value, JsonSerializerOptions options) {
        writer.WriteStringValue(Enum.GetName(typeof(VBlood), value));
    }
}
