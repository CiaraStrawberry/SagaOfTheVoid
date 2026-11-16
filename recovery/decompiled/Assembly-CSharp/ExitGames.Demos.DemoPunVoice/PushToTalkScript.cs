using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice;

public class PushToTalkScript : MonoBehaviour
{
	[SerializeField]
	private Button pushToTalkButton;

	private Text pushToTalkText;

	private PhotonVoiceRecorder rec;

	private Transform parent;

	[SerializeField]
	private GameObject buttonPrefab;

	private Dictionary<int, PushToTalkPrivateButton> buttons;

	public void OnEnable()
	{
		CharacterInstantiation.CharacterInstantiated += CharacterInstantiation_CharacterInstantiated;
	}

	public void OnDisable()
	{
		CharacterInstantiation.CharacterInstantiated -= CharacterInstantiation_CharacterInstantiated;
	}

	public void Start()
	{
		if (pushToTalkButton != null)
		{
			pushToTalkText = pushToTalkButton.GetComponentInChildren<Text>();
			if (pushToTalkText != null)
			{
				pushToTalkText.text = "Push-To-Talk";
			}
			else
			{
				Debug.LogWarning("Could not find the button's text component.");
			}
			pushToTalkButton.gameObject.SetActive(value: false);
			parent = pushToTalkButton.transform.parent;
		}
		buttons = new Dictionary<int, PushToTalkPrivateButton>(4);
		UnityVoiceFrontend client = PhotonVoiceNetwork.Client;
		client.OnStateChangeAction = (Action<ExitGames.Client.Photon.LoadBalancing.ClientState>)Delegate.Combine(client.OnStateChangeAction, new Action<ExitGames.Client.Photon.LoadBalancing.ClientState>(OnVoiceClientStateChanged));
	}

	private void CharacterInstantiation_CharacterInstantiated(GameObject character)
	{
		rec = character.GetComponent<PhotonVoiceRecorder>();
		if (rec != null)
		{
			rec.enabled = true;
		}
	}

	private void OnVoiceClientStateChanged(ExitGames.Client.Photon.LoadBalancing.ClientState state)
	{
		if (!(pushToTalkButton != null))
		{
			return;
		}
		if (state == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined)
		{
			pushToTalkButton.gameObject.SetActive(value: true);
			for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
			{
				OnPhotonPlayerConnected(PhotonNetwork.otherPlayers[i]);
			}
		}
		else
		{
			pushToTalkButton.gameObject.SetActive(value: false);
		}
	}

	public void PushToTalkOn()
	{
		PhotonVoiceNetwork.Client.GlobalAudioGroup = 0;
		PushToTalk(toggle: true);
		if (pushToTalkText != null)
		{
			pushToTalkText.text = "Talk now!";
		}
	}

	public void PushToTalkOff()
	{
		PushToTalk(toggle: false);
		if (pushToTalkText != null)
		{
			pushToTalkText.text = "Push-To-Talk";
		}
		byte[] array = new byte[buttons.Count];
		for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
		{
			if (!buttons[PhotonNetwork.otherPlayers[i].ID].Subscribed)
			{
				array[i] = buttons[PhotonNetwork.otherPlayers[i].ID].AudioGroup;
			}
		}
		if (array.Length != 0)
		{
			PhotonVoiceNetwork.Client.ChangeAudioGroups(null, array);
		}
	}

	public void PushToTalk(bool toggle)
	{
		if (rec != null)
		{
			rec.Transmit = toggle;
		}
	}

	public void OnLeftRoom()
	{
		if (rec != null)
		{
			rec.enabled = false;
			rec = null;
		}
	}

	public void Update()
	{
		if (Input.GetKeyDown("v"))
		{
			PushToTalkOn();
		}
		else if (Input.GetKeyUp("v"))
		{
			PushToTalkOff();
		}
	}

	public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(buttonPrefab);
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		gameObject.transform.SetSiblingIndex(newPlayer.ID);
		PushToTalkPrivateButton componentInChildren = gameObject.GetComponentInChildren<PushToTalkPrivateButton>();
		componentInChildren.SetAudioGroup(newPlayer);
		buttons.Add(newPlayer.ID, componentInChildren);
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer leavingPlayer)
	{
		PhotonVoiceNetwork.Client.ChangeAudioGroups(new byte[1] { buttons[leavingPlayer.ID].AudioGroup }, null);
		UnityEngine.Object.Destroy(buttons[leavingPlayer.ID].gameObject);
		buttons.Remove(leavingPlayer.ID);
	}
}
