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
            ("repair_frigate", -1200, 700), ("support_frigate", -1200, -700),
            ("fighter", 600, 2300), ("fighter", 600, -2300), ("bomber", 400, 2800), ("bomber", 400, -2800),
            ("attack_craft", 700, 0), ("scout", 900, 1600), ("scout", 900, -1600),
        };

        // One of most kinds, so every effect shows up, in a short fight. For fast iteration.
        public static readonly (string id, float x, float y)[] Quick =
        {
            ("capital", -700, 0), ("missile_frigate", -300, 900), ("laser_frigate", 0, -500), ("cannon_frigate", 0, 500),
            ("flak_frigate", 200, -1200), ("support_frigate", -900, -800), ("fighter", 500, 1600), ("bomber", 400, -1800),
        };

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
