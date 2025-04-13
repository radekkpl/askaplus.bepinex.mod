using HarmonyLib;
using SandSailorStudio.Assets;
using SSSGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class SpikesSelfDamageMod
    {
        static List<GameObject> wallMasters = new List<GameObject>();

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MainMenu.OnActivate))]
        public static void MainMenuOnActivatePostfix(MainMenu __instance)
        {
            var x = Resources.FindObjectsOfTypeAll<SSSGame.Combat.StructureDamageDealer>();

            foreach (var mb in x)
            {
                Plugin.Log.LogInfo($"Patching self damage in {mb.gameObject?.transform.parent?.name} from {mb.selfDamage} to 2");
                mb.selfDamage = 2;
            }
        }



    }
}