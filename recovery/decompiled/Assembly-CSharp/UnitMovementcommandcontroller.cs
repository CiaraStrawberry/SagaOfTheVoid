using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TrueSync;
using UnityEngine;
using UnityEngine.SceneManagement;
using simple;

public class UnitMovementcommandcontroller : TrueSyncBehaviour
{
	public class playerisreadychecker
	{
		public string name;

		public bool isready;

		public int playerid;

		public playerisreadychecker(string namein, bool isreadyin, int playeridin)
		{
			name = namein;
			isready = isreadyin;
			playerid = playeridin;
		}
	}

	public PlayerPrefabVoiceControlScript.VoiceChannel curchannel;

	public _Ship.eShipColor team;

	[SerializeField]
	private GameObject[] Blue = new GameObject[5];

	[SerializeField]
	private GameObject[] Green = new GameObject[5];

	[SerializeField]
	private GameObject[] Grey = new GameObject[5];

	[SerializeField]
	private GameObject[] Red = new GameObject[5];

	[SerializeField]
	private GameObject[] White = new GameObject[5];

	[SerializeField]
	private GameObject[] Yellow = new GameObject[5];

	[SerializeField]
	private TSTransform[] BlueTargets = new TSTransform[0];

	[SerializeField]
	private TSTransform[] GreenTargets = new TSTransform[0];

	[SerializeField]
	private TSTransform[] GreyTargets = new TSTransform[0];

	[SerializeField]
	private TSTransform[] RedTargets = new TSTransform[0];

	[SerializeField]
	private TSTransform[] WhiteTargets = new TSTransform[0];

	[SerializeField]
	private TSTransform[] YellowTargets = new TSTransform[0];

	public bool output;

	public List<GameObject> all_ships = new List<GameObject>();

	public List<_Ship> all_shipsScript = new List<_Ship>();

	private FP timepassed;

	public FP Timeleft = 600;

	public CrossLevelVariableHolder.gamemode Gamemode;

	public CrossLevelVariableHolder crosslevelholder;

	public PhotonView phopview;

	public GameObject Team1victory;

	public GameObject Team2Victory;

	private List<GameObject> Team1Capitals = new List<GameObject>();

	private List<GameObject> Team2Capitals = new List<GameObject>();

	public GameObject map1area;

	public GameObject map2area;

	public GameObject map3area;

	[SerializeField]
	private GameObject Objectsholder;

	[SerializeField]
	private int money = 1500;

	public GameObject Loading;

	private TutorialTextControlScript tutcontrol;

	public GameObject[] tutorialIcons;

	private AudioSource audiosource;

	private List<playerisreadychecker> players = new List<playerisreadychecker>();

	public bool running;

	public int randomnumber;

	public List<_Ship> allshipsscript = new List<_Ship>();

	public List<GameObject> allshipsgams = new List<GameObject>();

	public List<AIController> aicontrollers = new List<AIController>();

	public TSTransform[] allshipststransform = new TSTransform[0];

	public int moneyincreaserate = 30;

	public bool started;

	public bool isalreadywon;

	private IOComponent FleetBuilder;

	public bool endall;

	private bool victorytest;

	public GameObject hostlefticon;

	private bool simrunning;

	public GameObject debug;

	public GameObject Scout;

	public GameObject Fighter;

	public GameObject Bomber;

	public GameObject AttackCraft;

	public GameObject MissileFrigate;

	public GameObject FlackFrigate;

	public GameObject CannonFriage;

	public GameObject LazerFrigate;

	public GameObject SupportFrigate;

	public GameObject RepairFrigate;

	public GameObject Destroyer;

	public GameObject Capital;

	public GameObject FlagShip;

	public RelayController InputRelay;

