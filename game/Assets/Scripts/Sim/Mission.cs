using System;
using System.Collections.Generic;
using System.Linq;

namespace Saga.Sim
{
    // Campaign data shapes for Assets/Data/campaign.json (built by tools/make_campaign.py).
    [Serializable]
    public class MissionDef
    {
        public int index;
        public string name;
        public List<string> enemy_fleet = new List<string>();
        public string map;
        public string sky;
        public int start_money;
        public string objective;      // destroy_all, survive, kill_target
        public int time_limit;        // seconds
        public string ships_allowed;  // fighters, fighters_and_frigates, all
        public string story;
    }

    [Serializable]
    public class MapDef { public string id; public string scene_2017; public string contents; }

    [Serializable]
    public class CampaignDef
    {
        public List<MapDef> maps = new List<MapDef>();
        public List<MissionDef> missions = new List<MissionDef>();
    }

    public enum Objective { DestroyAll, Survive, KillTarget }

    // How a campaign mission is won and lost, and how it bends the skirmish rules (2017 sources:
    // UnitMovementcommandcontroller win checks and timer, AIController mission behaviour).
    public class MissionRules
    {
        public Objective Objective;
        public double TimeLimit;
        public int PlayerTeam, EnemyTeam;
        public Ship Target;               // kill_target: the enemy flagship
        public double EnemyHoldUntil = 15; // enemy waits this long before attacking (not in Survive); 2017 used 30 s
        public HashSet<string> Allowed = new HashSet<string>();
        public string Outcome;            // null while running; then "victory" or a reason for defeat

        public static Objective Parse(string s) =>
            s == "survive" ? Objective.Survive : s == "kill_target" ? Objective.KillTarget : Objective.DestroyAll;

        // Which ships the player may build (2017 build menu groups: Fighters, Frigates, Special).
        public static HashSet<string> AllowedShips(string tier)
        {
            var fighters = new[] { "scout", "fighter", "bomber", "attack_craft" };
            var frigates = new[] { "missile_frigate", "flak_frigate", "cannon_frigate", "laser_frigate", "support_frigate", "repair_frigate" };
            var special = new[] { "destroyer", "capital" };
            var set = new HashSet<string>(fighters);
            if (tier != "fighters") set.UnionWith(frigates);
            if (tier == "all") set.UnionWith(special);
            return set;
        }

        // A starting fleet for the player, by tier. Stand-in: in 2017 it came from the player's fleet builder.
        public static string[] StarterFleet(string tier) =>
            tier == "fighters" ? new[] { "fighter", "fighter", "scout", "bomber" }
            : tier == "fighters_and_frigates" ? new[] { "cannon_frigate", "missile_frigate", "fighter", "fighter", "bomber" }
            : new[] { "destroyer", "cannon_frigate", "missile_frigate", "repair_frigate", "fighter", "bomber" };
    }

    public partial class World
    {
        public MissionRules Mission;

        // Sets the world up as a campaign mission: player fleet on the left, enemy fleet on the right.
        public void SetUpMission(MissionDef def, int playerTeam = 0, int enemyTeam = 1)
        {
            var m = new MissionRules
            {
                Objective = MissionRules.Parse(def.objective),
                TimeLimit = def.time_limit,
                PlayerTeam = playerTeam,
                EnemyTeam = enemyTeam,
                Allowed = MissionRules.AllowedShips(def.ships_allowed),
            };
            if (m.Objective == Objective.Survive) m.EnemyHoldUntil = 0;
            Mission = m;
            PlayerTeams.Add(playerTeam);

            var starter = MissionRules.StarterFleet(def.ships_allowed);
            for (int i = 0; i < starter.Length; i++)
                Spawn(starter[i], playerTeam, new Vec2(-6500 + (i % 3) * -500, (i / 3 - 0.5) * 900 + (i % 3) * 300 - 300), -Math.PI / 2);

            // Enemy fleet in a column on the right (2017 spawned it one ship at a time down a line).
            for (int i = 0; i < def.enemy_fleet.Count; i++)
            {
                var s = Spawn(InPlay(def.enemy_fleet[i]), enemyTeam, new Vec2(6500 + (i % 2) * 600, (i - def.enemy_fleet.Count / 2.0) * 700), Math.PI / 2);
                if (def.enemy_fleet[i] == "super_capital" && m.Objective == Objective.KillTarget) m.Target = s;
            }

            var me = TeamOf(playerTeam);
            me.Money = def.start_money;
            // Campaign: no income except in Survive missions (2017: UnitMovementcommandcontroller.cs:296-299).
            if (m.Objective != Objective.Survive) me.IncomePerSecond = 0;
            var them = TeamOf(enemyTeam);
            them.Money = 0;
            them.BuysShips = m.Objective == Objective.Survive;
            them.IgnoreDifficulty = true;
            them.IncomePerSecond = m.Objective == Objective.Survive ? (def.name == "Final Assault" ? 80 : 70) : 0;
            them.HoldUntil = m.EnemyHoldUntil;
        }

        // Called every step while a mission runs.
        void CheckMission()
        {
            var m = Mission;
            if (m == null || m.Outcome != null) return;
            bool playerAlive = Count(m.PlayerTeam) > 0;
            bool enemyAlive = Count(m.EnemyTeam) > 0;
            if (!playerAlive) { End(m, m.EnemyTeam, "your fleet was destroyed"); return; }
            switch (m.Objective)
            {
                case Objective.DestroyAll:
                    if (!enemyAlive) End(m, m.PlayerTeam, "victory");
                    break;
                case Objective.KillTarget:
                    if (m.Target != null && m.Target.Dead) End(m, m.PlayerTeam, "victory");
                    break;
                case Objective.Survive:
                    if (Time >= m.TimeLimit) End(m, m.PlayerTeam, "victory");
                    break;
            }
            if (m.Outcome == null && m.Objective != Objective.Survive && Time >= m.TimeLimit) End(m, m.EnemyTeam, "time ran out");
        }

        void End(MissionRules m, int winner, string outcome)
        {
            m.Outcome = outcome;
            Winner = winner;
        }
    }
}
