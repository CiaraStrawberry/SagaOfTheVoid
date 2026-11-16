using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class CubeLerp : Photon.MonoBehaviour, IPunObservable
{
	private Vector3 latestCorrectPos;

	private Vector3 onUpdatePos;

	private float fraction;

	public void Start()
	{
		latestCorrectPos = transform.position;
		onUpdatePos = transform.position;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 obj = transform.localPosition;
			Quaternion obj2 = transform.localRotation;
			stream.Serialize(ref obj);
			stream.Serialize(ref obj2);
			return;
		}
		Vector3 obj3 = Vector3.zero;
		Quaternion obj4 = Quaternion.identity;
		stream.Serialize(ref obj3);
		stream.Serialize(ref obj4);
		latestCorrectPos = obj3;
		onUpdatePos = transform.localPosition;
		fraction = 0f;
		transform.localRotation = obj4;
	}

	public void Update()
	{
		if (!photonView.isMine)
		{
			fraction += Time.deltaTime * 9f;
			transform.localPosition = Vector3.Lerp(onUpdatePos, latestCorrectPos, fraction);
		}
	}
}
