using UnityEngine;

namespace VRTK.Examples;

public class Gun : VRTK_InteractableObject
{
	private GameObject bullet;

	private float bulletSpeed = 1000f;

	private float bulletLife = 5f;

	public override void StartUsing(GameObject usingObject)
	{
		base.StartUsing(usingObject);
		FireBullet();
	}

	protected void Start()
	{
		bullet = transform.Find("Bullet").gameObject;
		bullet.SetActive(value: false);
	}

	private void FireBullet()
	{
		GameObject gameObject = Object.Instantiate(bullet, bullet.transform.position, bullet.transform.rotation);
		gameObject.SetActive(value: true);
		Rigidbody component = gameObject.GetComponent<Rigidbody>();
		component.AddForce(-bullet.transform.forward * bulletSpeed);
		Object.Destroy(gameObject, bulletLife);
	}
}
