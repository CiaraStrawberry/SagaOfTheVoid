using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnAwakeUsePhotonView : Photon.MonoBehaviour
{
	private void Awake()
	{
		if (photonView.isMine)
		{
			photonView.RPC("OnAwakeRPC", PhotonTargets.All);
		}
	}

	private void Start()
	{
		if (photonView.isMine)
		{
			photonView.RPC("OnAwakeRPC", PhotonTargets.All, (byte)1);
		}
	}

	[PunRPC]
	public void OnAwakeRPC()
	{
		Debug.Log("RPC: 'OnAwakeRPC' PhotonView: " + photonView);
	}

	[PunRPC]
	public void OnAwakeRPC(byte myParameter)
	{
		Debug.Log("RPC: 'OnAwakeRPC' Parameter: " + myParameter + " PhotonView: " + photonView);
	}
}
