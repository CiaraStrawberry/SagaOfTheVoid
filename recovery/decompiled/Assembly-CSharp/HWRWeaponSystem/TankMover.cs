using UnityEngine;

namespace HWRWeaponSystem;

public class TankMover : MonoBehaviour
{
	public float Speed = 20f;

	public float TurnSpeed = 100f;

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
		transform.Rotate(new Vector3(0f, Input.GetAxis("Horizontal") * TurnSpeed * Time.deltaTime, 0f));
		transform.position += transform.forward * Input.GetAxis("Vertical") * Speed * Time.deltaTime;
	}
}
