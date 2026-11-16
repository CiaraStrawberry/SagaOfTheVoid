using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Oculus.Platform;
using Oculus.Platform.Models;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class MainMenuNetworkingController : MonoBehaviour
{
	public TextMesh PlayersOnlinetext;

	private CrossLevelVariableHolder crossvar;

	private List<TextMesh> matchtext = new List<TextMesh>();

	public GameObject[] MultiplayerMatchDisplay = new GameObject[5];

	public GameObject[] background = new GameObject[5];

	private MeshRenderer[] backgroundrend = new MeshRenderer[5];

	public GameObject creategamecontrol;

	public GameObject creategamecontrolprivate;

	public GameObject pingdisplay;

	public GameObject namedisplay;

	public GameObject pagenumdisplay;

	public GameObject status;

	public GameObject istrans;

	private TextMesh namedisplaytext;

	private TextMesh pingtext;

	private TextMesh pagenumtext;

	private TextMesh statustext;

	public Material selectmat;

	public Material nonselectmat;

	public GameObject steamconnectionthing;

	private TextMesh steamconnectiontext;

	private int ping;

	private int timer = -1;

	private int countthrough = 1;

	private string playername;

	private string roomName;

	private bool count;

	public int gamescount;

	public int roomlistcount;

	private VRTK_SDKManager vrtk_sdkman;

	public VRTK_SDK_CONTROLLER_MANAGER steammangercheck;

	public TextMesh ToggleStatus;

	private bool friendslist;

	public List<RoomInfo> roomscurrent = new List<RoomInfo>();

	public string oculusplayername;

	private int counter;

	private bool joinedyet;

	public List<string> friendslisttemp = new List<string>();

	public GameObject Loading;

	private void OnEnable()
	{
		joinedyet = false;
	}

	private void Start()
	{
		MainMenuValueHolder component = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
		Loading.SetActive(value: false);
		if (!component.testarea && !component.tutorial && !component.campaign)
		{
			PhotonNetwork.offlineMode = false;
			PhotonNetwork.ConnectUsingSettings("1.0");
		}
		GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading = false;
		steammangercheck = GameObject.Find("VRTK_SDK").GetComponent<VRTK_SDK_CONTROLLER_MANAGER>();
		statustext = status.GetComponent<TextMesh>();
		ping = PhotonNetwork.networkingPeer.RoundTripTime;
		namedisplaytext = namedisplay.GetComponent<TextMesh>();
		InvokeRepeating("refreshrep", 0f, 1f);
		GameObject[] multiplayerMatchDisplay = MultiplayerMatchDisplay;
		foreach (GameObject gameObject in multiplayerMatchDisplay)
		{
			matchtext.Add(gameObject.GetComponent<TextMesh>());
		}
		Debug.Log("running?");
		pingtext = pingdisplay.GetComponent<TextMesh>();
		pagenumtext = pagenumdisplay.GetComponent<TextMesh>();
		for (int j = 0; j < 5; j++)
		{
			backgroundrend[j] = background[j].GetComponent<MeshRenderer>();
		}
		creategamecontrol.SetActive(value: false);
		creategamecontrolprivate.SetActive(value: false);
		steamconnectiontext = steamconnectionthing.GetComponent<TextMesh>();
		if (component.testarea || component.tutorial || component.campaign)
		{
			Loading.SetActive(value: true);
		}
	}

	private void GetLoggedInUserCallback(Message msg)
	{
		if (!msg.IsError)
		{
			PhotonNetwork.playerName = msg.GetUser().OculusID;
		}
		else
		{
			Debug.Log(msg.GetError().Message);
		}
	}

	private void GetLoggedInUserCallbackfriends(Message msg)
	{
		friendslisttemp.Clear();
		if (msg.IsError)
		{
			return;
		}
		foreach (User user in msg.GetUserList())
		{
			Debug.Log(user.OculusID);
			friendslisttemp.Add(user.OculusID);
		}
	}

	private void refreshrep()
	{
		if (PhotonNetwork.connectedAndReady)
		{
			PhotonVoiceNetwork.Connect();
			if (steammangercheck.issteamversion && SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll) > 0)
			{
				getfriendslist();
			}
			if (PhotonNetwork.playerName != null)
			{
				namedisplaytext.text = "Player Name: " + PhotonNetwork.playerName;
			}
			if (!steammangercheck.issteamversion)
			{
				steamconnectiontext.text = "Oculus Connected";
			}
			else
			{
				steamconnectiontext.text = "SteamConnection: " + SteamAPI.IsSteamRunning();
			}
			refresh();
		}
		else
		{
			if (PhotonNetwork.connectionStateDetailed != ClientState.JoinedLobby)
			{
			}
			counter++;
			if (counter == 3)
			{
				PhotonNetwork.ConnectUsingSettings("1.0");
				counter = 0;
			}
		}
		statustext.text = "status: " + PhotonNetwork.connectionStateDetailed;
		MainMenuValueHolder component = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
		MainMenuValueHolder component2 = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
		if (component.testarea || component.tutorial || component.campaign)
		{
			Loading.SetActive(value: true);
		}
		if ((component.testarea || component.tutorial || component.campaign) && !joinedyet)
		{
			joinedyet = true;
			onevone();
			Debug.Log("1v1");
		}
	}

	private void Update()
	{
		if (timer < 125)
		{
			timer++;
		}
		if (timer == 121)
		{
			Setup1();
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			twovtwo();
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			JoinRoom();
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			onevone();
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			threevthree();
		}
		if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			createprivateroom();
		}
		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			twovtwoprivate();
		}
		if (!joinedyet && PhotonNetwork.insideLobby)
		{
			OnJoinedLobby();
		}
		if (PhotonNetwork.playerName != null)
		{
			roomName = PhotonNetwork.playerName + "'s Room";
		}
	}

	public void OnJoinedLobby()
	{
		joinedyet = true;
		Debug.Log("part of: " + PhotonNetwork.networkingPeer.CloudRegion);
		Debug.Log("players connected in master: " + PhotonNetwork.countOfPlayersOnMaster);
	}

	private void Setup1()
	{
		refresh();
	}

	private void refresh()
	{
		ToggleStatus.text = "Private Lobby Toggle: " + friendslist;
		if (!steammangercheck.issteamversion)
		{
			Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
			Users.GetLoggedInUserFriends().OnComplete((Message<UserList> msg) =>
			{
				if (!msg.IsError)
				{
					foreach (User datum in msg.Data)
					{
						Debug.Log("friend: " + datum);
						friendslisttemp.Add(datum.OculusID);
					}
					return;
				}
				Debug.Log(msg.GetError().Message);
			});
		}
		else
		{
			string playerName = (PhotonNetwork.playerName = SteamFriends.GetPersonaName());
			PhotonNetwork.playerName = playerName;
		}
		if (!PhotonNetwork.connectedAndReady)
		{
			return;
		}
		if (!friendslist)
		{
			int num = roomlistcount * 5;
			if (PhotonNetwork.GetRoomList().Length == 0)
			{
				Debug.Log("..no games available..");
				for (int num2 = 0; num2 < 5; num2++)
				{
					matchtext[num2].text = "Slot empty";
				}
			}
			else
			{
				Debug.Log(PhotonNetwork.GetRoomList().Length + " games available..");
				List<RoomInfo> list = new List<RoomInfo>();
				RoomInfo[] roomList = PhotonNetwork.GetRoomList();
				foreach (RoomInfo roomInfo in roomList)
				{
					Debug.Log((string)roomInfo.CustomProperties["Fr"]);
					if ((string)roomInfo.CustomProperties["Fr"] == "false")
					{
						list.Add(roomInfo);
					}
				}
				roomscurrent = list;
				int num4 = 0;
				int num5 = 0;
				for (int num6 = 0; num6 < 5; num6++)
				{
					matchtext[num6].text = "Slot empty";
				}
				foreach (RoomInfo item in list)
				{
					if (num4 < 5 && num5 > num - 1 && num5 < num + 5)
					{
						matchtext[num4].text = item.Name + " " + item.PlayerCount + "/" + item.MaxPlayers + " state: " + (string)item.CustomProperties["st"] + " 3v3: " + (string)item.CustomProperties["3v3"];
						num4++;
					}
					num5++;
				}
			}
		}
		else
		{
			int num7 = roomlistcount * 5;
			if (PhotonNetwork.GetRoomList().Length == 0)
			{
				Debug.Log("..no games available..");
				for (int num8 = 0; num8 < 5; num8++)
				{
					matchtext[num8].text = "Slot empty";
				}
			}
			else
			{
				List<RoomInfo> list2 = new List<RoomInfo>();
				RoomInfo[] roomList2 = PhotonNetwork.GetRoomList();
				foreach (RoomInfo roomInfo2 in roomList2)
				{
					Debug.Log((string)roomInfo2.CustomProperties["Un"]);
					if ((string)roomInfo2.CustomProperties["isFriendsOnly"] == "true" && friendslisttemp.Contains((string)roomInfo2.CustomProperties["Un"]))
					{
						list2.Add(roomInfo2);
					}
				}
				roomscurrent = list2;
				int num10 = 0;
				int num11 = 0;
				for (int num12 = 0; num12 < 5; num12++)
				{
					matchtext[num12].text = "Slot empty";
				}
				foreach (RoomInfo item2 in list2)
				{
					if (num10 < 5 && num11 > num7 - 1 && num11 < num7 + 5)
					{
						matchtext[num10].text = item2.Name + " " + item2.PlayerCount + "/" + item2.MaxPlayers + " state: " + (string)item2.CustomProperties["st"] + " 3v3: " + (string)item2.CustomProperties["3v3"];
						num10++;
					}
					num11++;
				}
			}
		}
		pingtext.text = "ping: " + PhotonNetwork.networkingPeer.RoundTripTime + "ms";
		PlayersOnlinetext.text = "Current Players: " + PhotonNetwork.countOfPlayers;
		float num13 = Mathf.Ceil(PhotonNetwork.GetRoomList().Length / 6);
		if (num13 == 0f)
		{
			num13 = 1f;
		}
		int num14 = roomlistcount + 1;
		pagenumtext.text = "page: " + num14 + " out of " + num13.ToString();
	}

	private IEnumerator startwait()
	{
		yield return new WaitForSeconds(1f);
		Debug.Log("eh");
		Setup1();
	}

	public void creatematchdisplay()
	{
		creategamecontrol.SetActive(value: true);
		GameObject[] multiplayerMatchDisplay = MultiplayerMatchDisplay;
		foreach (GameObject gameObject in multiplayerMatchDisplay)
		{
			gameObject.SetActive(value: false);
		}
	}

	public void createprivateroom()
	{
		creategamecontrolprivate.SetActive(value: true);
	}

	public void onevoneninvisible()
	{
		Loading.SetActive(value: true);
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("Fr", "false");
		hashtable.Add("Un", PhotonNetwork.playerName);
		hashtable.Add("st", "in Lobby");
		hashtable.Add("3v3", "No");
		hashtable.Add("team1a", 400);
		hashtable.Add("team1b", 400);
		hashtable.Add("team1c", 400);
		hashtable.Add("team2a", 400);
		hashtable.Add("team2b", 400);
		hashtable.Add("team2c", 400);
		hashtable.Add("GameMode", 0);
		hashtable.Add("Bots", true);
		hashtable.Add("Botdif", 0);
		hashtable.Add("sky", 0);
		hashtable.Add("Scenery", 0);
		ExitGames.Client.Photon.Hashtable customRoomProperties = hashtable;
		string[] customRoomPropertiesForLobby = new string[4] { "Fr", "Un", "st", "3v3" };
		PhotonNetwork.CreateRoom(roomName, new RoomOptions
		{
			MaxPlayers = 2,
			IsOpen = false,
			IsVisible = false,
			PublishUserId = true,
			CustomRoomProperties = customRoomProperties,
			CustomRoomPropertiesForLobby = customRoomPropertiesForLobby
		}, null);
		PhotonNetwork.automaticallySyncScene = false;
		PhotonNetwork.LoadLevel(1);
		Debug.Log(PhotonNetwork.room.CustomProperties);
		Debug.Log("1v1");
	}

	public void onevone()
	{
		if (PhotonNetwork.connectedAndReady && PhotonNetwork.playerName != null && PhotonNetwork.playerName != string.Empty && !GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading)
		{
			Loading.SetActive(value: true);
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("Fr", "false");
			hashtable.Add("Un", PhotonNetwork.playerName);
			hashtable.Add("st", "in Lobby");
			hashtable.Add("3v3", "No");
			hashtable.Add("team1a", 400);
			hashtable.Add("team1b", 400);
			hashtable.Add("team1c", 400);
			hashtable.Add("team2a", 400);
			hashtable.Add("team2b", 400);
			hashtable.Add("team2c", 400);
			hashtable.Add("GameMode", 0);
			hashtable.Add("Bots", true);
			hashtable.Add("Botdif", 0);
			hashtable.Add("sky", 0);
			hashtable.Add("Scenery", 0);
			ExitGames.Client.Photon.Hashtable customRoomProperties = hashtable;
			string[] customRoomPropertiesForLobby = new string[4] { "Fr", "Un", "st", "3v3" };
			PhotonNetwork.CreateRoom(roomName, new RoomOptions
			{
				MaxPlayers = 2,
				IsOpen = true,
				IsVisible = true,
				PublishUserId = true,
				CustomRoomProperties = customRoomProperties,
				CustomRoomPropertiesForLobby = customRoomPropertiesForLobby
			}, null);
			PhotonNetwork.automaticallySyncScene = false;
			PhotonNetwork.LoadLevel(1);
			Debug.Log(PhotonNetwork.room.CustomProperties);
			Debug.Log("1v1");
		}
	}

	public void twovtwo()
	{
		if (((PhotonNetwork.connectedAndReady && PhotonNetwork.playerName != null && PhotonNetwork.playerName != string.Empty) || PhotonNetwork.offlineMode) && !GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading)
		{
			Loading.SetActive(value: true);
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("Fr", "false");
			hashtable.Add("Un", PhotonNetwork.playerName);
			hashtable.Add("st", "in Lobby");
			hashtable.Add("3v3", "No");
			hashtable.Add("team1a", 400);
			hashtable.Add("team1b", 400);
			hashtable.Add("team1c", 400);
			hashtable.Add("team2a", 400);
			hashtable.Add("team2b", 400);
			hashtable.Add("team2c", 400);
			hashtable.Add("GameMode", 0);
			hashtable.Add("Bots", true);
			hashtable.Add("Botdif", 0);
			hashtable.Add("sky", 0);
			hashtable.Add("Scenery", 0);
			ExitGames.Client.Photon.Hashtable customRoomProperties = hashtable;
			string[] customRoomPropertiesForLobby = new string[4] { "Fr", "Un", "st", "3v3" };
			PhotonNetwork.CreateRoom(roomName, new RoomOptions
			{
				MaxPlayers = 4,
				IsOpen = true,
				IsVisible = true,
				PublishUserId = true,
				CustomRoomProperties = customRoomProperties,
				CustomRoomPropertiesForLobby = customRoomPropertiesForLobby
			}, null);
			PhotonNetwork.automaticallySyncScene = false;
			PhotonNetwork.LoadLevel(1);
			Debug.Log("2v2");
		}
		else
		{
			Debug.Log("ERROR: PLAYER NOT CONNECTED AND READY");
		}
	}

	public void threevthree()
	{
		if (((PhotonNetwork.connectedAndReady && PhotonNetwork.playerName != null && PhotonNetwork.playerName != string.Empty) || PhotonNetwork.offlineMode) && !GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading)
		{
			Loading.SetActive(value: true);
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("Fr", "false");
			hashtable.Add("Un", PhotonNetwork.playerName);
			hashtable.Add("st", "in Lobby");
			hashtable.Add("3v3", "Yes");
			hashtable.Add("team1a", 400);
			hashtable.Add("team1b", 400);
			hashtable.Add("team1c", 400);
			hashtable.Add("team2a", 400);
			hashtable.Add("team2b", 400);
			hashtable.Add("team2c", 400);
			hashtable.Add("GameMode", 0);
			hashtable.Add("Bots", true);
			hashtable.Add("Botdif", 0);
			hashtable.Add("sky", 0);
			hashtable.Add("Scenery", 0);
			ExitGames.Client.Photon.Hashtable customRoomProperties = hashtable;
			string[] customRoomPropertiesForLobby = new string[4] { "Fr", "Un", "st", "3v3" };
			PhotonNetwork.CreateRoom(roomName, new RoomOptions
			{
				MaxPlayers = 4,
				IsOpen = true,
				IsVisible = true,
				PublishUserId = true,
				CustomRoomProperties = customRoomProperties,
				CustomRoomPropertiesForLobby = customRoomPropertiesForLobby
			}, null);
			PhotonNetwork.automaticallySyncScene = false;
			PhotonNetwork.LoadLevel(1);
			Debug.Log("3v3");
		}
		else
		{
			Debug.Log("ERROR: PLAYER NOT CONNECTED AND READY");
		}
	}

	public void onevoneprivate()
	{
		if (((PhotonNetwork.connectedAndReady && PhotonNetwork.playerName != null && PhotonNetwork.playerName != string.Empty) || PhotonNetwork.offlineMode) && !GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading)
		{
			Loading.SetActive(value: true);
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("Fr", "true");
			hashtable.Add("Un", PhotonNetwork.playerName);
			hashtable.Add("st", "in Lobby");
			hashtable.Add("3v3", "No");
			hashtable.Add("team1a", 400);
			hashtable.Add("team1b", 400);
			hashtable.Add("team1c", 400);
			hashtable.Add("team2a", 400);
			hashtable.Add("team2b", 400);
			hashtable.Add("team2c", 400);
			hashtable.Add("GameMode", 0);
			hashtable.Add("Bots", true);
			hashtable.Add("Botdif", 0);
			hashtable.Add("sky", 0);
			hashtable.Add("Scenery", 0);
			ExitGames.Client.Photon.Hashtable customRoomProperties = hashtable;
			string[] customRoomPropertiesForLobby = new string[4] { "Fr", "Un", "st", "3v3" };
			PhotonNetwork.CreateRoom(roomName, new RoomOptions
			{
				MaxPlayers = 2,
				IsOpen = true,
				IsVisible = true,
				PublishUserId = true,
				CustomRoomProperties = customRoomProperties,
				CustomRoomPropertiesForLobby = customRoomPropertiesForLobby
			}, null);
			PhotonNetwork.automaticallySyncScene = false;
			PhotonNetwork.LoadLevel(1);
			Debug.Log("1v1");
		}
		else
		{
			Debug.Log("ERROR: PLAYER NOT CONNECTED AND READY");
		}
	}

	public void twovtwoprivate()
	{
		if (((PhotonNetwork.connectedAndReady && PhotonNetwork.playerName != null && PhotonNetwork.playerName != string.Empty) || PhotonNetwork.offlineMode) && !GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading)
		{
			Loading.SetActive(value: true);
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("Fr", "true");
			hashtable.Add("Un", PhotonNetwork.playerName);
			hashtable.Add("st", "in Lobby");
			hashtable.Add("3v3", "No");
			hashtable.Add("team1a", 400);
			hashtable.Add("team1b", 400);
			hashtable.Add("team1c", 400);
			hashtable.Add("team2a", 400);
			hashtable.Add("team2b", 400);
			hashtable.Add("team2c", 400);
			hashtable.Add("GameMode", 0);
			hashtable.Add("Bots", true);
			hashtable.Add("Botdif", 0);
			hashtable.Add("sky", 0);
			hashtable.Add("Scenery", 0);
			ExitGames.Client.Photon.Hashtable customRoomProperties = hashtable;
			string[] customRoomPropertiesForLobby = new string[4] { "Fr", "Un", "st", "3v3" };
			PhotonNetwork.CreateRoom(roomName, new RoomOptions
			{
				MaxPlayers = 4,
				IsOpen = true,
				IsVisible = true,
				PublishUserId = true,
				CustomRoomProperties = customRoomProperties,
				CustomRoomPropertiesForLobby = customRoomPropertiesForLobby
			}, null);
			PhotonNetwork.automaticallySyncScene = false;
			PhotonNetwork.LoadLevel(1);
			Debug.Log("2v2");
		}
		else
		{
			Debug.Log("ERROR: PLAYER NOT CONNECTED AND READY");
		}
	}

	public void threevthreeprivate()
	{
		if (((PhotonNetwork.connectedAndReady && PhotonNetwork.playerName != null && PhotonNetwork.playerName != string.Empty) || PhotonNetwork.offlineMode) && !GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading)
		{
			Loading.SetActive(value: true);
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("Fr", "true");
			hashtable.Add("Un", PhotonNetwork.playerName);
			hashtable.Add("st", "in Lobby");
			hashtable.Add("3v3", "Yes");
			hashtable.Add("team1a", 400);
			hashtable.Add("team1b", 400);
			hashtable.Add("team1c", 400);
			hashtable.Add("team2a", 400);
			hashtable.Add("team2b", 400);
			hashtable.Add("team2c", 400);
			hashtable.Add("GameMode", 0);
			hashtable.Add("Bots", true);
			hashtable.Add("Botdif", 0);
			hashtable.Add("sky", 0);
			hashtable.Add("Scenery", 0);
			ExitGames.Client.Photon.Hashtable customRoomProperties = hashtable;
			string[] customRoomPropertiesForLobby = new string[4] { "Fr", "Un", "st", "3v3" };
			PhotonNetwork.CreateRoom(roomName, new RoomOptions
			{
				MaxPlayers = 4,
				IsOpen = true,
				IsVisible = true,
				PublishUserId = true,
				CustomRoomProperties = customRoomProperties,
				CustomRoomPropertiesForLobby = customRoomPropertiesForLobby
			}, null);
			PhotonNetwork.automaticallySyncScene = false;
			PhotonNetwork.LoadLevel(1);
			Debug.Log("3v3");
		}
		else
		{
			Debug.Log("ERROR: PLAYER NOT CONNECTED AND READY");
		}
	}

	public void joinroomprivate()
	{
		if (!friendslist)
		{
			friendslist = true;
		}
		else
		{
			friendslist = false;
		}
	}

	public void JoinRoom()
	{
		PhotonNetwork.JoinRoom(roomscurrent[gamescount].Name);
		Loading.SetActive(value: true);
		PhotonNetwork.LoadLevel(1);
	}

	public void button1()
	{
		PhotonNetwork.JoinRoom(roomscurrent[roomlistcount * 5].Name);
		PhotonNetwork.LoadLevel(1);
		Loading.SetActive(value: true);
	}

	public void button2()
	{
		PhotonNetwork.JoinRoom(roomscurrent[1 + roomlistcount * 5].Name);
		PhotonNetwork.LoadLevel(1);
		Loading.SetActive(value: true);
	}

	public void button3()
	{
		PhotonNetwork.JoinRoom(roomscurrent[2 + roomlistcount * 5].Name);
		PhotonNetwork.LoadLevel(1);
		Loading.SetActive(value: true);
	}

	public void button4()
	{
		PhotonNetwork.JoinRoom(roomscurrent[3 + roomlistcount * 5].Name);
		PhotonNetwork.LoadLevel(1);
		Loading.SetActive(value: true);
	}

	public void button5()
	{
		PhotonNetwork.JoinRoom(roomscurrent[4 + roomlistcount * 5].Name);
		PhotonNetwork.LoadLevel(1);
		Loading.SetActive(value: true);
	}

	public string[] getfriendslist()
	{
		friendslisttemp.Clear();
		int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);
		string[] array = new string[friendCount];
		for (int i = 0; i < friendCount; i++)
		{
			array[i] = SteamFriends.GetFriendPersonaName(SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll));
			friendslisttemp.Add(array[i]);
		}
		return array;
	}

	public void next()
	{
		if ((float)roomlistcount < Mathf.Ceil(PhotonNetwork.GetRoomList().Length / 5))
		{
			roomlistcount++;
		}
		refresh();
	}

	public void last()
	{
		if (roomlistcount > 0)
		{
			roomlistcount--;
		}
		refresh();
		Debug.Log(roomlistcount);
	}

	public void up()
	{
		if (gamescount > 0)
		{
			gamescount--;
		}
		updatecolour();
	}

	public void down()
	{
		if (gamescount < 4)
		{
			gamescount++;
		}
		updatecolour();
	}

	private void updatecolour()
	{
		MeshRenderer[] array = backgroundrend;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.material = nonselectmat;
		}
		backgroundrend[gamescount].material = selectmat;
	}

	private void returntonormal()
	{
		if (Loading != null)
		{
			Loading.SetActive(value: true);
		}
		SceneManager.LoadSceneAsync(0);
	}
}
