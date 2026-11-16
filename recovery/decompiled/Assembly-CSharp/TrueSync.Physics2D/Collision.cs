using System;
using System.Collections.Generic;

namespace TrueSync.Physics2D;

public static class Collision
{
	private class EPCollider
	{
		private TempPolygon _polygonB = new TempPolygon();

		private Transform _xf;

		private TSVector2 _centroidB;

		private TSVector2 _v0;

		private TSVector2 _v1;

		private TSVector2 _v2;

		private TSVector2 _v3;

		private TSVector2 _normal0;

		private TSVector2 _normal1;

		private TSVector2 _normal2;

		private TSVector2 _normal;

		private TSVector2 _lowerLimit;

		private TSVector2 _upperLimit;

		private FP _radius;

		private bool _front;

		public void Collide(ref Manifold manifold, EdgeShape edgeA, ref Transform xfA, PolygonShape polygonB, ref Transform xfB)
		{
			_xf = MathUtils.MulT(xfA, xfB);
			_centroidB = MathUtils.Mul(ref _xf, polygonB.MassData.Centroid);
			_v0 = edgeA.Vertex0;
			_v1 = edgeA._vertex1;
			_v2 = edgeA._vertex2;
			_v3 = edgeA.Vertex3;
			bool hasVertex = edgeA.HasVertex0;
			bool hasVertex2 = edgeA.HasVertex3;
			TSVector2 tSVector = _v2 - _v1;
			tSVector.Normalize();
			_normal1 = new TSVector2(tSVector.y, -tSVector.x);
			FP fP = TSVector2.Dot(_normal1, _centroidB - _v1);
			FP fP2 = 0f;
			FP fP3 = 0f;
			bool flag = false;
			bool flag2 = false;
			if (hasVertex)
			{
				TSVector2 a = _v1 - _v0;
				a.Normalize();
				_normal0 = new TSVector2(a.y, -a.x);
				flag = MathUtils.Cross(a, tSVector) >= 0f;
				fP2 = TSVector2.Dot(_normal0, _centroidB - _v0);
			}
			if (hasVertex2)
			{
				TSVector2 b = _v3 - _v2;
				b.Normalize();
				_normal2 = new TSVector2(b.y, -b.x);
				flag2 = MathUtils.Cross(tSVector, b) > 0f;
				fP3 = TSVector2.Dot(_normal2, _centroidB - _v2);
			}
			if (hasVertex && hasVertex2)
			{
				if (flag && flag2)
				{
					_front = fP2 >= 0f || fP >= 0f || fP3 >= 0f;
					if (_front)
					{
						_normal = _normal1;
						_lowerLimit = _normal0;
						_upperLimit = _normal2;
					}
					else
					{
						_normal = -_normal1;
						_lowerLimit = -_normal1;
						_upperLimit = -_normal1;
					}
				}
				else if (flag)
				{
					_front = fP2 >= 0f || (fP >= 0f && fP3 >= 0f);
					if (_front)
					{
						_normal = _normal1;
						_lowerLimit = _normal0;
						_upperLimit = _normal1;
					}
					else
					{
						_normal = -_normal1;
						_lowerLimit = -_normal2;
						_upperLimit = -_normal1;
					}
				}
				else if (flag2)
				{
					_front = fP3 >= 0f || (fP2 >= 0f && fP >= 0f);
					if (_front)
					{
						_normal = _normal1;
						_lowerLimit = _normal1;
						_upperLimit = _normal2;
					}
					else
					{
						_normal = -_normal1;
						_lowerLimit = -_normal1;
						_upperLimit = -_normal0;
					}
				}
				else
				{
					_front = fP2 >= 0f && fP >= 0f && fP3 >= 0f;
					if (_front)
					{
						_normal = _normal1;
						_lowerLimit = _normal1;
						_upperLimit = _normal1;
					}
					else
					{
						_normal = -_normal1;
						_lowerLimit = -_normal2;
						_upperLimit = -_normal0;
					}
				}
			}
			else if (hasVertex)
			{
				if (flag)
				{
					_front = fP2 >= 0f || fP >= 0f;
					if (_front)
					{
						_normal = _normal1;
						_lowerLimit = _normal0;
						_upperLimit = -_normal1;
					}
					else
					{
						_normal = -_normal1;
						_lowerLimit = _normal1;
						_upperLimit = -_normal1;
					}
				}
				else
				{
					_front = fP2 >= 0f && fP >= 0f;
					if (_front)
					{
						_normal = _normal1;
						_lowerLimit = _normal1;
						_upperLimit = -_normal1;
					}
					else
					{
						_normal = -_normal1;
						_lowerLimit = _normal1;
						_upperLimit = -_normal0;
					}
				}
			}
			else if (hasVertex2)
			{
				if (flag2)
				{
					_front = fP >= 0f || fP3 >= 0f;
					if (_front)
					{
						_normal = _normal1;
						_lowerLimit = -_normal1;
						_upperLimit = _normal2;
					}
					else
					{
						_normal = -_normal1;
						_lowerLimit = -_normal1;
						_upperLimit = _normal1;
					}
				}
				else
				{
					_front = fP >= 0f && fP3 >= 0f;
					if (_front)
					{
						_normal = _normal1;
						_lowerLimit = -_normal1;
						_upperLimit = _normal1;
					}
					else
					{
						_normal = -_normal1;
						_lowerLimit = -_normal2;
						_upperLimit = _normal1;
					}
				}
			}
			else
			{
				_front = fP >= 0f;
				if (_front)
				{
					_normal = _normal1;
					_lowerLimit = -_normal1;
					_upperLimit = -_normal1;
				}
				else
				{
					_normal = -_normal1;
					_lowerLimit = _normal1;
					_upperLimit = _normal1;
				}
			}
			_polygonB.Count = polygonB.Vertices.Count;
			for (int i = 0; i < polygonB.Vertices.Count; i++)
			{
				ref TSVector2 reference = ref _polygonB.Vertices[i];
				reference = MathUtils.Mul(ref _xf, polygonB.Vertices[i]);
				ref TSVector2 reference2 = ref _polygonB.Normals[i];
				reference2 = MathUtils.Mul(_xf.q, polygonB.Normals[i]);
			}
			_radius = 2f * Settings.PolygonRadius;
			manifold.PointCount = 0;
			EPAxis ePAxis = ComputeEdgeSeparation();
			if (ePAxis.Type == EPAxisType.Unknown || ePAxis.Separation > _radius)
			{
				return;
			}
			EPAxis ePAxis2 = ComputePolygonSeparation();
			if (ePAxis2.Type != EPAxisType.Unknown && ePAxis2.Separation > _radius)
			{
				return;
			}
			FP fP4 = 0.98f;
			FP fP5 = 0.001f;
			EPAxis ePAxis3;
			if (ePAxis2.Type != EPAxisType.Unknown)
			{
				ePAxis3 = ((!(ePAxis2.Separation > fP4 * ePAxis.Separation + fP5)) ? ePAxis : ePAxis2);
			}
			else
			{
				ePAxis3 = ePAxis;
			}
			FixedArray2<ClipVertex> vIn = default;
			ReferenceFace referenceFace = default;
			if (ePAxis3.Type == EPAxisType.EdgeA)
			{
				manifold.Type = ManifoldType.FaceA;
				int num = 0;
				FP fP6 = TSVector2.Dot(_normal, _polygonB.Normals[0]);
				for (int j = 1; j < _polygonB.Count; j++)
				{
					FP fP7 = TSVector2.Dot(_normal, _polygonB.Normals[j]);
					if (fP7 < fP6)
					{
						fP6 = fP7;
						num = j;
					}
				}
				int num2 = num;
				int num3 = ((num2 + 1 < _polygonB.Count) ? (num2 + 1) : 0);
				ClipVertex value = vIn[0];
				value.V = _polygonB.Vertices[num2];
				value.ID.Features.IndexA = 0;
				value.ID.Features.IndexB = (byte)num2;
				value.ID.Features.TypeA = 1;
				value.ID.Features.TypeB = 0;
				vIn[0] = value;
				ClipVertex value2 = vIn[1];
				value2.V = _polygonB.Vertices[num3];
				value2.ID.Features.IndexA = 0;
				value2.ID.Features.IndexB = (byte)num3;
				value2.ID.Features.TypeA = 1;
				value2.ID.Features.TypeB = 0;
				vIn[1] = value2;
				if (_front)
				{
					referenceFace.i1 = 0;
					referenceFace.i2 = 1;
					referenceFace.v1 = _v1;
					referenceFace.v2 = _v2;
					referenceFace.normal = _normal1;
				}
				else
				{
					referenceFace.i1 = 1;
					referenceFace.i2 = 0;
					referenceFace.v1 = _v2;
					referenceFace.v2 = _v1;
					referenceFace.normal = -_normal1;
				}
			}
			else
			{
				manifold.Type = ManifoldType.FaceB;
				ClipVertex value3 = vIn[0];
				value3.V = _v1;
				value3.ID.Features.IndexA = 0;
				value3.ID.Features.IndexB = (byte)ePAxis3.Index;
				value3.ID.Features.TypeA = 0;
				value3.ID.Features.TypeB = 1;
				vIn[0] = value3;
				ClipVertex value4 = vIn[1];
				value4.V = _v2;
				value4.ID.Features.IndexA = 0;
				value4.ID.Features.IndexB = (byte)ePAxis3.Index;
				value4.ID.Features.TypeA = 0;
				value4.ID.Features.TypeB = 1;
				vIn[1] = value4;
				referenceFace.i1 = ePAxis3.Index;
				referenceFace.i2 = ((referenceFace.i1 + 1 < _polygonB.Count) ? (referenceFace.i1 + 1) : 0);
				referenceFace.v1 = _polygonB.Vertices[referenceFace.i1];
				referenceFace.v2 = _polygonB.Vertices[referenceFace.i2];
				referenceFace.normal = _polygonB.Normals[referenceFace.i1];
			}
			referenceFace.sideNormal1 = new TSVector2(referenceFace.normal.y, -referenceFace.normal.x);
			referenceFace.sideNormal2 = -referenceFace.sideNormal1;
			referenceFace.sideOffset1 = TSVector2.Dot(referenceFace.sideNormal1, referenceFace.v1);
			referenceFace.sideOffset2 = TSVector2.Dot(referenceFace.sideNormal2, referenceFace.v2);
			int num4 = ClipSegmentToLine(out var vOut, ref vIn, referenceFace.sideNormal1, referenceFace.sideOffset1, referenceFace.i1);
			if (num4 < 2)
			{
				return;
			}
			num4 = ClipSegmentToLine(out var vOut2, ref vOut, referenceFace.sideNormal2, referenceFace.sideOffset2, referenceFace.i2);
			if (num4 < 2)
			{
				return;
			}
			if (ePAxis3.Type == EPAxisType.EdgeA)
			{
				manifold.LocalNormal = referenceFace.normal;
				manifold.LocalPoint = referenceFace.v1;
			}
			else
			{
				manifold.LocalNormal = polygonB.Normals[referenceFace.i1];
				manifold.LocalPoint = polygonB.Vertices[referenceFace.i1];
			}
			int num5 = 0;
			for (int k = 0; k < 2; k++)
			{
				FP fP8 = TSVector2.Dot(referenceFace.normal, vOut2[k].V - referenceFace.v1);
				if (fP8 <= _radius)
				{
					ManifoldPoint value5 = manifold.Points[num5];
					if (ePAxis3.Type == EPAxisType.EdgeA)
					{
						value5.LocalPoint = MathUtils.MulT(ref _xf, vOut2[k].V);
						value5.Id = vOut2[k].ID;
					}
					else
					{
						value5.LocalPoint = vOut2[k].V;
						value5.Id.Features.TypeA = vOut2[k].ID.Features.TypeB;
						value5.Id.Features.TypeB = vOut2[k].ID.Features.TypeA;
						value5.Id.Features.IndexA = vOut2[k].ID.Features.IndexB;
						value5.Id.Features.IndexB = vOut2[k].ID.Features.IndexA;
					}
					manifold.Points[num5] = value5;
					num5++;
				}
			}
			manifold.PointCount = num5;
		}

