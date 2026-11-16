using System.IO;
using System.Linq;
using NUnit.Framework;
using Saga.Sim;
using UnityEngine;

namespace Saga.Tests
{
    // Checks the battle rules against the 2017 behaviour recorded in PLAN.md and the decisions since.
    public class RulesTests
    {
        static Rulebook rules;

        [OneTimeSetUp]
        public void Load()
        {
            rules = new Rulebook(
                JsonUtility.FromJson<ShipDefList>(File.ReadAllText("Assets/Data/ships.json")),
                JsonUtility.FromJson<WeaponDefList>(File.ReadAllText("Assets/Data/weapons.json")));
        }

        static World NewWorld() => new World(rules, 1);

        [Test]
        public void DataLoads_AllShipsAndWeapons()
        {
            Assert.AreEqual(13, rules.Ships.Count);
            Assert.AreEqual(10, rules.Weapons.Count);
            Assert.AreEqual(17000, rules.Ships["capital"].armor);
            Assert.AreEqual(3000, rules.Weapons["laser"].damage);
            Assert.IsFalse(rules.Weapons.ContainsKey("cloak"), "cloak was replaced by the shield field");
        }

        [Test]
        public void Shield_SoaksWholeHit_ThenBreaksToMinusOne()
        {
            var w = NewWorld();
            var s = w.Spawn("cannon_frigate", 0, Vec2.Zero, 0);
            s.Shield = 100;
            w.TakeDamage(s, 800, Hull.Frigate);
            Assert.AreEqual(-1, s.Shield);
            Assert.AreEqual(s.StartArmor, s.Armor, "overflow does not reach armour (2017 rule)");
            w.TakeDamage(s, 800, Hull.Frigate);
            Assert.AreEqual(s.StartArmor - 800, s.Armor);
        }

        [Test]
        public void Squadron_LosesCraftInSteps_WhateverTheDamage()
        {
            var w = NewWorld();
            var f = w.Spawn("fighter", 0, Vec2.Zero, 0);
            f.Shield = -1;
            int[] expected = { 21, 15, 9, 3, -5 }; // 70%, 50%, 30%, 10% of 30, then destroyed
            foreach (var e in expected)
            {
                w.TakeDamage(f, 5000, Hull.Capital);
                Assert.AreEqual(e, f.Armor);
            }
        }

        [Test]
        public void ShieldField_RechargesFriendsInRange_NotFighters_NotStacking()
        {
            var w = NewWorld();
            w.PlayerTeams.Add(0); // no computer orders moving things about
            w.Spawn("support_frigate", 0, new Vec2(0, 0), 0);
            w.Spawn("support_frigate", 0, new Vec2(100, 0), 0);
            var near = w.Spawn("capital", 0, new Vec2(500, 0), 0);
            var far = w.Spawn("capital", 0, new Vec2(5000, 0), 0);
            var fighter = w.Spawn("fighter", 0, new Vec2(300, 0), 0);
            near.Shield = 0; far.Shield = 0; fighter.Shield = 0;
            for (int i = 0; i < 600; i++) w.Step(1.0 / 60); // 10 s
            Assert.AreEqual(1700, near.Shield, 20, "1% of 17,000 per second for 10 s, one field only");
            Assert.AreEqual(0, far.Shield);
            Assert.AreEqual(0, fighter.Shield);
        }

        [Test]
        public void Waypoints_AreFollowedInOrder()
        {
            var w = NewWorld();
            w.PlayerTeams.Add(0);
            var s = w.Spawn("scout", 0, Vec2.Zero, 0);
            var group = new[] { s };
            w.OrderMove(group, new Vec2(0, 2000), false);
            w.OrderMove(group, new Vec2(2000, 2000), true);
            Assert.AreEqual(1, s.Waypoints.Count);
            bool reachedFirst = false;
            for (int i = 0; i < 60 * 20; i++)
            {
                w.Step(1.0 / 60);
                if (Vec2.Distance(s.Pos, new Vec2(0, 2000)) < 450) reachedFirst = true;
                if (s.Waypoints.Count == 0) break;
            }
            Assert.IsTrue(reachedFirst);
            Assert.AreEqual(0, s.Waypoints.Count);
            Assert.AreEqual(2000, s.MoveTarget.X, 1);
        }

        [Test]
        public void AttackOrder_TargetIsChosenOverPreference()
        {
            var w = NewWorld();
            w.PlayerTeams.Add(0);
            var me = w.Spawn("cannon_frigate", 0, Vec2.Zero, 0);          // cannons prefer capitals
            var capital = w.Spawn("capital", 1, new Vec2(1000, 0), 0);
            var frigate = w.Spawn("laser_frigate", 1, new Vec2(0, 1000), 0);
            w.PlayerTeams.Add(1);
            for (int i = 0; i < 70; i++) w.Step(1.0 / 60);
            Assert.AreEqual(capital, me.Target, "without orders: preferred hull");
            w.OrderAttack(new[] { me }, frigate);
            for (int i = 0; i < 70; i++) w.Step(1.0 / 60);
            Assert.AreEqual(frigate, me.Target, "with an attack order: the ordered ship");
        }

        [Test]
        public void SameSeed_SameBattle()
        {
            string Run()
            {
                var w = new World(rules, 42);
                SpawnBoth(w);
                for (int i = 0; i < 60 * 60; i++) w.Step(1.0 / 60);
                return string.Join(";", w.Ships.Select(s => s.Armor + "," + s.Shield + "," + s.Pos.X.ToString("F3")));
            }
            Assert.AreEqual(Run(), Run());
        }

        [Test]
        public void ComputerBattle_ReachesAWinner()
        {
            var w = new World(rules, 7);
            SpawnBoth(w);
            for (int i = 0; i < 60 * 400 && w.Winner < 0; i++) w.Step(1.0 / 60);
            Assert.GreaterOrEqual(w.Winner, 0, "battle should end within 400 s");
        }

        // Mirrors Saga.Game.Fleets.Quick (that assembly is not referenced here to keep these tests engine-light).
        static void SpawnBoth(World w)
        {
            var fleet = new (string, double, double)[]
            {
                ("capital", -700, 0), ("missile_frigate", -300, 900), ("laser_frigate", 0, -500), ("cannon_frigate", 0, 500),
                ("flak_frigate", 200, -1200), ("support_frigate", -900, -800), ("fighter", 500, 1600), ("bomber", 400, -1800),
            };
            for (int team = 0; team < 2; team++)
            {
                double side = team == 0 ? -1 : 1;
                foreach (var (id, x, y) in fleet)
                    w.Spawn(id, team, new Vec2(side * (2800 - x), y * (team == 0 ? 1 : -1)), side < 0 ? -System.Math.PI / 2 : System.Math.PI / 2);
            }
        }
    }
}
