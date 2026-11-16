using UnityEngine;

namespace HWRWeaponSystem;

public class GunHanddle : MonoBehaviour
{
	public WeaponController weapon;

	private void Start()
	{
		weapon = transform.GetComponentInChildren(typeof(WeaponController)).GetComponent<WeaponController>();
	}

	private void Update()
	{
		if (Input.GetButton("Fire1") && (bool)weapon)
		{
			weapon.LaunchWeapon();
		}
		if (Input.GetButton("Fire2") && (bool)weapon)
		{
			weapon.SwitchWeapon();
		}
	}
}