		private EPAxis ComputeEdgeSeparation()
		{
			EPAxis result = default;
			result.Type = EPAxisType.EdgeA;
			result.Index = ((!_front) ? 1 : 0);
			result.Separation = Settings.MaxFP;
			for (int i = 0; i < _polygonB.Count; i++)
			{
				FP fP = TSVector2.Dot(_normal, _polygonB.Vertices[i] - _v1);
				if (fP < result.Separation)
				{
					result.Separation = fP;
				}
			}
			return result;
		}

		private EPAxis ComputePolygonSeparation()
		{
			EPAxis result = default;
			result.Type = EPAxisType.Unknown;
			result.Index = -1;
			result.Separation = -Settings.MaxFP;
			TSVector2 value = new TSVector2(-_normal.y, _normal.x);
			for (int i = 0; i < _polygonB.Count; i++)
			{
				TSVector2 tSVector = -_polygonB.Normals[i];
				FP val = TSVector2.Dot(tSVector, _polygonB.Vertices[i] - _v1);
				FP val2 = TSVector2.Dot(tSVector, _polygonB.Vertices[i] - _v2);
				FP fP = TSMath.Min(val, val2);
				if (fP > _radius)
				{
					result.Type = EPAxisType.EdgeB;
					result.Index = i;
					result.Separation = fP;
					return result;
				}
				if (TSVector2.Dot(tSVector, value) >= 0f)
				{
					if (TSVector2.Dot(tSVector - _upperLimit, _normal) < -Settings.AngularSlop)
					{
						continue;
					}
				}
				else if (TSVector2.Dot(tSVector - _lowerLimit, _normal) < -Settings.AngularSlop)
				{
					continue;
				}
				if (fP > result.Separation)
				{
					result.Type = EPAxisType.EdgeB;
					result.Index = i;
					result.Separation = fP;
				}
			}
			return result;
		}
	}

