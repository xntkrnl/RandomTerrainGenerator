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

        internal static Texture2D ToTexture2D(RenderTexture renderTexture, int resolution)
        {
            RenderTexture currentActiveRT = RenderTexture.active;
            Texture2D texture2D = new Texture2D(resolution, resolution, TextureFormat.RGB24, false);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentActiveRT;
            return texture2D;
        }

        internal static RenderTexture toRenderTexture(Texture2D texture2D)
        {
            RenderTexture renderTexture = new RenderTexture();
            RenderTexture currentActiveRT = RenderTexture.active; //not sure if i actually need this

            Graphics.Blit(texture2D, renderTexture);

            RenderTexture.active = currentActiveRT;
            return renderTexture;
        }

        internal static void SaveTexture(RenderTexture renderTexture, string nameOfPNG, int resolution)
        {
            var texture = ToTexture2D(renderTexture, resolution);
            SaveTexturePNG(texture, nameOfPNG);
        }
    }
}
