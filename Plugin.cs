using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace askaplus.bepinex.mod
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        internal static new ManualLogSource Log;
        public static ConfigEntry<bool> configGrassPaintEnable;
        public static ConfigEntry<KeyCode> configGrassPaintKey;

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
            Harmony.CreateAndPatchAll(typeof(SettingsMenuPatch));
            configGrassPaintEnable = Config.Bind("GrassPaintig", "Enable mod", true, "Enable or disable mod");
            configGrassPaintKey = Config.Bind("GrassPainting", // The section under which the option is shown
                                            "KeyCode",  // The key of the configuration option in the configuration file
                                            KeyCode.RightBracket, // The default value
                                            "Key to paint grass"); // Description of the option to show in the config file

            SettingsMenuPatch.OnSettingsMenu += CharacterPatch.OnSettingsMenu;
            Harmony.CreateAndPatchAll(typeof(TorchesToBuildings));
            Harmony.CreateAndPatchAll(typeof(AnchorsFix));

    }
        
        public static class UIHelpers
        {
            public static Color greenColor = new Color(0, 0.5f, 0, 1);
            public static Color backGroundColor = new Color(0, 0, 0, 0.8f);
            public static readonly Vector2 HalfHalf = new Vector2(0.5f, 0.5f);
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
            public static RectTransform AddPanel(string name, Transform parent, Vector2 size, Color color,
            Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
            {
                GameObject panelGO = new GameObject(name);
                panelGO.transform.SetParent(parent, false);
                var panelRectTransform = panelGO.AddComponent<RectTransform>();
                var image = panelGO.AddComponent<Image>();
                image.color = color;
                panelRectTransform.anchorMin = anchorMin;
                panelRectTransform.anchorMax = anchorMax;
                panelRectTransform.anchoredPosition = anchoredPosition;
                panelRectTransform.pivot = pivot;
                panelRectTransform.sizeDelta = size;
                return panelRectTransform;
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
            public static Transform FindChildByNameCaseInsensitive(Transform parent, string name)
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

            public static ScrollRect AddScrollView(Transform parent, Vector2 size,
                Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, bool isVertical = false,
                float spacing = 0)
            {
                var scrollViewObject = new GameObject("ScrollView");
                var scrollViewRectTransform = scrollViewObject.AddComponent<RectTransform>();
                scrollViewRectTransform.SetParent(parent);
                scrollViewRectTransform.anchorMin = anchorMin;
                scrollViewRectTransform.anchorMax = anchorMax;
                scrollViewRectTransform.pivot = pivot;
                scrollViewRectTransform.sizeDelta = size;
                scrollViewRectTransform.anchoredPosition = anchoredPosition;
                var scrollRect = scrollViewObject.AddComponent<ScrollRect>();
                scrollRect.scrollSensitivity = 25;

                //Mask
                var maskGameObject = new GameObject("Mask");
                maskGameObject.transform.SetParent(scrollViewObject.transform, false);
                var maskRectTransform = maskGameObject.AddComponent<RectTransform>();
                maskRectTransform.anchorMax = new Vector2(1, 1);
                maskRectTransform.anchorMin = new Vector2(0, 0);
                maskRectTransform.sizeDelta = Vector2.zero;
                var maskImage = maskGameObject.AddComponent<Image>();
                maskImage.color = UIHelpers.backGroundColor;
                maskGameObject.AddComponent<Mask>();

                //Content
                var content = new GameObject("content");
                content.transform.SetParent(maskGameObject.transform, false);
                content.transform.localPosition = Vector3.zero;
                var contentRect = content.AddComponent<RectTransform>();
                contentRect.anchorMax = new Vector2(1, 1);
                contentRect.anchorMin = new Vector2(0, 0);
                contentRect.pivot = new Vector2(0.5f, 1);
                contentRect.offsetMin = Vector2.zero;
                contentRect.offsetMax = Vector2.zero;
                maskRectTransform.sizeDelta = Vector2.zero;

                if (isVertical)
                {
                    var verticalLayoutGroup = content.AddComponent<VerticalLayoutGroup>();
                    verticalLayoutGroup.childForceExpandHeight = false;
                    verticalLayoutGroup.childControlHeight = false;
                    verticalLayoutGroup.childForceExpandWidth = true;
                    verticalLayoutGroup.spacing = spacing;
                    var contentSizeFitter = content.AddComponent<ContentSizeFitter>();
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                }
                else
                {
                    var horizontalLayoutGroup = content.AddComponent<HorizontalLayoutGroup>();
                    horizontalLayoutGroup.childForceExpandHeight = true;
                    horizontalLayoutGroup.childForceExpandWidth = true;
                    horizontalLayoutGroup.childControlHeight = false;
                    horizontalLayoutGroup.childControlWidth = false;
                    horizontalLayoutGroup.spacing = spacing;
                    var contentSizeFitter = content.AddComponent<ContentSizeFitter>();
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                }

                scrollRect.content = contentRect;
                scrollRect.horizontal = !isVertical;
                scrollRect.vertical = isVertical;
                scrollRect.viewport = maskRectTransform;
                scrollRect.movementType = ScrollRect.MovementType.Clamped;
                return scrollRect;
            }
        }
    }
}
