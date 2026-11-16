namespace TrueSync.Physics3D;

public class Constraint2D : Constraint
{
	private bool freezeZAxis;

	public Constraint2D(RigidBody body, bool freezeZAxis)
		: base(body, null)
	{
		this.freezeZAxis = freezeZAxis;
	}

	public override void PostStep()
	{
		TSVector position = Body1.Position;
		position.z = 0;
		Body1.Position = position;
		TSQuaternion quaternion = TSQuaternion.CreateFromMatrix(Body1.Orientation);
		quaternion.Normalize();
		quaternion.x = 0;
		quaternion.y = 0;
		if (freezeZAxis)
		{
			quaternion.z = 0;
		}
		Body1.Orientation = TSMatrix.CreateFromQuaternion(quaternion);
		if (!Body1.isStatic)
		{
			TSVector linearVelocity = Body1.LinearVelocity;
			linearVelocity.z = 0;
			Body1.LinearVelocity = linearVelocity;
			TSVector angularVelocity = Body1.AngularVelocity;
			angularVelocity.x = 0;
			angularVelocity.y = 0;
			if (freezeZAxis)
			{
				angularVelocity.z = 0;
			}
			Body1.AngularVelocity = angularVelocity;
		}
	}
}
