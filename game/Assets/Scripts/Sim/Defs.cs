using System;
using System.Collections.Generic;

namespace Saga.Sim
{
    // Data shapes for Assets/Data/ships.json and weapons.json (built by tools/make_data.py).
    // Field names match the JSON so Unity's JsonUtility can fill them.

    [Serializable]
    public class TurretDef
    {
        public string weapon;
        public float range;
        public float time_between_shots;
        public int shots_per_burst;
        public float burst_wait;
    }

    [Serializable]
    public class ShipDef
    {
        public string id;
        public string name;
        public string sprite;
        public string hull;
        public int cost;
        public bool buyable;
        public float build_time;   // seconds in the build queue (2017 build menu values)
        public int armor;
        public int shield;
        public float speed;
        public float turn_rate;
        public float sprite_world_size;
        public bool squadron;
        public List<TurretDef> turrets = new List<TurretDef>();

        public Hull HullType => Hulls.Parse(hull);
    }

    [Serializable]
    public class HitTable
    {
        public float light, corvette, frigate, cruiser, destroyer, capital, super_capital;

        public float For(Hull h)
        {
            switch (h)
            {
                case Hull.Light: return light;
                case Hull.Corvette: return corvette;
                case Hull.Frigate: return frigate;
                case Hull.Cruiser: return cruiser;
                case Hull.Destroyer: return destroyer;
                case Hull.Capital: return capital;
                default: return super_capital;
            }
        }
    }

    [Serializable]
    public class WeaponDef
    {
        public string id;
        public int damage;
        public int damage_vs_capital_hull_owner;
        public int shield_restore;
        public string prefers;
        public HitTable hit;
        public bool always_hits_non_light;
        public bool never_targets_light;
        public bool targets_friends;
        public bool module;
        public string visual;
        public float projectile_speed = 440f;
        public float launch_delay = 0.5f;   // seconds added to the flight time (2017: 0.5)
        public float travel_time;
        public float aura_radius;        // shield_field: reach of the field
        public float shield_regen_pct;   // shield_field: % of max shield restored per second
    }

    [Serializable] public class ShipDefList { public List<ShipDef> ships = new List<ShipDef>(); }
    [Serializable] public class WeaponDefList { public List<WeaponDef> weapons = new List<WeaponDef>(); }

    public enum Hull { Light, Corvette, Frigate, Cruiser, Destroyer, Capital, SuperCapital }

    public static class Hulls
    {
        public static Hull Parse(string s)
        {
            switch (s)
            {
                case "light": return Hull.Light;
                case "corvette": return Hull.Corvette;
                case "frigate": return Hull.Frigate;
                case "cruiser": return Hull.Cruiser;
                case "destroyer": return Hull.Destroyer;
                case "capital": return Hull.Capital;
                case "super_capital": return Hull.SuperCapital;
                default: throw new ArgumentException("unknown hull " + s);
            }
        }

        // Fighters and corvettes fly as squadrons: always at full speed, damage knocks out craft in steps.
        public static bool IsSmall(Hull h) => h == Hull.Light || h == Hull.Corvette;
    }

    public class Rulebook
    {
        public readonly Dictionary<string, ShipDef> Ships = new Dictionary<string, ShipDef>();
        public readonly Dictionary<string, WeaponDef> Weapons = new Dictionary<string, WeaponDef>();

        public Rulebook(ShipDefList ships, WeaponDefList weapons)
        {
            foreach (var s in ships.ships) Ships[s.id] = s;
            foreach (var w in weapons.weapons) Weapons[w.id] = w;
            foreach (var s in ships.ships)
                foreach (var t in s.turrets)
                    if (!Weapons.ContainsKey(t.weapon))
                        throw new ArgumentException(s.id + " uses unknown weapon " + t.weapon);
        }
    }
}
