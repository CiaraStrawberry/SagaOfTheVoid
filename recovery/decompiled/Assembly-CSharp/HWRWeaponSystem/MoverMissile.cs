using UnityEngine;

namespace HWRWeaponSystem;

public class MoverMissile : WeaponBase
{
	public string selftag;

	public float Damping = 3f;

	public float Speed = 80f;

	public float SpeedMax = 80f;

	public float SpeedMult = 1f;

	public Vector3 Noise = new Vector3(20f, 20f, 20f);

	public float TargetLockDirection = 0.5f;

	public int DistanceLock = 70;

	public int DurationLock = 40;

	public bool Seeker;

	public float LifeTime = 5f;

	private bool locked;

	private int timetorock;

	private float timeCount;

	private float speedTemp;

	public GameObject target1;

	private void Awake()
	{
		speedTemp = Speed;
		objectPool = GetComponent<ObjectPool>();
		if ((bool)objectPool && WeaponSystem.Pool != null)
		{
			objectPool.LifeTime = LifeTime;
		}
		InvokeRepeating("fixed1", 0f, 1f);
	}

	private void Start()
	{
		if ((bool)objectPool && WeaponSystem.Pool != null)
		{
			objectPool.LifeTime = LifeTime;
			objectPool.SetDestroy(LifeTime);
		}
		else
		{
			Object.Destroy(gameObject, LifeTime);
		}
		timeCount = Time.time;
	}

	public void OnEnable()
	{
		Speed = speedTemp;
		timeCount = Time.time;
		if ((bool)objectPool)
		{
			objectPool.LifeTime = LifeTime;
			objectPool.SetDestroy(LifeTime);
		}
	}

	private void fixed1()
	{
	}

	private void Update()
	{
		if (WeaponSystem.Pool != null && objectPool != null && !objectPool.Active)
		{
			return;
		}
		if (Time.time >= timeCount + LifeTime - 0.5f && (bool)GetComponent<Damage>())
		{
			GetComponent<Damage>().Active();
			timeCount = Time.time;
		}
		if ((bool)target1)
		{
			Quaternion b = Quaternion.LookRotation(target1.transform.position - transform.transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, b, Time.deltaTime * Damping);
			transform.position += transform.forward * Time.deltaTime * 150f;
			Vector3 normalized = (target1.transform.position - transform.position).normalized;
			float num = Vector3.Dot(normalized, transform.forward);
			if (!(num < TargetLockDirection))
			{
			}
		}
		else
		{
			transform.position += transform.forward * Time.deltaTime * 150f;
		}
		if (!Seeker)
		{
			return;
		}
		if (!locked && !Target)
		{
			float num2 = 2.1474836E+09f;
			for (int i = 0; i < TargetTag.Length; i++)
			{
				if (GameObject.FindGameObjectsWithTag(TargetTag[i]).Length <= 0)
				{
					continue;
				}
				GameObject[] array = GameObject.FindGameObjectsWithTag(TargetTag[i]);
				for (int j = 0; j < array.Length; j++)
				{
					if (!array[j])
					{
						continue;
					}
					Vector3 normalized2 = (array[j].transform.position - transform.position).normalized;
					float num3 = Vector3.Dot(normalized2, transform.forward);
					float num4 = Vector3.Distance(array[j].transform.position, transform.position);
					if (num3 >= TargetLockDirection && (float)DistanceLock > num4)
					{
						if (num2 > num4)
						{
							num2 = num4;
						}
						locked = true;
					}
				}
			}
			timetorock = 0;
		}
		else
		{
			timetorock++;
		}
		if (!Target)
		{
			locked = false;
		}
	}
}
