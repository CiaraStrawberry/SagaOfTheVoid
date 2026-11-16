namespace TrueSync.Physics2D;

public class GearJoint : Joint2D
{
	private JointType _typeA;

	private JointType _typeB;

	private Body _bodyA;

	private Body _bodyB;

	private Body _bodyC;

	private Body _bodyD;

	private TSVector2 _localAnchorA;

	private TSVector2 _localAnchorB;

	private TSVector2 _localAnchorC;

	private TSVector2 _localAnchorD;

	private TSVector2 _localAxisC;

	private TSVector2 _localAxisD;

	private FP _referenceAngleA;

	private FP _referenceAngleB;

	private FP _constant;

	private FP _ratio;

	private FP _impulse;

	private int _indexA;

	private int _indexB;

	private int _indexC;

	private int _indexD;

	private TSVector2 _lcA;

	private TSVector2 _lcB;

	private TSVector2 _lcC;

	private TSVector2 _lcD;

	private FP _mA;

	private FP _mB;

	private FP _mC;

	private FP _mD;

	private FP _iA;

	private FP _iB;

	private FP _iC;

	private FP _iD;

	private TSVector2 _JvAC;

	private TSVector2 _JvBD;

	private FP _JwA;

	private FP _JwB;

	private FP _JwC;

	private FP _JwD;

	private FP _mass;

	public override TSVector2 WorldAnchorA
	{
		get
		{
			return _bodyA.GetWorldPoint(_localAnchorA);
		}
		set
		{
		}
	}

	public override TSVector2 WorldAnchorB
	{
		get
		{
			return _bodyB.GetWorldPoint(_localAnchorB);
		}
		set
		{
		}
	}

	public FP Ratio
	{
		get
		{
			return _ratio;
		}
		set
		{
			_ratio = value;
		}
	}

	public Joint2D JointA { get; private set; }

	public Joint2D JointB { get; private set; }

	public GearJoint(Body bodyA, Body bodyB, Joint2D jointA, Joint2D jointB, FP ratio)
	{
		JointType = JointType.Gear;
		BodyA = bodyA;
		BodyB = bodyB;
		JointA = jointA;
		JointB = jointB;
		Ratio = ratio;
		_typeA = jointA.JointType;
		_typeB = jointB.JointType;
		_bodyC = JointA.BodyA;
		_bodyA = JointA.BodyB;
		Transform xf = _bodyA._xf;
		FP a = _bodyA._sweep.A;
		Transform xf2 = _bodyC._xf;
		FP a2 = _bodyC._sweep.A;
		FP fP;
		if (_typeA == JointType.Revolute)
		{
			RevoluteJoint revoluteJoint = (RevoluteJoint)jointA;
			_localAnchorC = revoluteJoint.LocalAnchorA;
			_localAnchorA = revoluteJoint.LocalAnchorB;
			_referenceAngleA = revoluteJoint.ReferenceAngle;
			_localAxisC = TSVector2.zero;
			fP = a - a2 - _referenceAngleA;
		}
		else
		{
			PrismaticJoint prismaticJoint = (PrismaticJoint)jointA;
			_localAnchorC = prismaticJoint.LocalAnchorA;
			_localAnchorA = prismaticJoint.LocalAnchorB;
			_referenceAngleA = prismaticJoint.ReferenceAngle;
			_localAxisC = prismaticJoint.LocalXAxis;
			TSVector2 localAnchorC = _localAnchorC;
			TSVector2 tSVector = MathUtils.MulT(xf2.q, MathUtils.Mul(xf.q, _localAnchorA) + (xf.p - xf2.p));
			fP = TSVector2.Dot(tSVector - localAnchorC, _localAxisC);
		}
		_bodyD = JointB.BodyA;
		_bodyB = JointB.BodyB;
		Transform xf3 = _bodyB._xf;
		FP a3 = _bodyB._sweep.A;
		Transform xf4 = _bodyD._xf;
		FP a4 = _bodyD._sweep.A;
		FP fP2;
		if (_typeB == JointType.Revolute)
		{
			RevoluteJoint revoluteJoint2 = (RevoluteJoint)jointB;
			_localAnchorD = revoluteJoint2.LocalAnchorA;
			_localAnchorB = revoluteJoint2.LocalAnchorB;
			_referenceAngleB = revoluteJoint2.ReferenceAngle;
			_localAxisD = TSVector2.zero;
			fP2 = a3 - a4 - _referenceAngleB;
		}
		else
		{
			PrismaticJoint prismaticJoint2 = (PrismaticJoint)jointB;
			_localAnchorD = prismaticJoint2.LocalAnchorA;
			_localAnchorB = prismaticJoint2.LocalAnchorB;
			_referenceAngleB = prismaticJoint2.ReferenceAngle;
			_localAxisD = prismaticJoint2.LocalXAxis;
			TSVector2 localAnchorD = _localAnchorD;
			TSVector2 tSVector2 = MathUtils.MulT(xf4.q, MathUtils.Mul(xf3.q, _localAnchorB) + (xf3.p - xf4.p));
			fP2 = TSVector2.Dot(tSVector2 - localAnchorD, _localAxisD);
		}
		_ratio = ratio;
		_constant = fP + _ratio * fP2;
		_impulse = 0f;
	}

