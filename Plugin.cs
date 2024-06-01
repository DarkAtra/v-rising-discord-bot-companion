using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Bloodstone.API;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using v_rising_discord_bot_companion.query;

namespace v_rising_discord_bot_companion;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.Bloodstone")]
[Reloadable]
public class Plugin : BasePlugin {

    public static ManualLogSource Logger { get; private set; } = null!;
    public static Plugin Instance { get; private set; } = null!;
    private Harmony? _harmony;
    private Component? _queryDispatcher;

    private PluginConfig? _pluginConfig;
    private ConfigEntry<string> _basicAuthUsers;

    public Plugin() {

        Instance = this;
        Logger = Log;

        _basicAuthUsers = Config.Bind(
            "Authentication",
            "BasicAuthUsers",
            "",
            "A list of comma separated username:password entries that are allowed to query the HTTP API."
        );
    }

    public override void Load() {

        if (!VWorld.IsServer) {
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} must be installed on the server side.");
            return;
        }


        // Plugin startup logic
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

        // inject components
        ClassInjector.RegisterTypeInIl2Cpp<QueryDispatcher>();
        _queryDispatcher = AddComponent<QueryDispatcher>();

        // Harmony patching
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public override bool Unload() {
        _harmony?.UnpatchSelf();
        if (_queryDispatcher != null) {
            Object.Destroy(_queryDispatcher);
        }
        _pluginConfig = null;
        return true;
    }

    public PluginConfig GetPluginConfig() {
        _pluginConfig ??= ParsePluginConfig();
        return (PluginConfig) _pluginConfig;
    }

    private PluginConfig ParsePluginConfig() {

        var basicAuthUsers = new List<BasicAuthUser>();
        foreach (var basicAuthUser in _basicAuthUsers.Value.Split(",")) {
            var parts = basicAuthUser.Split(":", 2);
            if (parts.Length == 2) {
                basicAuthUsers.Add(
                    new BasicAuthUser(
                        Username: parts[0].Trim(),
                        Password: parts[1].Trim()
                    )
                );
            }
        }

        return new PluginConfig(
            BasicAuthUsers: basicAuthUsers
        );
    }
}
