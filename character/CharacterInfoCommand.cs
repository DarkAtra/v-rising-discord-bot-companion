using System.Collections.Generic;
using System.Linq;
using ProjectM;
using v_rising_discord_bot_companion.game;

namespace v_rising_discord_bot_companion.character;

public static class CharacterInfoCommand {

    public static List<CharacterResponse> GetCharacters() {

        return VWorld.GetAllPlayers()
            .Where(vPlayer => vPlayer.HasCharacter())
            .Select(player => {

            var clanEntity = player.VUser.User.ClanEntity._Entity;
            string? clan = null;
            if (VWorld.EntityManager.HasComponent<ClanTeam>(clanEntity)) {
                clan = VWorld.EntityManager.GetComponentData<ClanTeam>(clanEntity).Name.ToString();
            }

            var killedVBloods = new List<VBlood>();
            var hasProgression = ProgressionUtility.TryGetProgressionEntity(VWorld.EntityManager, player.VUser.UserEntity, out var progressionEntity);
            if (hasProgression) {
                ListUtils.Convert(VWorld.EntityManager.GetBuffer<UnlockedVBlood>(progressionEntity))
                    .ForEach(vBlood => killedVBloods.Add((VBlood) vBlood.VBlood.GuidHash));
            }

            return new CharacterResponse(
                Name: ((VCharacter) player.VCharacter!).Character.Name.ToString(),
                GearLevel: (int) VWorld.EntityManager.GetComponentData<Equipment>(((VCharacter) player.VCharacter!).CharacterEntity).GetFullLevel(),
                Clan: clan,
                KilledVBloods: killedVBloods
            );
        }).ToList();
    }
}
