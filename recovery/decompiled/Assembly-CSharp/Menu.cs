using System;
using System.Collections.Generic;
using Photon;
using TrueSync;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : PunBehaviour
{
	private const int TIME_TO_START_MATCH = 3;

	public Text infoText;

	[Header("Main Panel")]
	public GameObject mainPanel;

	[Header("Nick Panel")]
	public GameObject nickPanel;

	public Text nickWelcomeText;

	public InputField nickInput;

	[Header("Match Panel")]
	public GameObject matchPanel;

	public Text matchJoinText;

	public RectTransform matchListContent;

	public GameObject matchPrefabBtn;

	[Header("Multiplayer Panel")]
	public GameObject multiplayerPanel;

	public Transform[] playerBoxes;

	public Button multiplayerStartMatch;

	public Text startCountdown;

	public GameObject chatPanel;

	public Text chatText;

	public InputField chatInput;

	private ScrollRect chatScroll;

	public GameObject configPanel;

	public GameObject configBtn;

	public InputField configSyncWindow;

	public InputField configPanicWindow;

	public InputField configRollbackWindow;

	[Header("Replay Panel")]
	public GameObject replayPanel;

	public Text replayInfoText;

	public RectTransform replayListContent;

	public GameObject replayPrefabBtn;

	private string nickname;

	private bool toStart;

	private float countDown;

	private string lobbyName;

	private string levelToLoad;

	public static Menu instance;

	private void Start()
	{
		instance = this;
		PhotonNetwork.CrcCheckEnabled = true;
		ReplayUtils.Init();
		chatScroll = chatPanel.transform.Find("ChatScroll").GetComponent<ScrollRect>();
		if (PhotonNetwork.connected)
		{
			OnReceivedRoomListUpdate();
			ActivePanel(PanelType.Match);
		}
		else
		{
			ActivePanel(PanelType.Nick);
		}
	}

	private void Update()
	{
		if (chatPanel.activeSelf && Input.GetKeyDown(KeyCode.Return))
		{
			MultiplayerPanel_ChatSend();
		}
		if (toStart)
		{
			countDown += Time.deltaTime;
			startCountdown.text = $"Match starts in {3 - Mathf.FloorToInt(countDown)}...";
			if (countDown >= 3f)
			{
				PhotonNetwork.LoadLevel(levelToLoad);
				toStart = false;
			}
		}
	}

	public void NickPanel_OkBtn()
	{
		nickname = nickInput.text;
		nickWelcomeText.text = $"Welcome {nickname}";
		nickPanel.SetActive(value: false);
		mainPanel.SetActive(value: true);
	}

	public void MainPanel_SetLobby(string config)
	{
		string[] array = config.Split(';');
		lobbyName = array[0];
		levelToLoad = array[1];
		MainPanel_MultilayerBtn();
	}

	public void MainPanel_MultilayerBtn()
	{
		PhotonNetwork.player.NickName = nickname;
		PhotonNetwork.lobby = new TypedLobby(lobbyName, LobbyType.Default);
		PhotonNetwork.ConnectUsingSettings("v1.0");
		ReplayUtils.replayContext = lobbyName;
		infoText.text = "Connecting...";
		ActivePanel(PanelType.Info);
	}

	public void MainPanel_ExitBtn()
	{
		Application.Quit();
	}

	public void MainPanel_BackBtn()
	{
		ActivePanel(PanelType.Nick);
	}

	public void MatchPanel_BackBtn()
	{
		PhotonNetwork.Disconnect();
		ActivePanel(PanelType.Main);
	}

	public void MatchPanel_NewMatchBtn()
	{
		PhotonNetwork.CreateRoom(nickname);
		infoText.text = "Creating match... " + lobbyName;
		ActivePanel(PanelType.Info);
	}

	public void MatchPanel_ReplayBtn()
	{
		List<ReplayRecordInfo> contextRecords = ReplayUtils.GetContextRecords();
		if (contextRecords.Count == 0)
		{
			replayInfoText.text = "No Replays";
		}
		else
		{
			replayInfoText.text = "Replays: " + contextRecords.Count;
		}
		foreach (Transform item in replayListContent)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		for (int i = 0; i < contextRecords.Count; i++)
		{
			ReplayRecordInfo replayRecordInfo = contextRecords[i];
			GameObject gameObject = UnityEngine.Object.Instantiate(replayPrefabBtn);
			gameObject.transform.SetParent(replayListContent, worldPositionStays: false);
			gameObject.transform.Find("DateText").GetComponent<Text>().text = replayRecordInfo.creationDate.ToString("yyyy-MM-dd");
			gameObject.transform.Find("TimeText").GetComponent<Text>().text = replayRecordInfo.creationDate.ToString("HH:mm");
			gameObject.transform.Find("PlayersText").GetComponent<Text>().text = "Players: " + replayRecordInfo.numberOfPlayers;
			gameObject.GetComponent<ReplayPicker>().replayRecord = replayRecordInfo;
			RectTransform rectTransform = gameObject.transform as RectTransform;
			rectTransform.localPosition = new Vector3((float)(i % 3) * (rectTransform.sizeDelta.x + 10f), 0f - (float)(i / 3) * (rectTransform.sizeDelta.y + 10f), 0f);
		}
		replayListContent.sizeDelta = new Vector2(replayListContent.sizeDelta.x, (float)((contextRecords.Count - 1) / 3 + 1) * (replayPrefabBtn.GetComponent<RectTransform>().sizeDelta.y + 10f));
		ActivePanel(PanelType.Replay);
	}

	public void MultiplayerPanel_BackBtn()
	{
		PhotonNetwork.LeaveRoom();
		ActivePanel(PanelType.Match);
	}

	public void MultiplayerPanel_StartMatchBtn()
	{
		PhotonNetwork.room.IsVisible = false;
		int num = int.Parse(configSyncWindow.text);
		int num2 = int.Parse(configRollbackWindow.text);
		int num3 = int.Parse(configPanicWindow.text);
		TrueSyncConfig trueSyncGlobalConfig = TrueSyncManager.TrueSyncGlobalConfig;
		if (num == trueSyncGlobalConfig.syncWindow && num2 == trueSyncGlobalConfig.rollbackWindow && num3 == trueSyncGlobalConfig.panicWindow)
		{
			num = -1;
		}
		photonView.RPC("StartMatch", PhotonTargets.All, num, num2, num3);
	}

	public void MultiplayerPanel_ChatSend()
	{
		string text = chatInput.text;
		if (text != string.Empty)
		{
			chatInput.text = string.Empty;
			int num = Array.IndexOf(PhotonNetwork.playerList, PhotonNetwork.player);
			MultiplayerPanel_ChatReceived(PhotonNetwork.playerName, text, num);
			photonView.RPC("MultiplayerPanel_ChatReceived", PhotonTargets.Others, PhotonNetwork.playerName, text, num);
			chatInput.ActivateInputField();
		}
	}

	[PunRPC]
	public void MultiplayerPanel_ChatReceived(string playerName, string text, int spawnIndex)
	{
		if (spawnIndex < 0)
		{
			spawnIndex = 0;
		}
		chatText.text += $"{playerName}: {text}\n";
		chatScroll.normalizedPosition = new Vector2(0f, 0f);
	}

	public void MultiplayerPanel_ChatToggle()
	{
		configPanel.SetActive(value: false);
		chatPanel.SetActive(!chatPanel.activeSelf);
		if (chatPanel.activeSelf)
		{
			chatInput.ActivateInputField();
			chatScroll.normalizedPosition = new Vector2(0f, 0f);
		}
	}

	public void MultiplayerPanel_ConfigToggle()
	{
		chatPanel.SetActive(value: false);
		configPanel.SetActive(!configPanel.activeSelf);
	}

	public void OptionsPanel_BackBtn()
	{
		ActivePanel(PanelType.Main);
	}

	public void ReplayPanel_BackBtn()
	{
		ActivePanel(PanelType.Match);
	}

	public void ReplayPanel_ClearBtn()
	{
		ReplayUtils.ClearAllReplayRecords();
		MatchPanel_ReplayBtn();
	}

	public void ReplayPanel_LoadLevel()
	{
		SceneManager.LoadScene(levelToLoad);
	}

	public override void OnConnectionFail(DisconnectCause cause)
	{
		ActivePanel(PanelType.Main);
	}

	public override void OnConnectedToMaster()
	{
		infoText.text = "Entering lobby...";
		PhotonNetwork.JoinLobby(PhotonNetwork.lobby);
	}

	public override void OnReceivedRoomListUpdate()
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		if (roomList.Length == 0)
		{
			matchJoinText.text = "No Matches Online";
		}
		else
		{
			matchJoinText.text = "Join Match";
		}
		int childCount = matchListContent.transform.childCount;
		if (roomList.Length >= childCount)
		{
			for (int i = 0; i < roomList.Length - childCount; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(matchPrefabBtn);
				gameObject.transform.SetParent(matchListContent, worldPositionStays: false);
			}
		}
		else
		{
			for (int j = 0; j < childCount - roomList.Length; j++)
			{
				UnityEngine.Object.Destroy(matchListContent.transform.GetChild(childCount - (j + 1)).gameObject);
			}
		}
		for (int k = 0; k < roomList.Length; k++)
		{
			MatchJoiner component = matchListContent.transform.GetChild(k).GetComponent<MatchJoiner>();
			component.UpdateRoom(roomList[k]);
			RectTransform component2 = component.GetComponent<RectTransform>();
			component2.localPosition = new Vector3(component2.localPosition.x, 0f - (float)k * component2.sizeDelta.y, 0f);
		}
		matchListContent.sizeDelta = new Vector2(matchListContent.sizeDelta.x, (float)roomList.Length * matchPrefabBtn.GetComponent<RectTransform>().sizeDelta.y);
	}

	public override void OnJoinedLobby()
	{
		ActivePanel(PanelType.Match);
	}

	public override void OnJoinedRoom()
	{
		MultiplayerUpdateVisibility();
		ActivePanel(PanelType.Multiplayer);
		UpdatePlayerList();
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		UpdatePlayerList();
	}

	public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		MultiplayerUpdateVisibility();
	}

	private void MultiplayerUpdateVisibility()
	{
		configBtn.SetActive(PhotonNetwork.isMasterClient);
		if (configBtn.activeSelf)
		{
			configSyncWindow.text = TrueSyncManager.TrueSyncGlobalConfig.syncWindow + string.Empty;
			configRollbackWindow.text = TrueSyncManager.TrueSyncGlobalConfig.rollbackWindow + string.Empty;
			configPanicWindow.text = TrueSyncManager.TrueSyncGlobalConfig.panicWindow + string.Empty;
		}
		multiplayerStartMatch.gameObject.SetActive(PhotonNetwork.isMasterClient);
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer disconnetedPlayer)
	{
		UpdatePlayerList();
	}

	public void UpdatePlayerList()
	{
		ClearPlayersGUI();
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			Transform transform = playerBoxes[i];
			transform.GetComponent<Image>().enabled = true;
			Text component = transform.Find("PlayerNameText").GetComponent<Text>();
			component.text = PhotonNetwork.playerList[i].NickName.Trim();
		}
	}

	private void ClearPlayersGUI()
	{
		Transform[] array = playerBoxes;
		foreach (Transform transform in array)
		{
			transform.GetComponent<Image>().enabled = false;
			transform.Find("PlayerNameText").GetComponent<Text>().text = string.Empty;
		}
	}

	[PunRPC]
	public void StartMatch(int syncWindow, int rollbackWindow, int panicWindow)
	{
		if (syncWindow == -1)
		{
			TrueSyncManager.TrueSyncCustomConfig = null;
		}
		else
		{
			TrueSyncManager.TrueSyncCustomConfig = ScriptableObject.CreateInstance<TrueSyncConfig>();
			TrueSyncManager.TrueSyncCustomConfig.syncWindow = syncWindow;
			TrueSyncManager.TrueSyncCustomConfig.rollbackWindow = rollbackWindow;
			TrueSyncManager.TrueSyncCustomConfig.panicWindow = panicWindow;
		}
		ReplayRecord.replayMode = ReplayMode.NO_REPLAY;
		toStart = true;
		multiplayerStartMatch.gameObject.SetActive(value: false);
		startCountdown.gameObject.SetActive(value: true);
	}

	private void ActivePanel(PanelType panelType)
	{
		infoText.gameObject.SetActive(panelType == PanelType.Info);
		mainPanel.SetActive(panelType == PanelType.Main);
		nickPanel.SetActive(panelType == PanelType.Nick);
		matchPanel.SetActive(panelType == PanelType.Match);
		multiplayerPanel.SetActive(panelType == PanelType.Multiplayer);
		replayPanel.SetActive(panelType == PanelType.Replay);
	}
}
