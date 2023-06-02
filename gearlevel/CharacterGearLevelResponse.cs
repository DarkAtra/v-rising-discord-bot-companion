namespace v_rising_server_mod_test.gearlevel;

public class CharacterGearLevelResponse {
    public string Name { get; private set; }
    public float GearLevel { get; private set; }

    public CharacterGearLevelResponse(string name, float gearLevel) {
        Name = name;
        GearLevel = gearLevel;
    }
}
