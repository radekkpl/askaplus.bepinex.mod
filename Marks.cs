using HarmonyLib;
using SSSGame;
using UnityEngine.Events;
using UnityEngine;
using static askaplus.bepinex.mod.Plugin;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(Structure))]
    internal class Marks
    {
        [HarmonyPostfix()]
        [HarmonyPatch(nameof(Structure._Initialize))]
        public static void MarkPostFix(Structure __instance) 
        {
            if (Plugin.configMarksEnable.Value==false) return;

            if (__instance.name.StartsWith("FoodHarvestMaker")) 
            {
                float defaultValue = 60f;
                float newValue = defaultValue * Plugin.configMarks_FoodHarvestRange.Value;
                var harvestMarker = __instance.gameObject.GetComponentInChildren<HarvestMarker>(true);
                var navMesh = __instance.gameObject.GetComponent<NavMeshInterestArea>();
                var objectiveMarker = __instance.gameObject.GetComponent<StructureObjectiveMarker>();
                if (harvestMarker is not null) harvestMarker.radius = newValue;
                if (navMesh is not null) navMesh.size = newValue * 1.2f;
                if (objectiveMarker is not null) objectiveMarker.range = newValue;
                Plugin.Log.LogInfo($"Marker {__instance.name} range changed to {newValue}");
                return;
            }
            if (__instance.name.StartsWith("WoodHarvestMarker"))
            {
                float defaultValue = 60f;
                float newValue = defaultValue * Plugin.configMarks_WoodHarvestRange.Value;
                var harvestMarker = __instance.gameObject.GetComponentInChildren<HarvestMarker>(true);
                var navMesh = __instance.gameObject.GetComponent<NavMeshInterestArea>();
                var objectiveMarker = __instance.gameObject.GetComponent<StructureObjectiveMarker>();
                if (harvestMarker is not null) harvestMarker.radius = newValue;
                if (navMesh is not null) navMesh.size = newValue * 1.2f;
                if (objectiveMarker is not null) objectiveMarker.range = newValue;
                Plugin.Log.LogInfo($"Marker {__instance.name} range changed to {newValue}");
                return;
            }
            if (__instance.name.StartsWith("HuntingMarker"))
            {
                float defaultValue = 75f;
                float newValue = defaultValue * Plugin.configMarks_HuntingRange.Value;
                var harvestMarker = __instance.gameObject.GetComponentInChildren<HarvestMarker>(true);
                var navMesh = __instance.gameObject.GetComponent<NavMeshInterestArea>();
                var objectiveMarker = __instance.gameObject.GetComponent<StructureObjectiveMarker>();
                if (harvestMarker is not null) harvestMarker.radius = newValue;
                if (navMesh is not null) navMesh.size = newValue * 1.2f;
                if (objectiveMarker is not null) objectiveMarker.range = newValue;
                Plugin.Log.LogInfo($"Marker {__instance.name} range changed to {newValue}");
                return;
            }
            if (__instance.name.StartsWith("BuildingResourcesMarker"))
            {
                float defaultValue = 50f;
                float newValue = defaultValue * Plugin.configMarks_BuildingResourcesRange.Value;
                var harvestMarker = __instance.gameObject.GetComponentInChildren<HarvestMarker>(true);
                var navMesh = __instance.gameObject.GetComponent<NavMeshInterestArea>();
                var objectiveMarker = __instance.gameObject.GetComponent<StructureObjectiveMarker>();
                if (harvestMarker is not null) harvestMarker.radius = newValue;
                if (navMesh is not null) navMesh.size = newValue * 1.2f;
                if (objectiveMarker is not null) objectiveMarker.range = newValue;
                Plugin.Log.LogInfo($"Marker {__instance.name} range changed to {newValue}");
                return;
            }
            if (__instance.name.StartsWith("StoneHarvestMarker"))
            {
                float defaultValue = 60f;
                float newValue = defaultValue * Plugin.configMarks_StoneHarvestRange.Value;
                var harvestMarker = __instance.gameObject.GetComponentInChildren<HarvestMarker>(true);
                var navMesh = __instance.gameObject.GetComponent<NavMeshInterestArea>();
                var objectiveMarker = __instance.gameObject.GetComponent<StructureObjectiveMarker>();
                if (harvestMarker is not null) harvestMarker.radius = newValue;
                if (navMesh is not null) navMesh.size = newValue * 1.2f;
                if (objectiveMarker is not null) objectiveMarker.range = newValue;
               Plugin.Log.LogInfo($"Marker {__instance.name} range changed to {newValue}");
                return;
            }
            if (__instance.name.StartsWith("ForestryMarker"))
            {
                float defaultValue = 40f;
                float newValue = defaultValue * Plugin.configMarks_ForestryRange.Value;
                var harvestMarker = __instance.gameObject.GetComponentInChildren<HarvestMarker>(true);
                var navMesh = __instance.gameObject.GetComponent<NavMeshInterestArea>();
                var objectiveMarker = __instance.gameObject.GetComponent<StructureObjectiveMarker>();
                if(harvestMarker is not null) harvestMarker.radius = newValue;
                if (navMesh is not null) navMesh.size = newValue*1.2f;
                if (objectiveMarker is not null) objectiveMarker.range = newValue;
                Plugin.Log.LogInfo($"Marker {__instance.name} range changed to {newValue}");
                return;
            }
        }

        public static void OnSettingsMenu(Transform parent)
        {
            Helpers.CreateCategory(parent, "Markers mod");
            Helpers.CreateSwitch(parent, "* Enable mod", configMarksEnable);
            Helpers.CreateSelectRange(parent, "* Food Harvest Range", configMarks_FoodHarvestRange, [1f,1.5f,3f,5f]);
            Helpers.CreateSelectRange(parent, "* Wood Harvest Range", configMarks_WoodHarvestRange, [1f, 1.5f, 3f]);
            Helpers.CreateSelectRange(parent, "* Stone Harvest Range", configMarks_StoneHarvestRange, [1f, 1.5f, 3f, 5f]);
            Helpers.CreateSelectRange(parent, "* Building Resources Range", configMarks_BuildingResourcesRange, [1f, 1.5f, 3f, 5f]);
            Helpers.CreateSelectRange(parent, "* Hunting Range", configMarks_HuntingRange, [1f, 1.5f, 3f, 5f]);
            Helpers.CreateSelectRange(parent, "* Forestry Range", configMarks_ForestryRange, [1f, 1.5f, 3f]);
        }
    }
}
