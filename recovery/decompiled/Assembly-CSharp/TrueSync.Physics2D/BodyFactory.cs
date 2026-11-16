using System;
using System.Collections.Generic;

namespace TrueSync.Physics2D;

public static class BodyFactory
{
	public static Body CreateBody(World world, object userData = null)
	{
		return new Body(world, null, 0, userData);
	}

	public static Body CreateBody(World world, TSVector2 position, FP rotation, object userData = null)
	{
		return new Body(world, position, rotation, userData);
	}

	public static Body CreateEdge(World world, TSVector2 start, TSVector2 end, object userData = null)
	{
		Body body = CreateBody(world);
		FixtureFactory.AttachEdge(start, end, body, userData);
		return body;
	}

	public static Body CreateChainShape(World world, Vertices vertices, object userData = null)
	{
		return CreateChainShape(world, vertices, TSVector2.zero, userData);
	}

	public static Body CreateChainShape(World world, Vertices vertices, TSVector2 position, object userData = null)
	{
		Body body = CreateBody(world, position);
		FixtureFactory.AttachChainShape(vertices, body, userData);
		return body;
	}

	public static Body CreateLoopShape(World world, Vertices vertices, object userData = null)
	{
		return CreateLoopShape(world, vertices, TSVector2.zero, userData);
	}

	public static Body CreateLoopShape(World world, Vertices vertices, TSVector2 position, object userData = null)
	{
		Body body = CreateBody(world, position);
		FixtureFactory.AttachLoopShape(vertices, body, userData);
		return body;
	}

	public static Body CreateRectangle(World world, FP width, FP height, FP density, object userData = null)
	{
		return CreateRectangle(world, width, height, density, TSVector2.zero, userData);
	}

	public static Body CreateRectangle(World world, FP width, FP height, FP density, TSVector2 position, object userData = null)
	{
		if (width <= 0)
		{
			throw new ArgumentOutOfRangeException("width", "Width must be more than 0 meters");
		}
		if (height <= 0)
		{
			throw new ArgumentOutOfRangeException("height", "Height must be more than 0 meters");
		}
		Body body = CreateBody(world, position);
		body.UserData = userData;
		Vertices vertices = PolygonTools.CreateRectangle(width / 2, height / 2);
		PolygonShape shape = new PolygonShape(vertices, density);
		body.CreateFixture(shape);
		return body;
	}

	public static Body CreateCircle(World world, FP radius, FP density, object userData = null)
	{
		return CreateCircle(world, radius, density, TSVector2.zero, userData);
	}

	public static Body CreateCircle(World world, FP radius, FP density, TSVector2 position, object userData = null)
	{
		Body body = CreateBody(world, position);
		FixtureFactory.AttachCircle(radius, density, body, userData);
		return body;
	}

	public static Body CreateEllipse(World world, FP xRadius, FP yRadius, int edges, FP density, object userData = null)
	{
		return CreateEllipse(world, xRadius, yRadius, edges, density, TSVector2.zero, userData);
	}

	public static Body CreateEllipse(World world, FP xRadius, FP yRadius, int edges, FP density, TSVector2 position, object userData = null)
	{
		Body body = CreateBody(world, position);
		FixtureFactory.AttachEllipse(xRadius, yRadius, edges, density, body, userData);
		return body;
	}

	public static Body CreatePolygon(World world, Vertices vertices, FP density, object userData)
	{
		return CreatePolygon(world, vertices, density, TSVector2.zero, userData);
	}

	public static Body CreatePolygon(World world, Vertices vertices, FP density, TSVector2 position, object userData)
	{
		Body body = CreateBody(world, position);
		FixtureFactory.AttachPolygon(vertices, density, body, userData);
		return body;
	}

	public static Body CreateCompoundPolygon(World world, List<Vertices> list, FP density, object userData = null)
	{
		return CreateCompoundPolygon(world, list, density, TSVector2.zero, userData);
	}

	public static Body CreateCompoundPolygon(World world, List<Vertices> list, FP density, TSVector2 position, object userData = null)
	{
		Body body = CreateBody(world, position);
		FixtureFactory.AttachCompoundPolygon(list, density, body, userData);
		return body;
	}

