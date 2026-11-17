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
        else if (assetPath.StartsWith("Assets/Art/Portraits/"))
        {
            // Comms portraits (tools/portraits.py): transparent PNGs shown in the dialogue panel.
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.alphaIsTransparency = true;
            ti.mipmapEnabled = false;
            ti.maxTextureSize = 1024;
        }
        else if (assetPath.StartsWith("Assets/Art/Stacks/"))
        {
            // Sprite-stack atlases (tools/sprite_stack.py): one palette index per pixel, so exact bytes matter:
            // single channel, no filtering, no compression, readable for SpriteStacks to colour at load.
            ti.textureType = TextureImporterType.Default;
            ti.npotScale = TextureImporterNPOTScale.None; // atlases are any size; scaling would mix indices
            ti.alphaSource = TextureImporterAlphaSource.None;
            ti.sRGBTexture = false;
            ti.isReadable = true;
            ti.mipmapEnabled = false;
            ti.filterMode = UnityEngine.FilterMode.Point;
            ti.wrapMode = UnityEngine.TextureWrapMode.Clamp;
            ti.maxTextureSize = 4096;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            var ps = ti.GetDefaultPlatformTextureSettings();
            ps.format = TextureImporterFormat.R8;
            ti.SetPlatformTextureSettings(ps);
            foreach (var platform in new[] { "Standalone", "WebGL" })
                ti.SetPlatformTextureSettings(new TextureImporterPlatformSettings { name = platform, overridden = true, maxTextureSize = 4096, format = TextureImporterFormat.R8 });
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
