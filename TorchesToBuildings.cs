using HarmonyLib;
using Invector;
using SandSailorStudio.Inventory;
using SandSailorStudio.Pooling;
using SSSGame;
using SSSGame.Combat;
using SSSGame.Network;
using SSSGame.Render;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static askaplus.bepinex.mod.Plugin;
using static askaplus.bepinex.mod.TorchesToBuildings;

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

            if (Plugin.configTorchesBuildingEnable.Value == false) return;


            //Tests for adding Torches to b buildings
            var torches = Resources.FindObjectsOfTypeAll<Torch>();

            var torch = GameObject.Instantiate(torches[0].gameObject);
            Plugin.Log.LogDebug($"Torch instantiated");
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
            var light = torch.transform.FindChildByNameRecursive("Point Light").GetComponent<Light>();
            var hdData = light.gameObject.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalLightData>();

            if (Plugin.configTorchesBuildingShadowsEnable.Value)
            {
                light.shadows = LightShadows.Soft;
            }
            else
            {
                light.shadows = LightShadows.None;
            }
            if (Plugin.configTorchesLightExtended.Value)
            {
                hdData.fadeDistance = 200f;
            }
            else
            {
                hdData.fadeDistance = 60f;
            }
            torch.transform.GetChild(1).gameObject.SetActive(true);
            torch.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);

            var cave = Resources.FindObjectsOfTypeAll<CaveTorchOutlet>();

            foreach (var mb in cave)
            {
                if (mb.gameObject?.transform?.parent?.name == "CaveCorridor")
                {
                    Plugin.Log.LogDebug($"Found CaveTorchOutlet in {mb.gameObject.name} in {mb.gameObject.transform.parent.name}");
                    Plugin.Log.LogError($"Ignore next NullObjectError. It is perfectly OK and cannot be avoided. Yet. Or Forever.");
                    sourcePillar = GameObject.Instantiate(mb.gameObject.transform.gameObject);
                    Plugin.Log.LogDebug($"Source pillar instantiated");
                }
            }

            if (sourcePillar is null) { Plugin.Log.LogError("SourcePillar is null"); return; }
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
            Plugin.Log.LogDebug(sourcePillar.transform.GetChild(0).name);
            GameObject.DestroyImmediate(sourcePillar.transform.GetChild(0).gameObject);
            Plugin.Log.LogDebug("Destroying PillarObject");
            Plugin.Log.LogDebug($"New first child is {sourcePillar.transform.GetChild(0).name}");

            var Buildings = Resources.FindObjectsOfTypeAll<SSSGame.Structure>();
            var Carts = Resources.FindObjectsOfTypeAll<CartStructure>();
            foreach (var sb in Buildings)
            {
                System.Collections.Generic.List<PosRot> posRots = [];
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
                        posRots.Add(new PosRot(new Vector3(-2.3202f, 0f, 0f), Quaternion.Euler(0f, 180f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "barber_complete_2_0_roof", posRots);
                        break;
                    case "Armorsmith_L1":
                        posRots.Add(new PosRot(new Vector3(-2.3202f, 0f, 0f), Quaternion.Euler(0f, 180f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "armorsmith_complete_2_0_roof", posRots);
                        break;
                    case "Armorsmith_L2":
                        posRots.Add(new PosRot(new Vector3(-0.55f, -0.4347f, 6.7166f), Quaternion.Euler(0f, 45f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "armorsmith_complete_1_0_walls", posRots);
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
                        posRots.Add(new PosRot(new Vector3(1.5936f, 0.2816f, 1.8875f), Quaternion.Euler(0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "wall_t1_D_1_exterior_complete_0_0", posRots);
                        break;
                    case "Building2x1":
                        posRots.Add(new PosRot(new Vector3(1.8194f, 0.5035f, 4.368f), Quaternion.Euler(0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "wall_t1_D_1_exterior_complete_0_0", posRots);
                        break;
                    case "Cave_Entrance_L2":
                        posRots.Add(new PosRot(new Vector3(-2.12f, -0.6073f, 3.1109f), Quaternion.Euler(0f, 125f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "stone_storage_roof_complete_0_0_structure", posRots);
                        break;
                    case "CookingHouse_T1":
                        posRots.Add(new PosRot(new Vector3(-1.404f, -0f, 1.1311f), Quaternion.Euler(0f, 45f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "CookingHouse_T2":
                        posRots.Add(new PosRot(new Vector3(-2.155f, -0.2313f, 0.8692f), Quaternion.Euler(0f, 90f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_1_0_structure", posRots);
                        break;
                    case "Farm_L1":
                        posRots.Add(new PosRot(new Vector3(0.9836f, -0.5382f, 0.7018f), Quaternion.Euler(0f, 90f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Farm_L2":
                        posRots.Add(new PosRot(new Vector3(1.0309f, -0.8218f, 0.7909f), Quaternion.Euler(0f, 90f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "farm_complete_2_0_roof", posRots);
                        break;
                    case "Fisherman'sHut_L1":
                        posRots.Add(new PosRot(new Vector3(0.9857f, -0.0692f, 5.8965f), Quaternion.Euler(0f, 180f, 350f)));
                        AddTorches(sb, sourcePillar, torch, "fishermanhut_L1_C_complete_0_0", posRots);
                        break;
                    case "Fisherman'sHut_L2":
                        posRots.Add(new PosRot(new Vector3(-2.0497f, 2.5395f, 5.9878f), Quaternion.Euler(0f, 180f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "fisherman_l2_complete_0_0", posRots);
                        break;
                    case "FlimsyArch":
                        posRots.Add(new PosRot(new Vector3(-0.5873f, -0.8292f, -0.1091f), Quaternion.Euler(0f, 0f, 10f)));
                        posRots.Add(new PosRot(new Vector3(0.5945f, -0.8293f, 0.0782f), Quaternion.Euler(0f, 180f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "flimsy_arch_complete_0_0_foundation", posRots);
                        break;
                    case "Forester_L1":
                        posRots.Add(new PosRot(new Vector3(0.9182f, -0.7618f, -3.8436f), Quaternion.Euler(0f, 90f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "roof_complete_2_0_roof", posRots);
                        break;
                    case "Forester_L2":
                        posRots.Add(new PosRot(new Vector3(0.0618f, 0f, -3.26f), Quaternion.Euler(0f, 180f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "house_complete_1_0_house", posRots);
                        break;
                    case "Gatherer_L1":
                        posRots.Add(new PosRot(new Vector3(-1.3055f, -0.6673f, -0.5435f), Quaternion.Euler(0f, 90f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "gatherer1_complete_2_0_roof", posRots);
                        break;
                    case "Gatherer_L2":
                        posRots.Add(new PosRot(new Vector3(-0.1564f, -0.5928f, 0.2984f), Quaternion.Euler(0f, 90f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "HealingHouse_L1":
                        posRots.Add(new PosRot(new Vector3(-1.6927f, 0f, -1.7145f), Quaternion.Euler(0f, 290f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "healing_house_l1_complete_1_0_walls", posRots);
                        break;
                    case "HealingHouse_L2":
                        posRots.Add(new PosRot(new Vector3(-1.6927f, 0f, -1.7145f), Quaternion.Euler(0f, 290f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_1_0_walls", posRots);
                        break;
                    case "House_L1":
                        posRots.Add(new PosRot(new Vector3(1.2573f, 0f, 3.8631f), Quaternion.Euler(-0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "house1_complete_1_0_frame", posRots);
                        break;
                    case "House_L1_Chieftain":
                        posRots.Add(new PosRot(new Vector3(-0.55f, -0.4347f, 6.7166f), Quaternion.Euler(0f, 45f, 10f)));
                        posRots.Add(new PosRot(new Vector3(0.5936f, -0.3456f, 6.8236f), Quaternion.Euler(-0f, 135f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "house1_complete_1_0_frame", posRots);
                        break;
                    case "House_L2_Chieftain":
                        posRots.Add(new PosRot(new Vector3(-0.55f, -0.4347f, 6.7166f), Quaternion.Euler(0f, 45f, 10f)));
                        posRots.Add(new PosRot(new Vector3(0.5936f, -0.3456f, 6.8236f), Quaternion.Euler(-0f, 135f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_1_0_frame", posRots);
                        break;

                    case "House_L2":
                        posRots.Add(new PosRot(new Vector3(1.8194f, 0.5035f, 4.368f), Quaternion.Euler(0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "wall_t1_D_1_exterior_complete_0_0", posRots);
                        break;
                    case "Hunter_L1":
                        posRots.Add(new PosRot(new Vector3(-0.8018f, 0.751f, 1f), Quaternion.Euler(0f, 45f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "hunterL1_complete_2_2", posRots);
                        break;
                    case "Hunter_L2":
                        posRots.Add(new PosRot(new Vector3(0.8201f, 2.1738f, 0.877f), Quaternion.Euler(0f, 120f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Market_T1":
                        posRots.Add(new PosRot(new Vector3(0.4814f, 0f, 2.2372f), Quaternion.Euler(0f, 90f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "trading_post_l0_complete_1_0_frame", posRots);
                        break;
                    case "Market_T2":
                        posRots.Add(new PosRot(new Vector3(0.4814f, 0f, 2.2372f), Quaternion.Euler(0f, 90f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_1_0_walls", posRots);
                        break;
                    case "OuthouseL1":
                        posRots.Add(new PosRot(new Vector3(1.1673f, -1.0254f, 1.6219f), Quaternion.Euler(0f, 95f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "outhouse_complete_0_0", posRots);
                        break;
                    case "Outpost_L1_addon_wall2":
                        posRots.Add(new PosRot(new Vector3(-2.3664f, 0f, -4.47f), Quaternion.Euler(0f, 270f, 10f)));
                        posRots.Add(new PosRot(new Vector3(2.5264f, 0f, 4.4164f), Quaternion.Euler(0f, 90f, 10f)));
                        posRots.Add(new PosRot(new Vector3(-4.4982f, 0f, 2.6182f), Quaternion.Euler(0f, 0f, 10f)));
                        posRots.Add(new PosRot(new Vector3(4.4491f, 0f, -2.5591f), Quaternion.Euler(0f, 180f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "addon_wall2_complete_0_0", posRots);
                        break;
                    case "OutpostL1_addon_tower":
                        posRots.Add(new PosRot(new Vector3(-0.1091f, 3.9818f, 0.4018f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "addon_tower_complete_0_0", posRots);
                        break;
                    case "StoneCutter_L2":
                        posRots.Add(new PosRot(new Vector3(1.4255f, 0f, -2.0614f), Quaternion.Euler(0f, 90f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "TempleOfSol":
                        break;
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "a", posRots);
                        break;
                    case "WallGuardtower":
                        posRots.Add(new PosRot(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "tower_guardTower_complete_0_0", posRots);
                        break;
                    case "WallGuardtowerRoof":
                        posRots.Add(new PosRot(new Vector3(1.7782f, 5.7518f, 1.9091f), Quaternion.Euler(-0f, 315f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "tower_guardTower_roof_complete_0_0", posRots);
                        break;
                    case "WallHedgeGate_L1":
                        posRots.Add(new PosRot(new Vector3(-2.0855f, 0.8164f, 0.8255f), Quaternion.Euler(0f, 270f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "gatehedge_complete_0_0", posRots);
                        break;
                    case "WallHedgeTower_L1":
                        posRots.Add(new PosRot(new Vector3(-0.0709f, -0.2655f, 1.3236f), Quaternion.Euler(0f, 90f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "hedgetower_complete_0_0_frame", posRots);
                        break;
                    case "WallPlankGate_L2":
                        posRots.Add(new PosRot(new Vector3(1.8727f, 0.5109f, 0.8455f), Quaternion.Euler(0f, 240f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "gate_plank_L2_complete_0_0", posRots);
                        break;

                    case "WallPlankL2Section_SkewDown30":
                        posRots.Add(new PosRot(new Vector3(2.25f, 0f, -0.58f), Quaternion.Euler(0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;

                    case "WallPlankL2Section_SkewUp30":
                        posRots.Add(new PosRot(new Vector3(2.25f, 0f, -0.58f), Quaternion.Euler(0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;

                    case "WallPlankL2Section":
                        posRots.Add(new PosRot(new Vector3(2.25f, 0f, -0.58f), Quaternion.Euler(0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;

                    case "WallPlankSectionShortL2":
                        posRots.Add(new PosRot(new Vector3(1.22f, 0f, -0.49f), Quaternion.Euler(0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0_structure", posRots);
                        break;

                    case "WallPlankSectionShortL2_SkewDown30":
                        posRots.Add(new PosRot(new Vector3(1.22f, 0f, -0.49f), Quaternion.Euler(0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0_structure", posRots);
                        break;

                    case "WallPlankSectionShortL2_SkewUp30":
                        posRots.Add(new PosRot(new Vector3(1.22f, 0f, -0.49f), Quaternion.Euler(0f, 90f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0_structure", posRots);
                        break;

                    case "WallWatchtowerRoof":
                        posRots.Add(new PosRot(new Vector3(0.9473f, 6.16f, 2.1091f), Quaternion.Euler(0f, 270f, 20f)));
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
                        posRots.Add(new PosRot(new Vector3(1.1727f, -0.3767f, 0.5431f), Quaternion.Euler(0f, 90f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Workshop_addon_Carpenter_L2":
                        posRots.Add(new PosRot(new Vector3(0.909f, 0f, -3.2564f), Quaternion.Euler(0f, 180f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_1_0_frame", posRots);
                        break;
                    case "Workshop_addon_Metalworker_L2":
                        posRots.Add(new PosRot(new Vector3(-2.5964f, -0.6636f, -1.7255f), Quaternion.Euler(0f, 90f, 20f)));
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
                        posRots.Add(new PosRot(new Vector3(2.7822f, -0.2164f, -0.0236f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "workshop_t1_addon_dyer_complete_0_0", posRots);
                        break;
                    case "Workshop_L1_addon_Leatherworker_T1":
                        posRots.Add(new PosRot(new Vector3(-0.0709f, -0.1345f, -2.2601f), Quaternion.Euler(0f, 180f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Workshop_L1_addon_Leatherworker_T2":
                        posRots.Add(new PosRot(new Vector3(-0.0891f, -0.1855f, -2.2873f), Quaternion.Euler(0f, 180f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "leatherworker_complete_0_0_foundation", posRots);
                        break;
                    case "Workshop_L1_addon_Metalworker_L1":
                        posRots.Add(new PosRot(new Vector3(0.8236f, -0f, 2.7818f), Quaternion.Euler(0f, -45f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Workshop_L1_addon_Weaver":
                        posRots.Add(new PosRot(new Vector3(-1.1003f, 0f, -2.353f), Quaternion.Euler(0f, -20f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "workshop_t1_addonB_complete_0_0", posRots);
                        break;
                    case "Workshop_L2_addon_Dyer":
                        posRots.Add(new PosRot(new Vector3(-1.902f, -0.5494f, -2.448f), Quaternion.Euler(0f, 90f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Workshop_L2_addon_Weaver":
                        posRots.Add(new PosRot(new Vector3(-0.9147f, -0f, -2.8752f), Quaternion.Euler(0f, 0f, 20f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    case "Workshop_L2":

                        posRots.Add(new PosRot(new Vector3(-1.87f, 0f, -3.21f), Quaternion.Euler(0f, 270f, 10f)));
                        posRots.Add(new PosRot(new Vector3(2.15f, 0f, 3.26f), Quaternion.Euler(0f, 90f, 10f)));
                        posRots.Add(new PosRot(new Vector3(-2.1f, -0f, 3.2f), Quaternion.Euler(0f, 90f, 10f)));
                        posRots.Add(new PosRot(new Vector3(2.38f, 0f, -3.17f), Quaternion.Euler(0f, 270f, 10f)));
                        AddTorches(sb, sourcePillar, torch, "structure_complete_0_0", posRots);
                        break;
                    default:
                        //                        Plugin.Log.LogInfo($"Torshes not added to {sb.gameObject.name}");
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
                        //Plugin.Log.LogInfo($"Torshes not added to {cart.gameObject.name}");
                        break;
                }
            }

            GameObject.DestroyImmediate(sourcePillar);
            GameObject.DestroyImmediate(torch);
        }


        private static void AddTorches(Structure targetStructure, GameObject targetPosSource, GameObject torchSource, string GOname, System.Collections.Generic.List<PosRot> posRots)
        {
            //Plugin.Log.LogInfo($"Trying to add Torches to {targetStructure.gameObject.name}");

            var transf = targetStructure.transform.FindChildByNameRecursive(GOname);
            if (transf is null) return;

            GameObject AskaPlusGO = new("AskaPlusTorches");
            AskaPlusGO.transform.SetParent(transf, true);

            foreach (var pr in posRots)
            {
                AddTorch(targetPosSource, torchSource, AskaPlusGO, pr.pos, pr.rot);
            }
        }

        private static void Structure_Cart(Structure targetStructure, GameObject targetPosSource, GameObject torchSource, string GOname)
        {
            //  Plugin.Log.LogInfo($"Trying to add Torches to {targetStructure.gameObject.name}");
            var transf = targetStructure.transform.FindChildByNameRecursive(GOname);
            if (transf is null) return; GameObject AskaPlusGO = new GameObject("AskaPlusTorches");
            AskaPlusGO.transform.SetParent(transf, true);

            AddTorch(targetPosSource, torchSource, AskaPlusGO, new Vector3(0.3545f, -0.0745f, 0.9327f), Quaternion.Euler(-0, 125, 310));
            AskaPlusGO.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            AskaPlusGO.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            AskaPlusGO.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        }

        private static void AddTorch(GameObject extractrdPillarGO, GameObject extractedTorchGO, GameObject AskaPlusGO, Vector3 pos, Quaternion rot)
        {
            var TorchPositionGO = GameObject.Instantiate(extractrdPillarGO);
            var tor = GameObject.Instantiate(extractedTorchGO);
            TorchPositionGO.transform.SetParent(AskaPlusGO.transform, true);
            TorchPositionGO.transform.position = pos;
            TorchPositionGO.transform.rotation = rot;
            tor.transform.SetParent(TorchPositionGO.transform.GetChild(3), false);
        }
        public static void OnSettingsMenu(Transform parent)
        {
            Helpers.CreateCategory(parent, "Torches to buildings");
            Helpers.CreateSwitch(parent, "* Enable Mod", configTorchesBuildingEnable);
            Helpers.CreateSwitch(parent, "* Enable shadows", configTorchesBuildingShadowsEnable);
            Helpers.CreateSwitch(parent, "* Light extended visibility", configTorchesLightExtended);
            UnityAction applyCallback = (UnityAction)(() =>
            {
                Plugin.configGrassPaintKey.Value = KeyCode.Z;
            });
        }
        internal struct PosRot(Vector3 _pos, Quaternion _rot)
        {
            internal Vector3 pos = _pos;
            internal Quaternion rot = _rot;
        }
    }
    [HarmonyPatch(typeof(Structure))]
    internal class StrucutrePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Structure.Spawned))]
        public static void StructureBuild(Structure __instance)
        {
            switch (__instance.name)
            {
                case "WallPlankL2Section_SkewDown30(Clone)":
                case "WallPlankL2Section_SkewUp30(Clone)":
                case "WallPlankL2Section(Clone)":
                case "WallPlankSectionShortL2_SkewUp30(Clone)":
                case "WallPlankSectionShortL2_SkewDown30(Clone)":
                case "WallPlankSectionShortL2(Clone)":
                    Vector2 position = new Vector2(__instance.transform.position.x, __instance.transform.position.y);
                    float noiseValue = Mathf.PerlinNoise(position.x*10, position.y*10);

                    Transform askaplustorches = __instance.gameObject.transform.FindChildByNameRecursive("AskaPlusTorches");
                    askaplustorches?.gameObject.SetActive(noiseValue<0.3);
                    Plugin.Log.LogMessage($"Structure {__instance.name} spawned. Perlin value is {noiseValue}");
                    break;
                default:
                    break;
            }
           
        }

    }
}
























