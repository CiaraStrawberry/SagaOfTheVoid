using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using simple;

public class FleetBuilderController : MonoBehaviour
{
	public int limit;

	public int countthrough;

	private FleetNumControl guncontroller;

	private IOComponent FleetBuilder;

	public List<GameObject> Ships;

	public GameObject TurretHolder;

	public GameObject display;

	private TextMesh displaycomponent;

	public List<GameObject> shipstodeploy = new List<GameObject>();

	public GameObject fleet1;

	public GameObject fleet2;

	public GameObject fleet3;

	public int currenttotal;

	public int currentmoney;

	public GameObject defaultparent;

	public List<GameObject> Children = new List<GameObject>();

	private void Start()
	{
		checkupdate();
		FleetBuilder = IORoot.findIO("fleet1");
		FleetBuilder.read();
		StartCoroutine(Example());
		guncontroller = GetComponent<FleetNumControl>();
		countthrough = 0;
		checkupdate();
	}

	private void Update()
	{
		if (fleet1.GetActive())
		{
			currenttotal = checkall(fleet1);
		}
		else if (fleet2.GetActive())
		{
			currenttotal = checkall(fleet2);
		}
		else if (fleet3.GetActive())
		{
			currenttotal = checkall(fleet3);
		}
		currentmoney = limit - currenttotal;
		if (Input.GetKeyDown(KeyCode.R))
		{
			reset();
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			Addone();
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			gobackone();
		}
	}

	private int checkall(GameObject parent)
	{
		int num = 0;
		foreach (Transform item in parent.transform)
		{
			if (item.childCount > 0)
			{
				num += item.GetChild(0).GetComponent<WeaponBaseClass>().cost;
			}
		}
		return num;
	}

	public bool enough(int input)
	{
		if (input <= currentmoney)
		{
			return true;
		}
		return false;
	}

	public void quit()
	{
		GameObject.Find("Controllers").GetComponent<GalaxyController>().returntonormal();
	}

	public void reset()
	{
		Save_current_Ship(defaultparent.transform.Find(Ships[countthrough].name).gameObject);
		Deploy_Current_Fleet(Ships[countthrough]);
	}

	public void disableall()
	{
		foreach (GameObject ship in Ships)
		{
			ship.SetActive(value: false);
		}
	}

	public void enableall()
	{
		foreach (GameObject ship in Ships)
		{
			ship.SetActive(value: true);
		}
	}

	public void Addone()
	{
		Save_current_Ship(Ships[countthrough]);
		countthrough++;
		if (countthrough > 2)
		{
			countthrough = 0;
		}
		checkupdate();
		Debug.Log(countthrough);
		Deploy_Current_Fleet(Ships[countthrough]);
	}

	public void gobackone()
	{
		Save_current_Ship(Ships[countthrough]);
		countthrough--;
		if (countthrough < 0)
		{
			countthrough = 2;
		}
		checkupdate();
		Deploy_Current_Fleet(Ships[countthrough]);
	}

	private void checkupdate()
	{
		disableall();
		Ships[countthrough].SetActive(value: true);
	}

	private GameObject Gameobjectget()
	{
		return Ships[countthrough];
	}

	private float sizeget()
	{
		return 1f;
	}

	private void Save_current_Ship(GameObject saveobject)
	{
		enableall();
		int num = 0;
		Children.Clear();
		Debug.Log(saveobject.transform.childCount);
		foreach (Transform item in saveobject.transform)
		{
			if (item.childCount > 0)
			{
				Children.Add(item.GetChild(0).gameObject);
			}
			else
			{
				Children.Add(null);
			}
		}
		foreach (GameObject child in Children)
		{
			if (child != null)
			{
				string key = saveobject.name + num;
				FleetBuilder.add(key, Children[num].name);
				Vector3 localPosition = Children[num].transform.localPosition;
				Vector3 localEulerAngles = Children[num].transform.localEulerAngles;
				FleetBuilder.add(saveobject.name + "parent" + num, child.transform.parent.name);
				FleetBuilder.write();
				FleetBuilder.add(saveobject.name + "num", num + 1);
				FleetBuilder.add(saveobject.name + "scalex" + num, child.transform.parent.localScale.x);
				FleetBuilder.add(saveobject.name + "scaley" + num, child.transform.parent.localScale.y);
				FleetBuilder.add(saveobject.name + "scalez" + num, child.transform.parent.localScale.z);
			}
			else
			{
				string key2 = saveobject.name + num;
				FleetBuilder.add(key2, "nothing");
			}
			num++;
		}
		FleetBuilder.write();
		checkupdate();
	}

	private void Deploy_Current_Fleet(GameObject loadObj)
	{
		int num = FleetBuilder.get<int>(loadObj.name + "num");
		Debug.Log(num);
		Debug.Log(loadObj.name + "num");
		for (int i = 0; i < num; i++)
		{
			string text = FleetBuilder.get<string>(loadObj.name + i);
			float num2 = FleetBuilder.get<float>(loadObj.name + "scalex" + i);
			float num3 = FleetBuilder.get<float>(loadObj.name + "scaley" + i);
			float num4 = FleetBuilder.get<float>(loadObj.name + "scalez" + i);
			Debug.Log("Attachpoint " + (i + 1) + " " + text);
			if (!(text != "nothing"))
			{
				continue;
			}
			GameObject gameObject = null;
			bool active = false;
			for (int j = 0; j < shipstodeploy.Count; j++)
			{
				if (shipstodeploy[j].name == text)
				{
					gameObject = shipstodeploy[j].GetComponent<WeaponBaseClass>().copy();
					active = shipstodeploy[j].transform.parent.gameObject.GetActive();
				}
			}
			if ((bool)gameObject && loadObj.transform.Find("Attachpoint " + (i + 1)).transform.childCount < 1)
			{
				gameObject.SetActive(value: true);
				gameObject.transform.parent = loadObj.transform.Find("Attachpoint " + (i + 1));
				gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject.transform.localScale = new Vector3(100f, 100f, 100f);
				gameObject.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
				gameObject.GetComponent<WeaponBaseClass>().startget();
				gameObject.transform.parent.gameObject.SetActive(active);
				Debug.Log(gameObject);
			}
		}
		FleetBuilder.write();
		string message = FleetBuilder.get<string>(loadObj.name + "0");
		Debug.Log(message);
	}

	private IEnumerator Example()
	{
		yield return new WaitForSeconds(0.5f);
		Deploy_Current_Fleet(Ships[countthrough]);
	}
}
