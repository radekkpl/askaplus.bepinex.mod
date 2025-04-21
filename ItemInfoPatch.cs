using HarmonyLib;
using SandSailorStudio.Inventory;
using SSSGame;
using System.Linq;
using UnityEngine;


namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(ItemInfo))]
    internal class ItemInfoPatch
    {
        private static bool patched = false;
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ItemInfo.Configure))]
        public static void ItemInfoConfigurePreFix(ref ItemInfo __instance)
        {
            //Plugin.Log.LogInfo($"PlantableItemInfoConfigurePostFix - {__instance.name}");
            if (__instance.name.Contains("Seed"))
            {
                if (__instance.TryCast<PlantableItemInfo>() == true)
                {
                    var pi = __instance.Cast<PlantableItemInfo>();

                    //TO BE ABLE GROW PLANTS FULLY AT THE END OF THE SEASON CHANGE OF SEASON
                    pi.MaxOffseasonDays = pi.TimeToGrow;
                    var decayAttributes = pi.attributes.Where(at => at.attribute.attributeId == 1011).Select(at => at).ToArray();
                    if (decayAttributes.Length != 1)
                    {
                        Plugin.Log.LogError($"Decay attribute (id 1011) not found at object {__instance.name}");
                        return;
                    }
                    Plugin.Log.LogInfo($"Trying to change decay rate of {__instance.name} from value {decayAttributes[0].value} to {.1f}");
                    
                    //QUICKEST DECAY OF SEEDS = NO MORE WASTE EVERYWHERE
                    decayAttributes[0].value = 0.1f;
                }
            }
            else if (__instance.name.Contains("_Food_"))
            {
                //FOOD PATCH
                if (__instance.TryCast<ConsumableInfo>() == true) {
                    var food = __instance.Cast<ConsumableInfo>();

                    int[] attributes = [10, 14,15,11,12,13,]; 

                    foreach (var ce in food.modulatedConsumeEffects)
                    {
                        if (ce.normalizedRange.min == 0)
                        {
                            foreach (var se in food.consumeEffects)
                            {
                                if (se.duration > 0 && se.table?.attrElements.Count > 0)
                                {
                                    foreach (var ae in se.table.attrElements)
                                    {
                                        if (ae.modifier?.Operation == SandSailorStudio.Attributes.ModifierOperation.PERCENTADD || ae.modifier?.Operation == SandSailorStudio.Attributes.ModifierOperation.ADD)
                                        {
                                            if (attributes.Contains(ae.targetAttribute.attributeId))
                                            {
                                                Plugin.Log.LogInfo($"Patching {food.name}: {ae.targetAttribute.name} - {ae.modifier.Operation} - {ae.modifier.Value} : Duration from {se.duration} to {5*60}");
                                                se.duration = 5 * 60;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                 }

            }
        }
    }

}
