using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace RandomTerrainGenerator.Components.Moon
{
    public class SceneReferences : MonoBehaviour
    {
        private static SceneReferences _instance;
        public static SceneReferences Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<SceneReferences>();
                return _instance;
            }
        }

        [Header("Ai nodes")]
        public List<Transform> OutsideAiNodes;
        internal List<Transform> nodesToDestroy;

        [Header("Terrain")]
        public MeshCollider sandMeshTerrain;
        public MeshCollider grassMeshTerrain;
        public MeshCollider stoneMeshTerrain;
        public MeshCollider snowMeshTerrain;
        public Mesh textureMeshTerrain;

        [Header("Entrances")]
        public GameObject[] mainEntrancePrefabs;
        public GameObject[] fireExitPrefabs;
        public Transform mainEntrance;
        public Transform fireExit;
        internal List<GameObject> placedEntrancePrefabs;

        [Header("Misc")]
        public Transform itemShipAnimContainer;
        public NavMeshSurface environmentNavMeshSurface;

    }
}
