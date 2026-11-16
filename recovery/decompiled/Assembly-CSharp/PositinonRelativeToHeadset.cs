using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PositinonRelativeToHeadset : TrueSyncBehaviour
{
	public GameObject cam1;

	public GameObject cam2;

	public RelayController InputRelay;

	public GameObject Reasourcesthing;

	public TextMesh reasourcestextmesh;

	public UnitMovementcommandcontroller unitcon;

	public CrossLevelVariableHolder crossvar;

	private BuildQueController bulcontrol;

	public GameObject Fighters;

	public GameObject Frigates;

	public GameObject Special;

	public GameObject CameraObj;

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
	{
		if ((bool)Reasourcesthing && reasourcestextmesh == null)
		{
			reasourcestextmesh = Reasourcesthing.GetComponent<TextMesh>();
		}
		if (GameObject.Find("Controllers") != null)
		{
			if ((bool)GameObject.Find("Controllers"))
			{
				unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
			}
			if ((bool)GameObject.Find("CrossLevelVariables"))
			{
				crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
			}
		}
		if (this != null && gameObject != null && scene.name != null)
		{
			gameObject.SetActive(value: false);
		}
		if (this != null && gameObject != null && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
		{
			unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
			crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
		}
		if (!(Fighters != null))
		{
			return;
		}
		Fighters.SetActive(value: true);
		Frigates.SetActive(value: true);
		Special.SetActive(value: true);
		if (crossvar.campaign)
		{
			if (crossvar.campaignlevel.shipsavaiable == MainMenuCampaignControlScript.eshipsavailable.fighers)
			{
				Fighters.SetActive(value: true);
				Frigates.SetActive(value: false);
				Special.SetActive(value: false);
			}
			else if (crossvar.campaignlevel.shipsavaiable == MainMenuCampaignControlScript.eshipsavailable.frigates)
			{
				Fighters.SetActive(value: true);
				Frigates.SetActive(value: true);
				Special.SetActive(value: false);
			}
			else if (crossvar.campaignlevel.shipsavaiable == MainMenuCampaignControlScript.eshipsavailable.all)
			{
				Fighters.SetActive(value: true);
				Frigates.SetActive(value: true);
				Special.SetActive(value: true);
			}
		}
	}

	private void Awake()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		bulcontrol = GetComponent<BuildQueController>();
		OnLevelFinishedLoading(default, LoadSceneMode.Single);
	}

	public override void OnSyncedStart()
	{
		InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
	}

	private void Update()
	{
		if ((bool)Reasourcesthing)
		{
			if (unitcon == null)
			{
				unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
			}
			if (reasourcestextmesh == null)
			{
				reasourcestextmesh = Reasourcesthing.GetComponent<TextMesh>();
			}
			if ((bool)unitcon)
			{
				reasourcestextmesh.text = "Resources : " + unitcon.getmoney();
			}
		}
	}

	public void TurnOn()
	{
		GameObject gameObject = null;
		if (cam1 != null && cam1.gameObject != null && cam1.GetActive())
		{
			gameObject = cam1;
		}
		else if (cam2 != null && cam2.gameObject != null && cam2.GetActive())
		{
			gameObject = cam2;
		}
		transform.position = CameraObj.transform.position;
		transform.LookAt(gameObject.transform.position);
		transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
		if (unitcon == null)
		{
			unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
			crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
		}
	}

	public void spawnship(int a, string name, float timetospawn)
	{
		if (a != 0)
		{
			if (unitcon == null)
			{
				unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
				crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
			}
			if (checkmoney(unitcon.getmoney(), a))
			{
				bulcontrol.addbuildorder(new BuildQueController.buildorder(name, a, 1, timetospawn));
			}
		}
	}

	public void spawnshiprightnow(int a)
	{
		if (InputRelay == null)
		{
			InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
		}
		if (unitcon == null)
		{
			unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
			crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
		}
		if (InputRelay != null)
		{
			InputRelay.ordershipspawn(a, getspawnpos(unitcon.team), crossvar.findspawnpos(PhotonNetwork.player.ID), PhotonNetwork.AllocateViewID());
		}
	}

	public static TSVector getspawnpos(_Ship.eShipColor teamin)
	{
		List<GameObject> list = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>().teammembersout(teamin);
		TSVector tSVector = new TSVector(0, 0, 0);
		if (list.Count > 0)
		{
			if (list[0].gameObject != null)
			{
				tSVector = list[0].GetComponent<TSTransform>().position;
			}
			if (list.Count == 1)
			{
				tSVector += new TSVector(300, 300, 300);
			}
			tSVector += new TSVector(5, 5, 5) * list.Count;
			foreach (GameObject item in list)
			{
				if (item.gameObject != null && item != list[0])
				{
					tSVector = item.transform.GetComponent<TSTransform>().position + tSVector;
				}
			}
			return tSVector / list.Count;
		}
		return tSVector;
	}

	public static TSQuaternion getspawnrotation(_Ship.eShipColor teamin)
	{
		List<GameObject> list = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>().teammembersout(teamin);
		if (list.Count > 0)
		{
			GameObject gameObject = getrotationprime(list);
			if (gameObject != null && gameObject.gameObject != null)
			{
				return gameObject.GetComponent<TSTransform>().rotation;
			}
			return new TSQuaternion(0, 0, 0, 0);
		}
		return new TSQuaternion(0, 0, 0, 0);
	}

	public static GameObject getrotationprime(List<GameObject> ships)
	{
		GameObject gameObject = null;
		foreach (GameObject ship in ships)
		{
			if (ship.gameObject != null && ship.GetComponent<_Ship>().HullType != eHullType.Light)
			{
				gameObject = ship;
			}
		}
		if (gameObject == null)
		{
			gameObject = ships[0];
		}
		return gameObject;
	}

	public bool checkmoney(int money, int spawner)
	{
		bool flag = false;
		int pointsToDeploy = unitcon.getshipbynumber(spawner).GetComponent<_Ship>().PointsToDeploy;
		flag = ((pointsToDeploy <= money) ? true : false);
		int num = UnitMovementcommandcontroller.getmaxshipnumbers();
		int count = unitcon.teammembersout(unitcon.team).Count;
		if (count >= num - 1)
		{
			flag = false;
		}
		if (!unitcon.running)
		{
			flag = false;
		}
		if (flag)
		{
			unitcon.takemoney(pointsToDeploy);
		}
		if (unitcon.checkiffactiondead(unitcon.team))
		{
			flag = false;
		}
		return flag;
	}

	public void Scout()
	{
		spawnship(1, "Scout", 5f);
	}

	public void Fighter()
	{
		spawnship(2, "Fighter", 7f);
	}

	public void Bomber()
	{
		spawnship(3, "Bomber", 7f);
	}

	public void AttackCraft()
	{
		spawnship(4, "Attack Craft", 8f);
	}

	public void MissileFrigate()
	{
		spawnship(5, "Missile Frigate", 6f);
	}

	public void FlackFrigate()
	{
		spawnship(6, "Flack Frigate", 6f);
	}

	public void CannonFrigate()
	{
		spawnship(7, "Cannon Frigate", 7f);
	}

	public void LazerFrigate()
	{
		spawnship(8, "Lazer Frigate", 6f);
	}

	public void SupportFrigate()
	{
		spawnship(9, "Support Frigate", 6f);
	}

	public void RepairFrigate()
	{
		spawnship(10, "Repair Frigate", 8f);
	}

	public void Destroyer()
	{
		spawnship(11, "Destroyer", 15f);
	}

	public void Capital()
	{
		spawnship(12, "Capital", 20f);
	}
}
