using UnityEngine;

public class MainMenuSinglePlayerScript : MonoBehaviour
{
	private GameObject galaxy;

	private bool isup;

	private Vector3 startpos;

	private Vector3 startposme;

	private GameObject parent;

	private void Start()
	{
		galaxy = GameObject.Find("Controllers");
		startpos = transform.parent.localPosition;
		startposme = transform.position;
		parent = transform.parent.gameObject;
	}

	private void Update()
	{
		if ((bool)parent)
		{
			if (isup = false)
			{
			}
			if (!isup)
			{
				parent.transform.localPosition = startpos;
			}
			else
			{
				parent.transform.localPosition = startpos + new Vector3(0f, 0.01f, 0f);
			}
			isup = false;
		}
	}

	private void Load()
	{
		galaxy.GetComponent<GalaxyController>().SinglePlayerSort();
		Debug.Log("scaleup");
	}

	private void up()
	{
		isup = true;
		Debug.Log(isup);
	}
}
