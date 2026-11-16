using System.Collections;
using System.Collections.Generic;
using DFNetwork.Simulation;
using DFNetwork.Simulation.Modules;
using TrueSync;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;
using simple;

public class JoinMultiplayerGame : aGameState
{
	private enum UIState
	{
		Connecting,
		Lobby,
		WaitingRoom,
		LoadingLevel,
		Game,
		Options
	}

	public class ShipCon
	{
		public string ship;

		public Vector3 loc;

		public List<string> Weapons;

		public List<Vector3> Positions;

		public List<Vector3> Scales;

		public ShipCon(string nam, Vector3 startlo, List<string> Weaponsinput, List<Vector3> Positionsin, List<Vector3> Scalesin)
		{
			ship = nam;
			loc = startlo;
			Weapons = Weaponsinput;
			Positions = Positionsin;
			Scales = Scalesin;
		}
	}

	public class GunCon
	{
		public string name;

		public Vector3 pos;

		public Vector3 scale;

		public GunCon(string nam, Vector3 startlo, Vector3 scalelo)
		{
			name = nam;
			pos = startlo;
			scale = scalelo;
		}
	}

	private PhotonView pv;

	private NetImplement_Photon netObject;

	private SimulationLayer simObject;

	private IOComponent FleetBuilder;

	public bool visualUI;

	public string roomName = string.Empty;

	public VRTK_SDKManager vrtk_sdkman;

	private GameController _gc;

	private bool Spawned;

	public bool alreadyspawned;

	private Vector2 scrollPos = Vector2.zero;

	private UIState CurrentState;

	private CrossLevelVariableHolder crosslevelholder;

	private int i;

	private ModuleTypeRegistry ModuleRegistry => ModuleTypeRegistry.GetInstance;

	private T GetModule<T>(string name) where T : iModule
	{
		return (T)ModuleRegistry.GetModuleByName(name).Module;
	}

	private void OnGUI()
	{
		switch (CurrentState)
		{
		case UIState.Connecting:
			Spawned = false;
			ShowConnecting();
			break;
		case UIState.Lobby:
			ShowLobby();
			break;
		case UIState.WaitingRoom:
			break;
		case UIState.LoadingLevel:
			break;
		case UIState.Game:
			break;
		case UIState.Options:
			break;
		}
	}

	public override void OnActive()
	{
		pv = GetComponent<PhotonView>();
		Debug.Log("Connecting to network");
		netObject.DoNetworkConnect();
		netObject.OnNetworkConnect += NetConnected;
		netObject.OnRoomJoined += JoinedRoom;
		netObject.OnRoomLeft += LeftRoom;
		roomName = PhotonNetwork.playerName + "'s Room";
		Debug.Log("Connecting to PUN");
		if (!PhotonNetwork.connected)
		{
			PhotonNetwork.ConnectUsingSettings("0.1");
			PhotonNetwork.ConnectToBestCloudServer("0.1");
		}
	}

	private void JoinedRoom()
	{
		CurrentState = UIState.WaitingRoom;
	}

	private void LeftRoom()
	{
		CurrentState = UIState.Lobby;
	}

	private void NetConnected()
	{
		CurrentState = UIState.Lobby;
	}

	private void OnMasterInstructLoad()
	{
		CurrentState = UIState.LoadingLevel;
	}

	private void OnStartGame()
	{
		Camera.main.farClipPlane = 10000f;
		CurrentState = UIState.Game;
	}

	private void ShowConnecting()
	{
	}

	public int getrooms()
	{
		return PhotonNetwork.GetRoomList().Length;
	}

	public void Createroom()
	{
		PhotonNetwork.CreateRoom(roomName, new RoomOptions
		{
			MaxPlayers = 2,
			IsOpen = true,
			IsVisible = true
		}, null);
	}

