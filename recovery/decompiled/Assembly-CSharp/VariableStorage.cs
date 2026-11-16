using System.Collections;
using UnityEngine;
using simple;

public class VariableStorage : MonoBehaviour
{
	public GameObject enemyfleetEasiest;

	public GameObject enemyfleetEasy;

	public GameObject enemyfleetMedium;

	public GameObject enemyfleetHard;

	public GameObject enemyfleetExtreme;

	public GameObject CameraRigbig;

	public GameObject menu;

	private Vector3 tempos;

	private GameObject Ioroot;

	public string difficulty;

	public GameObject Easiestobj;

	private string fleet;

	public bool skirmish;

	private IOComponent FleetBuilder;

	private void Awake()
	{
		Ioroot = GameObject.Find("IORoot");
		FleetBuilder = IORoot.findIO("fleet1");
		GameObject[] array = GameObject.FindGameObjectsWithTag("Man");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject != base.gameObject)
			{
				Object.Destroy(gameObject.gameObject);
			}
		}
	}

	private void Start()
	{
		Medium();
		Fleet1();
		Fleet1();
		SetQuality component = Ioroot.GetComponent<SetQuality>();
		component.fleettest();
	}

	public void Easiest()
	{
		difficulty = "Easiest";
	}

	public void Easy()
	{
		difficulty = "Easy";
	}

	public void Medium()
	{
		difficulty = "Medium";
	}

	public void Hard()
	{
		difficulty = "Hard";
	}

	public void Extreme()
	{
		difficulty = "Extreme";
	}

	public void Fleet1()
	{
	}

	public void Fleet2()
	{
	}

	public void Fleet3()
	{
		SetQuality component = Ioroot.GetComponent<SetQuality>();
		component.fleet3fun();
	}

	public void startskirmish()
	{
		skirmish = true;
		GameObject basesav = GameObject.Find("HologramBase");
		save(basesav);
	}

	public void start()
	{
		skirmish = false;
	}

	public void save(GameObject basesav)
	{
		GameObject gameObject = GameObject.FindWithTag("Hol");
		SetQuality component = Ioroot.GetComponent<SetQuality>();
		component.save(fleet, basesav);
	}

	public void reset()
	{
		GameObject saveobject = GameObject.Find("ResetBase");
		SetQuality component = Ioroot.GetComponent<SetQuality>();
		component.save(fleet, saveobject);
	}

	public void spawn_allied_fleet()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("Hol");
		foreach (Transform item in gameObject.transform)
		{
			Object.Destroy(item.gameObject);
		}
		int num = FleetBuilder.get<int>(fleet + "num");
		FleetBuilder.read();
		Vector3 vector = new Vector3(0f, 0f, 0f);
		for (int i = 0; i < num; i++)
		{
			string text = FleetBuilder.get<string>(fleet + i);
			float x = FleetBuilder.get<float>(fleet + "locx" + i);
			float y = FleetBuilder.get<float>(fleet + "locy" + i);
			float z = FleetBuilder.get<float>(fleet + "locz" + i);
			float x2 = FleetBuilder.get<float>(fleet + "rotx" + i);
			float y2 = FleetBuilder.get<float>(fleet + "roty" + i);
			float z2 = FleetBuilder.get<float>(fleet + "rotz" + i);
			FleetBuilder.read();
			FleetBuilder.add(fleet + i, text);
			FleetBuilder.write();
			GameObject gameObject2 = Resources.Load(text) as GameObject;
			GameObject gameObject3 = Object.Instantiate(gameObject2, gameObject2.transform.position, Quaternion.Euler(0f, 180f, 0f));
			gameObject3.transform.parent = gameObject.transform;
			gameObject3.transform.localPosition = new Vector3(x, y, z);
			gameObject3.transform.localRotation = Quaternion.Euler(x2, y2, z2);
			Object.Destroy(gameObject3);
		}
	}

	public void startcor()
	{
		StartCoroutine("go");
	}

	public IEnumerator go()
	{
		yield return new WaitForSeconds(5f);
		Debug.Log(skirmish);
		if (skirmish)
		{
			Object.Destroy(GameObject.Find("BattleCapital"));
			GameObject gameObject = Object.Instantiate(CameraRigbig, tempos + new Vector3(0f, -300f, -1000f), Quaternion.Euler(0f, 0f, 0f));
			gameObject.transform.localScale = new Vector3(200f, 200f, 200f);
			GameObject gameObject2 = gameObject.transform.GetChild(0).gameObject;
			gameObject2.transform.eulerAngles = new Vector3(0f, 0f, 0f);
		}
	}

	private void spawn_enemy_fleet()
	{
		if (difficulty == "Hard")
		{
			spawn(enemyfleetEasiest);
		}
		if (difficulty == "Easy")
		{
			spawn(enemyfleetEasy);
		}
		if (difficulty == "Medium")
		{
			spawn(enemyfleetMedium);
		}
		if (difficulty == "Hard")
		{
			spawn(enemyfleetHard);
		}
		if (difficulty == "Extreme")
		{
			spawn(enemyfleetExtreme);
		}
	}

	private void spawn(GameObject spawner)
	{
		GameObject gameObject = Object.Instantiate(spawner);
		GameObject gameObject2 = GameObject.Find("Origin");
		gameObject.transform.position = gameObject2.transform.position;
	}

	public void spawnstart()
	{
		if (!skirmish)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Team1");
		GameObject[] array2 = array;
		foreach (GameObject obj in array2)
		{
			Object.Destroy(obj);
		}
		GameObject[] array3 = GameObject.FindGameObjectsWithTag("Team2");
		GameObject[] array4 = array3;
		foreach (GameObject gameObject in array4)
		{
			if (gameObject.name != "BattleCapital")
			{
				Object.Destroy(gameObject);
			}
		}
		spawn_allied_fleet();
		spawn_enemy_fleet();
	}
}
