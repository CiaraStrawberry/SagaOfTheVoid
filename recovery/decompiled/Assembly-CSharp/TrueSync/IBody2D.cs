namespace TrueSync;

public interface IBody2D : IBody
{
	bool TSIsStatic { get; set; }

	FP TSLinearDrag { get; set; }

	FP TSAngularDrag { get; set; }

	FP TSFriction { get; set; }

	FP TSRestitution { get; set; }

	TSVector2 TSPosition { get; set; }

	FP TSOrientation { get; set; }

	bool TSAffectedByGravity { get; set; }

	bool TSIsKinematic { get; set; }

	TSVector2 TSLinearVelocity { get; set; }

	FP TSAngularVelocity { get; set; }

	void TSApplyForce(TSVector2 force);

	void TSApplyForce(TSVector2 force, TSVector2 position);

	void TSApplyImpulse(TSVector2 force);

	void TSApplyImpulse(TSVector2 force, TSVector2 position);

	void TSApplyTorque(TSVector2 force);
}
