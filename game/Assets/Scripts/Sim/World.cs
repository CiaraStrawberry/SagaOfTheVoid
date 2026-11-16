using System;
using System.Collections.Generic;
using System.Linq;

namespace Saga.Sim
{
    public class Gun
    {
        public readonly TurretDef Turret;
        public readonly WeaponDef Weapon;
        public double Timer;
        public double NextDelay;
        public int ShotsInBurst;

        public Gun(TurretDef t, WeaponDef w) { Turret = t; Weapon = w; }
    }

    public class Ship
    {
        public int Id;
        public int Team;
        public ShipDef Def;
        public Hull Hull;
        public Vec2 Pos;
        public double Heading;
        public double Speed;
        public double TurnRate;
        public int Armor, Shield, StartArmor, StartShield;
        public Ship Target;
        public Vec2 MoveTarget;
        public bool HasMoveOrder;
        public bool Accelerating;
        public bool InShieldField;   // inside a friendly Support Frigate's shield field this step
        public double ShieldCarry;   // fractional shield points not yet applied
        public bool Dead;
        public double NearestEnemy = double.MaxValue;
        public Ship OrderedTarget;                           // player/AI attack order (2017: shiptarget)
        public readonly Queue<Vec2> Waypoints = new Queue<Vec2>(); // queued move points (2017: StoredOrders)
        public bool Holds = true;                            // HoldAtRange applies; off for plain move orders
        public int LastHitByTeam = -1;                       // who gets the kill bounty
        public Ship LastHitBy;                               // who gets the kill on the tally
        public long DamageDealt;                             // running tally: damage landed on enemies (shields and armour)
        public int Kills;
        public long ShieldsRestored;                         // repair ships
        public Vec2 Loiter; public bool LoiterSet;           // where an idle fighter squadron circles
        public double Arrived = -1;                          // world time a bought ship arrived (-1: started on the field)
        public readonly List<Gun> Guns = new List<Gun>();

        public Gun MainGun => Guns.FirstOrDefault(g => !g.Weapon.module);
        public Gun ShieldField => Guns.FirstOrDefault(g => g.Weapon.aura_radius > 0);
        public double MaxRange => Guns.Count == 0 ? 0 : Guns.Max(g => g.Turret.range);
        // Where a ship holds when HoldAtRange is on: inside the reach of its shortest damaging gun, so every gun fires.
        public double HoldRange => Guns.Where(g => !g.Weapon.module && g.Weapon.damage > 0).Select(g => (double)g.Turret.range).DefaultIfEmpty(1500).Min();
        public Vec2 Forward => Vec2.FromHeading(Heading);
    }

    // One fired shot, for the renderer. Damage is applied later by the world, when the shot lands.
    public struct ShotEvent
    {
        public Ship From, To;
        public int ShotId;
        public int GunIndex;    // which of the ship's guns fired (bursts keep their direction)
        public string Visual;
        public bool Hit;
        public bool AtShield;   // target's shields were up when fired
        public double TravelTime;
    }

    public struct ImpactEvent
    {
        public Ship Target;
        public Ship Source;
        public int ShotId;
        public bool OnShield;
        public bool Repair;
        public string Visual;
    }

    class PendingHit
    {
        public Ship Target, Source;
        public int ShotId;
        public int Damage;
        public bool Repair;
        public double TimeLeft;
        public string Visual;
    }

    // The battle simulation. Rules are taken from the 2017 code, flattened to a plane:
    //   movement  CustomPathfinding.SpecUpdate / Look / Move
    //   targeting _Ship.FindTarget / getpreferance
    //   firing    _Weapon.UpdateSpec / ishit / getmodifyer / hittime
    //   damage    _Ship.takeDamage / addShield
    //   shield field  new: replaces the 2017 visual-only cloak (decided 2026-11-16)
    //   computer  AIController (attack with more than 4 ships, else regroup)
    // Known old bugs fixed here: lasers hit once, and shots land after their travel time.
    public partial class World
    {
        public const double UpdateRate = 60;            // the old game moved ships once per 60th of a second
        const double Acceleration = 4 * UpdateRate;      // CustomPathfinding.acceleration, per second
        const double BrakeDistance = 300;                // mindecdistance
        const double ArriveDistance = 10;
        const double TurnScale = 2.0;                    // old Look() ran twice per frame (Update + synced update)
        const double MaxTurnLight = 4.0;
        // Fighters and corvettes turn on a circle at least this wide (2017 craft looped in ~90 units, which looked jittery).
        public const double SmallTurnRadius = 380;

