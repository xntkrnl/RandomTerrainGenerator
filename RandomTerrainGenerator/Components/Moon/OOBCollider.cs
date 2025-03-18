using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RandomTerrainGenerator.Components.Moon
{
    [RequireComponent(typeof(Collider))]
    internal class OOBCollider : MonoBehaviour
    {
        [SerializeField] private int targetLayer; 

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == targetLayer)
                other.transform.forward *= -1;
            //not tested
        }
    }
}
