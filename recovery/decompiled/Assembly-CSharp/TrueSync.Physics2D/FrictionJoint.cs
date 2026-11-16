namespace TrueSync.Physics2D;

public class FrictionJoint : Joint2D
{
	private TSVector2 _linearImpulse;

	private FP _angularImpulse;

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

	private FP _angularMass;

	private Mat22 _linearMass;

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

	public FP MaxForce { get; set; }

	public FP MaxTorque { get; set; }

	internal FrictionJoint()
	{
		JointType = JointType.Friction;
	}

	public FrictionJoint(Body bodyA, Body bodyB, TSVector2 anchor, bool useWorldCoordinates = false)
		: base(bodyA, bodyB)
	{
		JointType = JointType.Friction;
		if (useWorldCoordinates)
		{
			LocalAnchorA = BodyA.GetLocalPoint(anchor);
			LocalAnchorB = BodyB.GetLocalPoint(anchor);
		}
		else
		{
			LocalAnchorA = anchor;
			LocalAnchorB = anchor;
		}
	}

	public override TSVector2 GetReactionForce(FP invDt)
	{
		return invDt * _linearImpulse;
	}

	public override FP GetReactionTorque(FP invDt)
	{
		return invDt * _angularImpulse;
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
		Mat22 mat = default;
		mat.ex.x = invMassA + invMassB + invIA * _rA.y * _rA.y + invIB * _rB.y * _rB.y;
		mat.ex.y = -invIA * _rA.x * _rA.y - invIB * _rB.x * _rB.y;
		mat.ey.x = mat.ex.y;
		mat.ey.y = invMassA + invMassB + invIA * _rA.x * _rA.x + invIB * _rB.x * _rB.x;
		_linearMass = mat.Inverse;
		_angularMass = invIA + invIB;
		if (_angularMass > 0f)
		{
			_angularMass = 1f / _angularMass;
		}
		_linearImpulse *= data.step.dtRatio;
		_angularImpulse *= data.step.dtRatio;
		TSVector2 tSVector = new TSVector2(_linearImpulse.x, _linearImpulse.y);
		v -= invMassA * tSVector;
		w -= invIA * (MathUtils.Cross(_rA, tSVector) + _angularImpulse);
		v2 += invMassB * tSVector;
		w2 += invIB * (MathUtils.Cross(_rB, tSVector) + _angularImpulse);
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
		FP dt = data.step.dt;
		FP fP = w2 - w;
		FP fP2 = -_angularMass * fP;
		FP angularImpulse = _angularImpulse;
		FP fP3 = dt * MaxTorque;
		_angularImpulse = MathUtils.Clamp(_angularImpulse + fP2, -fP3, fP3);
		fP2 = _angularImpulse - angularImpulse;
		w -= invIA * fP2;
		w2 += invIB * fP2;
		TSVector2 v3 = v2 + MathUtils.Cross(w2, _rB) - v - MathUtils.Cross(w, _rA);
		TSVector2 tSVector = -MathUtils.Mul(ref _linearMass, v3);
		TSVector2 linearImpulse = _linearImpulse;
		_linearImpulse += tSVector;
		FP fP4 = dt * MaxForce;
		if (_linearImpulse.LengthSquared() > fP4 * fP4)
		{
			_linearImpulse.Normalize();
			_linearImpulse *= fP4;
		}
		tSVector = _linearImpulse - linearImpulse;
		v -= invMassA * tSVector;
		w -= invIA * MathUtils.Cross(_rA, tSVector);
		v2 += invMassB * tSVector;
		w2 += invIB * MathUtils.Cross(_rB, tSVector);
		data.velocities[_indexA].v = v;
		data.velocities[_indexA].w = w;
		data.velocities[_indexB].v = v2;
		data.velocities[_indexB].w = w2;
	}

	internal override bool SolvePositionConstraints(ref SolverData data)
	{
		return true;
	}
}
