using UnityEngine;

namespace HWRWeaponSystem;

public class TankHUD : MonoBehaviour
{
	private int currentWeapon;

	private WeaponController weaponManager;

	private void Start()
	{
		weaponManager = GetComponent<WeaponController>();
	}

	private void Update()
	{
		if (!weaponManager)
		{
			return;
		}
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.LoadLevel("Menu");
		}
		if (Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			weaponManager.CurrentWeapon--;
			if (weaponManager.CurrentWeapon < 0)
			{
				weaponManager.CurrentWeapon = weaponManager.WeaponLists.Length - 1;
			}
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			weaponManager.CurrentWeapon++;
			if (weaponManager.CurrentWeapon >= weaponManager.WeaponLists.Length)
			{
				weaponManager.CurrentWeapon = 0;
			}
		}
		currentWeapon = weaponManager.CurrentWeapon;
	}

	private void OnGUI()
	{
		if ((bool)weaponManager && currentWeapon <= weaponManager.WeaponLists.Length)
		{
			GUI.skin.label.fontSize = 15;
			GUI.Label(new Rect(20f, 20f, 300f, 30f), "Weapon Index " + currentWeapon);
			GUI.Label(new Rect(20f, Screen.height - 95, 300f, 30f), "Esc back to mainmenu");
			GUI.Label(new Rect(20f, Screen.height - 50, 300f, 30f), "Scroll Mouse to Change weapons");
			GUI.Label(new Rect(20f, Screen.height - 70, 300f, 30f), "W A S D to Move");
			if (!weaponManager.WeaponLists[currentWeapon].InfinityAmmo)
			{
				GUI.Label(new Rect(20f, 70f, 300f, 50f), "Ammo " + weaponManager.WeaponLists[currentWeapon].Ammo + " / " + weaponManager.WeaponLists[currentWeapon].AmmoMax);
			}
			else
			{
				GUI.Label(new Rect(20f, 70f, 300f, 50f), "Infinity ammo");
			}
			GUI.skin.label.fontSize = 25;
			GUI.Label(new Rect(20f, 40f, 300f, 50f), string.Empty + weaponManager.WeaponLists[currentWeapon].name);
		}
	}
}
