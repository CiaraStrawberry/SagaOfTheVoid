using System.Collections.Generic;
using System.IO;
using System.Linq;
using Saga.Game;
using Saga.Sim;
using UnityEngine;

// Pictures of ships dying, for checking the break-up and crash effects (BattleView.Wrecks).
//   Unity -batchmode -quit -projectPath game -executeMethod WreckShots.Run
// Puts a destroyer, a capital and a fighter squadron in empty space, destroys them, and writes
// tools/preview/wreck_<ship>_<n>.png at a few moments after.
public static class WreckShots
{
    public static void Run()
    {
        var lib = ShipLibrary.Load();
        var outDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../../tools/preview"));
        var world = new World(lib.Rules(), seed: 7);
        // Far apart and far from the one enemy, so nothing fights.
        var targets = new[] { ("destroyer", -1500f), ("capital", 0f), ("fighter", 1500f) }
            .Select(t => (t.Item1, world.Spawn(t.Item1, 0, new Vec2(t.Item2, 0), 0.3))).ToList();
        world.Spawn("scout", 1, new Vec2(0, 60000), 0);
        var cam = new GameObject("Camera").AddComponent<Camera>();
        cam.fieldOfView = 40f; cam.nearClipPlane = 20f; cam.farClipPlane = 200000f;
        cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(0.02f, 0.03f, 0.06f);
        BattleView.BuildStage(lib, cam, null, null);
        var view = new BattleView(world, lib, cam);

        const float dt = 1f / 30f;
        double t = 0;
        void Step()
        {
            world.ClearEvents();
            world.Step(dt);
            t += dt;
            view.Sync(world.Shots.ToList(), world.Impacts.ToList(), world.Deaths.ToList(), dt, t);
            StackFollow.SyncAll();
        }
        for (int i = 0; i < 120; i++) Step(); // warp-ins finish
        foreach (var (id, s) in targets)
        {
            var at = Aim(id, new Vector3((float)s.Pos.X, 0, (float)s.Pos.Y));
            float d = id == "fighter" ? 900f : id == "capital" ? 1500f : 1100f;
            cam.transform.position = at + new Vector3(0, d * 0.85f, -d * 0.7f);
            cam.transform.LookAt(at);
            Shoot(cam, Path.Combine(outDir, $"wreck_{id}_before.png"));
        }
        foreach (var (_, s) in targets) s.Armor = -1;
        var moments = new[] { 0.15f, 0.7f, 1.5f, 3f, 5.5f };
        int shot = 0;
        float since = 0;
        while (shot < moments.Length)
        {
            Step();
            since += dt;
            if (since >= moments[shot])
            {
                foreach (var (id, s) in targets)
                {
                    var at = Aim(id, new Vector3((float)s.Pos.X, 0, (float)s.Pos.Y));
                    float d = id == "fighter" ? 900f : id == "capital" ? 1500f : 1100f;
                    cam.transform.position = at + new Vector3(0, d * 0.85f, -d * 0.7f);
                    cam.transform.LookAt(at);
                    Shoot(cam, Path.Combine(outDir, $"wreck_{id}_{shot}.png"));
                }
                shot++;
            }
        }
        Debug.Log("WRECKSHOTS done");
    }

    // Fighters fly well away from their squadron's point, so look at the craft themselves (or what's left of them).
    static Vector3 Aim(string id, Vector3 at)
    {
        if (id != "fighter") return at;
        var all = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
        var falling = all.Where(x => x.name == "falling craft").Select(x => x.position).ToList();
        var craft = falling.Count > 0 ? falling : all.Where(x => x.name == "stack" && x.root.name.StartsWith("fighter_") || x.name == "stack" && x.parent != null && x.parent.name == "Ship" && x.gameObject.activeInHierarchy).Select(x => x.position).ToList();
        if (craft.Count == 0) return at;
        var c = craft.Aggregate(Vector3.zero, (a, b) => a + b) / craft.Count;
        c.y = 0;
        return c;
    }

    static void Shoot(Camera cam, string path)
    {
        var rt = new RenderTexture(800, 450, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;
        var tex = new Texture2D(800, 450, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, 800, 450), 0, 0);
        tex.Apply();
        File.WriteAllBytes(path, tex.EncodeToPNG());
        RenderTexture.active = null;
        cam.targetTexture = null;
        rt.Release();
    }
}
