using UnityEngine;

namespace HWRWeaponSystem;

public class WeaponController : MonoBehaviour
{
	public string[] TargetTag = new string[1] { "Enemy" };

	public WeaponLauncher[] WeaponLists;

	public int CurrentWeapon;

	public bool test;

	public bool ShowCrosshair;

	public GameObject target;

	private WeaponController conroller;

	private AILook AIL;

	private void Awake()
	{
		AIL = GetComponent<AILook>();
		if (transform.GetComponentsInChildren(typeof(WeaponLauncher)).Length > 0)
		{
			Component[] componentsInChildren = transform.GetComponentsInChildren(typeof(WeaponLauncher));
			WeaponLists = new WeaponLauncher[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				WeaponLists[i] = componentsInChildren[i].GetComponent<WeaponLauncher>();
				WeaponLists[i].TargetTag = TargetTag;
			}
		}
	}

	public WeaponLauncher GetCurrentWeapon()
	{
		if (CurrentWeapon < WeaponLists.Length && WeaponLists[CurrentWeapon] != null)
		{
			return WeaponLists[CurrentWeapon];
		}
		return null;
	}

	private void Start()
	{
		conroller = GetComponent<WeaponController>();
		for (int i = 0; i < WeaponLists.Length; i++)
		{
			if (WeaponLists[i] != null)
			{
				WeaponLists[i].TargetTag = TargetTag;
				WeaponLists[i].ShowCrosshair = ShowCrosshair;
			}
		}
	}

	private void Update()
	{
		if (AIL == null)
		{
		}
		if ((bool)AIL && AIL.target != null)
		{
			target = AIL.target;
		}
		for (int i = 0; i < WeaponLists.Length; i++)
		{
			if (WeaponLists[i] != null)
			{
				WeaponLists[i].OnActive = false;
			}
		}
		if (CurrentWeapon < WeaponLists.Length && WeaponLists[CurrentWeapon] != null)
		{
			WeaponLists[CurrentWeapon].OnActive = true;
		}
	}

	public void LaunchWeapon(int index)
	{
		CurrentWeapon = index;
		if (test)
		{
		}
		if (CurrentWeapon < WeaponLists.Length && WeaponLists[index] != null)
		{
			WeaponLists[index].Shoot();
		}
	}

	public void SwitchWeapon()
	{
		CurrentWeapon++;
		if (CurrentWeapon >= WeaponLists.Length)
		{
			CurrentWeapon = 0;
		}
		for (int i = 0; i < WeaponLists.Length; i++)
		{
			if (CurrentWeapon == i)
			{
				WeaponLists[i].OnActive = true;
			}
			else
			{
				WeaponLists[i].OnActive = false;
			}
		}
	}

	public void HideWeapon(GameObject weapon, bool show)
	{
		Renderer[] componentsInChildren = weapon.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.enabled = show;
		}
	}

	public void LaunchWeapon()
	{
		if (test)
		{
			Debug.Log(transform.gameObject);
		}
		if (CurrentWeapon < WeaponLists.Length && WeaponLists[CurrentWeapon] != null)
		{
			WeaponLists[CurrentWeapon].target1 = target;
			WeaponLists[CurrentWeapon].Shoot();
		}
	}
}
