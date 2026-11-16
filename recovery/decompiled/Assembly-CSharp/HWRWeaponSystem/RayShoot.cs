using UnityEngine;

namespace HWRWeaponSystem;

public class RayShoot : DamageBase
{
	public int Range = 10000;

	public Vector3 AimPoint;

	public GameObject Explosion;

	public float LifeTime = 1f;

	public LineRenderer Trail;

	private ObjectPool objPool;

	private bool actived;

	private void Awake()
	{
		objPool = GetComponent<ObjectPool>();
	}

	private void Start()
	{
		ShootRay();
		actived = true;
	}

	private void OnEnable()
	{
		if ((bool)objPool)
		{
			objPool.SetDestroy(LifeTime);
		}
		if (actived)
		{
			ShootRay();
		}
	}

	private void ShootRay()
	{
		if ((bool)GetComponent<Collider>())
		{
			Physics.IgnoreCollision(GetComponent<Collider>(), Owner.GetComponent<Collider>());
			if ((bool)Owner.transform.root)
			{
				Collider[] componentsInChildren = Owner.transform.root.GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren)
				{
					Physics.IgnoreCollision(GetComponent<Collider>(), collider);
				}
			}
		}
		GameObject gameObject = null;
		if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, Range))
		{
			AimPoint = hitInfo.point;
			if (Explosion != null)
			{
				gameObject = ((!(WeaponSystem.Pool != null)) ? Object.Instantiate(Explosion, AimPoint, transform.rotation) : WeaponSystem.Pool.Instantiate(Explosion, AimPoint, transform.rotation));
			}
			hitInfo.collider.gameObject.SendMessage("ApplyDamage", 5, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			AimPoint = transform.position + transform.forward * Range;
			if (Explosion != null)
			{
				gameObject = ((!(WeaponSystem.Pool != null)) ? Object.Instantiate(Explosion, AimPoint, transform.rotation) : WeaponSystem.Pool.Instantiate(Explosion, AimPoint, transform.rotation));
			}
		}
		if ((bool)gameObject)
		{
			gameObject.transform.forward = transform.forward;
			DamageBase component = gameObject.GetComponent<DamageBase>();
			if ((bool)component)
			{
				component.TargetTag = TargetTag;
			}
		}
		if ((bool)Trail)
		{
			Trail.SetPosition(0, transform.position);
			Trail.SetPosition(1, AimPoint);
		}
		if (WeaponSystem.Pool == null)
		{
			Object.Destroy(base.gameObject, LifeTime);
		}
	}

	private void Update()
	{
	}
}