	[ThreadStatic]
	private static DistanceInput _input;

	public static bool TestOverlap(Shape shapeA, int indexA, Shape shapeB, int indexB, ref Transform xfA, ref Transform xfB)
	{
		_input = _input ?? new DistanceInput();
		_input.ProxyA.Set(shapeA, indexA);
		_input.ProxyB.Set(shapeB, indexB);
		_input.TransformA = xfA;
		_input.TransformB = xfB;
		_input.UseRadii = true;
		Distance.ComputeDistance(out var output, out var _, _input);
		return output.Distance < 10f * Settings.Epsilon;
	}

	public static void GetPointStates(out FixedArray2<PointState> state1, out FixedArray2<PointState> state2, ref Manifold manifold1, ref Manifold manifold2)
	{
		state1 = default;
		state2 = default;
		for (int i = 0; i < manifold1.PointCount; i++)
		{
			ContactID id = manifold1.Points[i].Id;
			state1[i] = PointState.Remove;
			for (int j = 0; j < manifold2.PointCount; j++)
			{
				if (manifold2.Points[j].Id.Key == id.Key)
				{
					state1[i] = PointState.Persist;
					break;
				}
			}
		}
		for (int k = 0; k < manifold2.PointCount; k++)
		{
			ContactID id2 = manifold2.Points[k].Id;
			state2[k] = PointState.Add;
			for (int l = 0; l < manifold1.PointCount; l++)
			{
				if (manifold1.Points[l].Id.Key == id2.Key)
				{
					state2[k] = PointState.Persist;
					break;
				}
			}
		}
	}

