using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PickupItemSimple : Photon.MonoBehaviour
{
	public float SecondsBeforeRespawn = 2f;

	public bool PickupOnCollide;

	public bool SentPickup;

	public void OnTriggerEnter(Collider other)
	{
		PhotonView component = other.GetComponent<PhotonView>();
		if (PickupOnCollide && component != null && component.isMine)
		{
			Pickup();
		}
	}

	public void Pickup()
	{
		if (!SentPickup)
		{
			SentPickup = true;
			photonView.RPC("PunPickupSimple", PhotonTargets.AllViaServer);
		}
	}

	[PunRPC]
	public void PunPickupSimple(PhotonMessageInfo msgInfo)
	{
		if (!SentPickup || !msgInfo.sender.IsLocal || gameObject.GetActive())
		{
		}
		SentPickup = false;
		if (!gameObject.GetActive())
		{
			Debug.Log("Ignored PU RPC, cause item is inactive. " + gameObject);
			return;
		}
		double num = PhotonNetwork.time - msgInfo.timestamp;
		float num2 = SecondsBeforeRespawn - (float)num;
		if (num2 > 0f)
		{
			gameObject.SetActive(value: false);
			Invoke("RespawnAfter", num2);
		}
	}

	public void RespawnAfter()
	{
		if (gameObject != null)
		{
			gameObject.SetActive(value: true);
		}
	}
}
