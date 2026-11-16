using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrueSync.Physics3D;

public class SoftBody : IBroadphaseEntity, IComparable
{
	[Flags]
	public enum SpringType
	{
		EdgeSpring = 2,
		ShearSpring = 4,
		BendSpring = 8
	}

	public class Spring : Constraint
	{
		public enum DistanceBehavior
		{
			LimitDistance,
			LimitMaximumDistance,
			LimitMinimumDistance
		}

		private FP biasFactor = FP.EN1;

		private FP softness = FP.EN2;

		private FP distance;

		private DistanceBehavior behavior;

		private FP effectiveMass = FP.Zero;

		private FP accumulatedImpulse = FP.Zero;

		private FP bias;

		private FP softnessOverDt;

		private TSVector[] jacobian = new TSVector[2];

		private bool skipConstraint;

		public SpringType SpringType { get; set; }

		public FP AppliedImpulse => accumulatedImpulse;

		public FP Distance
		{
			get
			{
				return distance;
			}
			set
			{
				distance = value;
			}
		}

		public DistanceBehavior Behavior
		{
			get
			{
				return behavior;
			}
			set
			{
				behavior = value;
			}
		}

		public FP Softness
		{
			get
			{
				return softness;
			}
			set
			{
				softness = value;
			}
		}

		public FP BiasFactor
		{
			get
			{
				return biasFactor;
			}
			set
			{
				biasFactor = value;
			}
		}

		public Spring(RigidBody body1, RigidBody body2)
			: base(body1, body2)
		{
			distance = (body1.position - body2.position).magnitude;
		}

		public override void PrepareForIteration(FP timestep)
		{
			TSVector.Subtract(ref body2.position, ref body1.position, out var result);
			FP fP = result.magnitude - distance;
			if (behavior == DistanceBehavior.LimitMaximumDistance && fP <= FP.Zero)
			{
				skipConstraint = true;
				return;
			}
			if (behavior == DistanceBehavior.LimitMinimumDistance && fP >= FP.Zero)
			{
				skipConstraint = true;
				return;
			}
			skipConstraint = false;
			TSVector tSVector = result;
			if (tSVector.sqrMagnitude != FP.Zero)
			{
				tSVector.Normalize();
			}
			ref TSVector reference = ref jacobian[0];
			reference = -FP.One * tSVector;
			ref TSVector reference2 = ref jacobian[1];
			reference2 = FP.One * tSVector;
			effectiveMass = body1.inverseMass + body2.inverseMass;
			softnessOverDt = softness / timestep;
			effectiveMass += softnessOverDt;
			effectiveMass = FP.One / effectiveMass;
			bias = fP * biasFactor * (FP.One / timestep);
			if (!body1.isStatic)
			{
				body1.linearVelocity += body1.inverseMass * accumulatedImpulse * jacobian[0];
			}
			if (!body2.isStatic)
			{
				body2.linearVelocity += body2.inverseMass * accumulatedImpulse * jacobian[1];
			}
		}

		public override void Iterate()
		{
			if (!skipConstraint)
			{
				FP fP = TSVector.Dot(ref body1.linearVelocity, ref jacobian[0]);
				fP += TSVector.Dot(ref body2.linearVelocity, ref jacobian[1]);
				FP fP2 = accumulatedImpulse * softnessOverDt;
				FP fP3 = -effectiveMass * (fP + bias + fP2);
				if (behavior == DistanceBehavior.LimitMinimumDistance)
				{
					FP fP4 = accumulatedImpulse;
					accumulatedImpulse = TSMath.Max(accumulatedImpulse + fP3, 0);
					fP3 = accumulatedImpulse - fP4;
				}
				else if (behavior == DistanceBehavior.LimitMaximumDistance)
				{
					FP fP5 = accumulatedImpulse;
					accumulatedImpulse = TSMath.Min(accumulatedImpulse + fP3, 0);
					fP3 = accumulatedImpulse - fP5;
				}
				else
				{
					accumulatedImpulse += fP3;
				}
				TSVector result;
				if (!body1.isStatic)
				{
					TSVector.Multiply(ref jacobian[0], fP3 * body1.inverseMass, out result);
					TSVector.Add(ref result, ref body1.linearVelocity, out body1.linearVelocity);
				}
				if (!body2.isStatic)
				{
					TSVector.Multiply(ref jacobian[1], fP3 * body2.inverseMass, out result);
					TSVector.Add(ref result, ref body2.linearVelocity, out body2.linearVelocity);
				}
			}
		}

