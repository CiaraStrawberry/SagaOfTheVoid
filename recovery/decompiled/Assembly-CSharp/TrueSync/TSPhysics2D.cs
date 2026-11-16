using TrueSync.Physics2D;

namespace TrueSync;

public class TSPhysics2D
{
	public enum TSCapsuleDirection2D
	{
		VERTICAL,
		HORIZONTAL
	}

	private static FP POINT_RADIUS = 0.001f;

	private static object OverlapGeneric(Shape shape, TSVector2 position, BodySpecialSensor sensorType, int layerMask)
	{
		World world = (World)Physics2DWorldManager.instance.GetWorld();
		Body body = BodyFactory.CreateBody(world);
		body.CreateFixture(shape);
		body.BodyType = BodyType.Static;
		body.IsSensor = true;
		body.CollidesWith = Category.All;
		body.SpecialSensor = sensorType;
		body.SpecialSensorMask = layerMask;
		body.Position = position;
		world.RemoveBody(body);
		world.ProcessRemovedBodies();
		if (body._specialSensorResults.Count > 0)
		{
			if (sensorType == BodySpecialSensor.ActiveOnce)
			{
				return Physics2DWorldManager.instance.GetGameObject(body._specialSensorResults[0]).GetComponent<TSCollider2D>();
			}
			TSCollider2D[] array = new TSCollider2D[body._specialSensorResults.Count];
			for (int i = 0; i < body._specialSensorResults.Count; i++)
			{
				array[i] = Physics2DWorldManager.instance.GetGameObject(body._specialSensorResults[i]).GetComponent<TSCollider2D>();
			}
			return array;
		}
		return null;
	}

	private static object _OverlapCircle(TSVector2 point, FP radius, BodySpecialSensor sensorType, int layerMask = -5)
	{
		return OverlapGeneric(new CircleShape(radius, 1), point, sensorType, layerMask);
	}

	public static TSCollider2D OverlapCircle(TSVector2 point, FP radius, int layerMask = -5)
	{
		return (TSCollider2D)_OverlapCircle(point, radius, BodySpecialSensor.ActiveOnce, layerMask);
	}

	public static TSCollider2D[] OverlapCircleAll(TSVector2 point, FP radius, int layerMask = -5)
	{
		return (TSCollider2D[])_OverlapCircle(point, radius, BodySpecialSensor.ActiveAll, layerMask);
	}

	public static object _OverlapArea(TSVector2 pointA, TSVector2 pointB, BodySpecialSensor sensorType, int layerMask = -5)
	{
		TSVector2 tSVector = default;
		tSVector.x = (pointA.x + pointB.x) * FP.Half;
		tSVector.y = (pointA.y + pointB.y) * FP.Half;
		Vertices vertices = new Vertices(4);
		vertices.Add(new TSVector2(pointA.x, pointA.y) - tSVector);
		vertices.Add(new TSVector2(pointB.x, pointA.y) - tSVector);
		vertices.Add(new TSVector2(pointB.x, pointB.y) - tSVector);
		vertices.Add(new TSVector2(pointA.x, pointB.y) - tSVector);
		return OverlapGeneric(new PolygonShape(vertices, 1), tSVector, sensorType, layerMask);
	}

	public static TSCollider2D OverlapArea(TSVector2 pointA, TSVector2 pointB, int layerMask = -5)
	{
		return (TSCollider2D)_OverlapArea(pointA, pointB, BodySpecialSensor.ActiveOnce, layerMask);
	}

	public static TSCollider2D[] OverlapAreaAll(TSVector2 pointA, TSVector2 pointB, int layerMask = -5)
	{
		return (TSCollider2D[])_OverlapArea(pointA, pointB, BodySpecialSensor.ActiveAll, layerMask);
	}

	public static TSCollider2D OverlapPoint(TSVector2 point, int layerMask = -5)
	{
		return (TSCollider2D)_OverlapCircle(point, POINT_RADIUS, BodySpecialSensor.ActiveOnce, layerMask);
	}

	public static TSCollider2D[] OverlapPointAll(TSVector2 point, int layerMask = -5)
	{
		return (TSCollider2D[])_OverlapCircle(point, POINT_RADIUS, BodySpecialSensor.ActiveAll, layerMask);
	}

	private static object _OverlapBox(TSVector2 point, TSVector2 size, FP angle, BodySpecialSensor sensorType, int layerMask = -5)
	{
		size *= FP.Half;
		angle *= FP.Deg2Rad;
		return OverlapGeneric(new PolygonShape(PolygonTools.CreateRectangle(size.x, size.y, point, angle * -1), 1), point, sensorType, layerMask);
	}

