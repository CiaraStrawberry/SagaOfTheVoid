using UnityEngine;

namespace HWRWeaponSystem;

public class MoverBullet : WeaponBase
{
	public int Lifetime;

	public float Speed = 80f;

	public float SpeedMax = 80f;

	public float SpeedMult = 1f;

	private float speedTemp;

	private void Awake()
	{
		speedTemp = Speed;
		objectPool = GetComponent<ObjectPool>();
	}

	private void Start()
	{
		if ((bool)objectPool && WeaponSystem.Pool != null)
		{
			objectPool.SetDestroy(Lifetime);
		}
		else
		{
			Object.Destroy(gameObject, Lifetime);
		}
	}

	public void OnEnable()
	{
		Speed = speedTemp;
		if ((bool)objectPool && WeaponSystem.Pool != null)
		{
			objectPool.SetDestroy(Lifetime);
		}
	}

	private void FixedUpdate()
	{
		if ((!(WeaponSystem.Pool != null) || !(objectPool != null) || objectPool.Active) && (bool)GetComponent<Rigidbody>())
		{
			if (!RigidbodyProjectile)
			{
				GetComponent<Rigidbody>().velocity = transform.forward * Speed;
			}
			else if (GetComponent<Rigidbody>().velocity.normalized != Vector3.zero)
			{
				transform.forward = GetComponent<Rigidbody>().velocity.normalized;
			}
			if (Speed < SpeedMax)
			{
				Speed += SpeedMult * Time.fixedDeltaTime;
			}
		}
	}
}