	public override TSVector2 GetReactionForce(FP invDt)
	{
		TSVector2 tSVector = _impulse * _JvAC;
		return invDt * tSVector;
	}

	public override FP GetReactionTorque(FP invDt)
	{
		FP fP = _impulse * _JwA;
		return invDt * fP;
	}

	internal override void InitVelocityConstraints(ref SolverData data)
	{
		_indexA = _bodyA.IslandIndex;
		_indexB = _bodyB.IslandIndex;
		_indexC = _bodyC.IslandIndex;
		_indexD = _bodyD.IslandIndex;
		_lcA = _bodyA._sweep.LocalCenter;
		_lcB = _bodyB._sweep.LocalCenter;
		_lcC = _bodyC._sweep.LocalCenter;
		_lcD = _bodyD._sweep.LocalCenter;
		_mA = _bodyA._invMass;
		_mB = _bodyB._invMass;
		_mC = _bodyC._invMass;
		_mD = _bodyD._invMass;
		_iA = _bodyA._invI;
		_iB = _bodyB._invI;
		_iC = _bodyC._invI;
		_iD = _bodyD._invI;
		FP a = data.positions[_indexA].a;
		TSVector2 v = data.velocities[_indexA].v;
		FP w = data.velocities[_indexA].w;
		FP a2 = data.positions[_indexB].a;
		TSVector2 v2 = data.velocities[_indexB].v;
		FP w2 = data.velocities[_indexB].w;
		FP a3 = data.positions[_indexC].a;
		TSVector2 v3 = data.velocities[_indexC].v;
		FP w3 = data.velocities[_indexC].w;
		FP a4 = data.positions[_indexD].a;
		TSVector2 v4 = data.velocities[_indexD].v;
		FP w4 = data.velocities[_indexD].w;
		Rot q = new Rot(a);
		Rot q2 = new Rot(a2);
		Rot q3 = new Rot(a3);
		Rot q4 = new Rot(a4);
		_mass = 0f;
		if (_typeA == JointType.Revolute)
		{
			_JvAC = TSVector2.zero;
			_JwA = 1f;
			_JwC = 1f;
			_mass += _iA + _iC;
		}
		else
		{
			TSVector2 tSVector = MathUtils.Mul(q3, _localAxisC);
			TSVector2 a5 = MathUtils.Mul(q3, _localAnchorC - _lcC);
			TSVector2 a6 = MathUtils.Mul(q, _localAnchorA - _lcA);
			_JvAC = tSVector;
			_JwC = MathUtils.Cross(a5, tSVector);
			_JwA = MathUtils.Cross(a6, tSVector);
			_mass += _mC + _mA + _iC * _JwC * _JwC + _iA * _JwA * _JwA;
		}
		if (_typeB == JointType.Revolute)
		{
			_JvBD = TSVector2.zero;
			_JwB = _ratio;
			_JwD = _ratio;
			_mass += _ratio * _ratio * (_iB + _iD);
		}
		else
		{
			TSVector2 tSVector2 = MathUtils.Mul(q4, _localAxisD);
			TSVector2 a7 = MathUtils.Mul(q4, _localAnchorD - _lcD);
			TSVector2 a8 = MathUtils.Mul(q2, _localAnchorB - _lcB);
			_JvBD = _ratio * tSVector2;
			_JwD = _ratio * MathUtils.Cross(a7, tSVector2);
			_JwB = _ratio * MathUtils.Cross(a8, tSVector2);
			_mass += _ratio * _ratio * (_mD + _mB) + _iD * _JwD * _JwD + _iB * _JwB * _JwB;
		}
		_mass = ((!(_mass > 0f)) ? ((FP)0f) : (1f / _mass));
		v += _mA * _impulse * _JvAC;
		w += _iA * _impulse * _JwA;
		v2 += _mB * _impulse * _JvBD;
		w2 += _iB * _impulse * _JwB;
		v3 -= _mC * _impulse * _JvAC;
		w3 -= _iC * _impulse * _JwC;
		v4 -= _mD * _impulse * _JvBD;
		w4 -= _iD * _impulse * _JwD;
		data.velocities[_indexA].v = v;
		data.velocities[_indexA].w = w;
		data.velocities[_indexB].v = v2;
		data.velocities[_indexB].w = w2;
		data.velocities[_indexC].v = v3;
		data.velocities[_indexC].w = w3;
		data.velocities[_indexD].v = v4;
		data.velocities[_indexD].w = w4;
	}

