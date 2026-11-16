namespace TrueSync.Physics2D;

public class RopeJoint : Joint2D
{
	private FP _impulse;

	private FP _length;

	private int _indexA;

	private int _indexB;

	private TSVector2 _localCenterA;

	private TSVector2 _localCenterB;

	private FP _invMassA;

	private FP _invMassB;

	private FP _invIA;

	private FP _invIB;

	private FP _mass;

	private TSVector2 _rA;

	private TSVector2 _rB;

	private TSVector2 _u;

	public TSVector2 LocalAnchorA { get; set; }

	public TSVector2 LocalAnchorB { get; set; }

	public sealed override TSVector2 WorldAnchorA
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

	public sealed override TSVector2 WorldAnchorB
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

	public FP MaxLength { get; set; }

	public LimitState State { get; private set; }

	internal RopeJoint()
	{
		JointType = JointType.Rope;
	}

	public RopeJoint(Body bodyA, Body bodyB, TSVector2 anchorA, TSVector2 anchorB, bool useWorldCoordinates = false)
		: base(bodyA, bodyB)
	{
		JointType = JointType.Rope;
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
		MaxLength = (WorldAnchorB - WorldAnchorA).magnitude;
	}

	public override TSVector2 GetReactionForce(FP invDt)
	{
		return invDt * _impulse * _u;
	}

	public override FP GetReactionTorque(FP invDt)
	{
		return 0;
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
		TSVector2 c = data.positions[_indexA].c;
		FP a = data.positions[_indexA].a;
		TSVector2 v = data.velocities[_indexA].v;
		FP w = data.velocities[_indexA].w;
		TSVector2 c2 = data.positions[_indexB].c;
		FP a2 = data.positions[_indexB].a;
		TSVector2 v2 = data.velocities[_indexB].v;
		FP w2 = data.velocities[_indexB].w;
		Rot q = new Rot(a);
		Rot q2 = new Rot(a2);
		_rA = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		_rB = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
		_u = c2 + _rB - c - _rA;
		_length = _u.magnitude;
		FP fP = _length - MaxLength;
		if (fP > 0f)
		{
			State = LimitState.AtUpper;
		}
		else
		{
			State = LimitState.Inactive;
		}
		if (_length > Settings.LinearSlop)
		{
			_u *= 1f / _length;
			FP fP2 = MathUtils.Cross(_rA, _u);
			FP fP3 = MathUtils.Cross(_rB, _u);
			FP fP4 = _invMassA + _invIA * fP2 * fP2 + _invMassB + _invIB * fP3 * fP3;
			_mass = ((!(fP4 != 0f)) ? ((FP)0f) : (1f / fP4));
			_impulse *= data.step.dtRatio;
			TSVector2 tSVector = _impulse * _u;
			v -= _invMassA * tSVector;
			w -= _invIA * MathUtils.Cross(_rA, tSVector);
			v2 += _invMassB * tSVector;
			w2 += _invIB * MathUtils.Cross(_rB, tSVector);
			data.velocities[_indexA].v = v;
			data.velocities[_indexA].w = w;
			data.velocities[_indexB].v = v2;
			data.velocities[_indexB].w = w2;
		}
		else
		{
			_u = TSVector2.zero;
			_mass = 0f;
			_impulse = 0f;
		}
	}

	internal override void SolveVelocityConstraints(ref SolverData data)
	{
		TSVector2 v = data.velocities[_indexA].v;
		FP w = data.velocities[_indexA].w;
		TSVector2 v2 = data.velocities[_indexB].v;
		FP w2 = data.velocities[_indexB].w;
		TSVector2 tSVector = v + MathUtils.Cross(w, _rA);
		TSVector2 tSVector2 = v2 + MathUtils.Cross(w2, _rB);
		FP fP = _length - MaxLength;
		FP fP2 = TSVector2.Dot(_u, tSVector2 - tSVector);
		if (fP < 0f)
		{
			fP2 += data.step.inv_dt * fP;
		}
		FP fP3 = -_mass * fP2;
		FP impulse = _impulse;
		_impulse = TSMath.Min(0f, _impulse + fP3);
		fP3 = _impulse - impulse;
		TSVector2 tSVector3 = fP3 * _u;
		v -= _invMassA * tSVector3;
		w -= _invIA * MathUtils.Cross(_rA, tSVector3);
		v2 += _invMassB * tSVector3;
		w2 += _invIB * MathUtils.Cross(_rB, tSVector3);
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
		TSVector2 tSVector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		TSVector2 tSVector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
		TSVector2 tSVector3 = c2 + tSVector2 - c - tSVector;
		FP magnitude = tSVector3.magnitude;
		tSVector3.Normalize();
		FP a3 = magnitude - MaxLength;
		a3 = MathUtils.Clamp(a3, 0f, Settings.MaxLinearCorrection);
		FP fP = -_mass * a3;
		TSVector2 tSVector4 = fP * tSVector3;
		c -= _invMassA * tSVector4;
		a -= _invIA * MathUtils.Cross(tSVector, tSVector4);
		c2 += _invMassB * tSVector4;
		a2 += _invIB * MathUtils.Cross(tSVector2, tSVector4);
		data.positions[_indexA].c = c;
		data.positions[_indexA].a = a;
		data.positions[_indexB].c = c2;
		data.positions[_indexB].a = a2;
		return magnitude - MaxLength < Settings.LinearSlop;
	}
}
