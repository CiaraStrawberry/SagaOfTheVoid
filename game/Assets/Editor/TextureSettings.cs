using System.Linq;
using UnityEditor;
using UnityEngine;

// Compresses every game texture (the recovered art came in uncompressed) and caps sizes per platform:
// desktop 2048, web 1024. Run once after importing new art; ArtImport applies the same rules to new files.
//   Unity -batchmode -quit -projectPath game -executeMethod TextureSettings.Apply
public static class TextureSettings
{
    public const int DesktopMax = 2048, WebMax = 1024;

    [MenuItem("Saga/Compress all textures")]
    public static void Apply()
    {
        var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Imported", "Assets/Art", "Assets/Maps", "Assets/Ships" });
        int changed = 0;
        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                if (path.StartsWith("Assets/Art/Stacks/")) continue; // sprite-stack indices must stay exact (ArtImport)
                if (!(AssetImporter.GetAtPath(path) is TextureImporter ti)) continue;
                if (Configure(ti, path)) { ti.SaveAndReimport(); changed++; }
            }
        }
        finally { AssetDatabase.StopAssetEditing(); }
        Debug.Log("TEXTURES compressed/capped: " + changed + " of " + guids.Length);
        CompressMeshes();
    }

    // Mesh compression for the recovered meshes (stored as .asset, so there's no model importer to set it on).
    [MenuItem("Saga/Compress all meshes")]
    public static void CompressMeshes()
    {
        int n = 0;
        foreach (var g in AssetDatabase.FindAssets("t:Mesh", new[] { "Assets/Imported", "Assets/Ships", "Assets/Maps" }))
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            if (!path.EndsWith(".asset")) continue;
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (mesh == null || MeshUtility.GetMeshCompression(mesh) == ModelImporterMeshCompression.Medium) continue;
            MeshUtility.SetMeshCompression(mesh, ModelImporterMeshCompression.Medium);
            EditorUtility.SetDirty(mesh);
            n++;
        }
        AssetDatabase.SaveAssets();
        PlayerSettings.stripUnusedMeshComponents = true;
        Debug.Log("MESHES compressed: " + n);
    }

    // Returns true if anything changed.
    public static bool Configure(TextureImporter ti, string path)
    {
        bool icon = path.StartsWith("Assets/Art/Icons/");
        var name = System.IO.Path.GetFileNameWithoutExtension(path);
        // Detail maps don't need full size at strategy-camera distance.
        bool normal = name.Contains("Normal") || ti.textureType == TextureImporterType.NormalMap;
        bool glow = name.Contains("Illumination") || name.Contains("Emission") || name.Contains("Emissive");
        int desktop = icon ? 512 : glow ? 512 : normal ? 1024 : DesktopMax;
        int web = icon ? 256 : glow ? 256 : normal ? 512 : WebMax;
        bool changed = false;
        if (ti.textureCompression != TextureImporterCompression.Compressed) { ti.textureCompression = TextureImporterCompression.Compressed; changed = true; }
        if (ti.maxTextureSize > desktop) { ti.maxTextureSize = desktop; changed = true; }
        if (ti.crunchedCompression) { ti.crunchedCompression = false; changed = true; }
        var webSettings = ti.GetPlatformTextureSettings("WebGL");
        if (!webSettings.overridden || webSettings.maxTextureSize != web || !webSettings.crunchedCompression)
        {
            webSettings.overridden = true;
            webSettings.maxTextureSize = web;
            webSettings.format = TextureImporterFormat.Automatic;
            webSettings.textureCompression = TextureImporterCompression.Compressed;
            // Crunch: much smaller downloads, unpacked when loading.
            webSettings.crunchedCompression = true;
            webSettings.compressionQuality = 50;
            ti.SetPlatformTextureSettings(webSettings);
            changed = true;
        }
        return changed;
    }
}
