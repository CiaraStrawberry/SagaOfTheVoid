using TrueSync;

public class BallController : TrueSyncBehaviour
{
	public int maxX;

	public int maxY;

	[AddTracking]
	public float speedX;

	[AddTracking]
	public float speedY;

	public override void OnSyncedUpdate()
	{
		TSVector2 position = tsRigidBody2D.position;
		position.x += (FP)speedX;
		position.y += (FP)speedY;
		tsRigidBody2D.position = position;
	}

	public void OnSyncedTriggerEnter(TSCollision2D other)
	{
		if (other.gameObject.tag == "GroundHor")
		{
			speedX *= -1f;
		}
		else if (other.gameObject.tag == "GroundVer")
		{
			if (speedY < 0f)
			{
				CallPaddleScore(isTopSide: true);
			}
			else
			{
				CallPaddleScore(isTopSide: false);
			}
			speedY *= -1f;
		}
		else if (speedY * tsRigidBody2D.position.y > 0)
		{
			speedY *= -1f;
		}
	}

	private void CallPaddleScore(bool isTopSide)
	{
		if (PaddleController.paddlesBySide.ContainsKey(isTopSide))
		{
			PaddleController.paddlesBySide[isTopSide].Score();
		}
	}
}
