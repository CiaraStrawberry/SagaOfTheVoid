using System.Collections;
using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnClickDestroy : Photon.MonoBehaviour
{
	public bool DestroyByRpc;

	public void OnClick()
	{
		if (!DestroyByRpc)
		{
			PhotonNetwork.Destroy(gameObject);
		}
		else
		{
			photonView.RPC("DestroyRpc", PhotonTargets.AllBuffered);
		}
	}

	[PunRPC]
	public IEnumerator DestroyRpc()
	{
		Object.Destroy(gameObject);
		yield return 0;
		PhotonNetwork.UnAllocateViewID(photonView.viewID);
	}
}
