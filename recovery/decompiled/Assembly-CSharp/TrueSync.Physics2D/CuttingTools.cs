using System.Collections.Generic;

namespace TrueSync.Physics2D;

public static class CuttingTools
{
	public static void SplitShape(Fixture fixture, TSVector2 entryPoint, TSVector2 exitPoint, out Vertices first, out Vertices second)
	{
		TSVector2 localPoint = fixture.Body.GetLocalPoint(ref entryPoint);
		TSVector2 localPoint2 = fixture.Body.GetLocalPoint(ref exitPoint);
		if (!(fixture.Shape is PolygonShape polygonShape))
		{
			first = new Vertices();
			second = new Vertices();
			return;
		}
		foreach (TSVector2 vertex in polygonShape.Vertices)
		{
			if (vertex.Equals(localPoint))
			{
				localPoint -= new TSVector2(0, Settings.Epsilon);
			}
			if (vertex.Equals(localPoint2))
			{
				localPoint2 += new TSVector2(0, Settings.Epsilon);
			}
		}
		Vertices vertices = new Vertices(polygonShape.Vertices);
		Vertices[] array = new Vertices[2];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Vertices(vertices.Count);
		}
		int[] array2 = new int[2] { -1, -1 };
		int num = -1;
		for (int j = 0; j < vertices.Count; j++)
		{
			int num2 = ((!(TSVector2.Dot(MathUtils.Cross(localPoint2 - localPoint, 1), vertices[j] - localPoint) > Settings.Epsilon)) ? 1 : 0);
			if (num != num2)
			{
				if (num == 0)
				{
					array2[0] = array[num].Count;
					array[num].Add(localPoint2);
					array[num].Add(localPoint);
				}
				if (num == 1)
				{
					array2[num] = array[num].Count;
					array[num].Add(localPoint);
					array[num].Add(localPoint2);
				}
			}
			array[num2].Add(vertices[j]);
			num = num2;
		}
		if (array2[0] == -1)
		{
			array2[0] = array[0].Count;
			array[0].Add(localPoint2);
			array[0].Add(localPoint);
		}
		if (array2[1] == -1)
		{
			array2[1] = array[1].Count;
			array[1].Add(localPoint);
			array[1].Add(localPoint2);
		}
		for (int k = 0; k < 2; k++)
		{
			TSVector2 tSVector = ((array2[k] <= 0) ? (array[k][array[k].Count - 1] - array[k][0]) : (array[k][array2[k] - 1] - array[k][array2[k]]));
			tSVector.Normalize();
			if (!tSVector.IsValid())
			{
				tSVector = TSVector2.one;
			}
			array[k][array2[k]] += Settings.Epsilon * tSVector;
			tSVector = ((array2[k] >= array[k].Count - 2) ? (array[k][0] - array[k][array[k].Count - 1]) : (array[k][array2[k] + 2] - array[k][array2[k] + 1]));
			tSVector.Normalize();
			if (!tSVector.IsValid())
			{
				tSVector = TSVector2.one;
			}
			array[k][array2[k] + 1] += Settings.Epsilon * tSVector;
		}
		first = array[0];
		second = array[1];
	}

	public static bool Cut(World world, TSVector2 start, TSVector2 end)
	{
		List<Fixture> fixtures = new List<Fixture>();
		List<TSVector2> entryPoints = new List<TSVector2>();
		List<TSVector2> exitPoints = new List<TSVector2>();
		if (world.TestPoint(start) != null || world.TestPoint(end) != null)
		{
			return false;
		}
		world.RayCast((Fixture f, TSVector2 p, TSVector2 n, FP fr) =>
		{
			fixtures.Add(f);
			entryPoints.Add(p);
			return 1;
		}, start, end);
		world.RayCast((Fixture f, TSVector2 p, TSVector2 n, FP fr) =>
		{
			exitPoints.Add(p);
			return 1;
		}, end, start);
		if (entryPoints.Count + exitPoints.Count < 2)
		{
			return false;
		}
		for (int num = 0; num < fixtures.Count; num++)
		{
			if (fixtures[num].Shape.ShapeType == ShapeType.Polygon && fixtures[num].Body.BodyType != BodyType.Static)
			{
				SplitShape(fixtures[num], entryPoints[num], exitPoints[num], out var first, out var second);
				if (first.CheckPolygon() == PolygonError.NoError)
				{
					Body body = BodyFactory.CreatePolygon(world, first, fixtures[num].Shape.Density, fixtures[num].Body.Position);
					body.Rotation = fixtures[num].Body.Rotation;
					body.LinearVelocity = fixtures[num].Body.LinearVelocity;
					body.AngularVelocity = fixtures[num].Body.AngularVelocity;
					body.BodyType = BodyType.Dynamic;
				}
				if (second.CheckPolygon() == PolygonError.NoError)
				{
					Body body2 = BodyFactory.CreatePolygon(world, second, fixtures[num].Shape.Density, fixtures[num].Body.Position);
					body2.Rotation = fixtures[num].Body.Rotation;
					body2.LinearVelocity = fixtures[num].Body.LinearVelocity;
					body2.AngularVelocity = fixtures[num].Body.AngularVelocity;
					body2.BodyType = BodyType.Dynamic;
				}
				world.RemoveBody(fixtures[num].Body);
			}
		}
		return true;
	}
}
