using UnityEngine;

namespace RandomTerrainGenerator.Components.Moon
{
    public class EntrancePrefabScript : MonoBehaviour
    {
        public Transform entrancePoint;

        public void MoveEntrance(Transform entrance)
        {
            entrance.position = entrancePoint.position;
            entrance.rotation = entrancePoint.rotation;
        }
    }
}
