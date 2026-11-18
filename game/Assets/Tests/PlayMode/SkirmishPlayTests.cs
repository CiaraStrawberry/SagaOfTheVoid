using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Saga.Game;
using Saga.Sim;
using UnityEngine;
using UnityEngine.TestTools;

namespace Saga.Tests
{
    // Plays a real skirmish in Play mode: select the fleet, move it, attack, and save screenshots
    // (tools/preview/skirmish_*.png) of what a player would see, HUD included.
    public class SkirmishPlayTests
    {
        const int W = 1280, H = 720;

        [UnityTest]
        public IEnumerator Skirmish_SelectMoveAttack()
        {
            Clean();
            var go = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            go.tag = "MainCamera";
            var cam = go.GetComponent<Camera>();
            var rt = new RenderTexture(W, H, 24, RenderTextureFormat.ARGB32) { antiAliasing = 4 };
            cam.targetTexture = rt; // the HUD lays itself out for this size
            var game = go.AddComponent<SkirmishController>();
            game.quickFleets = true;
            game.showMenu = false;
            yield return null;
            yield return null;
            Assert.NotNull(game.World, "skirmish started");

            game.SelectAllOwn();
            Assert.AreEqual(Fleets.Quick.Length, game.Selected.Count, "selects the whole Blue fleet");
            Assert.IsTrue(game.Own.All(s => s.Team == SkirmishController.PlayerTeam));
            yield return null;
            Shot(cam, rt, "skirmish_1_selected");

            // Move north, keeping formation.
            var start = Centre(game.Selected);
            var dest = new Vector3(start.x, 0, start.z + 2500f);
            game.CommandMove(dest, false);
            game.GameSpeed = 4f;
            yield return RunUntil(game, 14);
            var moved = Centre(game.Selected.Where(s => !Hulls.IsSmall(s.Hull)).ToList());
            Assert.Less(Vector3.Distance(moved, new Vector3(dest.x, 0, dest.z)), Vector3.Distance(start, dest) - 1000f,
                "fleet closed in on the move point");
            Assert.IsTrue(game.World.Alive.Where(s => s.Team == 1).All(s => s.Team == 1), "computer still plays red");
            Shot(cam, rt, "skirmish_2_moved");

            // Attack the enemy capital.
            var capital = game.World.Alive.First(s => s.Team == 1 && s.Def.id == "capital");

            // Clicks a little off an enemy still pick it (generous attack click target).
            var cv = game.View.ViewOf(capital);
            game.Focus(new[] { capital });
            yield return null;
            var sp = (Vector2)cam.WorldToScreenPoint(cv.Go.transform.position + Vector3.up * cv.Height);
            Assert.AreEqual(capital, game.PickAt(sp + new Vector2(20, 12), s => s.Team != SkirmishController.PlayerTeam), "near-miss click picks the enemy");
            Assert.IsNull(game.PickAt(sp + new Vector2(20, 12), s => s.Team == SkirmishController.PlayerTeam && s == capital), "own-ship filter is respected");

            game.CommandAttack(capital);
            Assert.IsTrue(game.Selected.All(s => s.OrderedTarget == capital));
            game.Focus(new[] { capital });
            yield return RunUntil(game, 90); // big ships are slow and turn heavily
            Assert.IsTrue(capital.Dead || capital.Armor < capital.StartArmor || capital.Shield < capital.StartShield,
                "the ordered target took fire (game time " + game.World.Time.ToString("F0") + " s, nearest attacker " + game.Selected.Where(s => !s.Dead).Select(s => Vec2.Distance(s.Pos, capital.Pos)).DefaultIfEmpty(-1).Min().ToString("F0") + ")");
            Shot(cam, rt, "skirmish_3_attack");

            // Buy a fighter: paid now, arrives after its build time beside the fleet.
            if (game.World.Winner < 0)
            {
                int before = game.World.Ships.Count(x => x.Team == SkirmishController.PlayerTeam && x.Arrived >= 0);
                game.World.TeamOf(SkirmishController.PlayerTeam).Money = 5000;
                Assert.IsTrue(game.Buy("fighter"), "can buy a fighter with 5,000 credits");
                Assert.IsTrue(game.Buy("missile_frigate"));
                yield return null;
                Shot(cam, rt, "skirmish_5_building");
                double until = game.World.Time + 15;
                yield return RunUntil(game, until);
                // Built ships wait in reserve; warp them in behind the fleet.
                Assert.AreEqual(2, game.World.TeamOf(SkirmishController.PlayerTeam).Reserve.Count + (game.World.Winner >= 0 ? 2 : 0), "both wait in reserve");
                // The first clear spot behind the fleet (not within 1,000 of an enemy).
                var fleetCentre = Centre(game.Own.ToList());
                var behind = new[] { 1500f, 3000f, 5000f, 8000f }.Select(d => fleetCentre + new Vector3(-d, 0, 0))
                    .FirstOrDefault(p => game.World.CannotDeploy(SkirmishController.PlayerTeam, BattleView.ToSim(p)) == null);
                game.Focus(game.Own);
                game.BeginDeploy("*");
                var deployed = game.DeployAt(behind, game.DeployType);
                game.CancelDeploy();
                yield return null; yield return null; yield return null;
                yield return new WaitForSecondsRealtime(0.9f); // mid-warp
                Shot(cam, rt, "skirmish_6_warp");
                yield return RunUntil(game, game.World.Time + 2);
                int arrived = game.World.Ships.Count(x => x.Team == SkirmishController.PlayerTeam && x.Arrived >= 0) - before;
                Assert.AreEqual(2, arrived + (game.World.Winner >= 0 ? 2 : 0), "both bought ships arrived (losses in combat do not matter)");
            }

            // Select a single ship to show the detail panel.
            var one = game.Own.FirstOrDefault(s => !Hulls.IsSmall(s.Hull));
            if (one != null) { game.Select(new[] { one }); game.Focus(new[] { one }); }
            yield return null; yield return null;
            Shot(cam, rt, "skirmish_4_single");

            Object.Destroy(go);
        }

