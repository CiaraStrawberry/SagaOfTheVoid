using UnityEngine;

namespace HWRWeaponSystem;

public class TankMover2D : MonoBehaviour
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
		transform.position += Vector3.right * Input.GetAxis("Horizontal") * Speed * Time.deltaTime;
	}
}
