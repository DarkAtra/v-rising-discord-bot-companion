using System;
using System.Collections.Generic;
using System.Linq;
using ProjectM;
using Unity.Entities;

namespace v_rising_server_mod_test;

public class GetPlayerScoreCommand {

    public static string HandleGetPlayerScore(string command, List<string> args) {

        if (args.Count != 1) {
            throw new ArgumentException("Error: this command expects exactly one argument <name>");
        }

        var characterName = args[0];
        var characterEntity = VWorld.GetAllPlayerCharacters().FirstOrDefault(character =>
                                  VWorld.Server.EntityManager.GetComponentData<PlayerCharacter>(character).Name.ToString() == characterName
                              ) as Entity?
                              ?? throw new ArgumentException($"Error: no character with name '{characterName}' exists");

        try {
            var equipment = VWorld.Server.EntityManager.GetComponentData<Equipment>(characterEntity);
            return $"{equipment.GetFullLevel()}";
        }
        catch (ArgumentException) { // if the entity has no armor equipped
            return "0";
        }
    }
}
