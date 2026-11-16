using System;
using UnityEngine;

public class ClickDetector : MonoBehaviour
{
	public void Update()
	{
		if (PhotonNetwork.player.ID == GameLogic.playerWhoIsIt && Input.GetButton("Fire1"))
		{
			GameObject gameObject = RaycastObject(Input.mousePosition);
			if (gameObject != null && gameObject != base.gameObject && gameObject.name.Equals("monsterprefab(Clone)", StringComparison.OrdinalIgnoreCase))
			{
				PhotonView component = gameObject.transform.root.GetComponent<PhotonView>();
				GameLogic.TagPlayer(component.owner.ID);
			}
		}
	}

	private GameObject RaycastObject(Vector2 screenPos)
	{
		Camera main = Camera.main;
		if (Physics.Raycast(main.ScreenPointToRay(screenPos), out var hitInfo, 200f))
		{
			return hitInfo.collider.gameObject;
		}
		return null;
	}
}
