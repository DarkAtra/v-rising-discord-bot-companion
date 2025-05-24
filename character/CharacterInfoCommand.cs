using System;
using System.Collections.Generic;
using System.Linq;
using ProjectM;
using v_rising_discord_bot_companion.game;

namespace v_rising_discord_bot_companion.character;

public static class CharacterInfoCommand {

    public static List<CharacterResponse> GetCharacters() {

        return VPlayer.GetAllPlayers()
            .Where(vPlayer => vPlayer.HasCharacter())
            .Select(player => {

                var killedVBloods = new List<VBlood>();
                var hasProgression = ProgressionUtility.TryGetProgressionEntity(VWorld.Server.EntityManager, player.VUser.UserEntity, out var progressionEntity);
                if (hasProgression) {
                    foreach (var unlockedVBlood in VWorld.Server.EntityManager.GetBuffer<UnlockedVBlood>(progressionEntity)) {
                        if (Enum.IsDefined(typeof(VBlood), unlockedVBlood.VBlood.GuidHash)) {
                            killedVBloods.Add((VBlood) unlockedVBlood.VBlood.GuidHash);
                        }
                    }
                }

                return new CharacterResponse(
                    Name: ((VCharacter) player.VCharacter!).Character.Name.ToString(),
                    GearLevel: ((VCharacter) player.VCharacter!).getGearLevel(),
                    Clan: player.VUser.GetClanName(),
                    KilledVBloods: killedVBloods
                );
            }).ToList();
    }
}
