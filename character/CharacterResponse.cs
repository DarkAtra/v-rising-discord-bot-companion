namespace v_rising_server_mod_test.character;

public class CharacterResponse {
    public string Name { get; private set; }
    public int GearLevel { get; private set; }

    public CharacterResponse(string name, int gearLevel) {
        Name = name;
        GearLevel = gearLevel;
    }
}
