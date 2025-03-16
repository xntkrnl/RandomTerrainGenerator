using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace RandomTerrainGenerator.Utils
{
    internal static class TerrainToPlaneMesh
    {
        internal static void HeightmapToPlaneMesh(float[,] hMap, Mesh mesh, int multipier)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();

            for (int i = 0; i < 250; i++)
            {
                for (int j = 0; j < 250; j++)
                {
                    verts.Add(new Vector3(i, hMap[i,j] * multipier, j));

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
