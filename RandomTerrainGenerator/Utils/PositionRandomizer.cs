using RandomTerrainGenerator.Components.Moon;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace RandomTerrainGenerator.Utils
{
    internal static class PositionRandomizer
    {
        private static int areaMask;

        internal static void PlaceAiNodes(float mapRadius)
        {
            areaMask = 1 << NavMesh.GetAreaFromName("Walkable");
            var aiNodes = SceneReferences.Instance.OutsideAiNodes;

            foreach (var aiNode in aiNodes)
            {
                int attempts = 0;
                var position = Vector3.zero;

                do
                {
                    position = GetRandomPosition(mapRadius);
                    attempts++;
                }
                while (!Physics.CheckSphere(position, 15f, 4194304, QueryTriggerInteraction.Collide) && attempts < 4);

                if (position == Vector3.positiveInfinity /*|| Mathf.Abs(position.x) > 450 || Mathf.Abs(position.z) > 450*/)
                {
                    SceneReferences.Instance.nodesToDestroy.Add(aiNode);
                    continue;
                }

                aiNode.position = position;
            }

            //first node
            NavMesh.SamplePosition(Vector3.zero, out var hit, mapRadius, areaMask);
            if (hit.position == Vector3.positiveInfinity)
                SceneReferences.Instance.nodesToDestroy.Add(aiNodes[0]);

            aiNodes[0].position = hit.position;
        }

        private static Vector3 GetRandomPosition(float mapRadius)
        {
            var randomPostion = Random.insideUnitSphere * mapRadius;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPostion, out hit, mapRadius, areaMask);

            return hit.position;
        }

        internal static void PlaceEntrances()
        {
            if (!StartOfRound.Instance) return;

            foreach (var entranceTransform in SceneReferences.Instance.entrances)
                PlaceEntrance(entranceTransform);
        }
        private static void PlaceEntrance(Transform entranceTransform)
        {
            GameObject[] prefabs = SceneReferences.Instance.entrancePrefabs;
            GameObject randomPrefab;

            if (prefabs.Length > 0) randomPrefab = prefabs[Random.RandomRangeInt(0, prefabs.Length)];
            else randomPrefab = prefabs[0];

            randomPrefab = GameObject.Instantiate(randomPrefab, SceneReferences.Instance.transform);
            var moveEntranceComponent = randomPrefab.GetComponent<EntrancePrefabScript>();

            do
            {
                randomPrefab.transform.position = GetRandomPosition(512);
                randomPrefab.transform.rotation.eulerAngles.Set(0, Random.RandomRangeInt(0, 360), 0);
                moveEntranceComponent.MoveEntrance(entranceTransform);
            } while (!CheckPath(SceneReferences.Instance.OutsideAiNodes[0].position, moveEntranceComponent.entrancePoint.position) ||
                    (Vector3.Distance(SceneReferences.Instance.OutsideAiNodes[0].position, randomPrefab.transform.position) < 300));


            RaycastHit[] result = null!;
            Physics.SphereCastNonAlloc(randomPrefab.transform.position, 15f, Vector3.zero, result, 15f, LayerMask.GetMask("ScanNode"));

            if (result != null)
            {
                foreach (RaycastHit hit in result)
                {
                    Plugin.Log("hit: " + hit.transform);
                    SceneReferences.Instance.nodesToDestroy.Add(hit.transform);
                }
            }

            SceneReferences.Instance.placedEntrancePrefabs.Add(randomPrefab);
        }

        public static bool CheckPath(Vector3 position1, Vector3 position2)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(position1, position2, areaMask, path);

            return path.status == NavMeshPathStatus.PathComplete;
        }
    }
}
