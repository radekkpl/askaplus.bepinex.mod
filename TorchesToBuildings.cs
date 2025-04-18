using AsmResolver.Patching;
using HarmonyLib;
using SandSailorStudio.Assets;
using SandSailorStudio.Pooling;
using SSSGame;
using SSSGame.Combat;
using SSSGame.Network;
using SSSGame.Render;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SSSGame.Network.NetworkLink;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class TorchesToBuildings
    {
        private static bool patched = false;
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MainMenu.OnActivate))]
        public static void MainMenuOnActivatePostfix(MainMenu __instance)
        {
            if (patched) return;
            patched = true;
            GameObject sourcePillar = null;

            //Tests for adding Torches to b buildings
            var torches = Resources.FindObjectsOfTypeAll<SSSGame.Combat.Torch>();
            Plugin.Log.LogInfo($"Found {torches.Count} Torches");
            foreach (var t in torches)
            {
                Plugin.Log.LogMessage(t.gameObject.name);
            }


                var torch = GameObject.Instantiate(torches[0].gameObject);
            Plugin.Log.LogInfo($"Torch instantiated");
            Component.DestroyImmediate(torch.GetComponent<WeaponizedItemObject>());
            Component.DestroyImmediate(torch.GetComponent<Rigidbody>());
            Component.DestroyImmediate(torch.GetComponent<PooledObject>());
            Component.DestroyImmediate(torch.GetComponent<WearableItemObject>());
            Component.DestroyImmediate(torch.GetComponent<Torch>());
            Component.DestroyImmediate(torch.GetComponent<WeaponTrailSpawner>());
            Component.DestroyImmediate(torch.GetComponent<BoxCollider>());
            Component.DestroyImmediate(torch.GetComponent<PickableVolume>());
            Component.DestroyImmediate(torch.GetComponent<DynamicItemObject>());
            Component.DestroyImmediate(torch.GetComponent<NetworkInteractable>());
            Component.DestroyImmediate(torch.GetComponent<Pickable>());
            Component.DestroyImmediate(torch.GetComponent<PickupInteraction>());
            Component.DestroyImmediate(torch.GetComponent<MultiInteractionProxy>());
            torch.transform.GetChild(1).gameObject.SetActive(true);
            torch.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);

            var cave = Resources.FindObjectsOfTypeAll<SSSGame.CaveTorchOutlet>();

            foreach (var mb in cave)
            {
                if (mb.gameObject.transform.parent.name == "CaveCorridor")
                {
                    Plugin.Log.LogInfo($"Found CaveTorchOutlet in {mb.gameObject?.name} in {mb.gameObject.transform.parent.name}");
                    sourcePillar = GameObject.Instantiate(mb.gameObject.transform.gameObject);
                    Plugin.Log.LogInfo($"Source pillar instantiated");
                }
            }
            
            if (sourcePillar is null) Plugin.Log.LogError("SourcePillar is null");
            var comp = sourcePillar.GetComponent<SSSGame.CaveTorchOutlet>();
            Component.DestroyImmediate(comp);
            Plugin.Log.LogMessage(sourcePillar.transform.GetChild(0).name);
            GameObject.DestroyImmediate(sourcePillar.transform.GetChild(0).gameObject);
            Plugin.Log.LogMessage("Destroying PillarObject");
            Plugin.Log.LogMessage($"New first child is {sourcePillar.transform.GetChild(0).name}");

            

            var Buildings = Resources.FindObjectsOfTypeAll<SSSGame.Structure>();
            foreach (var sb in Buildings)
            {
                switch (sb.gameObject.name)
                {
                    case "ArcheryRange_L1":
                        ArcheryRange_L1(sb, sourcePillar, torch);
                        break;
                    case "ArcheryRange_L2":
                        ArcheryRange_L2(sb, sourcePillar, torch);
                        break;
                    case "Barber_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Barracks_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Barracks_L2":
                        Barracks_L2(sb, sourcePillar, torch);
                        break;
                    case "Bloomery_L1":
                    case "Bloomery_L2":
                    case "Boatbuilder_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "BoatFish":
                        Barracks_L2(sb, sourcePillar, torch);
                        break;
                    case "BuilderHut_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Cave_Entrance":
                    case "Cave_Entrance_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "CookingHouse_T1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "CookingHouse_T2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Farm_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Farm_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Fisherman'sHut_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Fisherman'sHut_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "FlimsyArch":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Forester_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Forester_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Gatherer_L0":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Gatherer_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Gatherer_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "HealingHouse_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "House_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "House_L1_Chieftain":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "House_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Hunter_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Hunter_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "CharcoalMaker_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Market_T1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "OuthouseL1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Outpost_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "OutpostL1_addon_tower":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "StoneCutter_L0":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "StoneCutter_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "StoneCutter_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Structure_Cart":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "TempleOfSol":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "WallGuardtower":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "WallHedgeGate_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "WallHedgeTower_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "WallPlankGate_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "WallWatchtower":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Warehouse_Extension_T1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Warehouse_Extension_T2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Warehouse_T1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Warehouse_T2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "WoodCutter_L0":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "WoodCutter_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "WoodCutter_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Workshop_addon_Carpenter_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Workshop_addon_Metalworker_L2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Workshop_L0":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Workshop_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Workshop_L1_addon_Carpenter_L1":
                        Workshop_L1_addon_Carpenter_L1(sb, sourcePillar, torch);
                        break;
                    case "Workshop_L1_addon_Dyer":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Workshop_L1_addon_Leatherworker_T1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Workshop_L1_addon_Leatherworker_T2":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Workshop_L1_addon_Metalworker_L1":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Workshop_L1_addon_Weaver":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Workshop_L2_addon_Dyer":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Workshop_L2_addon_Weaver":
                        Default(sb, sourcePillar, torch);
                        break;
                    case "Workshop_L2":
                        Workshop_L2(sb, sourcePillar, torch);
                        break;
                    default:
                        break;
                }

            }

            GameObject.DestroyImmediate(sourcePillar);
            GameObject.DestroyImmediate(torch);
        }

        private static void Workshop_L2(Structure targetStructure,GameObject targetPosSource, GameObject torchSource)
        {
            GameObject AskaPlusGO = new GameObject("AskaPlusTorches");
            AskaPlusGO.transform.SetParent(targetStructure.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0), true);
            Plugin.Log.LogInfo($"Trying to add Torches to {targetStructure.gameObject.name}");
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO,new Vector3(-1.87f, 0f, -3.21f),Quaternion.Euler(0f,270f,10f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(2.15f, 0f, 3.26f), Quaternion.Euler(0f, 90f, 10f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(-2.1f, - 0f, 3.2f), Quaternion.Euler(0f, 90f, 10f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(2.38f, 0f ,- 3.17f), Quaternion.Euler(0f, 270f, 10f));
        }
        private static void Workshop_L1_addon_Carpenter_L1(Structure targetStructure, GameObject targetPosSource, GameObject torchSource)
        {
            GameObject AskaPlusGO = new GameObject("AskaPlusTorches");
            AskaPlusGO.transform.SetParent(targetStructure.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(5), true);
            Plugin.Log.LogInfo($"Trying to add Torches to {targetStructure.gameObject.name}");
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(-1.8f, - 0, 1.55f), Quaternion.Euler(0f, 340f, 10f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(2.15f, 0f, 3.26f), Quaternion.Euler(0f, 90f, 10f));
        }
        private static void ArcheryRange_L1(Structure targetStructure, GameObject targetPosSource, GameObject torchSource)
        {
            GameObject AskaPlusGO = new GameObject("AskaPlusTorches");
            AskaPlusGO.transform.SetParent(targetStructure.transform.GetChild(1).GetChild(0).GetChild(2), true);
            Plugin.Log.LogInfo($"Trying to add Torches to {targetStructure.gameObject.name}");
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(2f, 0, 0.39f), Quaternion.Euler(0f, 125f, 10f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(-3.8f, 0f, 0.3f), Quaternion.Euler(0f, 35f, 0f));
        }
        private static void ArcheryRange_L2(Structure targetStructure, GameObject targetPosSource, GameObject torchSource)
        {
            GameObject AskaPlusGO = new GameObject("AskaPlusTorches");
            AskaPlusGO.transform.SetParent(targetStructure.transform.GetChild(1).GetChild(0).GetChild(4), true);
            Plugin.Log.LogInfo($"Trying to add Torches to {targetStructure.gameObject.name}");
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(2.22f, 0f, 0.52f), Quaternion.Euler(0f, 100f, 0f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(-4f, 0f, 0.4f), Quaternion.Euler(0f, 80f, 0f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(1.85f,-1.16f, 8.15f), Quaternion.Euler(0f, 120f, 15f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(-3.66f, - 1, 8.087f), Quaternion.Euler(0f, 60f, 15f));
        }

        private static void Barracks_L2(Structure targetStructure, GameObject targetPosSource, GameObject torchSource)
        {
            GameObject AskaPlusGO = new GameObject("AskaPlusTorches");
            AskaPlusGO.transform.SetParent(targetStructure.transform.GetChild(0).GetChild(0).GetChild(8), true);
            Plugin.Log.LogInfo($"Trying to add Torches to {targetStructure.gameObject.name}");
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(0f, 0f, 02f), Quaternion.Euler(0f, 0f, 10f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 10f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 10f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 10f));
        }

        private static void Default(Structure targetStructure, GameObject targetPosSource, GameObject torchSource)
        {
            return;
            GameObject AskaPlusGO = new GameObject("AskaPlusTorches");
            AskaPlusGO.transform.SetParent(targetStructure.transform.GetChild(0).GetChild(0).GetChild(0), true);
            Plugin.Log.LogInfo($"Trying to add Torches to {targetStructure.gameObject.name}");
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 10f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 10f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 10f));
            AddTorchToStructure(targetPosSource, torchSource, AskaPlusGO, new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 10f));
        }

        private static void AddTorchToStructure(GameObject targetPosSource, GameObject torchSource, GameObject AskaPlusGO,Vector3 pos,Quaternion rot)
        {
            var TorchPositionGO = GameObject.Instantiate(targetPosSource);
            var tor = GameObject.Instantiate(torchSource);
            TorchPositionGO.transform.SetParent(AskaPlusGO.transform, true);
            TorchPositionGO.transform.position = pos;
            TorchPositionGO.transform.rotation = rot;
            tor.transform.SetParent(TorchPositionGO.transform.GetChild(3), false);
        }
    }
}
























