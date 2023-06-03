using System.Collections.Generic;
using System.Linq;
using ProjectM;

namespace v_rising_discord_bot_companion.character;

public static class CharacterInfoCommand {

    public static List<CharacterResponse> GetCharacters() {

        return VWorld.GetAllPlayerCharacters().Select(player => {

            var clanEntity = player.User.ClanEntity._Entity;
            string? clan = null;
            if (VWorld.EntityManager.HasComponent<ClanTeam>(clanEntity)) {
                clan = VWorld.EntityManager.GetComponentData<ClanTeam>(clanEntity).Name.ToString();
            }

            var killedVBloods = new List<VBlood>();
            var hasProgression = ProgressionUtility.TryGetProgressionEntity(VWorld.EntityManager, player.UserEntity, out var progressionEntity);
            if (hasProgression) {
                ListUtils.Convert(VWorld.EntityManager.GetBuffer<UnlockedVBlood>(progressionEntity))
                    .ForEach(vBlood => killedVBloods.Add((VBlood) vBlood.VBlood.GuidHash));
            }

            return new CharacterResponse(
                Name: player.Character.Name.ToString(),
                GearLevel: (int) VWorld.EntityManager.GetComponentData<Equipment>(player.CharacterEntity).GetFullLevel(),
                Clan: clan,
                KilledVBloods: killedVBloods
            );
        }).ToList();
    }
}
