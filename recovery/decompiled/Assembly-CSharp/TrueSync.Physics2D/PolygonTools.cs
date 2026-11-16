using System;

namespace TrueSync.Physics2D;

public static class PolygonTools
{
	public static void TransformVertices(Vertices vertices, TSVector2 center, FP angle)
	{
		Transform T = default;
		T.p = center;
		T.q.Set(angle);
		for (int i = 0; i < vertices.Count; i++)
		{
			vertices[i] = MathUtils.Mul(ref T, vertices[i]) - center;
		}
	}

	public static Vertices CreateRectangle(FP hx, FP hy)
	{
		Vertices vertices = new Vertices(4);
		vertices.Add(new TSVector2(-hx, -hy));
		vertices.Add(new TSVector2(hx, -hy));
		vertices.Add(new TSVector2(hx, hy));
		vertices.Add(new TSVector2(-hx, hy));
		return vertices;
	}

	public static Vertices CreateRectangle(FP hx, FP hy, TSVector2 center, FP angle)
	{
		Vertices vertices = CreateRectangle(hx, hy);
		Transform T = default;
		T.p = center;
		T.q.Set(angle);
		for (int i = 0; i < 4; i++)
		{
			vertices[i] = MathUtils.Mul(ref T, vertices[i]) - center;
		}
		return vertices;
	}

	public static Vertices CreateRoundedRectangle(FP width, FP height, FP xRadius, FP yRadius, int segments)
	{
		if (yRadius > height / 2 || xRadius > width / 2)
		{
			throw new Exception("Rounding amount can't be more than half the height and width respectively.");
		}
		if (segments < 0)
		{
			throw new Exception("Segments must be zero or more.");
		}
		Vertices vertices = new Vertices();
		if (segments == 0)
		{
			vertices.Add(new TSVector2(width * 0.5f - xRadius, -height * 0.5f));
			vertices.Add(new TSVector2(width * 0.5f, -height * 0.5f + yRadius));
			vertices.Add(new TSVector2(width * 0.5f, height * 0.5f - yRadius));
			vertices.Add(new TSVector2(width * 0.5f - xRadius, height * 0.5f));
			vertices.Add(new TSVector2(-width * 0.5f + xRadius, height * 0.5f));
			vertices.Add(new TSVector2(-width * 0.5f, height * 0.5f - yRadius));
			vertices.Add(new TSVector2(-width * 0.5f, -height * 0.5f + yRadius));
			vertices.Add(new TSVector2(-width * 0.5f + xRadius, -height * 0.5f));
		}
		else
		{
			int num = segments * 4 + 8;
			FP fP = FP.PiTimes2 / (num - 4);
			int num2 = num / 4;
			TSVector2 tSVector = new TSVector2(width / 2 - xRadius, height / 2 - yRadius);
			vertices.Add(tSVector + new TSVector2(xRadius, -yRadius + yRadius));
			short num3 = 0;
			for (int i = 1; i < num; i++)
			{
				if (i - num2 == 0 || i - num2 * 3 == 0)
				{
					tSVector.x *= (FP)(-1);
					num3--;
				}
				else if (i - num2 * 2 == 0)
				{
					tSVector.y *= (FP)(-1);
					num3--;
				}
				vertices.Add(tSVector + new TSVector2(xRadius * FP.Cos(fP * -(i + num3)), -yRadius * FP.Sin(fP * -(i + num3))));
			}
		}
		return vertices;
	}

	public static Vertices CreateLine(TSVector2 start, TSVector2 end)
	{
		Vertices vertices = new Vertices(2);
		vertices.Add(start);
		vertices.Add(end);
		return vertices;
	}

	public static Vertices CreateCircle(FP radius, int numberOfEdges)
	{
		return CreateEllipse(radius, radius, numberOfEdges);
	}

	public static Vertices CreateEllipse(FP xRadius, FP yRadius, int numberOfEdges)
	{
		Vertices vertices = new Vertices();
		FP fP = FP.PiTimes2 / numberOfEdges;
		vertices.Add(new TSVector2(xRadius, 0));
		for (int num = numberOfEdges - 1; num > 0; num--)
		{
			vertices.Add(new TSVector2(xRadius * FP.Cos(fP * num), -yRadius * FP.Sin(fP * num)));
		}
		return vertices;
	}

