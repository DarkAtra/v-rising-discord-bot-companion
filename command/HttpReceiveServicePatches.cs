using System.Linq;
using HarmonyLib;
using Il2CppSystem.Net;
using Il2CppSystem.Security.Principal;
using ProjectM.Network;

namespace v_rising_discord_bot_companion.command;

[HarmonyPatch(typeof(HttpServiceReceiveThread))]
public class HttpReceiveServicePatches {

    [HarmonyPrefix]
    [HarmonyPatch("IsAllowed")]
    public static bool IsAllowed(HttpListenerContext context, ref bool __result) {

        var pluginConfig = Plugin.Instance.GetPluginConfig();

        if (pluginConfig.BasicAuthUsers.Count <= 0) {
            return true;
        }

        var currentBasicAuthUser = ParseBasicAuthUser(context);
        if (!currentBasicAuthUser.HasValue) {
            __result = false;
            return false;
        }

        __result = IsAuthorized((BasicAuthUser) currentBasicAuthUser);
        return false;
    }

    private static BasicAuthUser? ParseBasicAuthUser(HttpListenerContext context) {

        context.ParseAuthentication(AuthenticationSchemes.Basic);

        if (context.user == null) {
            return null;
        }

        var principal = context.user.TryCast<GenericPrincipal>();
        var identity = principal?.m_identity.TryCast<HttpListenerBasicIdentity>();

        if (identity == null) {
            return null;
        }

        var username = identity.Name;
        var password = identity.password;

        if (username == null || password == null) {
            return null;
        }

        return new BasicAuthUser(
            Username: username,
            Password: password
        );
    }

    private static bool IsAuthorized(BasicAuthUser currentBasicAuthUser) {
        return Plugin.Instance.GetPluginConfig().BasicAuthUsers
            .Count(it => it.Username.Equals(currentBasicAuthUser.Username) && it.Password.Equals(currentBasicAuthUser.Password)) == 1;
    }
}
