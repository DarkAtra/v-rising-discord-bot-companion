using System;
using System.Linq;
using ProjectM;
using Unity.Entities;

namespace v_rising_server_mod_test.gearlevel;

public static class CharacterGearLevelCommand {

    public static CharacterGearLevelResponse HandleGetPlayerScore(string characterName) {

        var characterEntity = VWorld.GetAllPlayerCharacters().FirstOrDefault(character =>
                                  VWorld.Server.EntityManager.GetComponentData<PlayerCharacter>(character).Name.ToString() == characterName
                              ) as Entity?
                              ?? throw new ArgumentException($"Error: no character with name '{characterName}' exists");

        try {
            var equipment = VWorld.Server.EntityManager.GetComponentData<Equipment>(characterEntity);
            return new CharacterGearLevelResponse(characterName, equipment.GetFullLevel());
        }
        catch (ArgumentException) { // if the entity has no armor equipped
            return new CharacterGearLevelResponse(characterName, 0);
        }
    }
}
