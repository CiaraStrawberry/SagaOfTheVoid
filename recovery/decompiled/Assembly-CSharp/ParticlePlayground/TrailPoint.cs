using UnityEngine;

namespace ParticlePlayground;

public class TrailPoint
{
	public static float trailPointRemoveTime;

	public Vector3 position;

	public Vector3 velocity;

	public float lifetime;

	public float startLifetime;

	public float width;

	private bool _canRemove;

	private float _lastTimeUpdated;

	public TrailPoint(Vector3 position, float startLifetime, float creationTime)
	{
		this.position = position;
		lifetime = startLifetime;
		this.startLifetime = startLifetime;
		width = 0f;
		_lastTimeUpdated = creationTime;
	}

	public TrailPoint(Vector3 position, float startLifetime, float width, float creationTime)
	{
		this.position = position;
		lifetime = startLifetime;
		this.startLifetime = startLifetime;
		this.width = width;
		_lastTimeUpdated = creationTime;
	}

	public TrailPoint(Vector3 position, Vector3 velocity, float startLifetime, float width, float creationTime)
	{
		this.position = position;
		lifetime = startLifetime;
		this.startLifetime = startLifetime;
		this.width = width;
		this.velocity = velocity;
		_lastTimeUpdated = creationTime;
	}

	public void Update(float updateTime, float width)
	{
		lifetime -= updateTime - _lastTimeUpdated;
		if (lifetime <= 0f - trailPointRemoveTime)
		{
			_canRemove = true;
		}
		this.width = width;
		_lastTimeUpdated = updateTime;
	}

	public float GetNormalizedLifetime()
	{
		return 1f - lifetime / startLifetime;
	}

	public bool CanRemove()
	{
		return _canRemove;
	}
}
