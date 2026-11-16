using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class ParticlePlaygroundTrail
{
	[HideInInspector]
	public bool update = true;

	[HideInInspector]
	public GameObject trailGameObject;

	[HideInInspector]
	public Transform trailTransform;

	[HideInInspector]
	public MeshRenderer trailRenderer;

	[HideInInspector]
	public MeshFilter trailMeshFilter;

	[HideInInspector]
	public Mesh trailMesh;

	[HideInInspector]
	public int particleId;

	[HideInInspector]
	public int minPointCache = 2;

	[HideInInspector]
	public int maxPointCache = 32767;

	[NonSerialized]
	public List<TrailPoint> trailPoints = new List<TrailPoint>();

	[NonSerialized]
	public Vector3[] meshVerticesCache;

	[NonSerialized]
	public Vector3[] meshNormalsCache;

	[NonSerialized]
	public Vector2[] meshUvsCache;

	[NonSerialized]
	public int[] meshTrianglesCache;

	[NonSerialized]
	public Color32[] meshColorsCache;

	private int _pointCache = 200;

	private int _birthIterator;

	private int _deathIterator;

	private Vector3 _particlePosition;

	private Vector3 _previousParticlePosition;

	private Vector3 _particleDirection;

	private Vector3 _lastAddedPointPosition;

	private Vector3 _lastAddedPointDirection;

	private bool _isDead;

	private bool _isReady;

	private bool _queuedForReuse;

	private float _timeCached;

	public ParticlePlaygroundTrail()
	{
		UpdateCache();
	}

	public ParticlePlaygroundTrail(int pointCache)
	{
		_pointCache = Mathf.Clamp(pointCache, minPointCache, maxPointCache);
		UpdateCache();
	}

	public void UpdateCache()
	{
		_timeCached = PlaygroundC.globalTime;
		meshVerticesCache = new Vector3[_pointCache * 2];
		meshNormalsCache = new Vector3[_pointCache * 2];
		meshUvsCache = new Vector2[_pointCache * 2];
		meshTrianglesCache = new int[(_pointCache - 1) * 6];
		meshColorsCache = new Color32[_pointCache * 2];
		_isReady = true;
	}

	public void UpdateMesh()
	{
		trailMesh.Clear();
		trailMesh.vertices = meshVerticesCache;
		trailMesh.uv = meshUvsCache;
		trailMesh.triangles = meshTrianglesCache;
		trailMesh.colors32 = meshColorsCache;
		trailMesh.normals = meshNormalsCache;
	}

	public void RecalculateBounds()
	{
		trailMesh.RecalculateBounds();
	}

	public void ClearTrail()
	{
		_birthIterator = 0;
		_deathIterator = 0;
		trailPoints.Clear();
		for (int i = 0; i < meshTrianglesCache.Length; i++)
		{
			meshTrianglesCache[i] = 0;
		}
		for (int j = 0; j < meshVerticesCache.Length; j++)
		{
			ref Vector3 reference = ref meshVerticesCache[j];
			reference = Vector3.zero;
		}
		UpdateMesh();
	}

	public void SetFirstPoint(Vector3 pos, Vector3 dir, float startWidth, float lifetime, float creationTime)
	{
		_particleDirection = dir;
		AddPoint(pos, startWidth, lifetime, creationTime);
		AddPoint(pos, startWidth, lifetime, creationTime);
		_birthIterator = 2;
		_deathIterator = 0;
	}

	public void SetLastPoint(Vector3 pos, Vector3 dir, float startWidth, float lifetime, float creationTime)
	{
		_particleDirection = dir;
		AddPoint(pos, startWidth, lifetime, creationTime);
		Die();
	}

	public void AddPoint(Vector3 position, float width, float lifetime, float creationTime)
	{
		if (_birthIterator < _pointCache && !_isDead)
		{
			trailPoints.Add(new TrailPoint(position, lifetime, width, creationTime));
			AddPoint(position);
		}
	}

	public void AddPoint(Vector3 position, Vector3 velocity, float width, float lifetime, float creationTime)
	{
		if (_birthIterator < _pointCache && !_isDead)
		{
			trailPoints.Add(new TrailPoint(position, velocity, lifetime, width, creationTime));
			AddPoint(position);
		}
	}

	private void AddPoint(Vector3 position)
	{
		_lastAddedPointDirection = position - _previousParticlePosition;
		_lastAddedPointPosition = position;
		meshVerticesCache[_birthIterator * 2] = position;
		meshVerticesCache[_birthIterator * 2 + 1] = position;
		ref Vector3 reference = ref meshNormalsCache[_birthIterator * 2];
		reference = -Vector3.forward;
		ref Vector3 reference2 = ref meshNormalsCache[_birthIterator * 2 + 1];
		reference2 = -Vector3.forward;
		ref Vector2 reference3 = ref meshUvsCache[_birthIterator * 2];
		reference3 = new Vector2(1f, 0f);
		ref Vector2 reference4 = ref meshUvsCache[_birthIterator * 2 + 1];
		reference4 = new Vector2(0f, 1f);
		meshColorsCache[_birthIterator * 2] = default;
		meshColorsCache[_birthIterator * 2 + 1] = default;
		if (trailPoints.Count > 1)
		{
			int num = _birthIterator * 2;
			int num2 = (_birthIterator - 1) * 6;
			meshTrianglesCache[num2] = num - 2;
			meshTrianglesCache[num2 + 1] = num - 1;
			meshTrianglesCache[num2 + 2] = num;
			meshTrianglesCache[num2 + 3] = num;
			meshTrianglesCache[num2 + 4] = num - 1;
			meshTrianglesCache[num2 + 5] = num + 1;
		}
		NextPoint();
	}

	public void RemovePoint(int index)
	{
		if (_deathIterator < _birthIterator - 2)
		{
			ref Vector3 reference = ref meshVerticesCache[_deathIterator * 2];
			reference = meshVerticesCache[_deathIterator * 2 + 2];
			ref Vector3 reference2 = ref meshVerticesCache[_deathIterator * 2 + 1];
			reference2 = meshVerticesCache[_deathIterator * 2 + 3];
			meshColorsCache[_deathIterator * 2] = default;
			meshColorsCache[_deathIterator * 2 + 1] = default;
			_deathIterator++;
		}
	}

	public void SetColor(int index, Color32 color)
	{
		meshColorsCache[index * 2] = color;
		meshColorsCache[index * 2 + 1] = color;
	}

	public void NextPoint()
	{
		_birthIterator++;
	}

	public Vector3 GetParticlePosition()
	{
		return _particlePosition;
	}

	public void SetParticlePosition(Vector3 position)
	{
		if (!_isDead)
		{
			_previousParticlePosition = _particlePosition;
			_particlePosition = position;
			_particleDirection = (position - _previousParticlePosition).normalized;
		}
	}

	public void SetParticleDirection(Vector3 direction)
	{
		_particleDirection = direction;
	}

	public Vector3 GetLastAddedPointPosition()
	{
		return _lastAddedPointPosition;
	}

	public Vector3 GetParticleDirection()
	{
		return _particleDirection;
	}

	public Vector3 GetLastAddedPointDirection()
	{
		return _lastAddedPointDirection;
	}

	public float GetPathDeviation()
	{
		return Vector3.Angle(_lastAddedPointDirection, _particleDirection);
	}

	public int GetBirthIterator()
	{
		return _birthIterator;
	}

	public int GetDeathIterator()
	{
		return _deathIterator;
	}

	public int GetPointCache()
	{
		return _pointCache;
	}

	public float TimeCached()
	{
		return _timeCached;
	}

	public bool CanRemovePoint(int point)
	{
		return trailPoints[point].CanRemove();
	}

	public bool CanRemoveTrail()
	{
		if (trailPoints.Count == 0 || !_isReady)
		{
			return false;
		}
		return trailPoints[trailPoints.Count - 1].CanRemove();
	}

	public void WakeUp()
	{
		_isDead = false;
	}

	public void Die()
	{
		_isDead = true;
	}

	public bool IsDead()
	{
		return _isDead;
	}

	public void QueueForReuse()
	{
		ClearTrail();
		_queuedForReuse = true;
	}

	public void DequeueForReuse()
	{
		WakeUp();
		_queuedForReuse = false;
	}

	public bool IsQueuedForReuse()
	{
		return _queuedForReuse;
	}
}
