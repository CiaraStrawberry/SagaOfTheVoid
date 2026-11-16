using UnityEngine;
using simple;

public class MainMenuCampaignControlScript : MonoBehaviour
{
	public enum eMissionObjective
	{
		Destroyall,
		Survive,
		killTarget
	}

	public enum eshipsavailable
	{
		fighers,
		frigates,
		all
	}

	public class mapcontainer
	{
		public string name;

		public int[] ships;

		public CrossLevelVariableHolder.mapcon map;

		public CrossLevelVariableHolder.skyboxcon skybox;

		public int panelnum;

		public int startmoney;

		public string story;

		public eMissionObjective objective;

		public eshipsavailable shipsavaiable;

		public mapcontainer(string namein, int[] shipsin, CrossLevelVariableHolder.mapcon mapin, CrossLevelVariableHolder.skyboxcon skyboxin, int panelnumin, int startmoneyin, eMissionObjective objectivein, string storyin, eshipsavailable shipsavaialbein)
		{
			name = namein;
			ships = shipsin;
			map = mapin;
			skybox = skyboxin;
			panelnum = panelnumin;
			startmoney = startmoneyin;
			story = storyin;
			objective = objectivein;
			shipsavaiable = shipsavaialbein;
		}
	}

	public static mapcontainer[] maps = new mapcontainer[8];

	private int countthrough;

	public TextMesh[] bts = new TextMesh[4];

	public TextMesh[] completedicons = new TextMesh[4];

	public GameObject[] panels = new GameObject[8];

	private IOComponent FleetBuilder;

	public int completedthrough;

	private void Start()
	{
		PhotonNetwork.Disconnect();
		PhotonNetwork.offlineMode = true;
		Populatemaps();
		loadshipstotext(0);
	}

	public void OnDisconnectedFromPhoton()
	{
		PhotonNetwork.offlineMode = true;
		Debug.Log("offlinemode activated");
	}