	internal override void SolveVelocityConstraints(ref SolverData data)
	{
		TSVector2 v = data.velocities[_indexA].v;
		FP w = data.velocities[_indexA].w;
		TSVector2 v2 = data.velocities[_indexB].v;
		FP w2 = data.velocities[_indexB].w;
		TSVector2 v3 = data.velocities[_indexC].v;
		FP w3 = data.velocities[_indexC].w;
		TSVector2 v4 = data.velocities[_indexD].v;
		FP w4 = data.velocities[_indexD].w;
		FP fP = TSVector2.Dot(_JvAC, v - v3) + TSVector2.Dot(_JvBD, v2 - v4);
		fP += _JwA * w - _JwC * w3 + (_JwB * w2 - _JwD * w4);
		FP fP2 = -_mass * fP;
		_impulse += fP2;
		v += _mA * fP2 * _JvAC;
		w += _iA * fP2 * _JwA;
		v2 += _mB * fP2 * _JvBD;
		w2 += _iB * fP2 * _JwB;
		v3 -= _mC * fP2 * _JvAC;
		w3 -= _iC * fP2 * _JwC;
		v4 -= _mD * fP2 * _JvBD;
		w4 -= _iD * fP2 * _JwD;
		data.velocities[_indexA].v = v;
		data.velocities[_indexA].w = w;
		data.velocities[_indexB].v = v2;
		data.velocities[_indexB].w = w2;
		data.velocities[_indexC].v = v3;
		data.velocities[_indexC].w = w3;
		data.velocities[_indexD].v = v4;
		data.velocities[_indexD].w = w4;
	}

