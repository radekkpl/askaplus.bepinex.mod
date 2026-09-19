using HarmonyLib;
using SandSailorStudio.Attributes;
using SandSailorStudio.Inventory;
using SandSailorStudio.WorldGen;
using SSSGame;
using SSSGame.Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using static askaplus.bepinex.mod.Plugin;
using static askaplus.bepinex.mod.Plugin.Helpers;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(PlayerCharacter))]
    public static class CharacterPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(PlayerCharacter.Spawned))]
        public static void Spawned(PlayerCharacter __instance)
        {
             if (__instance.IsPlayer() && __instance.GetLocalAuthorityMask() == 1)
            {
                Console.WriteLine("Player spawned");
                if (__instance.GetComponentInChildren<CaveResetTool>() != null) return;

                var AskaPlusGO = __instance.gameObject.transform.CreateChild("AskaPlusMODS");
                AskaPlusGO.transform.localPosition = new Vector3(0f,0f,2f);
                AskaPlusGO.gameObject.AddComponent<CaveResetTool>();
                AskaPlusGO.gameObject.AddComponent<PlayerBonusSpawn>();
                AskaPlusGO.gameObject.SetActive(true);
            }
        }


        public static void OnSettingsMenu(Transform parent)
        {
           //Helpers.CreateCategory(parent, "Grass painting");
           //Helpers.CreateSwitch(parent, "Enable Mod", configGrassPaintEnable);

           Helpers.CreateCategory(parent, "Bonus items");
           Helpers.CreateSwitch(parent, "* Enable Mod", configBonusSpawnEnable);
        }
    }

    internal class CaveResetTool:MonoBehaviour
    {
        private void ResetCave(int CaveID)
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            Log.LogInfo($"found scene {scene.name}");
            var rootGO = scene.GetRootGameObjects();

            List<CaveResourceStorage> caveResourceStorages = new List<CaveResourceStorage>();
            GameObject WorldGenerator = null;
            foreach (var item in rootGO)
            {
                if (item.name == "WorldGenerator")
                {
                    WorldGenerator = item;
                }
                if (item.GetComponent<CaveResourceStorage>() != null) 
                {
                    caveResourceStorages.Add(item.GetComponent<CaveResourceStorage>());
                }
            }

            Log.LogInfo($"Found {caveResourceStorages.Count} CaveBuildings");

            if (WorldGenerator != null) Log.LogInfo($"World Generator found in scene: {scene.name}");
            
            var caveManager = WorldGenerator.GetComponent<CavesManager>();

            if (caveManager != null) Log.LogInfo("Caves Manager found");
            var caveData = caveManager.GetCaveData(ref CaveID, DataAccessMode.FETCH);

            if (caveData.caveId != CaveID)
            {
                Log.LogInfo($"caveData.caveId: {caveData.caveId} and is not requested {CaveID}");
            }
            Log.LogInfo("Setting explored state to false");
            caveData.SetExploredState(false);
            if (caveData.digStates != null)
            {
                Log.LogInfo($"Dig states count: {caveData.digStates.Count}");
                foreach (var state in caveData.digStates)
                {
                    var digData = state.Item2;
                    digData.OnDigDataChanged = null;
                }
                caveData.digStates.Clear();
                caveData.digStates = null;
            }

            if (caveData.openNodes != null) 
            {
                Log.LogInfo($"Open nodes count: {caveData.openNodes.Count}");
                caveData.openNodes.Clear();
            }

            if (caveData.OnExploredStatusChanged != null)
            {
                Log.LogInfo("Clearing OnExploredStatusChanged");
                caveData.OnExploredStatusChanged = null;
            }
            if (caveData.OnNodeOpenStatusChanged != null)
            {
                Log.LogInfo("Clearing OnNodeOpenStatusChanged");
                caveData.OnNodeOpenStatusChanged = null;
            }
            caveData.explored = false;

            Log.LogInfo("finding caveEntrance");
            var caveEntrance = caveManager._residentCaves[CaveID];
            if (caveEntrance.caveId != CaveID)
            {
                Log.LogInfo($"caveEntrance.caveId: {caveEntrance.caveId} and is not requested {CaveID}");
            }
            var caveBiomeConfiguration = caveEntrance.gameObject.GetComponent<CaveBiome>().Configuration;
            if (caveBiomeConfiguration != null)
            {
                Log.LogInfo("CaveBiomeConfiguration found");

            }

            Log.LogInfo("finding parent");
            var parent = caveEntrance.gameObject.transform.parent;

            var trs = parent.transform;
            var name = parent.name;
            Log.LogInfo($"Parent name: {parent.name} with position: {trs.position} and rotation {trs.eulerAngles.y}");
            var caves = parent.transform.parent;
            var caveArea = caveEntrance.CaveArea;
            caveArea.Setup();
            CaveAreaData caveAreaData = caveArea.area.Cast<CaveAreaData>();

            var generator = caveAreaData.Generator;
            Log.LogInfo("Cave Generator found");

            var randomGenerator = generator.RandomGenerator;
            Log.LogInfo("Random Generator found");
            int num = System.Environment.TickCount ^ System.Guid.NewGuid().GetHashCode();
            if (num < 0) num = -num;
            randomGenerator.SetSeed(num);
            Log.LogInfo("Random Generator set new seed");

            caveArea.Release();
            Log.LogInfo("Cave area released");
            generator.ClearGeometry();
            Log.LogInfo("generator geometry cleared");
            var newCave = generator.Generate(trs.transform.position, trs.eulerAngles.y, name, randomGenerator);
            Log.LogInfo($"New cave {newCave.name} created at {newCave.transform.position}");
            
            newCave.transform.SetParent(caves, true);
            Log.LogInfo($"Parent was set");

            caveArea._root = newCave;
            Log.LogInfo($"caveArea root was set");
            var newCaveEntrance = newCave.GetComponentInChildren<CaveEntrance>();
            var caveBiome = newCave.GetComponentInChildren<CaveBiome>();
            caveBiome.Configuration = caveBiomeConfiguration;

            newCaveEntrance.caveId = CaveID;
            newCaveEntrance.CaveArea = caveArea;
            newCaveEntrance._persistentData = caveData;
            newCaveEntrance._initialized = false;
            newCaveEntrance.Initialize();
            newCaveEntrance.Open();
            caveManager._residentCaves[CaveID] = newCaveEntrance;
            caveData.SetExploredState(true);
            caveArea.isExplored = true;
            
            Log.LogInfo($"New Cave Entrance Initialized");
            newCaveEntrance.CutTerrainHole();
            Log.LogInfo($"New Cave Entrance TerrainHoleCutted");
            newCaveEntrance._UpdateRenderers();
            Log.LogInfo($"New Cave Entrance Renderers updated");
            newCaveEntrance._UpdateObjects();
            Log.LogInfo($"New Cave Entrance Objects updated");
            newCaveEntrance._UpdateVolumes();
            Log.LogInfo($"New Cave Entrance Volumes updated");
            newCaveEntrance._UpdateOpenState();
            Log.LogInfo($"New Cave Entrance Open state updated");

            foreach (var item in caveResourceStorages)
            {
                Log.LogInfo($"{item._NetConnectedCaveEntrance} is connectedCaveEntrance" );
                if (item._NetConnectedCaveEntrance == CaveID) 
                {
                    try
                    {
                        item.CaveEntrance = newCaveEntrance;
                        item._InitCaveEntrance(newCaveEntrance);
                    }
                    catch (Exception ex)
                    {
                        Log.LogInfo("Cave resource storage failed update cave entrance");
                        Log.LogError(ex);
                   }
                }
            }

        }
    }

    internal class PlayerBonusSpawn : MonoBehaviour
    {
        private PlayerInteractionAgent playerInteractionAgent;
        private AttributeManager attributeManager;
        public GameObject lastPickable;
        private NetworkSession _networkSession;

        private void Update()
        {
            if (Plugin.configBonusSpawnEnable.Value == false) return;
            if (playerInteractionAgent is null) { Plugin.Log.LogError("PlayerInteractionAgent is null"); }

            var _pickable = playerInteractionAgent._favoritePickable;
            if (_pickable is null)   return;

            //when last object is destroyed just before this call
            if (lastPickable is null)
            {
                Plugin.Log.LogInfo("Last pickable was null. Trying to get game object");
                lastPickable = _pickable?.gameObject;
            }
            //if looking to same object as in previous frame
            if (_pickable?.gameObject == lastPickable) return;

            //update last pickable and proceed
            lastPickable = _pickable?.gameObject;

            //last return
            if (lastPickable is null) return;

            Plugin.Log.LogInfo($"Target changed to {lastPickable?.name}");
            switch (lastPickable?.name)
            {
                case "Harvest_Stone4":
                case "Harvest_StoneClumpSmall":
                    TryAddBonusSpawner(lastPickable, AskaAttributesEnum.StoneHarvest, Helpers.resourceInfoSO["Item_Stone_Raw"], Vector3.zero, 1, true, true);
                    break;
                case "Item_Wood_birch1":
                case "Item_Wood_birch2":
                    TryAddBonusSpawner(lastPickable, AskaAttributesEnum.WoodHarvest, Helpers.resourceInfoSO["Item_Wood_HardWoodLongStick"], Vector3.zero, 1, true,true);
                    break;
                case "Item_Wood_Willow":
                    TryAddBonusSpawner(lastPickable, AskaAttributesEnum.WoodHarvest, Helpers.resourceInfoSO["Item_Wood_HardWoodLongStick"], Vector3.zero, 2, false, true);
                    break;
                case "Item_Wood_Fir1":
                case "Item_Wood_Fir2":
                case "Item_Wood_Fir3": 
                case "Item_Wood_Fir4":
                case "Item_Wood_Fir5":
                    TryAddBonusSpawner(lastPickable, AskaAttributesEnum.WoodHarvest, Helpers.resourceInfoSO["Item_Wood_RawLongStick"], Vector3.zero, 1, true, true);
                    break;
                case "Harvest_JotunBlood":
                case "Harvest_JotunBloodSmall":
                    TryAddBonusSpawner(lastPickable, AskaAttributesEnum.StoneHarvest, Helpers.resourceInfoSO["Item_Magic_EyeOfOdin"], Vector3.zero, 1, true, true);
                    break;
                case "Item_IronDeposit":
                    TryAddBonusSpawner(lastPickable, AskaAttributesEnum.StoneHarvest, Helpers.resourceInfoSO["Item_Iron_Ore"], Vector3.zero, 5, false, true);
                    break;
                case "Item_Food_MeatHunk":
                    TryAddBonusSpawner(lastPickable, AskaAttributesEnum.Skinning, Helpers.resourceInfoSO["Item_Misc_BoneFragments"], Vector3.zero, 2, false, true);
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

            if (randomChance <= skillValue && AmountIsFix)
            {
                if (AmountIsFix)
                {
                    bonusSpawner.amount = HowMuchToAdd;

                    Plugin.Log.LogMessage(
                        $"RND {randomChance:F1} <= ({skill}) {skillValue} = " +
                        $"Spawning additional {HowMuchToAdd} of {whatToSpawn.name}"
                    );
                }
                else
                {
                    // Base amount:
                    // Skill 0  -> 1
                    // Skill 75 -> HowMuchToAdd
                    float baseAmount = 1f +
                                       (HowMuchToAdd - 1f) *
                                       (skillValue / 75f);

                    // Mastery bonus starts at skill 75.
                    // Skill 75  -> +0
                    // Skill 100 -> +HowMuchToAdd
                    float masteryBonus = HowMuchToAdd *
                                         Mathf.Pow(
                                             Mathf.Max(0f, skillValue - 75f) / 25f,
                                             2f
                                         );

                    bonusSpawner.amount = Mathf.FloorToInt(
                        baseAmount + masteryBonus
                    );

                    Plugin.Log.LogMessage(
                        $"RND {randomChance:F1} <= ({skill}) {skillValue} | Base: {baseAmount:F1} | Mastery: {masteryBonus:F1} | Total: {bonusSpawner.amount} | Spawning additional {bonusSpawner.amount} of {whatToSpawn.name}"
                    );
                }
            }
            else
            {
                Plugin.Log.LogMessage(
                    $"No luck this time with {skill}. RND {randomChance:F1} > {skillValue}"
                );

                bonusSpawner.amount = 0;
            }
            if (RunOnFullyHarvested) bonusSpawner.UseFullyHarvested = true;
            Plugin.Log.LogMessage($"Adding harvestInteraction to bonusSpawner.");
            bonusSpawner.positionNoise = 0.5f;
            bonusSpawner.rotationNoise = 0.2f;
            bonusSpawner.spacing = new Vector3(0f, 0.25f, 0f); 
            bonusSpawner.harvestInteraction = harvestInteraction;
            bonusSpawner.componentInfo = whatToSpawn;
            bonusSpawner.ignoreMasterItem = true;
            bonusSpawner.originOffset = offsetOfSpawn + new Vector3(0,0.5f,0);
            bonusSpawner._networkSession = _networkSession;
        }
        private void Awake()
        {
            Plugin.Log.LogDebug($"PlayerCharacter bonus spawn awake");
            playerInteractionAgent = gameObject.GetComponentInParent<PlayerInteractionAgent>();
            attributeManager = gameObject.GetComponentInParent<AttributeManager>();
            Plugin.Log.LogDebug($"PlayerCharacter catching network session");
            _networkSession = gameObject.GetComponentInParent<PlayerCharacter>()._session;
            if (_networkSession is null) Plugin.Log.LogError($"PlayerCharacter network session is null");
        }

    }
}
