using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
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
        public override void Load()
        {

        // Plugin startup logic
            Log = base.Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            
            configGrassPaintEnable = Config.Bind("GrassPaintig", "Enable mod", true, "Enable or disable mod");
            configGrassPaintKey = Config.Bind("GrassPainting", "KeyCode", KeyCode.RightBracket, "Key to paint grass");
            ClassInjector.RegisterTypeInIl2Cpp<GrassTool>();

            //            ClassInjector.RegisterTypeInIl2Cpp<RoadMakerMOD>();
            configSpikesSelfDamageEnable = Config.Bind("Spikes selfdamage", "Enable mod", true, "Enable or disable mod");
            Harmony.CreateAndPatchAll(typeof(SpikesSelfDamageMod));

            configBonusSpawnEnable = Config.Bind("Bonus spawn", "Enable mod", true, "Enable or disable mod");
            ClassInjector.RegisterTypeInIl2Cpp<VillagerBonusSpawn>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerBonusSpawn>();
            Harmony.CreateAndPatchAll(typeof(VillagerPatch));
            
            Harmony.CreateAndPatchAll(typeof(CharacterPatch));            
            SettingsMenuPatch.OnSettingsMenu += CharacterPatch.OnSettingsMenu;



            Harmony.CreateAndPatchAll(typeof(TorchesToBuildings));
            Harmony.CreateAndPatchAll(typeof(AnchorsFix));


            Harmony.CreateAndPatchAll(typeof(SettingsMenuPatch));
            UIHelpers.ResourceInfos();
        }

        internal static class UIHelpers
        {
            public static Color greenColor = new Color(0, 0.5f, 0, 1);
            public static Color backGroundColor = new Color(0, 0, 0, 0.8f);
            public static readonly Vector2 HalfHalf = new Vector2(0.5f, 0.5f);
            public static Dictionary<string, AssetBundle> loadedAssetBundles = new Dictionary<string, AssetBundle>();
            public static List<ResourceInfo> resourceInfoSO = new List<ResourceInfo>();

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
                Component.DestroyImmediate(button.transform.GetChild(7).GetComponent<LocalizedText>());
                Component.DestroyImmediate(button.transform.GetChild(6).GetComponent<Button>());
                Component.DestroyImmediate(button.transform.GetChild(5).GetComponent<Button>());
                Component.DestroyImmediate(button.GetComponent<IncreaseDecreasePanel>());
                var valu = button.transform.GetChild(4).GetComponent<TextMeshProUGUI>();

                valu.text = configEntry.Value == true ? "True" : "False";
                Button btn1 = button.transform.GetChild(5).gameObject.AddComponent<Button>();
                Button btn2 = button.transform.GetChild(6).gameObject.AddComponent<Button>();

                UnityAction onIncreaseDelegate =
                    (UnityAction)(() =>
                    {
                        if (valu.text == "False")
                        {
                            valu.text = "True";
                        }

                        configEntry.Value = valu.text == "True";
                    });
                UnityAction onDecreaseDelegate =
                    (UnityAction)(() =>
                    {
                        if (valu.text == "True")
                        {
                            valu.text = "False";
                        }
                        configEntry.Value = valu.text == "True";
                    });

                btn1.onClick.AddListener(onIncreaseDelegate);
                btn2.onClick.AddListener(onDecreaseDelegate);
            }

            internal static void CreateCategory(Transform parent, string text)
            {
                var source = SettingsMenuPatch.Label;
                var labelInfo = GameObject.Instantiate(source, parent);
                labelInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
                Component.DestroyImmediate(labelInfo.transform.GetChild(0).GetComponent<LocalizedText>());
            }


            internal static void ResourceInfos() 
            {
                var allScriptableObjects = Resources.LoadAll("", Il2CppSystem.Type.GetType("SSSGame.ResourceInfo, Assembly-CSharp"));
                resourceInfoSO = allScriptableObjects
                    .Select(so => so.TryCast<ResourceInfo>())
                    .Where(ri => ri != null).ToList();
            }
        }
    }
}