	private void Awake()
	{
		crosslevelholder = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
		if (crosslevelholder.tutorial)
		{
			tutcontrol = GameObject.Find("TutorialPanelHolder").GetComponent<TutorialTextControlScript>();
		}
		team = crosslevelholder.findspawnteam(PhotonNetwork.player.ID);
		Gamemode = crosslevelholder.Gamemode;
		if (SceneManager.GetActiveScene().name != "mptest")
		{
			curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.All;
		}
		else if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch || crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip)
		{
			curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly;
		}
		else
		{
			curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.All;
		}
		map1area.SetActive(value: false);
		map2area.SetActive(value: false);
		map3area.SetActive(value: false);
		if (crosslevelholder.map == CrossLevelVariableHolder.mapcon.map1)
		{
			map1area.SetActive(value: true);
		}
		if (crosslevelholder.map == CrossLevelVariableHolder.mapcon.map2)
		{
			map2area.SetActive(value: true);
		}
		if (crosslevelholder.map == CrossLevelVariableHolder.mapcon.map3)
		{
			map3area.SetActive(value: true);
		}
	}

	private void Start()
	{
		tutorialIcons = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().TutorialObjs.ToArray();
		GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading = false;
		Timeleft = 1200;
		if (crosslevelholder.campaign && crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive)
		{
			Timeleft = 300;
		}
		phopview = GetComponent<PhotonView>();
		Objectsholder = GameObject.Find("Objects");
		RenderSettings.skybox = crosslevelholder.getskybox(crosslevelholder.skybox);
		audiosource = GetComponent<AudioSource>();
		audiosource.clip = crosslevelholder.getmusic();
		audiosource.loop = true;
		audiosource.volume = 0.25f;
		audiosource.Play();
		if (PhotonNetwork.isMasterClient)
		{
			phopview.RPC("setrandomnumberforthismatch", PhotonTargets.All, Random.Range(0, 100));
		}
		if (!crosslevelholder.campaign)
		{
			money = 1500;
		}
		else
		{
			money = crosslevelholder.campaignlevel.startmoney;
		}
		if (PhotonNetwork.isMasterClient)
		{
			PhotonPlayer[] otherPlayers = PhotonNetwork.otherPlayers;
			foreach (PhotonPlayer photonPlayer in otherPlayers)
			{
				players.Add(new playerisreadychecker(photonPlayer.NickName, isreadyin: false, photonPlayer.ID));
			}
			players.Add(new playerisreadychecker(PhotonNetwork.player.NickName, isreadyin: true, PhotonNetwork.player.ID));
		}
		GameObject[] array = tutorialIcons;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(value: false);
		}
	}

	[PunRPC]
	private void setrandomnumberforthismatch(int randomnum)
	{
		randomnumber = randomnum;
	}

	public void StartMain()
	{
		InputRelay.relay.running = true;
		Debug.Log("Invoke");
		foreach (Transform item in base.transform)
		{
			if (item.GetComponent<AIController>() != null)
			{
				aicontrollers.Add(item.GetComponent<AIController>());
			}
		}
		foreach (Transform item2 in transform)
		{
			if (item2.GetComponent<AIController>() != null)
			{
				item2.GetComponent<AIController>().StartMain(randomnumber);
			}
		}
		running = true;
		TrueSyncManager.SyncedStartCoroutine(waaait());
		OneSecond();
	}

	private IEnumerator waaait()
	{
		while (true)
		{
			yield return 1;
			if (!running)
			{
				continue;
			}
			if (Loading.gameObject != null)
			{
				Object.Destroy(Loading);
			}
			if (aicontrollers.Count == 0 || (aicontrollers.Count > 0 && !aicontrollers[0].ismission) || (crosslevelholder.campaign && crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive))
			{
				addmoney(moneyincreaserate);
			}
			if (teammembersout(team).Count == 0 && timepassed > 25)
			{
				money = 0;
			}
			OneSecond();
			resetships();
			all_ships = concatarraylist(new List<GameObject>[6]
			{
				Green.ToList(),
				Grey.ToList(),
				Red.ToList(),
				White.ToList(),
				Blue.ToList(),
				Yellow.ToList()
			}).ToList();
			foreach (AIController aicontroller in aicontrollers)
			{
				aicontroller.GiveOrderwait();
			}
			checkallships();
			foreach (_Ship item in allshipsscript)
			{
				item.callallreps();
			}
		}
	}

	public int getmoney()
	{
		return money;
	}

	public void addmoney(int amount)
	{
		if (money < 7000)
		{
			money += amount;
		}
	}

	public void takemoney(int amount)
	{
		money -= amount;
	}

	private void checkallships()
	{
		foreach (Transform item in Objectsholder.transform)
		{
			if (item.name != "Working" && item.name != "Engines" && !allshipsgams.Contains(item.gameObject))
			{
				allshipsscript.Add(item.GetComponent<_Ship>());
				allshipsgams.Add(item.gameObject);
				addtoarrayTSTransfrom(ref allshipststransform, item.GetComponent<TSTransform>());
			}
		}
		List<GameObject> list = new List<GameObject>();
		List<_Ship> list2 = new List<_Ship>();
		List<TSTransform> list3 = new List<TSTransform>();
		foreach (GameObject allshipsgam in allshipsgams)
		{
			if (allshipsgam == null || allshipsgam.gameObject == null)
			{
				list.Add(allshipsgam);
			}
		}
		foreach (_Ship item2 in allshipsscript)
		{
			if (item2 == null || item2.gameObject == null)
			{
				list2.Add(item2);
			}
		}
		TSTransform[] array = allshipststransform;
		foreach (TSTransform tSTransform in array)
		{
			if (tSTransform == null || tSTransform.gameObject == null)
			{
				list3.Add(tSTransform);
			}
		}
		foreach (GameObject item3 in list)
		{
			allshipsgams.Remove(item3);
		}
		foreach (_Ship item4 in list2)
		{
			allshipsscript.Remove(item4);
		}
		foreach (TSTransform item5 in list3)
		{
			RemovefromTStransform(ref allshipststransform, item5);
		}
	}

	public List<TSTransform> alliesout()
	{
		return alliesoutcustom(team);
	}

	public override void OnSyncedStart()
	{
		InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
	}

	private void addtoarrayTSTransfrom(ref TSTransform[] inputarray, TSTransform addition)
	{
		List<TSTransform> list = inputarray.ToList();
		list.Add(addition);
		inputarray = list.ToArray();
	}

	private void RemovefromTStransform(ref TSTransform[] inputarray, TSTransform addition)
	{
		List<TSTransform> list = inputarray.ToList();
		list.Remove(addition);
		inputarray = list.ToArray();
	}

	public List<TSTransform> alliesoutcustom(_Ship.eShipColor control)
	{
		List<TSTransform> result = new List<TSTransform>();
		if (Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch)
		{
			switch (control)
			{
			case _Ship.eShipColor.Green:
				result = combinearrays(new List<GameObject>[3]
				{
					Green.ToList(),
					Blue.ToList(),
					White.ToList()
				}).ToList();
				break;
			case _Ship.eShipColor.Blue:
				result = combinearrays(new List<GameObject>[3]
				{
					Green.ToList(),
					Blue.ToList(),
					White.ToList()
				}).ToList();
				break;
			case _Ship.eShipColor.White:
				result = combinearrays(new List<GameObject>[3]
				{
					Green.ToList(),
					Blue.ToList(),
					White.ToList()
				}).ToList();
				break;
			case _Ship.eShipColor.Yellow:
				result = combinearrays(new List<GameObject>[3]
				{
					Yellow.ToList(),
					Grey.ToList(),
					Red.ToList()
				}).ToList();
				break;
			case _Ship.eShipColor.Grey:
				result = combinearrays(new List<GameObject>[3]
				{
					Yellow.ToList(),
					Grey.ToList(),
					Red.ToList()
				}).ToList();
				break;
			case _Ship.eShipColor.Red:
				result = combinearrays(new List<GameObject>[3]
				{
					Yellow.ToList(),
					Grey.ToList(),
					Red.ToList()
				}).ToList();
				break;
			}
		}
		else
		{
			switch (control)
			{
			case _Ship.eShipColor.Blue:
				result = combinearrays(new List<GameObject>[1] { Blue.ToList() }).ToList();
				break;
			case _Ship.eShipColor.Green:
				result = combinearrays(new List<GameObject>[1] { Green.ToList() }).ToList();
				break;
			case _Ship.eShipColor.Grey:
				result = combinearrays(new List<GameObject>[1] { Grey.ToList() }).ToList();
				break;
			case _Ship.eShipColor.Red:
				result = combinearrays(new List<GameObject>[1] { Red.ToList() }).ToList();
				break;
			case _Ship.eShipColor.White:
				result = combinearrays(new List<GameObject>[1] { White.ToList() }).ToList();
				break;
			case _Ship.eShipColor.Yellow:
				result = combinearrays(new List<GameObject>[1] { Yellow.ToList() }).ToList();
				break;
			}
		}
		return result;
	}

	public List<GameObject> teammembersout(_Ship.eShipColor control)
	{
		List<GameObject> result = new List<GameObject>();
		switch (control)
		{
		case _Ship.eShipColor.Blue:
			result = Blue.ToList();
			break;
		case _Ship.eShipColor.Green:
			result = Green.ToList();
			break;
		case _Ship.eShipColor.Grey:
			result = Grey.ToList();
			break;
		case _Ship.eShipColor.Red:
			result = Red.ToList();
			break;
		case _Ship.eShipColor.White:
			result = White.ToList();
			break;
		case _Ship.eShipColor.Yellow:
			result = Yellow.ToList();
			break;
		}
		return result;
	}

	public void resetships()
	{
		foreach (Transform item in GameObject.Find("Objects").transform)
		{
			if (!checkelidgable(item))
			{
				continue;
			}
			switch (item.GetComponent<_Ship>().ShipColor)
			{
			case _Ship.eShipColor.Blue:
				if (!Blue.Contains(item.gameObject))
				{
					addtoarray(ref Blue, item.gameObject);
				}
				break;
			case _Ship.eShipColor.Green:
				if (!Green.Contains(item.gameObject))
				{
					addtoarray(ref Green, item.gameObject);
				}
				break;
			case _Ship.eShipColor.Grey:
				if (!Grey.Contains(item.gameObject))
				{
					addtoarray(ref Grey, item.gameObject);
				}
				break;
			case _Ship.eShipColor.Red:
				if (!Red.Contains(item.gameObject))
				{
					addtoarray(ref Red, item.gameObject);
				}
				break;
			case _Ship.eShipColor.White:
				if (!White.Contains(item.gameObject))
				{
					addtoarray(ref White, item.gameObject);
				}
				break;
			case _Ship.eShipColor.Yellow:
				if (!Yellow.Contains(item.gameObject))
				{
					addtoarray(ref Yellow, item.gameObject);
				}
				break;
			}
		}
	}

	private void addtoarray(ref GameObject[] inputarray, GameObject addition)
	{
		List<GameObject> list = inputarray.ToList();
		list.Add(addition);
		inputarray = list.ToArray();
	}

	private void Removefromarray(ref GameObject[] inputarray, GameObject addition)
	{
		inputarray = inputarray.Where((GameObject val) => val != addition).ToArray();
	}

	private TSTransform[] combinearrays(List<GameObject>[] input)
	{
		List<TSTransform> list = new List<TSTransform>();
		foreach (List<GameObject> list2 in input)
		{
			foreach (GameObject item in list2)
			{
				if ((bool)item && item.gameObject != null)
				{
					list.Add(item.GetComponent<TSTransform>());
				}
			}
		}
		return list.ToArray();
	}

	public void playerready(int playerview)
	{
		foreach (playerisreadychecker player in players)
		{
			if (player.playerid == playerview)
			{
				player.isready = true;
			}
		}
	}

	public void OneSecond()
	{
		removenullentities(ref Blue);
		removenullentities(ref Green);
		removenullentities(ref Yellow);
		removenullentities(ref Grey);
		removenullentities(ref Red);
		removenullentities(ref White);
		fogcheck();
		if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip || (crosslevelholder.campaign && crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.killTarget))
		{
			Team1Capitals.Clear();
			Team2Capitals.Clear();
			foreach (Transform item in Objectsholder.transform)
			{
				if (item.name == "Super-Capital(Clone)")
				{
					_Ship component = item.GetComponent<_Ship>();
					if (component.ShipColor == _Ship.eShipColor.Green || component.ShipColor == _Ship.eShipColor.Blue || component.ShipColor == _Ship.eShipColor.White)
					{
						Team1Capitals.Add(item.gameObject);
					}
					if (component.ShipColor == _Ship.eShipColor.Yellow || component.ShipColor == _Ship.eShipColor.Grey || component.ShipColor == _Ship.eShipColor.Red)
					{
						Team2Capitals.Add(item.gameObject);
					}
				}
			}
		}
		if (Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch || Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip)
		{
			GreenTargets = combinearrays(new List<GameObject>[3]
			{
				Grey.ToList(),
				Red.ToList(),
				Yellow.ToList()
			});
			YellowTargets = combinearrays(new List<GameObject>[3]
			{
				Green.ToList(),
				Blue.ToList(),
				White.ToList()
			});
			if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers > 2)
			{
				BlueTargets = combinearrays(new List<GameObject>[3]
				{
					Grey.ToList(),
					Red.ToList(),
					Yellow.ToList()
				});
				GreyTargets = combinearrays(new List<GameObject>[3]
				{
					Green.ToList(),
					Blue.ToList(),
					White.ToList()
				});
			}
			if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers > 2 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
			{
				WhiteTargets = combinearrays(new List<GameObject>[3]
				{
					Grey.ToList(),
					Red.ToList(),
					Yellow.ToList()
				});
				RedTargets = combinearrays(new List<GameObject>[3]
				{
					Green.ToList(),
					Blue.ToList(),
					White.ToList()
				});
			}
		}
		else
		{
			YellowTargets = combinearrays(new List<GameObject>[5]
			{
				Green.ToList(),
				Grey.ToList(),
				Red.ToList(),
				White.ToList(),
				Blue.ToList()
			});
			GreenTargets = combinearrays(new List<GameObject>[5]
			{
				Blue.ToList(),
				Grey.ToList(),
				Red.ToList(),
				White.ToList(),
				Yellow.ToList()
			});
			if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers > 2)
			{
				BlueTargets = combinearrays(new List<GameObject>[5]
				{
					Green.ToList(),
					Grey.ToList(),
					Red.ToList(),
					White.ToList(),
					Yellow.ToList()
				});
				GreyTargets = combinearrays(new List<GameObject>[5]
				{
					Green.ToList(),
					Blue.ToList(),
					Red.ToList(),
					White.ToList(),
					Yellow.ToList()
				});
			}
			if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers > 2 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
			{
				RedTargets = combinearrays(new List<GameObject>[5]
				{
					Green.ToList(),
					Grey.ToList(),
					Blue.ToList(),
					White.ToList(),
					Yellow.ToList()
				});
				WhiteTargets = combinearrays(new List<GameObject>[5]
				{
					Green.ToList(),
					Grey.ToList(),
					Red.ToList(),
					Blue.ToList(),
					Yellow.ToList()
				});
			}
		}
		if (!PhotonNetwork.isMasterClient || !(timepassed > 30) || crosslevelholder.tutorial || crosslevelholder.TestArea || (crosslevelholder.campaign && !(timepassed > 25)))
		{
			return;
		}
		if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch)
		{
			if (!PhotonNetwork.offlineMode)
			{
				if (alliesout().Count == 0)
				{
					if (crosslevelholder.findteam(PhotonNetwork.player.ID) == 1)
					{
						phopview.RPC("team2won", PhotonTargets.All, false);
					}
					else
					{
						phopview.RPC("team1won", PhotonTargets.All, false);
					}
				}
				if (targetsout(team).Count == 0)
				{
					if (crosslevelholder.findteam(PhotonNetwork.player.ID) == 2)
					{
						phopview.RPC("team2won", PhotonTargets.All, false);
					}
					else
					{
						phopview.RPC("team1won", PhotonTargets.All, false);
					}
				}
			}
			else
			{
				if (teammembersout(team).Count == 0)
				{
					if (crosslevelholder.findteam(PhotonNetwork.player.ID) == 1)
					{
						phopview.RPC("team2won", PhotonTargets.All, false);
					}
					else
					{
						phopview.RPC("team1won", PhotonTargets.All, false);
					}
				}
				if (targetsout(team).Count == 0 && (!crosslevelholder.campaign || crosslevelholder.campaignlevel.objective != MainMenuCampaignControlScript.eMissionObjective.Survive))
				{
					if (crosslevelholder.findteam(PhotonNetwork.player.ID) == 2)
					{
						phopview.RPC("team2won", PhotonTargets.All, false);
					}
					else
					{
						phopview.RPC("team1won", PhotonTargets.All, false);
					}
				}
			}
		}
		if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.FreeForAll)
		{
			if (!PhotonNetwork.offlineMode)
			{
				if (concatarraylist(new List<GameObject>[5]
				{
					Grey.ToList(),
					Red.ToList(),
					White.ToList(),
					Blue.ToList(),
					Yellow.ToList()
				}).Length == 0)
				{
					phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Green", false);
				}
				if (concatarraylist(new List<GameObject>[5]
				{
					Green.ToList(),
					Grey.ToList(),
					Red.ToList(),
					White.ToList(),
					Blue.ToList()
				}).Length == 0)
				{
					phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Yellow", false);
				}
				if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers > 2)
				{
					if (concatarraylist(new List<GameObject>[5]
					{
						Green.ToList(),
						Grey.ToList(),
						Red.ToList(),
						White.ToList(),
						Yellow.ToList()
					}).Length == 0)
					{
						phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Blue", false);
					}
					if (concatarraylist(new List<GameObject>[5]
					{
						Green.ToList(),
						Red.ToList(),
						White.ToList(),
						Blue.ToList(),
						Yellow.ToList()
					}).Length == 0)
					{
						phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Grey", false);
					}
				}
				if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers > 2 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
				{
					if (concatarraylist(new List<GameObject>[5]
					{
						Green.ToList(),
						Grey.ToList(),
						Red.ToList(),
						Blue.ToList(),
						Yellow.ToList()
					}).Length == 0)
					{
						phopview.RPC("teamfreeforallwon", PhotonTargets.All, "White", false);
					}
					if (concatarraylist(new List<GameObject>[5]
					{
						Green.ToList(),
						Grey.ToList(),
						White.ToList(),
						Blue.ToList(),
						Yellow.ToList()
					}).Length == 0)
					{
						phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Red", false);
					}
				}
			}
			else
			{
				if (team == _Ship.eShipColor.Green)
				{
					if (concatarraylist(new List<GameObject>[5]
					{
						Grey.ToList(),
						Red.ToList(),
						White.ToList(),
						Blue.ToList(),
						Yellow.ToList()
					}).Length == 0)
					{
						phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Green", false);
					}
					if (teammembersout(team).Count == 0)
					{
						phopview.RPC("teamfreeforallwon", PhotonTargets.All, "AI wins!", true);
					}
				}
				if (team == _Ship.eShipColor.Yellow)
				{
					if (concatarraylist(new List<GameObject>[5]
					{
						Grey.ToList(),
						Red.ToList(),
						White.ToList(),
						Blue.ToList(),
						Green.ToList()
					}).Length == 0)
					{
						phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Yellow", false);
					}
					if (teammembersout(team).Count == 0)
					{
						phopview.RPC("teamfreeforallwon", PhotonTargets.All, "AI wins", true);
					}
				}
			}
		}
		if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip || (crosslevelholder.campaign && crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.killTarget))
		{
			if (!crosslevelholder.campaign && Team1Capitals.Count == 0 && !isalreadywon)
			{
				phopview.RPC("team2won", PhotonTargets.All, false);
			}
			if (Team2Capitals.Count == 0 && !isalreadywon)
			{
				phopview.RPC("team1won", PhotonTargets.All, false);
			}
		}
		if (Timeleft < 0 && (!crosslevelholder.campaign || crosslevelholder.campaignlevel.objective != MainMenuCampaignControlScript.eMissionObjective.Survive))
		{
			teamfreeforallwon("time ran out", customstring: true);
		}
		if (Timeleft < 0 && crosslevelholder.campaign && crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive)
		{
			team1won(endallin: false);
		}
	}

	private void removenullentities(ref GameObject[] input)
	{
		GameObject addition = null;
		GameObject[] array = input;
		foreach (GameObject gameObject in array)
		{
			if (gameObject == null || gameObject.gameObject == null)
			{
				addition = gameObject;
			}
		}
		Removefromarray(ref input, addition);
	}

	private GameObject[] concatarraylist(List<GameObject>[] input)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (List<GameObject> list2 in input)
		{
			foreach (GameObject item in list2)
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	[PunRPC]
	private void teamfreeforallwon(string teamin, bool customstring)
	{
		isalreadywon = true;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Voice");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.GetComponent<AudioSource>().enabled = true;
		}
		StartCoroutine("wait");
		GameObject gameObject2 = Object.Instantiate(Team1victory);
		if ((bool)GameObject.Find("DragOBJ"))
		{
			gameObject2.transform.parent = GameObject.Find("DragOBJ").transform;
			gameObject2.transform.localPosition = new Vector3(0f, 1f, 4.25f);
			gameObject2.transform.localScale = new Vector3(15f, 2f, 2f);
			if (!customstring)
			{
				gameObject2.transform.Find("New Text").GetComponent<TextMesh>().text = "Team : " + teamin + " Wins!";
			}
			else
			{
				gameObject2.transform.Find("New Text").GetComponent<TextMesh>().text = teamin;
			}
		}
	}

	public static _Ship.eShipColor findspawnteam(int input)
	{
		_Ship.eShipColor result = _Ship.eShipColor.Green;
		if (input == 1)
		{
			result = _Ship.eShipColor.Green;
		}
		if (input == 2)
		{
			result = _Ship.eShipColor.Blue;
		}
		if (input == 3)
		{
			result = _Ship.eShipColor.White;
		}
		if (input == 4)
		{
			result = _Ship.eShipColor.Yellow;
		}
		if (input == 5)
		{
			result = _Ship.eShipColor.Grey;
		}
		if (input == 6)
		{
			result = _Ship.eShipColor.Red;
		}
		return result;
	}

	public static int findspawnteamreverse(_Ship.eShipColor input)
	{
		int result = 1;
		if (input == _Ship.eShipColor.Green)
		{
			result = 1;
		}
		if (input == _Ship.eShipColor.Blue)
		{
			result = 2;
		}
		if (input == _Ship.eShipColor.White)
		{
			result = 3;
		}
		if (input == _Ship.eShipColor.Yellow)
		{
			result = 4;
		}
		if (input == _Ship.eShipColor.Grey)
		{
			result = 5;
		}
		if (input == _Ship.eShipColor.Red)
		{
			result = 6;
		}
		return result;
	}

	private void VictoryCampaign()
	{
		if (crosslevelholder.campaignlevel != null)
		{
			FleetBuilder = IORoot.findIO("fleet1");
			FleetBuilder.read();
			int num = FleetBuilder.get<int>("Completednum");
			FleetBuilder.read();
			if (num < crosslevelholder.campaignlevel.panelnum + 1)
			{
				FleetBuilder.add("Completednum", crosslevelholder.campaignlevel.panelnum + 1);
				FleetBuilder.write();
			}
		}
	}

	[PunRPC]
	public void team1won(bool endallin)
	{
		victorytest = true;
		if (crosslevelholder.campaign)
		{
			VictoryCampaign();
		}
		isalreadywon = true;
		Debug.Log("Team1won");
		GameObject[] array = GameObject.FindGameObjectsWithTag("Voice");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.GetComponent<AudioSource>().enabled = true;
		}
		StartCoroutine("wait");
		GameObject gameObject2 = Object.Instantiate(Team1victory);
		gameObject2.transform.parent = GameObject.Find("DragOBJ").transform;
		gameObject2.transform.localPosition = new Vector3(0f, 1f, 4.25f);
		gameObject2.transform.localScale = new Vector3(15f, 2f, 2f);
		int num = getteam(team, crosslevelholder.Gamemode);
		if (num == 1)
		{
			gameObject2.transform.Find("New Text").GetComponent<TextMesh>().text = "Victory!";
		}
		else
		{
			gameObject2.transform.Find("New Text").GetComponent<TextMesh>().text = "Defeat!";
		}
		if (endallin)
		{
			endall = true;
		}
	}

	[PunRPC]
	public void team2won(bool endallin)
	{
		victorytest = false;
		isalreadywon = true;
		Debug.Log("Team2won");
		GameObject[] array = GameObject.FindGameObjectsWithTag("Voice");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.GetComponent<AudioSource>().enabled = true;
		}
		StartCoroutine("wait");
		GameObject gameObject2 = Object.Instantiate(Team2Victory);
		if ((bool)GameObject.Find("DragOBJ"))
		{
			gameObject2.transform.parent = GameObject.Find("DragOBJ").transform;
			gameObject2.transform.localPosition = new Vector3(0f, 1f, 4.25f);
			gameObject2.transform.localScale = new Vector3(15f, 2f, 2f);
			int num = getteam(team, crosslevelholder.Gamemode);
			if (num == 2)
			{
				gameObject2.transform.Find("New Text").GetComponent<TextMesh>().text = "Victory!";
			}
			else
			{
				gameObject2.transform.Find("New Text").GetComponent<TextMesh>().text = "Defeat!";
			}
			if (endallin)
			{
				endall = true;
			}
		}
	}

	[PunRPC]
	public void HostLeft()
	{
		victorytest = false;
		Debug.Log("hostleft");
		GameObject[] array = GameObject.FindGameObjectsWithTag("Voice");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.GetComponent<AudioSource>().enabled = true;
		}
		StartCoroutine("wait");
		if ((bool)GameObject.Find("DragOBJ"))
		{
			GameObject gameObject2 = Object.Instantiate(hostlefticon);
			if (PhotonNetwork.offlineMode)
			{
				gameObject2.transform.Find("New Text").GetComponent<TextMesh>().text = "Left Game";
			}
			gameObject2.transform.parent = GameObject.Find("DragOBJ").transform;
			gameObject2.transform.localPosition = new Vector3(0f, 1f, 4.25f);
			gameObject2.transform.localScale = new Vector3(15f, 2f, 2f);
		}
		endall = true;
	}

	private GameObject getcapitalofteam(_Ship.eShipColor input)
	{
		GameObject result = null;
		foreach (Transform item in Objectsholder.transform)
		{
			if (item.name == "Super-Capital(Clone)")
			{
				_Ship component = item.GetComponent<_Ship>();
				if (component.ShipColor == input)
				{
					result = component.gameObject;
				}
			}
		}
		return result;
	}

	public bool checkiffactiondead(_Ship.eShipColor input)
	{
		bool result = false;
		if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch || crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.FreeForAll)
		{
			if (teammembersout(input).Count == 0)
			{
				result = true;
			}
		}
		else if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip && getcapitalofteam(input) == null)
		{
			result = true;
		}
		return result;
	}

	public IEnumerator wait()
	{
		if ((bool)GameObject.Find("ResizeObj"))
		{
			GameObject.Find("ResizeObj").GetComponent<Resizescript>().forcereturntonormal();
		}
		yield return new WaitForSeconds(5f);
		bool test = PhotonNetwork.offlineMode;
		if ((bool)GameObject.Find("ResizeObj"))
		{
			GameObject.Find("ResizeObj").GetComponent<Resizescript>().forcereturntonormal();
		}
		transform.localScale = new Vector3(1f, 1f, 1f);
		if (crosslevelholder.campaign && !crosslevelholder.campaignnextlevelset)
		{
			crosslevelholder.campaignnextlevelset = true;
			if (crosslevelholder.campaignlevel.panelnum < MainMenuCampaignControlScript.maps.Length - 1)
			{
				if (victorytest)
				{
					crosslevelholder.campaignlevel = MainMenuCampaignControlScript.maps[crosslevelholder.campaignlevel.panelnum + 1];
				}
				else if (victorytest)
				{
					Debug.Log("Campain == false");
					GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().campaign = false;
					crosslevelholder.campaign = false;
				}
			}
		}
		bool temp = crosslevelholder.campaign;
		if (endall || (test && !temp))
		{
			PhotonNetwork.LeaveRoom();
			PhotonNetwork.LoadLevel(0);
		}
		else
		{
			PhotonNetwork.LoadLevel(1);
		}
	}

	public static int getmaxshipnumbers()
	{
		int result = 0;
		if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers == 2)
		{
			result = 23;
		}
		if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "No")
		{
			result = 14;
		}
		if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
		{
			result = 9;
		}
		return result;
	}

	public static int getteam(_Ship.eShipColor teamin, CrossLevelVariableHolder.gamemode inputgamemode)
	{
		int result = 0;
		if (inputgamemode == CrossLevelVariableHolder.gamemode.CapitalShip || inputgamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch)
		{
			result = ((teamin == _Ship.eShipColor.Blue || teamin == _Ship.eShipColor.Green || teamin == _Ship.eShipColor.White) ? 1 : 2);
		}
		else
		{
			if (teamin == _Ship.eShipColor.Green)
			{
				result = 1;
			}
			if (teamin == _Ship.eShipColor.Blue)
			{
				result = 2;
			}
			if (teamin == _Ship.eShipColor.White)
			{
				result = 3;
			}
			if (teamin == _Ship.eShipColor.Yellow)
			{
				result = 4;
			}
			if (teamin == _Ship.eShipColor.Grey)
			{
				result = 5;
			}
			if (teamin == _Ship.eShipColor.Red)
			{
				result = 6;
			}
		}
		return result;
	}

	public void fogcheck()
	{
		List<TSTransform> list = alliesout();
		foreach (TSTransform item in targetsout(team))
		{
			bool cloak = true;
			foreach (TSTransform item2 in list)
			{
				if ((bool)item2 && (bool)item && item.gameObject != null && item2.gameObject != null && Vector3.Distance(item.transform.localPosition, item2.transform.localPosition) < 14000f)
				{
					cloak = false;
				}
			}
			if ((bool)item && item.gameObject != null)
			{
				item.GetComponent<_Ship>().assigncloackfog(cloak);
			}
		}
	}

	public string gettimeleft()
	{
		float num = Mathf.Floor(Timeleft.AsFloat() / 60f);
		float num2 = Mathf.RoundToInt(Timeleft.AsFloat() % 60f);
		string empty = string.Empty;
		if (num2 > 9f)
		{
			return num + ":" + num2;
		}
		return num + ":0" + num2;
	}

	public List<TSTransform> targetsout(_Ship.eShipColor control)
	{
		List<TSTransform> result = new List<TSTransform>();
		switch (control)
		{
		case _Ship.eShipColor.Green:
			result = GreenTargets.ToList();
			break;
		case _Ship.eShipColor.Blue:
			result = BlueTargets.ToList();
			break;
		case _Ship.eShipColor.White:
			result = WhiteTargets.ToList();
			break;
		case _Ship.eShipColor.Yellow:
			result = YellowTargets.ToList();
			break;
		case _Ship.eShipColor.Grey:
			result = GreyTargets.ToList();
			break;
		case _Ship.eShipColor.Red:
			result = RedTargets.ToList();
			break;
		}
		return result;
	}

	private bool checkelidgable(Transform t)
	{
		if (t.name != "Engines" && t.name != "Working")
		{
			return true;
		}
		return false;
	}

	[PunRPC]
	public void runsim()
	{
		TrueSyncManager.RunSimulation();
	}

	private void Update()
	{
		timepassed += (FP)Time.deltaTime;
		if (Application.isEditor && Input.GetKeyDown(KeyCode.S))
		{
			phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Green", false);
		}
		if (timepassed > 3 && !simrunning && PhotonNetwork.isMasterClient)
		{
			phopview = GetComponent<PhotonView>();
			phopview.RPC("runsim", PhotonTargets.AllViaServer);
			simrunning = true;
		}
		if (!simrunning || started || !PhotonNetwork.isMasterClient)
		{
			return;
		}
		bool flag = true;
		foreach (playerisreadychecker player in players)
		{
			if (!player.isready)
			{
				flag = false;
			}
		}
		if (flag)
		{
			GameObject.Find("TrueSyncManager").GetComponent<RelayController>().relay.GetComponent<InputRelay>().runsimulationnow = true;
			Debug.Log("runsim");
			started = true;
		}
	}

	private void SpawnShipInput(GameObject spawnobj, TSVector startpos, int spawnori, int viewidin)
	{
		if (crosslevelholder.tutorial)
		{
			tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightMenuShipBuy);
		}
		GameObject gameObject = TrueSyncManager.SyncedInstantiate(spawnobj);
		gameObject.transform.parent = GameObject.Find("Objects").transform;
		gameObject.transform.GetComponent<TSTransform>().rotation = PositinonRelativeToHeadset.getspawnrotation(_Ship.getteambynumber(spawnori));
		gameObject.transform.localScale = new Vector3(8f, 8f, 8f);
		gameObject.GetComponent<TSTransform>().position = startpos;
		debug = gameObject;
		gameObject.GetComponent<_Ship>().spawnnum = spawnori;
		if (viewidin != 0)
		{
			gameObject.GetComponent<PhotonView>().viewID = viewidin;
		}
		gameObject.GetComponent<_Ship>().StartMain();
	}

	public void spawnship(int spawner, TSVector startpos, int spawnori, int viewidin)
	{
		if (running)
		{
			SpawnShipInput(getshipbynumber(spawner), startpos, spawnori, viewidin);
		}
	}

	public GameObject getshipbynumber(int i)
	{
		GameObject result = null;
		switch (i)
		{
		case 1:
			result = Scout;
			break;
		case 2:
			result = Fighter;
			break;
		case 3:
			result = Bomber;
			break;
		case 4:
			result = AttackCraft;
			break;
		case 5:
			result = MissileFrigate;
			break;
		case 6:
			result = FlackFrigate;
			break;
		case 7:
			result = CannonFriage;
			break;
		case 8:
			result = LazerFrigate;
			break;
		case 9:
			result = SupportFrigate;
			break;
		case 10:
			result = RepairFrigate;
			break;
		case 11:
			result = Destroyer;
			break;
		case 12:
			result = Capital;
			break;
		case 13:
			result = FlagShip;
			break;
		}
		return result;
	}

	public void ordershipspawn(int a, TSVector b, int c, int d)
	{
		if (InputRelay == null)
		{
			InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
		}
		if (InputRelay != null)
		{
			InputRelay.ordershipspawn(a, b, c, d);
		}
	}

	[PunRPC]
	public void SetTransmittmode(int channeltarget, int playertarget)
	{
		PlayerPrefabVoiceControlScript playerPrefabVoiceControlScript = null;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Voice");
		foreach (GameObject gameObject in array)
		{
			PlayerPrefabVoiceControlScript component = gameObject.GetComponent<PlayerPrefabVoiceControlScript>();
			if (component != null && component.locview.owner.ID == playertarget)
			{
				playerPrefabVoiceControlScript = component;
			}
		}
		if (playerPrefabVoiceControlScript != null)
		{
			if (channeltarget == 0)
			{
				playerPrefabVoiceControlScript.curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.Muteall;
			}
			if (channeltarget == 1)
			{
				playerPrefabVoiceControlScript.curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly;
			}
			if (channeltarget == 2)
			{
				playerPrefabVoiceControlScript.curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.All;
			}
		}
	}

	public void switchvoice(PlayerPrefabVoiceControlScript.VoiceChannel input, int playertarget)
	{
		curchannel = input;
		phopview.RPC("SetTransmittmode", PhotonTargets.All, (int)input, playertarget);
	}
}
