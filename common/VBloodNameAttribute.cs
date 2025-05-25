using System;

namespace v_rising_discord_bot_companion.common;

public class VBloodNameAttribute : Attribute {

    public String DisplayName { get; }

    public VBloodNameAttribute(string displayName) {
        DisplayName = displayName;
    }
}