        public readonly Rulebook Rules;
        public readonly List<Ship> Ships = new List<Ship>();
        public readonly List<ShotEvent> Shots = new List<ShotEvent>();
        public readonly List<ImpactEvent> Impacts = new List<ImpactEvent>();
        public readonly List<Ship> Deaths = new List<Ship>();
        public double Time;
        public int Winner = -1;

        // Not in the original: big ships stop just inside the range of their shortest gun instead of
        // flying into the enemy's position (approved 2026-11-16). Set false for the 2017 charge.
        public bool HoldAtRange = true;

        // Teams a person controls. The computer gives orders to every other team.
        public readonly HashSet<int> PlayerTeams = new HashSet<int>();

        readonly Random rng;
        readonly List<PendingHit> pending = new List<PendingHit>();
        double secondTimer;
        int nextId = 1, nextShotId = 1;

        public World(Rulebook rules, int seed)
        {
            Rules = rules;
            rng = new Random(seed);
        }

        public Ship Spawn(string shipId, int team, Vec2 pos, double heading)
        {
            var def = Rules.Ships[shipId];
            var s = new Ship
            {
                Id = nextId++, Team = team, Def = def, Hull = def.HullType, Pos = pos, Heading = heading,
                Armor = def.armor, Shield = def.shield, StartArmor = def.armor, StartShield = def.shield,
                TurnRate = def.turn_rate,
            };
            foreach (var t in def.turrets) s.Guns.Add(new Gun(t, Rules.Weapons[t.weapon]) { NextDelay = t.time_between_shots });
            Ships.Add(s);
            return s;
        }

        public IEnumerable<Ship> Alive => Ships.Where(s => !s.Dead);
        public int Count(int team) => Ships.Count(s => !s.Dead && s.Team == team);

        // Computer orders: move and engage on the way (holds at range).
        public void Order(Ship s, Vec2 target)
        {
            s.MoveTarget = target;
            s.HasMoveOrder = true;
            s.Accelerating = true;
            s.Holds = true;
        }

        // Player move: the group keeps its shape around the point and goes exactly there, firing on the way.
        // With queue, the point is added after the ships' current orders (2017 waypoints, popped within 400).
        public void OrderMove(IList<Ship> ships, Vec2 point, bool queue)
        {
            var live = ships.Where(x => !x.Dead).ToList();
            if (live.Count == 0) return;
            var centre = new Vec2(live.Average(x => x.Pos.X), live.Average(x => x.Pos.Y));
            foreach (var x in live)
            {
                var offset = x.Pos - centre;
                if (offset.Length > 900) offset = offset.Normalized * 900;
                var p = point + offset;
                if (queue && x.HasMoveOrder) { x.Waypoints.Enqueue(p); continue; }
                x.Waypoints.Clear();
                x.OrderedTarget = null;
                x.MoveTarget = p;
                x.HasMoveOrder = true;
                x.Accelerating = true;
                x.Holds = false;
            }
        }

        // Player attack: chase the target and fire on it first whenever it is in range.
        public void OrderAttack(IList<Ship> ships, Ship target)
        {
            if (target == null || target.Dead) return;
            foreach (var x in ships.Where(x => !x.Dead && x.Team != target.Team))
            {
                x.Waypoints.Clear();
                x.OrderedTarget = target;
                x.MoveTarget = target.Pos;
                x.HasMoveOrder = true;
                x.Accelerating = true;
                x.Holds = true;
            }
        }

