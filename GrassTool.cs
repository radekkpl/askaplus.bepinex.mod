using Fusion;
using HarmonyLib;
using SandSailorStudio.UI;
using SSSGame;
using SSSGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using static askaplus.bepinex.mod.Plugin;
using static Fusion.NetworkCharacterController;

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
                var GrassToolObj = __instance.gameObject.transform.CreateChild("GrassMod");
                GrassToolObj.gameObject.AddComponent<SSSGame.HeightmapTool>();
                
                var GrassTool = GrassToolObj.gameObject.AddComponent<GrassTool>();
                GrassToolObj.gameObject.SetActive(true);

            }
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

            if (Input.GetKeyDown(KeyCode.RightBracket))
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
