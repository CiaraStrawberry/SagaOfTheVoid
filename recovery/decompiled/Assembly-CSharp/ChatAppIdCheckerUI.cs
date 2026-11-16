using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ChatAppIdCheckerUI : MonoBehaviour
{
	public Text Description;

	private void Start()
	{
	}

	private void Update()
	{
		if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.VoiceAppID))
		{
			Description.text = "<Color=Red>WARNING:</Color>\nTo run this, set the Chat AppId in the PhotonServerSettings file.";
		}
		else
		{
			Description.text = string.Empty;
		}
	}
}
