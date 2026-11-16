using System;
using UnityEngine;

namespace Forge3D;

[RequireComponent(typeof(LineRenderer))]
public class F3DCurvedBeam : MonoBehaviour
{
	public Transform dest;

	public float beamScale;

	public float UVTime;

	private LineRenderer lineRenderer;

	public int curvePoints;

	public float curveHeight;

	private float initialBeamOffset;

	private void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		initialBeamOffset = UnityEngine.Random.Range(0f, 5f);
		lineRenderer.SetVertexCount(curvePoints);
	}

	private void Update()
	{
		lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(Time.time * UVTime + initialBeamOffset, 0f));
		float num = Vector3.Distance(transform.position, dest.position);
		lineRenderer.SetPosition(0, transform.position);
		float num2 = (float)Math.PI / (float)(curvePoints - 1);
		for (int i = 1; i < curvePoints - 1; i++)
		{
			float num3 = num / (float)(curvePoints - 1) * (float)i;
			Vector3 vector = Vector3.Normalize(dest.position - transform.position) * num3;
			float num4 = Mathf.Sin(num2 * (float)i) * curveHeight;
			vector += transform.up * num4;
			lineRenderer.SetPosition(i, transform.position + vector);
		}
		lineRenderer.SetPosition(curvePoints - 1, dest.position);
		float x = num * (beamScale / 10f);
		lineRenderer.material.SetTextureScale("_MainTex", new Vector2(x, 1f));
	}
}
