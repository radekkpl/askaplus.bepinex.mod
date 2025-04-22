using HarmonyLib;
using SSSGame;
using System.Linq;
using UnityEngine;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class Test
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MainMenu.OnActivate))]
        public static void MainMenuOnActivatePostfix(MainMenu __instance)
        {
            var structures = Resources.FindObjectsOfTypeAll<Structure>().ToDictionary(name => name.name, st => st);
            var nodeTemplates = Resources.FindObjectsOfTypeAll<NodeTemplate>().ToDictionary(name => name.name, nd => nd);


            var cs = structures["Workshop_L2"].GetComponent<CraftingStation>();


        }
    }
}
