using Forge3D;
using UnityEngine;

public class DespawnOnTimer : MonoBehaviour
{
	public float destroyontime = 1f;

	private float timepassed;

	public bool destroynormal;

	private GameObject World;

	private float StartScale;

	public bool loadtest;

	public void Start()
	{
		timepassed = 0f;
		World = GameObject.Find("WorldScaleBase");
		StartScale = World.transform.localScale.x;
		if ((bool)transform.parent && transform.parent.name != "Working" && !loadtest)
		{
			transform.parent = World.transform.Find("World").Find("Objects").Find("Working");
		}
	}

	private void Update()
	{
		timepassed += Time.deltaTime;
		if (timepassed > destroyontime)
		{
			F3DPoolManager.Pools["GeneratedPool"].Despawn(gameObject.transform);
			if (destroynormal)
			{
				Object.Destroy(gameObject);
			}
		}
		if (World.transform.localScale.x != StartScale)
		{
			F3DPoolManager.Pools["GeneratedPool"].Despawn(gameObject.transform);
		}
	}
}
