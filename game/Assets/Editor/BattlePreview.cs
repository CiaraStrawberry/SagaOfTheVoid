using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Saga.Game;
using Saga.Sim;
using UnityEngine;

// Records a computer-vs-computer battle to numbered JPG frames using the game's own BattleView.
//   Unity -batchmode -quit -projectPath game -executeMethod BattlePreview.Record          (18 v 18 showcase)
//   Unity -batchmode -quit -projectPath game -executeMethod BattlePreview.Record -quick   (8 v 8, for fast iteration)
// Then: ffmpeg -framerate 60 -i tools/preview/frames/%05d.jpg ... (2x speed)
public static class BattlePreview
{
    const int Width = 1280, Height = 720, Fps = 30, StepsPerFrame = 2;
    const float CamPitch = 52f, CamFov = 35f;

    public static void Record()
    {
        var args = Environment.GetCommandLineArgs();
        bool quick = args.Contains("-quick");
        string Arg(string name, string fallback) { int i = Array.IndexOf(args, name); return i >= 0 && i + 1 < args.Length ? args[i + 1] : fallback; }
        string mapId = Arg("-map", "asteroid_field"), skyId = Arg("-sky", "skybox5");
        var outDir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Application.dataPath)), "tools", "preview", "frames");
        if (Directory.Exists(outDir)) Directory.Delete(outDir, true);
        Directory.CreateDirectory(outDir);

        var lib = ShipLibrary.Load();
        if (lib == null) throw new Exception("Assets/Resources/ShipLibrary.asset missing; run ShipModelBuilder.BuildShips");
        var world = new World(lib.Rules(), seed: 2017);
        Fleets.Spawn(world, quick ? Fleets.Quick : Fleets.Full, quick ? Fleets.QuickGap : Fleets.FullGap);
        var mode = Arg("-mode", "ffa");
        Fleets.SpawnMode(world, mode == "capital" ? SkirmishMode.CapitalShip : mode == "base" ? SkirmishMode.BaseDefence : SkirmishMode.FreeForAll,
            quick ? Fleets.QuickGap : Fleets.FullGap);
        double maxSeconds = quick ? 75 : 300;

        var cam = new GameObject("Camera").AddComponent<Camera>();
        cam.fieldOfView = CamFov;
        cam.nearClipPlane = 50f;
        BattleView.BuildStage(lib, cam, mapId == "none" ? null : mapId, skyId);
        var view = new BattleView(world, lib, cam);
        var hud = MakeText("hud", 22, TextAnchor.UpperLeft);
        var title = MakeText("title", 24, TextAnchor.UpperRight);
        title.text = "SAGA OF THE VOID  ·  preview";
        Log((quick ? "quick" : "full") + " battle, " + world.Ships.Count + " ships, map " + mapId + ", sky " + skyId);

        var rt = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGB32) { antiAliasing = 4 };
        cam.targetTexture = rt;
        var grab = new Texture2D(Width, Height, TextureFormat.RGB24, false);

        var camTarget = Centre(world.Alive.ToList());
        float camDist = 9000;
        int frame = 0;
        double endAt = -1;
        double dt = 1.0 / (Fps * StepsPerFrame);

        while (world.Time < maxSeconds && (endAt < 0 || world.Time < endAt))
        {
            var shots = new List<ShotEvent>();
            var impacts = new List<ImpactEvent>();
            var deaths = new List<Ship>();
            for (int i = 0; i < StepsPerFrame; i++)
            {
                world.ClearEvents();
                world.Step(dt);
                shots.AddRange(world.Shots); impacts.AddRange(world.Impacts); deaths.AddRange(world.Deaths);
            }
            world.ClearEvents();
            if (world.Winner >= 0 && endAt < 0) endAt = world.Time + 4;

            // Camera: keep every live ship in view, eased.
            var alive = world.Alive.ToList();
            // -follow <ship id> [-dist n]: a close camera on the first live ship of that kind (looking at details).
            var follow = alive.FirstOrDefault(s => s.Def.id == Arg("-follow", "") && s.Team == 0);
            if (follow != null)
            {
                var fv = view.ViewOf(follow);
                var fp = fv != null ? BattleView.ShownCentre(fv) : BattleView.ToWorld(follow.Pos);
                camTarget = Vector3.Lerp(camTarget, new Vector3(fp.x, 0, fp.z), 0.08f);
                camDist = float.Parse(Arg("-dist", "2200"), System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (alive.Count > 0)
            {
                var c = Centre(alive);
                float spread = alive.Max(s => Vector3.Distance(BattleView.ToWorld(s.Pos), c)) + 900f;
                float want = Mathf.Max(3600f, spread / Mathf.Tan(CamFov * 0.5f * Mathf.Deg2Rad) * 0.5f);
                camTarget = Vector3.Lerp(camTarget, c, 0.035f);
                camDist = Mathf.Lerp(camDist, want, 0.025f);
            }
            var rot = Quaternion.Euler(CamPitch, 0, 0);
            cam.transform.rotation = rot;
            cam.transform.position = camTarget - rot * Vector3.forward * camDist;
            cam.farClipPlane = camDist * 3f;

            view.Sync(shots, impacts, deaths, 1f / Fps, world.Time);

            PlaceText(cam, hud, 24, 20);
            PlaceText(cam, title, Width - 24, 20);
            hud.text = string.Format("<color=#a8d4ff>BLUE  {0}</color>     <color=#ffb0a8>RED  {1}</color>     {2:0}s{3}",
                world.Count(0), world.Count(1), world.Time,
                world.Winner < 0 ? "" : world.Winner == 0 ? "     BLUE WINS" : "     RED WINS");

            Saga.Game.StackFollow.SyncAll();
            cam.Render();
            RenderTexture.active = rt;
            grab.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
            grab.Apply(false);
            RenderTexture.active = null;
            File.WriteAllBytes(Path.Combine(outDir, frame.ToString("00000") + ".jpg"), grab.EncodeToJPG(90));
            frame++;
            if (frame % 600 == 0) Log(string.Format("frame {0}  t={1:0}s  blue={2} red={3}", frame, world.Time, world.Count(0), world.Count(1)));
        }
        Log(string.Format("impacts: on hull surface {0}, hull fallback to centre {1}, on shield rim {2}", view.HullHits, view.HullFallbacks, view.ShieldHits));
        Log(string.Format("done: {0} frames, {1:0.0}s, winner={2}, blue left={3}, red left={4}",
            frame, world.Time, world.Winner, world.Count(0), world.Count(1)));
    }

    public static void BuildAndRecord()
    {
        ShipModelBuilder.BuildShips();
        Record();
    }

    static Vector3 Centre(List<Ship> ships) => ships.Count == 0 ? Vector3.zero
        : new Vector3((float)ships.Average(s => s.Pos.X), 0, (float)ships.Average(s => s.Pos.Y));

    static TextMesh MakeText(string name, int size, TextAnchor anchor)
    {
        var go = new GameObject(name);
        var tm = go.AddComponent<TextMesh>();
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        tm.font = font;
        tm.fontSize = size * 4;
        tm.characterSize = size / (tm.fontSize * 0.1f) * 0.8f;
        tm.anchor = anchor;
        tm.richText = true;
        tm.color = new Color(0.86f, 0.9f, 0.96f);
        go.GetComponent<MeshRenderer>().sharedMaterial = font.material;
        return tm;
    }

    // Pins a text to a pixel position on screen at constant pixel size, just in front of the camera.
    static void PlaceText(Camera cam, TextMesh t, float x, float y)
    {
        float d = cam.nearClipPlane * 2f;
        t.transform.position = cam.ScreenToWorldPoint(new Vector3(x, Height - y, d));
        t.transform.rotation = cam.transform.rotation;
        t.transform.localScale = Vector3.one * (2f * d * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) / Height);
    }

    static void Log(string s) => Debug.Log("PREVIEW " + s);
}
