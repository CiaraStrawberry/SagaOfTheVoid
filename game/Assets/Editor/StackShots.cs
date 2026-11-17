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
        new GameObject("sun").AddComponent<Light>().type = LightType.Directional;

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
            SpriteStacks.AttachScenery(lib, m, map);
            StackFollow.SyncAll();
            var b = m.GetComponentsInChildren<Renderer>().Select(r => r.bounds).Aggregate((a, c) => { a.Encapsulate(c); return a; });
            float d = Mathf.Max(b.size.x, b.size.z);
            Shot(cam, b.center + new Vector3(0, d * 0.75f, -d * 0.75f), b.center, 1600, 900, Path.Combine(outDir, "stacks_" + map + ".png"));
            UnityEngine.Object.DestroyImmediate(m);
        }
        Debug.Log("STACKSHOTS done");
    }

    static string Rules(ShipLibrary lib, string id) => JsonUtility.FromJson<ShipDefList>(lib.shipsJson.text).ships.First(s => s.id == id).sprite;

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