	public static TSCollider2D OverlapBox(TSVector2 point, TSVector2 size, FP angle, int layerMask = -5)
	{
		return (TSCollider2D)_OverlapBox(point, size, angle, BodySpecialSensor.ActiveOnce, layerMask);
	}

	public static TSCollider2D[] OverlapBoxAll(TSVector2 point, TSVector2 size, FP angle, int layerMask = -5)
	{
		return (TSCollider2D[])_OverlapBox(point, size, angle, BodySpecialSensor.ActiveAll, layerMask);
	}

	private static object _OverlapCapsule(TSVector2 point, TSVector2 size, TSCapsuleDirection2D direction, FP angle, BodySpecialSensor sensorType, int layerMask = -5)
	{
		if (direction == TSCapsuleDirection2D.HORIZONTAL)
		{
			FP y = size.y;
			size.y = size.x;
			size.x = y;
			angle += (FP)90;
		}
		FP fP = size.x * FP.Half;
		Vertices vertices = PolygonTools.CreateCapsule(size.y, fP, 8, fP, 8);
		PolygonTools.TransformVertices(vertices, point, angle * FP.Deg2Rad * -1);
		return OverlapGeneric(new PolygonShape(vertices, 1), point, sensorType, layerMask);
	}

	public static TSCollider2D OverlapCapsule(TSVector2 point, TSVector2 size, TSCapsuleDirection2D direction, FP angle, int layerMask = -5)
	{
		return (TSCollider2D)_OverlapCapsule(point, size, direction, angle, BodySpecialSensor.ActiveOnce, layerMask);
	}

	public static TSCollider2D[] OverlapCapsuleAll(TSVector2 point, TSVector2 size, TSCapsuleDirection2D direction, FP angle, int layerMask = -5)
	{
		return (TSCollider2D[])_OverlapCapsule(point, size, direction, angle, BodySpecialSensor.ActiveAll, layerMask);
	}

	public static object _CircleCast(TSVector2 origin, FP radius, TSVector2 direction, FP distance, BodySpecialSensor sensorType, int layerMask = -5)
	{
		if (distance + radius > FP.MaxValue)
		{
			distance = FP.MaxValue - radius;
		}
		direction.Normalize();
		TSVector2 tSVector = direction * distance * FP.Half;
		tSVector.x = FP.Abs(tSVector.x);
		tSVector.y = FP.Abs(tSVector.y);
		FP fP = TSVector2.Angle(direction, TSVector2.right);
		if (direction.x <= 0 && direction.y >= 0)
		{
			tSVector.x = -tSVector.x;
		}
		else if (direction.x <= 0 && direction.y <= 0)
		{
			tSVector.x = -tSVector.x;
			tSVector.y = -tSVector.y;
			fP = -fP;
		}
		else if (direction.x >= 0 && direction.y <= 0)
		{
			tSVector.y = -tSVector.y;
			fP = -fP;
		}
		TSVector2 point = origin + tSVector;
		object obj = _OverlapCapsule(point, new TSVector2(distance + radius * 2, radius * 2), TSCapsuleDirection2D.HORIZONTAL, -fP, sensorType, layerMask);
		if (obj is TSCollider2D)
		{
			return new TSRaycastHit2D((TSCollider2D)obj);
		}
		TSCollider2D[] array = (TSCollider2D[])obj;
		TSRaycastHit2D[] array2 = new TSRaycastHit2D[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = new TSRaycastHit2D(array[i]);
		}
		return array2;
	}

	public static TSRaycastHit2D CircleCast(TSVector2 origin, FP radius, TSVector2 direction, FP distance, int layerMask = -5)
	{
		return (TSRaycastHit2D)_CircleCast(origin, radius, direction, distance, BodySpecialSensor.ActiveOnce, layerMask);
	}

	public static TSRaycastHit2D[] CircleCastAll(TSVector2 origin, FP radius, TSVector2 direction, FP distance, int layerMask = -5)
	{
		return (TSRaycastHit2D[])_CircleCast(origin, radius, direction, distance, BodySpecialSensor.ActiveAll, layerMask);
	}

	public static TSRaycastHit2D Raycast(TSVector2 origin, TSVector2 direction, FP distance)
	{
		return Physics2DWorldManager.instance.Raycast(origin, direction, distance);
	}

	public static TSRaycastHit2D[] RaycastAll(TSVector2 origin, TSVector2 direction, FP distance)
	{
		return Physics2DWorldManager.instance.RaycastAll(origin, direction, distance);
	}
}
