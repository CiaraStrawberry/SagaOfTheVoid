using UnityEngine;

public class FleetNumControl : MonoBehaviour
{
	public int waythrough = 4;

	public GameObject capitals;

	public GameObject cruisers;

	public GameObject frigates;

	public GameObject destroyers;

	public GameObject corvettes;

	public GameObject LightCraft;

	private int i;

	private void Awake()
	{
		enableall();
	}

	private void Start()
	{
		checkupdate();
	}

	private void Update()
	{
	}

	public void addone()
	{
		waythrough++;
		if (waythrough > 5)
		{
			waythrough = 0;
		}
		checkupdate();
	}

	public void backone()
	{
		waythrough--;
		if (waythrough < 0)
		{
			waythrough = 5;
		}
		checkupdate();
	}

	public void checkupdate()
	{
		disableall();
		switch (waythrough)
		{
		case 5:
			LightCraft.SetActive(value: true);
			break;
		case 4:
			corvettes.SetActive(value: true);
			break;
		case 3:
			capitals.SetActive(value: true);
			break;
		case 2:
			cruisers.SetActive(value: true);
			break;
		case 1:
			frigates.SetActive(value: true);
			break;
		case 0:
			destroyers.SetActive(value: true);
			break;
		}
	}

	private void disableall()
	{
		LightCraft.SetActive(value: false);
		corvettes.SetActive(value: false);
		capitals.SetActive(value: false);
		cruisers.SetActive(value: false);
		frigates.SetActive(value: false);
		destroyers.SetActive(value: false);
	}

	private void enableall()
	{
		LightCraft.SetActive(value: true);
		corvettes.SetActive(value: true);
		capitals.SetActive(value: true);
		cruisers.SetActive(value: true);
		frigates.SetActive(value: true);
		destroyers.SetActive(value: true);
	}
}
