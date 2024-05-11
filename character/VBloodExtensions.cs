using System.Reflection;

namespace v_rising_discord_bot_companion.character;

public static class VBloodExtensions {
    public static string DisplayName(this VBlood vBlood) {
        return vBlood.GetType().GetField(vBlood.ToString())!
            .GetCustomAttribute<VBloodNameAttribute>(false)!
            .DisplayName;
    }
}
