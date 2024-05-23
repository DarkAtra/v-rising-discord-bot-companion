using System.Collections.Generic;
using System.Linq;
using Bloodstone.API;
using ProjectM;
using v_rising_discord_bot_companion.game;

namespace v_rising_discord_bot_companion.character;

public static class CharacterInfoCommand {

    public static List<CharacterResponse> GetCharacters() {

        return VPlayer.GetAllPlayers()
            .Where(vPlayer => vPlayer.HasCharacter())
            .Select(player => {

                var clanEntity = player.VUser.User.ClanEntity._Entity;
                string? clan = null;
                var entityManager = VWorld.Server.EntityManager;
                if (entityManager.HasComponent<ClanTeam>(clanEntity)) {
                    clan = entityManager.GetComponentData<ClanTeam>(clanEntity).Name.ToString();
                }

                var killedVBloods = new List<VBlood>();
                var hasProgression = ProgressionUtility.TryGetProgressionEntity(entityManager, player.VUser.UserEntity, out var progressionEntity);
                if (hasProgression) {
                    foreach (var unlockedVBlood in entityManager.GetBuffer<UnlockedVBlood>(progressionEntity)) {
                        VBlood? vBlood = (VBlood?) unlockedVBlood.VBlood.GuidHash;
                        if (vBlood != null) {
                            killedVBloods.Add((VBlood) vBlood);
                        }
                    }
                }

                return new CharacterResponse(
                    Name: ((VCharacter) player.VCharacter!).Character.Name.ToString(),
                    GearLevel: ((VCharacter) player.VCharacter!).getGearLevel(),
                    Clan: clan,
                    KilledVBloods: killedVBloods
                );
            }).ToList();
    }
}
