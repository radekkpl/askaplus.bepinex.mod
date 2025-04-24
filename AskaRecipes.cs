using HarmonyLib;
using SandSailorStudio.Inventory;
using SSSGame;
using System.Linq;
using static askaplus.bepinex.mod.Plugin;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class AskaRecipes
    {
        private static bool patched = false;
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MainMenu.OnActivate))]
        public static void PostAwake(MainMenu __instance)
        {
            if (patched) return;
            patched = true;


            var bpinfos = UnityEngine.Resources.FindObjectsOfTypeAll<BlueprintInfo>().ToDictionary(name => name.name, bp =>  bp);
            var cbpinfos = UnityEngine.Resources.FindObjectsOfTypeAll<CraftBlueprintInfo>().ToDictionary(name => name.name, bp => bp);

            //Plugin.Log.LogMessage("BLUEPRINT INFOS");

            //foreach (var bp in bpinfos)
            //{
            //    Plugin.Log.LogMessage(bp.Key);
            //}
            //Plugin.Log.LogMessage("CRAFT BLUEPRINT INFOS");

            //foreach (var cbp in cbpinfos)
            //{
            //    Plugin.Log.LogMessage(cbp.Key);
            //}
        }

        
    }


    //[HarmonyPatch(typeof(CraftingStation))]
    //internal class CraftingStationFix 
    //{
    //    [HarmonyPostfix]
    //    [HarmonyPatch(nameof(CraftingStation.GetMinimumFetchManifest))]
    //    public static void GetMinimumFetchManifestPostFix(CraftingStation __instance) 
    //    {
    //        if (__instance.name.StartsWith("Workshop_L2("))
    //        {
    //            Plugin.Log.LogInfo($"Crafting station {__instance.name} called GetMinimumFetchManifest");
    //            var items = __instance._minimumFetchManifest._items;

    //            if (items.ContainsKey(Helpers.itemInfoSO["Item_Wood_HardWoodLog"])) 
    //            {
    //                Plugin.Log.LogInfo($"Contains {items[Helpers.itemInfoSO["Item_Wood_HardWoodLog"]]} items of Item_Wood_HardWoodLog");
    //                items[Helpers.itemInfoSO["Item_Wood_HardWoodLog"]] = 9;
    //            }
    //            else
    //            {
    //                items.Add(Helpers.itemInfoSO["Item_Wood_HardWoodLog"], 9);
    //            }

    //        }
    //    }
    
    //}


}
