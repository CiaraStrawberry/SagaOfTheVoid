using ParticlePlayground;
using UnityEngine;

public class ParticleSystemScaler : MonoBehaviour
{
	public float scale = 1f;

	public bool scaleVelocity = true;

	public bool scaleParticleSize = true;

	public bool scaleLifetimePositioning = true;

	public bool scaleOverflowOffset = true;

	public bool scaleSourceScatter = true;

	private float origVelocityScale;

	private float origScale;

	private float origLifetimePositioningScale;

	private Vector3 origOverflowOffset;

	private Vector3 origSourceScatterMin;

	private Vector3 origSourceScatterMax;

	private PlaygroundParticlesC particles;

	private void OnEnable()
	{
		if (particles == null)
		{
			particles = GetComponent<PlaygroundParticlesC>();
		}
		origVelocityScale = particles.velocityScale;
		origScale = particles.scale;
		origLifetimePositioningScale = particles.lifetimePositioningScale;
		origOverflowOffset = particles.overflowOffset;
		origSourceScatterMin = particles.sourceScatterMin;
		origSourceScatterMax = particles.sourceScatterMax;
	}

	private void OnDisable()
	{
		particles.velocityScale = origVelocityScale;
		particles.scale = origScale;
		particles.lifetimePositioningScale = origLifetimePositioningScale;
		particles.overflowOffset = origOverflowOffset;
		particles.sourceScatterMin = origSourceScatterMin;
		particles.sourceScatterMax = origSourceScatterMax;
	}

	private void Update()
	{
		if (scaleVelocity)
		{
			particles.velocityScale = origVelocityScale * scale;
		}
		if (scaleParticleSize)
		{
			particles.scale = origScale * scale;
		}
		if (scaleLifetimePositioning)
		{
			particles.lifetimePositioningScale = origLifetimePositioningScale * scale;
		}
		if (scaleOverflowOffset)
		{
			particles.overflowOffset = origOverflowOffset * scale;
		}
		if (scaleSourceScatter && particles.applySourceScatter && (particles.sourceScatterMin != origSourceScatterMin * scale || particles.sourceScatterMax != origSourceScatterMax * scale))
		{
			particles.sourceScatterMin = origSourceScatterMin * scale;
			particles.sourceScatterMax = origSourceScatterMax * scale;
			particles.RefreshScatter();
		}
	}
}