		public override void DebugDraw(IDebugDrawer drawer)
		{
			drawer.DrawLine(body1.position, body2.position);
		}
	}

	public class MassPoint : RigidBody
	{
		public SoftBody SoftBody { get; private set; }

		public MassPoint(Shape shape, SoftBody owner, BodyMaterial material)
			: base(shape, material, isParticle: true)
		{
			SoftBody = owner;
		}
	}

	public class Triangle : ISupportMappable
	{
		private SoftBody owner;

		internal TSBBox boundingBox;

		internal int dynamicTreeID;

		internal TriangleVertexIndices indices;

		public SoftBody Owner => owner;

		public TSBBox BoundingBox => boundingBox;

		public int DynamicTreeID => dynamicTreeID;

		public TriangleVertexIndices Indices => indices;

		public MassPoint VertexBody1 => owner.points[indices.I0];

		public MassPoint VertexBody2 => owner.points[indices.I1];

		public MassPoint VertexBody3 => owner.points[indices.I2];

		public Triangle(SoftBody owner)
		{
			this.owner = owner;
		}

		public void GetNormal(out TSVector normal)
		{
			TSVector.Subtract(ref owner.points[indices.I1].position, ref owner.points[indices.I0].position, out var result);
			TSVector.Subtract(ref owner.points[indices.I2].position, ref owner.points[indices.I0].position, out normal);
			TSVector.Cross(ref result, ref normal, out normal);
		}

		public void UpdateBoundingBox()
		{
			boundingBox = TSBBox.SmallBox;
			boundingBox.AddPoint(ref owner.points[indices.I0].position);
			boundingBox.AddPoint(ref owner.points[indices.I1].position);
			boundingBox.AddPoint(ref owner.points[indices.I2].position);
			boundingBox.min -= new TSVector(owner.triangleExpansion);
			boundingBox.max += new TSVector(owner.triangleExpansion);
		}

		public FP CalculateArea()
		{
			return ((owner.points[indices.I1].position - owner.points[indices.I0].position) % (owner.points[indices.I2].position - owner.points[indices.I0].position)).magnitude;
		}

		public void SupportMapping(ref TSVector direction, out TSVector result)
		{
			FP fP = TSVector.Dot(ref owner.points[indices.I0].position, ref direction);
			FP fP2 = TSVector.Dot(ref owner.points[indices.I1].position, ref direction);
			TSVector position = owner.points[indices.I0].position;
			if (fP2 > fP)
			{
				fP = fP2;
				position = owner.points[indices.I1].position;
			}
			fP2 = TSVector.Dot(ref owner.points[indices.I2].position, ref direction);
			if (fP2 > fP)
			{
				fP = fP2;
				position = owner.points[indices.I2].position;
			}
			TSVector.Normalize(ref direction, out var result2);
			result2 *= owner.triangleExpansion;
			result = position + result2;
		}

		public void SupportCenter(out TSVector center)
		{
			center = owner.points[indices.I0].position;
			TSVector.Add(ref center, ref owner.points[indices.I1].position, out center);
			TSVector.Add(ref center, ref owner.points[indices.I2].position, out center);
			TSVector.Multiply(ref center, FP.One / (3 * FP.One), out center);
		}
	}

	private struct Edge(int index1, int index2)
	{
		public int Index1 = index1;

		public int Index2 = index2;

