using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueSync.Physics2D;

public sealed class RealExplosion : PhysicsLogic
{
	private static readonly FP MaxEdgeOffset = FP.Pi / 90;

	public FP EdgeRatio = 0.025f;

	public bool IgnoreWhenInsideShape;

	public FP MaxAngle = FP.Pi / 15;

	public int MaxShapes = 100;

	public int MinRays = 5;

	private List<ShapeData> _data = new List<ShapeData>();

	private RayDataComparer _rdc;

	public RealExplosion(World world)
		: base(world, PhysicsLogicType.Explosion)
	{
		_rdc = new RayDataComparer();
		_data = new List<ShapeData>();
	}

	public Dictionary<Fixture, TSVector2> Activate(TSVector2 pos, FP radius, FP maxForce)
	{
		AABB aabb = default;
		aabb.LowerBound = pos + new TSVector2(-radius, -radius);
		aabb.UpperBound = pos + new TSVector2(radius, radius);
		Fixture[] shapes = new Fixture[MaxShapes];
		Fixture[] containedShapes = new Fixture[5];
		bool exit = false;
		int shapeCount = 0;
		int containedShapeCount = 0;
		World.QueryAABB((Fixture fixture4) =>
		{
			if (fixture4.TestPoint(ref pos))
			{
				if (IgnoreWhenInsideShape)
				{
					exit = true;
					return false;
				}
				containedShapes[containedShapeCount++] = fixture4;
			}
			else
			{
				shapes[shapeCount++] = fixture4;
			}
			return true;
		}, ref aabb);
		if (exit)
		{
			return new Dictionary<Fixture, TSVector2>();
		}
		Dictionary<Fixture, TSVector2> dictionary = new Dictionary<Fixture, TSVector2>(shapeCount + containedShapeCount);
		FP[] array = new FP[shapeCount * 2];
		int num = 0;
		for (int num2 = 0; num2 < shapeCount; num2++)
		{
			PolygonShape polygonShape;
			if (shapes[num2].Shape is CircleShape circleShape)
			{
				Vertices vertices = new Vertices();
				TSVector2 item = TSVector2.zero + new TSVector2(circleShape.Radius, 0);
				vertices.Add(item);
				item = TSVector2.zero + new TSVector2(0, circleShape.Radius);
				vertices.Add(item);
				item = TSVector2.zero + new TSVector2(-circleShape.Radius, circleShape.Radius);
				vertices.Add(item);
				item = TSVector2.zero + new TSVector2(0, -circleShape.Radius);
				vertices.Add(item);
				polygonShape = new PolygonShape(vertices, 0);
			}
			else
			{
				polygonShape = shapes[num2].Shape as PolygonShape;
			}
			if (shapes[num2].Body.BodyType != BodyType.Dynamic || polygonShape == null)
			{
				continue;
			}
			TSVector2 tSVector = shapes[num2].Body.GetWorldPoint(polygonShape.MassData.Centroid) - pos;
			FP fP = FP.Atan2(tSVector.y, tSVector.x);
			FP fP2 = FP.MaxValue;
			FP fP3 = FP.MinValue;
			FP fP4 = 0f;
			FP fP5 = 0f;
			for (int num3 = 0; num3 < polygonShape.Vertices.Count; num3++)
			{
				TSVector2 tSVector2 = shapes[num2].Body.GetWorldPoint(polygonShape.Vertices[num3]) - pos;
				FP fP6 = FP.Atan2(tSVector2.y, tSVector2.x);
				FP fP7 = fP6 - fP;
				fP7 = (fP7 - FP.Pi) % (2 * FP.Pi);
				if (fP7 < 0f)
				{
					fP7 += 2 * FP.Pi;
				}
				fP7 -= FP.Pi;
				if (!(FP.Abs(fP7) > FP.Pi))
				{
					if (fP7 > fP3)
					{
						fP3 = fP7;
						fP5 = fP6;
					}
					if (fP7 < fP2)
					{
						fP2 = fP7;
						fP4 = fP6;
					}
				}
			}
			array[num] = fP4;
			num++;
			array[num] = fP5;
			num++;
		}
		Array.Sort(array, 0, num, _rdc);
		_data.Clear();
		bool flag = true;
		ShapeData item2 = default;
		for (int num4 = 0; num4 < num; num4++)
		{
			Fixture fixture = null;
			int num5 = ((num4 != num - 1) ? (num4 + 1) : 0);
			if (array[num4] == array[num5])
			{
				continue;
			}
			FP x = ((num4 != num - 1) ? (array[num4 + 1] + array[num4]) : (array[0] + FP.PiTimes2 + array[num4]));
			x /= (FP)2;
			TSVector2 point = pos;
			TSVector2 point2 = radius * new TSVector2(FP.Cos(x), FP.Sin(x)) + pos;
			bool hitClosest = false;
			World.RayCast((Fixture f, TSVector2 p, TSVector2 n, FP fr) =>
			{
				Body body = f.Body;
				if (!IsActiveOn(body))
				{
					return 0;
				}
				hitClosest = true;
				fixture = f;
				return fr;
			}, point, point2);
			if (hitClosest && fixture.Body.BodyType == BodyType.Dynamic)
			{
				if (_data.Any() && _data.Last().Body == fixture.Body && !flag)
				{
					int index = _data.Count - 1;
					ShapeData value = _data[index];
					value.Max = array[num5];
					_data[index] = value;
				}
				else
				{
					item2.Body = fixture.Body;
					item2.Min = array[num4];
					item2.Max = array[num5];
					_data.Add(item2);
				}
				if (_data.Count > 1 && num4 == num - 1 && _data.Last().Body == _data.First().Body && _data.Last().Max == _data.First().Min)
				{
					ShapeData value2 = _data[0];
					value2.Min = _data.Last().Min;
					_data.RemoveAt(_data.Count - 1);
					_data[0] = value2;
					while (_data.First().Min >= _data.First().Max)
					{
						value2.Min -= FP.PiTimes2;
						_data[0] = value2;
					}
				}
				int index2 = _data.Count - 1;
				ShapeData value3 = _data[index2];
				while (_data.Count > 0 && _data.Last().Min >= _data.Last().Max)
				{
					value3.Min = _data.Last().Min - FP.PiTimes2;
					_data[index2] = value3;
				}
				flag = false;
			}
			else
			{
				flag = true;
			}
		}
		RayCastInput input = default;
		for (int num6 = 0; num6 < _data.Count; num6++)
		{
			if (!IsActiveOn(_data[num6].Body))
			{
				continue;
			}
			FP fP8 = _data[num6].Max - _data[num6].Min;
			FP fP9 = TSMath.Min(MaxEdgeOffset, EdgeRatio * fP8);
			int num7 = FP.Ceiling((fP8 - 2f * fP9 - (MinRays - 1) * MaxAngle) / MaxAngle).AsInt();
			if (num7 < 0)
			{
				num7 = 0;
			}
			FP fP10 = (fP8 - fP9 * 2f) / ((FP)MinRays + (FP)num7 - 1);
			for (FP fP11 = _data[num6].Min + fP9; fP11 < _data[num6].Max || MathUtils.FPEquals(fP11, _data[num6].Max, 0.0001f); fP11 += fP10)
			{
				TSVector2 tSVector3 = pos;
				TSVector2 tSVector4 = pos + radius * new TSVector2(FP.Cos(fP11), FP.Sin(fP11));
				TSVector2 point3 = TSVector2.zero;
				FP fP12 = FP.MaxValue;
				List<Fixture> fixtureList = _data[num6].Body.FixtureList;
				for (int num8 = 0; num8 < fixtureList.Count; num8++)
				{
					Fixture fixture2 = fixtureList[num8];
					input.Point1 = tSVector3;
					input.Point2 = tSVector4;
					input.MaxFraction = 50f;
					if (fixture2.RayCast(out var output, ref input, 0) && fP12 > output.Fraction)
					{
						fP12 = output.Fraction;
						point3 = output.Fraction * tSVector4 + (1 - output.Fraction) * tSVector3;
					}
					FP fP13 = fP8 / (MinRays + num7) * maxForce * 180f / FP.Pi * (1f - TSMath.Min(FP.One, fP12));
					TSVector2 impulse = TSVector2.Dot(fP13 * new TSVector2(FP.Cos(fP11), FP.Sin(fP11)), -output.Normal) * new TSVector2(FP.Cos(fP11), FP.Sin(fP11));
					_data[num6].Body.ApplyLinearImpulse(ref impulse, ref point3);
					if (dictionary.ContainsKey(fixture2))
					{
						dictionary[fixture2] += impulse;
					}
					else
					{
						dictionary.Add(fixture2, impulse);
					}
					if (fP12 > 1f)
					{
						point3 = tSVector4;
					}
				}
			}
		}
		for (int num9 = 0; num9 < containedShapeCount; num9++)
		{
			Fixture fixture3 = containedShapes[num9];
			if (IsActiveOn(fixture3.Body))
			{
				FP fP14 = MinRays * maxForce * 180f / FP.Pi;
				TSVector2 point4;
				if (fixture3.Shape is CircleShape circleShape2)
				{
					point4 = fixture3.Body.GetWorldPoint(circleShape2.Position);
				}
				else
				{
					PolygonShape polygonShape2 = fixture3.Shape as PolygonShape;
					point4 = fixture3.Body.GetWorldPoint(polygonShape2.MassData.Centroid);
				}
				TSVector2 impulse2 = fP14 * (point4 - pos);
				fixture3.Body.ApplyLinearImpulse(ref impulse2, ref point4);
				if (!dictionary.ContainsKey(fixture3))
				{
					dictionary.Add(fixture3, impulse2);
				}
			}
		}
		return dictionary;
	}
}
