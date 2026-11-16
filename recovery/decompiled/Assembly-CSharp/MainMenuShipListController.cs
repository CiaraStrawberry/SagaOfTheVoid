using UnityEngine;

public class MainMenuShipListController : MonoBehaviour
{
	public GameObject fleet1;

	public GameObject fleet2;

	public GameObject fleet3;

	private TextMesh textmesh;

	private void Start()
	{
		textmesh = GetComponent<TextMesh>();
	}

	private void Update()
	{
		string text = string.Empty;
		if (fleet1.GetActive())
		{
			text = updatedisplay(fleet1);
		}
		if (fleet2.GetActive())
		{
			text = updatedisplay(fleet2);
		}
		if (fleet3.GetActive())
		{
			text = updatedisplay(fleet3);
		}
		textmesh.text = text;
	}

	private string updatedisplay(GameObject displaytoupdate)
	{
		string text = string.Empty;
		foreach (Transform item in displaytoupdate.transform)
		{
			if (item.childCount > 0)
			{
				text = item.GetChild(0).name + "\n" + text;
			}
		}
		return text;
	}
}