	public static void CollideCircles(ref Manifold manifold, CircleShape circleA, ref Transform xfA, CircleShape circleB, ref Transform xfB)
	{
		manifold.PointCount = 0;
		TSVector2 tSVector = MathUtils.Mul(ref xfA, circleA.Position);
		TSVector2 tSVector2 = MathUtils.Mul(ref xfB, circleB.Position);
		TSVector2 tSVector3 = tSVector2 - tSVector;
		FP fP = TSVector2.Dot(tSVector3, tSVector3);
		FP fP2 = circleA.Radius + circleB.Radius;
		if (!(fP > fP2 * fP2))
		{
			manifold.Type = ManifoldType.Circles;
			manifold.LocalPoint = circleA.Position;
			manifold.LocalNormal = TSVector2.zero;
			manifold.PointCount = 1;
			ManifoldPoint value = manifold.Points[0];
			value.LocalPoint = circleB.Position;
			value.Id.Key = 0u;
			manifold.Points[0] = value;
		}
	}

	public static void CollidePolygonAndCircle(ref Manifold manifold, PolygonShape polygonA, ref Transform xfA, CircleShape circleB, ref Transform xfB)
	{
		manifold.PointCount = 0;
		TSVector2 v = MathUtils.Mul(ref xfB, circleB.Position);
		TSVector2 tSVector = MathUtils.MulT(ref xfA, v);
		int num = 0;
		FP fP = -Settings.MaxFP;
		FP fP2 = polygonA.Radius + circleB.Radius;
		int count = polygonA.Vertices.Count;
		for (int i = 0; i < count; i++)
		{
			TSVector2 tSVector2 = polygonA.Normals[i];
			TSVector2 tSVector3 = tSVector - polygonA.Vertices[i];
			FP fP3 = tSVector2.x * tSVector3.x + tSVector2.y * tSVector3.y;
			if (fP3 > fP2)
			{
				return;
			}
			if (fP3 > fP)
			{
				fP = fP3;
				num = i;
			}
		}
		int num2 = num;
		int index = ((num2 + 1 < count) ? (num2 + 1) : 0);
		TSVector2 tSVector4 = polygonA.Vertices[num2];
		TSVector2 tSVector5 = polygonA.Vertices[index];
		if (fP < Settings.Epsilon)
		{
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.FaceA;
			manifold.LocalNormal = polygonA.Normals[num];
			manifold.LocalPoint = 0.5f * (tSVector4 + tSVector5);
			ManifoldPoint value = manifold.Points[0];
			value.LocalPoint = circleB.Position;
			value.Id.Key = 0u;
			manifold.Points[0] = value;
			return;
		}
		FP fP4 = (tSVector.x - tSVector4.x) * (tSVector5.x - tSVector4.x) + (tSVector.y - tSVector4.y) * (tSVector5.y - tSVector4.y);
		FP fP5 = (tSVector.x - tSVector5.x) * (tSVector4.x - tSVector5.x) + (tSVector.y - tSVector5.y) * (tSVector4.y - tSVector5.y);
		if (fP4 <= 0f)
		{
			FP fP6 = (tSVector.x - tSVector4.x) * (tSVector.x - tSVector4.x) + (tSVector.y - tSVector4.y) * (tSVector.y - tSVector4.y);
			if (!(fP6 > fP2 * fP2))
			{
				manifold.PointCount = 1;
				manifold.Type = ManifoldType.FaceA;
				manifold.LocalNormal = tSVector - tSVector4;
				FP fP7 = 1f / FP.Sqrt(manifold.LocalNormal.x * manifold.LocalNormal.x + manifold.LocalNormal.y * manifold.LocalNormal.y);
				manifold.LocalNormal.x = manifold.LocalNormal.x * fP7;
				manifold.LocalNormal.y = manifold.LocalNormal.y * fP7;
				manifold.LocalPoint = tSVector4;
				ManifoldPoint value2 = manifold.Points[0];
				value2.LocalPoint = circleB.Position;
				value2.Id.Key = 0u;
				manifold.Points[0] = value2;
			}
		}
		else if (fP5 <= 0f)
		{
			FP fP8 = (tSVector.x - tSVector5.x) * (tSVector.x - tSVector5.x) + (tSVector.y - tSVector5.y) * (tSVector.y - tSVector5.y);
			if (!(fP8 > fP2 * fP2))
			{
				manifold.PointCount = 1;
				manifold.Type = ManifoldType.FaceA;
				manifold.LocalNormal = tSVector - tSVector5;
				FP fP9 = 1f / FP.Sqrt(manifold.LocalNormal.x * manifold.LocalNormal.x + manifold.LocalNormal.y * manifold.LocalNormal.y);
				manifold.LocalNormal.x = manifold.LocalNormal.x * fP9;
				manifold.LocalNormal.y = manifold.LocalNormal.y * fP9;
				manifold.LocalPoint = tSVector5;
				ManifoldPoint value3 = manifold.Points[0];
				value3.LocalPoint = circleB.Position;
				value3.Id.Key = 0u;
				manifold.Points[0] = value3;
			}
		}
		else
		{
			TSVector2 tSVector6 = 0.5f * (tSVector4 + tSVector5);
			TSVector2 tSVector7 = tSVector - tSVector6;
			TSVector2 tSVector8 = polygonA.Normals[num2];
			FP fP10 = tSVector7.x * tSVector8.x + tSVector7.y * tSVector8.y;
			if (!(fP10 > fP2))
			{
				manifold.PointCount = 1;
				manifold.Type = ManifoldType.FaceA;
				manifold.LocalNormal = polygonA.Normals[num2];
				manifold.LocalPoint = tSVector6;
				ManifoldPoint value4 = manifold.Points[0];
				value4.LocalPoint = circleB.Position;
				value4.Id.Key = 0u;
				manifold.Points[0] = value4;
			}
		}
	}

