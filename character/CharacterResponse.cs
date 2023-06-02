namespace v_rising_discord_bot_companion.character;

public class CharacterResponse {

    public CharacterResponse(string name, int gearLevel) {
        Name = name;
        GearLevel = gearLevel;
    }

    public string Name { get; private set; }
    public int GearLevel { get; private set; }
}
