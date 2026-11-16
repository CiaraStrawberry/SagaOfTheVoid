using System;
using System.Collections.Generic;
using System.Linq;

namespace Saga.Sim
{
    public enum Difficulty { Easy, Normal, Hard }

    public class BuildOrder
    {
        public string ShipId;
        public int Count;
        public double Progress;   // seconds spent on the current ship
    }

    // Money, the build queue and (for the computer) its buying plan, per team.
    // 2017 sources: UnitMovementcommandcontroller (income, cap, bounty), BuildQueController (queue),
    // AIController (computer money, difficulty, getshiptobuy).
    public class TeamState
    {
        public int Team;
        public double Money;
        public double IncomePerSecond = World.Income;
        public Difficulty Difficulty = Difficulty.Normal;
        public readonly List<BuildOrder> Queue = new List<BuildOrder>();
        public Vec2 Home;            // where the fleet started; retreat point and fallback spawn point
        public bool HomeSet;
        public bool Retreating;
        public double HoldUntil;        // computer waits at home until this time (campaign: 30 s)
        public bool BuysShips = true;   // computer buys (campaign: only in Survive missions)
        public bool IgnoreDifficulty;   // campaign income is fixed, not scaled by difficulty

        // computer buying plan (AIController)
        public string NextBuy;
        public string LastBuy = "scout";

        public int Queued => Queue.Sum(o => o.Count);
        public BuildOrder Building => Queue.Count > 0 ? Queue[0] : null;
    }

    public partial class World
    {
        public const double StartMoney = 1500, ComputerStartMoney = 1000, Income = 30, MoneyCap = 7000;
        public const int ShipCap1v1 = 23;  // 2017 getmaxshipnumbers: 23 (1v1), 14 (2v2), 9 (3v3); buying stops one short

        public bool EconomyEnabled = true;
        public int ShipCap = ShipCap1v1;
        readonly Dictionary<int, TeamState> teams = new Dictionary<int, TeamState>();

        // Ship numbers in the 2017 build menu order (UnitMovementcommandcontroller.getshipbynumber).
        public static readonly string[] BuyOrder =
        {
            null, "scout", "fighter", "bomber", "attack_craft", "missile_frigate", "flak_frigate", "cannon_frigate",
            "laser_frigate", "support_frigate", "repair_frigate", "destroyer", "capital",
        };

        public TeamState TeamOf(int team)
        {
            if (!teams.TryGetValue(team, out var t))
            {
                t = new TeamState { Team = team, Money = PlayerTeams.Contains(team) ? StartMoney : ComputerStartMoney };
                teams[team] = t;
            }
            return t;
        }

        public IEnumerable<TeamState> Teams => teams.Values;

        public static double DifficultyIncome(Difficulty d) => d == Difficulty.Easy ? 0.6 : d == Difficulty.Normal ? 0.9 : 1.4;

        // Why a ship can't be bought right now, or null if it can.
        public string CannotBuy(int team, string shipId)
        {
            if (!Rules.Ships.TryGetValue(shipId, out var def) || !def.buyable) return "not for sale";
            var t = TeamOf(team);
            if (Count(team) == 0) return "fleet destroyed";
            if (Mission != null && team == Mission.PlayerTeam && !Mission.Allowed.Contains(shipId)) return "not available in this mission";
            if (Mission?.Outcome != null) return "mission over";
            if (Count(team) + t.Queued >= ShipCap - 1) return "fleet is full";
            if (t.Money < def.cost) return "not enough credits";
            return null;
        }

        // Pays now and queues the ship (repeats of the same kind stack, as in 2017).
        public bool TryBuy(int team, string shipId)
        {
            if (CannotBuy(team, shipId) != null) return false;
            var t = TeamOf(team);
            t.Money -= Rules.Ships[shipId].cost;
            var same = t.Queue.FirstOrDefault(o => o.ShipId == shipId);
            if (same != null) same.Count++;
            else t.Queue.Add(new BuildOrder { ShipId = shipId, Count = 1 });
            return true;
        }

        // Removes one queued ship of this kind and refunds it.
        public bool CancelBuy(int team, string shipId)
        {
            var t = TeamOf(team);
            var o = t.Queue.LastOrDefault(x => x.ShipId == shipId);
            if (o == null) return false;
            o.Count--;
            if (o.Count == 0) { t.Queue.Remove(o); }
            t.Money += Rules.Ships[shipId].cost;
            return true;
        }

