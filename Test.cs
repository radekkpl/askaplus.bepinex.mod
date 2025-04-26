using AsmResolver.Patching;
using HarmonyLib;
using SandSailorStudio.UI;
using SSSGame;


namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(HarvestInteraction))]
    internal class Test
    {        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(HarvestInteraction.add_OnHarvestDamageTaken))]
        public static void add_OnHarvestDamageTakenPostfix(HarvestInteraction __instance)
        {
            Plugin.Log.LogInfo($"Add_OnHarvestDamageTaken running just now on {__instance.gameObject.transform.parent?.name}");
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(HarvestInteraction.remove_OnHarvestDamageTaken))]
        public static void remove_OnHarvestDamageTakenPostfix(HarvestInteraction __instance)
        {
            Plugin.Log.LogInfo($"Remove_OnHarvestDamageTaken running just now on {__instance.gameObject.transform.parent?.name}");
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(HarvestInteraction.add_OnFullyHarvested))]
        public static void add_OnFullyHarvestedPostfix(HarvestInteraction __instance)
        {
            Plugin.Log.LogInfo($"Add_OnFullyHarvested running just now on {__instance.gameObject.transform.parent?.name}");
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(HarvestInteraction.remove_OnFullyHarvested))]
        public static void remove_OnFullyHarvestedPostfix(HarvestInteraction __instance)
        {
            Plugin.Log.LogInfo($"Remove_OnFullyHarvested running just now on {__instance.gameObject.transform.parent?.name}");
        }

    }
}
