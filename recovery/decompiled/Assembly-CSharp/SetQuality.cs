using System.Collections.Generic;
using UnityEngine;
using simple;

public class SetQuality : MonoBehaviour
{
	private GameObject basehol;

	private IOComponent FleetBuilder;

	private GameObject Serializationmanager;

	public List<GameObject> Children;

	public string ma;

	private void Awake()
	{
		FleetBuilder = IORoot.findIO("fleet1");
	}

	private void Update()
	{
	}

	private void Start()
	{
		if (name == "high")
		{
			QualitySettings.SetQualityLevel(5);
		}
		if (name == "medium")
		{
			QualitySettings.SetQualityLevel(3);
		}
		if (name == "low")
		{
			QualitySettings.SetQualityLevel(1);
		}
		if (!(FleetBuilder == null))
		{
		}
	}

	public void fleettest()
	{
		int num = FleetBuilder.get<int>("fleet1num");
		FleetBuilder.read();
		Debug.Log(num);
		if (num == 0)
		{
			GameObject gameObject = GameObject.FindWithTag("Man");
			VariableStorage component = gameObject.GetComponent<VariableStorage>();
			component.reset();
		}
	}

	public void fleet3fun()
	{
		basehol = GameObject.FindGameObjectWithTag("Hol");
		foreach (Transform item in basehol.transform)
		{
			Object.Destroy(item.gameObject);
		}
		int num = FleetBuilder.get<int>("fleet2num");
		FleetBuilder.read();
		Debug.Log(num);
		for (int i = 0; i < num; i++)
		{
			string text = FleetBuilder.get<string>("fleet3" + i);
			float x = FleetBuilder.get<float>("fleet3locx" + i);
			float y = FleetBuilder.get<float>("fleet3locy" + i);
			float z = FleetBuilder.get<float>("fleet3locz" + i);
			float x2 = FleetBuilder.get<float>("fleet3rotx" + i);
			float y2 = FleetBuilder.get<float>("fleet3roty" + i);
			float z2 = FleetBuilder.get<float>("fleet3rotz" + i);
			FleetBuilder.read();
			FleetBuilder.add("fleet3" + i, text);
			FleetBuilder.write();
			GameObject gameObject = Resources.Load(text) as GameObject;
			GameObject gameObject2 = Object.Instantiate(gameObject, gameObject.transform.position, Quaternion.Euler(x2, y2, z2));
			gameObject2.transform.parent = basehol.transform;
			gameObject2.transform.localPosition = new Vector3(x, y, z);
			gameObject2.transform.localRotation = Quaternion.Euler(x2, y2, z2);
			gameObject2.name = text;
		}
	}

	public void save(string fleet, GameObject saveobject)
	{
		Children.Clear();
		foreach (Transform item in saveobject.transform)
		{
			if (item.gameObject != saveobject)
			{
				Children.Add(item.gameObject);
			}
		}
		switch (fleet)
		{
		case "fleet1":
		{
			int num2 = 0;
			foreach (GameObject child in Children)
			{
				string key2 = "fleet1" + num2;
				FleetBuilder.add(key2, Children[num2].name);
				Vector3 localPosition2 = Children[num2].transform.localPosition;
				Vector3 localEulerAngles2 = Children[num2].transform.localEulerAngles;
				FleetBuilder.add("fleet1locx" + num2, localPosition2.x);
				FleetBuilder.add("fleet1locy" + num2, localPosition2.y);
				FleetBuilder.add("fleet1locz" + num2, localPosition2.z);
				FleetBuilder.add("fleet1rotx" + num2, localEulerAngles2.x);
				FleetBuilder.add("fleet1roty" + num2, localEulerAngles2.y);
				FleetBuilder.add("fleet1rotz" + num2, localEulerAngles2.z);
				FleetBuilder.write();
				num2++;
			}
			FleetBuilder.add("fleet1num", num2);
			FleetBuilder.write();
			FleetBuilder.read();
			break;
		}
		case "fleet2":
		{
			int num3 = 0;
			foreach (GameObject child2 in Children)
			{
				string key3 = "fleet2" + num3;
				FleetBuilder.add(key3, Children[num3].name);
				Vector3 localPosition3 = Children[num3].transform.localPosition;
				Vector3 localEulerAngles3 = Children[num3].transform.localEulerAngles;
				FleetBuilder.add("fleet2locx" + num3, localPosition3.x);
				FleetBuilder.add("fleet2locy" + num3, localPosition3.y);
				FleetBuilder.add("fleet2locz" + num3, localPosition3.z);
				FleetBuilder.add("fleet2rotx" + num3, localEulerAngles3.x);
				FleetBuilder.add("fleet2roty" + num3, localEulerAngles3.y);
				FleetBuilder.add("fleet2rotz" + num3, localEulerAngles3.z);
				FleetBuilder.write();
				num3++;
			}
			FleetBuilder.add("fleet2num", num3);
			FleetBuilder.write();
			FleetBuilder.read();
			break;
		}
		case "fleet3":
		{
			int num = 0;
			foreach (GameObject child3 in Children)
			{
				string key = "fleet3" + num;
				FleetBuilder.add(key, Children[num].name);
				Vector3 localPosition = Children[num].transform.localPosition;
				Vector3 localEulerAngles = Children[num].transform.localEulerAngles;
				FleetBuilder.add("fleet3locx" + num, localPosition.x);
				FleetBuilder.add("fleet3locy" + num, localPosition.y);
				FleetBuilder.add("fleet3locz" + num, localPosition.z);
				FleetBuilder.add("fleet3rotx" + num, localEulerAngles.x);
				FleetBuilder.add("fleet3roty" + num, localEulerAngles.y);
				FleetBuilder.add("fleet3rotz" + num, localEulerAngles.z);
				FleetBuilder.write();
				num++;
			}
			FleetBuilder.add("fleet3num", num);
			FleetBuilder.write();
			FleetBuilder.read();
			fleet3fun();
			break;
		}
		default:
			Debug.Log("null fleet");
			break;
		}
	}
}
