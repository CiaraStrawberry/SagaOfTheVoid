using System;
using ExitGames.Client.Photon.LoadBalancing;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice;

[RequireComponent(typeof(Button))]
public class PushToTalkPrivateButton : MonoBehaviour
{
	[SerializeField]
	private Button pushToTalkPrivateButton;

	[SerializeField]
	private Text buttonText;

	private PushToTalkScript pttScript;

	public byte AudioGroup;

	public bool Subscribed;

	private void Start()
	{
		pttScript = UnityEngine.Object.FindObjectOfType<PushToTalkScript>();
		UnityVoiceFrontend client = PhotonVoiceNetwork.Client;
		client.OnStateChangeAction = (Action<ExitGames.Client.Photon.LoadBalancing.ClientState>)Delegate.Combine(client.OnStateChangeAction, new Action<ExitGames.Client.Photon.LoadBalancing.ClientState>(OnVoiceClientStateChanged));
	}

	private void OnVoiceClientStateChanged(ExitGames.Client.Photon.LoadBalancing.ClientState state)
	{
		if (pushToTalkPrivateButton != null)
		{
			if (state == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined)
			{
				pushToTalkPrivateButton.gameObject.SetActive(value: true);
				Subscribed = Subscribed || PhotonVoiceNetwork.Client.ChangeAudioGroups(null, new byte[1] { AudioGroup });
			}
			else
			{
				pushToTalkPrivateButton.gameObject.SetActive(value: false);
			}
		}
	}

	public void SetAudioGroup(PhotonPlayer player)
	{
		if (Subscribed)
		{
			return;
		}
		buttonText.text = $"Talk-To-Player{player.ID}";
		int iD = player.ID;
		if (PhotonNetwork.player.ID < iD)
		{
			AudioGroup = (byte)(iD + PhotonNetwork.player.ID * 10);
		}
		else
		{
			if (PhotonNetwork.player.ID <= iD)
			{
				return;
			}
			AudioGroup = (byte)(PhotonNetwork.player.ID + iD * 10);
		}
		if (PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined)
		{
			Subscribed = PhotonVoiceNetwork.Client.ChangeAudioGroups(null, new byte[1] { AudioGroup });
		}
	}

	public void PushToTalkOn()
	{
		if (Subscribed)
		{
			PhotonVoiceNetwork.Client.GlobalAudioGroup = AudioGroup;
			pttScript.PushToTalk(toggle: true);
		}
	}

	public void PushToTalkOff()
	{
		pttScript.PushToTalkOff();
	}
}
