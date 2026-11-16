using UnityEngine;

namespace HWRWeaponSystem;

public class ExplosionObject : MonoBehaviour
{
	public Vector3 Force;

	public GameObject Prefab;

	public int Num;

	public int Scale = 20;

	public AudioClip[] Sounds;

	public float LifeTimeObject = 2f;

	public bool RandomScale;

	private void OnEnable()
	{
		if (Sounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(Sounds[Random.Range(0, Sounds.Length)], transform.position);
		}
		if (!Prefab)
		{
			return;
		}
		for (int i = 0; i < Num; i++)
		{
			Vector3 vector = new Vector3(Random.Range(-10, 10), Random.Range(-10, 20), Random.Range(-10, 10)) / 10f;
			GameObject gameObject;
			if (WeaponSystem.Pool != null)
			{
				gameObject = WeaponSystem.Pool.Instantiate(Prefab, transform.position + vector, Random.rotation, LifeTimeObject);
			}
			else
			{
				gameObject = Object.Instantiate(Prefab, transform.position + vector, Random.rotation);
				Object.Destroy(gameObject, LifeTimeObject);
			}
			float num = Scale;
			if (RandomScale)
			{
				num = Random.Range(1, Scale);
			}
			if (num > 0f)
			{
				gameObject.transform.localScale = new Vector3(num, num, num);
			}
			if ((bool)gameObject.GetComponent<Rigidbody>())
			{
				Vector3 force = new Vector3(Random.Range(0f - Force.x, Force.x), Random.Range(0f - Force.y, Force.y), Random.Range(0f - Force.z, Force.z));
				gameObject.GetComponent<Rigidbody>().AddForce(force);
			}
		}
	}

	private void Start()
	{
	}
}
