using System;

namespace Saga.Sim
{
    // Flat-plane vector. The old game was 3D, but no rule used height (see PLAN.md).
    public struct Vec2
    {
        public double X, Y;

        public Vec2(double x, double y) { X = x; Y = y; }

        public static readonly Vec2 Zero = new Vec2(0, 0);

        public double Length => Math.Sqrt(X * X + Y * Y);
        public double LengthSq => X * X + Y * Y;
        public Vec2 Normalized { get { var l = Length; return l > 1e-9 ? new Vec2(X / l, Y / l) : Zero; } }

        public static Vec2 operator +(Vec2 a, Vec2 b) => new Vec2(a.X + b.X, a.Y + b.Y);
        public static Vec2 operator -(Vec2 a, Vec2 b) => new Vec2(a.X - b.X, a.Y - b.Y);
        public static Vec2 operator *(Vec2 a, double k) => new Vec2(a.X * k, a.Y * k);
        public static Vec2 operator /(Vec2 a, double k) => new Vec2(a.X / k, a.Y / k);

        public static double Distance(Vec2 a, Vec2 b) => (a - b).Length;
        public static double Dot(Vec2 a, Vec2 b) => a.X * b.X + a.Y * b.Y;

        // Heading is in radians, 0 = pointing along +Y ("up" on screen), growing anticlockwise.
        public static Vec2 FromHeading(double h) => new Vec2(-Math.Sin(h), Math.Cos(h));
        public static double HeadingOf(Vec2 d) => Math.Atan2(-d.X, d.Y);

        public override string ToString() => $"({X:F0}, {Y:F0})";
    }
}
