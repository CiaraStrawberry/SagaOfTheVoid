using UnityEngine;

public class Gui : MonoBehaviour
{
	private float dataRateNextTime;

	private int prevInBytes;

	private int prevOutBytes;

	private int dataRateIn;

	private int dataRateOut;

	private void Start()
	{
	}

	private void OnGUI()
	{
		PhotonVoiceRecorder photonVoiceRecorder = null;
		PhotonVoiceRecorder[] array = Object.FindObjectsOfType<PhotonVoiceRecorder>();
		foreach (PhotonVoiceRecorder photonVoiceRecorder2 in array)
		{
			if (photonVoiceRecorder2.photonView.isMine)
			{
				photonVoiceRecorder = photonVoiceRecorder2;
				break;
			}
		}
	}
}
