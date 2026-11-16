using System;

namespace TrueSync.Physics2D;

internal struct Simplex
{
	internal int Count;

	internal FixedArray3<SimplexVertex> V;

	internal void ReadCache(ref SimplexCache cache, DistanceProxy proxyA, ref Transform transformA, DistanceProxy proxyB, ref Transform transformB)
	{
		Count = cache.Count;
		for (int i = 0; i < Count; i++)
		{
			SimplexVertex value = V[i];
			value.IndexA = cache.IndexA[i];
			value.IndexB = cache.IndexB[i];
			TSVector2 v = proxyA.Vertices[value.IndexA];
			TSVector2 v2 = proxyB.Vertices[value.IndexB];
			value.WA = MathUtils.Mul(ref transformA, v);
			value.WB = MathUtils.Mul(ref transformB, v2);
			value.W = value.WB - value.WA;
			value.A = 0f;
			V[i] = value;
		}
		if (Count > 1)
		{
			FP metric = cache.Metric;
			FP metric2 = GetMetric();
			if (metric2 < 0.5f * metric || 2f * metric < metric2 || metric2 < Settings.Epsilon)
			{
				Count = 0;
			}
		}
		if (Count == 0)
		{
			SimplexVertex value2 = V[0];
			value2.IndexA = 0;
			value2.IndexB = 0;
			TSVector2 v3 = proxyA.Vertices[0];
			TSVector2 v4 = proxyB.Vertices[0];
			value2.WA = MathUtils.Mul(ref transformA, v3);
			value2.WB = MathUtils.Mul(ref transformB, v4);
			value2.W = value2.WB - value2.WA;
			value2.A = 1f;
			V[0] = value2;
			Count = 1;
		}
	}

	internal void WriteCache(ref SimplexCache cache)
	{
		cache.Metric = GetMetric();
		cache.Count = (ushort)Count;
		for (int i = 0; i < Count; i++)
		{
			cache.IndexA[i] = (byte)V[i].IndexA;
			cache.IndexB[i] = (byte)V[i].IndexB;
		}
	}

	internal TSVector2 GetSearchDirection()
	{
		switch (Count)
		{
		case 1:
			return -V[0].W;
		case 2:
		{
			TSVector2 a = V[1].W - V[0].W;
			FP fP = MathUtils.Cross(a, -V[0].W);
			if (fP > 0f)
			{
				return new TSVector2(-a.y, a.x);
			}
			return new TSVector2(a.y, -a.x);
		}
		default:
			return TSVector2.zero;
		}
	}

	internal TSVector2 GetClosestPoint()
	{
		return Count switch
		{
			0 => TSVector2.zero, 
			1 => V[0].W, 
			2 => V[0].A * V[0].W + V[1].A * V[1].W, 
			3 => TSVector2.zero, 
			_ => TSVector2.zero, 
		};
	}

	internal void GetWitnessPoints(out TSVector2 pA, out TSVector2 pB)
	{
		switch (Count)
		{
		case 0:
			pA = TSVector2.zero;
			pB = TSVector2.zero;
			break;
		case 1:
			pA = V[0].WA;
			pB = V[0].WB;
			break;
		case 2:
			pA = V[0].A * V[0].WA + V[1].A * V[1].WA;
			pB = V[0].A * V[0].WB + V[1].A * V[1].WB;
			break;
		case 3:
			pA = V[0].A * V[0].WA + V[1].A * V[1].WA + V[2].A * V[2].WA;
			pB = pA;
			break;
		default:
			throw new Exception();
		}
	}

	internal FP GetMetric()
	{
		return Count switch
		{
			0 => 0f, 
			1 => 0f, 
			2 => (V[0].W - V[1].W).magnitude, 
			3 => MathUtils.Cross(V[1].W - V[0].W, V[2].W - V[0].W), 
			_ => 0f, 
		};
	}

