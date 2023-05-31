using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using VampireCommandFramework;
using VampireCommandFramework.Breadstone;

namespace v_rising_server_mod_test;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[Reloadable]
public class Plugin : BasePlugin {

    public static ManualLogSource Logger = null!;
    private Harmony? _harmony;

    public override void Load() {

        if (!VWorld.IsServer) {
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} must be installed on the server side.");
            return;
        }

        Logger = Log;

        // Plugin startup logic
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

        // Harmony patching
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(Assembly.GetExecutingAssembly());

        // Register all commands in the assembly with VCF
        CommandRegistry.RegisterAll();
    }

    public override bool Unload() {
        CommandRegistry.UnregisterAssembly();
        _harmony?.UnpatchSelf();
        return true;
    }

    /// <summary> 
    /// Example VCF command that demonstrated default values and primitive types
    /// Visit https://github.com/decaprime/VampireCommandFramework for more info 
    /// </summary>
    /// <remarks>
    /// How you could call this command from chat:
    ///
    /// .v_rising_server_mod_test-example "some quoted string" 1 1.5
    /// .v_rising_server_mod_test-example boop 21232
    /// .v_rising_server_mod_test-example boop-boop
    ///</remarks>
    [Command(
        name: "v_rising_server_mod_test-example",
        description: "Example command from v_rising_server_mod_test",
        adminOnly: true
    )]
    public void ExampleCommand(ICommandContext ctx, string someString, int num = 5, float num2 = 1.5f) {
        ctx.Reply($"You passed in {someString} and {num} and {num2}");

        VWorld.GetAllOnlinePlayerCharacters().ForEach(character => Logger.LogInfo($"Char: {character.Name.ToString()}"));
    }
}
