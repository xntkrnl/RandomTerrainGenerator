using RandomTerrainGenerator.Components.Moon;
using UnityEngine;
using UnityEngine.AI;

namespace RandomTerrainGenerator.Utils
{
    internal static class PositionRandomizer
    {
        internal static void PlaceAiNodes(float mapRadius)
        {
            var aiNodes = SceneReferences.Instance.OutsideAiNodes;

            foreach (var aiNode in aiNodes)
            {
                int attempts = 0;
                var position = Vector3.zero;

                do
                {
                    position = PlaceAtRandomPosition(mapRadius);
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
            NavMesh.SamplePosition(Vector3.zero, out var hit, mapRadius, 1 << NavMesh.GetAreaFromName("Walkable"));
            if (hit.position == Vector3.positiveInfinity)
                SceneReferences.Instance.nodesToDestroy.Add(aiNodes[0]);

            aiNodes[0].position = hit.position;
        }

        private static Vector3 PlaceAtRandomPosition(float mapRadius)
        {
            var randomPostion = UnityEngine.Random.insideUnitSphere * mapRadius;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPostion, out hit, mapRadius, 1 << NavMesh.GetAreaFromName("Walkable"));

            return hit.position;
        }

        internal static void PlaceEntrances()
        {
            if (!StartOfRound.Instance) return;

            PlaceEntrance(EntranceType.Main);
            PlaceEntrance(EntranceType.Fire);
        }
        private static void PlaceEntrance(EntranceType entranceType)
        {
            GameObject[] prefabs;
            GameObject randomPrefab;

            if (entranceType == EntranceType.Main) prefabs = SceneReferences.Instance.mainEntrancePrefabs;
            else prefabs = SceneReferences.Instance.fireExitPrefabs;

            if (prefabs.Length > 0) randomPrefab = prefabs[Random.RandomRangeInt(0, prefabs.Length)];
            else randomPrefab = prefabs[0];

            randomPrefab = GameObject.Instantiate(randomPrefab, SceneReferences.Instance.transform);
            randomPrefab.transform.position = PlaceAtRandomPosition(512);
            randomPrefab.transform.rotation.eulerAngles.Set(0, Random.RandomRangeInt(0, 360), 0);

            RaycastHit[] result = null!;
            var num = Physics.SphereCastNonAlloc(randomPrefab.transform.position, 15f, Vector3.zero, result, 15f, LayerMask.GetMask("ScanNode"));
            Plugin.Log($"num = {num}");
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
    }
}
