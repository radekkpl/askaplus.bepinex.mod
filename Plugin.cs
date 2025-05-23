using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Mono.Cecil.Cil;
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
        internal static ConfigEntry<bool> configRecipesEnable;
        internal static ConfigEntry<bool> configTorchesLightExtended;
        internal static ConfigEntry<bool> configMarksEnable;
        internal static ConfigEntry<float> configMarks_FoodHarvestRange;
        internal static ConfigEntry<float> configMarks_WoodHarvestRange;
        internal static ConfigEntry<float> configMarks_ForestryRange;
        internal static ConfigEntry<float> configMarks_StoneHarvestRange;
        internal static ConfigEntry<float> configMarks_HuntingRange;
        internal static ConfigEntry<float> configMarks_BuildingResourcesRange;
        
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
            configRecipesEnable = Config.Bind("Recipes mod", "Add custom recipes", true, "Add custom recipes to some stations");
            configTorchesLightExtended = Config.Bind("Torches to buildings", "Extended visibility range", false, "Light visibility distance. Default 60m, extended 200m");
            configMarksEnable = Config.Bind("Marks","Enable mod",true, "Enable or disable mod");
            configMarks_WoodHarvestRange = Config.Bind("Marks", "Wood Harvest Distance", 2.5f, "Distance multiplikator");
            configMarks_StoneHarvestRange = Config.Bind("Marks", "Stone Harvest Distance", 2.5f, "Distance multiplikator");
            configMarks_FoodHarvestRange = Config.Bind("Marks", "Food Harvest Distance", 2.5f, "Distance multiplikator");
            configMarks_HuntingRange = Config.Bind("Marks", "Hunting Distance", 2.5f, "Distance multiplikator");
            configMarks_BuildingResourcesRange = Config.Bind("Marks", "Building Resources Distance", 2.5f, "Distance multiplikator");
            configMarks_ForestryRange = Config.Bind("Marks", "Forestry Distance", 2.5f, "Distance multiplikator");

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
            Harmony.CreateAndPatchAll(typeof(StrucutrePatch));
            SettingsMenuPatch.OnSettingsMenu += TorchesToBuildings.OnSettingsMenu;
            Harmony.CreateAndPatchAll(typeof(AnchorsFix));
            Harmony.CreateAndPatchAll(typeof(ItemInfoPatch));
            SettingsMenuPatch.OnSettingsMenu += ItemInfoPatch.OnSettingsMenu;

            Harmony.CreateAndPatchAll(typeof(Marks));
            SettingsMenuPatch.OnSettingsMenu += Marks.OnSettingsMenu;

            Harmony.CreateAndPatchAll(typeof(SettingsMenuPatch));
            Harmony.CreateAndPatchAll(typeof(Test));
            Helpers.ResourceInfos();
            AskaRecipes.CreateRecipes();
            SettingsMenuPatch.OnSettingsMenu += AskaRecipes.OnSettingsMenu;
        }

        internal static class Helpers
        {
            public static Color greenColor = new Color(0, 0.5f, 0, 1);
            public static Color backGroundColor = new Color(0, 0, 0, 0.8f);
            public static Color SelectedOpt = new Color(1f,0.6824f,0f);
            public static Color UnselectedOpt = new Color(1f,1f,1f);
            public static readonly Vector2 HalfHalf = new Vector2(0.5f, 0.5f);
            public static Dictionary<string, AssetBundle> loadedAssetBundles = [];
            public static Dictionary<string, ResourceInfo> resourceInfoSO = [];
            public static Dictionary<string, ItemInfo> itemInfoSO = [];
            public static Dictionary<string, BlueprintConditionsRule> Dict_BCR = [];
            public static Dictionary<string, ItemStorageClass> Dict_ISC = [];
            public static Dictionary<string, ItemCategoryInfo> Dict_ICI = [];
            public static Dictionary<string, CraftInteraction> Dict_CI = [];
            public static Dictionary<string, ItemInfoList> Dict_BlueprintsList = [];

            public enum AskaAttributesEnum
            {
                WoodHarvest = 300,
                StoneHarvest = 301,
                Skinning = 307
            }

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

                btn1.onClick.AddListener(onIncreaseDelegate);
                btn2.onClick.AddListener(onIncreaseDelegate);
            }
            internal static void CreateSelectRange(Transform parent, string text, ConfigEntry<float> configEntry, float[] ranges)
            {
                var button = GameObject.Instantiate(SettingsMenuPatch.SelectRange, parent);
                button.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = text;
                button.transform.GetChild(8).gameObject.SetActive(true);
                Plugin.Log.LogInfo($"Item has this amount of childrens {button.transform.GetChild(8).childCount}");

                var valu = button.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
                switch (configEntry.Value)
                {
                    case 1f:
                        valu.text = "Default";
                        break;
                    case 1.5f:
                        valu.text = "Extended";
                        break;
                    case 3f:
                        valu.text = "Big";
                        break;
                    case 5f:
                        valu.text = "Huge";
                        break;
                    default:
                        valu.text = "Default";
                        configEntry.Value = 1f;
                        break;
                }
                Plugin.Log.LogInfo("Creating images");

             
                for (int i = 1; i < ranges.Length; i++) 
                {
                    var imgA = button.transform.GetChild(8).GetChild(0).gameObject;
                    var imgB = GameObject.Instantiate(imgA, imgA.transform.parent);
                    imgA.GetComponent<Image>().color = configEntry.Value == ranges[0] ? SelectedOpt : UnselectedOpt;
                    imgB.GetComponent<Image>().color = configEntry.Value == ranges[i] ? SelectedOpt : UnselectedOpt;
                }

              
                Component.DestroyImmediate(button.transform.GetChild(7).GetComponent<LocalizedText>());
                var ColorSchema = button.transform.GetChild(6).GetComponent<Button>().colors;
                Component.DestroyImmediate(button.transform.GetChild(6).GetComponent<Button>());
                Component.DestroyImmediate(button.transform.GetChild(5).GetComponent<Button>());
                Component.DestroyImmediate(button.GetComponent<IncreaseDecreasePanel>());

                Button btn1 = button.transform.GetChild(5).gameObject.AddComponent<Button>();
                Button btn2 = button.transform.GetChild(6).gameObject.AddComponent<Button>();

                btn1.targetGraphic = btn1.transform.GetChild(0).GetComponent<Image>();
                btn2.targetGraphic = btn2.transform.GetChild(0).GetComponent<Image>();
                btn1.colors = ColorSchema;
                btn2.colors = ColorSchema;

                UnityAction onIncreaseDelegate =
                    (UnityAction)(() =>
                    {
                        switch (configEntry.Value) 
                        {
                            case 1f:
                                valu.text = "Extended";
                                configEntry.Value = 1.5f;
                                button.transform.GetChild(8).GetChild(0).gameObject.GetComponent<Image>().color = UnselectedOpt;
                                button.transform.GetChild(8).GetChild(1).gameObject.GetComponent<Image>().color = SelectedOpt;
                                break;
                            case 1.5f:
                                valu.text = "Big";
                                configEntry.Value = 3f;
                                button.transform.GetChild(8).GetChild(1).gameObject.GetComponent<Image>().color = UnselectedOpt;
                                button.transform.GetChild(8).GetChild(2).gameObject.GetComponent<Image>().color = SelectedOpt;
                                break;
                            case 3f:
                                if (ranges.Length == 3) break;
                                valu.text = "Huge";
                                configEntry.Value = 5f;
                                button.transform.GetChild(8).GetChild(2).gameObject.GetComponent<Image>().color = UnselectedOpt;
                                button.transform.GetChild(8).GetChild(3).gameObject.GetComponent<Image>().color = SelectedOpt;
                                break;
                            default:
                                break;
                        }
                    });
                UnityAction onDecreaseDelegate =
                    (UnityAction)(() =>
                    {
                        switch (configEntry.Value)
                        {
                            case 5f:
                                valu.text = "Big";
                                configEntry.Value = 3f;
                                button.transform.GetChild(8).GetChild(3).gameObject.GetComponent<Image>().color = UnselectedOpt;
                                button.transform.GetChild(8).GetChild(2).gameObject.GetComponent<Image>().color = SelectedOpt;
                                break;
                            case 3f:
                                valu.text = "Extended";
                                configEntry.Value = 1.5f;
                                button.transform.GetChild(8).GetChild(2).gameObject.GetComponent<Image>().color = UnselectedOpt;
                                button.transform.GetChild(8).GetChild(1).gameObject.GetComponent<Image>().color = SelectedOpt;
                                break;
                            case 1.5f:
                                valu.text = "Default";
                                configEntry.Value = 1f;
                                button.transform.GetChild(8).GetChild(1).gameObject.GetComponent<Image>().color = UnselectedOpt;
                                button.transform.GetChild(8).GetChild(0).gameObject.GetComponent<Image>().color = SelectedOpt;
                                break;
                            default:
                                break;
                        }
                    });

                btn2.onClick.AddListener(onDecreaseDelegate);
                btn1.onClick.AddListener(onIncreaseDelegate);
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
                            Plugin.Log.LogMessage($"{ci.name} is type of {ci.GetType().Name}");
                            Plugin.Log.LogInfo($"{ri.name} has {ci.modulatedConsumeEffects.Length} effects");
                            foreach (var ce in ci.modulatedConsumeEffects)
                            {
                                Plugin.Log.LogInfo($"from {ce.normalizedRange.min} to {ce.normalizedRange.max} has {ce.randomStatusEffects.Length} effects");
                                foreach (var se in ce.randomStatusEffects)
                                {
                                    Plugin.Log.LogInfo($"duration {se.duration} with effect {se.table.effectType.name}({se.table.dialogueAdded}) which modify {se.table.vattrElements.Count} vital attributes and modify {se.table.attrElements.Count} character attributes");
                                    foreach (var vattel in se.table.vattrElements)
                                    {
                                        Plugin.Log.LogInfo($"VITAL ATTRIBUTE: {vattel.modifier.mode} effect give {vattel.modifier.value} of attribute {vattel.targetAttribute.name}/{vattel.targetAttribute.localizedName} (attribid: {vattel.targetAttribute.attributeId}, type {vattel.targetAttribute.attributeType?.name})");
                                    }
                                    foreach (var attel in se.table.attrElements)
                                    {
                                        Plugin.Log.LogInfo($"CHARACTER ATTRIBUTE: {attel.modifier.Operation} effect give {attel.modifier.Value} of attribute {attel.targetAttribute.name}/{attel.targetAttribute.localizedName} (attribid: {attel.targetAttribute.attributeId}, type {attel.targetAttribute.attributeType?.name})");
                                    }
                                }
                            }



                            Plugin.Log.LogMessage("EXPORT strings:");
                            foreach (var ce in ci.modulatedConsumeEffects)
                            {
                                foreach (var se in ce.randomStatusEffects)
                                {
                                    foreach (var vattel in se.table.vattrElements)
                                    {
                                        Plugin.Log.LogInfo($"{ri.name};{ce.normalizedRange.min};{ce.normalizedRange.max};{se.table.effectType.name}({se.table.dialogueAdded});{se.duration};{vattel.modifier.mode};{vattel.targetAttribute.name};{vattel.modifier.value}");
                                    }
                                    foreach (var attel in se.table.attrElements)
                                    {
                                        Plugin.Log.LogInfo($"{ri.name};{ce.normalizedRange.min};{ce.normalizedRange.max};{se.table.effectType.name}({se.table.dialogueAdded});{se.duration};{attel.modifier.Operation};{attel.targetAttribute.name};{attel.modifier.Value}");
                                    }
                                }
                            }

                        }
                    }
                }
            }

            internal static void ResourceInfos()
            {
                //preload all resources before menu, after menu loading we can modify items
                var allScriptableObjects = Resources.LoadAll("", Il2CppSystem.Type.GetType("SSSGame.ResourceInfo, Assembly-CSharp"));

                var itemInfo = Resources.FindObjectsOfTypeAll<ItemInfo>();
                foreach (var item in itemInfo)
                {
                    if (itemInfoSO.ContainsKey(item.name)) continue;
                    itemInfoSO.Add(item.name, item);
                }
                resourceInfoSO = Resources.FindObjectsOfTypeAll<ResourceInfo>().ToDictionary(name => name.name, ri => ri);

                //RECIPES
                Dict_BlueprintsList = Resources.FindObjectsOfTypeAll<ItemInfoList>().ToDictionary(name => name.name,i=>i);

                //Get Blueprint condition rules
                Plugin.Log.LogMessage("-----BlueprintsConditionRule-----");
                Dict_BCR = Resources.FindObjectsOfTypeAll<BlueprintConditionsRule>().ToDictionary(name => name.name, bcr => bcr);
                
                Plugin.Log.LogMessage("-----ItemStorageClass-----");
                Dict_ISC = Resources.FindObjectsOfTypeAll<ItemStorageClass>().ToDictionary(name => name.name, bcr => bcr);
                
                //Get ItemInfoCategory
                Plugin.Log.LogMessage("-----ItemInfoCategory-----");
                Dict_ICI = Resources.FindObjectsOfTypeAll<ItemCategoryInfo>().ToDictionary(name => name.name, bcr => bcr);
               
                //Get CraftInteractions
                Plugin.Log.LogMessage("-----CraftInteractions-----");
                var _CI = Resources.FindObjectsOfTypeAll<CraftInteraction>();
                Dict_CI = [];
                foreach (var _item in _CI)
                {
                    if (!Dict_CI.ContainsKey(_item.name))
                    {
                        //Plugin.Log.LogMessage($"{_item.name}");
                        Dict_CI.Add(_item.name, _item);
                    }
                }

            }
        }
    }
}
