using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.SceneManagement;
using simple;

public class LobbyScreenController : MonoBehaviour
{
	public TextMesh StartMoneytext;

	public TextMesh Storytext;

	public TextMesh OpposingShipstext;

	public TextMesh MissionName;

	public TextMesh countdown2;

	public TextMesh MissionObjective;

	public MeshRenderer[] skyboxbuttonsrend = new MeshRenderer[5];

	private MeshRenderer[] mapbuttonsrend = new MeshRenderer[3];

	private MeshRenderer[] fleetshelfrend = new MeshRenderer[3];

	private TextMesh[] displaytext = new TextMesh[6];

	public GameObject[] display = new GameObject[6];

	public GameObject[] fleetshelf = new GameObject[3];

	public GameObject[] mapbuttons = new GameObject[3];

	public GameObject[] icons = new GameObject[6];

	public GameObject[] Ships;

	public GameObject status;

	public GameObject fleetdispaly;

	public GameObject countdownobj;

	public GameObject ping;

	public GameObject istrans;

	public GameObject playernamedisply;

	public GameObject defaultparent;

	public GameObject startbutton;

	private TextMesh statustext;

	private TextMesh fleetdisplaytext;

	private TextMesh pingtext;

	private TextMesh countdowntext;

	private TextMesh playernametext;

	private IOComponent FleetBuilder;

	public Material lightmat;

	public Material darkmat;

	private PhotonVoiceRecorder phoprec;

	private PhotonView phopview;

	public TextMesh lobbystateindicatortext;

	private int fleet;

	private int count;

	private int countdown = 10;

	private bool startrunning;

	public Material map1mat;

	public Material map2mat;

	public Material map3mat;

	private CrossLevelVariableHolder crosslevelvarholder;

	private List<PhotonVoiceRecorder> phopvoicerecoreders = new List<PhotonVoiceRecorder>();

	public CrossLevelVariableHolder.gamemode Gamemode;

	public TextMesh GameModeDisplay;

	public GameObject DefaultBot;

	public GameObject DefaultShipBot;

	public bool bots;

	private SceneSwitcherController sceneswitch;

	public GameObject RoomNameDisplaygam;

	public TextMesh RoomNameDisplay;

	public GameObject team1displaygam;

	public GameObject team2displaygam;

	public TextMesh Addbotstext;

	public GameObject ChangeRoomNumber;

	public TextMesh ChangeRoomNumberText;

	public GameObject[] panels = new GameObject[8];

	public GameObject multimatchseg;

	public GameObject campaignseg;

	public GameObject[] MenuLocations;

	public GameObject[] CameraLocations;

	public GameObject Loadingobj;

	private int wait;

	public string test;

	public List<int> team1;

	public List<int> team2;

	public List<int> team1Debug;

	public List<int> team2Debug;

	public int debugid;

	public TextMesh botdificultytext;

	public TextMesh Startmatchwriting;

	public TextMesh startmatchwriting2;

