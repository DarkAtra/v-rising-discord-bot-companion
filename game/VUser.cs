using ProjectM;
using ProjectM.Network;
using Unity.Entities;

namespace v_rising_discord_bot_companion.game;

public readonly record struct VUser(
    User User,
    Entity UserEntity
) {

    public static VUser from(Entity userEntity) {
        var user = VWorld.Server.EntityManager.GetComponentData<User>(userEntity);
        return new VUser(
            User: user,
            UserEntity: userEntity
        );
    }

    public string? GetClanName() {
        var clanEntity = User.ClanEntity._Entity;
        string? clan = null;
        if (VWorld.Server.EntityManager.HasComponent<ClanTeam>(clanEntity)) {
            clan = VWorld.Server.EntityManager.GetComponentData<ClanTeam>(clanEntity).Name.ToString();
        }
        return clan;
    }
}