        public void OrderStop(IList<Ship> ships)
        {
            foreach (var x in ships)
            {
                x.Waypoints.Clear();
                x.OrderedTarget = null;
                x.HasMoveOrder = false;
                x.Speed = 0;
            }
        }

        // Clears the event lists, then advances the battle by dt seconds.
        public void Step(double dt)
        {
            Time += dt;
            secondTimer += dt;
            if (secondTimer >= 1.0)
            {
                secondTimer -= 1.0;
                foreach (var team in Alive.Select(s => s.Team).Distinct().ToList())
                    if (!PlayerTeams.Contains(team)) { ComputerBuys(team); ComputerOrders(team); }
                foreach (var s in Alive)
                {
                    if (s.OrderedTarget != null && s.OrderedTarget.Dead) { s.OrderedTarget = null; s.HasMoveOrder = false; }
                    if (s.OrderedTarget != null) s.MoveTarget = s.OrderedTarget.Pos;
                }
                foreach (var s in Alive)
                {
                    s.Target = FindTarget(s);
                    s.NearestEnemy = Alive.Where(o => o.Team != s.Team).Select(o => Vec2.Distance(o.Pos, s.Pos)).DefaultIfEmpty(double.MaxValue).Min();
                }
            }
            ApplyShieldFields(dt);
            StepEconomy(dt);

            foreach (var s in Alive) Move(s, dt);
            PushApart(dt);
            foreach (var s in Alive.ToList()) Fire(s, dt);
            LandShots(dt);

            foreach (var s in Ships.Where(s => !s.Dead && s.Armor < 0).ToList())
            {
                s.Dead = true;
                if (s.LastHitBy != null && s.LastHitBy.Team != s.Team) s.LastHitBy.Kills++;
                PayBounty(s);
                Deaths.Add(s);
            }
            if (Mission != null) CheckMission();
            else if (Winner < 0)
            {
                var teams = Alive.Select(s => s.Team).Distinct().ToList();
                if (teams.Count == 1 && Time > 1) Winner = teams[0];
            }
        }

        public void ClearEvents()
        {
            Shots.Clear();
            Impacts.Clear();
            Deaths.Clear();
        }

        // ---------- computer player (AIController, simplified: no buying yet) ----------

        void ComputerOrders(int team)
        {
            var mine = Alive.Where(s => s.Team == team).ToList();
            var enemies = Alive.Where(s => s.Team != team).ToList();
            if (mine.Count == 0 || enemies.Count == 0) return;
            var t = TeamOf(team);
            var lead = mine[0];
            var nearest = enemies.OrderBy(e => Vec2.Distance(e.Pos, lead.Pos)).First();
            double nearestDist = Vec2.Distance(nearest.Pos, lead.Pos);

            // AIController states, simplified: attacking (> 4 ships), retreating home (<= 3), and a
            // retreat turns back into an attack when an enemy gets within 1,000 of the lead ship.
            if (Mission != null && team == Mission.EnemyTeam) t.Retreating = false;
            else if (mine.Count > 4) t.Retreating = false;
            else if (mine.Count <= 3 && nearestDist >= 1000) t.Retreating = true;
            if (t.Retreating && nearestDist < 1000) t.Retreating = false;

            bool holding = Time < t.HoldUntil && nearestDist >= 1000;
            var goal = t.Retreating || holding ? t.Home : nearest.Pos;
            // Whole fleet moves together, keeping its formation; fighters chase their own targets when attacking.
            var centre = new Vec2(mine.Average(s => s.Pos.X), mine.Average(s => s.Pos.Y));
            foreach (var s in mine)
            {
                var offset = s.Pos - centre;
                if (offset.Length > 900) offset = offset.Normalized * 900;
                Order(s, !t.Retreating && Hulls.IsSmall(s.Hull) && s.Target != null ? s.Target.Pos : goal + offset);
            }
        }

        // ---------- movement ----------

