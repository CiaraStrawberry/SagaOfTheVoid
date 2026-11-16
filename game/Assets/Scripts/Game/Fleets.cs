using System;
using Saga.Sim;

namespace Saga.Game
{
    // Starting fleets for skirmishes and recorded battles. Positions are relative to the fleet's centre,
    // x toward the enemy, y across; mirrored for the second team.
    public static class Fleets
    {
        public static readonly (string id, float x, float y)[] Full =
        {
            ("capital", -900, 0), ("destroyer", -600, 900), ("destroyer", -600, -900),
            ("missile_frigate", -300, 1700), ("missile_frigate", -300, -1700),
            ("laser_frigate", 0, 400), ("cannon_frigate", 0, -400),
            ("flak_frigate", 300, 1200), ("flak_frigate", 300, -1200),
            ("repair_frigate", -1200, 700), ("repair_frigate", -1200, -700),
            ("fighter", 600, 2300), ("fighter", 600, -2300), ("bomber", 400, 2800), ("bomber", 400, -2800),
            ("fighter", 700, 0), ("scout", 900, 1600), ("scout", 900, -1600),
        };

        // One of most kinds, so every effect shows up, in a short fight. For fast iteration.
        public static readonly (string id, float x, float y)[] Quick =
        {
            ("capital", -700, 0), ("missile_frigate", -300, 900), ("laser_frigate", 0, -500), ("cannon_frigate", 0, 500),
            ("flak_frigate", 200, -1200), ("repair_frigate", -900, -800), ("fighter", 500, 1600), ("bomber", 400, -1800),
        };

        // A small, fast skirmish: one destroyer, two frigates and a fighter squadron each.
        public static readonly (string id, float x, float y)[] Small =
        {
            ("destroyer", -500, 0), ("cannon_frigate", 0, 600), ("flak_frigate", 0, -600), ("fighter", 400, 1400),
        };

        // Distance from the middle to each fleet's centre (owner asked for bigger maps: was 4,200 and 2,800).
        public const float FullGap = 6500f, QuickGap = 4500f, SmallGap = 3500f;

        public static (string id, float x, float y)[] FleetFor(BattleConfig c) => c.Small ? Small : c.Quick ? Quick : Full;
        public static float GapFor(BattleConfig c) => c.Small ? SmallGap : c.Quick ? QuickGap : FullGap;

        // Adds each side's flagship or home base (behind its fleet) for the capital ship and base defence modes.
        public static void SpawnMode(World w, SkirmishMode mode, float gap)
        {
            if (mode == SkirmishMode.FreeForAll) return;
            for (int team = 0; team < 2; team++)
            {
                float side = team == 0 ? -1 : 1;
                double heading = side < 0 ? -Math.PI / 2 : Math.PI / 2;
                if (mode == SkirmishMode.CapitalShip)
                    w.KeyShips[team] = w.Spawn("super_capital", team, new Vec2(side * (gap + 2000), 0), heading);
                else
                {
                    var at = new Vec2(side * (gap + 2800), 0);
                    w.KeyShips[team] = w.Spawn("home_base", team, at, heading);
                    foreach (var (dx, dy) in new[] { (1200.0, 1200.0), (1200.0, -1200.0), (-1200.0, 1200.0), (-1200.0, -1200.0) })
                        w.Spawn("defence_platform", team, at + new Vec2(dx * -side, dy), heading);
                }
            }
        }

        // Team 0 starts on the left facing right, team 1 on the right facing left, `gap` from the middle.
        public static void Spawn(World w, (string id, float x, float y)[] fleet, float gap)
        {
            for (int team = 0; team < 2; team++)
            {
                float side = team == 0 ? -1 : 1;
                foreach (var e in fleet)
                    w.Spawn(e.id, team, new Vec2(side * (gap - e.x), e.y * (team == 0 ? 1 : -1)), side < 0 ? -Math.PI / 2 : Math.PI / 2);
            }
        }
    }
}