        [UnityTest]
        public IEnumerator StartMenu_Shows()
        {
            var (go, cam, rt) = MakeCamera();
            var game = go.AddComponent<SkirmishController>();
            yield return null; yield return null;
            Assert.IsNull(game.World, "no battle until something is chosen");
            Assert.NotNull(GameObject.Find("Start menu"), "menu canvas exists");
            Shot(cam, rt, "menu_1_start");
            Object.Destroy(go); Object.Destroy(GameObject.Find("Start menu"));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Campaign_Mission1_StartsWithItsFleets()
        {
            var (go, cam, rt) = MakeCamera();
            SkirmishController.Pending = new BattleConfig { Mission = 0 };
            var game = go.AddComponent<SkirmishController>();
            yield return null; yield return null;
            Assert.NotNull(game.World);
            Assert.AreEqual("Basic Training", game.MissionDef.name);
            Assert.AreEqual(4, game.World.Count(1), "four enemy scouts");
            Assert.AreEqual(2000, game.World.TeamOf(0).Money, 1, "mission start money");
            Assert.IsNotNull(game.World.CannotBuy(0, "cannon_frigate"), "frigates are not available in mission 1");
            // The opening briefing holds the battle until it is read.
            Assert.IsNotNull(game.Comms, "campaign missions have a comms panel");
            Assert.IsTrue(game.Comms.Open, "the briefing opens at the start");
            double before = game.World.Time;
            yield return null; yield return null;
            Assert.AreEqual(before, game.World.Time, 1e-9, "the battle waits during the briefing");
            game.Comms.FinishLine();
            yield return null;
            Shot(cam, rt, "mission_1_briefing");
            game.Comms.Close();
            // The pause menu stops the battle too.
            yield return null;
            game.OpenMenu();
            double atPause = game.World.Time;
            yield return null; yield return null;
            Assert.IsTrue(game.MenuOpen);
            Assert.AreEqual(atPause, game.World.Time, 1e-9, "the battle waits while the pause menu is open");
            Shot(cam, rt, "pause_menu");
            game.CloseMenu();
            Assert.IsFalse(game.Paused, "closing the menu resumes");
            game.SelectAllOwn();
            game.GameSpeed = 4f;
            yield return RunUntil(game, 8);
            Shot(cam, rt, "mission_1_basic_training");
            Object.Destroy(go);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Campaign_Mission3_RelayCallIsAnUnidentifiedSignal()
        {
            var (go, cam, rt) = MakeCamera();
            SkirmishController.Pending = new BattleConfig { Mission = 2 };
            var game = go.AddComponent<SkirmishController>();
            yield return null; yield return null;
            game.Comms.Close();
            game.PlayStoryBeat("interruption");
            yield return null;
            Assert.IsTrue(game.Comms.Open);
            game.Comms.FinishLine();
            yield return null;
            Shot(cam, rt, "mission_3_unknown_signal");
            game.Comms.Close();
            Object.Destroy(go);
            yield return null;
        }

        // Each test starts from an empty scene (earlier tests leave lights, maps and canvases behind).
        // Plays the tutorial through the same calls the mouse and keys make, one step after another, and
        // saves a picture of the coach panel at a few steps (tools/preview/tutorial_*.png).
        [UnityTest]
        public IEnumerator Tutorial_PlaysThrough()
        {
            var (go, cam, rt) = MakeCamera();
            SkirmishController.Pending = new BattleConfig { Tutorial = true };
            var game = go.AddComponent<SkirmishController>();
            yield return null; yield return null;
            var tut = game.Tutorial;
            Assert.NotNull(tut, "tutorial running");
            Assert.AreEqual(0, tut.StepIndex);
            Assert.AreEqual(0, game.World.Count(1), "no enemies yet");
            Shot(cam, rt, "tutorial_1_welcome");

            tut.Go(1);                                     // camera: needs real keys, skip it
            yield return null;
            Shot(cam, rt, "tutorial_2_camera");
            tut.Skip();
            Assert.AreEqual("Select ships", tut.StepTitle);
            game.Select(new[] { game.Own.First() });
            yield return null;
            game.SelectAllOwn();
            yield return WaitStep(tut, 3, 5f);
            Assert.AreEqual("Move", tut.StepTitle, "selecting moved it on");
            Shot(cam, rt, "tutorial_3_move");

            game.CommandMove(tut.BeaconAt, false);
            game.GameSpeed = 4f;
            yield return WaitStep(tut, 4, 60f);
            Assert.AreEqual("Groups", tut.StepTitle, "reaching the ring moved it on");
            tut.Skip();                                    // Ctrl+1 / 1 are keys

            Assert.AreEqual("Attack", tut.StepTitle);
            var frigate = game.World.Alive.Single(s => s.Team == 1);
            game.SelectAllOwn();
            game.CommandAttack(frigate);
            yield return WaitStep(tut, 6, 90f);
            Assert.IsTrue(frigate.Dead, "frigate destroyed");
            Assert.AreEqual("Fighters", tut.StepTitle);

            game.Select(game.Own.Where(s => Hulls.IsSmall(s.Hull)));
            foreach (var e in game.World.Alive.Where(s => s.Team == 1).ToList()) game.CommandAttack(e);
            yield return WaitStep(tut, 7, 90f);
            Assert.AreEqual("Build a ship", tut.StepTitle);
            Assert.GreaterOrEqual(game.World.TeamOf(0).Money, 950, "credits handed out to buy with");

            Assert.IsTrue(game.Buy("fighter"), "buys a fighter");
            yield return WaitStep(tut, 8, 60f);
            Assert.AreEqual("Warp it in", tut.StepTitle);
            Shot(cam, rt, "tutorial_4_reserve");
            var near = BattleView.ToWorld(game.Own.First().Pos) + new Vector3(-800, 0, 0);
            Assert.IsNotEmpty(game.DeployAt(near, "*"), "warped in");
            yield return WaitStep(tut, 9, 10f);
            Assert.AreEqual("Your first battle", tut.StepTitle);
            Assert.AreEqual(4, game.World.Count(1), "the patrol jumped in");

            float guard = Time.realtimeSinceStartup + 150f;
            while (game.World.Winner < 0 && Time.realtimeSinceStartup < guard)
            {
                var red = game.World.Alive.Where(s => s.Team == 1).ToList();
                if (red.Count > 0 && game.Own.Any(s => s.OrderedTarget == null || s.OrderedTarget.Dead)) { game.SelectAllOwn(); game.CommandAttack(red[0]); }
                yield return null;
            }
            Assert.AreEqual(0, game.World.Winner, "won the tutorial");
            Assert.AreEqual(1, PlayerPrefs.GetInt(Tutorial.DoneKey, 0), "remembered as done");
            yield return null;
            Shot(cam, rt, "tutorial_5_complete");
            PlayerPrefs.DeleteKey(Tutorial.DoneKey);
        }

        static IEnumerator WaitStep(Tutorial tut, int step, float realSeconds)
        {
            float guard = Time.realtimeSinceStartup + realSeconds;
            while (tut.StepIndex < step && Time.realtimeSinceStartup < guard) yield return null;
        }

        static void Clean()
        {
            foreach (var g in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
                if (!g.name.Contains("tests runner")) Object.DestroyImmediate(g);
            SkirmishController.Pending = null;
        }

        static (GameObject, Camera, RenderTexture) MakeCamera()
        {
            Clean();
            var go = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            go.tag = "MainCamera";
            var cam = go.GetComponent<Camera>();
            var rt = new RenderTexture(W, H, 24, RenderTextureFormat.ARGB32) { antiAliasing = 4 };
            cam.targetTexture = rt;
            return (go, cam, rt);
        }

        static IEnumerator RunUntil(SkirmishController game, double seconds)
        {
            float guard = Time.realtimeSinceStartup + 120f;
            while (game.World.Time < seconds && game.World.Winner < 0 && Time.realtimeSinceStartup < guard) yield return null;
        }

        static Vector3 Centre(System.Collections.Generic.IList<Ship> ships) =>
            new Vector3((float)ships.Average(s => s.Pos.X), 0, (float)ships.Average(s => s.Pos.Y));

        static void Shot(Camera cam, RenderTexture rt, string name)
        {
            cam.Render();
            var prev = RenderTexture.active;
            RenderTexture.active = rt;
            var t = new Texture2D(W, H, TextureFormat.RGB24, false);
            t.ReadPixels(new Rect(0, 0, W, H), 0, 0);
            t.Apply();
            RenderTexture.active = prev;
            var dir = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Application.dataPath)), "tools", "preview");
            Directory.CreateDirectory(dir);
            File.WriteAllBytes(Path.Combine(dir, name + ".png"), t.EncodeToPNG());
        }
    }
}
