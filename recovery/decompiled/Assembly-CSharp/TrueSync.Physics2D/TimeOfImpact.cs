using System;

namespace TrueSync.Physics2D;

public static class TimeOfImpact
{
	[ThreadStatic]
	public static int TOICalls;

	[ThreadStatic]
	public static int TOIIters;

	[ThreadStatic]
	public static int TOIMaxIters;

	[ThreadStatic]
	public static int TOIRootIters;

	[ThreadStatic]
	public static int TOIMaxRootIters;

	[ThreadStatic]
	private static DistanceInput _distanceInput;

	public static void CalculateTimeOfImpact(out TOIOutput output, TOIInput input)
	{
		output = default;
		output.State = TOIOutputState.Unknown;
		output.T = input.TMax;
		Sweep sweepA = input.SweepA;
		Sweep sweepB = input.SweepB;
		sweepA.Normalize();
		sweepB.Normalize();
		FP tMax = input.TMax;
		FP fP = input.ProxyA.Radius + input.ProxyB.Radius;
		FP fP2 = TSMath.Max(Settings.LinearSlop, fP - 3f * Settings.LinearSlop);
		FP fP3 = 0.25f * Settings.LinearSlop;
		FP fP4 = 0f;
		int num = 0;
		_distanceInput = _distanceInput ?? new DistanceInput();
		_distanceInput.ProxyA = input.ProxyA;
		_distanceInput.ProxyB = input.ProxyB;
		_distanceInput.UseRadii = false;
		do
		{
			sweepA.GetTransform(out var xfb, fP4);
			sweepB.GetTransform(out var xfb2, fP4);
			_distanceInput.TransformA = xfb;
			_distanceInput.TransformB = xfb2;
			Distance.ComputeDistance(out var output2, out var cache, _distanceInput);
			if (output2.Distance <= 0f)
			{
				output.State = TOIOutputState.Overlapped;
				output.T = 0f;
				return;
			}
			if (output2.Distance < fP2 + fP3)
			{
				output.State = TOIOutputState.Touching;
				output.T = fP4;
				return;
			}
			SeparationFunction.Set(ref cache, input.ProxyA, ref sweepA, input.ProxyB, ref sweepB, fP4);
			bool flag = false;
			FP fP5 = tMax;
			int num2 = 0;
			do
			{
				FP fP6 = SeparationFunction.FindMinSeparation(out var indexA, out var indexB, fP5);
				if (fP6 > fP2 + fP3)
				{
					output.State = TOIOutputState.Seperated;
					output.T = tMax;
					flag = true;
					break;
				}
				if (fP6 > fP2 - fP3)
				{
					fP4 = fP5;
					break;
				}
				FP fP7 = SeparationFunction.Evaluate(indexA, indexB, fP4);
				if (fP7 < fP2 - fP3)
				{
					output.State = TOIOutputState.Failed;
					output.T = fP4;
					flag = true;
					break;
				}
				if (fP7 <= fP2 + fP3)
				{
					output.State = TOIOutputState.Touching;
					output.T = fP4;
					flag = true;
					break;
				}
				int num3 = 0;
				FP fP8 = fP4;
				FP fP9 = fP5;
				do
				{
					FP fP10 = (((num3 & 1) == 0) ? (0.5f * (fP8 + fP9)) : (fP8 + (fP2 - fP7) * (fP9 - fP8) / (fP6 - fP7)));
					num3++;
					FP fP11 = SeparationFunction.Evaluate(indexA, indexB, fP10);
					if (FP.Abs(fP11 - fP2) < fP3)
					{
						fP5 = fP10;
						break;
					}
					if (fP11 > fP2)
					{
						fP8 = fP10;
						fP7 = fP11;
					}
					else
					{
						fP9 = fP10;
						fP6 = fP11;
					}
				}
				while (num3 != 50);
				num2++;
			}
			while (num2 != Settings.MaxPolygonVertices);
			num++;
			if (flag)
			{
				return;
			}
		}
		while (num != 20);
		output.State = TOIOutputState.Failed;
		output.T = fP4;
	}
}
