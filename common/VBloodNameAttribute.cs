using System;

namespace v_rising_discord_bot_companion.common;

[AttributeUsage(AttributeTargets.Field)]
public class VBloodNameAttribute(
    string displayName
) : Attribute {

    public string DisplayName { get; } = displayName;
}
