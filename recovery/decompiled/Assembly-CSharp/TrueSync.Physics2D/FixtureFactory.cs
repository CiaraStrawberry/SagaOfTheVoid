using System;
using System.Collections.Generic;

namespace TrueSync.Physics2D;

public static class FixtureFactory
{
	public static Fixture AttachEdge(TSVector2 start, TSVector2 end, Body body, object userData = null)
	{
		EdgeShape shape = new EdgeShape(start, end);
		return body.CreateFixture(shape, userData);
	}

	public static Fixture AttachChainShape(Vertices vertices, Body body, object userData = null)
	{
		ChainShape shape = new ChainShape(vertices);
		return body.CreateFixture(shape, userData);
	}

	public static Fixture AttachLoopShape(Vertices vertices, Body body, object userData = null)
	{
		ChainShape shape = new ChainShape(vertices, createLoop: true);
		return body.CreateFixture(shape, userData);
	}

	public static Fixture AttachRectangle(FP width, FP height, FP density, TSVector2 offset, Body body, object userData = null)
	{
		Vertices vertices = PolygonTools.CreateRectangle(width / 2, height / 2);
		vertices.Translate(ref offset);
		PolygonShape shape = new PolygonShape(vertices, density);
		return body.CreateFixture(shape, userData);
	}

	public static Fixture AttachCircle(FP radius, FP density, Body body, object userData = null)
	{
		if (radius <= 0)
		{
			throw new ArgumentOutOfRangeException("radius", "Radius must be more than 0 meters");
		}
		CircleShape shape = new CircleShape(radius, density);
		return body.CreateFixture(shape, userData);
	}

	public static Fixture AttachCircle(FP radius, FP density, Body body, TSVector2 offset, object userData = null)
	{
		if (radius <= 0)
		{
			throw new ArgumentOutOfRangeException("radius", "Radius must be more than 0 meters");
		}
		CircleShape circleShape = new CircleShape(radius, density);
		circleShape.Position = offset;
		return body.CreateFixture(circleShape, userData);
	}

	public static Fixture AttachPolygon(Vertices vertices, FP density, Body body, object userData = null)
	{
		if (vertices.Count <= 1)
		{
			throw new ArgumentOutOfRangeException("vertices", "Too few points to be a polygon");
		}
		PolygonShape shape = new PolygonShape(vertices, density);
		return body.CreateFixture(shape, userData);
	}

	public static Fixture AttachEllipse(FP xRadius, FP yRadius, int edges, FP density, Body body, object userData = null)
	{
		if (xRadius <= 0)
		{
			throw new ArgumentOutOfRangeException("xRadius", "X-radius must be more than 0");
		}
		if (yRadius <= 0)
		{
			throw new ArgumentOutOfRangeException("yRadius", "Y-radius must be more than 0");
		}
		Vertices vertices = PolygonTools.CreateEllipse(xRadius, yRadius, edges);
		PolygonShape shape = new PolygonShape(vertices, density);
		return body.CreateFixture(shape, userData);
	}

	public static List<Fixture> AttachCompoundPolygon(List<Vertices> list, FP density, Body body, object userData = null)
	{
		List<Fixture> list2 = new List<Fixture>(list.Count);
		foreach (Vertices item in list)
		{
			if (item.Count == 2)
			{
				EdgeShape shape = new EdgeShape(item[0], item[1]);
				list2.Add(body.CreateFixture(shape, userData));
			}
			else
			{
				PolygonShape shape2 = new PolygonShape(item, density);
				list2.Add(body.CreateFixture(shape2, userData));
			}
		}
		return list2;
	}

	public static Fixture AttachLineArc(FP radians, int sides, FP radius, TSVector2 position, FP angle, bool closed, Body body)
	{
		Vertices vertices = PolygonTools.CreateArc(radians, sides, radius);
		vertices.Rotate((FP.Pi - radians) / 2 + angle);
		vertices.Translate(ref position);
		return (!closed) ? AttachChainShape(vertices, body) : AttachLoopShape(vertices, body);
	}

	public static List<Fixture> AttachSolidArc(FP density, FP radians, int sides, FP radius, TSVector2 position, FP angle, Body body)
	{
		Vertices vertices = PolygonTools.CreateArc(radians, sides, radius);
		vertices.Rotate((FP.Pi - radians) / 2 + angle);
		vertices.Translate(ref position);
		vertices.Add(vertices[0]);
		List<Vertices> list = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Earclip, discardAndFixInvalid: true, FP.EN3);
		return AttachCompoundPolygon(list, density, body);
	}
}
