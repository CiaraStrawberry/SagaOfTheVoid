using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Lifts the three 2017 battle maps (MapsPack1-3 in the battle scene) out as clean prefabs:
// only meshes and their materials, positioned relative to the map root. Exports them with their
// meshes, materials and textures as a .unitypackage for the game project.
//   Unity -batchmode -quit -projectPath tools/mapbake -executeMethod MapExtractor.Run
public static class MapExtractor
{
    static readonly (string pack, string id)[] Maps = { ("MapsPack1", "asteroid_field"), ("MapsPack2", "space_station"), ("MapsPack3", "ship_graveyard") };

    public static void Run()
    {
        var scene = EditorSceneManager.OpenScene("Assets/_Scenes/CaptureAndHold/mptest.unity");
        var all = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<Transform>(true)).ToList();
        Directory.CreateDirectory("Assets/MapsOut");

        // Where ships lived in 2017, for lining the maps up with the battle plane.
        foreach (var name in new[] { "WorldScaleBase", "World", "Objects" })
        {
            var t = all.FirstOrDefault(x => x.name == name);
            if (t != null) Log(string.Format("frame {0}: path {1} pos {2} lossyScale {3}", name, PathOf(t), t.position, t.lossyScale));
        }

        foreach (var (pack, id) in Maps)
        {
            var src = all.FirstOrDefault(t => t.name == pack);
            if (src == null) { Log("MISSING " + pack); continue; }
            Log(string.Format("{0}: path {1} pos {2} lossyScale {3} active {4}", pack, PathOf(src), src.position, src.lossyScale, src.gameObject.activeSelf));
            var root = new GameObject(id);
            var bounds = new Bounds(); bool first = true;
            int parts = 0;
            foreach (var r in src.GetComponentsInChildren<MeshRenderer>(true))
            {
                var mf = r.GetComponent<MeshFilter>();
                if (mf == null || mf.sharedMesh == null) continue;
                var part = new GameObject(r.name);
                part.transform.SetParent(root.transform, false);
                // Pose relative to the map root, so the map can be placed and scaled as a whole.
                var rel = src.worldToLocalMatrix * r.transform.localToWorldMatrix;
                part.transform.localPosition = rel.GetColumn(3);
                part.transform.localRotation = rel.rotation;
                part.transform.localScale = rel.lossyScale;
                part.AddComponent<MeshFilter>().sharedMesh = mf.sharedMesh;
                part.AddComponent<MeshRenderer>().sharedMaterials = r.sharedMaterials;
                var b = new Bounds(part.transform.localPosition, Vector3.Scale(mf.sharedMesh.bounds.size, part.transform.localScale));
                if (first) { bounds = b; first = false; } else bounds.Encapsulate(b);
                parts++;
            }
            PrefabUtility.SaveAsPrefabAsset(root, "Assets/MapsOut/" + id + ".prefab");
            Object.DestroyImmediate(root);
            Log(string.Format("{0} -> {1}: {2} parts, bounds centre {3} size {4}", pack, id, parts, bounds.center, bounds.size));
        }
        AssetDatabase.SaveAssets();
        ExportForGame();
    }

    // The game project builds its own URP materials, so only meshes and textures travel, plus a manifest
    // saying which part uses which mesh, pose and textures. Everything moves under Assets/Imported/Maps first
    // so the package lands in the same folder in the game project.
    [System.Serializable] class Part { public string name, mesh, albedo, normal, emission; public float[] pos, rot, scale, colour; }
    [System.Serializable] class MapEntry { public string id; public List<Part> parts = new List<Part>(); }
    [System.Serializable] class Manifest { public List<MapEntry> maps = new List<MapEntry>(); }

    static void ExportForGame()
    {
        const string dst = "Assets/Imported/Maps";
        Directory.CreateDirectory(dst + "/Mesh");
        Directory.CreateDirectory(dst + "/Texture2D");
        AssetDatabase.Refresh();
        var moved = new Dictionary<string, string>();
        string Move(Object o, string folder)
        {
            if (o == null) return null;
            var p = AssetDatabase.GetAssetPath(o);
            if (string.IsNullOrEmpty(p) || !p.StartsWith("Assets/")) return null;
            if (moved.TryGetValue(p, out var already)) return already;
            if (p.StartsWith(dst + "/")) return p; // already moved (used by more than one part)
            var to = AssetDatabase.GenerateUniqueAssetPath(dst + "/" + folder + "/" + System.IO.Path.GetFileName(p));
            var err = AssetDatabase.MoveAsset(p, to);
            if (!string.IsNullOrEmpty(err)) { Log("move failed " + p + ": " + err); to = p; }
            moved[p] = to;
            return to;
        }
        Texture Tex(Material m, params string[] names)
        {
            if (m == null) return null;
            foreach (var n in names) if (m.HasProperty(n) && m.GetTexture(n) != null) return m.GetTexture(n);
            return null;
        }

        var manifest = new Manifest();
        foreach (var (pack, id) in Maps)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MapsOut/" + id + ".prefab");
            if (prefab == null) continue;
            var entry = new MapEntry { id = id };
            foreach (Transform t in prefab.transform)
            {
                var mf = t.GetComponent<MeshFilter>();
                var mr = t.GetComponent<MeshRenderer>();
                var m = mr.sharedMaterial;
                var c = m != null && m.HasProperty("_Color") ? m.color : Color.white;
                entry.parts.Add(new Part
                {
                    name = t.name,
                    mesh = Move(mf.sharedMesh, "Mesh"),
                    albedo = Move(Tex(m, "_MainTex", "_BaseMap", "_Albedo"), "Texture2D"),
                    normal = Move(Tex(m, "_BumpMap", "_NormalMap", "_Normal"), "Texture2D"),
                    emission = Move(Tex(m, "_EmissionMap", "_Illum", "_Emission"), "Texture2D"),
                    pos = new[] { t.localPosition.x, t.localPosition.y, t.localPosition.z },
                    rot = new[] { t.localRotation.x, t.localRotation.y, t.localRotation.z, t.localRotation.w },
                    scale = new[] { t.localScale.x, t.localScale.y, t.localScale.z },
                    colour = new[] { c.r, c.g, c.b, c.a },
                });
            }
            manifest.maps.Add(entry);
            Log(string.Format("{0}: {1} parts, {2} with albedo", id, entry.parts.Count, entry.parts.Count(p => p.albedo != null)));
        }
        File.WriteAllText(dst + "/maps.json", JsonUtility.ToJson(manifest, true));
        AssetDatabase.Refresh();
        var outFile = System.IO.Path.GetFullPath("maps.unitypackage");
        AssetDatabase.ExportPackage(new[] { dst }, outFile, ExportPackageOptions.Recurse);
        Log("exported " + outFile + " " + new FileInfo(outFile).Length / 1048576 + " MB, " + moved.Count + " assets");
    }

    static string PathOf(Transform t) => t.parent == null ? t.name : PathOf(t.parent) + "/" + t.name;
    static void Log(string s) => Debug.Log("MAPS " + s);
}