		public override int GetHashCode()
		{
			return Index1.GetHashCode() + Index2.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			Edge edge = (Edge)obj;
			return (edge.Index1 == Index1 && edge.Index2 == Index2) || (edge.Index1 == Index2 && edge.Index2 == Index1);
		}
	}

	private SphereShape sphere = new SphereShape(FP.EN1);

	protected List<Spring> springs = new List<Spring>();

	protected List<MassPoint> points = new List<MassPoint>();

	protected List<Triangle> triangles = new List<Triangle>();

	protected FP triangleExpansion = FP.EN1;

	private bool selfCollision;

	private FP volume = FP.One;

	private FP mass = FP.One;

	internal DynamicTree<Triangle> dynamicTree = new DynamicTree<Triangle>();

	private BodyMaterial material = new BodyMaterial();

	private TSBBox box = default;

	private bool active = true;

	private FP pressure = FP.Zero;

	private List<int> queryList = new List<int>();

	public ReadOnlyCollection<Spring> EdgeSprings { get; private set; }

	public ReadOnlyCollection<MassPoint> VertexBodies { get; private set; }

	public ReadOnlyCollection<Triangle> Triangles { get; private set; }

	public bool SelfCollision
	{
		get
		{
			return selfCollision;
		}
		set
		{
			selfCollision = value;
		}
	}

	public FP TriangleExpansion
	{
		get
		{
			return triangleExpansion;
		}
		set
		{
			triangleExpansion = value;
		}
	}

	public FP VertexExpansion
	{
		get
		{
			return sphere.Radius;
		}
		set
		{
			sphere.Radius = value;
		}
	}

	public DynamicTree<Triangle> DynamicTree => dynamicTree;

	public BodyMaterial Material => material;

	public FP Pressure
	{
		get
		{
			return pressure;
		}
		set
		{
			pressure = value;
		}
	}

	public FP Mass
	{
		get
		{
			return mass;
		}
		set
		{
			for (int i = 0; i < points.Count; i++)
			{
				points[i].Mass = value / points.Count;
			}
		}
	}

	public FP Volume => volume;

	public TSBBox BoundingBox => box;

	public int BroadphaseTag { get; set; }

	public object Tag { get; set; }

	public bool IsStaticOrInactive => !active;

	public bool IsStaticNonKinematic => !active;

	public SoftBody()
	{
	}

	public SoftBody(int sizeX, int sizeY, FP scale)
	{
		List<TriangleVertexIndices> list = new List<TriangleVertexIndices>();
		List<TSVector> list2 = new List<TSVector>();
		for (int i = 0; i < sizeY; i++)
		{
			for (int j = 0; j < sizeX; j++)
			{
				list2.Add(new TSVector(i, 0, j) * scale);
			}
		}
		for (int k = 0; k < sizeX - 1; k++)
		{
			for (int l = 0; l < sizeY - 1; l++)
			{
				TriangleVertexIndices item = new TriangleVertexIndices
				{
					I0 = l * sizeX + k,
					I1 = l * sizeX + k + 1,
					I2 = (l + 1) * sizeX + k + 1
				};
				list.Add(item);
				item.I0 = l * sizeX + k;
				item.I1 = (l + 1) * sizeX + k + 1;
				item.I2 = (l + 1) * sizeX + k;
				list.Add(item);
			}
		}
		EdgeSprings = springs.AsReadOnly();
		VertexBodies = points.AsReadOnly();
		Triangles = triangles.AsReadOnly();
		AddPointsAndSprings(list, list2);
		for (int m = 0; m < sizeX - 1; m++)
		{
			for (int n = 0; n < sizeY - 1; n++)
			{
				Spring item2 = new Spring(points[n * sizeX + m + 1], points[(n + 1) * sizeX + m])
				{
					Softness = FP.EN2,
					BiasFactor = FP.EN1
				};
				springs.Add(item2);
			}
		}
		foreach (Spring spring3 in springs)
		{
			TSVector tSVector = spring3.body1.position - spring3.body2.position;
			if (tSVector.z != FP.Zero && tSVector.x != FP.Zero)
			{
				spring3.SpringType = SpringType.ShearSpring;
			}
			else
			{
				spring3.SpringType = SpringType.EdgeSpring;
			}
		}
		for (int num = 0; num < sizeX - 2; num++)
		{
			for (int num2 = 0; num2 < sizeY - 2; num2++)
			{
				Spring spring = new Spring(points[num2 * sizeX + num], points[num2 * sizeX + num + 2])
				{
					Softness = FP.EN2,
					BiasFactor = FP.EN1
				};
				Spring spring2 = new Spring(points[num2 * sizeX + num], points[(num2 + 2) * sizeX + num])
				{
					Softness = FP.EN2,
					BiasFactor = FP.EN1
				};
				spring.SpringType = SpringType.BendSpring;
				spring2.SpringType = SpringType.BendSpring;
				springs.Add(spring);
				springs.Add(spring2);
			}
		}
	}