	public static void CollidePolygons(ref Manifold manifold, PolygonShape polyA, ref Transform transformA, PolygonShape polyB, ref Transform transformB)
	{
		manifold.PointCount = 0;
		FP fP = polyA.Radius + polyB.Radius;
		int edgeIndex = 0;
		FP fP2 = FindMaxSeparation(out edgeIndex, polyA, ref transformA, polyB, ref transformB);
		if (fP2 > fP)
		{
			return;
		}
		int edgeIndex2 = 0;
		FP fP3 = FindMaxSeparation(out edgeIndex2, polyB, ref transformB, polyA, ref transformA);
		if (fP3 > fP)
		{
			return;
		}
		FP fP4 = 0.98f;
		FP fP5 = 0.001f;
		PolygonShape polygonShape;
		PolygonShape poly;
		Transform xf;
		Transform xf2;
		int num;
		bool flag;
		if (fP3 > fP4 * fP2 + fP5)
		{
			polygonShape = polyB;
			poly = polyA;
			xf = transformB;
			xf2 = transformA;
			num = edgeIndex2;
			manifold.Type = ManifoldType.FaceB;
			flag = true;
		}
		else
		{
			polygonShape = polyA;
			poly = polyB;
			xf = transformA;
			xf2 = transformB;
			num = edgeIndex;
			manifold.Type = ManifoldType.FaceA;
			flag = false;
		}
		FindIncidentEdge(out var c, polygonShape, ref xf, num, poly, ref xf2);
		int count = polygonShape.Vertices.Count;
		int num2 = num;
		int num3 = ((num + 1 < count) ? (num + 1) : 0);
		TSVector2 tSVector = polygonShape.Vertices[num2];
		TSVector2 tSVector2 = polygonShape.Vertices[num3];
		TSVector2 v = tSVector2 - tSVector;
		v.Normalize();
		TSVector2 localNormal = new TSVector2(v.y, -v.x);
		TSVector2 localPoint = 0.5f * (tSVector + tSVector2);
		TSVector2 tSVector3 = MathUtils.Mul(xf.q, v);
		FP y = tSVector3.y;
		FP fP6 = -tSVector3.x;
		tSVector = MathUtils.Mul(ref xf, tSVector);
		tSVector2 = MathUtils.Mul(ref xf, tSVector2);
		FP fP7 = y * tSVector.x + fP6 * tSVector.y;
		FP offset = -(tSVector3.x * tSVector.x + tSVector3.y * tSVector.y) + fP;
		FP offset2 = tSVector3.x * tSVector2.x + tSVector3.y * tSVector2.y + fP;
		int num4 = ClipSegmentToLine(out var vOut, ref c, -tSVector3, offset, num2);
		if (num4 < 2)
		{
			return;
		}
		num4 = ClipSegmentToLine(out var vOut2, ref vOut, tSVector3, offset2, num3);
		if (num4 < 2)
		{
			return;
		}
		manifold.LocalNormal = localNormal;
		manifold.LocalPoint = localPoint;
		int num5 = 0;
		for (int i = 0; i < 2; i++)
		{
			TSVector2 v2 = vOut2[i].V;
			FP fP8 = y * v2.x + fP6 * v2.y - fP7;
			if (fP8 <= fP)
			{
				ManifoldPoint value = manifold.Points[num5];
				value.LocalPoint = MathUtils.MulT(ref xf2, vOut2[i].V);
				value.Id = vOut2[i].ID;
				if (flag)
				{
					ContactFeature features = value.Id.Features;
					value.Id.Features.IndexA = features.IndexB;
					value.Id.Features.IndexB = features.IndexA;
					value.Id.Features.TypeA = features.TypeB;
					value.Id.Features.TypeB = features.TypeA;
				}
				manifold.Points[num5] = value;
				num5++;
			}
		}
		manifold.PointCount = num5;
	}

