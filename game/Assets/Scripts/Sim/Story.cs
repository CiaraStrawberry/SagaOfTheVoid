using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Saga.Sim
{
    // Campaign story: dialogue beats per mission (Assets/Data/narrative.json, built by tools/make_narrative.py from
    // the owner's narrative package "The Thessa Record"), and when each one plays.

    [Serializable] public class StoryLine { public string speaker; public string text; }
    [Serializable] public class StoryBeat { public string id; public string trigger; public string type; public List<StoryLine> lines = new List<StoryLine>(); }
    [Serializable]
    public class StoryMission
    {
        public int index;
        public string name, act, premise, primary, secondary, bonus;
        public List<StoryBeat> beats = new List<StoryBeat>();
    }
    [Serializable] public class StoryCharacter { public string id, name, role, faction, color; }
    [Serializable] public class StoryEnding { public string id, choice, title, text; }
    [Serializable]
    public class StoryDef
    {
        public string title;
        public List<StoryCharacter> characters = new List<StoryCharacter>();
        public List<StoryMission> missions = new List<StoryMission>();
        public List<StoryEnding> endings = new List<StoryEnding>();

        public StoryMission Mission(int index) => missions.FirstOrDefault(m => m.index == index);
        public StoryCharacter Character(string id) => characters.FirstOrDefault(c => c.id == id);
    }

    // Decides which beats are due. Triggers (from the narrative package):
    //   mission_start, mission_victory, first_enemy_destroyed,
    //   "<measure> <op> <number>" with measure one of enemy_kills, remaining_enemies, target_hull,
    //   enemy_destroyer_hull (fractions of armour left) or time_remaining (seconds), and op <=, >=, <, >.
    // Every beat plays once. A battle beat whose moment never came (the target died too fast) plays at victory,
    // before the victory beat, so no part of the story is skipped. Nothing plays after a defeat.
    public class StoryDirector
    {
        public readonly StoryMission Mission;
        readonly World world;
        readonly int player, enemy;
        readonly HashSet<StoryBeat> played = new HashSet<StoryBeat>();

        public StoryDirector(StoryMission mission, World world, int player, int enemy)
        {
            Mission = mission; this.world = world; this.player = player; this.enemy = enemy;
        }

        // Commander Sen is only a voice on a pirate relay until she names herself in mission 6 (index 5).
        public static bool Hidden(string speaker, int missionIndex) => speaker == "sen" && missionIndex < 5;

        public bool Played(StoryBeat b) => played.Contains(b);

        // The next beat due during the battle (mission start or a battle trigger), or null.
        public StoryBeat Due()
        {
            if (world.Winner >= 0) return null;
            foreach (var b in Mission.beats)
            {
                if (played.Contains(b) || b.trigger == "mission_victory") continue;
                if (Fires(b.trigger)) { played.Add(b); return b; }
            }
            return null;
        }

        // On a player victory: any battle beats that never fired, then the victory beats.
        public List<StoryBeat> AtVictory()
        {
            var list = new List<StoryBeat>();
            if (world.Winner != player) return list;
            foreach (var b in Mission.beats.Where(b => b.trigger != "mission_victory" && b.trigger != "mission_start" && !played.Contains(b)))
                list.Add(b);
            list.AddRange(Mission.beats.Where(b => b.trigger == "mission_victory" && !played.Contains(b)));
            foreach (var b in list) played.Add(b);
            return list;
        }

        bool Fires(string trigger)
        {
            if (trigger == "mission_start") return true;
            if (trigger == "first_enemy_destroyed") return EnemyKills() >= 1;
            var parts = trigger.Split(' ');
            if (parts.Length != 3 || !double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) return false;
            var measured = Measure(parts[0]);
            if (!measured.HasValue) return false;
            double m = measured.Value;
            switch (parts[1])
            {
                case "<=": return m <= value;
                case ">=": return m >= value;
                case "<": return m < value;
                case ">": return m > value;
                default: return false;
            }
        }

        int EnemyKills() => world.Ships.Count(s => s.Team == enemy && s.Dead);

        static double HullLeft(Ship s) => s.Dead ? 0 : Math.Max(0, s.Armor) / (double)Math.Max(1, s.StartArmor);

        double? Measure(string what)
        {
            switch (what)
            {
                case "enemy_kills": return EnemyKills();
                case "remaining_enemies": return world.Count(enemy);
                case "target_hull":
                    var t = world.Mission?.Target;
                    return t == null ? (double?)null : HullLeft(t);
                case "enemy_destroyer_hull":
                    var d = world.Ships.Where(s => s.Team == enemy && s.Hull == Hull.Destroyer).ToList();
                    return d.Count == 0 ? (double?)null : d.Min(HullLeft);
                case "time_remaining":
                    return world.Mission == null ? (double?)null : world.Mission.TimeLimit - world.Time;
                default: return null;
            }
        }
    }
}
