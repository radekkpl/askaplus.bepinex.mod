using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using Invector;

//using Invector;
using SandSailorStudio.Inventory;
using SandSailorStudio.Pooling;
using SSSGame;
using SSSGame.Combat;
using SSSGame.Network;
using SSSGame.Render;
using UnityEngine;

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
            var torches = Resources.FindObjectsOfTypeAll<Torch>();
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
            Component.DestroyImmediate(torch.transform.GetChild(1).GetChild(2).gameObject);
            torch.transform.GetChild(1).gameObject.SetActive(true);
            torch.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);

            var cave = Resources.FindObjectsOfTypeAll<CaveTorchOutlet>();

            foreach (var mb in cave)
            {
                if (mb.gameObject.transform.parent.name == "CaveCorridor")
                {
                    Plugin.Log.LogInfo($"Found CaveTorchOutlet in {mb.gameObject?.name} in {mb.gameObject.transform.parent.name}");
                    Plugin.Log.LogError($"Ignore next NullObjectError. It is perfectly OK and cannot be avoided. Yet. Or Forever.");
                    sourcePillar = GameObject.Instantiate(mb.gameObject.transform.gameObject);
                    Plugin.Log.LogInfo($"Source pillar instantiated");
                }
            }

            if (sourcePillar is null) Plugin.Log.LogError("SourcePillar is null");
            Component.DestroyImmediate(sourcePillar.GetComponent<CaveTorchOutlet>());
            Component.DestroyImmediate(sourcePillar.GetComponentInChildren<EquipmentDisplaySlot>());
            Component.DestroyImmediate(sourcePillar.GetComponentInChildren<AnimatorEventPlaySounds>());
            Component.DestroyImmediate(sourcePillar.GetComponentInChildren<Pickable>());
            Component.DestroyImmediate(sourcePillar.GetComponentInChildren<Rigidbody>());
            Component.DestroyImmediate(sourcePillar.GetComponentInChildren<CavePillarInteraction>());
            Component.DestroyImmediate(sourcePillar.GetComponentInChildren<NetworkInteractable>());
            Component.DestroyImmediate(sourcePillar.GetComponentInChildren<EquipmentManager>());
            Component.DestroyImmediate(sourcePillar.GetComponentInChildren<StandaloneInteractionArea>());
            foreach (var com in sourcePillar.GetComponentsInChildren<StorageInteraction>()) 
            {
                Component.DestroyImmediate(com);
            }
            Plugin.Log.LogMessage(sourcePillar.transform.GetChild(0).name);
            GameObject.DestroyImmediate(sourcePillar.transform.GetChild(0).gameObject);
            Plugin.Log.LogMessage("Destroying PillarObject");
            Plugin.Log.LogMessage($"New first child is {sourcePillar.transform.GetChild(0).name}");

            

            var Buildings = Resources.FindObjectsOfTypeAll<SSSGame.Structure>();
            var Carts = Resources.FindObjectsOfTypeAll<CartStructure>();
            foreach (var sb in Buildings)
            {
                System.Collections.Generic.List<PosRot> posRots = new();
                switch (sb.gameObject.name)
                {
                    case "ArcheryRange_L1":
                        
                        posRots.Add(new PosRot(new Vector3(2f, 0, 0.39f), Quaternion.Euler(0f, 125f, 10f)));
                        posRots.Add(new PosRot(new Vector3(-3.8f, 0f, 0.3f), Quaternion.Euler(0f, 35f, 0f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_2_0_roof", posRots);
                        break;
                    case "ArcheryRange_L2":
                       
                        posRots.Add(new PosRot(new Vector3(2.22f, 0f, 0.52f), Quaternion.Euler(0f, 100f, 0f)));
                        posRots.Add(new PosRot(new Vector3(-4f, 0f, 0.4f), Quaternion.Euler(0f, 80f, 0f)));
                        posRots.Add(new PosRot(new Vector3(1.85f, -1.16f, 8.15f), Quaternion.Euler(0f, 120f, 15f)));
                        posRots.Add(new PosRot(new Vector3(-3.66f, -1, 8.087f), Quaternion.Euler(0f, 60f, 15f)));
                        AddTorches(sb, sourcePillar, torch, "archery_complete_3_0_roof", posRots);
                        break;
                    case "Barber_L2":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "barber_complete_2_0_roof", posRots);
                        break;
                    case "Barracks_L1":
                       
                        posRots.Add(new PosRot(new Vector3(2.7526f, -0.1073f, 2.6436f), Quaternion.Euler(-0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "barracks_l1_complete_2_0_roof", posRots);
                        break;
                    case "Barracks_L2":
                      
                        posRots.Add(new PosRot(new Vector3(3.0235f, -0.622f, 2.9054f), Quaternion.Euler(-0f, 90f, 10f)));
                        posRots.Add(new PosRot(new Vector3(-4.3547f, -0.622f, 2.8545f), Quaternion.Euler(-0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_1_0_structure", posRots);
                        break;
                    case "Bloomery_L2":
                        posRots.Add(new PosRot(new Vector3(-1.5636f, -0.52f, 2.0255f), Quaternion.Euler(-0f, 45f, 15f)));
                        AddTorches(sb, sourcePillar, torch, "bloomery_complete_2_0_roof", posRots);
                        break;
                    case "Boatbuilder_L2":
                       
                        posRots.Add(new PosRot(new Vector3(-1.2528f, 0.6255f, -0.9836f), Quaternion.Euler(0f, 224.6845f, 350f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "BoatFish":
                         
                        posRots.Add(new PosRot(new Vector3(-0.0818f, -0.7002f, 3.9345f), Quaternion.Euler(-0f, 90f, 350f)));
                        AddTorches(sb, sourcePillar, torch, "boat_part_05_floor", posRots);
                        break;
                    case "BuilderHut_L1":
                         
                        posRots.Add(new PosRot(new Vector3(0.7527f, -0.2f, 0.3489f), Quaternion.Euler(0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0_frame", posRots);
                        break;
                    case "Building1x1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "wall_t1_D_1_exterior_complete_0_0", posRots);
                        break;
                    case "Building2x1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "wall_t1_D_1_exterior_complete_0_0", posRots);
                        break;
                    case "Cave_Entrance_L2":
                         
                        posRots.Add(new PosRot(new Vector3(-2.12f, -0.6073f, 3.1109f), Quaternion.Euler(0f, 125f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "stone_storage_roof_complete_0_0_structure", posRots);
                        break;
                    case "CookingHouse_T1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                       AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "CookingHouse_T2":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_1_0_structure", posRots);
                        break;
                    case "Farm_L1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Farm_L2":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "farm_complete_2_0_roof", posRots);
                        break;
                    case "Fisherman'sHut_L1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "fishermanhut_L1_C_complete_0_0", posRots);
                        break;
                    case "Fisherman'sHut_L2":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "fisherman_l2_complete_0_0", posRots);
                        break;
                    case "FlimsyArch":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "flimsy_arch_complete_0_0_foundation", posRots);
                        break;
                    case "Forester_L1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "roof_complete_2_0_roof", posRots);
                        break;
                    case "Forester_L2":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "house_complete_1_0_house", posRots);
                        break;
                    case "Gatherer_L1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "gatherer1_complete_2_0_roof", posRots);
                        break;
                    case "Gatherer_L2":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "HealingHouse_L1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "healing_house_l1_complete_1_0_walls", posRots);
                        break;
                    case "House_L1":

                        posRots.Add(new PosRot(new Vector3(1.2573f, -0.004f, 3.8631f), Quaternion.Euler(-0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "house1_complete_1_0_frame", posRots);
                        break;
                    case "House_L1_Chieftain":
                         
                        posRots.Add(new PosRot(new Vector3(-0.55f, -0.4347f, 6.7166f), Quaternion.Euler(0f, 45f, 10f)));
                        posRots.Add(new PosRot(new Vector3(0.5936f, -0.3456f, 6.8236f), Quaternion.Euler(-0f, 135f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "house1_complete_1_0_frame", posRots);
                        break;
                    case "House_L2":
                         
                        posRots.Add(new PosRot(new Vector3(-1.87f, 0f, -3.21f), Quaternion.Euler(0f, 270f, 10f)));
                        posRots.Add(new PosRot(new Vector3(2.15f, 0f, 3.26f), Quaternion.Euler(0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "wall_t1_D_1_exterior_complete_0_0", posRots);
                        break;
                    case "Hunter_L1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "hunterL1_complete_2_2", posRots);
                        break;
                    case "Hunter_L2":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Market_T1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "trading_post_l0_complete_1_0_frame", posRots);
                        break;
                    case "OuthouseL1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "outhouse_complete_0_0", posRots);
                        break;
                    case "Outpost_L1_addon_wall2":
                         
                        posRots.Add(new PosRot(new Vector3(-1.87f, 0f, -3.21f), Quaternion.Euler(0f, 270f, 10f)));
                        posRots.Add(new PosRot(new Vector3(2.15f, 0f, 3.26f), Quaternion.Euler(0f, 90f, 10f)));
                        posRots.Add(new PosRot(new Vector3(-2.1f, -0f, 3.2f), Quaternion.Euler(0f, 90f, 10f)));
                        posRots.Add(new PosRot(new Vector3(2.38f, 0f, -3.17f), Quaternion.Euler(0f, 270f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "addon_wall2_complete_0_0", posRots);
                        break;
                    case "OutpostL1_addon_tower":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "addon_tower_complete_0_0", posRots);
                        break;
                    case "StoneCutter_L2":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "TempleOfSol":
                        break;
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch,"a",posRots);
                        break;
                    case "WallGuardtowerRoof":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "tower_guardTower_roof_complete_0_0", posRots);
                        break;
                    case "WallHedgeGate_L1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "gatehedge_complete_0_0", posRots);
                        break;
                    case "WallHedgeTower_L1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "hedgetower_complete_0_0_frame", posRots);
                        break;
                    case "WallPlankGate_L2":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "gate_plank_L2_complete_0_0", posRots);
                        break;
                    case "WallWatchtowerRoof":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "tower_watchtower_roof", posRots);
                        break;
                    case "Warehouse_L0_Addon_Roof":
                         
                        posRots.Add(new PosRot(new Vector3(0.0247f, 0f, 1.1173f), Quaternion.Euler(0f, 270f, 10f)));
                        posRots.Add(new PosRot(new Vector3(3.9037f, -0.3004f, -2.4455f), Quaternion.Euler(-0f, 270f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "structure_addon_complete_0_0", posRots);
                        break;
                    case "Warehouse_T2_Addon_Roof":
                         
                        posRots.Add(new PosRot(new Vector3(-3.7082f, -0f, -1.4827f), Quaternion.Euler(0f, 180f, 20f)));
                        posRots.Add(new PosRot(new Vector3(4.3809f, -0f, -1.7455f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "addon_complete_0_0_frame", posRots);
                        break;
                    case "WoodCutter_L2":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Workshop_addon_Carpenter_L2":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_1_0_frame", posRots);
                        break;
                    case "Workshop_addon_Metalworker_L2":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "metalworker_complete_1_0_frame", posRots);
                        break;
                    case "Workshop_L1":
                         
                        posRots.Add(new PosRot(new Vector3(-1.87f, 0f, -3.21f), Quaternion.Euler(0f, 270f, 10f)));
                        posRots.Add(new PosRot(new Vector3(2.15f, 0f, 3.26f), Quaternion.Euler(0f, 90f, 10f)));
                        posRots.Add(new PosRot(new Vector3(-2.1f, -0f, 3.2f), Quaternion.Euler(0f, 90f, 10f)));
                        posRots.Add(new PosRot(new Vector3(2.38f, 0f, -3.17f), Quaternion.Euler(0f, 270f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "workshop_t1_complete_0_0", posRots);
                        break;
                    case "Workshop_L1_addon_Carpenter_L1":
                         
                        posRots.Add(new PosRot(new Vector3(-1.8f, -0, 1.55f), Quaternion.Euler(0f, 340f, 10f)));
                        posRots.Add(new PosRot(new Vector3(1.166f, -0f, 1.5f), Quaternion.Euler(0f, 180f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Workshop_L1_addon_Dyer":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "workshop_t1_addon_dyer_complete_0_0", posRots);
                        break;
                    case "Workshop_L1_addon_Leatherworker_T1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Workshop_L1_addon_Leatherworker_T2":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                       AddTorches(sb, sourcePillar, torch, "leatherworker_complete_0_0_foundation", posRots);
                        break;
                    case "Workshop_L1_addon_Metalworker_L1":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Workshop_L1_addon_Weaver":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "workshop_t1_addonB_complete_0_0", posRots);
                        break;
                    case "Workshop_L2_addon_Dyer":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Workshop_L2_addon_Weaver":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Workshop_L2":
          
                        posRots.Add(new PosRot( new Vector3(-1.87f, 0f, -3.21f), Quaternion.Euler(0f, 270f, 10f)));
                        posRots.Add(new PosRot(new Vector3(2.15f, 0f, 3.26f), Quaternion.Euler(0f, 90f, 10f)));
                        posRots.Add(new PosRot(new Vector3(-2.1f, -0f, 3.2f), Quaternion.Euler(0f, 90f, 10f)));
                        posRots.Add(new PosRot(new Vector3(2.38f, 0f, -3.17f), Quaternion.Euler(0f, 270f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    default:
                        Plugin.Log.LogInfo($"Torshes not added to {sb.gameObject.name}");
                        break;
                }
            }

            foreach (var cart in Carts)
            {
                switch (cart.gameObject.name)
                {
                    case "Structure_Cart":
                        Structure_Cart(cart, sourcePillar, torch, "Appearance");
                        break;
                    default:
                        Plugin.Log.LogInfo($"Torshes not added to {cart.gameObject.name}");
                        break;
                }
            }

            GameObject.DestroyImmediate(sourcePillar);
            GameObject.DestroyImmediate(torch);
        }

       
        private static void AddTorches(Structure targetStructure, GameObject targetPosSource, GameObject torchSource, string GOname, System.Collections.Generic.List<PosRot> posRots)
        {
            Plugin.Log.LogInfo($"Trying to add Torches to {targetStructure.gameObject.name}");

            var transf = targetStructure.transform.FindChildByNameRecursive(GOname);
            if (transf is null ) return;

            GameObject AskaPlusGO = new("AskaPlusTorches");
            AskaPlusGO.transform.SetParent(transf, true);

            foreach (var pr in posRots)
            {
                AddTorch(targetPosSource, torchSource, AskaPlusGO, pr.pos, pr.rot);
            }
        }
        
        private static void Structure_Cart(Structure targetStructure, GameObject targetPosSource, GameObject torchSource,string GOname)
        {
            Plugin.Log.LogInfo($"Trying to add Torches to {targetStructure.gameObject.name}");
            var transf = targetStructure.transform.FindChildByNameRecursive(GOname);
            if (transf is null) return; GameObject AskaPlusGO = new GameObject("AskaPlusTorches");
            AskaPlusGO.transform.SetParent(transf, true);
           
            AddTorch(targetPosSource, torchSource, AskaPlusGO, new Vector3(0.3545f, - 0.0745f, 0.9327f), Quaternion.Euler(-0, 125, 310));
            AskaPlusGO.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            AskaPlusGO.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            AskaPlusGO.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        }

        private static void AddTorch(GameObject extractrdPillarGO, GameObject extractedTorchGO, GameObject AskaPlusGO,Vector3 pos,Quaternion rot)
        {
            var TorchPositionGO = GameObject.Instantiate(extractrdPillarGO);
            var tor = GameObject.Instantiate(extractedTorchGO);
            TorchPositionGO.transform.SetParent(AskaPlusGO.transform, true);
            TorchPositionGO.transform.position = pos;
            TorchPositionGO.transform.rotation = rot;
            tor.transform.SetParent(TorchPositionGO.transform.GetChild(3), false);
        }
        internal struct PosRot(Vector3 _pos,Quaternion _rot)
        {
            internal Vector3 pos = _pos;
            internal Quaternion rot = _rot;
        }
    }
}
























