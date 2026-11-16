using System;
using System.Collections.Generic;

namespace TrueSync.Physics2D;

public static class PathManager
{
	public enum LinkType
	{
		Revolute,
		Slider
	}

	public static void ConvertPathToEdges(Path path, Body body, int subdivisions)
	{
		Vertices vertices = path.GetVertices(subdivisions);
		if (path.Closed)
		{
			ChainShape shape = new ChainShape(vertices, createLoop: true);
			body.CreateFixture(shape);
			return;
		}
		for (int i = 1; i < vertices.Count; i++)
		{
			body.CreateFixture(new EdgeShape(vertices[i], vertices[i - 1]));
		}
	}

	public static void ConvertPathToPolygon(Path path, Body body, FP density, int subdivisions)
	{
		if (!path.Closed)
		{
			throw new Exception("The path must be closed to convert to a polygon.");
		}
		List<TSVector2> vertices = path.GetVertices(subdivisions);
		List<Vertices> list = Triangulate.ConvexPartition(new Vertices(vertices), TriangulationAlgorithm.Bayazit, discardAndFixInvalid: true, FP.EN3);
		foreach (Vertices item in list)
		{
			body.CreateFixture(new PolygonShape(item, density));
		}
	}

	public static List<Body> EvenlyDistributeShapesAlongPath(World world, Path path, IEnumerable<Shape> shapes, BodyType type, int copies, object userData = null)
	{
		List<TSVector> list = path.SubdivideEvenly(copies);
		List<Body> list2 = new List<Body>();
		for (int i = 0; i < list.Count; i++)
		{
			Body body = new Body(world, null, 0);
			body.BodyType = type;
			body.Position = new TSVector2(list[i].x, list[i].y);
			body.Rotation = list[i].z;
			body.UserData = userData;
			foreach (Shape shape in shapes)
			{
				body.CreateFixture(shape);
			}
			list2.Add(body);
		}
		return list2;
	}

	public static List<Body> EvenlyDistributeShapesAlongPath(World world, Path path, Shape shape, BodyType type, int copies, object userData)
	{
		List<Shape> list = new List<Shape>(1);
		list.Add(shape);
		return EvenlyDistributeShapesAlongPath(world, path, list, type, copies, userData);
	}

	public static List<Body> EvenlyDistributeShapesAlongPath(World world, Path path, Shape shape, BodyType type, int copies)
	{
		return EvenlyDistributeShapesAlongPath(world, path, shape, type, copies, null);
	}

	public static void MoveBodyOnPath(Path path, Body body, FP time, FP strength, FP timeStep)
	{
		TSVector2 position = path.GetPosition(time);
		TSVector2 tSVector = body.Position - position;
		TSVector2 tSVector2 = tSVector / timeStep * strength;
		body.LinearVelocity = -tSVector2;
	}

	public static List<RevoluteJoint> AttachBodiesWithRevoluteJoint(World world, List<Body> bodies, TSVector2 localAnchorA, TSVector2 localAnchorB, bool connectFirstAndLast, bool collideConnected)
	{
		List<RevoluteJoint> list = new List<RevoluteJoint>(bodies.Count + 1);
		for (int i = 1; i < bodies.Count; i++)
		{
			RevoluteJoint revoluteJoint = new RevoluteJoint(bodies[i], bodies[i - 1], localAnchorA, localAnchorB);
			revoluteJoint.CollideConnected = collideConnected;
			world.AddJoint(revoluteJoint);
			list.Add(revoluteJoint);
		}
		if (connectFirstAndLast)
		{
			RevoluteJoint revoluteJoint2 = new RevoluteJoint(bodies[0], bodies[bodies.Count - 1], localAnchorA, localAnchorB);
			revoluteJoint2.CollideConnected = collideConnected;
			world.AddJoint(revoluteJoint2);
			list.Add(revoluteJoint2);
		}
		return list;
	}
}
