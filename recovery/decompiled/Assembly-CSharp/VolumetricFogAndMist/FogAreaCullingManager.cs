using UnityEngine;

namespace VolumetricFogAndMist;

public class FogAreaCullingManager : MonoBehaviour
{
	public VolumetricFog fog;

	private bool isSpherical;

	private void OnEnable()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		isSpherical = component.sharedMesh.vertexCount > 24;
		if (!Application.isPlaying)
		{
			Debug.Log("Fog Areas should be created on runtime.");
		}
	}

	private void OnBecameVisible()
	{
		if (fog != null)
		{
			fog.enabled = true;
		}
	}

	private void OnBecameInvisible()
	{
		if (fog != null)
		{
			fog.enabled = false;
		}
	}

	private void OnDestroy()
	{
		if (fog != null)
		{
			fog.DestroySelf();
		}
	}

	private void Update()
	{
		UpdateFogAreaExtents();
	}

	public void UpdateFogAreaExtents()
	{
		if (fog == null)
		{
			Object.Destroy(gameObject);
			return;
		}
		fog.fogAreaPosition = transform.position;
		fog.fogAreaRadius = transform.localScale.x * 0.5f;
		if (isSpherical)
		{
			transform.localScale = Vector3.one * transform.localScale.x;
			fog.fogAreaHeight = 0f;
			fog.fogAreaDepth = 0f;
		}
		else
		{
			fog.fogAreaHeight = transform.localScale.y * 0.5f;
			fog.fogAreaDepth = transform.localScale.z * 0.5f;
		}
	}
}
