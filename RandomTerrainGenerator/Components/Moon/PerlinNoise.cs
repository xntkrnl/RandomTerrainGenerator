/*using UnityEngine;

namespace RandomTerrainGenerator.Components
{
    public class PerlinNoiseScript : MonoBehaviour
    {
        public bool updateOnValidate;
        public Terrain terrain;

        [Space(5f)]
        [Header("Perlin noise")]
        public float xMultiplier = .1f;
        public float yMultiplier = .1f;
        public float multiplier = .1f;

        [Space(5f)]
        public int heightMapResolution;

        private void OnValidate()
        {
            if (!updateOnValidate) return;

            Start();
        }

        private void Start()
        {
            PerlinNoise();
        }

        protected void PerlinNoise()
        {
            terrain.terrainData.heightmapResolution = heightMapResolution;
            float[,] mesh = new float[heightMapResolution, heightMapResolution];
            mesh = terrain.terrainData.GetHeights(0, 0, heightMapResolution, heightMapResolution);

            for (int x = 0; x < heightMapResolution; x++)
                for (int y = 0; y < heightMapResolution; y++)
                    mesh[x, y] = Mathf.PerlinNoise(x * xMultiplier, y * yMultiplier) * multiplier;

            //heightmapTexture = terrain.terrainData.heightmapTexture;

            terrain.terrainData.SetHeights(0, 0, mesh);
        }


    }
}
*/