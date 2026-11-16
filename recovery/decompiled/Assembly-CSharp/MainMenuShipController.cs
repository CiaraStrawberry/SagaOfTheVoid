using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using simple;

public class MainMenuShipController : MonoBehaviour
{
	public int countthrough;

	private GunController guncontroller;

	private IOComponent FleetBuilder;

	public List<GameObject> Ships;

	public List<ShipContainer> ShipList = new List<ShipContainer>();

	public GameObject TurretHolder;

	public GameObject display;

	private TextMesh displaycomponent;

	public GameObject resetparent;

	public List<GameObject> Children = new List<GameObject>();

	public void Awake()
	{
		FleetBuilder = IORoot.findIO("fleet1");
		if ((bool)FleetBuilder)
		{
			FleetBuilder.read();
		}
	}

	private void Update()
	{
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

	public Vector3 scaleout()
	{
		return new Vector3(ShipList[countthrough].size, ShipList[countthrough].size, ShipList[countthrough].size);
	}

	public void Start()
	{
		ShipList.Add(new ShipContainer(Ships[0], 0.7f, "Capital Offense", Ships[0].transform.position, specialin: false));
		ShipList.Add(new ShipContainer(Ships[1], 0.7f, "Capital Defense", Ships[1].transform.position, specialin: false));
		ShipList.Add(new ShipContainer(Ships[2], 0.7f, "Capital Support", Ships[2].transform.position, specialin: true));
		ShipList.Add(new ShipContainer(Ships[3], 0.7f, "Capital Tank", Ships[3].transform.position, specialin: false));
		ShipList.Add(new ShipContainer(Ships[4], 0.35f, "Cruiser Defense", Ships[4].transform.position, specialin: false));
		ShipList.Add(new ShipContainer(Ships[5], 0.35f, "Cruiser Offense", Ships[5].transform.position, specialin: false));
		ShipList.Add(new ShipContainer(Ships[6], 0.35f, "Cruiser Support", Ships[6].transform.position, specialin: true));
		ShipList.Add(new ShipContainer(Ships[7], 0.35f, "Cruiser Tank", Ships[7].transform.position, specialin: false));
		ShipList.Add(new ShipContainer(Ships[8], 0.4f, "Destroyer Defense", Ships[8].transform.position, specialin: false));
		ShipList.Add(new ShipContainer(Ships[9], 0.4f, "Destroyer Offense", Ships[9].transform.position, specialin: false));
		ShipList.Add(new ShipContainer(Ships[10], 0.4f, "Destroyer Support", Ships[10].transform.position, specialin: true));
		ShipList.Add(new ShipContainer(Ships[11], 0.4f, "Destroyer Tank", Ships[11].transform.position, specialin: false));
		ShipList.Add(new ShipContainer(Ships[12], 0.3f, "Frigate Defense", Ships[12].transform.position, specialin: false));
		ShipList.Add(new ShipContainer(Ships[13], 0.3f, "Frigate Offense", Ships[13].transform.position, specialin: false));
		ShipList.Add(new ShipContainer(Ships[14], 0.3f, "Frigate Support", Ships[14].transform.position, specialin: true));
		ShipList.Add(new ShipContainer(Ships[15], 0.3f, "Frigate Tank", Ships[15].transform.position, specialin: false));
		checkupdate();
		guncontroller = TurretHolder.GetComponent<GunController>();
		guncontroller.reset(sizeget());
		displaycomponent = display.GetComponent<TextMesh>();
		updatedisplay();
		StartCoroutine(Example());
		Deploy_Current_Ship(ShipList[countthrough].ship);
	}

	public void quit()
	{
		Save_current_Ship(ShipList[countthrough].ship);
		GameObject.Find("Controllers").GetComponent<GalaxyController>().returntonormal();
	}

	private void updatedisplay()
	{
		displaycomponent.text = ShipList[countthrough].namestring;
	}

	public void reset()
	{
		foreach (ShipContainer ship in ShipList)
		{
			ship.ship.transform.position = ship.startloc;
			ship.ship.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
		}
		Save_current_Ship(resetparent.transform.Find(ShipList[countthrough].ship.name).gameObject);
		Deploy_Current_Ship(ShipList[countthrough].ship);
	}

	public void disableall()
	{
		foreach (ShipContainer ship in ShipList)
		{
			ship.ship.SetActive(value: false);
		}
	}

	public void Addone()
	{
		Save_current_Ship(ShipList[countthrough].ship);
		countthrough++;
		if (countthrough == 16)
		{
			countthrough = 0;
		}
		checkupdate();
		guncontroller.reset(sizeget());
		Deploy_Current_Ship(ShipList[countthrough].ship);
		ShipList[countthrough].ship.transform.localPosition = new Vector3(-0.1339f, 0.0317f, -0.259f);
		updatedisplay();
	}

	public void gobackone()
	{
		Save_current_Ship(ShipList[countthrough].ship);
		countthrough--;
		if (countthrough == -1)
		{
			countthrough = 15;
		}
		checkupdate();
		guncontroller.reset(sizeget());
		Deploy_Current_Ship(ShipList[countthrough].ship);
		ShipList[countthrough].ship.transform.localPosition = new Vector3(-0.1339f, 0.0317f, -0.259f);
		updatedisplay();
	}

	private void checkupdate()
	{
		disableall();
		ShipList[countthrough].ship.SetActive(value: true);
	}

	private GameObject Gameobjectget()
	{
		return ShipList[countthrough].ship;
	}

	private float sizeget()
	{
		return ShipList[countthrough].size;
	}

	private void Save_current_Ship(GameObject saveobject)
	{
		int num = 0;
		Children.Clear();
		if (saveobject.transform.Find("Weapons").childCount > 0 && saveobject.transform.Find("Weapons").Find("Attachpoint 1").childCount > 0)
		{
			Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 1").GetChild(0)
				.gameObject);
			}
			else
			{
				Children.Add(null);
			}
			if (saveobject.transform.Find("Weapons").childCount > 1 && saveobject.transform.Find("Weapons").Find("Attachpoint 2").childCount > 0)
			{
				Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 2").GetChild(0)
					.gameObject);
				}
				else
				{
					Children.Add(null);
				}
				if (saveobject.transform.Find("Weapons").childCount > 2 && saveobject.transform.Find("Weapons").Find("Attachpoint 3").childCount > 0)
				{
					Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 3").GetChild(0)
						.gameObject);
					}
					else
					{
						Children.Add(null);
					}
					if (saveobject.transform.Find("Weapons").childCount > 3 && saveobject.transform.Find("Weapons").Find("Attachpoint 4").childCount > 0)
					{
						Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 4").GetChild(0)
							.gameObject);
						}
						else
						{
							Children.Add(null);
						}
						if (saveobject.transform.Find("Weapons").childCount > 4 && saveobject.transform.Find("Weapons").Find("Attachpoint 5").childCount > 0)
						{
							Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 5").GetChild(0)
								.gameObject);
							}
							else
							{
								Children.Add(null);
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
									FleetBuilder.add(saveobject.name + "scalex" + num, child.transform.localScale.x);
									FleetBuilder.add(saveobject.name + "scaley" + num, child.transform.localScale.y);
									FleetBuilder.add(saveobject.name + "scalez" + num, child.transform.localScale.z);
									FleetBuilder.add(saveobject.name + "posx" + num, child.transform.parent.localPosition.x);
									FleetBuilder.add(saveobject.name + "posy" + num, child.transform.parent.localPosition.y);
									FleetBuilder.add(saveobject.name + "posz" + num, child.transform.parent.localPosition.z);
								}
								else
								{
									string key2 = saveobject.name + num;
									FleetBuilder.add(key2, "nothing");
									Debug.Log("nothing");
								}
								FleetBuilder.write();
								num++;
							}
						}

						private void Deploy_Current_Ship(GameObject loadObj)
						{
							if (ShipList[countthrough].special)
							{
								guncontroller.supporton();
							}
							else
							{
								guncontroller.supportoff();
							}
							int num = FleetBuilder.get<int>(loadObj.name + "num");
							Debug.Log(num);
							for (int i = 0; i < num; i++)
							{
								string text = FleetBuilder.get<string>(loadObj.name + i);
								float x = FleetBuilder.get<float>(loadObj.name + "scalex" + i);
								float y = FleetBuilder.get<float>(loadObj.name + "scaley" + i);
								float z = FleetBuilder.get<float>(loadObj.name + "scalez" + i);
								if (!(text != "nothing"))
								{
									continue;
								}
								Debug.Log(text);
								GameObject gameObject = GameObject.Find("ShipParent").transform.Find("TurretHolder").Find(text).gameObject.GetComponent<WeaponBaseClass>().copy();
								if (loadObj.transform.Find("Weapons").Find("Attachpoint " + (i + 1)).transform.childCount < 1)
								{
									gameObject.transform.parent = loadObj.transform.Find("Weapons").Find("Attachpoint " + (i + 1));
									gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
									gameObject.transform.localScale = new Vector3(x, y, z);
									gameObject.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
									gameObject.GetComponent<WeaponBaseClass>().startget();
									continue;
								}
								foreach (Transform item in loadObj.transform.Find("Weapons").Find("Attachpoint " + (i + 1)).transform)
								{
									Object.Destroy(item.gameObject);
								}
								gameObject.transform.parent = loadObj.transform.Find("Weapons").Find("Attachpoint " + (i + 1));
								gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
								gameObject.transform.localScale = new Vector3(x, y, z);
								gameObject.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
								gameObject.GetComponent<WeaponBaseClass>().startget();
							}
							if (num == 0)
							{
								Save_current_Ship(resetparent.transform.Find(loadObj.name).gameObject);
								Deploy_Current_Ship(loadObj);
							}
							FleetBuilder.write();
							string message = FleetBuilder.get<string>(loadObj.name + "0");
							Debug.Log(message);
						}

						private void checkreset1(GameObject loadObj)
						{
							if (FleetBuilder.get<int>(loadObj.name + "num") == 0)
							{
								Save_current_Ship(resetparent.transform.Find(loadObj.name).gameObject);
							}
						}

						private IEnumerator Example()
						{
							yield return new WaitForSeconds(0.5f);
							Deploy_Current_Ship(ShipList[countthrough].ship);
						}

						public void checkresetall()
						{
							foreach (ShipContainer ship in ShipList)
							{
								checkreset1(ship.ship);
							}
						}
					}
