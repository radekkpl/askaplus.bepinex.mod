using HarmonyLib;
using Invector;
using SandSailorStudio.UI;
using SandSailorStudio.Utils;
using SSSGame.Localization;
using SSSGame.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static askaplus.bepinex.mod.Plugin;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(SettingsMenu))]
    internal class SettingsMenuPatch
    {
        internal static Action<Transform> OnSettingsMenu;
        internal static GameObject Label;
        internal static GameObject RebindKey;
        internal static GameObject Toggle;
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SettingsMenu.Start))]
        public static void StartPostfix(SettingsMenu __instance)
        {
            var panelRef = __instance.transform.FindChild("Panel");
            var videoSettingsTransform = panelRef.FindChild("VideoPage");
            var sourceForItems = panelRef.FindChildByNameRecursive("KeyboardButtonListContent");
            //Plugin.Log.LogInfo("SourceForItem");
            var sourceForItems2 = panelRef.FindChild("ControlsPage").FindChild("LayoutGroup");
            //Plugin.Log.LogInfo("SourceForItem2");
            var PageSubtitle = videoSettingsTransform.FindChild("PageSubTitle");
            //Plugin.Log.LogInfo("PageSubtitle");
            var Rect = videoSettingsTransform.FindChild("Rect");
            var tabsButtons = panelRef.FindChild("TabsButtons");
            var layoutGroup = tabsButtons.FindChild("LayoutGroup");
            var videoTabButton = UIHelpers.FindChildByNameCaseInsensitive(layoutGroup, "VideoTabButton");
            var newSettingButtonGO = GameObject.Instantiate(videoTabButton.gameObject, videoTabButton.parent);
            newSettingButtonGO.transform.SetSiblingIndex(layoutGroup.childCount - 2);
            newSettingButtonGO.name = "ModTabButton";
            newSettingButtonGO.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Aska+";
            
            var iconGO = newSettingButtonGO.transform.FindChild("Icon")?.gameObject;
            var iconShadowGO = newSettingButtonGO.transform.FindChild("Icon_Shadow")?.gameObject;
            var icon = iconGO.GetComponent<Image>();
            var icon_shadow = iconShadowGO.GetComponent<Image>();
            var iconTexture =UIHelpers.LoadAssetBundle("askaplus", "AskaPLUS").TryCast<Texture2D>();

            icon.sprite = UIHelpers.GetSpriteFromTexture2D(iconTexture);
            icon_shadow.sprite = UIHelpers.GetSpriteFromTexture2D(iconTexture);
            //Plugin.Log.LogInfo("Icons");

            var AskaPlusSettingPage = new GameObject("AskaPlusSettingsPage");
            AskaPlusSettingPage.AddComponent<TabPage>();
            AskaPlusSettingPage.transform.SetParent(panelRef, false);
            AskaPlusSettingPage.transform.localPosition = Vector3.zero;

            //Plugin.Log.LogInfo("AskaSettingsPage");

            var subtitle = GameObject.Instantiate(PageSubtitle, AskaPlusSettingPage.transform);
            Component.DestroyImmediate(subtitle.GetComponent<LocalizedText>());
            subtitle.GetComponent<TextMeshProUGUI>().text = "Aska+ settings. (Settings marked with * need restart a game)";

            Plugin.Log.LogInfo("Subtitle");

            newSettingButtonGO.GetComponent<TabButton>().page = AskaPlusSettingPage.GetComponent<TabPage>();
            newSettingButtonGO.GetComponent<TabButton>().order = 4;
            newSettingButtonGO.GetComponent<TabButtonTooltip>().toolTipDetails = "Aska+ Settings";

            Plugin.Log.LogInfo("Button");

            var trasfEventHelper = AskaPlusSettingPage.AddComponent<TransformEventsHelper>();
            var newPageTransform = AskaPlusSettingPage.GetComponent<RectTransform>() ?? AskaPlusSettingPage.AddComponent<RectTransform>();
            newPageTransform.anchorMin = Vector2.zero;
            newPageTransform.anchorMax = Vector2.one;
            newPageTransform.anchoredPosition = new Vector2(0, -28.415f);
            newPageTransform.offsetMin = Vector2.zero;
            newPageTransform.offsetMax = new Vector2(0, -56.83f);
            newPageTransform.sizeDelta = new Vector2(0, -56.83f);
            AskaPlusSettingPage.SetActive(false);
            

            Plugin.Log.LogInfo("NewPage");

            var customSettings = GameObject.Instantiate(Rect.gameObject, AskaPlusSettingPage.transform).transform.GetChild(0).GetChild(0).gameObject;

            Label = sourceForItems.FindChild("Movement Categ Label").gameObject;
            RebindKey = sourceForItems.FindChild("RebindKey").gameObject;
            Toggle = sourceForItems2.FindChild("InvertLookX").gameObject;


            Plugin.Log.LogInfo("SettingsSources");
            Plugin.Log.LogInfo($"Custom settings: {customSettings.name} .childs: {customSettings.transform.GetChildCount()}");

            while (customSettings.transform.GetChildCount() > 0)
            {
                GameObject.DestroyImmediate(customSettings.transform.GetChild(0).gameObject);
            }

            OnSettingsMenu?.Invoke(customSettings.transform);
        }

    }
}
