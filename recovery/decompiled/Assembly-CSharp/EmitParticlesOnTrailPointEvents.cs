using ParticlePlayground;
using UnityEngine;

[ExecuteInEditMode]
public class EmitParticlesOnTrailPointEvents : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public PlaygroundTrails trails;

	public Vector3 velocity;

	public Color32 color = Color.white;

	private void OnEnable()
	{
		if (trails != null)
		{
			trails.trailPointEvent += OnTrailPoint;
		}
	}

	private void OnDisable()
	{
		if (trails != null)
		{
			trails.trailPointEvent -= OnTrailPoint;
		}
	}

	private void OnTrailPoint(PlaygroundEventTrailPoint trailPoint)
	{
		Vector3 position = trailPoint.position;
		particles.Emit(position, velocity, color);
	}
}
