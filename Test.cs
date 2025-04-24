using AsmResolver.Patching;
using HarmonyLib;
using SandSailorStudio.UI;
using SSSGame;
using SSSGame.AI.FSM;
using System.Linq;
using UnityEngine;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(Menu))]
    internal class Test
    {
        static bool patched;
        
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Menu.OnActivate))]
        public static void MainMenuOnActivatePostfix(MainMenu __instance)
        {
            if (patched) return;
            patched = true;

            var structures = Resources.FindObjectsOfTypeAll<Structure>().ToDictionary(name => name.name, st => st);
            var nodeTemplates = Resources.FindObjectsOfTypeAll<NodeTemplate>().ToDictionary(name => name.name, nd => nd);
            var test = Resources.FindObjectsOfTypeAll<FSM_CleanupInventory>()[0];

            Plugin.Log.LogInfo("Storage to return patched");
            //var cs = structures["Workshop_L2"].GetComponent<CraftingStation>();
            //var ciq = cs._cleanupInventoryQuest;
           
            //var fsmb = ciq.fsmBehaviour;
           
            //var fsmst = fsmb.selectedNode;
           
            //var act = fsmst.actions;
            //FSM_CleanupInventory cli = null;

            //foreach (var ac in act)
            //{
            //    if (ac.defaultName == "Clean-up Inventory") 
            //    {
            //        cli = ac.Cast<FSM_CleanupInventory>();
            //    }
            //}
            //if (cli == null) return;

          //  test.storagesToReturnTo.Add(nodeTemplates["Item_Addons_LeatherworkerL1"]);
          //  test.storagesToReturnTo.Add(nodeTemplates["Item_Structures_WoodCutterL2"]);
          //  test.storagesToReturnTo.Add(nodeTemplates["Item_Structures_StoneCutterL2"]);
          //  test.storagesToReturnTo.Add(nodeTemplates["Item_Structures_BloomeryL2"]);
            test.minimumDistance = 1;

        }
    }
}