        void Move(Ship s, double dt)
        {
            if (s.TurnRate < s.Def.turn_rate) s.TurnRate = Math.Min(s.Def.turn_rate, s.TurnRate + 0.003 * UpdateRate * dt);
            bool smallIdle = !s.HasMoveOrder && Hulls.IsSmall(s.Hull);
            if (!s.HasMoveOrder && !smallIdle) return;
            if (smallIdle)
            {
                // Fighters never sit still: they chase what they're shooting at, or circle where they were left.
                if (!s.LoiterSet) { s.Loiter = s.Pos; s.LoiterSet = true; }
                s.MoveTarget = s.Target != null && !s.Target.Dead ? s.Target.Pos : s.Loiter;
            }
            else s.LoiterSet = false;

            double maxSpeed = s.Def.speed;
            double dist = Vec2.Distance(s.Pos, s.MoveTarget);
            if (dist < 400 && s.Waypoints.Count > 0)
            {
                s.MoveTarget = s.Waypoints.Dequeue();
                s.Accelerating = true;
                dist = Vec2.Distance(s.Pos, s.MoveTarget);
            }
            bool small = Hulls.IsSmall(s.Hull);

            if (small)
            {
                s.Speed = maxSpeed;
            }
            else if (dist > ArriveDistance)
            {
                double enemyDist = s.OrderedTarget != null ? Vec2.Distance(s.OrderedTarget.Pos, s.Pos) : s.NearestEnemy;
                bool hold = HoldAtRange && s.Holds && enemyDist < Math.Max(1200, s.HoldRange * 0.9);
                if (dist < BrakeDistance || hold) s.Speed = Math.Max(0, s.Speed - Acceleration / 2 * dt);
                else if (s.Accelerating && s.Speed < maxSpeed) s.Speed += Acceleration * dt;
                if (s.Speed > maxSpeed) s.Speed = maxSpeed;
            }

            if (dist > 150 || small) Turn(s, s.MoveTarget, dt);

            // Big ships only thrust when the target is within 45 degrees of the nose.
            bool facing = small || Vec2.Dot(s.Forward, (s.MoveTarget - s.Pos).Normalized) >= Math.Cos(Math.PI / 4);
            if (facing || dist <= ArriveDistance) s.Pos = s.Pos + s.Forward * (s.Speed * dt);
        }

        void Turn(Ship s, Vec2 target, double dt)
        {
            double want = Vec2.HeadingOf(target - s.Pos);
            double diff = Math.IEEERemainder(want - s.Heading, 2 * Math.PI);
            if (Hulls.IsSmall(s.Hull))
            {
                // Constant turn rate giving a wide circle; hits from other fighters (TurnRate penalty) slow it further.
                double w = Math.Max(0.15, s.Def.speed / SmallTurnRadius) * Math.Max(0.2, s.TurnRate / Math.Max(0.01, s.Def.turn_rate));
                s.Heading += Math.Max(-w * dt, Math.Min(w * dt, diff));
                return;
            }
            double rate = Math.Min(MaxTurnLight, Math.Max(0.05, s.TurnRate) * TurnScale);
            double frac = Math.Min(1.0, rate * dt);
            s.Heading += diff * frac;
        }

        void PushApart(double dt)
        {
            var big = Alive.Where(s => !Hulls.IsSmall(s.Hull)).ToList();
            foreach (var a in big)
                foreach (var b in big)
                {
                    if (a == b) continue;
                    var d = a.Pos - b.Pos;
                    if (d.Length < 300) a.Pos = a.Pos + d * (0.005 * UpdateRate * dt);
                }
        }

        // ---------- targeting ----------