	private void ShowLobby()
	{
		if (PhotonNetwork.room != null || !visualUI)
		{
			return;
		}
		GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400f, 300f));
		GUILayout.Label("Main Menu");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Player name:", GUILayout.Width(150f));
		PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
		if (GUI.changed)
		{
			PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(15f);
		GUILayout.BeginHorizontal();
		GUILayout.Label("JOIN ROOM:", GUILayout.Width(150f));
		roomName = GUILayout.TextField(roomName);
		if (GUILayout.Button("GO"))
		{
			PhotonNetwork.JoinRoom(roomName);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("CREATE ROOM:", GUILayout.Width(150f));
		roomName = GUILayout.TextField(roomName);
		if (GUILayout.Button("GO"))
		{
			PhotonNetwork.CreateRoom(roomName, new RoomOptions
			{
				MaxPlayers = 2,
				IsOpen = true,
				IsVisible = true
			}, null);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("JOIN RANDOM ROOM:", GUILayout.Width(150f));
		if (GUILayout.Button("GO"))
		{
			PhotonNetwork.JoinRandomRoom();
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(30f);
		GUILayout.Label("ROOM LISTING:");
		GUILayout.Space(30f);
		GUILayout.Label("ROOM LISTING:");
		if (PhotonNetwork.GetRoomList().Length == 0)
		{
			GUILayout.Label("..no games available..");
		}
		else
		{
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			RoomInfo[] roomList = PhotonNetwork.GetRoomList();
			foreach (RoomInfo roomInfo in roomList)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(roomInfo.name + " " + roomInfo.playerCount + "/" + roomInfo.maxPlayers);
				if (GUILayout.Button("JOIN"))
				{
					PhotonNetwork.JoinRoom(roomInfo.name);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}
		GUILayout.EndArea();
	}

	private void ShowWaitingRoom()
	{
		GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400f, 300f));
		GUILayout.Label("Waiting Room");
		GUILayout.BeginVertical();
		GUILayout.Label("Players:");
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in playerList)
		{
			GUILayout.BeginHorizontal(GUILayout.Width(200f));
			GUILayout.Toggle(GetModule<MOD_ReadyState>("Ready State").GetReadyStateForPlayer((short)photonPlayer.ID), "Ready");
			if (photonPlayer.isMasterClient)
			{
				GUILayout.Label("*Master* " + photonPlayer.name);
			}
			else
			{
				GUILayout.Label(photonPlayer.name);
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.Label("The map will be \"Map1\"");
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Quit"))
		{
			PhotonNetwork.LeaveRoom();
		}
		if (PhotonNetwork.isMasterClient)
		{
			if (GetModule<MOD_ReadyState>("Ready State").AllPlayersReady)
			{
				CurrentState = UIState.LoadingLevel;
			}
			else
			{
				GUILayout.Box("Start Game");
			}
		}
		else
		{
			GUILayout.Box("Only master can start");
		}
		if (GUILayout.Button(GetModule<MOD_ReadyState>("Ready State").GetReadyStateForPlayer(netObject.GetCurrentPlayerID) ? "I am not ready" : "I am ready"))
		{
			GetModule<MOD_ReadyState>("Ready State").SendReady(!GetModule<MOD_ReadyState>("Ready State").GetReadyStateForPlayer(netObject.GetCurrentPlayerID));
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	public void spawnallships()
	{
		if (PhotonNetwork.connected)
		{
			if (PhotonNetwork.isMasterClient)
			{
				int num = 0;
				foreach (int item in crosslevelholder.team1wbots())
				{
					num++;
					if (item == 300)
					{
						SpawnObjects(bot: true, num, 4);
					}
				}
				num = 3;
				foreach (int item2 in crosslevelholder.team2wbots())
				{
					num++;
					if (item2 == 300)
					{
						SpawnObjects(bot: true, num, 4);
					}
				}
			}
			else
			{
				int num2 = 0;
				foreach (int item3 in crosslevelholder.team1wbots())
				{
					num2++;
					if (item3 == 300)
					{
						spawnbotcontroller(getshipcolbot(num2));
					}
				}
				num2 = 3;
				foreach (int item4 in crosslevelholder.team2wbots())
				{
					num2++;
					if (item4 == 300)
					{
						spawnbotcontroller(getshipcolbot(num2));
					}
				}
			}
		}
		if (PhotonNetwork.connected)
		{
			SpawnObjects(bot: false, crosslevelholder.findspawnpos(PhotonNetwork.player.ID), 4);
		}
	}

	public _Ship.eShipColor getshipcolbot(int a)
	{
		_Ship.eShipColor result = _Ship.eShipColor.Green;
		switch (a)
		{
		case 1:
			result = _Ship.eShipColor.Green;
			break;
		case 2:
			result = _Ship.eShipColor.Blue;
			break;
		case 3:
			result = _Ship.eShipColor.White;
			break;
		case 4:
			result = _Ship.eShipColor.Yellow;
			break;
		case 5:
			result = _Ship.eShipColor.Grey;
			break;
		case 6:
			result = _Ship.eShipColor.Red;
			break;
		}
		return result;
	}

	private void SpawnObjects(bool bot, int a, int spawnfleet)
	{
		Debug.Log(a);
		if ((bot && crosslevelholder.bots && !crosslevelholder.TestArea && !crosslevelholder.campaign) || (!bot && !alreadyspawned))
		{
			if (!bot)
			{
				alreadyspawned = true;
			}
			Spawned = true;
			Debug.Log("Joined room \"" + PhotonNetwork.room.Name + "\"");
			if (PhotonNetwork.isMasterClient && !PhotonNetwork.offlineMode)
			{
				PhotonNetwork.room.IsOpen = true;
			}
			CrossLevelVariableHolder component = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
			_gc.convertspawnpoints(component.Gamemode);
			Vector3 vector = spawnpoint(_gc, a);
			Quaternion identity = Quaternion.identity;
			_Ship.eShipColor colour = getspawncolor(a);
			string text = "TestFleet.json";
			string prefabName = "Super-Capital";
			savefleetform2(loadshipsform1(spawnfleet));
			Vector3 forward = Vector3.zero - vector;
			identity = Quaternion.LookRotation(forward);
			fleetFileSerializer fleetFileSerializer2 = fleetFileSerializer.Load(text);
			Debug.Log("Loaded \"" + text + "\" with the fleet named \"" + fleetFileSerializer2.FleetName + "\"");
			Debug.Log(fleetFileSerializer2.Ships.Count);
			if (bot && !crosslevelholder.tutorial)
			{
				spawnbotcontroller(colour);
			}
			float num = 0f;
			foreach (fleetFileSerializer.Ship ship in fleetFileSerializer2.Ships)
			{
				if (ship.Type != "nothing")
				{
					Debug.Log(ship.Type);
					int num2 = PhotonNetwork.AllocateViewID();
					int iD = PhotonNetwork.player.ID;
					GetModule<MOD_SpawnObject>("Spawn Object").SendSpawnObject(ship.Type, vector + new Vector3(0f, 0f, num), identity, num2);
					if (!crosslevelholder.tutorial || bot)
					{
						pv.RPC("fixturrets", PhotonTargets.All, num2, ship.Weapon1, ship.Weapon2, ship.Weapon3, ship.Weapon4, ship.Weapon5, ship.WeaponPos1, ship.WeaponPos2, ship.WeaponPos3, ship.WeaponPos4, ship.WeaponPos5, ship.WeaponScale1, ship.WeaponScale2, ship.WeaponScale3, ship.WeaponScale4, ship.WeaponScale5, a, bot, vector * 1000f + new Vector3(0f, 0f, num * 1000f));
					}
					else
					{
						pv.RPC("fixturrets", PhotonTargets.All, num2, ship.Weapon1, ship.Weapon2, ship.Weapon3, ship.Weapon4, ship.Weapon5, ship.WeaponPos1, ship.WeaponPos2, ship.WeaponPos3, ship.WeaponPos4, ship.WeaponPos5, ship.WeaponScale1, ship.WeaponScale2, ship.WeaponScale3, ship.WeaponScale4, ship.WeaponScale5, a, bot, new Vector3(0f, 0f, num * 100f) + new Vector3(1f, 1f, 1f));
					}
					num++;
				}
			}
			int num3 = PhotonNetwork.AllocateViewID();
			StartCoroutine("Waitforread");
			if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip)
			{
				GetModule<MOD_SpawnObject>("Spawn Object").SendSpawnObject(prefabName, vector * 1000f + new Vector3(0f, 0f, num * 1000f), identity, num3);
				pv.RPC("fixShip", PhotonTargets.All, num3, a, vector * 1000f + new Vector3(0f, 0f, num * 1000f));
			}
		}
		if (bot && crosslevelholder.campaign)
		{
			_Ship.eShipColor eShipColor = getspawncolor(a);
			Debug.Log(eShipColor);
			AIController aIController = spawnbotcontroller(eShipColor);
			_gc.convertspawnpoints(CrossLevelVariableHolder.gamemode.TeamDeathMatch);
			aIController.setmission(crosslevelholder.campaignlevel, spawnpoint(_gc, a).ToTSVector());
		}
	}

	private IEnumerator Waitforread()
	{
		yield return new WaitForSeconds(3f);
		PhotonView phop = GetComponent<PhotonView>();
		Debug.Log("callall");
		phop.RPC("ready", PhotonTargets.All, PhotonNetwork.player.ID);
	}

	[PunRPC]
	public void ready(int player)
	{
		transform.parent.GetComponent<UnitMovementcommandcontroller>().playerready(player);
	}

	private _Ship.eShipColor getspawncolor(int a)
	{
		_Ship.eShipColor result = _Ship.eShipColor.Yellow;
		switch (a)
		{
		case 1:
			result = _Ship.eShipColor.Green;
			break;
		case 2:
			result = _Ship.eShipColor.Blue;
			break;
		case 3:
			result = _Ship.eShipColor.White;
			break;
		case 4:
			result = _Ship.eShipColor.Yellow;
			break;
		case 5:
			result = _Ship.eShipColor.Grey;
			break;
		case 6:
			result = _Ship.eShipColor.Red;
			break;
		}
		return result;
	}

	private Vector3 spawnpoint(GameController _gc, int a)
	{
		Vector3 result = Vector3.zero;
		switch (a)
		{
		case 1:
			result = (result = _gc.SpawnPoint1);
			break;
		case 2:
			result = (result = _gc.SpawnPoint2);
			break;
		case 3:
			result = (result = _gc.SpawnPoint3);
			break;
		case 4:
			result = (result = _gc.SpawnPoint4);
			break;
		case 5:
			result = (result = _gc.SpawnPoint5);
			break;
		case 6:
			result = (result = _gc.SpawnPoint6);
			break;
		}
		return result;
	}

	public AIController spawnbotcontroller(_Ship.eShipColor colour)
	{
		GameObject gameObject = new GameObject("AIController");
		gameObject.transform.parent = GameObject.Find("Controllers").transform;
		gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		AIController aIController = gameObject.AddComponent<AIController>();
		aIController.team = colour;
		return aIController;
	}

	[PunRPC]
	public void fixturrets(int viewID, string Weapon1, string Weapon2, string Weapon3, string Weapon4, string Weapon5, Vector3 WeaponPos1, Vector3 WeaponPos2, Vector3 WeaponPos3, Vector3 WeaponPos4, Vector3 WeaponPos5, Vector3 WeaponScale1, Vector3 WeaponScale2, Vector3 WeaponScale3, Vector3 WeaponScale4, Vector3 WeaponScale5, int botnum, bool bot, Vector3 input)
	{
		PhotonView.Find(viewID).gameObject.GetComponent<_Ship>().SpawnWeapons(Weapon1, Weapon2, Weapon3, Weapon4, Weapon5, WeaponPos1, WeaponPos2, WeaponPos3, WeaponPos4, WeaponPos5, WeaponScale1, WeaponScale2, WeaponScale3, WeaponScale4, WeaponScale5, botnum, bot, input);
	}

	[PunRPC]
	public void fixShip(int viewID, int spawnpos, Vector3 pos)
	{
		PhotonView.Find(viewID).GetComponent<_Ship>().spawnnum = spawnpos;
		PhotonView.Find(viewID).GetComponent<TSTransform>().position = pos.ToTSVector();
	}

	private List<ShipCon> loadshipsform1(int fleet)
	{
		string key = "AttachPointsList" + fleet + "num";
		string text = "AttachPointsList" + fleet;
		List<ShipCon> list = new List<ShipCon>();
		int num = FleetBuilder.get<int>(key);
		bool bot = false;
		if (fleet > 3)
		{
			bot = true;
		}
		for (int i = 0; i < num; i++)
		{
			List<GunCon> list2 = LoadWeapons(FleetBuilder.get<string>(text + i), bot);
			float x = FleetBuilder.get<float>(text + "scalex" + i);
			float y = FleetBuilder.get<float>(text + "scaley" + i);
			float z = FleetBuilder.get<float>(text + "scalez" + i);
			List<string> list3 = new List<string>();
			List<Vector3> list4 = new List<Vector3>();
			List<Vector3> list5 = new List<Vector3>();
			foreach (GunCon item in list2)
			{
				list3.Add(item.name);
				list4.Add(item.pos);
				list5.Add(item.scale);
			}
			list.Add(new ShipCon(FleetBuilder.get<string>(text + i), new Vector3(x, y, z), list3, list4, list5));
		}
		FleetBuilder.read();
		return list;
	}

	public void savefleetform2(List<ShipCon> ships)
	{
		string jsonFile = "TestFleet.json";
		fleetFileSerializer fleetFileSerializer2 = new fleetFileSerializer();
		fleetFileSerializer2.FleetName = "Another Test Fleet";
		foreach (ShipCon ship in ships)
		{
			string[] array = new string[5];
			for (int i = 0; i < 5; i++)
			{
				if (ship.Weapons.Count > i)
				{
					array[i] = ship.Weapons[i];
				}
			}
			Vector3[] array2 = new Vector3[5]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			};
			for (int j = 0; j < 5; j++)
			{
				if (ship.Positions.Count > j)
				{
					ref Vector3 reference = ref array2[j];
					reference = ship.Positions[j];
				}
			}
			Vector3[] array3 = new Vector3[5]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			};
			for (int k = 0; k < 5; k++)
			{
				if (ship.Scales.Count > k)
				{
					ref Vector3 reference2 = ref array3[k];
					reference2 = ship.Scales[k];
				}
			}
			fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
			{
				Type = ship.ship,
				Position = ship.loc,
				Weapon1 = array[0],
				Weapon2 = array[1],
				Weapon3 = array[2],
				Weapon4 = array[3],
				Weapon5 = array[4],
				WeaponPos1 = array2[0],
				WeaponPos2 = array2[1],
				WeaponPos3 = array2[2],
				WeaponPos4 = array2[3],
				WeaponPos5 = array2[4],
				WeaponScale1 = array3[0],
				WeaponScale2 = array3[1],
				WeaponScale3 = array3[2],
				WeaponScale4 = array3[3],
				WeaponScale5 = array3[4]
			});
		}
		fleetFileSerializer.Save(fleetFileSerializer2, jsonFile);
	}

	public void jsonsavedefaultshiplist(string jsonFile)
	{
		fleetFileSerializer fleetFileSerializer2 = new fleetFileSerializer();
		fleetFileSerializer2.FleetName = "Another Test Fleet";
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "CruiserDefense",
			Position = new Vector3(-0.5f, 1f, 0f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "CruiserOffense",
			Position = new Vector3(0f, 1f, 0f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "CruiserDefense",
			Position = new Vector3(0.5f, 1f, 0f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "FrigateOffense",
			Position = new Vector3(-1f, 0f, 2.5f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "FrigateOffense",
			Position = new Vector3(-0.5f, 0f, 2.5f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "FrigateOffense",
			Position = new Vector3(0f, 0f, 2.5f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "FrigateOffense",
			Position = new Vector3(0.5f, 0f, 2.5f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "FrigateOffense",
			Position = new Vector3(1f, 0f, 2.5f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "CorvetteOffense",
			Position = new Vector3(-1f, 0f, 3f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "CorvetteSupport",
			Position = new Vector3(-0.5f, 0f, 3f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "CorvetteOffense",
			Position = new Vector3(0f, 0f, 3f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "CorvetteSupport",
			Position = new Vector3(0.5f, 0f, 3f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "CorvetteOffense",
			Position = new Vector3(1f, 0f, 3f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "LightCraftOffense",
			Position = new Vector3(-1f, 0f, 3.25f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "LightCraftDefense",
			Position = new Vector3(-0.5f, 0f, 3.25f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "LightCraftSupport",
			Position = new Vector3(0f, 0f, 3.25f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "LightCraftTank",
			Position = new Vector3(0.5f, 0f, 3.25f)
		});
		fleetFileSerializer2.Ships.Add(new fleetFileSerializer.Ship
		{
			Type = "LightCraftOffense",
			Position = new Vector3(1f, 0f, 3.25f)
		});
		fleetFileSerializer.Save(fleetFileSerializer2, jsonFile);
	}

	public override void OnDeactivate()
	{
	}

	public override void OnTransitionFrom()
	{
	}

	public override void OnTransitionTo()
	{
	}

	public void OnJoinedLobby()
	{
	}

	public void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby();
	}

	public void OnPhotonRandomJoinFailed()
	{
	}

	private void Awake()
	{
		netObject = Object.FindObjectOfType(typeof(NetImplement_Photon)) as NetImplement_Photon;
	}

	private void Start()
	{
		if (PhotonNetwork.connectedAndReady)
		{
			crosslevelholder = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
		}
		OnActive();
		_gc = Object.FindObjectOfType<GameController>();
		simObject = GetComponent<SimulationLayer>();
		FleetBuilder = IORoot.findIO("fleet1");
		FleetBuilder.read();
	}

	private void Update()
	{
		i++;
		if (i == 50 && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1)
		{
			spawnallships();
		}
	}

	private List<GunCon> LoadWeapons(string loadObj, bool bot)
	{
		List<GunCon> list = new List<GunCon>();
		string text = string.Empty;
		if (bot)
		{
			text = "bot";
		}
		loadObj += text;
		int num = FleetBuilder.get<int>(loadObj + "num");
		for (int i = 0; i < num; i++)
		{
			string text2 = FleetBuilder.get<string>(loadObj + i);
			float x = FleetBuilder.get<float>(loadObj + "posx" + i);
			float y = FleetBuilder.get<float>(loadObj + "posy" + i);
			float z = FleetBuilder.get<float>(loadObj + "posz" + i);
			float x2 = FleetBuilder.get<float>(loadObj + "scalex" + i);
			float y2 = FleetBuilder.get<float>(loadObj + "scaley" + i);
			float z2 = FleetBuilder.get<float>(loadObj + "scalez" + i);
			if (text2 != "nothing")
			{
				list.Add(new GunCon(text2, new Vector3(x, y, z), new Vector3(x2, y2, z2)));
			}
		}
		FleetBuilder.read();
		return list;
	}
}
