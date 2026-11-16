using UnityEngine;

namespace Forge3D;

public class F3DWarpJumpTunnel : MonoBehaviour
{
	private new Transform transform;

	private MeshRenderer meshRenderer;

	public float StartDelay;

	public float FadeDelay;

	public Vector3 ScaleTo;

	public float ScaleTime;

	public float ColorTime;

	public float ColorFadeTime;

	public float RotationSpeed;

	private bool grow;

	private float alpha;

	private int alphaID;

	private void Awake()
	{
		transform = GetComponent<Transform>();
		meshRenderer = GetComponent<MeshRenderer>();
		alphaID = Shader.PropertyToID("_Alpha");
	}

	public void OnSpawned()
	{
	}

	private void ToggleGrow()
	{
		grow = !grow;
	}

	private void Update()
	{
		transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
		if (grow)
		{
			transform.localScale = Vector3.Lerp(transform.localScale, ScaleTo, Time.deltaTime * ScaleTime);
			alpha = Mathf.Lerp(alpha, 1f, Time.deltaTime * ColorTime);
			meshRenderer.material.SetFloat(alphaID, alpha);
		}
		else
		{
			alpha = Mathf.Lerp(alpha, 0f, Time.deltaTime * ColorFadeTime);
			meshRenderer.material.SetFloat(alphaID, alpha);
		}
	}
}
