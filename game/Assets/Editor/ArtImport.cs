using UnityEditor;

// Import rules:
//   Art/Icons        ship icons (the baked top-down renders), as UI sprites
//   Art/Backgrounds  sky pictures (flat 2:1 wallpapers shown by SkyBackdrop), as plain textures
//   Imported/        textures recovered from the 2017 build, capped at 2048 (8K is wasted on an RTS camera)
public class ArtImport : AssetPostprocessor
{
    // Re-applies these rules to the sky pictures (Unity -executeMethod ArtImport.ReimportBackgrounds).
    public static void ReimportBackgrounds()
    {
        foreach (var g in AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Art/Backgrounds" }))
            AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(g), ImportAssetOptions.ForceUpdate);
    }

    void OnPreprocessAudio()
    {
        if (!assetPath.StartsWith("Assets/Art/Audio/")) return;
        var ai = (AudioImporter)assetImporter;
        bool music = assetPath.Contains("/Music/");
        var s = ai.defaultSampleSettings;
        s.loadType = music ? UnityEngine.AudioClipLoadType.Streaming : UnityEngine.AudioClipLoadType.DecompressOnLoad;
        s.compressionFormat = UnityEngine.AudioCompressionFormat.Vorbis;
        s.quality = music ? 0.45f : 0.7f;
        ai.defaultSampleSettings = s;
        ai.forceToMono = !music;
    }

    void OnPreprocessTexture()
    {
        var ti = (TextureImporter)assetImporter;
        if (assetPath.StartsWith("Assets/Art/Icons/"))
        {
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.alphaIsTransparency = true;
            ti.mipmapEnabled = true;
            ti.maxTextureSize = 512;
        }
        else if (assetPath.StartsWith("Assets/Art/Backgrounds/"))
        {
            ti.textureType = TextureImporterType.Default;
            ti.mipmapEnabled = false;
            ti.maxTextureSize = 2048;
            ti.wrapModeU = UnityEngine.TextureWrapMode.Clamp; // flat pictures (SkyBackdrop), not seamless panoramas
            ti.wrapModeV = UnityEngine.TextureWrapMode.Clamp;
        }
        else if (assetPath.StartsWith("Assets/Imported/"))
        {
            if (ti.maxTextureSize > 2048) ti.maxTextureSize = 2048;
        }
        // Compression and per-platform size caps for everything the game ships.
        if (assetPath.StartsWith("Assets/Imported/") || assetPath.StartsWith("Assets/Art/"))
            TextureSettings.Configure(ti, assetPath);
    }
}
