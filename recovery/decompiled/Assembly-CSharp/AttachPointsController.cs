using UnityEngine;

public class AttachPointsController : MonoBehaviour
{
	public GameObject[] attachpoints;

	public int countthrough;

	private Vector3 startpos;

	private void Start()
	{
		disableall();
		InvokeRepeating("rep", 0f, 0.3f);
		disableall();
		if ((bool)attachpoints[0])
		{
			attachpoints[0].SetActive(value: true);
		}
		startpos = transform.localPosition;
		rep();
	}

	private void disableall()
	{
		GameObject[] array = attachpoints;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(value: false);
			}
		}
	}

	private void Update()
	{
		transform.localPosition = startpos;
	}

	private void rep()
	{
		if (attachpoints[countthrough] != null && attachpoints[countthrough].GetActive() && attachpoints[countthrough].transform.childCount > 0)
		{
			countthrough++;
		}
		if (countthrough < 1)
		{
			countthrough = 1;
		}
		if (attachpoints[countthrough] != null && attachpoints[countthrough - 1].transform.childCount < 1)
		{
			countthrough--;
		}
		disableall();
		for (int i = 0; i < countthrough + 1; i++)
		{
			if (attachpoints[i] != null)
			{
				attachpoints[i].SetActive(value: true);
			}
		}
		int num = 0;
		GameObject[] array = attachpoints;
		foreach (GameObject gameObject in array)
		{
			num++;
			if ((bool)gameObject && gameObject.transform.childCount > 0 && num > countthrough)
			{
				countthrough++;
			}
		}
	}

	public void AttachShip(GameObject ship)
	{
		ship.transform.parent = attachpoints[countthrough].transform;
		ship.transform.localPosition = new Vector3(0f, 0f, 0f);
		ship.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
		rep();
	}
}
