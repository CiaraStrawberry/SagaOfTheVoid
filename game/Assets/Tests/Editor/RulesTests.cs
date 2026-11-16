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

        static void Run(World w, double seconds) { for (int i = 0; i < (int)(seconds * 60); i++) w.Step(1.0 / 60); }

        static World Duel()
        {
            var w = NewWorld();
            w.PlayerTeams.Add(0); w.PlayerTeams.Add(1); // nobody moves on their own
            w.Spawn("capital", 0, new Vec2(-20000, 0), 0);
            w.Spawn("capital", 1, new Vec2(20000, 0), 0);
            return w;
        }

        [Test]
        public void Economy_IncomeIs30PerSecond_FromStartMoney()
        {
            var w = Duel();
            Run(w, 10);
            Assert.AreEqual(World.StartMoney + 300, w.TeamOf(0).Money, 1.0);
        }

        [Test]
        public void Economy_BuyPaysNow_QueueStacks_BuiltShipsWaitInReserve()
        {
            var w = Duel();
            Run(w, 0.1);
            double before = w.TeamOf(0).Money;
            Assert.IsTrue(w.TryBuy(0, "scout"));
            Assert.IsTrue(w.TryBuy(0, "scout"));
            Assert.AreEqual(before - 2 * 650, w.TeamOf(0).Money, 0.01);
            Assert.AreEqual(1, w.TeamOf(0).Queue.Count, "same kind stacks into one order");
            Assert.AreEqual(2, w.TeamOf(0).Queued);
            Run(w, 4.9);
            Assert.AreEqual(0, w.TeamOf(0).Reserve.Count, "not yet: scouts take 5 s");
            Run(w, 0.2);
            Assert.AreEqual(1, w.TeamOf(0).InReserve("scout"), "built ship waits in reserve");
            Assert.AreEqual(1, w.Count(0), "not on the field until deployed");
            Run(w, 5);
            Assert.AreEqual(2, w.TeamOf(0).InReserve("scout"));
            Assert.AreEqual(0, w.TeamOf(0).Queued);
        }

        [Test]
        public void Reserve_DeployWarpsInAtThePoint_NotNextToTheEnemy()
        {
            var w = Duel();
            Run(w, 0.1);
            w.TeamOf(0).Reserve.AddRange(new[] { "fighter", "cannon_frigate", "cannon_frigate" });
            Assert.AreEqual("too close to the enemy", w.CannotDeploy(0, new Vec2(20000, 500)));
            Assert.AreEqual(0, w.Deploy(0, new[] { "fighter" }, new Vec2(20000, 500)).Count);
            var one = w.Deploy(0, new[] { "fighter" }, new Vec2(0, 0));
            Assert.AreEqual(1, one.Count);
            Assert.Less(Vec2.Distance(one[0].Pos, Vec2.Zero), 700);
            Assert.GreaterOrEqual(one[0].Arrived, 0);
            var both = w.Deploy(0, new[] { "cannon_frigate", "cannon_frigate", "capital" }, new Vec2(-5000, 0));
            Assert.AreEqual(2, both.Count, "only ships actually in reserve deploy");
            Assert.AreEqual(0, w.TeamOf(0).Reserve.Count);
        }

        [Test]
        public void Reserve_CountsTowardTheFleetCap_ComputerSkipsIt()
        {
            var w = Duel();
            w.ShipCap = 4;
            Run(w, 0.1);
            w.TeamOf(0).Money = 100000;
            w.TeamOf(0).Reserve.Add("scout");
            Assert.IsTrue(w.TryBuy(0, "scout"));
            Assert.AreEqual("fleet is full", w.CannotBuy(0, "scout"), "1 on field + 1 reserve + 1 queued = cap - 1");

            var c = NewWorld();
            c.PlayerTeams.Add(0);
            c.Spawn("capital", 0, new Vec2(-30000, 0), 0);
            c.Spawn("capital", 1, new Vec2(30000, 0), 0);
            c.TeamOf(1).Difficulty = Difficulty.Hard;
            Run(c, 90);
            Assert.AreEqual(0, c.TeamOf(1).Reserve.Count, "the computer deploys straight to its fleet");
            Assert.Greater(c.Count(1), 1);
        }

        [Test]
        public void Economy_CantOverspend_CancelRefunds()
        {
            var w = Duel();
            Run(w, 0.1);
            Assert.AreEqual("not enough credits", w.CannotBuy(0, "capital"));
            Assert.IsFalse(w.TryBuy(0, "capital"));
            Assert.AreEqual("not for sale", w.CannotBuy(0, "super_capital"));
            double before = w.TeamOf(0).Money;
            Assert.IsTrue(w.TryBuy(0, "fighter"));
            Assert.IsTrue(w.CancelBuy(0, "fighter"));
            Assert.AreEqual(before, w.TeamOf(0).Money, 0.01);
            Assert.AreEqual(0, w.TeamOf(0).Queued);
        }

        [Test]
        public void Economy_FleetCapStopsBuying()
        {
            var w = Duel();
            w.ShipCap = 3;
            Run(w, 0.1);
            w.TeamOf(0).Money = 100000;
            Assert.IsTrue(w.TryBuy(0, "scout"));
            Assert.AreEqual("fleet is full", w.CannotBuy(0, "scout"), "cap 3 means two ships, counting the queue");
        }

        [Test]
        public void Economy_KillPaysAThirdOfTheCost()
        {
            var w = Duel();
            Run(w, 0.1);
            var victim = w.Spawn("cannon_frigate", 1, new Vec2(0, 0), 0);
            w.Spawn("laser_frigate", 0, new Vec2(0, 1500), 0);
            w.TeamOf(0).IncomePerSecond = 0; // only the bounty changes the balance
            double before = w.TeamOf(0).Money;
            victim.Shield = -1; victim.Armor = 10;
            for (int i = 0; i < 60 * 30 && !victim.Dead; i++) w.Step(1.0 / 60);
            Assert.IsTrue(victim.Dead, "laser frigate killed it");
            Assert.AreEqual(before + 1050 / 3.0, w.TeamOf(0).Money, 0.01);
        }

        [Test]
        public void Computer_BuysShipsOverTime()
        {
            var w = NewWorld();
            w.PlayerTeams.Add(0);
            w.Spawn("capital", 0, new Vec2(-30000, 0), 0);
            w.Spawn("capital", 1, new Vec2(30000, 0), 0);
            w.TeamOf(1).Difficulty = Difficulty.Hard;
            Run(w, 120);
            Assert.Greater(w.Count(1), 3, "computer bought ships with its income");
        }

        static CampaignDef Campaign() => JsonUtility.FromJson<CampaignDef>(File.ReadAllText("Assets/Data/campaign.json"));

        [Test]
        public void Campaign_EightMissions_SetUpFromData()
        {
            var c = Campaign();
            Assert.AreEqual(8, c.missions.Count);
            foreach (var m in c.missions)
            {
                var w = NewWorld();
                w.SetUpMission(m);
                Assert.AreEqual(m.enemy_fleet.Count, w.Count(1), m.name + ": enemy fleet");
                Assert.Greater(w.Count(0), 0, m.name + ": player has a starting fleet");
                Assert.AreEqual(m.start_money, w.TeamOf(0).Money, 0.01, m.name + ": start money");
                if (m.objective == "kill_target") Assert.NotNull(w.Mission.Target, m.name + ": has a flagship to kill");
            }
        }

        [Test]
        public void Campaign_NoIncomeOutsideSurvive_AndBuildTiersApply()
        {
            var c = Campaign();
            var w = NewWorld();
            w.SetUpMission(c.missions[0]); // Basic Training: destroy all, fighters only
            Run(w, 5);
            Assert.AreEqual(2000, w.TeamOf(0).Money, 0.01, "no income");
            Assert.AreEqual("not available in this mission", w.CannotBuy(0, "cannon_frigate"));
            Assert.IsNull(w.CannotBuy(0, "fighter"));
            var s = NewWorld();
            s.SetUpMission(c.missions[4]); // Military Excursion: survive
            Run(s, 5);
            Assert.Greater(s.TeamOf(0).Money, 4000, "survive missions pay income");
        }

        [Test]
        public void Campaign_KillTarget_WinsWhenFlagshipDies()
        {
            var w = NewWorld();
            w.SetUpMission(Campaign().missions[2]); // System Patrol
            Run(w, 1);
            Assert.Less(w.Winner, 0);
            w.Mission.Target.Armor = -10;
            Run(w, 0.1);
            Assert.AreEqual(0, w.Winner);
            Assert.AreEqual("victory", w.Mission.Outcome);
        }

        [Test]
        public void Campaign_Survive_WinsWhenTimeRunsOut_LosesIfFleetDies()
        {
            var c = Campaign();
            var w = NewWorld();
            w.SetUpMission(c.missions[4]);
            w.Mission.TimeLimit = 3;
            Run(w, 3.1);
            Assert.AreEqual(0, w.Winner, "survived");
            var l = NewWorld();
            l.SetUpMission(c.missions[4]);
            foreach (var s in l.Ships.Where(s => s.Team == 0)) s.Armor = -10;
            Run(l, 0.1);
            Assert.AreEqual(1, l.Winner);
            Assert.AreEqual("your fleet was destroyed", l.Mission.Outcome);
        }

        [Test]
        public void Campaign_DestroyAll_TimeOutIsADefeat()
        {
            var w = NewWorld();
            w.SetUpMission(Campaign().missions[0]);
            w.Mission.TimeLimit = 2;
            foreach (var s in w.Ships) s.Pos = s.Pos * 10; // too far apart to fight in time
            Run(w, 2.1);
            Assert.AreEqual(1, w.Winner);
            Assert.AreEqual("time ran out", w.Mission.Outcome);
        }

        [Test]
        public void Campaign_EnemyClosesInAfterTheHold_FightersNeverSitStill()
        {
            var w = NewWorld();
            w.SetUpMission(Campaign().missions[1]); // Core Outskirts: 3 bombers (2017 would have them retreat forever)
            var enemyStart = w.Ships.Where(s => s.Team == 1).Select(s => s.Pos).ToList();
            var mine = w.Ships.Where(s => s.Team == 0 && Hulls.IsSmall(s.Hull)).ToList();
            var mineStart = mine.Select(s => s.Pos).ToList();
            Run(w, 3);
            for (int i = 0; i < mine.Count; i++)
                Assert.Greater(Vec2.Distance(mine[i].Pos, mineStart[i]), 100, "idle fighters keep flying");
            Run(w, 25);
            var playerCentre = new Vec2(w.Ships.Where(s => s.Team == 0).Average(s => s.Pos.X), 0);
            double before = enemyStart.Average(p => Vec2.Distance(p, playerCentre));
            double after = w.Ships.Where(s => s.Team == 1 && !s.Dead).Select(s => Vec2.Distance(s.Pos, playerCentre)).DefaultIfEmpty(0).Average();
            Assert.Less(after, before - 1500, "the enemy came for the player after its hold");
        }

        [Test]
        public void Tally_CountsDamageLandedAndKills()
        {
            var w = Duel();
            Run(w, 0.1);
            var victim = w.Spawn("cannon_frigate", 1, new Vec2(0, 0), 0);
            var shooter = w.Spawn("laser_frigate", 0, new Vec2(0, 1500), 0);
            victim.Shield = -1; victim.Armor = 1000;
            for (int i = 0; i < 60 * 30 && !victim.Dead; i++) w.Step(1.0 / 60);
            Assert.IsTrue(victim.Dead);
            Assert.AreEqual(1000, shooter.DamageDealt, "only damage that actually landed counts (3,000 laser on 1,000 armour)");
            Assert.AreEqual(1, shooter.Kills);
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
