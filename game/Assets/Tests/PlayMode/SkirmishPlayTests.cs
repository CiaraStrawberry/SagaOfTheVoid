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
            var go = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            go.tag = "MainCamera";
            var cam = go.GetComponent<Camera>();
            var rt = new RenderTexture(W, H, 24, RenderTextureFormat.ARGB32) { antiAliasing = 4 };
            cam.targetTexture = rt; // the HUD lays itself out for this size
            var game = go.AddComponent<SkirmishController>();
            game.quickFleets = true;
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
            yield return RunUntil(game, 60);
            Assert.IsTrue(capital.Dead || capital.Armor < capital.StartArmor || capital.Shield < capital.StartShield,
                "the ordered target took fire");
            Shot(cam, rt, "skirmish_3_attack");

            // Select a single ship to show the detail panel.
            var one = game.Own.FirstOrDefault(s => !Hulls.IsSmall(s.Hull));
            if (one != null) { game.Select(new[] { one }); game.Focus(new[] { one }); }
            yield return null; yield return null;
            Shot(cam, rt, "skirmish_4_single");

            Object.Destroy(go);
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
