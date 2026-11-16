using System.Collections;
using ParticlePlayground;
using UnityEngine;

public class SnapshotToLocalPosition : MonoBehaviour
{
	public Transform transformPoint;

	public PlaygroundParticlesC particles;

	public int mask = -1;

	private Vector3[] positionCache;

	private int snapshotToLoad = 1;

	private void Start()
	{
		if (particles == null)
		{
			particles = GetComponent<PlaygroundParticlesC>();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			StartCoroutine(LoadSnapshot());
		}
	}

	private IEnumerator LoadSnapshot()
	{
		positionCache = (Vector3[])particles.snapshots[snapshotToLoad].settings.snapshotData.position.Clone();
		for (int i = 0; i < particles.snapshots[snapshotToLoad].settings.snapshotData.position.Length; i++)
		{
			ref Vector3 reference = ref particles.snapshots[snapshotToLoad].settings.snapshotData.position[i];
			reference = transformPoint.TransformPoint(particles.snapshots[snapshotToLoad].settings.snapshotData.position[i]);
		}
		particles.snapshots[snapshotToLoad].loadTransform = false;
		if (mask < 0)
		{
			particles.Load(snapshotToLoad);
		}
		else
		{
			particles.LoadAndApplyMask(snapshotToLoad, mask);
		}
		while (particles.IsLoading())
		{
			yield return null;
		}
		particles.snapshots[snapshotToLoad].settings.snapshotData.position = (Vector3[])positionCache.Clone();
		snapshotToLoad = (snapshotToLoad + 1) % particles.snapshots.Count;
	}
}
