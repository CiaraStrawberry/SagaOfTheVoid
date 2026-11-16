using UnityEngine;

namespace ParticlePlayground;

public class PlaygroundEventTrailPoint
{
	[HideInInspector]
	public int trailId;

	[HideInInspector]
	public int pointId;

	[HideInInspector]
	public Vector3 position;

	[HideInInspector]
	public float width;

	[HideInInspector]
	public float lifetime;

	[HideInInspector]
	public float timeCreated;

	public void Update(int trailId, int pointId, Vector3 position, float width, float lifetime, float timeCreated)
	{
		this.trailId = trailId;
		this.pointId = pointId;
		this.position = position;
		this.width = width;
		this.lifetime = lifetime;
		this.timeCreated = timeCreated;
	}

	public PlaygroundEventTrailPoint Clone()
	{
		PlaygroundEventTrailPoint playgroundEventTrailPoint = new PlaygroundEventTrailPoint();
		playgroundEventTrailPoint.Update(trailId, pointId, position, width, lifetime, timeCreated);
		return playgroundEventTrailPoint;
	}
}
