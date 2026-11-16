namespace TrueSync.Physics2D;

internal class PolygonGenerator
{
	private static readonly TSRandom RNG = TSRandom.New(0);

	private static FP PI_2 = 2.0 * FP.Pi;

	public static Polygon RandomCircleSweep(FP scale, int vertexCount)
	{
		FP fP = scale / 4;
		PolygonPoint[] array = new PolygonPoint[vertexCount];
		for (int i = 0; i < vertexCount; i++)
		{
			do
			{
				if (i % 250 == 0)
				{
					fP += scale / 2 * (0.5 - RNG.NextFP());
				}
				else if (i % 50 == 0)
				{
					fP += scale / 5 * (0.5 - RNG.NextFP());
				}
				else
				{
					fP += 25 * scale / vertexCount * (0.5 - RNG.NextFP());
				}
				fP = ((!(fP > scale / 2)) ? fP : (scale / 2));
				fP = ((!(fP < scale / 10)) ? fP : (scale / 10));
			}
			while (fP < scale / 10 || fP > scale / 2);
			PolygonPoint polygonPoint = new PolygonPoint(fP * FP.Cos(PI_2 * i / vertexCount), fP * FP.Sin(PI_2 * i / vertexCount));
			array[i] = polygonPoint;
		}
		return new Polygon(array);
	}

	public static Polygon RandomCircleSweep2(FP scale, int vertexCount)
	{
		FP fP = scale / 4;
		PolygonPoint[] array = new PolygonPoint[vertexCount];
		for (int i = 0; i < vertexCount; i++)
		{
			do
			{
				fP += scale / 5 * (0.5 - RNG.NextFP());
				fP = ((!(fP > scale / 2)) ? fP : (scale / 2));
				fP = ((!(fP < scale / 10)) ? fP : (scale / 10));
			}
			while (fP < scale / 10 || fP > scale / 2);
			PolygonPoint polygonPoint = new PolygonPoint(fP * FP.Cos(PI_2 * i / vertexCount), fP * FP.Sin(PI_2 * i / vertexCount));
			array[i] = polygonPoint;
		}
		return new Polygon(array);
	}
}
