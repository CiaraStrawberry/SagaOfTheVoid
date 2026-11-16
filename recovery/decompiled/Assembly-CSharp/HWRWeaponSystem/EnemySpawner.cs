using UnityEngine;

namespace HWRWeaponSystem;

public class EnemySpawner : MonoBehaviour
{
	public GameObject ObjectSpawn;

	public float ScaleMult = 1f;

	public bool RandomScale = true;

	private float timeSpawnTemp;

	public float TimeSpawn = 20f;

	public float ObjectCount;

	public int Radiun;

	public float Delay = 3f;

	private int numberOfSpawned;

	private void Start()
	{
		if ((bool)GetComponent<Renderer>())
		{
			GetComponent<Renderer>().enabled = false;
		}
		timeSpawnTemp = Time.time + Delay;
	}

	private void Update()
	{
		if (!ObjectSpawn)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
		if ((float)array.Length < ObjectCount && Time.time >= timeSpawnTemp + TimeSpawn)
		{
			GameObject gameObject = Object.Instantiate(ObjectSpawn, transform.position + new Vector3(Random.Range(-Radiun, Radiun), transform.position.y, Random.Range(-Radiun, Radiun)), Quaternion.identity);
			if ((bool)gameObject.GetComponent<EnemyDead>())
			{
				gameObject.GetComponent<EnemyDead>().MoneyPlus += (int)((float)numberOfSpawned * 0.1f);
			}
			float num = gameObject.transform.localScale.x;
			if (RandomScale)
			{
				num = (float)Random.Range(0, 100) * 0.01f;
			}
			gameObject.transform.localScale = new Vector3(num, num, num) * ScaleMult;
			timeSpawnTemp = Time.time;
			numberOfSpawned++;
		}
	}
}
