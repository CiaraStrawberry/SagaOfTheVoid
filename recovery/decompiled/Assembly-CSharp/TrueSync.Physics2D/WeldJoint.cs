namespace TrueSync.Physics2D;

public class WeldJoint : Joint2D
{
	private TSVector _impulse;

	private FP _gamma;

	private FP _bias;

	private int _indexA;

	private int _indexB;

	private TSVector2 _rA;

	private TSVector2 _rB;

	private TSVector2 _localCenterA;

	private TSVector2 _localCenterB;

	private FP _invMassA;

	private FP _invMassB;

	private FP _invIA;

	private FP _invIB;

	private Mat33 _mass;

	public TSVector2 LocalAnchorA { get; set; }

	public TSVector2 LocalAnchorB { get; set; }

	public override TSVector2 WorldAnchorA
	{
		get
		{
			return BodyA.GetWorldPoint(LocalAnchorA);
		}
		set
		{
			LocalAnchorA = BodyA.GetLocalPoint(value);
		}
	}

	public override TSVector2 WorldAnchorB
	{
		get
		{
			return BodyB.GetWorldPoint(LocalAnchorB);
		}
		set
		{
			LocalAnchorB = BodyB.GetLocalPoint(value);
		}
	}

	public FP ReferenceAngle { get; set; }

	public FP FrequencyHz { get; set; }

	public FP DampingRatio { get; set; }

	internal WeldJoint()
	{
		JointType = JointType.Weld;
	}

	public WeldJoint(Body bodyA, Body bodyB, TSVector2 anchorA, TSVector2 anchorB, bool useWorldCoordinates = false)
		: base(bodyA, bodyB)
	{
		JointType = JointType.Weld;
		if (useWorldCoordinates)
		{
			LocalAnchorA = bodyA.GetLocalPoint(anchorA);
			LocalAnchorB = bodyB.GetLocalPoint(anchorB);
		}
		else
		{
			LocalAnchorA = anchorA;
			LocalAnchorB = anchorB;
		}
		ReferenceAngle = BodyB.Rotation - BodyA.Rotation;
	}

	public override TSVector2 GetReactionForce(FP invDt)
	{
		return invDt * new TSVector2(_impulse.x, _impulse.y);
	}

	public override FP GetReactionTorque(FP invDt)
	{
		return invDt * _impulse.z;
	}

	internal override void InitVelocityConstraints(ref SolverData data)
	{
		_indexA = BodyA.IslandIndex;
		_indexB = BodyB.IslandIndex;
		_localCenterA = BodyA._sweep.LocalCenter;
		_localCenterB = BodyB._sweep.LocalCenter;
		_invMassA = BodyA._invMass;
		_invMassB = BodyB._invMass;
		_invIA = BodyA._invI;
		_invIB = BodyB._invI;
		FP a = data.positions[_indexA].a;
		TSVector2 v = data.velocities[_indexA].v;
		FP w = data.velocities[_indexA].w;
		FP a2 = data.positions[_indexB].a;
		TSVector2 v2 = data.velocities[_indexB].v;
		FP w2 = data.velocities[_indexB].w;
		Rot q = new Rot(a);
		Rot q2 = new Rot(a2);
		_rA = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		_rB = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
		FP invMassA = _invMassA;
		FP invMassB = _invMassB;
		FP invIA = _invIA;
		FP invIB = _invIB;
		Mat33 mat = default;
		mat.ex.x = invMassA + invMassB + _rA.y * _rA.y * invIA + _rB.y * _rB.y * invIB;
		mat.ey.x = -_rA.y * _rA.x * invIA - _rB.y * _rB.x * invIB;
		mat.ez.x = -_rA.y * invIA - _rB.y * invIB;
		mat.ex.y = mat.ey.x;
		mat.ey.y = invMassA + invMassB + _rA.x * _rA.x * invIA + _rB.x * _rB.x * invIB;
		mat.ez.y = _rA.x * invIA + _rB.x * invIB;
		mat.ex.z = mat.ez.x;
		mat.ey.z = mat.ez.y;
		mat.ez.z = invIA + invIB;
		if (FrequencyHz > 0f)
		{
			mat.GetInverse22(ref _mass);
			FP fP = invIA + invIB;
			FP fP2 = ((!(fP > 0f)) ? ((FP)0f) : (1f / fP));
			FP fP3 = a2 - a - ReferenceAngle;
			FP fP4 = 2f * Settings.Pi * FrequencyHz;
			FP fP5 = 2f * fP2 * DampingRatio * fP4;
			FP fP6 = fP2 * fP4 * fP4;
			FP dt = data.step.dt;
			_gamma = dt * (fP5 + dt * fP6);
			_gamma = ((!(_gamma != 0f)) ? ((FP)0f) : (1f / _gamma));
			_bias = fP3 * dt * fP6 * _gamma;
			fP += _gamma;
			_mass.ez.z = ((!(fP != 0f)) ? ((FP)0f) : (1f / fP));
		}
		else
		{
			mat.GetSymInverse33(ref _mass);
			_gamma = 0f;
			_bias = 0f;
		}
		_impulse *= data.step.dtRatio;
		TSVector2 tSVector = new TSVector2(_impulse.x, _impulse.y);
		v -= invMassA * tSVector;
		w -= invIA * (MathUtils.Cross(_rA, tSVector) + _impulse.z);
		v2 += invMassB * tSVector;
		w2 += invIB * (MathUtils.Cross(_rB, tSVector) + _impulse.z);
		data.velocities[_indexA].v = v;
		data.velocities[_indexA].w = w;
		data.velocities[_indexB].v = v2;
		data.velocities[_indexB].w = w2;
	}