	private void Start()
	{
		campaignseg.SetActive(value: true);
		multimatchseg.SetActive(value: true);
		GameObject[] array = GameObject.FindGameObjectsWithTag("TutorialThings");
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(value: false);
		}
		GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading = false;
		if ((bool)GameObject.Find("Team1wins(Clone)"))
		{
			Object.Destroy(GameObject.Find("Team1wins(Clone)"));
		}
		if ((bool)GameObject.Find("Team2Wins(Clone)"))
		{
			Object.Destroy(GameObject.Find("Team2wins(Clone)"));
		}
		if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient && !PhotonNetwork.offlineMode)
		{
			PhotonNetwork.room.IsOpen = true;
			PhotonNetwork.room.IsVisible = true;
		}
		RoomNameDisplay = RoomNameDisplaygam.GetComponent<TextMesh>();
		crosslevelvarholder = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
		crosslevelvarholder.wipebots();
		int num = 0;
		GameObject[] array2 = mapbuttons;
		foreach (GameObject gameObject2 in array2)
		{
			mapbuttonsrend[num] = gameObject2.GetComponent<MeshRenderer>();
			num++;
		}
		phopview = GetComponent<PhotonView>();
		FleetBuilder = IORoot.findIO("fleet1");
		statustext = status.GetComponent<TextMesh>();
		int num2 = 0;
		GameObject[] array3 = display;
		foreach (GameObject gameObject3 in array3)
		{
			displaytext[num2] = gameObject3.GetComponent<TextMesh>();
			num2++;
		}
		InvokeRepeating("refresh", 0f, 1f);
		pingtext = ping.GetComponent<TextMesh>();
		countdowntext = countdownobj.GetComponent<TextMesh>();
		countdowntext.text = countdown.ToString();
		countdown2.text = countdown.ToString();
		InvokeRepeating("secondrep", 1f, 1f);
		startrunning = false;
		playernametext = playernamedisply.GetComponent<TextMesh>();
		if ((bool)GameObject.Find("VRTK_SDK"))
		{
			sceneswitch = GameObject.Find("VRTK_SDK").GetComponent<SceneSwitcherController>();
		}
		FleetBuilder.read();
		PhotonVoiceSettings.Instance.DebugInfo = true;
		Save_current_Ship(DefaultBot);
		Save_current_Fleet(DefaultShipBot);
		if (PhotonNetwork.connectedAndReady)
		{
			OnJoinedRoom();
		}
		if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
		{
			Hashtable customProperties = PhotonNetwork.room.CustomProperties;
			customProperties["st"] = "in Lobby";
			PhotonNetwork.room.SetCustomProperties(customProperties);
		}
		if (!PhotonNetwork.offlineMode)
		{
			ChangeRoomNumber.SetActive(value: false);
		}
		else
		{
			UpdateMaxPlayerDisplay();
		}
		if (crosslevelvarholder.campaign)
		{
			StartMoneytext.text = crosslevelvarholder.campaignlevel.startmoney.ToString();
			OpposingShipstext.text = string.Empty;
			MissionName.text = crosslevelvarholder.campaignlevel.name;
			Storytext.text = crosslevelvarholder.campaignlevel.story;
			MissionObjective.text = getstringfromobjective(crosslevelvarholder.campaignlevel.objective);
			int[] ships = crosslevelvarholder.campaignlevel.ships;
			foreach (int i2 in ships)
			{
				OpposingShipstext.text = OpposingShipstext.text + "\n" + MainMenuCampaignControlScript.getshipnamebynumber(i2);
			}
			GameObject[] array4 = panels;
			foreach (GameObject gameObject4 in array4)
			{
				gameObject4.SetActive(value: false);
			}
			panels[crosslevelvarholder.campaignlevel.panelnum].SetActive(value: true);
		}
		if (crosslevelvarholder.tutorial)
		{
			startrunning = true;
			countdown = 3;
			Loadingobj.SetActive(value: true);
		}
	}

	private string getstringfromobjective(MainMenuCampaignControlScript.eMissionObjective input)
	{
		return input switch
		{
			MainMenuCampaignControlScript.eMissionObjective.Destroyall => "Destroy all", 
			MainMenuCampaignControlScript.eMissionObjective.killTarget => "Kill Target", 
			_ => "Survive", 
		};
	}

	private void setmapclientbasedonint(int input)
	{
		if (input == 0)
		{
			directlysetmap(CrossLevelVariableHolder.mapcon.map1);
		}
		if (input == 1)
		{
			directlysetmap(CrossLevelVariableHolder.mapcon.map2);
		}
		if (input == 2)
		{
			directlysetmap(CrossLevelVariableHolder.mapcon.map3);
		}
	}

	private void setskyboxclientbasedonint(int input)
	{
		if (input == 0)
		{
			directlysetskybox(CrossLevelVariableHolder.skyboxcon.skybox1);
		}
		if (input == 1)
		{
			directlysetskybox(CrossLevelVariableHolder.skyboxcon.skybox2);
		}
		if (input == 2)
		{
			directlysetskybox(CrossLevelVariableHolder.skyboxcon.skybox3);
		}
		if (input == 3)
		{
			directlysetskybox(CrossLevelVariableHolder.skyboxcon.skybox4);
		}
		if (input == 4)
		{
			directlysetskybox(CrossLevelVariableHolder.skyboxcon.skybox5);
		}
	}

	private void setbotdifficultybasedonint(int botdifficulty)
	{
		if (botdifficulty == 0)
		{
			crosslevelvarholder.botdifficulty = CrossLevelVariableHolder.BotDifficultyhol.easy;
		}
		if (botdifficulty == 1)
		{
			crosslevelvarholder.botdifficulty = CrossLevelVariableHolder.BotDifficultyhol.medium;
		}
		if (botdifficulty == 2)
		{
			crosslevelvarholder.botdifficulty = CrossLevelVariableHolder.BotDifficultyhol.hard;
		}
		if (crosslevelvarholder.botdifficulty == CrossLevelVariableHolder.BotDifficultyhol.medium)
		{
			botdificultytext.text = "Bot Difficulty = medium";
		}
		else if (crosslevelvarholder.botdifficulty == CrossLevelVariableHolder.BotDifficultyhol.hard)
		{
			botdificultytext.text = "Bot Difficulty = hard";
		}
		else if (crosslevelvarholder.botdifficulty == CrossLevelVariableHolder.BotDifficultyhol.easy)
		{
			botdificultytext.text = "Bot Difficulty = easy";
		}
	}

	private void setgamemodebasedonint(int input)
	{
		if (input == 0)
		{
			Gamemode = CrossLevelVariableHolder.gamemode.TeamDeathMatch;
			crosslevelvarholder.Gamemode = CrossLevelVariableHolder.gamemode.TeamDeathMatch;
		}
		if (input == 1)
		{
			Gamemode = CrossLevelVariableHolder.gamemode.FreeForAll;
			crosslevelvarholder.Gamemode = CrossLevelVariableHolder.gamemode.FreeForAll;
		}
		if (input == 2)
		{
			Gamemode = CrossLevelVariableHolder.gamemode.CapitalShip;
			crosslevelvarholder.Gamemode = CrossLevelVariableHolder.gamemode.CapitalShip;
		}
		if ((bool)GameModeDisplay)
		{
			GameModeDisplay.text = Gamemode.ToString();
		}
	}

	private void setbotsbasedonbool(bool input)
	{
		crosslevelvarholder.bots = input;
		bots = input;
		if (!crosslevelvarholder.bots)
		{
			Addbotstext.text = "add bots";
		}
		else
		{
			Addbotstext.text = "Remove bots";
		}
	}

	public List<int> getplayerlistinorder(List<PhotonPlayer> input)
	{
		List<int> list = new List<int>();
		foreach (PhotonPlayer item in input)
		{
			list.Add(item.ID);
		}
		list.Sort();
		return list;
	}

	private void setLobbyPosition(int a)
	{
		GameObject gameObject = GameObject.Find("VRTK_SDK");
		transform.position = MenuLocations[a].transform.position;
		transform.rotation = MenuLocations[a].transform.rotation;
		gameObject.transform.position = CameraLocations[a].transform.position;
		gameObject.transform.rotation = CameraLocations[a].transform.rotation;
		gameObject.transform.Find("ResizeObj").transform.localPosition = new Vector3(0.119999f, 4.86f, 5.71f);
	}

	private void OnPhotonPlayerConnected(PhotonPlayer other)
	{
		List<int> list = getplayerlistinorder(new List<PhotonPlayer>(PhotonNetwork.playerList));
		setLobbyPosition(list.IndexOf(PhotonNetwork.player.ID));
	}

	private void OnJoinedRoom()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			startbutton.SetActive(value: false);
		}
		else
		{
			startbutton.SetActive(value: true);
		}
		Hashtable customProperties = PhotonNetwork.player.CustomProperties;
		SwitchBotDificulty();
		List<int> list = getplayerlistinorder(new List<PhotonPlayer>(PhotonNetwork.playerList));
		setLobbyPosition(list.IndexOf(PhotonNetwork.player.ID));
		if (PhotonNetwork.isMasterClient && getteam1().Count == 0)
		{
			crosslevelvarholder.AsignTeam(PhotonNetwork.player.ID, GetStartTeam());
		}
		Gamemode = CrossLevelVariableHolder.gamemode.CapitalShip;
		MainMenuValueHolder component = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
		if (!crosslevelvarholder.campaign)
		{
			crosslevelvarholder.campaign = component.campaign;
			crosslevelvarholder.campaignlevel = component.selectedcampaign;
			if (component.selectedcampaign != null)
			{
				crosslevelvarholder.map = component.selectedcampaign.map;
			}
			if (component.selectedcampaign != null)
			{
				crosslevelvarholder.skybox = component.selectedcampaign.skybox;
			}
		}
		crosslevelvarholder.TestArea = component.testarea;
		crosslevelvarholder.tutorial = component.tutorial;
		if (crosslevelvarholder.campaign)
		{
			crosslevelvarholder.skybox = crosslevelvarholder.campaignlevel.skybox;
			crosslevelvarholder.map = crosslevelvarholder.campaignlevel.map;
		}
		if (crosslevelvarholder.campaign)
		{
			pseudodisable(multimatchseg);
		}
		else
		{
			pseudodisable(campaignseg);
		}
		if (PhotonNetwork.isMasterClient && !crosslevelvarholder.campaign && !crosslevelvarholder.tutorial)
		{
			SwitchMode();
			float value = Random.value;
			if (value < 0.33f)
			{
				map1();
			}
			if (value > 0.33f && value < 0.66f)
			{
				map2();
			}
			if (value > 0.66f)
			{
				map3();
			}
			float value2 = Random.value;
			if (value2 < 0.2f)
			{
				skybox1();
			}
			if (value2 > 0.2f && value2 < 0.4f)
			{
				skybox2();
			}
			if (value2 > 0.4f && value2 < 0.6f)
			{
				skybox3();
			}
			if (value2 > 0.6f && value2 < 0.8f)
			{
				skybox4();
			}
			if (value2 > 0.8f)
			{
				skybox5();
			}
		}
		else if (crosslevelvarholder.campaign)
		{
			switch (crosslevelvarholder.map)
			{
			case CrossLevelVariableHolder.mapcon.map1:
				map1client();
				break;
			case CrossLevelVariableHolder.mapcon.map2:
				map2client();
				break;
			case CrossLevelVariableHolder.mapcon.map3:
				map3client();
				break;
			}
			switch (crosslevelvarholder.skybox)
			{
			case CrossLevelVariableHolder.skyboxcon.skybox1:
				skybox1client();
				break;
			case CrossLevelVariableHolder.skyboxcon.skybox2:
				skybox2client();
				break;
			case CrossLevelVariableHolder.skyboxcon.skybox3:
				skybox3client();
				break;
			case CrossLevelVariableHolder.skyboxcon.skybox4:
				skybox4client();
				break;
			case CrossLevelVariableHolder.skyboxcon.skybox5:
				skybox5client();
				break;
			}
		}
		else if (crosslevelvarholder.tutorial)
		{
			map1client();
			skybox3client();
		}
		Loadingobj.SetActive(value: false);
		refresh();
		if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers == 4)
		{
			ChangeRoomNumber.SetActive(value: true);
		}
		UpdateMaxPlayerDisplay();
	}

	public void pseudodisable(GameObject disableobj)
	{
		MeshRenderer[] componentsInChildren = disableobj.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			meshRenderer.enabled = false;
		}
		TextMesh[] componentsInChildren2 = disableobj.GetComponentsInChildren<TextMesh>();
		foreach (TextMesh textMesh in componentsInChildren2)
		{
			textMesh.characterSize = 0f;
		}
		BoxCollider[] componentsInChildren3 = disableobj.GetComponentsInChildren<BoxCollider>();
		foreach (BoxCollider boxCollider in componentsInChildren3)
		{
			boxCollider.enabled = false;
		}
	}

	private int GetStartTeam()
	{
		int result = 1;
		if (PhotonNetwork.inRoom)
		{
			if (getteam1().Count > getteam2().Count)
			{
				result = 2;
			}
			else if (getteam2().Count > getteam1().Count)
			{
				result = 1;
			}
			else if (getteam1().Count == getteam2().Count)
			{
				int num = PhotonNetwork.room.MaxPlayers;
				if (num == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
				{
					num = 6;
				}
				int num2 = num / 2;
				result = ((getteam1().Count < num2) ? 1 : 2);
			}
		}
		return result;
	}

	public void ChangePlayerNumber()
	{
		if (PhotonNetwork.offlineMode)
		{
			if (PhotonNetwork.room.MaxPlayers == 2)
			{
				PhotonNetwork.room.MaxPlayers = 4;
				Hashtable customProperties = PhotonNetwork.room.CustomProperties;
				customProperties["3v3"] = "No";
				PhotonNetwork.room.SetCustomProperties(customProperties);
			}
			else if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "No")
			{
				PhotonNetwork.room.MaxPlayers = 4;
				Hashtable customProperties2 = PhotonNetwork.room.CustomProperties;
				customProperties2["3v3"] = "Yes";
				PhotonNetwork.room.SetCustomProperties(customProperties2);
			}
			else if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
			{
				PhotonNetwork.room.MaxPlayers = 2;
				Hashtable customProperties3 = PhotonNetwork.room.CustomProperties;
				customProperties3["3v3"] = "No";
				PhotonNetwork.room.SetCustomProperties(customProperties3);
			}
		}
		else if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "No")
		{
			PhotonNetwork.room.MaxPlayers = 4;
			Hashtable customProperties4 = PhotonNetwork.room.CustomProperties;
			customProperties4["3v3"] = "Yes";
			PhotonNetwork.room.SetCustomProperties(customProperties4);
		}
		else if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
		{
			PhotonNetwork.room.MaxPlayers = 4;
			Hashtable customProperties5 = PhotonNetwork.room.CustomProperties;
			customProperties5["3v3"] = "No";
			PhotonNetwork.room.SetCustomProperties(customProperties5);
		}
		UpdateMaxPlayerDisplay();
		refresh();
	}

	private void UpdateMaxPlayerDisplay()
	{
		if (PhotonNetwork.room != null)
		{
			if (PhotonNetwork.room.MaxPlayers == 2)
			{
				ChangeRoomNumberText.text = "one v one";
			}
			else if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "No")
			{
				ChangeRoomNumberText.text = "two v two";
			}
			else if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
			{
				ChangeRoomNumberText.text = "three v three";
			}
		}
	}

	public void SwitchMode()
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch)
			{
				Gamemode = CrossLevelVariableHolder.gamemode.FreeForAll;
			}
			else if (Gamemode == CrossLevelVariableHolder.gamemode.FreeForAll)
			{
				Gamemode = CrossLevelVariableHolder.gamemode.CapitalShip;
			}
			else if (Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip)
			{
				Gamemode = CrossLevelVariableHolder.gamemode.TeamDeathMatch;
			}
			else
			{
				Gamemode = CrossLevelVariableHolder.gamemode.TeamDeathMatch;
			}
			if (Gamemode == CrossLevelVariableHolder.gamemode.FreeForAll)
			{
				team1displaygam.SetActive(value: false);
				team2displaygam.SetActive(value: false);
			}
			else
			{
				team1displaygam.SetActive(value: true);
				team2displaygam.SetActive(value: true);
			}
			Hashtable customProperties = PhotonNetwork.room.CustomProperties;
			customProperties["GameMode"] = (int)Gamemode;
			PhotonNetwork.room.SetCustomProperties(customProperties);
			refresh();
		}
	}

	public int gettargetteam()
	{
		int result = 1;
		int num = PhotonNetwork.room.MaxPlayers;
		if (num == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
		{
			num = 6;
		}
		int num2 = num / 2;
		if (getteam1().Contains(PhotonNetwork.player.ID) && getteam2().Count < num2)
		{
			result = 2;
		}
		if (getteam2().Contains(PhotonNetwork.player.ID) && getteam1().Count < num2)
		{
			result = 1;
		}
		return result;
	}

	public void SwitchTeam()
	{
		Debug.Log(PhotonNetwork.player.ID + " " + gettargetteam());
		crosslevelvarholder.AsignTeam(PhotonNetwork.player.ID, gettargetteam());
		refresh();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			StartMatch();
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			map1();
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			skybox4();
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			skybox5();
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			SwitchBotDificulty();
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			SwitchTeam();
		}
		if (Input.GetKeyDown(KeyCode.I))
		{
			SwitchMode();
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			map1();
		}
		GameObject[] array = icons;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(value: false);
		}
		List<int> list = getplayerlistinorder(new List<PhotonPlayer>(PhotonNetwork.playerList));
		if (!PhotonNetwork.offlineMode)
		{
			test = PhotonVoiceNetwork.CurrentRoomName;
			GameObject[] array2 = GameObject.FindGameObjectsWithTag("Voice");
			foreach (GameObject gameObject2 in array2)
			{
				if (!gameObject2.GetComponent<PhotonVoiceRecorder>().IsTransmitting && !gameObject2.GetComponent<PhotonVoiceSpeaker>().IsPlaying)
				{
					continue;
				}
				int iD = gameObject2.GetComponent<PhotonView>().owner.ID;
				int num = 1;
				if (getteam1().Contains(iD))
				{
					int num2 = getteam1().IndexOf(iD);
					if (num2 == 0)
					{
						num = 0;
					}
					if (num2 == 1)
					{
						num = 1;
					}
					if (num2 == 2)
					{
						num = 2;
					}
				}
				if (getteam2().Contains(iD))
				{
					int num3 = getteam2().IndexOf(iD);
					if (num3 == 0)
					{
						num = 3;
					}
					if (num3 == 1)
					{
						num = 4;
					}
					if (num3 == 2)
					{
						num = 5;
					}
				}
				icons[num].SetActive(value: true);
			}
		}
		wait++;
		if (wait == 200 && !PhotonNetwork.inRoom)
		{
			PhotonNetwork.LoadLevel(0);
		}
	}

	private void secondrep()
	{
		if (startrunning)
		{
			countdown--;
			if (countdown > -1)
			{
				countdowntext.text = countdown.ToString();
				countdown2.text = countdown.ToString();
			}
			if (countdown == 0 && PhotonNetwork.isMasterClient)
			{
				startmatchactual();
			}
		}
	}

	public static List<int> getteaminput(string input)
	{
		if (PhotonNetwork.room != null)
		{
			Hashtable customProperties = PhotonNetwork.room.CustomProperties;
			List<int> list = new List<int>();
			if ((int)customProperties[input + "a"] != 400)
			{
				list.Add((int)customProperties[input + "a"]);
			}
			if ((int)customProperties[input + "b"] != 400)
			{
				list.Add((int)customProperties[input + "b"]);
			}
			if ((int)customProperties[input + "c"] != 400)
			{
				list.Add((int)customProperties[input + "c"]);
			}
			return list;
		}
		return null;
	}

	public static List<int> getteam1()
	{
		return getteaminput("team1");
	}

	public static List<int> getteam2()
	{
		return getteaminput("team2");
	}

	public static void setteaminput(List<int> teamsset, string targetteam)
	{
		if (PhotonNetwork.room != null)
		{
			Hashtable customProperties = PhotonNetwork.room.CustomProperties;
			if (teamsset.Count > 0)
			{
				customProperties[targetteam + "a"] = teamsset[0];
			}
			else
			{
				customProperties[targetteam + "a"] = 400;
			}
			if (teamsset.Count > 1)
			{
				customProperties[targetteam + "b"] = teamsset[1];
			}
			else
			{
				customProperties[targetteam + "b"] = 400;
			}
			if (teamsset.Count > 2)
			{
				customProperties[targetteam + "c"] = teamsset[2];
			}
			else
			{
				customProperties[targetteam + "c"] = 400;
			}
			PhotonNetwork.room.SetCustomProperties(customProperties);
		}
	}

	public static void setteam1(List<int> teamsset)
	{
		setteaminput(teamsset, "team1");
	}

	public static void setteam2(List<int> teamsset)
	{
		setteaminput(teamsset, "team2");
	}

	public static string getstartkeystring(int input)
	{
		if (input == 1)
		{
			return "team1";
		}
		return "team2";
	}

	private void refresh()
	{
		if (PhotonNetwork.room == null)
		{
			return;
		}
		if (!PhotonNetwork.isMasterClient)
		{
			startbutton.SetActive(value: false);
		}
		else
		{
			startbutton.SetActive(value: true);
		}
		team1 = getteam1();
		team2 = getteam2();
		foreach (int item in getteam1())
		{
			if (PhotonPlayer.Find(item) == null && item != 300)
			{
				crosslevelvarholder.removetoteam(item, "team1");
			}
		}
		foreach (int item2 in getteam2())
		{
			if (PhotonPlayer.Find(item2) == null && item2 != 300)
			{
				crosslevelvarholder.removetoteam(item2, "team2");
			}
		}
		if (!getteam1().Contains(PhotonNetwork.player.ID) && !getteam2().Contains(PhotonNetwork.player.ID))
		{
			crosslevelvarholder.addtoteam(PhotonNetwork.player.ID, getstartkeystring(GetStartTeam()));
		}
		debugid = PhotonNetwork.player.ID;
		team1Debug = getteam1();
		team2Debug = getteam2();
		playernametext.text = "player name: " + PhotonNetwork.playerName;
		pingtext.text = "ping: " + PhotonNetwork.networkingPeer.RoundTripTime + "ms";
		statustext.text = "status: " + PhotonNetwork.connectionState;
		RoomNameDisplay.text = "Room name: " + PhotonNetwork.room.Name;
		GameObject[] array = icons;
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetActive())
			{
				gameObject.SetActive(value: false);
			}
		}
		if (!crosslevelvarholder.campaign)
		{
			setgamemodebasedonint((int)PhotonNetwork.room.CustomProperties["GameMode"]);
			setbotsbasedonbool((bool)PhotonNetwork.room.CustomProperties["Bots"]);
			setbotdifficultybasedonint((int)PhotonNetwork.room.CustomProperties["Botdif"]);
			setskyboxclientbasedonint((int)PhotonNetwork.room.CustomProperties["sky"]);
			setmapclientbasedonint((int)PhotonNetwork.room.CustomProperties["Scenery"]);
		}
		if (PhotonNetwork.inRoom)
		{
			TextMesh[] array2 = displaytext;
			foreach (TextMesh textMesh in array2)
			{
				textMesh.text = string.Empty;
			}
			if (PhotonNetwork.room.MaxPlayers == 2)
			{
				displaytext[0].text = getplayername(0, 1);
				displaytext[1].text = getplayername(0, 2);
			}
			if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "No")
			{
				displaytext[0].text = getplayername(0, 1);
				displaytext[1].text = getplayername(0, 2);
				displaytext[2].text = getplayername(1, 1);
				displaytext[3].text = getplayername(1, 2);
			}
			if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
			{
				displaytext[0].text = getplayername(0, 1);
				displaytext[1].text = getplayername(0, 2);
				displaytext[2].text = getplayername(1, 1);
				displaytext[3].text = getplayername(1, 2);
				displaytext[4].text = getplayername(2, 1);
				displaytext[5].text = getplayername(2, 2);
			}
			PhotonPlayer[] playerList = PhotonNetwork.playerList;
			foreach (PhotonPlayer photonPlayer in playerList)
			{
				Debug.Log(photonPlayer.ID);
			}
			lobbystateindicatortext.text = "lobby state : " + (string)PhotonNetwork.room.CustomProperties["st"];
		}
		List<string> list = loadships(fleet);
		string text = string.Empty;
		foreach (string item3 in list)
		{
			if (item3 != "nothing")
			{
				text = text + "\n" + item3;
			}
		}
	}

	public string getplayername(int index, int team)
	{
		string result = string.Empty;
		if (crosslevelvarholder.bots)
		{
			result = "bot";
		}
		if (index != 300)
		{
			if (team == 1 && getteam1().Count > index && PhotonPlayer.Find(getteam1()[index]) != null)
			{
				result = PhotonPlayer.Find(getteam1()[index]).NickName;
			}
			if (team == 2 && getteam2().Count > index && PhotonPlayer.Find(getteam2()[index]) != null)
			{
				result = PhotonPlayer.Find(getteam2()[index]).NickName;
			}
		}
		return result;
	}

	public void StartMatch()
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (!startrunning)
			{
				phopview.RPC("StartMatchclient", PhotonTargets.AllViaServer);
				Startmatchwriting.text = "     Reset";
				startmatchwriting2.text = "     Reset";
			}
			else if (countdown > 2)
			{
				phopview.RPC("stopstartmatchclient", PhotonTargets.All);
				Startmatchwriting.text = "Start Match";
				startmatchwriting2.text = "Start Match";
			}
		}
	}

	[PunRPC]
	public void stopstartmatchclient()
	{
		startrunning = false;
		countdown = 10;
		countdowntext.text = countdown.ToString();
		countdown2.text = countdown.ToString();
	}

	[PunRPC]
	public void StartMatchclient()
	{
		startrunning = true;
	}

	public void SwitchBotDificulty()
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (crosslevelvarholder.botdifficulty == CrossLevelVariableHolder.BotDifficultyhol.easy)
			{
				crosslevelvarholder.botdifficulty = CrossLevelVariableHolder.BotDifficultyhol.medium;
			}
			else if (crosslevelvarholder.botdifficulty == CrossLevelVariableHolder.BotDifficultyhol.medium)
			{
				crosslevelvarholder.botdifficulty = CrossLevelVariableHolder.BotDifficultyhol.hard;
			}
			else if (crosslevelvarholder.botdifficulty == CrossLevelVariableHolder.BotDifficultyhol.hard)
			{
				crosslevelvarholder.botdifficulty = CrossLevelVariableHolder.BotDifficultyhol.easy;
			}
			Hashtable customProperties = PhotonNetwork.room.CustomProperties;
			customProperties["Botdif"] = (int)crosslevelvarholder.botdifficulty;
			PhotonNetwork.room.SetCustomProperties(customProperties);
			refresh();
		}
	}

	public void RemoveBots()
	{
		if (PhotonNetwork.isMasterClient)
		{
			RemoveBotsClient();
		}
	}

	[PunRPC]
	public void RemoveBotsClient()
	{
		if (crosslevelvarholder.bots)
		{
			crosslevelvarholder.bots = false;
		}
		else
		{
			crosslevelvarholder.bots = true;
		}
		Hashtable customProperties = PhotonNetwork.room.CustomProperties;
		customProperties["Bots"] = crosslevelvarholder.bots;
		PhotonNetwork.room.SetCustomProperties(customProperties);
		refresh();
	}

	public void Quit()
	{
		PhotonNetwork.LeaveRoom();
		SceneManager.LoadScene(0);
	}

	private void startmatchactual()
	{
		if (PhotonNetwork.isMasterClient)
		{
			Hashtable customProperties = PhotonNetwork.room.CustomProperties;
			customProperties["st"] = "in Game";
			PhotonNetwork.room.SetCustomProperties(customProperties);
			phopview.RPC("startmatchactualclient", PhotonTargets.AllViaServer);
		}
	}

	[PunRPC]
	public void startmatchactualclient()
	{
		if (!PhotonNetwork.offlineMode)
		{
			PhotonNetwork.room.IsOpen = true;
			PhotonNetwork.room.IsVisible = true;
		}
		Loadingobj.SetActive(value: true);
		PhotonNetwork.LoadLevel(3);
	}

	private List<string> loadships(int fleet)
	{
		string key = "AttachPointsList" + fleet + "num";
		string text = "AttachPointsList" + fleet;
		List<string> list = new List<string>();
		int num = FleetBuilder.get<int>(key);
		if (num == 0)
		{
			if (Ships.Length < fleet - 1)
			{
				Save_current_Ship(defaultparent.transform.Find(Ships[fleet - 1].name).gameObject);
			}
			else
			{
				for (int i = 0; i < num; i++)
				{
					list.Add(FleetBuilder.get<string>(text + i));
				}
			}
		}
		return list;
	}

	public void map1()
	{
		if (PhotonNetwork.isMasterClient)
		{
			map1client();
		}
	}

	public void map2()
	{
		if (PhotonNetwork.isMasterClient)
		{
			map2client();
		}
	}

	public void map3()
	{
		if (PhotonNetwork.isMasterClient)
		{
			map3client();
		}
	}

	public void skybox1()
	{
		if (PhotonNetwork.isMasterClient)
		{
			skybox1client();
		}
	}

	public void skybox2()
	{
		if (PhotonNetwork.isMasterClient)
		{
			skybox2client();
		}
	}

	public void skybox3()
	{
		if (PhotonNetwork.isMasterClient)
		{
			skybox3client();
		}
	}

	public void skybox4()
	{
		if (PhotonNetwork.isMasterClient)
		{
			skybox4client();
		}
	}

	public void skybox5()
	{
		if (PhotonNetwork.isMasterClient)
		{
			skybox5client();
		}
	}

	[PunRPC]
	private void map1client()
	{
		changemapselected(CrossLevelVariableHolder.mapcon.map1);
	}

	[PunRPC]
	private void map2client()
	{
		changemapselected(CrossLevelVariableHolder.mapcon.map2);
	}

	[PunRPC]
	private void map3client()
	{
		changemapselected(CrossLevelVariableHolder.mapcon.map3);
	}

	[PunRPC]
	private void skybox1client()
	{
		changeskyboxrendselected(CrossLevelVariableHolder.skyboxcon.skybox1);
	}

	[PunRPC]
	private void skybox2client()
	{
		changeskyboxrendselected(CrossLevelVariableHolder.skyboxcon.skybox2);
	}

	[PunRPC]
	private void skybox3client()
	{
		changeskyboxrendselected(CrossLevelVariableHolder.skyboxcon.skybox3);
	}

	[PunRPC]
	private void skybox4client()
	{
		changeskyboxrendselected(CrossLevelVariableHolder.skyboxcon.skybox4);
	}

	[PunRPC]
	private void skybox5client()
	{
		changeskyboxrendselected(CrossLevelVariableHolder.skyboxcon.skybox5);
	}

	private void directlysetmap(CrossLevelVariableHolder.mapcon maptoset)
	{
		for (int i = 0; i < 3; i++)
		{
			if (i == (int)maptoset)
			{
				mapbuttonsrend[i].material = lightmat;
			}
			else
			{
				mapbuttonsrend[i].material = darkmat;
			}
		}
		crosslevelvarholder.map = maptoset;
	}

	private void changemapselected(CrossLevelVariableHolder.mapcon maptoset)
	{
		Hashtable customProperties = PhotonNetwork.room.CustomProperties;
		customProperties["Scenery"] = (int)maptoset;
		PhotonNetwork.room.SetCustomProperties(customProperties);
		refresh();
	}

	private void directlysetskybox(CrossLevelVariableHolder.skyboxcon skyboxtoset)
	{
		for (int i = 0; i < 5; i++)
		{
			if (i == (int)skyboxtoset)
			{
				skyboxbuttonsrend[i].material = lightmat;
			}
			else
			{
				skyboxbuttonsrend[i].material = darkmat;
			}
		}
		crosslevelvarholder.skybox = skyboxtoset;
	}

	private void changeskyboxrendselected(CrossLevelVariableHolder.skyboxcon skyboxtoset)
	{
		Hashtable customProperties = PhotonNetwork.room.CustomProperties;
		customProperties["sky"] = (int)skyboxtoset;
		PhotonNetwork.room.SetCustomProperties(customProperties);
		refresh();
	}

	private void Save_current_Ship(GameObject saveobject)
	{
		int num = 0;
		List<GameObject> list = new List<GameObject>();
		foreach (Transform item in saveobject.transform)
		{
			if (item.childCount > 0)
			{
				list.Add(item.GetChild(0).gameObject);
			}
			else
			{
				list.Add(null);
			}
		}
		foreach (GameObject item2 in list)
		{
			if (item2 != null)
			{
				string key = saveobject.name + num;
				FleetBuilder.add(key, list[num].name);
				Vector3 localPosition = list[num].transform.localPosition;
				Vector3 localEulerAngles = list[num].transform.localEulerAngles;
				FleetBuilder.add(saveobject.name + "parent" + num, item2.transform.parent.name);
				FleetBuilder.write();
				FleetBuilder.add(saveobject.name + "num", num + 1);
				FleetBuilder.add(saveobject.name + "scalex" + num, item2.transform.localScale.x);
				FleetBuilder.add(saveobject.name + "scaley" + num, item2.transform.localScale.y);
				FleetBuilder.add(saveobject.name + "scalez" + num, item2.transform.localScale.z);
			}
			else
			{
				string key2 = saveobject.name + num;
				FleetBuilder.add(key2, "nothing");
			}
			FleetBuilder.write();
			num++;
		}
	}

	private void Save_current_Fleet(GameObject input)
	{
		foreach (Transform item in input.transform)
		{
			List<GameObject> list = new List<GameObject>();
			int num = 0;
			list.Clear();
			if (item.transform.Find("Weapons").childCount > 0 && item.transform.Find("Weapons").Find("Attachpoint 1").childCount > 0)
			{
				list.Add(item.transform.Find("Weapons").Find("Attachpoint 1").GetChild(0)
					.gameObject);
				}
				else
				{
					list.Add(null);
				}
				if (item.transform.Find("Weapons").childCount > 1 && item.transform.Find("Weapons").Find("Attachpoint 2").childCount > 0)
				{
					list.Add(item.transform.Find("Weapons").Find("Attachpoint 2").GetChild(0)
						.gameObject);
					}
					else
					{
						list.Add(null);
					}
					if (item.transform.Find("Weapons").childCount > 2 && item.transform.Find("Weapons").Find("Attachpoint 3").childCount > 0)
					{
						list.Add(item.transform.Find("Weapons").Find("Attachpoint 3").GetChild(0)
							.gameObject);
						}
						else
						{
							list.Add(null);
						}
						if (item.transform.Find("Weapons").childCount > 3 && item.transform.Find("Weapons").Find("Attachpoint 4").childCount > 0)
						{
							list.Add(item.transform.Find("Weapons").Find("Attachpoint 4").GetChild(0)
								.gameObject);
							}
							else
							{
								list.Add(null);
							}
							if (item.transform.Find("Weapons").childCount > 4 && item.transform.Find("Weapons").Find("Attachpoint 5").childCount > 0)
							{
								list.Add(item.transform.Find("Weapons").Find("Attachpoint 5").GetChild(0)
									.gameObject);
								}
								else
								{
									list.Add(null);
								}
								foreach (GameObject item2 in list)
								{
									if (item2 != null)
									{
										string key = item.name + "bot" + num;
										FleetBuilder.add(key, list[num].name);
										Vector3 localPosition = list[num].transform.localPosition;
										Vector3 localEulerAngles = list[num].transform.localEulerAngles;
										FleetBuilder.add(item.name + "botparent" + num, item2.transform.parent.name);
										FleetBuilder.add(item.name + "botnum", num + 1);
										FleetBuilder.add(item.name + "botscalex" + num, item2.transform.localScale.x);
										FleetBuilder.add(item.name + "botscaley" + num, item2.transform.localScale.y);
										FleetBuilder.add(item.name + "botscalez" + num, item2.transform.localScale.z);
										FleetBuilder.add(item.name + "botposx" + num, item2.transform.parent.localPosition.x);
										FleetBuilder.add(item.name + "botposy" + num, item2.transform.parent.localPosition.y);
										FleetBuilder.add(item.name + "botposz" + num, item2.transform.parent.localPosition.z);
									}
									else
									{
										string key2 = item.name + num;
										FleetBuilder.add(key2, "nothing");
									}
									FleetBuilder.write();
									num++;
								}
							}
						}
					}