	public static Body CreateGear(World world, FP radius, int numberOfTeeth, FP tipPercentage, FP toothHeight, FP density, object userData = null)
	{
		Vertices vertices = PolygonTools.CreateGear(radius, numberOfTeeth, tipPercentage, toothHeight);
		if (!vertices.IsConvex())
		{
			List<Vertices> list = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Earclip, discardAndFixInvalid: true, FP.EN3);
			return CreateCompoundPolygon(world, list, density, userData);
		}
		return CreatePolygon(world, vertices, density, userData);
	}

	public static Body CreateCapsule(World world, FP height, FP topRadius, int topEdges, FP bottomRadius, int bottomEdges, FP density, TSVector2 position, object userData = null)
	{
		Vertices vertices = PolygonTools.CreateCapsule(height, topRadius, topEdges, bottomRadius, bottomEdges);
		Body body;
		if (vertices.Count >= Settings.MaxPolygonVertices)
		{
			List<Vertices> list = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Earclip, discardAndFixInvalid: true, FP.EN3);
			body = CreateCompoundPolygon(world, list, density, userData);
			body.Position = position;
			return body;
		}
		body = CreatePolygon(world, vertices, density, userData);
		body.Position = position;
		return body;
	}

	public static Body CreateCapsule(World world, FP height, FP endRadius, FP density, object userData = null)
	{
		Vertices item = PolygonTools.CreateRectangle(endRadius, height / 2);
		List<Vertices> list = new List<Vertices>();
		list.Add(item);
		Body body = CreateCompoundPolygon(world, list, density, userData);
		body.UserData = userData;
		CircleShape circleShape = new CircleShape(endRadius, density);
		circleShape.Position = new TSVector2(0, height / 2);
		body.CreateFixture(circleShape);
		CircleShape circleShape2 = new CircleShape(endRadius, density);
		circleShape2.Position = new TSVector2(0, -(height / 2));
		body.CreateFixture(circleShape2);
		return body;
	}

	public static Body CreateRoundedRectangle(World world, FP width, FP height, FP xRadius, FP yRadius, int segments, FP density, TSVector2 position, object userData = null)
	{
		Vertices vertices = PolygonTools.CreateRoundedRectangle(width, height, xRadius, yRadius, segments);
		if (vertices.Count >= Settings.MaxPolygonVertices)
		{
			List<Vertices> list = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Earclip, discardAndFixInvalid: true, FP.EN3);
			Body body = CreateCompoundPolygon(world, list, density, userData);
			body.Position = position;
			return body;
		}
		return CreatePolygon(world, vertices, density, null);
	}

	public static Body CreateRoundedRectangle(World world, FP width, FP height, FP xRadius, FP yRadius, int segments, FP density, object userData = null)
	{
		return CreateRoundedRectangle(world, width, height, xRadius, yRadius, segments, density, TSVector2.zero, userData);
	}

	public static BreakableBody CreateBreakableBody(World world, Vertices vertices, FP density)
	{
		return CreateBreakableBody(world, vertices, density, TSVector2.zero);
	}

	public static BreakableBody CreateBreakableBody(World world, IEnumerable<Shape> shapes)
	{
		return CreateBreakableBody(world, shapes, TSVector2.zero);
	}

	public static BreakableBody CreateBreakableBody(World world, Vertices vertices, FP density, TSVector2 position)
	{
		List<Vertices> vertices2 = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Earclip, discardAndFixInvalid: true, FP.EN3);
		BreakableBody breakableBody = new BreakableBody(vertices2, world, density);
		breakableBody.MainBody.Position = position;
		world.AddBreakableBody(breakableBody);
		return breakableBody;
	}

	public static BreakableBody CreateBreakableBody(World world, IEnumerable<Shape> shapes, TSVector2 position)
	{
		BreakableBody breakableBody = new BreakableBody(shapes, world);
		breakableBody.MainBody.Position = position;
		world.AddBreakableBody(breakableBody);
		return breakableBody;
	}

	public static Body CreateLineArc(World world, FP radians, int sides, FP radius, TSVector2 position, FP angle, bool closed)
	{
		Body body = CreateBody(world);
		FixtureFactory.AttachLineArc(radians, sides, radius, position, angle, closed, body);
		return body;
	}

	public static Body CreateSolidArc(World world, FP density, FP radians, int sides, FP radius, TSVector2 position, FP angle)
	{
		Body body = CreateBody(world);
		FixtureFactory.AttachSolidArc(density, radians, sides, radius, position, angle, body);
		return body;
	}
}
