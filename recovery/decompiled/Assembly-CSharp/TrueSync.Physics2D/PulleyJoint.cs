namespace TrueSync.Physics2D;

public class PulleyJoint : Joint2D
{
	private FP _impulse;

	private int _indexA;

	private int _indexB;

	private TSVector2 _uA;

	private TSVector2 _uB;

	private TSVector2 _rA;

	private TSVector2 _rB;

	private TSVector2 _localCenterA;

	private TSVector2 _localCenterB;

	private FP _invMassA;

	private FP _invMassB;

	private FP _invIA;

	private FP _invIB;

	private FP _mass;

	public TSVector2 LocalAnchorA { get; set; }

	public TSVector2 LocalAnchorB { get; set; }

	public sealed override TSVector2 WorldAnchorA { get; set; }

	public sealed override TSVector2 WorldAnchorB { get; set; }

	public FP LengthA { get; set; }

	public FP LengthB { get; set; }

	public FP CurrentLengthA
	{
		get
		{
			TSVector2 worldPoint = BodyA.GetWorldPoint(LocalAnchorA);
			TSVector2 worldAnchorA = WorldAnchorA;
			return (worldPoint - worldAnchorA).magnitude;
		}
	}

	public FP CurrentLengthB
	{
		get
		{
			TSVector2 worldPoint = BodyB.GetWorldPoint(LocalAnchorB);
			TSVector2 worldAnchorB = WorldAnchorB;
			return (worldPoint - worldAnchorB).magnitude;
		}
	}

	public FP Ratio { get; set; }

	internal FP Constant { get; set; }

	internal PulleyJoint()
	{
		JointType = JointType.Pulley;
	}

	public PulleyJoint(Body bodyA, Body bodyB, TSVector2 anchorA, TSVector2 anchorB, TSVector2 worldAnchorA, TSVector2 worldAnchorB, FP ratio, bool useWorldCoordinates = false)
		: base(bodyA, bodyB)
	{
		JointType = JointType.Pulley;
		WorldAnchorA = worldAnchorA;
		WorldAnchorB = worldAnchorB;
		if (useWorldCoordinates)
		{
			LocalAnchorA = BodyA.GetLocalPoint(anchorA);
			LocalAnchorB = BodyB.GetLocalPoint(anchorB);
			LengthA = (anchorA - worldAnchorA).magnitude;
			LengthB = (anchorB - worldAnchorB).magnitude;
		}
		else
		{
			LocalAnchorA = anchorA;
			LocalAnchorB = anchorB;
			LengthA = (anchorA - BodyA.GetLocalPoint(worldAnchorA)).magnitude;
			LengthB = (anchorB - BodyB.GetLocalPoint(worldAnchorB)).magnitude;
		}
		Ratio = ratio;
		Constant = LengthA + ratio * LengthB;
		_impulse = 0f;
	}

	public override TSVector2 GetReactionForce(FP invDt)
	{
		TSVector2 tSVector = _impulse * _uB;
		return invDt * tSVector;
	}

	public override FP GetReactionTorque(FP invDt)
	{
		return 0f;
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
		_uA = c + _rA - WorldAnchorA;
		_uB = c2 + _rB - WorldAnchorB;
		FP magnitude = _uA.magnitude;
		FP magnitude2 = _uB.magnitude;
		if (magnitude > 10f * Settings.LinearSlop)
		{
			_uA *= 1f / magnitude;
		}
		else
		{
			_uA = TSVector2.zero;
		}
		if (magnitude2 > 10f * Settings.LinearSlop)
		{
			_uB *= 1f / magnitude2;
		}
		else
		{
			_uB = TSVector2.zero;
		}
		FP fP = MathUtils.Cross(_rA, _uA);
		FP fP2 = MathUtils.Cross(_rB, _uB);
		FP fP3 = _invMassA + _invIA * fP * fP;
		FP fP4 = _invMassB + _invIB * fP2 * fP2;
		_mass = fP3 + Ratio * Ratio * fP4;
		if (_mass > 0f)
		{
			_mass = 1f / _mass;
		}
		_impulse *= data.step.dtRatio;
		TSVector2 tSVector = -_impulse * _uA;
		TSVector2 tSVector2 = -Ratio * _impulse * _uB;
		v += _invMassA * tSVector;
		w += _invIA * MathUtils.Cross(_rA, tSVector);
		v2 += _invMassB * tSVector2;
		w2 += _invIB * MathUtils.Cross(_rB, tSVector2);
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
		TSVector2 value = v + MathUtils.Cross(w, _rA);
		TSVector2 value2 = v2 + MathUtils.Cross(w2, _rB);
		FP fP = -TSVector2.Dot(_uA, value) - Ratio * TSVector2.Dot(_uB, value2);
		FP fP2 = -_mass * fP;
		_impulse += fP2;
		TSVector2 tSVector = -fP2 * _uA;
		TSVector2 tSVector2 = -Ratio * fP2 * _uB;
		v += _invMassA * tSVector;
		w += _invIA * MathUtils.Cross(_rA, tSVector);
		v2 += _invMassB * tSVector2;
		w2 += _invIB * MathUtils.Cross(_rB, tSVector2);
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
		TSVector2 tSVector3 = c + tSVector - WorldAnchorA;
		TSVector2 tSVector4 = c2 + tSVector2 - WorldAnchorB;
		FP magnitude = tSVector3.magnitude;
		FP magnitude2 = tSVector4.magnitude;
		if (magnitude > 10f * Settings.LinearSlop)
		{
			tSVector3 *= 1f / magnitude;
		}
		else
		{
			tSVector3 = TSVector2.zero;
		}
		if (magnitude2 > 10f * Settings.LinearSlop)
		{
			tSVector4 *= 1f / magnitude2;
		}
		else
		{
			tSVector4 = TSVector2.zero;
		}
		FP fP = MathUtils.Cross(tSVector, tSVector3);
		FP fP2 = MathUtils.Cross(tSVector2, tSVector4);
		FP fP3 = _invMassA + _invIA * fP * fP;
		FP fP4 = _invMassB + _invIB * fP2 * fP2;
		FP fP5 = fP3 + Ratio * Ratio * fP4;
		if (fP5 > 0f)
		{
			fP5 = 1f / fP5;
		}
		FP fP6 = Constant - magnitude - Ratio * magnitude2;
		FP fP7 = FP.Abs(fP6);
		FP fP8 = -fP5 * fP6;
		TSVector2 tSVector5 = -fP8 * tSVector3;
		TSVector2 tSVector6 = -Ratio * fP8 * tSVector4;
		c += _invMassA * tSVector5;
		a += _invIA * MathUtils.Cross(tSVector, tSVector5);
		c2 += _invMassB * tSVector6;
		a2 += _invIB * MathUtils.Cross(tSVector2, tSVector6);
		data.positions[_indexA].c = c;
		data.positions[_indexA].a = a;
		data.positions[_indexB].c = c2;
		data.positions[_indexB].a = a2;
		return fP7 < Settings.LinearSlop;
	}
}
