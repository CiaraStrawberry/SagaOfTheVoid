using UnityEngine;

namespace HWRWeaponSystem;

public class Spawner : MonoBehaviour
{
	public GameObject ObjectSpawn;

	public float ScaleMult = 1f;

	public bool RandomScale = true;

	private float timeSpawnTemp;

	public float TimeSpawn = 20f;

	public float ObjectCount;

	public int Radiun;

	public bool LockX;

	public bool LockY;

	public bool LockZ;

	private void Start()
	{
		if ((bool)GetComponent<Renderer>())
		{
			GetComponent<Renderer>().enabled = false;
		}
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
			float num = gameObject.transform.localScale.x;
			if (RandomScale)
			{
				num = (float)Random.Range(0, 100) * 0.01f;
			}
			gameObject.transform.localScale = new Vector3(num, num, num) * ScaleMult;
			gameObject.transform.position = AxisLock(gameObject.transform.position);
			timeSpawnTemp = Time.time;
		}
	}

	public Vector3 AxisLock(Vector3 axis)
	{
		if (LockX)
		{
			axis.x = transform.position.x;
		}
		if (LockY)
		{
			axis.y = transform.position.y;
		}
		if (LockZ)
		{
			axis.z = transform.position.z;
		}
		return axis;
	}
}
