using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace RandomTerrainGenerator.Components
{
    [RequireComponent(typeof(NavMeshSurface))]
    public class SceneReferences : MonoBehaviour
    {
        private static SceneReferences _instance;
        public static SceneReferences Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<SceneReferences>();
                return (_instance);
            }
        }

        public Transform itemShipAnimContainer;
        public NavMeshSurface environmentNavMeshSurface;
        public List<Transform> OutsideAiNodes;
        internal List<Transform> nodesToDestroy;
        public Mesh planeMeshTerrain;
    }
}