        Ship FindTarget(Ship s)
        {
            var gun = s.MainGun;
            if (gun == null) return null;
            double range = gun.Turret.range;

            if (gun.Weapon.targets_friends)
                return Alive.Where(o => o.Team == s.Team && o != s && o.Shield < o.StartShield && o.Hull != Hull.Light
                                        && Vec2.Distance(o.Pos, s.Pos) <= range)
                            .OrderBy(o => Vec2.Distance(o.Pos, s.Pos)).FirstOrDefault();

            var ordered = s.OrderedTarget;
            if (ordered != null && !ordered.Dead && ordered.Team != s.Team && Vec2.Distance(ordered.Pos, s.Pos) <= range
                && !(gun.Weapon.never_targets_light && ordered.Hull == Hull.Light))
                return ordered;

            var candidates = Alive.Where(o => o.Team != s.Team && Vec2.Distance(o.Pos, s.Pos) <= range
                                              && !(gun.Weapon.never_targets_light && o.Hull == Hull.Light)).ToList();
            if (candidates.Count == 0) return null;
            var preferred = Hulls.Parse(gun.Weapon.prefers ?? "frigate");
            var best = candidates.Where(o => o.Hull == preferred || (preferred == Hull.Capital && o.Hull == Hull.SuperCapital))
                                 .OrderBy(o => Vec2.Distance(o.Pos, s.Pos)).FirstOrDefault();
            return best ?? candidates.OrderBy(o => Vec2.Distance(o.Pos, s.Pos)).First();
        }

        // ---------- firing ----------

        void Fire(Ship s, double dt)
        {
            if (s.Target != null && s.Target.Dead) s.Target = null;
            if (s.Target == null) return;
            foreach (var g in s.Guns)
            {
                if (g.Weapon.module) continue;
                g.Timer += dt;
                if (g.Timer < g.NextDelay) continue;
                g.Timer = 0;

                double dist = Vec2.Distance(s.Pos, s.Target.Pos);
                if (dist <= g.Turret.range) Shoot(s, g, dist);

                g.ShotsInBurst++;
                if (g.ShotsInBurst == g.Turret.shots_per_burst)
                {
                    g.NextDelay = g.Turret.burst_wait;
                    g.ShotsInBurst = 0;
                }
                else g.NextDelay = g.Turret.time_between_shots;
            }
        }

        void Shoot(Ship s, Gun g, double dist)
        {
            var w = g.Weapon;
            var t = s.Target;
            double travel = w.travel_time > 0 ? w.travel_time : dist / w.projectile_speed + w.launch_delay;
            if (w.targets_friends)
            {
                int rid = nextShotId++;
                Shots.Add(new ShotEvent { ShotId = rid, GunIndex = s.Guns.IndexOf(g), From = s, To = t, Visual = w.visual, Hit = true, TravelTime = 0.6 });
                pending.Add(new PendingHit { ShotId = rid, Target = t, Source = s, Damage = w.shield_restore, Repair = true, TimeLeft = 0.6, Visual = w.visual });
                return;
            }
            bool hit;
            if (w.always_hits_non_light && t.Hull != Hull.Light) hit = true;
            else hit = Math.Round(100 * w.hit.For(t.Hull)) > rng.Next(0, 100);

            int damage = w.damage;
            if (w.damage_vs_capital_hull_owner > 0 && s.Hull == Hull.Capital) damage = w.damage_vs_capital_hull_owner;

            int id = nextShotId++;
            Shots.Add(new ShotEvent { ShotId = id, GunIndex = s.Guns.IndexOf(g), From = s, To = t, Visual = w.visual, Hit = hit, AtShield = t.Shield > 0, TravelTime = travel });
            if (hit) pending.Add(new PendingHit { ShotId = id, Target = t, Source = s, Damage = damage, TimeLeft = travel, Visual = w.visual });
        }