	public static Vertices CreateArc(FP radians, int sides, FP radius)
	{
		Vertices vertices = new Vertices();
		FP fP = radians / sides;
		for (int num = sides - 1; num > 0; num--)
		{
			vertices.Add(new TSVector2(radius * FP.Cos(fP * num), radius * FP.Sin(fP * num)));
		}
		return vertices;
	}

	public static Vertices CreateCapsule(FP height, FP endRadius, int edges)
	{
		if (endRadius >= height / 2)
		{
			throw new ArgumentException("The radius must be lower than height / 2. Higher values of radius would create a circle, and not a half circle.", "endRadius");
		}
		return CreateCapsule(height, endRadius, edges, endRadius, edges);
	}

	public static Vertices CreateCapsule(FP height, FP topRadius, int topEdges, FP bottomRadius, int bottomEdges)
	{
		if (height <= 0)
		{
			throw new ArgumentException("Height must be longer than 0", "height");
		}
		if (topRadius <= 0)
		{
			throw new ArgumentException("The top radius must be more than 0", "topRadius");
		}
		if (topEdges <= 0)
		{
			throw new ArgumentException("Top edges must be more than 0", "topEdges");
		}
		if (bottomRadius <= 0)
		{
			throw new ArgumentException("The bottom radius must be more than 0", "bottomRadius");
		}
		if (bottomEdges <= 0)
		{
			throw new ArgumentException("Bottom edges must be more than 0", "bottomEdges");
		}
		if (topRadius >= height / 2)
		{
			throw new ArgumentException("The top radius must be lower than height / 2. Higher values of top radius would create a circle, and not a half circle.", "topRadius");
		}
		if (bottomRadius >= height / 2)
		{
			throw new ArgumentException("The bottom radius must be lower than height / 2. Higher values of bottom radius would create a circle, and not a half circle.", "bottomRadius");
		}
		Vertices vertices = new Vertices();
		FP fP = (height - topRadius - bottomRadius) * 0.5f;
		vertices.Add(new TSVector2(topRadius, fP));
		FP fP2 = FP.Pi / topEdges;
		for (int i = 1; i < topEdges; i++)
		{
			vertices.Add(new TSVector2(topRadius * FP.Cos(fP2 * i), topRadius * FP.Sin(fP2 * i) + fP));
		}
		vertices.Add(new TSVector2(-topRadius, fP));
		vertices.Add(new TSVector2(-bottomRadius, -fP));
		fP2 = FP.Pi / bottomEdges;
		for (int j = 1; j < bottomEdges; j++)
		{
			vertices.Add(new TSVector2(-bottomRadius * FP.Cos(fP2 * j), -bottomRadius * FP.Sin(fP2 * j) - fP));
		}
		vertices.Add(new TSVector2(bottomRadius, -fP));
		return vertices;
	}

	public static Vertices CreateGear(FP radius, int numberOfTeeth, FP tipPercentage, FP toothHeight)
	{
		Vertices vertices = new Vertices();
		FP fP = FP.PiTimes2 / numberOfTeeth;
		tipPercentage /= (FP)100f;
		TSMath.Clamp(tipPercentage, 0f, 1f);
		FP fP2 = fP / 2f * tipPercentage;
		FP fP3 = (fP - fP2 * 2f) / 2f;
		for (int num = numberOfTeeth - 1; num >= 0; num--)
		{
			if (fP2 > 0f)
			{
				vertices.Add(new TSVector2(radius * FP.Cos(fP * num + fP3 * 2f + fP2), -radius * FP.Sin(fP * num + fP3 * 2f + fP2)));
				vertices.Add(new TSVector2((radius + toothHeight) * FP.Cos(fP * num + fP3 + fP2), -(radius + toothHeight) * FP.Sin(fP * num + fP3 + fP2)));
			}
			vertices.Add(new TSVector2((radius + toothHeight) * FP.Cos(fP * num + fP3), -(radius + toothHeight) * FP.Sin(fP * num + fP3)));
			vertices.Add(new TSVector2(radius * FP.Cos(fP * num), -radius * FP.Sin(fP * num)));
		}
		return vertices;
	}
}