	public SoftBody(List<TriangleVertexIndices> indices, List<TSVector> vertices)
	{
		EdgeSprings = springs.AsReadOnly();
		VertexBodies = points.AsReadOnly();
		AddPointsAndSprings(indices, vertices);
		Triangles = triangles.AsReadOnly();
	}

	public int CompareTo(object otherObj)
	{
		SoftBody softBody = (SoftBody)otherObj;
		if (softBody.volume < volume)
		{
			return -1;
		}
		if (softBody.volume > volume)
		{
			return 1;
		}
		return 0;
	}

	private void AddPressureForces(FP timeStep)
	{
		if (pressure == FP.Zero || volume == FP.Zero)
		{
			return;
		}
		FP fP = FP.One / volume;
		foreach (Triangle triangle in triangles)
		{
			TSVector position = points[triangle.indices.I0].position;
			TSVector position2 = points[triangle.indices.I1].position;
			TSVector position3 = points[triangle.indices.I2].position;
			TSVector tSVector = (position3 - position) % (position2 - position);
			points[triangle.indices.I0].AddForce(fP * tSVector * pressure);
			points[triangle.indices.I1].AddForce(fP * tSVector * pressure);
			points[triangle.indices.I2].AddForce(fP * tSVector * pressure);
		}
	}

	public void Translate(TSVector position)
	{
		foreach (MassPoint point in points)
		{
			point.Position += position;
		}
		Update(FP.Epsilon);
	}

	public void AddForce(TSVector force)
	{
		throw new NotImplementedException();
	}

	public void Rotate(TSMatrix orientation, TSVector center)
	{
		for (int i = 0; i < points.Count; i++)
		{
			points[i].position = TSVector.Transform(points[i].position - center, orientation);
		}
	}

	public TSVector CalculateCenter()
	{
		throw new NotImplementedException();
	}

	private HashSet<Edge> GetEdges(List<TriangleVertexIndices> indices)
	{
		HashSet<Edge> hashSet = new HashSet<Edge>();
		for (int i = 0; i < indices.Count; i++)
		{
			Edge item = new Edge(indices[i].I0, indices[i].I1);
			if (!hashSet.Contains(item))
			{
				hashSet.Add(item);
			}
			item = new Edge(indices[i].I1, indices[i].I2);
			if (!hashSet.Contains(item))
			{
				hashSet.Add(item);
			}
			item = new Edge(indices[i].I2, indices[i].I0);
			if (!hashSet.Contains(item))
			{
				hashSet.Add(item);
			}
		}
		return hashSet;
	}

