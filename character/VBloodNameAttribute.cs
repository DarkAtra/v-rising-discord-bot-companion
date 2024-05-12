using System;

namespace v_rising_discord_bot_companion.character;

public class VBloodNameAttribute : Attribute {

    public String DisplayName { get; }

    public VBloodNameAttribute(string displayName) {
        DisplayName = displayName;
    }
}
