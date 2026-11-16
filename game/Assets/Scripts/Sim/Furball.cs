using System.Collections.Generic;
using System.Linq;

namespace Saga.Sim
{
    // A dogfight area: once enemy fighter squadrons meet, they stay tangled up circling one fixed point
    // (Battlefleet Gothic style). Any number of squadrons from either side can be in the same fight.
    public class Furball
    {
        public int Id;
        public Vec2 Centre;
        public double Radius = World.FurballRadius;
        public readonly List<Ship> Members = new List<Ship>();
        public IEnumerable<Ship> Alive => Members.Where(m => !m.Dead);
        public bool Contested => Alive.Select(m => m.Team).Distinct().Count() > 1;
    }

    public partial class World
    {
        public const double FurballRadius = 700;
        public readonly List<Furball> Furballs = new List<Furball>();
        int nextFurballId = 1;

        public Furball FurballAt(Vec2 p) => Furballs.FirstOrDefault(f => Vec2.Distance(f.Centre, p) <= f.Radius);

        void Join(Ship s, Furball f)
        {
            if (s.Furball == f) return;
            Leave(s);
            s.Furball = f;
            f.Members.Add(s);
        }

        public void Leave(Ship s)
        {
            if (s.Furball == null) return;
            s.Furball.Members.Remove(s);
            s.Furball = null;
        }

        static bool PlainMove(Ship s) => s.HasMoveOrder && !s.Holds; // a player move order pulls fighters out

        // Once a second: form, join and close fights.
        void UpdateFurballs()
        {
            foreach (var f in Furballs) f.Members.RemoveAll(m => m.Dead || m.Furball != f);

            foreach (var s in Alive.Where(x => Hulls.IsSmall(x.Hull) && x.Furball == null && !PlainMove(x)).ToList())
            {
                var t = s.Target;
                if (t == null || t.Dead || !Hulls.IsSmall(t.Hull) || Vec2.Distance(s.Pos, t.Pos) >= DogfightRange) continue;
                var f = t.Furball
                        ?? Furballs.FirstOrDefault(x => Vec2.Distance(x.Centre, s.Pos) < x.Radius + 600)
                        ?? NewFurball((s.Pos + t.Pos) / 2);
                Join(s, f);
                if (t.Furball == null && !PlainMove(t)) Join(t, f);
            }

            // A fight with only one side left is over; its fighters go back to their orders.
            foreach (var f in Furballs.Where(f => !f.Contested).ToList())
            {
                foreach (var m in f.Members.ToList())
                {
                    Leave(m);
                    // Attack-moving fighters carry on to where they were sent.
                    if (m.AttackMoveTo.HasValue) { m.MoveTarget = m.AttackMoveTo.Value; m.HasMoveOrder = true; m.Holds = true; }
                }
                Furballs.Remove(f);
            }
        }

        Furball NewFurball(Vec2 at)
        {
            var f = new Furball { Id = nextFurballId++, Centre = at };
            Furballs.Add(f);
            return f;
        }

        // Player command on a fight area: fighters join the fight, everything else attacks the enemies in it.
        public void OrderIntoFight(IList<Ship> ships, Furball f, int team)
        {
            var enemies = f.Alive.Where(m => m.Team != team).ToList();
            foreach (var s in ships.Where(x => !x.Dead))
            {
                if (Hulls.IsSmall(s.Hull))
                {
                    s.Waypoints.Clear();
                    s.OrderedTarget = null;
                    s.AttackMoveTo = null;
                    s.MoveTarget = f.Centre;
                    s.HasMoveOrder = true;
                    s.Holds = true;
                    Join(s, f);
                }
                else if (enemies.Count > 0)
                    OrderAttack(new[] { s }, enemies.OrderBy(e => Vec2.Distance(e.Pos, s.Pos)).First());
            }
        }
    }
}
