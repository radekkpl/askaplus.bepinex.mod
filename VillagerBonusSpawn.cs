using System;
using HarmonyLib;
using Il2CppSystem;
using SSSGame;
using UnityEngine;
using static askaplus.bepinex.mod.Plugin;

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

            if (!villager._mtActive | villager._mtTarget == lastInteraction) return;

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
                case "Item_Wood_birch1":
                case "Item_Wood_birch2":
                   var tSpawner = lastInteraction.parent.FindChild("TrunkSpawner");
                    AskaPlusSpawner bonusSpawner;
                    if (tSpawner.gameObject.TryGetComponent<AskaPlusSpawner>(out bonusSpawner) == true) return;
                    bonusSpawner = tSpawner.gameObject.AddComponent<AskaPlusSpawner>();
                    var harvestInteraction = lastInteraction.parent.FindChild("HarvestInteraction").GetComponent<HarvestInteraction>();

                    var info = Helpers.resourceInfoSO["Item_Wood_HardWoodLog"];

                    //Woodcutting = 300   
                    var skillValue = villager.Attributes.GetAttribute(300).GetValue();
                    var randomChance = UnityEngine.Random.value * 75;
                    Plugin.Log.LogInfo($"{villager.gameObject.name}: WoodHarvesting skill is {skillValue} and GM rolled {UnityEngine.Mathf.Round(randomChance)}");
                    if (randomChance <= skillValue)
                    {
                        bonusSpawner.amount = 1;
                        Plugin.Log.LogMessage($"Spawning additional HardWoodLog.");
                    }
                    else
                    {
                        Plugin.Log.LogMessage($"No luck this time.");
                    }
                    bonusSpawner.harvestInteraction = harvestInteraction;
                    bonusSpawner.componentInfo = info;
                    bonusSpawner.ignoreMasterItem = true;
                    harvestInteraction.add_OnHarvestDamageTaken(new System.Action(() => bonusSpawner.OnHarvestDamageTaken()));
                    break;
                case "Item_Wood_fir2":
                    break;
                default:
                    break;
            }
        }

        private void Awake()
        {
            //Plugin.Log.LogInfo($"VillagerBonusSpawn awake");
            villager = GetComponent<Villager>();
        }

        
    }

}
