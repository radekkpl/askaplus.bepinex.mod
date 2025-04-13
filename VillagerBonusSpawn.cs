using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using SandSailorStudio.RNG;
using SSSGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (lastInteraction.parent == null) return;

            Plugin.Log.LogInfo($"{villager.gameObject.name} changed _mtTarget to {lastInteraction.name} in {lastInteraction.parent.name}");

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
                               var attrib = villager.Attributes;
                               var woodCutting = attrib.GetAttribute(300);
                               var amount = woodCutting.GetValue();
                               Plugin.Log.LogInfo($"{villager.gameObject.name}: WoodHarvesting skill is {amount}");
                               var rnd = UnityEngine.Random.value * 100;
                               Plugin.Log.LogInfo($"{villager.gameObject.name}: Rnd value for bonus spawn is {rnd}");
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
                                Plugin.Log.LogInfo($"{villager.gameObject.name}: WoodHarvesting skill is {amount}");
                                var rnd = UnityEngine.Random.value * 100;
                                Plugin.Log.LogInfo($"{villager.gameObject.name}: Rnd value for bonus spawn is {rnd}");
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