	public static void CollideEdgeAndCircle(ref Manifold manifold, EdgeShape edgeA, ref Transform transformA, CircleShape circleB, ref Transform transformB)
	{
		manifold.PointCount = 0;
		TSVector2 tSVector = MathUtils.MulT(ref transformA, MathUtils.Mul(ref transformB, ref circleB._position));
		TSVector2 vertex = edgeA.Vertex1;
		TSVector2 vertex2 = edgeA.Vertex2;
		TSVector2 value = vertex2 - vertex;
		FP fP = TSVector2.Dot(value, vertex2 - tSVector);
		FP fP2 = TSVector2.Dot(value, tSVector - vertex);
		FP fP3 = edgeA.Radius + circleB.Radius;
		ContactFeature features = default;
		features.IndexB = 0;
		features.TypeB = 0;
		TSVector2 tSVector2;
		TSVector2 value2;
		if (fP2 <= 0f)
		{
			tSVector2 = vertex;
			value2 = tSVector - tSVector2;
			TSVector2.Dot(ref value2, ref value2, out var result);
			if (result > fP3 * fP3)
			{
				return;
			}
			if (edgeA.HasVertex0)
			{
				TSVector2 vertex3 = edgeA.Vertex0;
				TSVector2 tSVector3 = vertex;
				TSVector2 value3 = tSVector3 - vertex3;
				FP fP4 = TSVector2.Dot(value3, tSVector3 - tSVector);
				if (fP4 > 0f)
				{
					return;
				}
			}
			features.IndexA = 0;
			features.TypeA = 0;
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.Circles;
			manifold.LocalNormal = TSVector2.zero;
			manifold.LocalPoint = tSVector2;
			ManifoldPoint value4 = new ManifoldPoint
			{
				Id = 
				{
					Key = 0u,
					Features = features
				},
				LocalPoint = circleB.Position
			};
			manifold.Points[0] = value4;
			return;
		}
		if (fP <= 0f)
		{
			tSVector2 = vertex2;
			value2 = tSVector - tSVector2;
			TSVector2.Dot(ref value2, ref value2, out var result2);
			if (result2 > fP3 * fP3)
			{
				return;
			}
			if (edgeA.HasVertex3)
			{
				TSVector2 vertex4 = edgeA.Vertex3;
				TSVector2 tSVector4 = vertex2;
				TSVector2 value5 = vertex4 - tSVector4;
				FP fP5 = TSVector2.Dot(value5, tSVector - tSVector4);
				if (fP5 > 0f)
				{
					return;
				}
			}
			features.IndexA = 1;
			features.TypeA = 0;
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.Circles;
			manifold.LocalNormal = TSVector2.zero;
			manifold.LocalPoint = tSVector2;
			ManifoldPoint value6 = new ManifoldPoint
			{
				Id = 
				{
					Key = 0u,
					Features = features
				},
				LocalPoint = circleB.Position
			};
			manifold.Points[0] = value6;
			return;
		}
		TSVector2.Dot(ref value, ref value, out var result3);
		tSVector2 = 1f / result3 * (fP * vertex + fP2 * vertex2);
		value2 = tSVector - tSVector2;
		TSVector2.Dot(ref value2, ref value2, out var result4);
		if (!(result4 > fP3 * fP3))
		{
			TSVector2 tSVector5 = new TSVector2(-value.y, value.x);
			if (TSVector2.Dot(tSVector5, tSVector - vertex) < 0f)
			{
				tSVector5 = new TSVector2(-tSVector5.x, -tSVector5.y);
			}
			tSVector5.Normalize();
			features.IndexA = 0;
			features.TypeA = 1;
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.FaceA;
			manifold.LocalNormal = tSVector5;
			manifold.LocalPoint = vertex;
			ManifoldPoint value7 = new ManifoldPoint
			{
				Id = 
				{
					Key = 0u,
					Features = features
				},
				LocalPoint = circleB.Position
			};
			manifold.Points[0] = value7;
		}
	}

