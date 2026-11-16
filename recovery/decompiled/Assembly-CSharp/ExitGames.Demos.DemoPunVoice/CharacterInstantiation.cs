using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice;

public class CharacterInstantiation : OnJoinedInstantiate
{
	public delegate void OnCharacterInstantiated(GameObject character);

	public static event OnCharacterInstantiated CharacterInstantiated;

	public new void OnJoinedRoom()
	{
		if (PrefabsToInstantiate != null)
		{
			GameObject gameObject = PrefabsToInstantiate[(PhotonNetwork.player.ID - 1) % 4];
			Vector3 vector = Vector3.zero;
			if (SpawnPosition != null)
			{
				vector = SpawnPosition.position;
			}
			Vector3 insideUnitSphere = Random.insideUnitSphere;
			insideUnitSphere = PositionOffset * insideUnitSphere.normalized;
			vector += insideUnitSphere;
			vector.y = 0f;
			Camera.main.transform.position += vector;
			gameObject = PhotonNetwork.Instantiate(gameObject.name, vector, Quaternion.identity, 0);
			if (CharacterInstantiated != null)
			{
				CharacterInstantiated(gameObject);
			}
		}
	}
}
