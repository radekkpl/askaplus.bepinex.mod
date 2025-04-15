using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Numerics;
using SSSGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace askaplus.bepinex.mod
{
    [HarmonyPatch(typeof(MainMenu))]
    internal class AnchorsFix
    {

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MainMenu.OnActivate))]
        public static void PostAwake(MainMenu __instance)
        {
            var x = Resources.FindObjectsOfTypeAll<SSSGame.Anchor>();
            string name = string.Empty;
            Vector3 posA = Vector3.zero;

            foreach (var mb in x)
            {

                //Plugin.Log.LogInfo($"----Anchor found -----");
                var goparent = mb.gameObject;
                BoxCollider coll = null;
                if (goparent is not null)
                {
                    //  Plugin.Log.LogInfo($"In GameObject {goparent.name}");
                    name = goparent.name;
                    var gologic = goparent?.transform.parent;
                    if (gologic is not null)
                    {
                        coll = gologic.transform.FindChild("Footprint")?.GetComponentInChildren<UnityEngine.BoxCollider>();
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
                                //        Plugin.Log.LogInfo($"Parent propably found with name: {goMaster.name}");
                                name = goMaster.name;

                                //if (name == "WallHedgeTower_L1")
                                //{
                                //    if (goparent.name == "Anchor2")
                                //    {
                                //        Plugin.Log.LogMessage($"Position of Anchor2: {goparent.transform.localPosition}");
                                //        posA = goparent.transform.localPosition;
                                //    }
                                //    else
                                //    {
                                //        Plugin.Log.LogMessage($"Position of Anchor: {goparent.transform.localPosition}");
                                //        var posB = goparent.transform.localPosition;
                                //        gologic.FindChild("Anchor2").position = posB;
                                //        goparent.transform.position = posA;
                                //    }
                                //}
                            }
                        }
                    }
                }
                Plugin.Log.LogMessage($"Found Anchor in GO {name} with value {mb.offset}");
                if (name.Contains("WallHedgePillar") || name.Contains("Cave") || name.Contains("WaterWell"))
                {
                    mb.offset = 0;
                }
                else
                {
                    mb.offset = 0f;
                    if (coll != null)
                    {
                        Plugin.Log.LogMessage($"Collider update in GO {name} with x:{coll.size.x}, y:{coll.size.y}, z: {coll.size.z}");
                        var size = coll.size;
                        size.x -= .15f;
                        coll.size = size;
                    }
                }
            }

        }
    }
}
