using HarmonyLib;
using SandSailorStudio.Inventory;
using SSSGame;
using UnityEngine;
using static askaplus.bepinex.mod.Plugin;
using static askaplus.bepinex.mod.Plugin.Helpers;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(Villager))]
    static class VillagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Villager.Awake))]
        public static void Awake(ref Villager __instance)
        {
            VillagerBonusSpawn villager;
            //Plugin.Log.LogInfo($"Villager awake");
            if (__instance.gameObject.TryGetComponent<VillagerBonusSpawn>(out villager) == true) return;
            villager = __instance.gameObject.AddComponent<VillagerBonusSpawn>();
        }
    }

    [HarmonyPatch(typeof(VillagerSurvival))]
    static class VillagerSurvivalPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(VillagerSurvival.Spawned))]
        public static void Spawned(ref VillagerSurvival __instance)
        {
            //Plugin.Log.LogInfo($"{__instance.gameObject.name} hp threshold for fighting is {__instance._dataSheet?.hpThresholdForFighting}");
            __instance._dataSheet.hpThresholdForFighting = 0.2f;
        }
    }


    public class VillagerBonusSpawn : MonoBehaviour
    {
        public Villager villager = null;
        private Transform lastInteraction;
      
        private void Update()
        {
            if (Plugin.configBonusSpawnEnable.Value == false) return;

            if (!villager._mtActive || villager._mtTarget == lastInteraction) return;

            lastInteraction = villager._mtTarget;
            if (lastInteraction.parent is null) return;

     
            if (villager.HasWorkstation())
            {
                Plugin.Log.LogDebug($"{villager.gameObject.name} : {villager.GetWorkstation().GetName()} -> changed _mtTarget to {lastInteraction.name} in {lastInteraction.parent.name}");
            }
            else 
            {
                Plugin.Log.LogDebug($"{villager.gameObject.name} : No Workstation -> changed _mtTarget to {lastInteraction.name} in {lastInteraction.parent.name}");
            }

            if (lastInteraction.name != "HarvestInteraction") return;

            switch (lastInteraction.parent.name)
            {
                case "Harvest_Stone4":
                case "Harvest_Stone_StoneClumpSmall":
                    TryAddBonusSpawner(lastInteraction.gameObject, AskaAttributesEnum.StoneHarvest, Helpers.resourceInfoSO["Item_Stone_Raw"], Vector3.zero, 1, true, true);
                    break;
                case "Item_Wood_birch1":
                case "Item_Wood_birch2":
                    TryAddBonusSpawner(lastInteraction.gameObject, AskaAttributesEnum.WoodHarvest, Helpers.resourceInfoSO["Item_Wood_HardWoodLog"],Vector3.zero, 1, true,true);
                    break;
                case "Item_Wood_Willow":
                    TryAddBonusSpawner(lastInteraction.gameObject, AskaAttributesEnum.WoodHarvest, Helpers.resourceInfoSO["Item_Wood_HardWoodLog"], Vector3.zero, 2, false, true);
                    break;
                case "Item_Wood_Fir1":
                case "Item_Wood_Fir2":
                case "Item_Wood_Fir3":
                case "Item_Wood_Fir4":
                case "Item_Wood_Fir5":
                    TryAddBonusSpawner(lastInteraction.gameObject, AskaAttributesEnum.WoodHarvest, Helpers.resourceInfoSO["Item_Wood_RawLog"], Vector3.zero, 1, true, true);                    
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
            var harvestInteraction = lastInteraction.GetComponent<HarvestInteraction>();
            var skillValue = villager.Attributes.GetAttribute((int)skill).GetValue();
            var randomChance = Random.value * 75;

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
            //Plugin.Log.LogInfo($"VillagerBonusSpawn awake");
            villager = GetComponent<Villager>();
        }
    }

}
