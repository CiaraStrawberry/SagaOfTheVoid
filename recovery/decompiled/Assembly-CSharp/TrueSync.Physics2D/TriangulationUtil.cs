namespace TrueSync.Physics2D;

internal class TriangulationUtil
{
	public static FP EPSILON = 1E-12;

	public static bool SmartIncircle(TriangulationPoint pa, TriangulationPoint pb, TriangulationPoint pc, TriangulationPoint pd)
	{
		FP x = pd.X;
		FP y = pd.Y;
		FP fP = pa.X - x;
		FP fP2 = pa.Y - y;
		FP fP3 = pb.X - x;
		FP fP4 = pb.Y - y;
		FP fP5 = fP * fP4;
		FP fP6 = fP3 * fP2;
		FP fP7 = fP5 - fP6;
		if (fP7 <= 0)
		{
			return false;
		}
		FP fP8 = pc.X - x;
		FP fP9 = pc.Y - y;
		FP fP10 = fP8 * fP2;
		FP fP11 = fP * fP9;
		FP fP12 = fP10 - fP11;
		if (fP12 <= 0)
		{
			return false;
		}
		FP fP13 = fP3 * fP9;
		FP fP14 = fP8 * fP4;
		FP fP15 = fP * fP + fP2 * fP2;
		FP fP16 = fP3 * fP3 + fP4 * fP4;
		FP fP17 = fP8 * fP8 + fP9 * fP9;
		FP fP18 = fP15 * (fP13 - fP14) + fP16 * fP12 + fP17 * fP7;
		return fP18 > 0;
	}

	public static bool InScanArea(TriangulationPoint pa, TriangulationPoint pb, TriangulationPoint pc, TriangulationPoint pd)
	{
		FP fP = (pa.X - pb.X) * (pd.Y - pb.Y) - (pd.X - pb.X) * (pa.Y - pb.Y);
		if (fP >= -EPSILON)
		{
			return false;
		}
		FP fP2 = (pa.X - pc.X) * (pd.Y - pc.Y) - (pd.X - pc.X) * (pa.Y - pc.Y);
		if (fP2 <= EPSILON)
		{
			return false;
		}
		return true;
	}

	public static Orientation Orient2d(TriangulationPoint pa, TriangulationPoint pb, TriangulationPoint pc)
	{
		FP fP = (pa.X - pc.X) * (pb.Y - pc.Y);
		FP fP2 = (pa.Y - pc.Y) * (pb.X - pc.X);
		FP fP3 = fP - fP2;
		if (fP3 > -EPSILON && fP3 < EPSILON)
		{
			return Orientation.Collinear;
		}
		if (fP3 > 0)
		{
			return Orientation.CCW;
		}
		return Orientation.CW;
	}
}