        void LandShots(double dt)
        {
            for (int i = pending.Count - 1; i >= 0; i--)
            {
                var p = pending[i];
                p.TimeLeft -= dt;
                if (p.TimeLeft > 0) continue;
                pending.RemoveAt(i);
                if (p.Target.Dead) continue;
                if (p.Repair)
                {
                    if (p.Target.Hull != Hull.Light)
                    {
                        int before = Math.Max(0, p.Target.Shield);
                        p.Target.Shield = Math.Min(p.Target.StartShield, before + p.Damage);
                        p.Source.ShieldsRestored += p.Target.Shield - before;
                    }
                    Impacts.Add(new ImpactEvent { ShotId = p.ShotId, Target = p.Target, Source = p.Source, Repair = true, Visual = p.Visual });
                    continue;
                }
                bool onShield = p.Target.Shield > 0;
                p.Target.LastHitByTeam = p.Source.Team;
                p.Target.LastHitBy = p.Source;
                long hpBefore = Math.Max(0, p.Target.Shield) + Math.Max(0, p.Target.Armor);
                TakeDamage(p.Target, p.Damage, p.Source.Hull);
                p.Source.DamageDealt += hpBefore - (Math.Max(0, p.Target.Shield) + Math.Max(0, p.Target.Armor));
                Impacts.Add(new ImpactEvent { ShotId = p.ShotId, Target = p.Target, Source = p.Source, OnShield = onShield, Visual = p.Visual });
            }
        }

        // _Ship.takeDamage: shield soaks whole hits; squadrons lose craft in steps whatever the damage.
        public void TakeDamage(Ship s, int damage, Hull attackerHull)
        {
            if (s.Shield > 0) s.Shield -= damage;
            else if (s.Hull == Hull.Corvette)
            {
                if (s.Armor > s.StartArmor * 0.6) s.Armor = (int)Math.Round(s.StartArmor * 0.5);
                else if (s.Armor > s.StartArmor * 0.3) s.Armor = (int)Math.Round(s.StartArmor * 0.2);
                else s.Armor = -5;
            }
            else if (s.Hull == Hull.Light)
            {
                if (s.Armor > s.StartArmor * 0.8) s.Armor = (int)Math.Round(s.StartArmor * 0.7);
                else if (s.Armor > s.StartArmor * 0.6) s.Armor = (int)Math.Round(s.StartArmor * 0.5);
                else if (s.Armor > s.StartArmor * 0.4) s.Armor = (int)Math.Round(s.StartArmor * 0.3);
                else if (s.Armor > s.StartArmor * 0.2) s.Armor = (int)Math.Round(s.StartArmor * 0.1);
                else s.Armor = -5;
            }
            else s.Armor -= damage;
            if (s.Shield < 0) s.Shield = -1;
            if (attackerHull == Hull.Light && s.Hull == Hull.Light) s.TurnRate = Math.Max(0.2, s.TurnRate - 0.5);
        }

        // Squadron craft still flying, out of 5, for the renderer.
        public static int CraftLeft(Ship s)
        {
            if (!Hulls.IsSmall(s.Hull)) return 1;
            double f = (double)Math.Max(0, s.Armor) / s.StartArmor;
            if (s.Hull == Hull.Corvette) return f > 0.6 ? 5 : f > 0.3 ? 3 : 1;
            return f > 0.8 ? 5 : f > 0.6 ? 4 : f > 0.4 ? 3 : f > 0.2 ? 2 : 1;
        }

        // ---------- shield field (Support Frigate) ----------
        // Friendly non-fighter ships within the field recharge a share of their max shield per second,
        // even from broken (-1). Fields do not stack: a ship gets the strongest one it is inside.

        void ApplyShieldFields(double dt)
        {
            var fields = Alive.Where(s => s.ShieldField != null).ToList();
            foreach (var s in Alive)
            {
                s.InShieldField = false;
                if (Hulls.IsSmall(s.Hull) || s.Shield >= s.StartShield) { s.ShieldCarry = 0; continue; }
                double best = 0;
                foreach (var f in fields)
                {
                    var w = f.ShieldField.Weapon;
                    if (f.Team == s.Team && Vec2.Distance(f.Pos, s.Pos) <= w.aura_radius) best = Math.Max(best, w.shield_regen_pct);
                }
                if (best <= 0) { s.ShieldCarry = 0; continue; }
                s.InShieldField = true;
                s.ShieldCarry += s.StartShield * best / 100.0 * dt;
                int whole = (int)s.ShieldCarry;
                if (whole <= 0) continue;
                s.ShieldCarry -= whole;
                s.Shield = Math.Min(s.StartShield, Math.Max(0, s.Shield) + whole);
            }
        }
    }
}
