using Photon;
using UnityEngine;

public class OnDoubleclickDestroy : Photon.MonoBehaviour
{
	private float timeOfLastClick;

	private const float ClickDeltaForDoubleclick = 0.2f;

	private void OnClick()
	{
		if (photonView.isMine)
		{
			if (Time.time - timeOfLastClick < 0.2f)
			{
				PhotonNetwork.Destroy(gameObject);
			}
			else
			{
				timeOfLastClick = Time.time;
			}
		}
	}
}
