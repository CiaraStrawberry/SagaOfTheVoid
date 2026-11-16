using Photon;
using UnityEngine;

public class NetworkCharacter : Photon.MonoBehaviour
{
	private Vector3 correctPlayerPos = Vector3.zero;

	private Quaternion correctPlayerRot = Quaternion.identity;

	private void Update()
	{
		if (!photonView.isMine)
		{
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5f);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5f);
		}
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			myThirdPersonController component = GetComponent<myThirdPersonController>();
			stream.SendNext((int)component._characterState);
		}
		else
		{
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
			myThirdPersonController component2 = GetComponent<myThirdPersonController>();
			component2._characterState = (CharacterState)stream.ReceiveNext();
		}
	}
}
