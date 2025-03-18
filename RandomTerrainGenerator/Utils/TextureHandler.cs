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

        internal static Texture2D ToTexture2D(RenderTexture renderTexture)
        {
            RenderTexture currentActiveRT = RenderTexture.active;
            Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentActiveRT;
            return texture2D;
        }

        internal static RenderTexture ToRenderTexture(Texture2D texture2D)
        {
            RenderTexture renderTexture = new RenderTexture();

            Graphics.Blit(texture2D, renderTexture);

            return renderTexture;
        }

        internal static void SaveTexture(RenderTexture renderTexture, string nameOfPNG, int resolution)
        {
            var texture = ToTexture2D(renderTexture);
            SaveTexturePNG(texture, nameOfPNG);
        }
    }
}