	internal override void SolveVelocityConstraints(ref SolverData data)
	{
		TSVector2 v = data.velocities[_indexA].v;
		FP w = data.velocities[_indexA].w;
		TSVector2 v2 = data.velocities[_indexB].v;
		FP w2 = data.velocities[_indexB].w;
		FP invMassA = _invMassA;
		FP invMassB = _invMassB;
		FP invIA = _invIA;
		FP invIB = _invIB;
		if (FrequencyHz > 0f)
		{
			FP fP = w2 - w;
			FP fP2 = -_mass.ez.z * (fP + _bias + _gamma * _impulse.z);
			_impulse.z += fP2;
			w -= invIA * fP2;
			w2 += invIB * fP2;
			TSVector2 v3 = v2 + MathUtils.Cross(w2, _rB) - v - MathUtils.Cross(w, _rA);
			TSVector2 tSVector = -MathUtils.Mul22(_mass, v3);
			_impulse.x += tSVector.x;
			_impulse.y += tSVector.y;
			TSVector2 tSVector2 = tSVector;
			v -= invMassA * tSVector2;
			w -= invIA * MathUtils.Cross(_rA, tSVector2);
			v2 += invMassB * tSVector2;
			w2 += invIB * MathUtils.Cross(_rB, tSVector2);
		}
		else
		{
			TSVector2 tSVector3 = v2 + MathUtils.Cross(w2, _rB) - v - MathUtils.Cross(w, _rA);
			FP z = w2 - w;
			TSVector tSVector4 = MathUtils.Mul(v: new TSVector(tSVector3.x, tSVector3.y, z), A: _mass) * -1;
			_impulse += tSVector4;
			TSVector2 tSVector5 = new TSVector2(tSVector4.x, tSVector4.y);
			v -= invMassA * tSVector5;
			w -= invIA * (MathUtils.Cross(_rA, tSVector5) + tSVector4.z);
			v2 += invMassB * tSVector5;
			w2 += invIB * (MathUtils.Cross(_rB, tSVector5) + tSVector4.z);
		}
		data.velocities[_indexA].v = v;
		data.velocities[_indexA].w = w;
		data.velocities[_indexB].v = v2;
		data.velocities[_indexB].w = w2;
	}

	internal override bool SolvePositionConstraints(ref SolverData data)
	{
		TSVector2 c = data.positions[_indexA].c;
		FP a = data.positions[_indexA].a;
		TSVector2 c2 = data.positions[_indexB].c;
		FP a2 = data.positions[_indexB].a;
		Rot q = new Rot(a);
		Rot q2 = new Rot(a2);
		FP invMassA = _invMassA;
		FP invMassB = _invMassB;
		FP invIA = _invIA;
		FP invIB = _invIB;
		TSVector2 tSVector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		TSVector2 tSVector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
		Mat33 mat = default;
		mat.ex.x = invMassA + invMassB + tSVector.y * tSVector.y * invIA + tSVector2.y * tSVector2.y * invIB;
		mat.ey.x = -tSVector.y * tSVector.x * invIA - tSVector2.y * tSVector2.x * invIB;
		mat.ez.x = -tSVector.y * invIA - tSVector2.y * invIB;
		mat.ex.y = mat.ey.x;
		mat.ey.y = invMassA + invMassB + tSVector.x * tSVector.x * invIA + tSVector2.x * tSVector2.x * invIB;
		mat.ez.y = tSVector.x * invIA + tSVector2.x * invIB;
		mat.ex.z = mat.ez.x;
		mat.ey.z = mat.ez.y;
		mat.ez.z = invIA + invIB;
		FP magnitude;
		FP fP;
		if (FrequencyHz > 0f)
		{
			TSVector2 b = c2 + tSVector2 - c - tSVector;
			magnitude = b.magnitude;
			fP = 0f;
			TSVector2 tSVector3 = -mat.Solve22(b);
			c -= invMassA * tSVector3;
			a -= invIA * MathUtils.Cross(tSVector, tSVector3);
			c2 += invMassB * tSVector3;
			a2 += invIB * MathUtils.Cross(tSVector2, tSVector3);
		}
		else
		{
			TSVector2 tSVector4 = c2 + tSVector2 - c - tSVector;
			FP fP2 = a2 - a - ReferenceAngle;
			magnitude = tSVector4.magnitude;
			fP = FP.Abs(fP2);
			TSVector b2 = new TSVector(tSVector4.x, tSVector4.y, fP2);
			TSVector tSVector5 = mat.Solve33(b2) * -1;
			TSVector2 tSVector6 = new TSVector2(tSVector5.x, tSVector5.y);
			c -= invMassA * tSVector6;
			a -= invIA * (MathUtils.Cross(tSVector, tSVector6) + tSVector5.z);
			c2 += invMassB * tSVector6;
			a2 += invIB * (MathUtils.Cross(tSVector2, tSVector6) + tSVector5.z);
		}
		data.positions[_indexA].c = c;
		data.positions[_indexA].a = a;
		data.positions[_indexB].c = c2;
		data.positions[_indexB].a = a2;
		return magnitude <= Settings.LinearSlop && fP <= Settings.AngularSlop;
	}
}
