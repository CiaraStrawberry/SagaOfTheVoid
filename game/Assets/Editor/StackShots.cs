using System;
using System.IO;
using System.Linq;
using Saga.Game;
using Saga.Sim;
using UnityEditor;
using UnityEngine;

// Still pictures of the sprite-stacked ships and scenery, for checking the look quickly.
//   Unity -batchmode -quit -projectPath game -executeMethod StackShots.Run
// Writes tools/preview/stacks_*.png: a line-up of every ship (Blue, then Red) seen from the battle camera's angle at
// two distances, and each battle map from above.
public static class StackShots
{
    public static void Run()
    {
        if (Environment.GetCommandLineArgs().Contains("-reimport"))
            AssetDatabase.ImportAsset("Assets/Art/Stacks", ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
        var lib = ShipLibrary.Load();
        var outDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../../tools/preview"));
        var cam = new GameObject("Camera").AddComponent<Camera>();
        cam.fieldOfView = 40f; cam.nearClipPlane = 20f; cam.farClipPlane = 200000f;
        cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(0.02f, 0.03f, 0.06f);
        // The battle stage's own lighting (BattleView.BuildStage), so 3D and stacks compare as they look in play.
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.5f, 0.55f, 0.65f);
        RenderSettings.ambientEquatorColor = new Color(0.35f, 0.36f, 0.4f);
        RenderSettings.ambientGroundColor = new Color(0.2f, 0.18f, 0.18f);
        var sun = new GameObject("Sun").AddComponent<Light>();
        sun.type = LightType.Directional; sun.color = new Color(1f, 0.95f, 0.86f); sun.intensity = 2.4f;
        sun.transform.rotation = Quaternion.Euler(60f, -25f, 0f);
        var rim = new GameObject("Rim light").AddComponent<Light>();
        rim.type = LightType.Directional; rim.color = new Color(0.45f, 0.6f, 1f); rim.intensity = 0.8f;
        rim.transform.rotation = Quaternion.Euler(30f, 160f, 0f);
        var gains = new System.Text.StringBuilder("{");

        // Ship line-up, laid out like BattleView sizes them.
        var ids = new[] { "fighter", "bomber", "scout", "missile_frigate", "flak_frigate", "cannon_frigate", "laser_frigate", "repair_frigate", "destroyer", "capital", "super_capital" };
        float x = 0;
        foreach (var colour in new[] { "Blue", "Red" })
        {
            x = 0;
            float z = colour == "Blue" ? 0 : -1400;
            foreach (var id in ids)
            {
                var prefab = lib.Prefab(id);
                if (prefab == null) continue;
                var go = UnityEngine.Object.Instantiate(prefab);
                var model = go.GetComponent<ShipModel>();
                model.ApplyPaint(colour);
                bool small = id == "fighter" || id == "bomber" || id == "scout";
                float len = BattleView.SizeK * Mathf.Pow(model.length, BattleView.SizePow) * (small ? BattleView.SquadronBoost : 1f);
                go.transform.localScale = Vector3.one * (len / model.length);
                go.transform.position = new Vector3(x + len / 2, 0, z);
                go.transform.rotation = Quaternion.Euler(0, 25, 0);
                var sprite = lib.ships.Find(e => e.id == id) != null ? Rules(lib, id) : null;
                SpriteStacks.AttachShip(lib, model, sprite, colour, small);
                x += len + 120;
            }
        }
        StackFollow.SyncAll();
        float width = x;
        Shot(cam, new Vector3(width / 2, 2600, -2700), new Vector3(width / 2, 0, -700), 2400, 1000, Path.Combine(outDir, "stacks_lineup.png"));
        Shot(cam, new Vector3(1500, 900, -900), new Vector3(1500, 0, 0), 1600, 900, Path.Combine(outDir, "stacks_close.png"));
        Shot(cam, new Vector3(width - 1100, 1100, -1300), new Vector3(width - 1100, 0, -100), 1600, 900, Path.Combine(outDir, "stacks_capitals.png"));
        foreach (var go in UnityEngine.Object.FindObjectsByType<ShipModel>(FindObjectsSortMode.None)) UnityEngine.Object.DestroyImmediate(go.gameObject);

        foreach (var map in new[] { "asteroid_field", "space_station", "ship_graveyard" })
        {
            var m = UnityEngine.Object.Instantiate(lib.Map(map));
            var b = m.GetComponentsInChildren<Renderer>().Select(r => r.bounds).Aggregate((a, c) => { a.Encapsulate(c); return a; });
            float d = Mathf.Max(b.size.x, b.size.z);
            // Brightness of the 3D scenery against the stacks (lit pixels only), to keep the look matched.
            float look3d = Brightness(cam, b.center + new Vector3(0, d * 0.75f, -d * 0.75f), b.center);
            SpriteStacks.AttachScenery(lib, m, map);
            StackFollow.SyncAll();
            float lookStack = Brightness(cam, b.center + new Vector3(0, d * 0.75f, -d * 0.75f), b.center);
            float ratio = look3d / Mathf.Max(1e-4f, lookStack);
            Debug.Log(string.Format("STACKSHOTS {0}: 3D brightness {1:F3}, stacks {2:F3}, ratio {3:F2}", map, look3d, lookStack, ratio));
            // How much brighter the stacks must be drawn to match (on top of any gain they already have).
            gains.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "{0}\"{1}\": {2:F3}", gains.Length > 1 ? ", " : "", map, ratio * SpriteStacks.SceneryGain(lib, map));
            Shot(cam, b.center + new Vector3(0, d * 0.75f, -d * 0.75f), b.center, 1600, 900, Path.Combine(outDir, "stacks_" + map + ".png"));
            UnityEngine.Object.DestroyImmediate(m);
        }
        if (Environment.GetCommandLineArgs().Contains("-calibrate"))
            File.WriteAllText(Path.GetFullPath(Path.Combine(Application.dataPath, "../../tools/scenery_out/gains.json")), gains.Append("}").ToString());
        Debug.Log("STACKSHOTS done");
    }

    static string Rules(ShipLibrary lib, string id) => JsonUtility.FromJson<ShipDefList>(lib.shipsJson.text).ships.First(s => s.id == id).sprite;

    static float Brightness(Camera cam, Vector3 from, Vector3 at)
    {
        cam.transform.position = from; cam.transform.LookAt(at);
        var rt = new RenderTexture(400, 225, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt; cam.Render(); RenderTexture.active = rt;
        var t = new Texture2D(400, 225, TextureFormat.RGB24, false);
        t.ReadPixels(new Rect(0, 0, 400, 225), 0, 0); t.Apply();
        RenderTexture.active = null; cam.targetTexture = null; rt.Release();
        var bg = cam.backgroundColor; double sum = 0; int n = 0;
        foreach (var c in t.GetPixels())
        {
            if (Mathf.Abs(c.r - bg.r) + Mathf.Abs(c.g - bg.g) + Mathf.Abs(c.b - bg.b) < 0.03f) continue;
            sum += 0.3f * c.r + 0.59f * c.g + 0.11f * c.b; n++;
        }
        UnityEngine.Object.DestroyImmediate(t);
        return n > 0 ? (float)(sum / n) : 0f;
    }

    static void Shot(Camera cam, Vector3 from, Vector3 at, int w, int h, string path)
    {
        cam.transform.position = from;
        cam.transform.LookAt(at);
        var rt = new RenderTexture(w, h, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;
        var t = new Texture2D(w, h, TextureFormat.RGB24, false);
        t.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        t.Apply();
        File.WriteAllBytes(path, t.EncodeToPNG());
        RenderTexture.active = null;
        cam.targetTexture = null;
        rt.Release();
    }
}