	internal override bool SolvePositionConstraints(ref SolverData data)
	{
		TSVector2 c = data.positions[_indexA].c;
		FP a = data.positions[_indexA].a;
		TSVector2 c2 = data.positions[_indexB].c;
		FP a2 = data.positions[_indexB].a;
		TSVector2 c3 = data.positions[_indexC].c;
		FP a3 = data.positions[_indexC].a;
		TSVector2 c4 = data.positions[_indexD].c;
		FP a4 = data.positions[_indexD].a;
		Rot q = new Rot(a);
		Rot q2 = new Rot(a2);
		Rot q3 = new Rot(a3);
		Rot q4 = new Rot(a4);
		FP fP = 0f;
		FP fP2 = 0f;
		TSVector2 tSVector;
		FP fP3;
		FP fP4;
		FP fP5;
		if (_typeA == JointType.Revolute)
		{
			tSVector = TSVector2.zero;
			fP3 = 1f;
			fP4 = 1f;
			fP2 += _iA + _iC;
			fP5 = a - a3 - _referenceAngleA;
		}
		else
		{
			TSVector2 tSVector2 = MathUtils.Mul(q3, _localAxisC);
			TSVector2 a5 = MathUtils.Mul(q3, _localAnchorC - _lcC);
			TSVector2 tSVector3 = MathUtils.Mul(q, _localAnchorA - _lcA);
			tSVector = tSVector2;
			fP4 = MathUtils.Cross(a5, tSVector2);
			fP3 = MathUtils.Cross(tSVector3, tSVector2);
			fP2 += _mC + _mA + _iC * fP4 * fP4 + _iA * fP3 * fP3;
			TSVector2 tSVector4 = _localAnchorC - _lcC;
			TSVector2 tSVector5 = MathUtils.MulT(q3, tSVector3 + (c - c3));
			fP5 = TSVector2.Dot(tSVector5 - tSVector4, _localAxisC);
		}
		TSVector2 tSVector6;
		FP fP6;
		FP fP7;
		FP fP8;
		if (_typeB == JointType.Revolute)
		{
			tSVector6 = TSVector2.zero;
			fP6 = _ratio;
			fP7 = _ratio;
			fP2 += _ratio * _ratio * (_iB + _iD);
			fP8 = a2 - a4 - _referenceAngleB;
		}
		else
		{
			TSVector2 tSVector7 = MathUtils.Mul(q4, _localAxisD);
			TSVector2 a6 = MathUtils.Mul(q4, _localAnchorD - _lcD);
			TSVector2 tSVector8 = MathUtils.Mul(q2, _localAnchorB - _lcB);
			tSVector6 = _ratio * tSVector7;
			fP7 = _ratio * MathUtils.Cross(a6, tSVector7);
			fP6 = _ratio * MathUtils.Cross(tSVector8, tSVector7);
			fP2 += _ratio * _ratio * (_mD + _mB) + _iD * fP7 * fP7 + _iB * fP6 * fP6;
			TSVector2 tSVector9 = _localAnchorD - _lcD;
			TSVector2 tSVector10 = MathUtils.MulT(q4, tSVector8 + (c2 - c4));
			fP8 = TSVector2.Dot(tSVector10 - tSVector9, _localAxisD);
		}
		FP fP9 = fP5 + _ratio * fP8 - _constant;
		FP fP10 = 0f;
		if (fP2 > 0f)
		{
			fP10 = -fP9 / fP2;
		}
		c += _mA * fP10 * tSVector;
		a += _iA * fP10 * fP3;
		c2 += _mB * fP10 * tSVector6;
		a2 += _iB * fP10 * fP6;
		c3 -= _mC * fP10 * tSVector;
		a3 -= _iC * fP10 * fP4;
		c4 -= _mD * fP10 * tSVector6;
		a4 -= _iD * fP10 * fP7;
		data.positions[_indexA].c = c;
		data.positions[_indexA].a = a;
		data.positions[_indexB].c = c2;
		data.positions[_indexB].a = a2;
		data.positions[_indexC].c = c3;
		data.positions[_indexC].a = a3;
		data.positions[_indexD].c = c4;
		data.positions[_indexD].a = a4;
		return fP < Settings.LinearSlop;
	}
}
