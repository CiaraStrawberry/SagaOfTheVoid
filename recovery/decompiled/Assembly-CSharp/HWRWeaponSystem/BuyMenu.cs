using UnityEngine;

namespace HWRWeaponSystem;

public class BuyMenu : MonoBehaviour
{
	public GameObject[] Towers;

	public int[] TowersPrice;

	public int Money;

	public int Score;

	private int indexSelected = -1;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.LoadLevel("Menu");
		}
		if (indexSelected == -1)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0) && GUIUtility.hotControl == 0)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hitInfo, 100f))
			{
				PlaceTower(hitInfo.point);
			}
		}
		if (Input.GetMouseButtonDown(1))
		{
			indexSelected = -1;
		}
	}

	private void OnGUI()
	{
		if (indexSelected != -1)
		{
			GUI.Label(new Rect(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y, 200f, 50f), Towers[indexSelected].name);
		}
		for (int i = 0; i < Towers.Length; i++)
		{
			if (GUI.Button(new Rect(20f, 35 * i + 20, 150f, 30f), Towers[i].name + " -" + TowersPrice[i] + " $") && Money >= TowersPrice[i])
			{
				indexSelected = i;
			}
		}
		GUI.skin.label.alignment = TextAnchor.UpperRight;
		GUI.skin.label.fontSize = 25;
		GUI.Label(new Rect(Screen.width - 300, 20f, 250f, 50f), Money + " $");
		GUI.Label(new Rect(Screen.width - 300, 50f, 250f, 50f), Score + " Kills");
	}

	public void PlaceTower(Vector3 position)
	{
		if (indexSelected != -1 && Money >= TowersPrice[indexSelected])
		{
			Money -= TowersPrice[indexSelected];
			Object.Instantiate(Towers[indexSelected].gameObject, position, Towers[indexSelected].gameObject.transform.rotation);
			indexSelected = -1;
		}
	}
}