	public static void CollideEdgeAndPolygon(ref Manifold manifold, EdgeShape edgeA, ref Transform xfA, PolygonShape polygonB, ref Transform xfB)
	{
		EPCollider ePCollider = new EPCollider();
		ePCollider.Collide(ref manifold, edgeA, ref xfA, polygonB, ref xfB);
	}

	private static int ClipSegmentToLine(out FixedArray2<ClipVertex> vOut, ref FixedArray2<ClipVertex> vIn, TSVector2 normal, FP offset, int vertexIndexA)
	{
		vOut = default;
		ClipVertex value = vIn[0];
		ClipVertex value2 = vIn[1];
		int num = 0;
		FP fP = normal.x * value.V.x + normal.y * value.V.y - offset;
		FP fP2 = normal.x * value2.V.x + normal.y * value2.V.y - offset;
		if (fP <= 0f)
		{
			vOut[num++] = value;
		}
		if (fP2 <= 0f)
		{
			vOut[num++] = value2;
		}
		if (fP * fP2 < 0f)
		{
			FP fP3 = fP / (fP - fP2);
			ClipVertex value3 = vOut[num];
			value3.V.x = value.V.x + fP3 * (value2.V.x - value.V.x);
			value3.V.y = value.V.y + fP3 * (value2.V.y - value.V.y);
			value3.ID.Features.IndexA = (byte)vertexIndexA;
			value3.ID.Features.IndexB = value.ID.Features.IndexB;
			value3.ID.Features.TypeA = 0;
			value3.ID.Features.TypeB = 1;
			vOut[num] = value3;
			num++;
		}
		return num;
	}

