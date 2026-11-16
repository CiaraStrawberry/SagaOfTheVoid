using Photon;
using UnityEngine;

public class SoundsForJoinAndLeave : PunBehaviour
{
	public AudioClip JoinClip;

	public AudioClip LeaveClip;

	private AudioSource source;

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		if (JoinClip != null)
		{
			if (source == null)
			{
				source = Object.FindObjectOfType<AudioSource>();
			}
			source.PlayOneShot(JoinClip);
		}
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		if (LeaveClip != null)
		{
			if (source == null)
			{
				source = Object.FindObjectOfType<AudioSource>();
			}
			source.PlayOneShot(LeaveClip);
		}
	}
}
