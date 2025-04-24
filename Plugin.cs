﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using SandSailorStudio.Inventory;
using SandSailorStudio.UI;
using SSSGame;
using SSSGame.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace askaplus.bepinex.mod
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        internal static new ManualLogSource Log;
        internal static ConfigEntry<bool> configGrassPaintEnable;
        internal static ConfigEntry<KeyCode> configGrassPaintKey;
        internal static ConfigEntry<bool> configSpikesSelfDamageEnable;
        internal static ConfigEntry<bool> configBonusSpawnEnable;
        internal static ConfigEntry<bool> configTorchesBuildingEnable;
        internal static ConfigEntry<bool> configTorchesBuildingShadowsEnable;
        internal static ConfigEntry<bool> configSeedsDecayEnable;
        internal static ConfigEntry<bool> configFoodEnable;

        public override void Load()
        {

            // Plugin startup logic
            Log = base.Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            configGrassPaintEnable = Config.Bind("GrassPaintig", "Enable mod", true, "Enable or disable mod");
            configGrassPaintKey = Config.Bind("GrassPainting", "KeyCode", KeyCode.RightBracket, "Key to paint grass");
            configSpikesSelfDamageEnable = Config.Bind("Spikes selfdamage", "Enable mod", true, "Enable or disable mod");
            configBonusSpawnEnable = Config.Bind("Bonus spawn", "Enable mod", true, "Enable or disable mod");
            configTorchesBuildingEnable = Config.Bind("Torches to buildings", "Enable mod", true, "Enable or disable mod");
            configTorchesBuildingShadowsEnable = Config.Bind("Torches to buildings", "Enable shadows", true, "If torches should cast shadows. False can improve performance.");
            configSeedsDecayEnable = Config.Bind("Seeds mod", "Enable mod", true, "If seeds decay rate should be increased to get rid of a seeds mess on floor.");
            configFoodEnable = Config.Bind("Food mod", "Increase duration of food buff", true, "If foods duration effect should be increased to 5 minutes");


            ClassInjector.RegisterTypeInIl2Cpp<GrassTool>();

            //ClassInjector.RegisterTypeInIl2Cpp<RoadMakerMOD>();
            Harmony.CreateAndPatchAll(typeof(SpikesSelfDamageMod));
            SettingsMenuPatch.OnSettingsMenu += SpikesSelfDamageMod.OnSettingsMenu;

            ClassInjector.RegisterTypeInIl2Cpp<AskaPlusSpawner>();
            ClassInjector.RegisterTypeInIl2Cpp<VillagerBonusSpawn>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerBonusSpawn>();

            Harmony.CreateAndPatchAll(typeof(VillagerPatch));
            Harmony.CreateAndPatchAll(typeof(VillagerSurvivalPatch));
            Harmony.CreateAndPatchAll(typeof(CharacterPatch));
            SettingsMenuPatch.OnSettingsMenu += CharacterPatch.OnSettingsMenu;


            Harmony.CreateAndPatchAll(typeof(TorchesToBuildings));
            SettingsMenuPatch.OnSettingsMenu += TorchesToBuildings.OnSettingsMenu;
            Harmony.CreateAndPatchAll(typeof(AnchorsFix));
            Harmony.CreateAndPatchAll(typeof(ItemInfoPatch));
            SettingsMenuPatch.OnSettingsMenu += ItemInfoPatch.OnSettingsMenu;

            Harmony.CreateAndPatchAll(typeof(SettingsMenuPatch));
            Harmony.CreateAndPatchAll(typeof(Test));
 //           Harmony.CreateAndPatchAll(typeof(AskaRecipes));
            Helpers.ResourceInfos();
        }

        internal static class Helpers
        {
            public static Color greenColor = new Color(0, 0.5f, 0, 1);
            public static Color backGroundColor = new Color(0, 0, 0, 0.8f);
            public static Color SelectedOpt = new Color(1f,0.6824f,0f);
            public static Color UnselectedOpt = new Color(1f,1f,1f);
            public static readonly Vector2 HalfHalf = new Vector2(0.5f, 0.5f);
            public static Dictionary<string, AssetBundle> loadedAssetBundles = new Dictionary<string, AssetBundle>();
            public static Dictionary<string, ResourceInfo> resourceInfoSO = new Dictionary<string, ResourceInfo>();
            public static Dictionary<string, ItemInfo> itemInfoSO = new Dictionary<string, ItemInfo>();

            internal static Transform FindChildByNameCaseInsensitive(Transform parent, string name)
            {
                foreach (var o in parent)
                {
                    var child = o.TryCast<Transform>();
                    if (string.Equals(child.name, name, StringComparison.OrdinalIgnoreCase))
                        return child;

                    // Recursively search in the child
                    Transform found = FindChildByNameCaseInsensitive(child, name);
                    if (found != null)
                        return found;
                }
                return null;
            }
            internal static Object LoadAssetBundle(string assetBundleFileName, string prefabName, bool dontDestroyOnLoad = true)
            {
                System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;
                AssetBundle myAssetBundle;

                if (loadedAssetBundles.ContainsKey(assetBundleFileName))
                {
                    myAssetBundle = loadedAssetBundles[assetBundleFileName];
                    Debug.Log("AssetBundle is already loaded.");
                }
                else
                {
                    myAssetBundle = AssetBundle.LoadFromMemory((byte[])rm.GetObject(assetBundleFileName));

                    if (myAssetBundle == null)
                    {
                        Plugin.Log.LogError("Failed to load AssetBundle!");
                        return null;
                    }
                    else
                    {
                        loadedAssetBundles[assetBundleFileName] = myAssetBundle;
                    }
                }

                var loadedObject = myAssetBundle.LoadAsset(prefabName);
                //GameObject prefab = loadedObject.TryCast<GameObject>();

                // if (prefab != null && dontDestroyOnLoad)
                // {
                //     // Instantiate the prefab in the game
                //     //GameObject.Instantiate(prefab);
                //     GameObject.DontDestroyOnLoad(prefab);
                // }
                // else
                // {
                //     Plugin.Log.LogError("Failed to load prefab from AssetBundle!");
                // }

                return loadedObject;
            }

            internal static Sprite GetSpriteFromTexture2D(Texture2D texture2D)
            {
                if (!texture2D)
                    return null;
                Rect rect = new Rect(0, 0, texture2D.width, texture2D.height);
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                Sprite sprite = Sprite.Create(texture2D, rect, pivot);
                return sprite;
            }
            internal static void CreateSwitch(Transform parent, string text, ConfigEntry<bool> configEntry)
            {
                var button = GameObject.Instantiate(SettingsMenuPatch.Toggle, parent);
                button.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = text;
                button.transform.GetChild(8).gameObject.SetActive(true);
                var imgA = button.transform.GetChild(8).GetChild(0).gameObject;
                var imgB = GameObject.Instantiate(imgA, imgA.transform.parent);

                imgA.SetActive(true);
                imgB.SetActive(true);

                Component.DestroyImmediate(button.transform.GetChild(7).GetComponent<LocalizedText>());
                var ColorSchema = button.transform.GetChild(6).GetComponent<Button>().colors;
                Component.DestroyImmediate(button.transform.GetChild(6).GetComponent<Button>());
                Component.DestroyImmediate(button.transform.GetChild(5).GetComponent<Button>());
                Component.DestroyImmediate(button.GetComponent<IncreaseDecreasePanel>());
                var valu = button.transform.GetChild(4).GetComponent<TextMeshProUGUI>();

                valu.text = configEntry.Value == true ? "On" : "Off";
                imgA.GetComponent<Image>().color = valu.text == "Off" ? SelectedOpt : UnselectedOpt;
                imgB.GetComponent<Image>().color = valu.text == "Off" ? UnselectedOpt : SelectedOpt;

                Button btn1 = button.transform.GetChild(5).gameObject.AddComponent<Button>();
                Button btn2 = button.transform.GetChild(6).gameObject.AddComponent<Button>();

                btn1.targetGraphic = btn1.transform.GetChild(0).GetComponent<Image>();
                btn2.targetGraphic = btn2.transform.GetChild(0).GetComponent<Image>();
                btn1.colors = ColorSchema;
                btn2.colors = ColorSchema;

                UnityAction onIncreaseDelegate =
                    (UnityAction)(() =>
                    {
                        valu.text = valu.text == "Off" ? "On" : "Off";
                        configEntry.Value = valu.text == "On";
                        imgA.GetComponent<Image>().color = valu.text == "Off" ? SelectedOpt : UnselectedOpt;
                        imgB.GetComponent<Image>().color = valu.text == "Off" ? UnselectedOpt:SelectedOpt;
                      
                    });
                //UnityAction onDecreaseDelegate =
                //    (UnityAction)(() =>
                //    {
                //        valu.text = valu.text == "On" ? "Off" : "On";

                //        configEntry.Value = valu.text == "On";
                //    });

                btn1.onClick.AddListener(onIncreaseDelegate);
                btn2.onClick.AddListener(onIncreaseDelegate);
            }

            internal static void CreateCategory(Transform parent, string text)
            {
                var source = SettingsMenuPatch.Label;
                var labelInfo = GameObject.Instantiate(source, parent);
                labelInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
                Component.DestroyImmediate(labelInfo.transform.GetChild(0).GetComponent<LocalizedText>());
            }

            internal static void CreateItemsGoogleSheet()
            {
                //Plugin.Log.LogInfo($"Found {resourceInfoSO.Count} ResoureInfos");
                Plugin.Log.LogMessage("CONSUMABLES");
                foreach (var ri in resourceInfoSO.Values)
                {
                    // Plugin.Log.LogMessage($"{ri.name}");
                    if (ri.name.StartsWith("Item_Food"))
                    {
                        if (ri.TryCast<ConsumableInfo>() == true)
                        {
                            var ci = ri.Cast<ConsumableInfo>();
                            //    Plugin.Log.LogMessage($"{ci.name} is type of {ci.GetType().Name}");
                            //   Plugin.Log.LogInfo($"{ri.name} has {ci.modulatedConsumeEffects.Length} effects");
                            //foreach (var ce in ci.modulatedConsumeEffects)
                            //{
                            //    Plugin.Log.LogInfo($"from {ce.normalizedRange.min} to {ce.normalizedRange.max} has {ce.randomStatusEffects.Length} effects");
                            //    foreach (var se in ce.randomStatusEffects)
                            //    {
                            //        Plugin.Log.LogInfo($"duration {se.duration} with effect {se.table.effectType.name}({se.table.dialogueAdded}) which modify {se.table.vattrElements.Count} vital attributes and modify {se.table.attrElements.Count} character attributes");
                            //        foreach (var vattel in se.table.vattrElements)
                            //        {
                            //            Plugin.Log.LogInfo($"VITAL ATTRIBUTE: {vattel.modifier.mode} effect give {vattel.modifier.value} of attribute {vattel.targetAttribute.name}/{vattel.targetAttribute.localizedName} (attribid: {vattel.targetAttribute.attributeId}, type {vattel.targetAttribute.attributeType?.name})");
                            //        }
                            //        foreach (var attel in se.table.attrElements)
                            //        {
                            //            Plugin.Log.LogInfo($"CHARACTER ATTRIBUTE: {attel.modifier.Operation} effect give {attel.modifier.Value} of attribute {attel.targetAttribute.name}/{attel.targetAttribute.localizedName} (attribid: {attel.targetAttribute.attributeId}, type {attel.targetAttribute.attributeType?.name})");
                            //        }
                            //    }
                            //}



                            //Plugin.Log.LogMessage("EXPORT strings:");
                            //foreach (var ce in ci.modulatedConsumeEffects)
                            //{
                            //    foreach (var se in ce.randomStatusEffects)
                            //    {
                            //        foreach (var vattel in se.table.vattrElements)
                            //        {
                            //            Plugin.Log.LogInfo($"{ri.name};{ce.normalizedRange.min};{ce.normalizedRange.max};{se.table.effectType.name}({se.table.dialogueAdded});{se.duration};{vattel.modifier.mode};{vattel.targetAttribute.name};{vattel.modifier.value}");
                            //        }
                            //        foreach (var attel in se.table.attrElements)
                            //        {
                            //            Plugin.Log.LogInfo($"{ri.name};{ce.normalizedRange.min};{ce.normalizedRange.max};{se.table.effectType.name}({se.table.dialogueAdded});{se.duration};{attel.modifier.Operation};{attel.targetAttribute.name};{attel.modifier.Value}");
                            //        }
                            //    }
                            //}

                        }
                    }
                }
            }

            internal static void ResourceInfos()
            {
                //preload all resources before menu, after menu loading we can modify items
                var allScriptableObjects = Resources.LoadAll("", Il2CppSystem.Type.GetType("SSSGame.ResourceInfo, Assembly-CSharp"));

                resourceInfoSO = Resources.FindObjectsOfTypeAll<ResourceInfo>().ToDictionary(name => name.name, ri => ri);
                var iinfo = Resources.FindObjectsOfTypeAll<ItemInfo>();

              //  Plugin.Log.LogMessage("ItemInfos");

                foreach (var item in iinfo) {
             //       Plugin.Log.LogMessage(item.name);
                    itemInfoSO.TryAdd(item.name, item);
                }
            }
        }
    }
}
