using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
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
            //Plugin.Log.LogInfo($"Villager awake");
            if (__instance.gameObject.GetComponent<VillagerBonusSpawn>() is not null) return;
            var villagerBonusSpawner = __instance.gameObject.AddComponent<VillagerBonusSpawn>();
        }
    }

    public class VillagerBonusSpawn : MonoBehaviour
    {
        public Villager villager = null;
        private Transform lastInteraction;
        private Transform tSpawner;
        private void Update()
        {
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
                    tSpawner = lastInteraction.parent.FindChild("TrunkSpawner");

                    if (tSpawner.GetComponent<AskaPlusSpawner>() is not null) return;

                    var bonusSpawner = tSpawner.gameObject.AddComponent<AskaPlusSpawner>();
                    var harvestInteraction = tSpawner.gameObject.GetComponent<HarvestInteraction>();

                    var info = UIHelpers.resourceInfoSO.Find(x => x.name.ToLower() == "hardwood log");

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
