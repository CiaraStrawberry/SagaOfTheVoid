using UnityEngine;

namespace Forge3D;

public class F3DWarpJump : MonoBehaviour
{
	public ParticleSystem WarpSpark;

	public Transform ShipPos;

	public float ShipJumpSpeed;

	public Vector3 ShipJumpStartPoint;

	public Vector3 ShipJumpEndPoint;

	public bool SendOnSpawned;

	public AudioSource source;

	public AudioClip warpsound;

	private bool isWarping;

	private void Start()
	{
		if (SendOnSpawned)
		{
			BroadcastMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);
		}
		source = GetComponent<AudioSource>();
		source.PlayOneShot(warpsound);
	}

	public void OnSpawned()
	{
	}

	private void OnWarp()
	{
		isWarping = true;
	}

	private void ShiftShipPosition()
	{
	}

	private void Update()
	{
		if (isWarping)
		{
			ShiftShipPosition();
		}
	}
}
