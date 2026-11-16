using TrueSync;
using UnityEngine;

public class PongSyncManager : TrueSyncBehaviour
{
	public GameObject ballPrefab;

	public override void OnSyncedStart()
	{
		CreateBall();
	}

	private void CreateBall()
	{
		TrueSyncManager.SyncedInstantiate(ballPrefab, TSVector.zero, TSQuaternion.identity);
	}
}