	internal void Solve2()
	{
		TSVector2 w = V[0].W;
		TSVector2 w2 = V[1].W;
		TSVector2 value = w2 - w;
		FP fP = -TSVector2.Dot(w, value);
		if (fP <= 0f)
		{
			SimplexVertex value2 = V[0];
			value2.A = 1f;
			V[0] = value2;
			Count = 1;
			return;
		}
		FP fP2 = TSVector2.Dot(w2, value);
		if (fP2 <= 0f)
		{
			SimplexVertex value3 = V[1];
			value3.A = 1f;
			V[1] = value3;
			Count = 1;
			V[0] = V[1];
		}
		else
		{
			FP fP3 = 1f / (fP2 + fP);
			SimplexVertex value4 = V[0];
			SimplexVertex value5 = V[1];
			value4.A = fP2 * fP3;
			value5.A = fP * fP3;
			V[0] = value4;
			V[1] = value5;
			Count = 2;
		}
	}

	internal void Solve3()
	{
		TSVector2 w = V[0].W;
		TSVector2 w2 = V[1].W;
		TSVector2 w3 = V[2].W;
		TSVector2 tSVector = w2 - w;
		FP fP = TSVector2.Dot(w, tSVector);
		FP fP2 = TSVector2.Dot(w2, tSVector);
		FP fP3 = fP2;
		FP fP4 = -fP;
		TSVector2 tSVector2 = w3 - w;
		FP fP5 = TSVector2.Dot(w, tSVector2);
		FP fP6 = TSVector2.Dot(w3, tSVector2);
		FP fP7 = fP6;
		FP fP8 = -fP5;
		TSVector2 value = w3 - w2;
		FP fP9 = TSVector2.Dot(w2, value);
		FP fP10 = TSVector2.Dot(w3, value);
		FP fP11 = fP10;
		FP fP12 = -fP9;
		FP fP13 = MathUtils.Cross(tSVector, tSVector2);
		FP fP14 = fP13 * MathUtils.Cross(w2, w3);
		FP fP15 = fP13 * MathUtils.Cross(w3, w);
		FP fP16 = fP13 * MathUtils.Cross(w, w2);
		if (fP4 <= 0f && fP8 <= 0f)
		{
			SimplexVertex value2 = V[0];
			value2.A = 1f;
			V[0] = value2;
			Count = 1;
		}
		else if (fP3 > 0f && fP4 > 0f && fP16 <= 0f)
		{
			FP fP17 = 1f / (fP3 + fP4);
			SimplexVertex value3 = V[0];
			SimplexVertex value4 = V[1];
			value3.A = fP3 * fP17;
			value4.A = fP4 * fP17;
			V[0] = value3;
			V[1] = value4;
			Count = 2;
		}
		else if (fP7 > 0f && fP8 > 0f && fP15 <= 0f)
		{
			FP fP18 = 1f / (fP7 + fP8);
			SimplexVertex value5 = V[0];
			SimplexVertex value6 = V[2];
			value5.A = fP7 * fP18;
			value6.A = fP8 * fP18;
			V[0] = value5;
			V[2] = value6;
			Count = 2;
			V[1] = V[2];
		}
		else if (fP3 <= 0f && fP12 <= 0f)
		{
			SimplexVertex value7 = V[1];
			value7.A = 1f;
			V[1] = value7;
			Count = 1;
			V[0] = V[1];
		}
		else if (fP7 <= 0f && fP11 <= 0f)
		{
			SimplexVertex value8 = V[2];
			value8.A = 1f;
			V[2] = value8;
			Count = 1;
			V[0] = V[2];
		}
		else if (fP11 > 0f && fP12 > 0f && fP14 <= 0f)
		{
			FP fP19 = 1f / (fP11 + fP12);
			SimplexVertex value9 = V[1];
			SimplexVertex value10 = V[2];
			value9.A = fP11 * fP19;
			value10.A = fP12 * fP19;
			V[1] = value9;
			V[2] = value10;
			Count = 2;
			V[0] = V[2];
		}
		else
		{
			FP fP20 = 1f / (fP14 + fP15 + fP16);
			SimplexVertex value11 = V[0];
			SimplexVertex value12 = V[1];
			SimplexVertex value13 = V[2];
			value11.A = fP14 * fP20;
			value12.A = fP15 * fP20;
			value13.A = fP16 * fP20;
			V[0] = value11;
			V[1] = value12;
			V[2] = value13;
			Count = 3;
		}
	}
}
