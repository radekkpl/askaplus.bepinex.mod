using HarmonyLib;
using Il2CppSystem.Linq;
using LibCpp2IL.Elf;
using SandSailorStudio.Inventory;
using SSSGame;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static askaplus.bepinex.mod.Plugin;


namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(ItemInfo))]
    internal class ItemInfoPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ItemInfo.Configure))]
        public static void ItemInfoConfigurePreFix(ref ItemInfo __instance)
        {
            //Plugin.Log.LogInfo($"PlantableItemInfoConfigurePostFix - {__instance.name}");
            if (configSeedsDecayEnable.Value && __instance.name.Contains("Seed") && __instance.TryCast<PlantableItemInfo>() == true)
            {
                var pi = __instance.Cast<PlantableItemInfo>();

                if (pi.MaxOffseasonDays < pi.TimeToGrow)
                {
                    //TO BE ABLE GROW PLANTS FULLY AT THE END OF THE SEASON
                    Plugin.Log.LogDebug($"Fixing MaxOffSeasonDays to allow fully grow plant at the end of the season: {__instance.name}");
                    Plugin.Log.LogDebug($"MaxOffSeasonDays: {pi.MaxOffseasonDays}, TimeToGrow: {pi.timeToGrow}");
                    pi.MaxOffseasonDays = pi.TimeToGrow + 1;
                    Plugin.Log.LogDebug($"New value of MaxOffSeasonDays: {pi.MaxOffseasonDays}");
                }
                else 
                {
                    Plugin.Log.LogDebug($"{__instance.name}: MaxOffSeasonDays: {pi.MaxOffseasonDays} Time to grow: {pi.timeToGrow}");
                }

                //var decayAttributes = pi.attributes.Where(at => at.attribute.attributeId == 1011).Select(at => at).ToArray();
                //if (decayAttributes.Length != 1)
                //{
                //    Plugin.Log.LogError($"Decay attribute (id 1011) not found at object {__instance.name}");
                //    return;
                //}
                //Plugin.Log.LogDebug($"Trying to change decay rate of {__instance.name} from value {decayAttributes[0].value} to {.07f}");

                //QUICKEST DECAY OF SEEDS = NO MORE WASTE EVERYWHERE
                //var attribute = decayAttributes[0].attribute;

                //                __instance.attributes[0].value
            }

            if (configFoodEnable.Value && __instance.name.Contains("_Food_"))
            {
                //   Plugin.Log.LogInfo($"Found {__instance.name}:");
                //FOOD PATCH
                if (__instance.TryCast<ConsumableInfo>() == true)
                {
                    var food = __instance.Cast<ConsumableInfo>();

                    int[] attributes = [10, 14, 15, 11, 12, 13];

                    foreach (var ce in food.consumeEffects)
                    {
                        if (ce.duration > 0 && ce.table?.attrElements?.Count > 0)
                        {
                            foreach (var ae in ce.table.attrElements)
                            {
                                if (ae.modifier?.Operation == SandSailorStudio.Attributes.ModifierOperation.PERCENTADD || ae.modifier?.Operation == SandSailorStudio.Attributes.ModifierOperation.ADD)
                                {
                                    if (attributes.Contains(ae.targetAttribute.attributeId))
                                    {
                                        Plugin.Log.LogDebug($"Patching {food.name}: {ae.targetAttribute.name} - {ae.modifier.Operation} - {ae.modifier.Value} : Duration from {ce.duration} to {5 * 60}");
                                        ce.duration = 5 * 60;
                                    }
                                }
                            }
                        }
                    }


                    foreach (var ce in food.modulatedConsumeEffects)
                    {
                        if (ce.normalizedRange.min == 0)
                        {
                            foreach (var se in ce.randomStatusEffects)
                            {
                                if (se.duration > 0 && se.table?.attrElements.Count > 0)
                                {
                                    foreach (var ae in se.table.attrElements)
                                    {
                                        if (ae.modifier?.Operation == SandSailorStudio.Attributes.ModifierOperation.PERCENTADD || ae.modifier?.Operation == SandSailorStudio.Attributes.ModifierOperation.ADD)
                                        {
                                            if (attributes.Contains(ae.targetAttribute.attributeId))
                                            {
                                                Plugin.Log.LogDebug($"Patching {food.name}: {ae.targetAttribute.name} - {ae.modifier.Operation} - {ae.modifier.Value} : Duration from {se.duration} to {5 * 60}");
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
        public static void OnSettingsMenu(Transform parent)
        {
            Helpers.CreateCategory(parent, "Seeds mod");
            Helpers.CreateSwitch(parent, "* Increase decay rate of seeds.", configSeedsDecayEnable);
            Helpers.CreateCategory(parent, "Food mod");
            Helpers.CreateSwitch(parent, "* Increase duration of food effects", configFoodEnable);

            UnityAction applyCallback = (UnityAction)(() =>
            {
                Plugin.configGrassPaintKey.Value = KeyCode.Z;
            });
        }
    }

}
