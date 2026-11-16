using UnityEngine;

namespace HWRWeaponSystem;

public class Damage : DamageBase
{
	public bool Explosive;

	public float ExplosionRadius = 20f;

	public float ExplosionForce = 1000f;

	public bool HitedActive = true;

	public float TimeActive;

	private float timetemp;

	private ObjectPool objPool;

	public string selftag;

	private void Awake()
	{
		objPool = GetComponent<ObjectPool>();
	}

	private void OnEnable()
	{
		timetemp = Time.time;
	}

	private void Start()
	{
		if ((bool)Owner && (bool)Owner.GetComponent<Collider>())
		{
			timetemp = Time.time;
		}
	}

	private void Update()
	{
		if ((!objPool || objPool.Active) && (!HitedActive || TimeActive > 0f) && Time.time >= timetemp + TimeActive)
		{
			Active();
		}
	}

	public void Active()
	{
		if ((bool)Effect)
		{
			if (WeaponSystem.Pool != null)
			{
				float num = Random.Range(0, 1);
				if (num == 1f)
				{
					WeaponSystem.Pool.Instantiate(Effect, transform.position, transform.rotation, 3f);
				}
			}
			else
			{
				GameObject obj = Object.Instantiate(Effect, transform.position, transform.rotation);
				Object.Destroy(obj, 3f);
			}
		}
		if (Explosive)
		{
			ExplosionDamage();
		}
		if ((bool)objPool)
		{
			objPool.OnDestroyed();
		}
		else
		{
			Object.Destroy(gameObject);
		}
	}

	private void ExplosionDamage()
	{
		Collider[] array = Physics.OverlapSphere(transform.position, ExplosionRadius);
		foreach (Collider collider in array)
		{
			if ((bool)collider && DoDamageCheck(collider.gameObject))
			{
				collider.gameObject.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void NormalDamage(Collider collision)
	{
		collision.gameObject.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag != selftag)
		{
		}
		if ((!objPool || objPool.Active || !(WeaponSystem.Pool != null)) && collision.gameObject != gameObject && collision.collider.tag != selftag && (collision.gameObject.tag == "FighterTeam1" || collision.gameObject.tag == "FighterTeam2" || collision.gameObject.tag == "Team1" || collision.gameObject.tag == "Team2"))
		{
			if (!Explosive)
			{
				NormalDamage(collision.collider);
			}
			Active();
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.tag != selftag)
		{
		}
		if ((!objPool || objPool.Active || !(WeaponSystem.Pool != null)) && collision.gameObject != gameObject && collision.tag != selftag && (collision.gameObject.tag == "FighterTeam1" || collision.gameObject.tag == "FighterTeam2" || collision.gameObject.tag == "Team1" || collision.gameObject.tag == "Team2"))
		{
			if (!Explosive)
			{
				NormalDamage(collision);
			}
			Active();
		}
	}
}