	public virtual void DoSelfCollision(CollisionDetectedHandler collision)
	{
		if (!selfCollision)
		{
			return;
		}
		for (int i = 0; i < points.Count; i++)
		{
			queryList.Clear();
			dynamicTree.Query(queryList, ref points[i].boundingBox);
			for (int j = 0; j < queryList.Count; j++)
			{
				Triangle userData = dynamicTree.GetUserData(queryList[j]);
				if (userData.VertexBody1 != points[i] && userData.VertexBody2 != points[i] && userData.VertexBody3 != points[i] && XenoCollide.Detect(points[i].Shape, userData, ref points[i].orientation, ref TSMatrix.InternalIdentity, ref points[i].position, ref TSVector.InternalZero, out var point, out var normal, out var penetration))
				{
					int index = CollisionSystem.FindNearestTrianglePoint(this, queryList[j], ref point);
					collision(points[i], points[index], point, point, normal, penetration);
				}
			}
		}
	}

	private void AddPointsAndSprings(List<TriangleVertexIndices> indices, List<TSVector> vertices)
	{
		for (int i = 0; i < vertices.Count; i++)
		{
			MassPoint massPoint = new MassPoint(sphere, this, material);
			massPoint.Position = vertices[i];
			massPoint.Mass = FP.EN1;
			points.Add(massPoint);
		}
		for (int j = 0; j < indices.Count; j++)
		{
			TriangleVertexIndices indices2 = indices[j];
			Triangle triangle = new Triangle(this);
			triangle.indices = indices2;
			triangles.Add(triangle);
			triangle.boundingBox = TSBBox.SmallBox;
			triangle.boundingBox.AddPoint(points[triangle.indices.I0].position);
			triangle.boundingBox.AddPoint(points[triangle.indices.I1].position);
			triangle.boundingBox.AddPoint(points[triangle.indices.I2].position);
			triangle.dynamicTreeID = dynamicTree.AddProxy(ref triangle.boundingBox, triangle);
		}
		HashSet<Edge> edges = GetEdges(indices);
		int num = 0;
		foreach (Edge item in edges)
		{
			Spring spring = new Spring(points[item.Index1], points[item.Index2]);
			spring.Softness = FP.EN2;
			spring.BiasFactor = FP.EN1;
			spring.SpringType = SpringType.EdgeSpring;
			springs.Add(spring);
			num++;
		}
	}

	public void SetSpringValues(FP bias, FP softness)
	{
		SetSpringValues(SpringType.EdgeSpring | SpringType.ShearSpring | SpringType.BendSpring, bias, softness);
	}

	public void SetSpringValues(SpringType type, FP bias, FP softness)
	{
		for (int i = 0; i < springs.Count; i++)
		{
			if ((springs[i].SpringType & type) != 0)
			{
				springs[i].Softness = softness;
				springs[i].BiasFactor = bias;
			}
		}
	}

	public virtual void Update(FP timestep)
	{
		active = false;
		foreach (MassPoint point in points)
		{
			if (point.isActive && !point.isStatic)
			{
				active = true;
				break;
			}
		}
		if (!active)
		{
			return;
		}
		box = TSBBox.SmallBox;
		volume = FP.Zero;
		mass = FP.Zero;
		foreach (MassPoint point2 in points)
		{
			mass += point2.Mass;
			box.AddPoint(point2.position);
		}
		box.min -= new TSVector(TriangleExpansion);
		box.max += new TSVector(TriangleExpansion);
		foreach (Triangle triangle in triangles)
		{
			triangle.UpdateBoundingBox();
			TSVector tSVector = triangle.VertexBody1.linearVelocity + triangle.VertexBody2.linearVelocity + triangle.VertexBody3.linearVelocity;
			tSVector *= FP.One / (3 * FP.One);
			dynamicTree.MoveProxy(triangle.dynamicTreeID, ref triangle.boundingBox, tSVector * timestep);
			TSVector position = points[triangle.indices.I0].position;
			TSVector position2 = points[triangle.indices.I1].position;
			TSVector position3 = points[triangle.indices.I2].position;
			volume -= ((position2.y - position.y) * (position3.z - position.z) - (position2.z - position.z) * (position3.y - position.y)) * (position.x + position2.x + position3.x);
		}
		volume /= (FP)6;
		AddPressureForces(timestep);
	}
}
