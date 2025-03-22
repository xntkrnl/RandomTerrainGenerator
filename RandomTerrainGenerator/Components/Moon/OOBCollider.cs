using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace RandomTerrainGenerator.Components.Moon
{
    public class OOBCollider : MonoBehaviour
    {
        [SerializeField] private int targetLayer;

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == targetLayer)
                other.transform.forward *= -1;
            
        }
    }
}
