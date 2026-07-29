using UnityEditor;
using UnityEngine;

namespace VertigoDemo.Editor
{
    public sealed class DemoSpriteImporter : AssetPostprocessor
    {
        private const string ArtRoot = "Assets/VertigoDemo/Fortune Raid/Art/";

        private void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(ArtRoot))
            {
                return;
            }

            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.wrapMode = UnityEngine.TextureWrapMode.Clamp;
            importer.filterMode = UnityEngine.FilterMode.Bilinear;
            importer.maxTextureSize = 2048;
            importer.spritePixelsPerUnit = 100f;
            string lower = assetPath.ToLowerInvariant();
            if (lower.Contains("frame_4px"))
            {
                importer.spriteBorder = new UnityEngine.Vector4(8f, 8f, 8f, 8f);
            }
            else if (lower.Contains("button_") || lower.Contains("panel_zone") || lower.Contains("frame_12px"))
            {
                importer.spriteBorder = new UnityEngine.Vector4(18f, 18f, 18f, 18f);
            }
        }
    }
}
