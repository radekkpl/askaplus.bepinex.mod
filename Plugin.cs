using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

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
            ClassInjector.RegisterTypeInIl2Cpp<GrassTool>();
//            ClassInjector.RegisterTypeInIl2Cpp<RoadMakerMOD>();
            Harmony.CreateAndPatchAll(typeof(SpikesSelfDamageMod));
            Harmony.CreateAndPatchAll(typeof(VillagerPatch));
            Harmony.CreateAndPatchAll(typeof(CharacterPatch));
            Harmony.CreateAndPatchAll(typeof(AnchorsFix));
        }
        
        public static class UIHelpers
        {
            public static Color greenColor = new Color(0, 0.5f, 0, 1);
            public static Button AddButton(string text, Transform parent, Vector2 size, Vector2 anchoredPosition,
                Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Color color, UnityAction callback)
            {
                Vector2 rectTransformSize;
                GameObject buttonGO = new GameObject("Button");
                buttonGO.transform.SetParent(parent);
                RectTransform rectTransform = buttonGO.AddComponent<RectTransform>();
                Image image = buttonGO.AddComponent<Image>();
                image.color = color;
                Button button = buttonGO.AddComponent<Button>();

                rectTransform.sizeDelta = size;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.pivot = pivot;
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransformSize = rectTransform.rect.size;
                AddTextMeshPro(text, rectTransformSize.y / 2, buttonGO.transform, Vector2.zero, Vector2.zero,
                    new Vector2(0, 0),
                    new Vector2(1, 1), new Vector2(0.5f, 0.5f));

                if (callback != null)
                    button.onClick.AddListener(callback);
                return button;
            }

            public static TextMeshProUGUI AddTextMeshPro(string text, float fontSize, Transform parent, Vector2 size,
            Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
            {
                GameObject textObject = new GameObject("MyText");
                var rectTransform = textObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = size;
                TextMeshProUGUI textMeshPro = textObject.AddComponent<TextMeshProUGUI>();
                textMeshPro.text = text;
                textMeshPro.fontSize = fontSize;
                textMeshPro.alignment = TextAlignmentOptions.Center;

                // Set TextMeshProUGUI as child of Canvas
                textObject.transform.SetParent(parent, false);
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.pivot = pivot;
                rectTransform.anchoredPosition = anchoredPosition;
                return textMeshPro;
            }
        }
    }
}
