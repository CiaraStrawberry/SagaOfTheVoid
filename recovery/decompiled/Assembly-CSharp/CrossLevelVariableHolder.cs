using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CrossLevelVariableHolder : MonoBehaviour
{
	public enum BotDifficultyhol
	{
		easy,
		medium,
		hard
	}

	public enum gamemode
	{
		TeamDeathMatch,
		FreeForAll,
		CapitalShip
	}

	public enum mapcon
	{
		map1,
		map2,
		map3
	}

	public enum skyboxcon
	{
		skybox1,
		skybox2,
		skybox3,
		skybox4,
		skybox5
	}

	public int fleet = 1;

	public gamemode Gamemode;

	public AudioClip musicmain;

	public AudioClip music1;

	public AudioClip music2;

	public AudioClip music3;

	public VRTK_SDKSetup SDKLOADEDCURRENTLY;

	public mapcon map;

	public skyboxcon skybox;

	public bool tutorial;

	public bool TestArea;

	public bool loading;

	public BotDifficultyhol botdifficulty;

	public Material[] skyboxes = new Material[3];

	public bool campaign;

	public bool campaignnextlevelset;

	public MainMenuCampaignControlScript.mapcontainer campaignlevel;

	public bool bots;

	public Material getskybox(skyboxcon input)
	{
		Material result = skyboxes[0];
		switch (input)
		{
		case skyboxcon.skybox1:
			result = skyboxes[0];
			break;
		case skyboxcon.skybox2:
			result = skyboxes[1];
			break;
		case skyboxcon.skybox3:
			result = skyboxes[2];
			break;
		case skyboxcon.skybox4:
			result = skyboxes[3];
			break;
		case skyboxcon.skybox5:
			result = skyboxes[4];
			break;
		}
		return result;
	}

	private void Awake()
	{
		if (Object.FindObjectsOfType(GetType()).Length > 1)
		{
			Object.Destroy(gameObject);
		}
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(gameObject);
		bots = true;
		Gamemode = gamemode.TeamDeathMatch;
	}

	public AudioClip getmusic()
	{
		AudioClip result = music1;
		float value = Random.value;
		if (value < 0.33f)
		{
			result = music1;
		}
		if (value > 0.33f && value < 0.66f)
		{
			result = music2;
		}
		if (value > 0.66f)
		{
			result = music3;
		}
		return result;
	}

	public void AsignTeam(int ownerID, int team)
	{
		List<int> list = LobbyScreenController.getteam1();
		List<int> list2 = LobbyScreenController.getteam2();
		int num = PhotonNetwork.room.MaxPlayers;
		if (num == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
		{
			num = 6;
		}
		int num2 = num / 2;
		if (team == 1 && list.Count <= num2)
		{
			if (list2.Contains(ownerID))
			{
				list2.Remove(ownerID);
			}
			if (!list.Contains(ownerID))
			{
				list.Add(ownerID);
			}
		}
		else if (list2.Count <= num2)
		{
			if (list.Contains(ownerID))
			{
				list.Remove(ownerID);
			}
			if (!list2.Contains(ownerID))
			{
				list2.Add(ownerID);
			}
		}
		LobbyScreenController.setteam1(list);
		LobbyScreenController.setteam2(list2);
	}

	public void addtoteam(int thingtoadd, string teamkey)
	{
		List<int> list = LobbyScreenController.getteaminput(teamkey);
		int num = PhotonNetwork.room.MaxPlayers;
		if (num == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
		{
			num = 6;
		}
		int num2 = num / 2;
		if (list.Count <= num2)
		{
			if (!list.Contains(thingtoadd))
			{
				list.Add(thingtoadd);
			}
		}
		else
		{
			Debug.Log("ERROR: TEAM FULL");
		}
		LobbyScreenController.setteaminput(list, teamkey);
	}

	public void removetoteam(int thingtoadd, string teamkey)
	{
		List<int> list = LobbyScreenController.getteaminput(teamkey);
		if (list.Contains(thingtoadd))
		{
			list.Remove(thingtoadd);
		}
		LobbyScreenController.setteaminput(list, teamkey);
		Debug.Log("removed: " + thingtoadd + " from " + teamkey);
	}

	public int findteam(int check)
	{
		if (PhotonNetwork.inRoom)
		{
			int result = 2;
			if (LobbyScreenController.getteam1().Contains(check))
			{
				result = 1;
			}
			else if (LobbyScreenController.getteam2().Contains(check))
			{
				result = 2;
			}
			return result;
		}
		return 1;
	}

	public List<int> team1wbots()
	{
		List<int> list = LobbyScreenController.getteam1();
		int num = PhotonNetwork.room.MaxPlayers;
		if (num == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
		{
			num = 6;
		}
		while (list.Count < num / 2)
		{
			if (bots)
			{
				list.Add(300);
			}
		}
		LobbyScreenController.setteam1(list);
		return list;
	}

	public List<int> team2wbots()
	{
		List<int> list = LobbyScreenController.getteam2();
		int num = PhotonNetwork.room.MaxPlayers;
		if (num == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
		{
			num = 6;
		}
		while (list.Count < num / 2)
		{
			if (bots)
			{
				list.Add(300);
			}
		}
		LobbyScreenController.setteam2(list);
		return list;
	}

	public int findspawnpos(int input)
	{
		int result = 0;
		Debug.Log(LobbyScreenController.getteam1());
		if (LobbyScreenController.getteam1().Contains(input))
		{
			result = LobbyScreenController.getteam1().IndexOf(input) + 1;
		}
		if (LobbyScreenController.getteam2().Contains(input))
		{
			result = 4 + LobbyScreenController.getteam2().IndexOf(input);
		}
		return result;
	}

	public _Ship.eShipColor findspawnteam(int input)
	{
		int num = findspawnpos(input);
		_Ship.eShipColor result = _Ship.eShipColor.Green;
		if (num == 1)
		{
			result = _Ship.eShipColor.Green;
		}
		if (num == 2)
		{
			result = _Ship.eShipColor.Blue;
		}
		if (num == 3)
		{
			result = _Ship.eShipColor.White;
		}
		if (num == 4)
		{
			result = _Ship.eShipColor.Yellow;
		}
		if (num == 5)
		{
			result = _Ship.eShipColor.Grey;
		}
		if (num == 6)
		{
			result = _Ship.eShipColor.Red;
		}
		return result;
	}

	public void wipebots()
	{
		LobbyScreenController.setteam1(new List<int>());
		LobbyScreenController.setteam2(new List<int>());
	}
}
