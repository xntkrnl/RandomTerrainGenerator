using RandomTerrainGenerator.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RandomTerrainGenerator.Components.Moon
{
    [RequireComponent(typeof(SceneReferences))]
    public class DiamondSquareScript : MonoBehaviour
    {
        public bool updateButton = false;
        public Terrain terrain;

        [Space(5f)]
        [Header("Diamond Square")]
        public int heightMapResolution;
        public bool randomizeCornerValues = false;
        public float[,] heights;
        public float _roughness = 0.8f;
        public int heightMultiplier = 300;
        public bool randomHeightMultiplier;

        [Space(5f)]
        [Header("Random")]
        public int seed = 28;
        public bool StartOfRoundSeed = true;

        [Space(5f)]
        [Header("Validation")]
        public float minimum;
        public bool skipMinimumCheck;
        public float mapRadiusForNavMeshCheck = 512f;
        public bool skipNavMeshCheck;

        [Space(5f)]
        [Header("Misc")]
        public bool saveTerrainHegihtMapToDisk = false;

        private void OnValidate()
        {
            if (!updateButton) return;

            updateButton = false;
            InitDiamondSquare();
        }

        private void Awake() => InitDiamondSquare();

        private void InitDiamondSquare()
        {
            if (StartOfRoundSeed && StartOfRound.Instance) //yeah im checking for instance because unity editor
                seed = StartOfRound.Instance.randomMapSeed;

            bool terrainValidated = false;
            if (SceneReferences.Instance.sandMeshTerrain && SceneReferences.Instance.grassMeshTerrain && SceneReferences.Instance.stoneMeshTerrain && SceneReferences.Instance.snowMeshTerrain && SceneReferences.Instance.textureMeshTerrain)
                while (!terrainValidated)
                {
                    Random.InitState(seed);
                    Reset();
                    ExecuteDiamondSquare();

                    if (randomHeightMultiplier)
                        heightMultiplier = Random.RandomRangeInt(300, 600);
                    TerrainToPlaneMesh.HeightmapTo5Meshes(heights, heightMultiplier);

                    if (ValidateTerrain(seed))
                        terrainValidated = true;
                    else
                        seed++;
                }
            else
            {
                Plugin.Log("SandMeshTerrain/GrassMeshTerrain/StoneMeshTerrain/SnowMeshTerrain/TextureMesh is missing. Add meshes to SceneReferences to continue.");
                return;
            }

            if (terrain)
            {
                terrain.terrainData.SetHeights(0, 0, heights);
                if (saveTerrainHegihtMapToDisk)
                {
                    TextureHandler.SaveTexture(terrain.terrainData.heightmapTexture, $"Heightmap_{seed}", heightMapResolution);
                    saveTerrainHegihtMapToDisk = false;
                }
            }

            PositionRandomizer.PlaceEntrances();
            CleanupAiNodes();
            StartCoroutine(LastNavmeshRebuild());
        }

        private bool ValidateTerrain(int seed)
        {
            if (!skipMinimumCheck)
            {
                float percentage = 100f;
                float heightUnderWater = 0;
                for (int x = 0; x < heightMapResolution - 1; x++)
                    for (int y = 0; y < heightMapResolution - 1; y++)
                        if (heights[x, y]*heightMultiplier <= minimum)
                            heightUnderWater++;

                percentage = heightUnderWater / (heightMapResolution * heightMapResolution) * 100;
                Plugin.Log($"seed {seed}: {percentage}% under water");
                if (percentage > 65)
                    return false;
            }

            if (!skipNavMeshCheck)
            {
                SceneReferences.Instance.environmentNavMeshSurface.BuildNavMesh();
                PositionRandomizer.PlaceAiNodes(mapRadiusForNavMeshCheck);

                var mainAiNode = SceneReferences.Instance.OutsideAiNodes[0];
                if (Mathf.Abs(mainAiNode.position.x) > 40f || Mathf.Abs(mainAiNode.position.z) > 40f || SceneReferences.Instance.nodesToDestroy.Contains(mainAiNode))
                {
                    Plugin.Log($"Seed {seed}: Main ai node is not in the center or the center is in the water");
                    return false;
                }
                else
                {
                    foreach (var aiNode in SceneReferences.Instance.OutsideAiNodes)
                    {
                        if (aiNode == mainAiNode) continue;

                        NavMeshPath path = new NavMeshPath();
                        NavMesh.CalculatePath(aiNode.position, mainAiNode.position, 1 << NavMesh.GetAreaFromName("Walkable"), path);

                        if (path.status != NavMeshPathStatus.PathComplete)
                            SceneReferences.Instance.nodesToDestroy.Add(aiNode);
                    }

                    Plugin.Log($"Seed {seed}: NavMeshErrors: {(float)SceneReferences.Instance.nodesToDestroy.Count / SceneReferences.Instance.OutsideAiNodes.Count}, navMeshErrorsCounter: {SceneReferences.Instance.nodesToDestroy.Count}");
                    if ((float)SceneReferences.Instance.nodesToDestroy.Count / SceneReferences.Instance.OutsideAiNodes.Count >= 0.5) return false;
                }
            }

            Plugin.Log($"Seed {seed} is valid.");
            return true;
        }

        private void Reset()
        {
            heights = new float[heightMapResolution, heightMapResolution];
            SceneReferences.Instance.nodesToDestroy = new List<Transform>();

            if (SceneReferences.Instance.placedEntrancePrefabs != null)
                foreach(var prefab in SceneReferences.Instance.placedEntrancePrefabs)
                    Destroy(prefab);
            SceneReferences.Instance.placedEntrancePrefabs = new List<GameObject>();

            if (randomizeCornerValues)
            {
                heights[0, 0] = Random.value;
                heights[heightMapResolution - 1, 0] = Random.value;
                heights[0, heightMapResolution - 1] = Random.value;
                heights[heightMapResolution - 1, heightMapResolution - 1] = Random.value;
            }

            if (terrain)
                terrain.terrainData.SetHeights(0, 0, heights);
        }

        private void ExecuteDiamondSquare()
        {
            heights = new float[heightMapResolution, heightMapResolution];
            float average, range = 0.5f;
            int sideLength, halfSide, x, y;

            for (sideLength = heightMapResolution - 1; sideLength > 1; sideLength /= 2)
            {
                halfSide = sideLength / 2;

                // Run Diamond Step
                for (x = 0; x < heightMapResolution - 1; x += sideLength)
                {
                    for (y = 0; y < heightMapResolution - 1; y += sideLength)
                    {
                        average = heights[x, y];
                        average += heights[x + sideLength, y];
                        average += heights[x, y + sideLength];
                        average += heights[x + sideLength, y + sideLength];
                        average /= 4.0f;

                        average += Random.value * (range * 2.0f) - range;
                        heights[x + halfSide, y + halfSide] = average;
                    }
                }

                // Run Square Step
                for (x = 0; x < heightMapResolution - 1; x += halfSide)
                {
                    for (y = (x + halfSide) % sideLength; y < heightMapResolution - 1; y += sideLength)
                    {
                        average = heights[(x - halfSide + heightMapResolution - 1) % (heightMapResolution - 1), y];
                        average += heights[(x + halfSide) % (heightMapResolution - 1), y];
                        average += heights[x, (y + halfSide) % (heightMapResolution - 1)];
                        average += heights[x, (y - halfSide + heightMapResolution - 1) % (heightMapResolution - 1)];
                        average /= 4.0f;

                        average += Random.value * (range * 2.0f) - range;
                        heights[x, y] = average;

                        if (x == 0)
                        {
                            heights[heightMapResolution - 1, y] = average;
                        }

                        if (y == 0)
                        {
                            heights[x, heightMapResolution - 1] = average;
                        }
                    }
                }

                range -= range * 0.5f * _roughness;
            }

            Plugin.Log("Diamond Square algorithm done.");
        }

        private void CleanupAiNodes()
        {
            if (Plugin.Instance && StartOfRound.Instance)
            {
                Plugin.Log("Removing 'bad' ai nodes.");

                foreach (var aiNode in SceneReferences.Instance.nodesToDestroy)
                    Destroy(aiNode.gameObject);

                SceneReferences.Instance.nodesToDestroy.Clear();
            }
        }

        private IEnumerator LastNavmeshRebuild()
        {
            yield return null;
            SceneReferences.Instance.environmentNavMeshSurface.BuildNavMesh();
        }
    }
}
