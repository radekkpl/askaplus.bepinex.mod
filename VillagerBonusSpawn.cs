using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using SSSGame;
using UnityEngine;

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
            //villagerBonusSpawner.villager = __instance;
            //Plugin.Log.LogInfo($"VillagerBonusSpawn awaked for character: {__instance.gameObject.name}");
        }
    }
    

    public class VillagerBonusSpawn : MonoBehaviour
    {
        public Villager villager = null;
        private Transform lastInteraction;
        private Transform tSpawner;
        private Il2CppArrayBase <SubcomponentSpawner> sSpawner;
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
                    sSpawner = tSpawner?.GetComponents<SubcomponentSpawner>();
                    //Plugin.Log.LogInfo($"{villager.gameObject.name}: TrunkSpawner found in {lastInteraction.parent.name}");
                    if (sSpawner == null) return;
    
                    foreach (var spw in sSpawner)
                        {
                           if (spw.componentInfo.Name == "Hardwood Log")
                            {
                                //Woodcutting = 300   
                               var skillValue = villager.Attributes.GetAttribute(300).GetValue();
                               var randomChance = UnityEngine.Random.value * 75;
                               Plugin.Log.LogInfo($"{villager.gameObject.name}: WoodHarvesting skill is {skillValue} and GM rolled {UnityEngine.Mathf.Round(randomChance)}");
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
                    tSpawner = lastInteraction.parent.FindChild("TrunkSpawner");
                    sSpawner = tSpawner?.GetComponents<SubcomponentSpawner>();
                    if (sSpawner == null) return;
                        foreach (var spw in sSpawner)
                        {
                            if (spw.componentInfo.Name == "Hardwood Log")
                            {

                                var attrib = villager.Attributes;
                                var woodCutting = attrib.GetAttribute(300);
                                var amount = woodCutting.GetValue();
                                var rnd = UnityEngine.Random.value * 100;
                                Plugin.Log.LogInfo($"{villager.gameObject.name}: WoodHarvesting skill is {amount} and chance is {rnd}");
                                if (rnd < amount)
                                {
                                    spw.amount += 1;
                                    Plugin.Log.LogInfo($"Spawning additional HardWoodLog.");
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
            //Plugin.Log.LogInfo($"VillagerBonusSpawn awake");
            villager = GetComponent<Villager>();
        }

        
    }

}
