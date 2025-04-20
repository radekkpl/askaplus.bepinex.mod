using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using SandSailorStudio.Attributes;
using SSSGame;
using SSSGame.Render;
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

                var GrassToolObj = __instance.gameObject.transform.CreateChild("AskaPlusMODS");
                GrassToolObj.transform.localPosition = new Vector3(0f,0f,2f);
                GrassToolObj.gameObject.AddComponent<SSSGame.HeightmapTool>();
                
                var GrassTool = GrassToolObj.gameObject.AddComponent<GrassTool>();
                GrassToolObj.gameObject.AddComponent<PlayerBonusSpawn>();
                GrassToolObj.gameObject.SetActive(true);
            }
        }


        public static void OnSettingsMenu(Transform parent)
        {
           UIHelpers.CreateCategory(parent, "Grass painting");
           UIHelpers.CreateSwitch(parent, "Enable Mod", configGrassPaintEnable);

            UnityAction applyCallback = (UnityAction)(() =>
            {
                Plugin.configGrassPaintKey.Value = KeyCode.Z;
            });
        }

    }

    internal class GrassTool:MonoBehaviour
    {
        Vector3 position;
        HeightmapTool HeightmapTool;
        PlayerInteractionAgent PlayerInteractionAgent;
        private TerraformingToolOperation operation = TerraformingToolOperation.PAINT;

        private void Start()
        {
            PlayerInteractionAgent = gameObject.GetComponentInParent<PlayerInteractionAgent>();
            HeightmapTool = gameObject.GetComponent<HeightmapTool>();
            //if (HeightmapTool == null) Plugin.Log.LogInfo("Heightmaptool not found");
        }

        private void Update() {
            if (Input.GetKeyDown(Plugin.configGrassPaintKey.Value))
            {
                position = gameObject.transform.position;
                Plugin.Log.LogInfo($"Trying _UpdateTerraforming with radius {HeightmapTool.radius}");
                HeightmapTool.radius = 1f;
                HeightmapTool.clearVegetation = false;
                HeightmapTool.setTerrainType = true;
                HeightmapTool.terrainType = TerraformingMap.TerrainType.NATURAL;
                HeightmapTool.Run(operation,position);
                HeightmapTool.PaintHere();
            }
        }
    }
    internal class PlayerBonusSpawn : MonoBehaviour
    {
        private PlayerInteractionAgent playerInteractionAgent;
        private AttributeManager attributeManager;
        private Pickable lastPickable;
        private Transform tSpawner;
        private Il2CppArrayBase<SubcomponentSpawner> sSpawner;
        private void Update()
        {
            if (playerInteractionAgent is null) { Plugin.Log.LogError("PlayerInteravtionAgeint is null"); }

            if (playerInteractionAgent._favoritePickable is null)   return;
            if (playerInteractionAgent._favoritePickable == lastPickable) return;

            lastPickable = playerInteractionAgent._favoritePickable;
            
 
            Plugin.Log.LogDebug($"Target changed to {lastPickable.name}");
 

            switch (lastPickable.name)
            {
                case "Item_Wood_birch1":
                case "Item_Wood_birch2":
                    tSpawner = lastPickable.gameObject.transform.FindChild("TrunkSpawner");
                    sSpawner = tSpawner?.GetComponents<SubcomponentSpawner>();
                    //Plugin.Log.LogInfo($"{villager.gameObject.name}: TrunkSpawner found in {lastInteraction.parent.name}");
                    if (sSpawner == null) return;

                    foreach (var spw in sSpawner)
                    {
                        if (spw.componentInfo.Name == "Hardwood Log")
                        {
                            //Woodcutting = 300   
                            var skillValue = attributeManager.GetAttribute(300).GetValue();
                            var randomChance = UnityEngine.Random.value * 75;
                            Plugin.Log.LogInfo($"PlyerCharacter: WoodHarvesting skill is {skillValue} and GM rolled {UnityEngine.Mathf.Round(randomChance)}");
                            if (randomChance <= skillValue)
                            {
                                spw.amount += 1;
                                Plugin.Log.LogMessage($"Spawning additional HardWoodLog. Total of: {spw.amount}");
                            }
                            else
                            {
                                Plugin.Log.LogMessage($"No luck this time. Spawning only {spw.amount}");
                            }
                        }
                    }
                    break;
                case "Item_Wood_fir2":
                    break;
                    tSpawner = lastPickable.gameObject.transform.FindChild("TrunkSpawner");
                    sSpawner = tSpawner?.GetComponents<SubcomponentSpawner>();
                    if (sSpawner == null) return;
                    foreach (var spw in sSpawner)
                    {
                        if (spw.componentInfo.Name == "Log")
                        {

                            //Woodcutting = 300   
                            var skillValue = attributeManager.GetAttribute(300).GetValue();
                            var randomChance = UnityEngine.Random.value * 75;
                            Plugin.Log.LogInfo($"PlyerCharacter: WoodHarvesting skill is {skillValue} and GM rolled {UnityEngine.Mathf.Round(randomChance)}");
                            if (randomChance <= skillValue)
                            {
                                spw.amount += 1;
                                Plugin.Log.LogMessage($"Spawning additional Log. Total of: {spw.amount}");
                            }
                            else
                            {
                                Plugin.Log.LogInfo($"No luck this time.");
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void Awake()
        {
            Plugin.Log.LogInfo($"PlayerCharacter bonusspawn awake");
            playerInteractionAgent = gameObject.GetComponentInParent<PlayerInteractionAgent>();
            attributeManager = gameObject.GetComponentInParent<AttributeManager>();



        }
    }
}
