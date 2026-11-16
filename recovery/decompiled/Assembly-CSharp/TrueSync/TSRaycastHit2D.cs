namespace TrueSync;

public class TSRaycastHit2D
{
	public TSCollider2D collider;

	public TSRigidBody2D rigidbody;

	public TSTransform2D transform;

	public TSRaycastHit2D(TSCollider2D collider)
	{
		this.collider = collider;
		rigidbody = collider.GetComponent<TSRigidBody2D>();
		transform = collider.GetComponent<TSTransform2D>();
	}
}
