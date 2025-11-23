using HarmonyLib;
using SSSGame;
using UnityEngine;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class AnchorsFix
    {
        private static bool patched = false;
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MainMenu.OnActivate))]
        public static void PostAwake(MainMenu __instance)
        {
            if (patched) return;
            patched = true;

            var x = Resources.FindObjectsOfTypeAll<SSSGame.Anchor>();
            string name = string.Empty;
            Vector3 posA = Vector3.zero;

            foreach (var mb in x)
            {
                //Plugin.Log.LogInfo($"----Anchor found -----");
                var goparent = mb.gameObject;
                BoxCollider coll = null;

                if (goparent is null) continue;

                //  Plugin.Log.LogInfo($"In GameObject {goparent.name}");
                name = goparent.name;
                var gologic = goparent?.transform.parent;
                if (gologic is null) continue;

                coll = gologic.transform.FindChild("Footprint")?.GetComponentInChildren<BoxCollider>();
                //    Plugin.Log.LogInfo($"Looking for parent: {gologic.name}");
                name = gologic.name;
                var gopreview = gologic?.transform.parent;
                if (gopreview is not null)
                {
                    //      Plugin.Log.LogInfo($"Looking for parent: {gopreview.name}");
                    name = gopreview.name;
                    var goMaster = gopreview?.transform.parent;
                    if (goMaster is not null)
                    {
                        // Plugin.Log.LogInfo($"Parent propably found with name: {goMaster.name}");
                        name = goMaster.name;
                    }
                }

                Plugin.Log.LogMessage($"Found Anchor in GO {name} with value {mb.offset}");

               
                if (name.Contains("WallHedgePillar") || name.Contains("Cave") || name.Contains("WaterWell"))
                {
                    mb.offset = 0;
                    Plugin.Log.LogMessage($"New offset is {mb.offset}");
                }
                else if(name.StartsWith("WallHedge") || name.StartsWith("WallPlank"))
                {
                    mb.offset = 0.1f;
                    Plugin.Log.LogMessage($"New offset is {mb.offset}");
                    if (coll != null)
                    {
                        //Plugin.Log.LogInfo($"Collider update in GO {name} with x:{coll.size.x}, y:{coll.size.y}, z: {coll.size.z}");
                        var size = coll.size;
                        size.x -= .15f;
                        if(coll.size.z >= 1) size.z -= .15f;
                        coll.size = size;
                        //Plugin.Log.LogInfo($"New Collider size {name} is x:{coll.size.x}, y:{coll.size.y}, z: {coll.size.z}");
                    }
                }
            }
        }
    }
}
