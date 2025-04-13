using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;

namespace askaplus.bepinex.mod
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        internal static new ManualLogSource Log;

        public override void Load()
        {
            // Plugin startup logic
            Log = base.Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            ClassInjector.RegisterTypeInIl2Cpp<VillagerBonusSpawn>();
            Harmony.CreateAndPatchAll(typeof(SpikesSelfDamageMod));
            Harmony.CreateAndPatchAll(typeof(VillagerPatch));
        }
    }
}
