using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
	public List<GameObject> notspecials = new List<GameObject>();

	public List<GameObject> specials = new List<GameObject>();

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void reset(float size)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Pickup");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if ((bool)gameObject.transform.parent && (bool)gameObject.transform.parent.GetComponent<WeaponAttachPoint>())
			{
				gameObject.GetComponent<WeaponBaseClass>().reset(size);
			}
		}
	}

	public void supporton()
	{
		foreach (GameObject notspecial in notspecials)
		{
			notspecial.GetComponent<BoxCollider>().enabled = false;
			foreach (Transform item in notspecial.transform)
			{
				item.gameObject.SetActive(value: false);
			}
		}
		foreach (GameObject special in specials)
		{
			special.GetComponent<BoxCollider>().enabled = true;
			foreach (Transform item2 in special.transform)
			{
				item2.gameObject.SetActive(value: true);
			}
		}
	}

	public void supportoff()
	{
		foreach (GameObject notspecial in notspecials)
		{
			notspecial.GetComponent<BoxCollider>().enabled = true;
			foreach (Transform item in notspecial.transform)
			{
				item.gameObject.SetActive(value: true);
			}
		}
		foreach (GameObject special in specials)
		{
			special.GetComponent<BoxCollider>().enabled = false;
			foreach (Transform item2 in special.transform)
			{
				item2.gameObject.SetActive(value: false);
			}
		}
	}
}
