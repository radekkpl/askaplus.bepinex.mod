using HarmonyLib;
using SandSailorStudio.UI;
using SandSailorStudio.Utils;
using SSSGame.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using static askaplus.bepinex.mod.Plugin;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(SettingsMenu))]
    internal class SettingsMenuPatch
    {
        public static Action<Transform> OnSettingsMenu;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(SettingsMenu.Start))]
        public static void StartPostfix(SettingsMenu __instance)
        {
            var panelRef = __instance.transform.FindChild("Panel");
            var videoSettingsTransform = panelRef.FindChild("VideoPage");
            var newPage = new GameObject("ModSettingsPage");
            newPage.transform.SetParent(panelRef, false);
            newPage.transform.localPosition = Vector3.zero;
            newPage.AddComponent<TabPage>();
            newPage.AddComponent<TransformEventsHelper>();
            var tabsButtons = panelRef.FindChild("TabsButtons");
            var layoutGroup = tabsButtons.FindChild("LayoutGroup");
            var videoTabButton = UIHelpers.FindChildByNameCaseInsensitive(layoutGroup, "VideoTabButton");
            var newSettingButtonGO = GameObject.Instantiate(videoTabButton.gameObject, videoTabButton.parent);
            newSettingButtonGO.transform.SetSiblingIndex(layoutGroup.childCount - 2);
            newSettingButtonGO.name = "ModTabButton";
            var iconGO = newSettingButtonGO.transform.FindChild("Icon")?.gameObject;
            var iconShadowGO = newSettingButtonGO.transform.FindChild("Icon_Shadow")?.gameObject;
            var icon = iconGO.GetComponent<Image>();
            var icon_shadow = iconShadowGO.GetComponent<Image>();
            //var iconTexture = LoadAssetBundle("markerbundle.bundle", "wrench").TryCast<Texture2D>();

            //icon.sprite = StaticCoreHelpers.GetSpriteFromTexture2D(iconTexture);
            //icon_shadow.sprite = StaticCoreHelpers.GetSpriteFromTexture2D(iconTexture);
            newSettingButtonGO.GetComponent<TabButton>().page = newPage.GetComponent<TabPage>();
            newSettingButtonGO.GetComponent<TabButton>().order = 4;
            var newPageTransform = newPage.GetComponent<RectTransform>() ?? newPage.AddComponent<RectTransform>();
            newPageTransform.anchorMin = Vector2.zero;
            newPageTransform.anchorMax = Vector2.one;
            newPageTransform.sizeDelta = new Vector2(-150, -150);
            newPage.SetActive(false);
            newSettingButtonGO.GetComponent<TabButtonTooltip>().toolTipDetails = "Mod Settings";
            var scrollView = UIHelpers.AddScrollView(newPageTransform, Vector2.zero, Vector2.zero, Vector2.zero,
                Vector2.one, new Vector2(0.5f, 0.5f), true, 5);
            OnSettingsMenu?.Invoke(scrollView.content);
        }

        
    }
}
