using System.Collections.Generic;

namespace TrueSync.Physics2D;

public static class LinkFactory
{
	public static Path CreateChain(World world, TSVector2 start, TSVector2 end, FP linkWidth, FP linkHeight, int numberOfLinks, FP linkDensity, bool attachRopeJoint)
	{
		Path path = new Path();
		path.Add(start);
		path.Add(end);
		PolygonShape shape = new PolygonShape(PolygonTools.CreateRectangle(linkWidth, linkHeight), linkDensity);
		List<Body> list = PathManager.EvenlyDistributeShapesAlongPath(world, path, shape, BodyType.Dynamic, numberOfLinks);
		PathManager.AttachBodiesWithRevoluteJoint(world, list, new TSVector2(0, -linkHeight), new TSVector2(0, linkHeight), connectFirstAndLast: false, collideConnected: false);
		if (attachRopeJoint)
		{
			JointFactory.CreateRopeJoint(world, list[0], list[list.Count - 1], TSVector2.zero, TSVector2.zero);
		}
		return path;
	}
}
