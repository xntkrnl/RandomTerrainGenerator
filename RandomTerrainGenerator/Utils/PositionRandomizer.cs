using RandomTerrainGenerator.Components;
using System;
using System.Text;
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
                //check if ai nodes too close
                do
                {
                    position = PlaceAtRandomPosition(mapRadius);
                    attempts++;
                }
                while ((!Physics.CheckSphere(position, 15f, 4194304, QueryTriggerInteraction.Collide) && attempts < 4));

                if (position == Vector3.positiveInfinity /*|| Mathf.Abs(position.x) > 450 || Mathf.Abs(position.z) > 450*/)
                {
                    SceneReferences.Instance.nodesToDestroy.Add(aiNode);
                    continue;
                }

                aiNode.position = position;
            }

            //first node
            NavMesh.SamplePosition(Vector3.zero, out var hit, mapRadius, 1 << NavMesh.GetAreaFromName("Walkable"));
            aiNodes[0].position = hit.position;
        }

        private static Vector3 PlaceAtRandomPosition(float mapRadius)
        {
            var randomPostion = UnityEngine.Random.insideUnitSphere * mapRadius;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomPostion, out hit, mapRadius, 1 << NavMesh.GetAreaFromName("Walkable"));

            return hit.position;
        }

        //Plans for fireexit/main/something else prefabs positions:
        //PlaceAtRandomPosition(400f)
        //check if can actually path to it
        //mark ai nodes in some radius to destroy if needed
    }
}