        public double BuildTime(string shipId) => Math.Max(1.0, Rules.Ships[shipId].build_time);

        void StepEconomy(double dt)
        {
            foreach (var team in Alive.Select(s => s.Team).Distinct().ToList())
            {
                var t = TeamOf(team);
                if (!t.HomeSet)
                {
                    var mine = Alive.Where(s => s.Team == team).ToList();
                    t.Home = new Vec2(mine.Average(s => s.Pos.X), mine.Average(s => s.Pos.Y));
                    t.HomeSet = true;
                }
                if (!EconomyEnabled) continue;

                // Income: +30 a second while under the 7,000 cap (2017 addmoney checks before adding).
                double rate = PlayerTeams.Contains(team) || t.IgnoreDifficulty ? t.IncomePerSecond : t.IncomePerSecond * DifficultyIncome(t.Difficulty);
                if (t.Money < MoneyCap) t.Money += rate * dt;

                var b = t.Building;
                if (b == null) continue;
                b.Progress += dt;
                if (b.Progress < BuildTime(b.ShipId)) continue;
                b.Progress = 0;
                b.Count--;
                if (b.Count <= 0) t.Queue.RemoveAt(0);
                SpawnBought(team, b.ShipId);
            }
        }

        // New ships appear beside the fleet (2017 getspawnpos: the fleet's average position), facing the enemy.
        void SpawnBought(int team, string shipId)
        {
            var t = TeamOf(team);
            var mine = Alive.Where(s => s.Team == team).ToList();
            var at = mine.Count > 0 ? new Vec2(mine.Average(s => s.Pos.X), mine.Average(s => s.Pos.Y)) : t.Home;
            var enemies = Alive.Where(s => s.Team != team).ToList();
            var toward = enemies.Count > 0 ? new Vec2(enemies.Average(s => s.Pos.X), enemies.Average(s => s.Pos.Y)) - at : new Vec2(1, 0);
            // Behind the fleet, a little to one side so arrivals don't stack.
            var back = toward.Normalized * -600;
            var side = new Vec2(-toward.Normalized.Y, toward.Normalized.X) * ((rng.NextDouble() - 0.5) * 1200);
            var s = Spawn(shipId, team, at + back + side, Vec2.HeadingOf(toward));
            s.Arrived = Time;
        }

        // Kill bounty: a third of the ship's cost to the team that landed the last hit.
        void PayBounty(Ship dead)
        {
            if (!EconomyEnabled || dead.LastHitByTeam < 0 || dead.LastHitByTeam == dead.Team) return;
            if (Mission != null && dead.LastHitByTeam == Mission.PlayerTeam) return; // 2017: no bounty for the player in the campaign
            TeamOf(dead.LastHitByTeam).Money += dead.Def.cost / 3.0;
        }

        // Computer buying (AIController.BuyShip/getshiptobuy): roll 0-199 on a weighted list that leans toward
        // frigates; above 100 repeats the last purchase. Keeps trying the same pick until it can afford it.
        void ComputerBuys(int team)
        {
            if (!EconomyEnabled) return;
            var t = TeamOf(team);
            if (!t.BuysShips) return;
            if (t.NextBuy == null)
            {
                int roll = rng.Next(0, 200);
                t.NextBuy = roll > 100 ? t.LastBuy : BuyOrder[PickNumber(roll)];
            }
            if (TryBuy(team, t.NextBuy)) { t.LastBuy = t.NextBuy; t.NextBuy = null; }
        }

        static int PickNumber(int roll)
        {
            if (roll <= 5) return 1;     // scout
            if (roll <= 15) return 2;    // fighter
            if (roll <= 25) return 3;    // bomber
            if (roll <= 30) return 4;    // attack craft
            if (roll <= 40) return 5;    // missile frigate
            if (roll <= 52) return 6;    // flak frigate
            if (roll <= 64) return 7;    // cannon frigate
            if (roll <= 76) return 8;    // laser frigate
            if (roll <= 82) return 9;    // support frigate
            if (roll <= 92) return 10;   // repair frigate
            if (roll <= 97) return 11;   // destroyer
            return 12;                   // capital
        }
    }
}
