using HarmonyLib;
using SSSGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof (Villager))]
    static class VillagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Villager.Awake))]
        public static void Awake(ref Villager __instance) {
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

    private void Update() 
        {
            if (villager._mtActive & villager._mtTarget != lastInteraction) { 
                lastInteraction = villager._mtTarget;
                Plugin.Log.LogInfo($"{villager.gameObject.name} changed _mtTarget to {lastInteraction.name}");

            }
        }

        private void Awake()
        {
            //Plugin.Log.LogInfo($"VillagerBonusSpawn awake");
            villager = GetComponent<Villager>();
        }
    }

}
