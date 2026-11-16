using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using simple;

public class GalaxyController : MonoBehaviour
{
	private GameObject Galaxy;

	public GameObject Singleplayer;

	private GameObject SinglePlayerCollider;

	public GameObject Multiplayer;

	public GameObject MultiplayerCollider;

	public GameObject Settings;

	private GameObject SettingsCollider;

	public GameObject Ship1;

	public GameObject Ship2;

	public GameObject Ship3;

	public GameObject SinglePlayerTestLevel;

	public GameObject ShipBuilder;

	public GameObject ShipBuilderMain;

	public GameObject fleetBuilder;

	public GameObject ShipParent;

	public GameObject Menu;

	public GameObject settingsparent;

	public GameObject fleetbuildermenu;

	public GameObject findgamemenu;

	private IOComponent FleetBuilder;

	public GameObject deniedobj;

	public bool startup;

	public int wait;

	private Vector3 startpos;

	private Vector3 galstartpos2;

	public GameObject CampaignController;

	public GameObject CreateGameMenu;

	public GameObject creditsobj;

	public void deniedsort()
	{
		deniedobj.SetActive(value: true);
	}

	private void Awake()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("TutorialThings");
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(value: false);
		}
		Galaxy = GameObject.Find("GalaxyParent");
		startpos = Galaxy.transform.position;
		galstartpos2 = Galaxy.transform.Find("Galaxy").position;
		findgamemenu.SetActive(value: false);
		returntonormal();
	}

	public void fleetbuildersort()
	{
		Vector3 position = Galaxy.transform.Find("Galaxy").position;
		Galaxy.transform.position = Singleplayer.transform.position;
		Galaxy.transform.Find("Galaxy").position = position;
		mainmenuDisable();
		SinglePlayerTestLevel.SetActive(value: false);
		fleetBuilder.SetActive(value: false);
		ShipBuilderMain.SetActive(value: false);
		Menu.SetActive(value: false);
		settingsparent.SetActive(value: false);
		fleetbuildermenu.SetActive(value: true);
		iTween.ScaleTo(Galaxy, new Vector3(6f, 6f, 6f), 8f);
		Debug.Log("fleetbuilder");
	}

	private void Start()
	{
		if ((bool)GameObject.Find("Team1wins(Clone)"))
		{
			Object.Destroy(GameObject.Find("Team1wins(Clone)"));
		}
		if ((bool)GameObject.Find("Team2Wins(Clone)"))
		{
			Object.Destroy(GameObject.Find("Team2wins(Clone)"));
		}
		MultiplayerCollider = Multiplayer.transform.Find("Multiplayer").gameObject;
		Ship1.SetActive(value: false);
		Ship2.SetActive(value: false);
		Ship3.SetActive(value: false);
		ShipBuilder.SetActive(value: false);
		SinglePlayerTestLevel.SetActive(value: false);
		ShipBuilder.SetActive(value: true);
		Galaxy.SetActive(value: true);
		Multiplayer.SetActive(value: true);
		Singleplayer.SetActive(value: true);
		ShipBuilderMain.SetActive(value: false);
		fleetBuilder.SetActive(value: false);
		ShipParent.SetActive(value: false);
		fleetbuildermenu.SetActive(value: false);
		mainmenuDisable();
		returntonormal();
		if (startup)
		{
			findagame();
		}
		Debug.Log("start");
		FleetBuilder = IORoot.findIO("fleet1");
		if (!FleetBuilder)
		{
			return;
		}
		FleetBuilder.read();
		string text = FleetBuilder.get<string>("Settings");
		FleetBuilder.read();
		switch (text)
		{
		case "High":
			QualitySettings.SetQualityLevel(5, applyExpensiveChanges: true);
			break;
		case "Medium":
			QualitySettings.SetQualityLevel(3, applyExpensiveChanges: true);
			break;
		case "Low":
			QualitySettings.SetQualityLevel(1, applyExpensiveChanges: true);
			break;
		default:
			if (text == null)
			{
				FleetBuilder.add("Settings", "Medium");
				FleetBuilder.write();
				QualitySettings.SetQualityLevel(3, applyExpensiveChanges: true);
				text = "Medium";
			}
			break;
		}
		Debug.Log(text);
		returntonormal();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			shipbuilderbuilder();
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			findagame();
		}
		if (Input.GetKeyDown(KeyCode.Z))
		{
			fleetbuildersort();
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			Tutorial();
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			CampaignTrailsort();
		}
		wait++;
		if (wait == 1)
		{
			ShipParent.SetActive(value: true);
		}
		if (wait == 2)
		{
			ShipParent.GetComponent<MainMenuShipController>().checkresetall();
		}
		if (wait == 3)
		{
			ShipParent.SetActive(value: false);
		}
	}

	private void mainmenuDisable()
	{
		Singleplayer.SetActive(value: false);
		Multiplayer.SetActive(value: false);
		Settings.SetActive(value: false);
		MultiplayerCollider.SetActive(value: false);
		ShipBuilder.SetActive(value: false);
		Menu.SetActive(value: false);
		fleetbuildermenu.SetActive(value: false);
		findgamemenu.SetActive(value: false);
		creditsobj.SetActive(value: false);
		CampaignController.SetActive(value: false);
	}

	public void credits()
	{
		Vector3 position = Galaxy.transform.Find("Galaxy").position;
		Galaxy.transform.position = ShipBuilder.transform.position;
		Galaxy.transform.Find("Galaxy").position = position;
		mainmenuDisable();
		ShipBuilderMain.SetActive(value: false);
		fleetBuilder.SetActive(value: false);
		settingsparent.SetActive(value: false);
		findgamemenu.SetActive(value: false);
		iTween.ScaleTo(Galaxy, new Vector3(6f, 6f, 6f), 8f);
		Debug.Log("settiings");
		creditsobj.SetActive(value: true);
	}

	public void returntonormal()
	{
		SinglePlayerTestLevel.SetActive(value: false);
		Ship1.SetActive(value: false);
		Ship2.SetActive(value: false);
		Ship3.SetActive(value: false);
		Galaxy.SetActive(value: true);
		ShipParent.SetActive(value: false);
		findgamemenu.SetActive(value: false);
		Galaxy.transform.position = startpos;
		Galaxy.transform.Find("Galaxy").transform.position = galstartpos2;
		Menu.SetActive(value: true);
		fleetBuilder.SetActive(value: false);
		fleetbuildermenu.SetActive(value: false);
		settingsparent.SetActive(value: false);
		CampaignController.SetActive(value: false);
		creditsobj.SetActive(value: false);
		CreateGameMenu.SetActive(value: false);
		if (PhotonNetwork.inRoom)
		{
			PhotonNetwork.LeaveRoom();
		}
		GameObject.Find("MainMenuControlParent").GetComponent<MainMenuButtonControlScript>().returntomenu();
		iTween.ScaleTo(Galaxy, new Vector3(1f, 1f, 1f), 8f);
	}

	public void CampaignTrailsort()
	{
		mainmenuDisable();
		CampaignController.SetActive(value: true);
	}

	public void settingsscaleup()
	{
		Vector3 position = Galaxy.transform.Find("Galaxy").position;
		Galaxy.transform.position = Settings.transform.position;
		Galaxy.transform.Find("Galaxy").position = position;
		mainmenuDisable();
		iTween.ScaleTo(Galaxy, new Vector3(6f, 6f, 6f), 8f);
		Debug.Log("settiings");
	}

	public void scaledown()
	{
		Singleplayer.SetActive(value: true);
		Multiplayer.SetActive(value: true);
		Settings.SetActive(value: true);
		MultiplayerCollider.SetActive(value: true);
		ShipBuilder.SetActive(value: true);
		SinglePlayerTestLevel.SetActive(value: false);
		Ship1.SetActive(value: false);
		Ship2.SetActive(value: false);
		Ship3.SetActive(value: false);
		Galaxy.SetActive(value: true);
		iTween.ScaleTo(Galaxy, new Vector3(1f, 1f, 1f), 8f);
	}

	public void shipbuilderbuilder()
	{
		Vector3 position = Galaxy.transform.Find("Galaxy").position;
		Galaxy.transform.position = ShipBuilder.transform.position;
		Galaxy.transform.Find("Galaxy").position = position;
		mainmenuDisable();
		Galaxy.SetActive(value: false);
		ShipParent.SetActive(value: true);
		Menu.SetActive(value: false);
		iTween.ScaleTo(Galaxy, new Vector3(6f, 6f, 6f), 8f);
		Debug.Log("settiings");
	}

	public void Multiplayersort()
	{
		findagame();
	}

	public void SettingsSort()
	{
		Ship1.SetActive(value: false);
		Ship2.SetActive(value: false);
		Ship3.SetActive(value: false);
		fleetBuilder.SetActive(value: false);
		ShipBuilderMain.SetActive(value: false);
		ShipBuilder.SetActive(value: false);
		ShipParent.SetActive(value: false);
		mainmenuDisable();
		settingsparent.SetActive(value: true);
		iTween.ScaleTo(Galaxy, new Vector3(6f, 6f, 6f), 8f);
		Debug.Log("settiings");
	}

	public void SinglePlayerSort()
	{
		Ship1.SetActive(value: false);
		Ship2.SetActive(value: false);
		Ship3.SetActive(value: false);
		fleetBuilder.SetActive(value: false);
		ShipBuilderMain.SetActive(value: false);
		ShipBuilder.SetActive(value: false);
		ShipParent.SetActive(value: false);
		GameObject.Find("MainMenuControlParent").GetComponent<MainMenuButtonControlScript>().SinglePlayer();
		iTween.ScaleTo(Galaxy, new Vector3(6f, 6f, 6f), 8f);
		Debug.Log("singleplayer");
	}

	public void findagame()
	{
		Vector3 position = Galaxy.transform.Find("Galaxy").position;
		Galaxy.transform.position = ShipBuilder.transform.position;
		Galaxy.transform.Find("Galaxy").position = position;
		mainmenuDisable();
		ShipBuilderMain.SetActive(value: false);
		fleetBuilder.SetActive(value: false);
		settingsparent.SetActive(value: false);
		findgamemenu.SetActive(value: true);
		iTween.ScaleTo(Galaxy, new Vector3(6f, 6f, 6f), 8f);
		Debug.Log("settiings");
	}

	public void findagameactual()
	{
		PhotonNetwork.Disconnect();
		GameObject.Find("MainMenuControlParent").GetComponent<MainMenuButtonControlScript>().Loading.SetActive(value: true);
		SceneManager.LoadSceneAsync(6);
	}

	private void Save_current_Ship(GameObject saveobject)
	{
		int num = 0;
		List<GameObject> list = new List<GameObject>();
		list.Clear();
		if (saveobject.transform.Find("Weapons").childCount > 0 && saveobject.transform.Find("Weapons").Find("Attachpoint 1").childCount > 0)
		{
			list.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 1").GetChild(0)
				.gameObject);
			}
			else
			{
				list.Add(null);
			}
			if (saveobject.transform.Find("Weapons").childCount > 1 && saveobject.transform.Find("Weapons").Find("Attachpoint 2").childCount > 0)
			{
				list.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 2").GetChild(0)
					.gameObject);
				}
				else
				{
					list.Add(null);
				}
				if (saveobject.transform.Find("Weapons").childCount > 2 && saveobject.transform.Find("Weapons").Find("Attachpoint 3").childCount > 0)
				{
					list.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 3").GetChild(0)
						.gameObject);
					}
					else
					{
						list.Add(null);
					}
					foreach (GameObject item in list)
					{
						if (item != null)
						{
							string text = saveobject.name + num;
							Debug.Log(text);
							FleetBuilder.add(text, list[num].name);
							Vector3 localPosition = list[num].transform.localPosition;
							Vector3 localEulerAngles = list[num].transform.localEulerAngles;
							FleetBuilder.add(saveobject.name + "parent" + num, item.transform.parent.name);
							FleetBuilder.write();
							FleetBuilder.add(saveobject.name + "num", num + 1);
							FleetBuilder.add(saveobject.name + "scalex" + num, item.transform.localScale.x);
							FleetBuilder.add(saveobject.name + "scaley" + num, item.transform.localScale.y);
							FleetBuilder.add(saveobject.name + "scalez" + num, item.transform.localScale.z);
							FleetBuilder.add(saveobject.name + "posx" + num, item.transform.parent.localPosition.x);
							FleetBuilder.add(saveobject.name + "posy" + num, item.transform.parent.localPosition.y);
							FleetBuilder.add(saveobject.name + "posz" + num, item.transform.parent.localPosition.z);
							Object.Destroy(item);
						}
						else
						{
							string key = saveobject.name + num;
							FleetBuilder.add(key, "nothing");
							Debug.Log("nothing");
						}
						FleetBuilder.write();
						num++;
					}
				}

				public void TestArea()
				{
					GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().testarea = true;
					PhotonNetwork.offlineMode = true;
					findagame();
				}

				public void Tutorial()
				{
					GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().tutorial = true;
					if (!PhotonNetwork.connected)
					{
						PhotonNetwork.offlineMode = true;
						findagame();
					}
					else
					{
						PhotonNetwork.Disconnect();
					}
				}

				private void OnDisconnectedFromPhoton()
				{
					if ((bool)GameObject.Find("MainMenuHolder") && GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().tutorial)
					{
						PhotonNetwork.offlineMode = true;
						findagame();
					}
				}
			}
