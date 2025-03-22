using UnityEngine;

namespace RandomTerrainGenerator.Components.Moon
{
    public enum EntranceType
    {
        Main,
        Fire
    }

    public class EntrancePrefabScript : MonoBehaviour
    {
        public EntranceType type;
        public Transform entrancePoint;

        void Start()
        {
            Transform entrance;

            if (type == EntranceType.Main) entrance = SceneReferences.Instance.mainEntrance;
            else entrance = SceneReferences.Instance.fireExit;

            entrance.position = entrancePoint.position;
            entrance.rotation = entrancePoint.rotation;
        }
    }
}
