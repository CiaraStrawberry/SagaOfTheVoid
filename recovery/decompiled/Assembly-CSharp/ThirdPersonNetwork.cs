using Photon;
using UnityEngine;

public class ThirdPersonNetwork : Photon.MonoBehaviour
{
	private ThirdPersonCamera cameraScript;

	private ThirdPersonController controllerScript;

	private bool firstTake;

	private Vector3 correctPlayerPos = Vector3.zero;

	private Quaternion correctPlayerRot = Quaternion.identity;

	private void OnEnable()
	{
		firstTake = true;
	}

	private void Awake()
	{
		cameraScript = GetComponent<ThirdPersonCamera>();
		controllerScript = GetComponent<ThirdPersonController>();
		if (photonView.isMine)
		{
			cameraScript.enabled = true;
			controllerScript.enabled = true;
		}
		else
		{
			cameraScript.enabled = false;
			controllerScript.enabled = true;
			controllerScript.isControllable = false;
		}
		gameObject.name += photonView.viewID;
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext((int)controllerScript._characterState);
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			return;
		}
		controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
		correctPlayerPos = (Vector3)stream.ReceiveNext();
		correctPlayerRot = (Quaternion)stream.ReceiveNext();
		if (firstTake)
		{
			firstTake = false;
			transform.position = correctPlayerPos;
			transform.rotation = correctPlayerRot;
		}
	}

	private void Update()
	{
		if (!photonView.isMine)
		{
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5f);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5f);
		}
	}
}
