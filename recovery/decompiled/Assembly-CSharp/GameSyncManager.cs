using TrueSync;
using UnityEngine;

public class GameSyncManager : TrueSyncBehaviour
{
	public GameObject boxPrefab;

	public int numberOfBoxesX;

	public int numberOfBoxesZ;

	public override void OnSyncedStart()
	{
		CreateBoxes();
	}

	private void CreateBoxes()
	{
		for (int i = 0; i < numberOfBoxesX; i++)
		{
			for (int j = 0; j < numberOfBoxesZ; j++)
			{
				GameObject gameObject = TrueSyncManager.SyncedInstantiate(boxPrefab, TSVector.zero, TSQuaternion.identity);
				TSRigidBody component = gameObject.GetComponent<TSRigidBody>();
				component.position = new TSVector(i * 2 - 5, 1, j * 2);
			}
		}
	}

	public override void OnGamePaused()
	{
		Debug.Log("Game Paused");
	}

	public override void OnGameUnPaused()
	{
		Debug.Log("Game UnPaused");
	}

	public override void OnGameEnded()
	{
		Debug.Log("Game Ended");
	}

	public override void OnPlayerDisconnection(int playerId)
	{
		TrueSyncManager.RemovePlayer(playerId);
	}
}
