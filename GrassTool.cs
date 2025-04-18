using BepInEx.Configuration;
using HarmonyLib;
using SSSGame;
using System;
using UnityEngine;
using UnityEngine.Events;
using static askaplus.bepinex.mod.Plugin;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(Character))]
    public static class CharacterPatch
    {
       

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Character.Spawned))]
        public static void Spawned(Character __instance)
        {
            if (__instance.IsPlayer() && __instance.GetLocalAuthorityMask() == 1)
            {
                Console.WriteLine("Player spawned");
                if (__instance.GetComponentInChildren<GrassTool>() != null) return;

                var GrassToolObj = __instance.gameObject.transform.CreateChild("GrassMod");
                GrassToolObj.gameObject.AddComponent<SSSGame.HeightmapTool>();
                
                var GrassTool = GrassToolObj.gameObject.AddComponent<GrassTool>();
                GrassToolObj.gameObject.SetActive(true);

            }
        }


        public static void OnSettingsMenu(Transform parent)
        {
            UIHelpers.AddTextMeshPro("--- Grass painting ---", 24, parent, new Vector2(0, 50), UIHelpers.HalfHalf,
                Vector2.zero, Vector2.right, UIHelpers.HalfHalf);
            UnityAction applyCallback = (UnityAction)(() =>
              {
                 return;
              });
             // UnityAction<bool> changeModEnabled = (UnityAction<bool>)((bool value) =>
             // {

             //     configGrass ModEnabled = value;
             // });
             // UnityAction<KeyCode> ApplyKeyCodeCallback = (UnityAction<KeyCode>)((KeyCode key) =>
             // {
             //     ApplyPainting = key;
             // });
             var panel = UIHelpers.AddPanel("Panel", parent, new Vector2(100, 100),
                UIHelpers.backGroundColor, Vector2.zero,
                new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f));
            UIHelpers.AddButton("Apply", panel, new Vector2(200, 25),
                new Vector2(0, 0), UIHelpers.HalfHalf, UIHelpers.HalfHalf,
                UIHelpers.HalfHalf, UIHelpers.greenColor, applyCallback);
        }
    }

    internal class GrassTool:MonoBehaviour
    {
        Transform position;
        HeightmapTool HeightmapTool;
        private float x = 10;
        private float z = 10;
        private TerraformingToolOperation operation = TerraformingToolOperation.PAINT;

        private void Start()
        {
            HeightmapTool = gameObject.GetComponent<HeightmapTool>();
            //if (HeightmapTool == null) Plugin.Log.LogInfo("Heightmaptool not found");
            position = gameObject.transform.parent.transform;
        }

        private void Update() {

            if (Input.GetKeyDown(Plugin.configGrassPaintKey.Value))
            {
                position = gameObject.transform.parent.transform;
                //Plugin.Log.LogInfo("Trying _UpdateTerraforming");
                HeightmapTool.radius = 0.25f;
                HeightmapTool.clearVegetation = false;
                HeightmapTool.setTerrainType = true;
                HeightmapTool.terrainType = TerraformingMap.TerrainType.NATURAL;
                HeightmapTool.PaintHere();
            }
        }
    }
}
