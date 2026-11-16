using UnityEngine;

namespace Devdog.General;

public class PositionAtPlayer : MonoBehaviour
{
	public Vector3 offset;

	public bool continuous;

	protected void OnEnable()
	{
		PositionNow();
	}

	protected void LateUpdate()
	{
		if (continuous)
		{
			PositionNow();
		}
	}

	private void PositionNow()
	{
		if (!(ManagerBase<PlayerManager>.instance == null) && !(ManagerBase<PlayerManager>.instance.currentPlayer == null))
		{
			transform.position = ManagerBase<PlayerManager>.instance.currentPlayer.transform.position;
		}
	}
}
