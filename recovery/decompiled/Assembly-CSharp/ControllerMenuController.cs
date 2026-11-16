using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerMenuController : MonoBehaviour
{
	public TextMesh Kills_Text;

	public TextMesh Ships_Left_Text;

	public TextMesh Time_Left_Text;

	public GameObject Map;

	public UnitMovementcommandcontroller unitcontrol;

	public List<bliplinecontroller> allyblips = new List<bliplinecontroller>();

	public List<bliplinecontroller> enemyblips = new List<bliplinecontroller>();

	public GameObject playerblip;

	public GameObject allyblip;

	public GameObject enemyblip;

	public GameObject parentforblips;

	public AIController AIController;

	public bool ismission;

	public GameObject Playericon;

	public GameObject Cameratracking;

	private GameObject Objectsholder;

	public TextMesh Blue;

	public TextMesh Green;

	public TextMesh Grey;

	public TextMesh Red;

	public TextMesh White;

	public TextMesh Yellow;

	private int waitforit;

	private CrossLevelVariableHolder crossvar;

	private void Start()
	{
		if ((bool)Map)
		{
			for (int i = 0; i < 40; i++)
			{
				spawnblip(ref allyblips, allyblip);
				spawnblip(ref enemyblips, enemyblip);
			}
			playerblip = Object.Instantiate(Playericon);
			playerblip.transform.parent = Map.transform;
			playerblip.transform.localScale = new Vector3(600f, 600f, 600f);
			playerblip.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
			foreach (bliplinecontroller allyblip in allyblips)
			{
				allyblip.transform.parent = Map.transform;
				allyblip.transform.localScale = new Vector3(400f, 400f, 400f);
				allyblip.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
			}
			foreach (bliplinecontroller enemyblip in enemyblips)
			{
				enemyblip.transform.parent = Map.transform;
				enemyblip.transform.localScale = new Vector3(400f, 400f, 400f);
				enemyblip.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
			}
		}
		if (GameObject.Find("Camera (eye)") != null)
		{
			Cameratracking = GameObject.Find("Camera (eye)");
		}
		else
		{
			Cameratracking = GameObject.Find("CenterEyeAnchor");
		}
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		OnLevelFinishedLoading(default, LoadSceneMode.Single);
		InvokeRepeating("Rep", 0f, 1f);
	}

	private void Rep()
	{
		if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
		{
			return;
		}
		if ((bool)Blue)
		{
			if (Green.gameObject.GetActive())
			{
				Green.text = getplayername(LobbyScreenController.getteam1(), 0) + "Green: " + unitcontrol.teammembersout(_Ship.eShipColor.Green).Count + " ships";
			}
			if (Blue.gameObject.GetActive())
			{
				Blue.text = getplayername(LobbyScreenController.getteam1(), 1) + "Blue: " + unitcontrol.teammembersout(_Ship.eShipColor.Blue).Count + " ships";
			}
			if (White.gameObject.GetActive())
			{
				White.text = getplayername(LobbyScreenController.getteam1(), 2) + "White: " + unitcontrol.teammembersout(_Ship.eShipColor.White).Count + " ships";
			}
			if (Yellow.gameObject.GetActive())
			{
				Yellow.text = getplayername(LobbyScreenController.getteam2(), 0) + "Yellow: " + unitcontrol.teammembersout(_Ship.eShipColor.Yellow).Count + " ships";
			}
			if (Grey.gameObject.GetActive())
			{
				Grey.text = getplayername(LobbyScreenController.getteam2(), 1) + "Grey: " + unitcontrol.teammembersout(_Ship.eShipColor.Grey).Count + " ships";
			}
			if (Red.gameObject.GetActive())
			{
				Red.text = getplayername(LobbyScreenController.getteam2(), 2) + "Red: " + unitcontrol.teammembersout(_Ship.eShipColor.Red).Count + " ships";
			}
		}
		if (unitcontrol == null)
		{
			unitcontrol = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
		}
		foreach (bliplinecontroller allyblip in allyblips)
		{
			allyblip.transform.localPosition = new Vector3(100000f, 100000f, 100000f);
		}
		foreach (bliplinecontroller enemyblip in enemyblips)
		{
			enemyblip.transform.localPosition = new Vector3(100000f, 100000f, 100000f);
		}
		foreach (bliplinecontroller allyblip2 in allyblips)
		{
			allyblip2.targetgam = new Vector3(0f, 0f, 0f);
		}
		List<GameObject> list = unitcontrol.teammembersout(unitcontrol.team);
		if (list != null && Ships_Left_Text != null)
		{
			Kills_Text.text = " ships : " + list.Count + " out of " + UnitMovementcommandcontroller.getmaxshipnumbers();
			List<TSTransform> list2 = unitcontrol.targetsout(unitcontrol.team);
			for (int i = 0; i < list2.Count; i++)
			{
				if (enemyblips[i] != null && enemyblips[i].gameObject != null && list2[i] != null && list2[i].gameObject != null)
				{
					enemyblips[i].transform.localPosition = new Vector3(0f, list2[i].transform.localPosition.x * 0.35f, list2[i].transform.localPosition.z * 0.35f);
				}
			}
		}
		if ((bool)Time_Left_Text && !ismission)
		{
			Time_Left_Text.text = unitcontrol.gettimeleft();
		}
		else if ((bool)AIController && AIController.timepassedactual < 30)
		{
			Time_Left_Text.text = "Time til attack: " + (30 - TSMath.Round(AIController.timepassedactual)).ToString();
		}
		else if (unitcontrol.crosslevelholder.campaign && unitcontrol.crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive)
		{
			Time_Left_Text.text = "Survive for: " + unitcontrol.gettimeleft();
		}
		else if ((bool)Time_Left_Text)
		{
			Time_Left_Text.text = unitcontrol.gettimeleft();
		}
		List<TSTransform> list3 = unitcontrol.alliesout();
		for (int j = 0; j < list3.Count; j++)
		{
			if ((bool)this.allyblip && allyblips[j].gameObject != null)
			{
				allyblips[j].source = list3[j];
				allyblips[j].transform.localPosition = new Vector3(0f, list3[j].transform.localPosition.x * 0.35f, list3[j].transform.localPosition.z * 0.35f);
				Vector3 vector = list3[j].shipscript.fac.localtarget.ToVector() * 0.35f;
				allyblips[j].targetgam = new Vector3(0f, vector.x, vector.z);
			}
		}
		if (Objectsholder == null)
		{
			Objectsholder = GameObject.Find("Objects");
		}
		if (Cameratracking == null)
		{
			if (GameObject.Find("Camera (eye)") != null)
			{
				Cameratracking = GameObject.Find("Camera (eye)");
			}
			else
			{
				Cameratracking = GameObject.Find("CenterEyeAnchor");
			}
		}
		Vector3 vector2 = Objectsholder.transform.InverseTransformPoint(Cameratracking.transform.position);
		playerblip.transform.localPosition = new Vector3(0f, vector2.x * 0.35f, vector2.z * 0.35f);
	}

	private string getplayername(List<int> inputarray, int inputpos)
	{
		string result = string.Empty;
		if (inputarray[inputpos] != 300)
		{
			result = PhotonPlayer.Find(inputarray[inputpos]).NickName + " - ";
		}
		return result;
	}

	private void spawnblip(ref List<bliplinecontroller> blips, GameObject thingtospawn)
	{
		GameObject gameObject = Object.Instantiate(thingtospawn);
		blips.Add(gameObject.GetComponent<bliplinecontroller>());
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
	{
		if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
		{
			return;
		}
		Objectsholder = GameObject.Find("Objects");
		unitcontrol = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
		waitforit = 0;
		if (PhotonNetwork.room == null)
		{
			return;
		}
		Debug.Log(PhotonNetwork.room.MaxPlayers);
		if ((bool)Blue)
		{
			Green.gameObject.SetActive(value: true);
			Blue.gameObject.SetActive(value: true);
			White.gameObject.SetActive(value: true);
			Yellow.gameObject.SetActive(value: true);
			Grey.gameObject.SetActive(value: true);
			Red.gameObject.SetActive(value: true);
			if (PhotonNetwork.room.MaxPlayers == 2)
			{
				Blue.gameObject.SetActive(value: false);
				White.gameObject.SetActive(value: false);
				Grey.gameObject.SetActive(value: false);
				Red.gameObject.SetActive(value: false);
			}
			if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "No")
			{
				White.gameObject.SetActive(value: false);
				Red.gameObject.SetActive(value: false);
			}
		}
	}

	private void Update()
	{
		waitforit++;
		if (waitforit == 10)
		{
			if ((bool)GameObject.Find("AIController"))
			{
				AIController = GameObject.Find("AIController").GetComponent<AIController>();
			}
			if ((bool)AIController)
			{
				ismission = AIController.ismission;
			}
		}
	}
}