	private static FP EdgeSeparation(PolygonShape poly1, ref Transform xf1, int edge1, PolygonShape poly2, ref Transform xf2)
	{
		List<TSVector2> vertices = poly1.Vertices;
		List<TSVector2> normals = poly1.Normals;
		int count = poly2.Vertices.Count;
		List<TSVector2> vertices2 = poly2.Vertices;
		TSVector2 tSVector = MathUtils.Mul(xf1.q, normals[edge1]);
		TSVector2 value = MathUtils.MulT(xf2.q, tSVector);
		int index = 0;
		FP fP = Settings.MaxFP;
		for (int i = 0; i < count; i++)
		{
			FP fP2 = TSVector2.Dot(vertices2[i], value);
			if (fP2 < fP)
			{
				fP = fP2;
				index = i;
			}
		}
		TSVector2 tSVector2 = MathUtils.Mul(ref xf1, vertices[edge1]);
		TSVector2 tSVector3 = MathUtils.Mul(ref xf2, vertices2[index]);
		return TSVector2.Dot(tSVector3 - tSVector2, tSVector);
	}

	private static FP FindMaxSeparation(out int edgeIndex, PolygonShape poly1, ref Transform xf1, PolygonShape poly2, ref Transform xf2)
	{
		int count = poly1.Vertices.Count;
		List<TSVector2> normals = poly1.Normals;
		TSVector2 v = MathUtils.Mul(ref xf2, poly2.MassData.Centroid) - MathUtils.Mul(ref xf1, poly1.MassData.Centroid);
		TSVector2 value = MathUtils.MulT(xf1.q, v);
		int num = 0;
		FP fP = -Settings.MaxFP;
		for (int i = 0; i < count; i++)
		{
			FP fP2 = TSVector2.Dot(normals[i], value);
			if (fP2 > fP)
			{
				fP = fP2;
				num = i;
			}
		}
		FP fP3 = EdgeSeparation(poly1, ref xf1, num, poly2, ref xf2);
		int num2 = ((num - 1 < 0) ? (count - 1) : (num - 1));
		FP fP4 = EdgeSeparation(poly1, ref xf1, num2, poly2, ref xf2);
		int num3 = ((num + 1 < count) ? (num + 1) : 0);
		FP fP5 = EdgeSeparation(poly1, ref xf1, num3, poly2, ref xf2);
		int num4;
		int num5;
		FP fP6;
		if (fP4 > fP3 && fP4 > fP5)
		{
			num4 = -1;
			num5 = num2;
			fP6 = fP4;
		}
		else
		{
			if (!(fP5 > fP3))
			{
				edgeIndex = num;
				return fP3;
			}
			num4 = 1;
			num5 = num3;
			fP6 = fP5;
		}
		while (true)
		{
			if (num4 != -1)
			{
				num = ((num5 + 1 < count) ? (num5 + 1) : 0);
			}
			else
			{
				num = ((num5 - 1 < 0) ? (count - 1) : (num5 - 1));
			}
			fP3 = EdgeSeparation(poly1, ref xf1, num, poly2, ref xf2);
			if (fP3 > fP6)
			{
				num5 = num;
				fP6 = fP3;
				continue;
			}
			break;
		}
		edgeIndex = num5;
		return fP6;
	}

	private static void FindIncidentEdge(out FixedArray2<ClipVertex> c, PolygonShape poly1, ref Transform xf1, int edge1, PolygonShape poly2, ref Transform xf2)
	{
		c = default;
		Vertices normals = poly1.Normals;
		int count = poly2.Vertices.Count;
		Vertices vertices = poly2.Vertices;
		Vertices normals2 = poly2.Normals;
		TSVector2 value = MathUtils.MulT(xf2.q, MathUtils.Mul(xf1.q, normals[edge1]));
		int num = 0;
		FP fP = Settings.MaxFP;
		for (int i = 0; i < count; i++)
		{
			FP fP2 = TSVector2.Dot(value, normals2[i]);
			if (fP2 < fP)
			{
				fP = fP2;
				num = i;
			}
		}
		int num2 = num;
		int num3 = ((num2 + 1 < count) ? (num2 + 1) : 0);
		ClipVertex value2 = c[0];
		value2.V = MathUtils.Mul(ref xf2, vertices[num2]);
		value2.ID.Features.IndexA = (byte)edge1;
		value2.ID.Features.IndexB = (byte)num2;
		value2.ID.Features.TypeA = 1;
		value2.ID.Features.TypeB = 0;
		c[0] = value2;
		ClipVertex value3 = c[1];
		value3.V = MathUtils.Mul(ref xf2, vertices[num3]);
		value3.ID.Features.IndexA = (byte)edge1;
		value3.ID.Features.IndexB = (byte)num3;
		value3.ID.Features.TypeA = 1;
		value3.ID.Features.TypeB = 0;
		c[1] = value3;
	}
}
