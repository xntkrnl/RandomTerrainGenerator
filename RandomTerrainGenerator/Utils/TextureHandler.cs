using System.IO;
using System.Reflection;
using UnityEngine;

namespace RandomTerrainGenerator.Utils
{
    internal static class TextureHandler
    {
        private static void SaveTexturePNG(Texture2D tex, string nameOfPNG)
        {
            byte[] bytes = tex.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(tex);

            string pathAndFileName = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/{nameOfPNG}.png";

            File.WriteAllBytes(pathAndFileName, bytes);

            Plugin.Log($"HeightMap saved in: {pathAndFileName}");
        }

        internal static Texture2D ToTexture2D(RenderTexture rTex, int resolution)
        {
            Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGB24, false);

            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            return tex;
        }

        internal static void SaveTexture(RenderTexture renderTexture, string nameOfPNG, int resolution)
        {
            var texture = ToTexture2D(renderTexture, resolution);
            SaveTexturePNG(texture, nameOfPNG);
        }
    }
}
