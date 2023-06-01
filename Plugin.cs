using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using VampireCommandFramework;
using VampireCommandFramework.Breadstone;

namespace v_rising_server_mod_test;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[Reloadable]
public class Plugin : BasePlugin {

    public static ManualLogSource Logger = null!;
    private Harmony? _harmony;
    private Component? _rconCommandDispatcherComponent;

    public override void Load() {

        if (!VWorld.IsServer) {
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} must be installed on the server side.");
            return;
        }

        Logger = Log;

        // Plugin startup logic
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

        // inject components
        ClassInjector.RegisterTypeInIl2Cpp<RconCommandDispatcher>();
        _rconCommandDispatcherComponent = AddComponent<RconCommandDispatcher>();

        // Harmony patching
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(Assembly.GetExecutingAssembly());

        // Register all commands in the assembly with VCF
        CommandRegistry.RegisterAll();
    }

    public override bool Unload() {
        CommandRegistry.UnregisterAssembly();
        _harmony?.UnpatchSelf();
        if (_rconCommandDispatcherComponent != null) {
            Object.Destroy(_rconCommandDispatcherComponent);
        }
        return true;
    }
}
