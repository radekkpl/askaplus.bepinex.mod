using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace askaplus.bepinex.mod
{
    internal class RoadMakerMOD : MonoBehaviour
    {
        private int count = 0;

        public void Update()
        {
            var coll = gameObject.GetComponentsInChildren<BoxCollider>(true);
            if (coll is null) return;
            if (count == coll.Count) return;
            count = coll.Count;
            foreach (var box in coll)
            {
                if (box.gameObject.name == "Footprint")
                {
                    box.excludeLayers = LayerMask.NameToLayer("Structure");
                }
            }
        }
    }
}