	private void Populatemaps()
	{
		maps[0] = new mapcontainer("Basic Training", new int[4] { 1, 1, 1, 1 }, CrossLevelVariableHolder.mapcon.map1, CrossLevelVariableHolder.skyboxcon.skybox3, 0, 2000, eMissionObjective.Destroyall, "Practice your skills \nagainst drones in \npreperation for \ndefense of the  \nempire.", eshipsavailable.fighers);
		maps[1] = new mapcontainer("Core Outskirts", new int[3] { 3, 3, 3 }, CrossLevelVariableHolder.mapcon.map2, CrossLevelVariableHolder.skyboxcon.skybox1, 1, 2000, eMissionObjective.Destroyall, "Rebels have been \nsending sorties into \ncivilian shiping lanes. \n\nDestroy their ships \nand protect our \neconomic interests.", eshipsavailable.fighers);
		maps[2] = new mapcontainer("System Patrol", new int[4] { 7, 7, 7, 13 }, CrossLevelVariableHolder.mapcon.map3, CrossLevelVariableHolder.skyboxcon.skybox4, 2, 4000, eMissionObjective.killTarget, "The outskirts have \nalways been a pit \nof despair and piracy. \n\n A Famed Pirate boss \nhas been located \nconducting a raid. \nend him.", eshipsavailable.fighers);
		maps[3] = new mapcontainer("Pirate Base", new int[5] { 6, 2, 2, 2, 2 }, CrossLevelVariableHolder.mapcon.map1, CrossLevelVariableHolder.skyboxcon.skybox5, 3, 5500, eMissionObjective.Destroyall, "Our intelligence has \nlocated one of their \ndens. \nDestroy it.", eshipsavailable.frigates);
		maps[4] = new mapcontainer("Military Excursion", new int[3] { 7, 7, 0 }, CrossLevelVariableHolder.mapcon.map1, CrossLevelVariableHolder.skyboxcon.skybox3, 4, 4000, eMissionObjective.Survive, "We need you to make \na full excursion into \nrebel territory \nand recorver a high \nranking prisoner.", eshipsavailable.frigates);
		maps[5] = new mapcontainer("All out war", new int[8] { 3, 3, 3, 2, 2, 2, 11, 9 }, CrossLevelVariableHolder.mapcon.map2, CrossLevelVariableHolder.skyboxcon.skybox4, 5, 8000, eMissionObjective.Destroyall, "The rebels have \nlaunched an all out \nassault on the \nCapital. \n\nDestroy one of their \nen-route fleets.", eshipsavailable.frigates);
		maps[6] = new mapcontainer("Final Assault", new int[3] { 11, 11, 0 }, CrossLevelVariableHolder.mapcon.map3, CrossLevelVariableHolder.skyboxcon.skybox3, 6, 6000, eMissionObjective.Survive, "The Imperial palace \nis under siege! \n\nyou need to survive \nagainst the rebel \nfleets!", eshipsavailable.all);
		maps[7] = new mapcontainer("Last Hope", new int[11]
		{
			12, 11, 11, 3, 3, 3, 3, 3, 2, 2,
			13
		}, CrossLevelVariableHolder.mapcon.map1, CrossLevelVariableHolder.skyboxcon.skybox4, 7, 12000, eMissionObjective.killTarget, "The rebel flagship is \nexposed. \n\nstrike now and \nend the rebellion.", eshipsavailable.all);
		GetCompleted();
		updatedisplay();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			loadcampaignactual(0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			loadcampaignactual(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			loadcampaignactual(2);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			loadcampaignactual(3);
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			loadcampaignactual(4);
		}
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			loadcampaignactual(5);
		}
		if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			loadcampaignactual(6);
		}
		if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			loadcampaignactual(7);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			next();
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			last();
		}
	}

	private void GetCompleted()
	{
		FleetBuilder = IORoot.findIO("fleet1");
		FleetBuilder.read();
		completedthrough = FleetBuilder.get<int>("Completednum");
		Debug.Log(completedthrough);
	}

	private void updatedisplay()
	{
		GameObject[] array = panels;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject && gameObject.gameObject != null)
			{
				gameObject.SetActive(value: false);
			}
		}
		for (int j = 0; j < 4; j++)
		{
			bts[j].text = maps[j + countthrough * 4].name;
			if (panels[maps[j + countthrough * 4].panelnum] != null)
			{
				panels[maps[j + countthrough * 4].panelnum].SetActive(value: true);
			}
			Debug.Log(j + countthrough * 4);
			if (j + countthrough * 4 < completedthrough)
			{
				completedicons[j].text = "✓";
			}
			else if (j + countthrough * 4 < completedthrough + 2)
			{
				completedicons[j].text = string.Empty;
			}
			else
			{
				completedicons[j].text = "x";
			}
		}
	}

	public void next()
	{
		if (countthrough < 1)
		{
			countthrough++;
		}
		updatedisplay();
	}

	public void last()
	{
		if (countthrough > 0)
		{
			countthrough--;
		}
		updatedisplay();
	}

	private void loadcampaign(int map)
	{
		if (completedicons[map].text != "x")
		{
			loadcampaignactual(map);
		}
	}

	private void loadcampaignactual(int map)
	{
		MainMenuValueHolder component = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
		component.campaign = true;
		component.selectedcampaign = maps[map + countthrough * 4];
		GameObject.Find("Controllers").GetComponent<GalaxyController>().findagame();
	}

	public void Button1()
	{
		loadcampaign(0);
	}

	public void Button2()
	{
		loadcampaign(1);
	}

	public void Button3()
	{
		loadcampaign(2);
	}

	public void Button4()
	{
		loadcampaign(3);
	}

	public void ButtonHover1()
	{
		loadshipstotext(0);
	}

	public void ButtonHover2()
	{
		loadshipstotext(1);
	}

	public void ButtonHover3()
	{
		loadshipstotext(2);
	}

	public void ButtonHover4()
	{
		loadshipstotext(3);
	}

	private void loadshipstotext(int map)
	{
		mapcontainer mapcontainer2 = maps[map + countthrough * 4];
	}

	public static string getshipnamebynumber(int i)
	{
		string result = null;
		switch (i)
		{
		case 0:
			result = "+Reinforcements";
			break;
		case 1:
			result = "Scout";
			break;
		case 2:
			result = "Fighter";
			break;
		case 3:
			result = "Bomber";
			break;
		case 4:
			result = "AttackCraft";
			break;
		case 5:
			result = "MissileFrigate";
			break;
		case 6:
			result = "FlackFrigate";
			break;
		case 7:
			result = "CannonFriage";
			break;
		case 8:
			result = "LazerFrigate";
			break;
		case 9:
			result = "SupportFrigate";
			break;
		case 10:
			result = "RepairFrigate";
			break;
		case 11:
			result = "Destroyer";
			break;
		case 12:
			result = "Capital";
			break;
		case 13:
			result = "FlagShip";
			break;
		}
		return result;
	}
}
