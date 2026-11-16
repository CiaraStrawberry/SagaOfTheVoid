namespace TrueSync.Physics3D;

public sealed class GJKCollide
{
	private const int MaxIterations = 15;

	private static ResourcePool<VoronoiSimplexSolver> simplexSolverPool = new ResourcePool<VoronoiSimplexSolver>();

	private static void SupportMapTransformed(ISupportMappable support, ref TSMatrix orientation, ref TSVector position, ref TSVector direction, out TSVector result)
	{
		result.x = direction.x * orientation.M11 + direction.y * orientation.M12 + direction.z * orientation.M13;
		result.y = direction.x * orientation.M21 + direction.y * orientation.M22 + direction.z * orientation.M23;
		result.z = direction.x * orientation.M31 + direction.y * orientation.M32 + direction.z * orientation.M33;
		support.SupportMapping(ref result, out result);
		FP fP = result.x * orientation.M11 + result.y * orientation.M21 + result.z * orientation.M31;
		FP fP2 = result.x * orientation.M12 + result.y * orientation.M22 + result.z * orientation.M32;
		FP fP3 = result.x * orientation.M13 + result.y * orientation.M23 + result.z * orientation.M33;
		result.x = position.x + fP;
		result.y = position.y + fP2;
		result.z = position.z + fP3;
	}

	public static bool Pointcast(ISupportMappable support, ref TSMatrix orientation, ref TSVector position, ref TSVector point)
	{
		SupportMapTransformed(support, ref orientation, ref position, ref point, out var result);
		TSVector.Subtract(ref point, ref result, out result);
		support.SupportCenter(out var center);
		TSVector.Transform(ref center, ref orientation, out center);
		TSVector.Add(ref position, ref center, out center);
		TSVector.Subtract(ref point, ref center, out center);
		TSVector value = point;
		TSVector.Subtract(ref value, ref result, out var result2);
		FP fP = result2.sqrMagnitude;
		FP eN = FP.EN4;
		int num = 15;
		VoronoiSimplexSolver voronoiSimplexSolver = simplexSolverPool.GetNew();
		voronoiSimplexSolver.Reset();
		while (fP > eN && num-- != 0)
		{
			SupportMapTransformed(support, ref orientation, ref position, ref result2, out var result3);
			TSVector.Subtract(ref value, ref result3, out var result4);
			FP fP2 = TSVector.Dot(ref result2, ref result4);
			if (fP2 > FP.Zero)
			{
				FP fP3 = TSVector.Dot(ref result2, ref center);
				if (fP3 >= -(TSMath.Epsilon * TSMath.Epsilon))
				{
					simplexSolverPool.GiveBack(voronoiSimplexSolver);
					return false;
				}
				voronoiSimplexSolver.Reset();
			}
			if (!voronoiSimplexSolver.InSimplex(result4))
			{
				voronoiSimplexSolver.AddVertex(result4, value, result3);
			}
			fP = ((!voronoiSimplexSolver.Closest(out result2)) ? FP.Zero : result2.sqrMagnitude);
		}
		simplexSolverPool.GiveBack(voronoiSimplexSolver);
		return true;
	}

	public static bool ClosestPoints(ISupportMappable support1, ISupportMappable support2, ref TSMatrix orientation1, ref TSMatrix orientation2, ref TSVector position1, ref TSVector position2, out TSVector p1, out TSVector p2, out TSVector normal)
	{
		VoronoiSimplexSolver voronoiSimplexSolver = simplexSolverPool.GetNew();
		voronoiSimplexSolver.Reset();
		p1 = (p2 = TSVector.zero);
		TSVector direction = position1 - position2;
		TSVector direction2 = TSVector.Negate(direction);
		SupportMapTransformed(support1, ref orientation1, ref position1, ref direction2, out var result);
		SupportMapTransformed(support2, ref orientation2, ref position2, ref direction, out var result2);
		TSVector direction3 = result - result2;
		normal = TSVector.zero;
		int num = 15;
		FP fP = direction3.sqrMagnitude;
		FP eN = FP.EN5;
		while (fP > eN && num-- != 0)
		{
			TSVector direction4 = TSVector.Negate(direction3);
			SupportMapTransformed(support1, ref orientation1, ref position1, ref direction4, out result);
			SupportMapTransformed(support2, ref orientation2, ref position2, ref direction3, out result2);
			TSVector w = result - result2;
			if (!voronoiSimplexSolver.InSimplex(w))
			{
				voronoiSimplexSolver.AddVertex(w, result, result2);
			}
			if (voronoiSimplexSolver.Closest(out direction3))
			{
				fP = direction3.sqrMagnitude;
				normal = direction3;
			}
			else
			{
				fP = FP.Zero;
			}
		}
		voronoiSimplexSolver.ComputePoints(out p1, out p2);
		if (normal.sqrMagnitude > TSMath.Epsilon * TSMath.Epsilon)
		{
			normal.Normalize();
		}
		simplexSolverPool.GiveBack(voronoiSimplexSolver);
		return true;
	}

	public static bool Raycast(ISupportMappable support, ref TSMatrix orientation, ref TSMatrix invOrientation, ref TSVector position, ref TSVector origin, ref TSVector direction, out FP fraction, out TSVector normal)
	{
		VoronoiSimplexSolver voronoiSimplexSolver = simplexSolverPool.GetNew();
		voronoiSimplexSolver.Reset();
		normal = TSVector.zero;
		fraction = FP.MaxValue;
		FP zero = FP.Zero;
		TSVector direction2 = direction;
		TSVector value = origin;
		SupportMapTransformed(support, ref orientation, ref position, ref direction2, out var result);
		TSVector.Subtract(ref value, ref result, out var result2);
		int num = 15;
		FP fP = result2.sqrMagnitude;
		FP eN = FP.EN6;
		while (fP > eN && num-- != 0)
		{
			SupportMapTransformed(support, ref orientation, ref position, ref result2, out var result3);
			TSVector.Subtract(ref value, ref result3, out var result4);
			FP fP2 = TSVector.Dot(ref result2, ref result4);
			if (fP2 > FP.Zero)
			{
				FP fP3 = TSVector.Dot(ref result2, ref direction2);
				if (fP3 >= -TSMath.Epsilon)
				{
					simplexSolverPool.GiveBack(voronoiSimplexSolver);
					return false;
				}
				zero -= fP2 / fP3;
				TSVector.Multiply(ref direction2, zero, out value);
				TSVector.Add(ref origin, ref value, out value);
				TSVector.Subtract(ref value, ref result3, out result4);
				normal = result2;
			}
			if (!voronoiSimplexSolver.InSimplex(result4))
			{
				voronoiSimplexSolver.AddVertex(result4, value, result3);
			}
			fP = ((!voronoiSimplexSolver.Closest(out result2)) ? FP.Zero : result2.sqrMagnitude);
		}
		voronoiSimplexSolver.ComputePoints(out var _, out var p2);
		fraction = (p2 - origin).magnitude / direction.magnitude;
		if (normal.sqrMagnitude > TSMath.Epsilon * TSMath.Epsilon)
		{
			normal.Normalize();
		}
		simplexSolverPool.GiveBack(voronoiSimplexSolver);
		return true;
	}
}
