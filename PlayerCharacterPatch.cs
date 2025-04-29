using HarmonyLib;
using SandSailorStudio.Attributes;
using SandSailorStudio.Inventory;
using SSSGame;
using System;
using UnityEngine;
using static askaplus.bepinex.mod.Plugin;
using static askaplus.bepinex.mod.Plugin.Helpers;

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
                GrassToolObj.gameObject.AddComponent<HeightmapTool>();
                
                var GrassTool = GrassToolObj.gameObject.AddComponent<GrassTool>();
                GrassToolObj.gameObject.AddComponent<PlayerBonusSpawn>();
                GrassToolObj.gameObject.SetActive(true);
            }
        }


        public static void OnSettingsMenu(Transform parent)
        {
           Helpers.CreateCategory(parent, "Grass painting");
           Helpers.CreateSwitch(parent, "Enable Mod", configGrassPaintEnable);

           Helpers.CreateCategory(parent, "Bonus items");
           Helpers.CreateSwitch(parent, "* Enable Mod", configBonusSpawnEnable);
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
            if (Plugin.configGrassPaintEnable.Value && Input.GetKeyDown(Plugin.configGrassPaintKey.Value) )
            {
                position = gameObject.transform.position;
                Plugin.Log.LogDebug($"Trying _UpdateTerraforming with radius {HeightmapTool.radius}");
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
        private GameObject lastPickable;

        
        private void Update()
        {
            if (Plugin.configBonusSpawnEnable.Value == false) return;
            if (playerInteractionAgent is null) { Plugin.Log.LogError("PlayerInteractionAgent is null"); }

            if (playerInteractionAgent._favoritePickable is null)   return;
            if (playerInteractionAgent._favoritePickable.gameObject == lastPickable) return;

            lastPickable = playerInteractionAgent._favoritePickable?.gameObject;
             
           // Plugin.Log.LogInfo($"Target changed to {lastPickable.name}");

            switch (lastPickable.name)
            {
                case "Harvest_Stone4":
                case "Harvest_StoneClumpSmall":
                    TryAddBonusSpawner(lastPickable, AskaAttributesEnum.StoneHarvest, Helpers.resourceInfoSO["Item_Stone_Raw"], Vector3.zero, 1, true, true);
                    break;
                case "Item_Wood_birch1":
                case "Item_Wood_birch2":
                    TryAddBonusSpawner(lastPickable, AskaAttributesEnum.WoodHarvest, Helpers.resourceInfoSO["Item_Wood_HardWoodLog"], Vector3.zero, 1, true,true);
                    break;
                case "Item_Wood_Willow":
                    TryAddBonusSpawner(lastPickable, AskaAttributesEnum.WoodHarvest, Helpers.resourceInfoSO["Item_Wood_HardWoodLog"], Vector3.zero, 2, false, true);
                    break;
                case "Item_Wood_Fir1":
                case "Item_Wood_Fir2":
                case "Item_Wood_Fir3": 
                case "Item_Wood_Fir4":
                case "Item_Wood_Fir5":
                    TryAddBonusSpawner(lastPickable, AskaAttributesEnum.WoodHarvest, Helpers.resourceInfoSO["Item_Wood_RawLog"], Vector3.zero, 1, true, true);
                    break;
                case "Harvest_JotunBlood":
                case "Harvest_JotunBloodSmall":
                    break;
                case "Item_Misc_CrawlerEgg1":
                case "Item_Misc_CrawlerEgg2":
                case "Item_Misc_CrawlerEgg3":
                case "Item_Misc_CrawlerEgg4":
                //THIS DOESNOT WORK, OnFullHarvested is not called, on Harvest damage is called but never with 0 health. SO SPAWNER CANNOT RUN. AND ALSO 25 COPIES OF ITEM IS ALSO NOT PERFECT
                // TO DO FIND A BETTER WAY TO SPAWN MODE IN ONE SPAWN
                // SPAWNER IT SEEMS GET INFORMATION ABOUT AMOUNT FROM ITEM AND IGNORE AMOUNT FROM SpawnItemChance :(          
                // Plugin.Log.LogDebug($"{villager.gameObject.name} : {villager.GetWorkstation().GetName()} -> changed _mtTarget to {lastInteraction.name} in {lastInteraction.parent.name}");
                //  TryAddBonusSpawner(lastInteraction.gameObject, AskaAttributesEnum.Skinning, Helpers.resourceInfoSO["Item_Wood_Resin"],new Vector3(0f,1f,0f), 25, false,false);
                break;
                default:
                    break;
            }
        }
        private void TryAddBonusSpawner(GameObject WhereToLook, AskaAttributesEnum skill, ItemInfo whatToSpawn, Vector3 offsetOfSpawn, int HowMuchToAdd, bool AmountIsFix, bool RunOnFullyHarvested)
        {
            AskaPlusSpawner bonusSpawner;
            if (WhereToLook.TryGetComponent<AskaPlusSpawner>(out bonusSpawner) == true) return;
            bonusSpawner = WhereToLook.AddComponent<AskaPlusSpawner>();
            var harvestInteraction = lastPickable.GetComponentInChildren<HarvestInteraction>();
            var skillValue = attributeManager.GetAttribute((int)skill).GetValue();
            var randomChance = UnityEngine.Random.value * 75;

            if (randomChance <= skillValue)
            {
                bonusSpawner.amount = HowMuchToAdd;
                Plugin.Log.LogMessage($"RND {randomChance} <= ({skill}) {skillValue} = Spawning additional {HowMuchToAdd} of {whatToSpawn.name}");

            }
            else if (!AmountIsFix)
            {
                bonusSpawner.amount = Mathf.CeilToInt((100 - (randomChance - skillValue)) / 100 * HowMuchToAdd);
                Plugin.Log.LogMessage($"RND {randomChance} > ({skill}) {skillValue} = Diff is {randomChance - skillValue} = Spawning additional {bonusSpawner.amount} of {whatToSpawn.name}");
            }
            else
            {
                Plugin.Log.LogMessage($"No luck this time with {skill}.");
                bonusSpawner.amount = 0; //Just for clarification       
            }
            if(RunOnFullyHarvested) bonusSpawner.UseFullyHarvested = true;
            Plugin.Log.LogMessage($"Adding harvestInteraction to bonusSpawner.");
            bonusSpawner.positionNoise = 0.5f;
            bonusSpawner.rotationNoise = 0.2f;
            bonusSpawner.spacing = new Vector3(2, 0, 0); 
            bonusSpawner.harvestInteraction = harvestInteraction;
            bonusSpawner.componentInfo = whatToSpawn;
            bonusSpawner.ignoreMasterItem = true;
            bonusSpawner.originOffset = offsetOfSpawn;
        }
        private void Awake()
        {
            Plugin.Log.LogDebug($"PlayerCharacter bonus spawn awake");
            playerInteractionAgent = gameObject.GetComponentInParent<PlayerInteractionAgent>();
            attributeManager = gameObject.GetComponentInParent<AttributeManager>();
        }
        
    }
}
