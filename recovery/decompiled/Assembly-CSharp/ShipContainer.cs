using UnityEngine;

public class ShipContainer
{
	public GameObject ship;

	public string namestring;

	public float size;

	public Vector3 startloc;

	public bool special;

	public ShipContainer(GameObject nam, float siz, string namestri, Vector3 startlo, bool specialin)
	{
		ship = nam;
		size = siz;
		namestring = namestri;
		startloc = startlo;
		special = specialin;
	}
}
