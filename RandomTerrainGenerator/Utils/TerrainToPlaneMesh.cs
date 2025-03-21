using RandomTerrainGenerator.Components.Moon;
using System.Collections.Generic;
using UnityEngine;
using static RandomTerrainGenerator.Utils.TerrainToPlaneMesh;

namespace RandomTerrainGenerator.Utils
{
    internal static class TerrainToPlaneMesh
    {
        internal enum TerrainLevel
        {
            texture,
            sand,
            grass,
            stone,
            snow
        }

        internal static void HeightmapTo5Meshes(float[,] hMap, int multipier)
        {
            HeightmapToTextureMesh(hMap, SceneReferences.Instance.textureMeshTerrain, multipier);

            HeightmapToMeshCollider(hMap, SceneReferences.Instance.sandMeshTerrain, multipier, TerrainLevel.sand);
            HeightmapToMeshCollider(hMap, SceneReferences.Instance.grassMeshTerrain, multipier, TerrainLevel.grass);
            HeightmapToMeshCollider(hMap, SceneReferences.Instance.stoneMeshTerrain, multipier, TerrainLevel.stone);
            HeightmapToMeshCollider(hMap, SceneReferences.Instance.snowMeshTerrain, multipier, TerrainLevel.snow);
        }

        private static void HeightmapToMeshCollider(float[,] hMap, MeshCollider meshCollider, int multipier, TerrainLevel terrainLevel)
        {
            Mesh mesh = meshCollider.sharedMesh;
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();

            for (int i = 0; i < 250; i++)
            {
                for (int j = 0; j < 250; j++)
                {
                    var height = hMap[i, j] * multipier;

                    verts.Add(new Vector3(i, height, j));

                    if (terrainLevel == TerrainLevel.sand && height > 30) continue; //13 - water level
                    if (terrainLevel == TerrainLevel.grass && (height <= 30 || height > 80)) continue;
                    if (terrainLevel == TerrainLevel.stone && (height <= 80 || height > 130)) continue;
                    if (terrainLevel == TerrainLevel.snow && height <= 130) continue;
                    if (i == 0 || j == 0) continue;

                    //Adds the index of the three vertices in order to make up each of the two tris
                    tris.Add(250 * i + j); //Top right
                    tris.Add(250 * i + j - 1); //Bottom right
                    tris.Add(250 * (i - 1) + j - 1); //Bottom left - First triangle
                    tris.Add(250 * (i - 1) + j - 1); //Bottom left 
                    tris.Add(250 * (i - 1) + j); //Top left
                    tris.Add(250 * i + j); //Top right - Second triangle

                }
            }

            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();

            meshCollider.sharedMesh = mesh;
        }

        private static void HeightmapToTextureMesh(float[,] hMap, Mesh mesh, int multipier)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();

            for (int i = 0; i < 250; i++)
            {
                for (int j = 0; j < 250; j++)
                {
                    var height = hMap[i, j] * multipier;
                    verts.Add(new Vector3(i, height, j));

                    if (i == 0 || j == 0) continue;

                    //Adds the index of the three vertices in order to make up each of the two tris
                    tris.Add(250 * i + j); //Top right
                    tris.Add(250 * i + j - 1); //Bottom right
                    tris.Add(250 * (i - 1) + j - 1); //Bottom left - First triangle
                    tris.Add(250 * (i - 1) + j - 1); //Bottom left 
                    tris.Add(250 * (i - 1) + j); //Top left
                    tris.Add(250 * i + j); //Top right - Second triangle

                }
            }

            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();
        }
    }
}
