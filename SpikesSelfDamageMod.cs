using HarmonyLib;
using SSSGame;
using UnityEngine;
using UnityEngine.Events;
using static askaplus.bepinex.mod.Plugin;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class SpikesSelfDamageMod
    {
        private static bool patched = false;
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MainMenu.OnActivate))]
        public static void MainMenuOnActivatePostfix(MainMenu __instance)
        {
            if (patched) return;
            patched = true;

            var x = Resources.FindObjectsOfTypeAll<SSSGame.Combat.StructureDamageDealer>();

            foreach (var mb in x)
            {
                Plugin.Log.LogDebug($"Patching self damage in {mb.gameObject?.transform.parent?.name} from {mb.selfDamage} to 2");
                Plugin.Log.LogDebug($"Patching damage in {mb.gameObject?.transform.parent?.name} from {mb.damage} to 3");
                mb.damage = 3;
                mb.selfDamage = 2;
            }

        }


        public static void OnSettingsMenu(Transform parent)
        {
            Helpers.CreateCategory(parent, "Spikes selfdamage");
            Helpers.CreateSwitch(parent, "* Enable Mod", configSpikesSelfDamageEnable);

            UnityAction applyCallback = (UnityAction)(() =>
            {
                Plugin.configGrassPaintKey.Value = KeyCode.Z;
            });
        }



    }
}