using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ParticlePlayground;

[ExecuteInEditMode]
public class PlaygroundTrails : MonoBehaviour
{
	[HideInInspector]
	public PlaygroundParticlesC playgroundSystem;

	[HideInInspector]
	public Material material;

	[HideInInspector]
	public Gradient lifetimeColor = new Gradient();

	[HideInInspector]
	public AnimationCurve pointArrayAlpha;

	[HideInInspector]
	public TrailColorMode colorMode;

	[HideInInspector]
	public TrailUvMode uvMode;

	[HideInInspector]
	public TrailRenderMode renderMode;

	[HideInInspector]
	public Transform billboardTransform;

	[HideInInspector]
	public Vector3 customRenderScale = Vector3.one;

	[HideInInspector]
	public bool receiveShadows;

	[HideInInspector]
	public ShadowCastingMode shadowCastingMode;

	[HideInInspector]
	public int layer = -1;

	[HideInInspector]
	public float time = 3f;

	public AnimationCurve timeWidth;

	public float widthScale = 0.1f;

	public float minVertexDistance = 0.1f;

	public float maxVertexDistance = 100f;

	public float maxPathDeviation = 1f;

	public bool createPointsOnCollision;

	public int maxPoints = 100;

	public bool createFirstPointOnParticleBirth;

	public bool createLastPointOnParticleDeath;

	public bool multithreading = true;

	[HideInInspector]
	public PlaygroundEventC birthEvent;

	[HideInInspector]
	public PlaygroundEventC deathEvent;

	[HideInInspector]
	public PlaygroundEventC collisionEvent;

	[NonSerialized]
	public List<ParticlePlaygroundTrail> trails = new List<ParticlePlaygroundTrail>();

	private Transform _parentTransform;

	private GameObject _parentGameObject;

	private Material _materialCache;

	private float _calculationStartTime;

	private int _currentParticleCount;

	private float _currentParticleMinLifetime;

	private float _currentParticleMaxLifetime;

	private bool _currentLoopValue;

	private bool _localSpace;

	private Vector3 _billboardTransformPosition;

	private bool _isDoneThread = true;

	private Matrix4x4 _localMatrix;

	private bool _hasTrailPointEventListener;

	private PlaygroundEventTrailPoint _eventTrailPoint = new PlaygroundEventTrailPoint();

	private Action _trailsCalculationAction;

	private readonly Queue<TrailParticleInfo> _birthQueue = new Queue<TrailParticleInfo>();

	private readonly Queue<int> _trailReuseQueue = new Queue<int>();

	[HideInInspector]
	public event OnPlaygroundTrailPoint trailPointEvent;

	private void OnEnable()
	{
		if (playgroundSystem == null)
		{
			playgroundSystem = GetComponent<PlaygroundParticlesC>();
		}
		if (billboardTransform == null)
		{
			billboardTransform = Camera.main.transform;
		}
		if (material == null)
		{
			material = new Material(Shader.Find("Playground/Vertex Color"));
			_materialCache = material;
		}
		ResetTrails();
		AddRequiredParticleEvents();
		if (timeWidth == null)
		{
			timeWidth = new AnimationCurve(DefaultWidthKeys());
		}
		if (pointArrayAlpha == null)
		{
			pointArrayAlpha = new AnimationCurve(DefaultWidthKeys());
		}
		_isDoneThread = true;
	}

	private void OnDisable()
	{
		DestroyAllTrails();
		RemoveRequiredEvents();
	}

	private void OnDestroy()
	{
		DestroyAllTrails();
		RemoveRequiredEvents();
	}

	private void Update()
	{
		maxPoints = Mathf.Clamp(maxPoints, 2, 32767);
		if (billboardTransform != null)
		{
			_billboardTransformPosition = billboardTransform.position;
		}
		if (playgroundSystem == null || !playgroundSystem.IsReady() || playgroundSystem.IsSettingParticleCount() || playgroundSystem.IsSettingLifetime() || playgroundSystem.particleCache == null || playgroundSystem.particleCache.Length == 0)
		{
			return;
		}
		if (_currentParticleCount != playgroundSystem.particleCount || _currentLoopValue != playgroundSystem.loop || _currentParticleMinLifetime != playgroundSystem.lifetimeMin || _currentParticleMaxLifetime != playgroundSystem.lifetime || _localSpace != (playgroundSystem.shurikenParticleSystem.simulationSpace == ParticleSystemSimulationSpace.Local))
		{
			ResetTrails();
		}
		if (_localSpace)
		{
			_localMatrix.SetTRS(playgroundSystem.particleSystemTransform.position, playgroundSystem.particleSystemTransform.rotation, playgroundSystem.particleSystemTransform.lossyScale);
		}
		if (material != _materialCache)
		{
			SetMaterial(material);
		}
		while (_isDoneThread && _birthQueue.Count > 0)
		{
			bool flag = trails.Count > 0 && _trailReuseQueue.Count > 0;
			int num = (flag ? _trailReuseQueue.Peek() : 0);
			if (flag && (num < 0 || num > trails.Count - 1 || !trails[num].IsQueuedForReuse()))
			{
				_trailReuseQueue.Dequeue();
				flag = false;
			}
			if (flag)
			{
				ReuseTrail(_trailReuseQueue.Dequeue(), _birthQueue.Dequeue());
			}
			else
			{
				CreateTrail(_birthQueue.Dequeue());
			}
		}
		if (_isDoneThread)
		{
			for (int i = 0; i < trails.Count; i++)
			{
				if (trails[i].trailPoints != null && trails[i].trailPoints.Count > 1 && trails[i].trailPoints[trails[i].trailPoints.Count - 1] != null && trails[i].CanRemoveTrail() && !trails[i].IsQueuedForReuse())
				{
					_trailReuseQueue.Enqueue(i);
					trails[i].QueueForReuse();
				}
			}
		}
		for (int j = 0; j < trails.Count; j++)
		{
			ParticlePlaygroundTrail particlePlaygroundTrail = trails[j];
			particlePlaygroundTrail.trailRenderer.receiveShadows = receiveShadows;
			particlePlaygroundTrail.trailRenderer.shadowCastingMode = shadowCastingMode;
			if (_isDoneThread && !particlePlaygroundTrail.IsQueuedForReuse())
			{
				particlePlaygroundTrail.UpdateMesh();
			}
		}
		_hasTrailPointEventListener = trailPointEvent != null;
		if (_trailsCalculationAction == null)
		{
			_trailsCalculationAction = CalculationDelegate;
		}
		_calculationStartTime = ((!Application.isPlaying) ? Time.realtimeSinceStartup : Time.time);
		if (multithreading)
		{
			if (_isDoneThread)
			{
				_isDoneThread = false;
				PlaygroundC.RunAsync(_trailsCalculationAction);
			}
		}
		else
		{
			_isDoneThread = false;
			_trailsCalculationAction();
		}
	}

	private void CalculationDelegate()
	{
		if (!_isDoneThread)
		{
			CalculateTrail();
			_isDoneThread = true;
		}
	}

	private void OnParticleBirthEvent(PlaygroundEventParticle particle)
	{
		_birthQueue.Enqueue(new TrailParticleInfo(particle.particleId, particle.position, particle.velocity));
	}

	private void OnParticleDeathEvent(PlaygroundEventParticle particle)
	{
		int oldestTrailWithParticleId = GetOldestTrailWithParticleId(particle.particleId);
		if (oldestTrailWithParticleId > -1)
		{
			if (createLastPointOnParticleDeath)
			{
				trails[oldestTrailWithParticleId].SetLastPoint(particle.position, particle.velocity, EvaluateWidth(0f), time, _calculationStartTime);
				return;
			}
			trails[oldestTrailWithParticleId].SetParticlePosition(particle.position);
			trails[oldestTrailWithParticleId].Die();
		}
	}

	private void OnParticleCollisionEvent(PlaygroundEventParticle particle)
	{
		if (createPointsOnCollision)
		{
			int newestTrailWithParticleId = GetNewestTrailWithParticleId(particle.particleId);
			if (newestTrailWithParticleId >= 0)
			{
				ParticlePlaygroundTrail particlePlaygroundTrail = trails[newestTrailWithParticleId];
				particlePlaygroundTrail.AddPoint(playgroundSystem.particleCache[particle.particleId].position, EvaluateWidth(0f), time, _calculationStartTime);
			}
		}
	}

	public PlaygroundEventC GetBirthEvent()
	{
		return birthEvent;
	}

	public PlaygroundEventC GetDeathEvent()
	{
		return deathEvent;
	}

	public PlaygroundEventC GetCollisionEvent()
	{
		return collisionEvent;
	}

	public void AddRequiredParticleEvents()
	{
		if (playgroundSystem != null)
		{
			birthEvent = GetEventFromType(EVENTTYPEC.Birth);
			if (birthEvent == null)
			{
				birthEvent = PlaygroundC.CreateEvent(playgroundSystem);
				birthEvent.broadcastType = EVENTBROADCASTC.EventListeners;
				birthEvent.eventType = EVENTTYPEC.Birth;
			}
			birthEvent.particleEvent += OnParticleBirthEvent;
			deathEvent = GetEventFromType(EVENTTYPEC.Death);
			if (deathEvent == null)
			{
				deathEvent = PlaygroundC.CreateEvent(playgroundSystem);
				deathEvent.broadcastType = EVENTBROADCASTC.EventListeners;
				deathEvent.eventType = EVENTTYPEC.Death;
			}
			deathEvent.particleEvent += OnParticleDeathEvent;
			collisionEvent = GetEventFromType(EVENTTYPEC.Collision);
			if (collisionEvent == null)
			{
				collisionEvent = PlaygroundC.CreateEvent(playgroundSystem);
				collisionEvent.broadcastType = EVENTBROADCASTC.EventListeners;
				collisionEvent.eventType = EVENTTYPEC.Collision;
			}
			collisionEvent.particleEvent += OnParticleCollisionEvent;
		}
	}

	public void RemoveRequiredEvents()
	{
		if (playgroundSystem != null)
		{
			if (birthEvent != null)
			{
				birthEvent.particleEvent -= OnParticleBirthEvent;
				birthEvent = null;
			}
			if (deathEvent != null)
			{
				deathEvent.particleEvent -= OnParticleDeathEvent;
				deathEvent = null;
			}
			if (collisionEvent != null)
			{
				collisionEvent.particleEvent -= OnParticleCollisionEvent;
				collisionEvent = null;
			}
		}
	}

	public PlaygroundEventC GetEventFromType(EVENTTYPEC eventType)
	{
		for (int i = 0; i < playgroundSystem.events.Count; i++)
		{
			if (playgroundSystem.events[i].eventType == eventType)
			{
				return playgroundSystem.events[i];
			}
		}
		return null;
	}

	public Keyframe[] DefaultWidthKeys()
	{
		Keyframe[] array = new Keyframe[2];
		array[0].time = 0f;
		array[1].time = 1f;
		array[0].value = 1f;
		array[1].value = 1f;
		return array;
	}

	public void SetMaterial(Material material)
	{
		for (int i = 0; i < trails.Count; i++)
		{
			if (trails[i] != null && trails[i].trailRenderer != null)
			{
				trails[i].trailRenderer.sharedMaterial = material;
			}
		}
		_materialCache = material;
	}

	public float EvaluateWidth(float normalizedTime)
	{
		return timeWidth.Evaluate(normalizedTime) * widthScale;
	}

	public Color32 EvaluateColor(float normalizedTime)
	{
		return lifetimeColor.Evaluate(normalizedTime);
	}

	public Color32 EvaluateColor(int trailIndex, int trailPointIndex)
	{
		return lifetimeColor.Evaluate((float)trailPointIndex * 1f / (float)(trails[trailIndex].GetBirthIterator() - 1));
	}

	public void ReuseTrail(int trailIndex, TrailParticleInfo particleInfo)
	{
		trails[trailIndex].DequeueForReuse();
		trails[trailIndex].particleId = particleInfo.particleId;
		trails[trailIndex].ClearTrail();
		if (createFirstPointOnParticleBirth)
		{
			float num = EvaluateWidth(0f);
			trails[trailIndex].SetFirstPoint(particleInfo.position, particleInfo.velocity, num, time, _calculationStartTime);
			if (_hasTrailPointEventListener)
			{
				_eventTrailPoint.Update(trails.Count, 0, particleInfo.position, num, time, _calculationStartTime);
				trailPointEvent(_eventTrailPoint);
			}
		}
	}

	public void CreateTrail(TrailParticleInfo particleInfo)
	{
		if (_parentGameObject == null)
		{
			_parentGameObject = new GameObject("Playground Trails (" + playgroundSystem.name + ")", typeof(PlaygroundTrailParent));
			_parentTransform = _parentGameObject.transform;
			_parentGameObject.GetComponent<PlaygroundTrailParent>().trailsReference = this;
		}
		ParticlePlaygroundTrail particlePlaygroundTrail = new ParticlePlaygroundTrail(maxPoints);
		particlePlaygroundTrail.trailGameObject = new GameObject("Playground Trail " + trails.Count);
		layer = Mathf.Clamp(layer, 0, 32);
		particlePlaygroundTrail.trailGameObject.layer = layer;
		particlePlaygroundTrail.trailTransform = particlePlaygroundTrail.trailGameObject.transform;
		particlePlaygroundTrail.trailTransform.parent = _parentTransform;
		particlePlaygroundTrail.trailRenderer = particlePlaygroundTrail.trailGameObject.AddComponent<MeshRenderer>();
		particlePlaygroundTrail.trailMeshFilter = particlePlaygroundTrail.trailGameObject.AddComponent<MeshFilter>();
		particlePlaygroundTrail.trailMesh = new Mesh();
		particlePlaygroundTrail.trailMesh.MarkDynamic();
		particlePlaygroundTrail.trailMeshFilter.sharedMesh = particlePlaygroundTrail.trailMesh;
		particlePlaygroundTrail.trailRenderer.sharedMaterial = material;
		particlePlaygroundTrail.particleId = particleInfo.particleId;
		if (createFirstPointOnParticleBirth)
		{
			float num = EvaluateWidth(0f);
			particlePlaygroundTrail.SetFirstPoint(particleInfo.position, particleInfo.velocity, num, time, _calculationStartTime);
			if (_hasTrailPointEventListener)
			{
				_eventTrailPoint.Update(trails.Count, 0, particleInfo.position, num, time, _calculationStartTime);
				trailPointEvent(_eventTrailPoint);
			}
		}
		trails.Add(particlePlaygroundTrail);
	}

	public int GetOldestTrailWithParticleId(int particleId)
	{
		for (int i = 0; i < trails.Count; i++)
		{
			if (trails[i].particleId == particleId && !trails[i].IsDead())
			{
				return i;
			}
		}
		return -1;
	}

	public int GetNewestTrailWithParticleId(int particleId)
	{
		for (int num = trails.Count - 1; num >= 0; num--)
		{
			if (trails[num].particleId == particleId && !trails[num].IsDead())
			{
				return num;
			}
		}
		return -1;
	}

	public Transform GetParentTransform()
	{
		return _parentTransform;
	}

	public GameObject GetParentGameObject()
	{
		return _parentGameObject;
	}

	public void StopTrail(int trailNumber)
	{
		if (trailNumber >= 0)
		{
			trails[trailNumber].Die();
		}
	}

	public void StopOldestTrailWithParticleId(int particleId)
	{
		StopTrail(GetOldestTrailWithParticleId(particleId));
	}

	public void StopNewestTrailWithParticleId(int particleId)
	{
		StopTrail(GetNewestTrailWithParticleId(particleId));
	}

	public void ResetTrails()
	{
		DestroyAllTrails();
		if (playgroundSystem != null && gameObject.activeInHierarchy)
		{
			_currentParticleCount = playgroundSystem.particleCount;
			_currentParticleMinLifetime = playgroundSystem.lifetimeMin;
			_currentParticleMaxLifetime = playgroundSystem.lifetime;
			_currentLoopValue = playgroundSystem.loop;
			_localSpace = playgroundSystem.shurikenParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local;
		}
		_isDoneThread = true;
	}

	public void DestroyTrail(int index)
	{
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(trails[index].trailGameObject);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(trails[index].trailGameObject);
		}
		trails.RemoveAt(index);
	}

	public void DestroyAllTrails()
	{
		foreach (ParticlePlaygroundTrail trail in trails)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(trail.trailGameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(trail.trailGameObject);
			}
		}
		if (_parentGameObject != null)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(_parentGameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(_parentGameObject);
			}
		}
		trails.Clear();
		_birthQueue.Clear();
	}

	private void CalculateTrail()
	{
		for (int i = 0; i < trails.Count; i++)
		{
			ParticlePlaygroundTrail particlePlaygroundTrail = trails[i];
			if (particlePlaygroundTrail.CanRemoveTrail())
			{
				continue;
			}
			if (particlePlaygroundTrail.particleId >= 0 && !particlePlaygroundTrail.IsDead())
			{
				if (particlePlaygroundTrail.GetBirthIterator() > 0)
				{
					float num = Vector3.Distance(particlePlaygroundTrail.GetParticlePosition(), particlePlaygroundTrail.GetLastAddedPointPosition());
					if (num > minVertexDistance)
					{
						float pathDeviation = particlePlaygroundTrail.GetPathDeviation();
						if (num > maxVertexDistance || pathDeviation > maxPathDeviation)
						{
							Vector3 position = playgroundSystem.particleCache[particlePlaygroundTrail.particleId].position;
							float width = EvaluateWidth(0f);
							particlePlaygroundTrail.AddPoint(position, width, time, _calculationStartTime);
							if (_hasTrailPointEventListener)
							{
								_eventTrailPoint.Update(i, particlePlaygroundTrail.GetBirthIterator() - 1, position, width, time, _calculationStartTime);
								trailPointEvent(_eventTrailPoint);
							}
						}
					}
				}
				else
				{
					particlePlaygroundTrail.SetFirstPoint(playgroundSystem.particleCache[particlePlaygroundTrail.particleId].position, playgroundSystem.particleCache[particlePlaygroundTrail.particleId].velocity, EvaluateWidth(0f), time, _calculationStartTime);
				}
				particlePlaygroundTrail.SetParticlePosition(playgroundSystem.particleCache[particlePlaygroundTrail.particleId].position);
			}
			for (int j = 0; j < particlePlaygroundTrail.trailPoints.Count; j++)
			{
				TrailPoint trailPoint = particlePlaygroundTrail.trailPoints[j];
				float normalizedLifetime = trailPoint.GetNormalizedLifetime();
				trailPoint.Update(_calculationStartTime, EvaluateWidth(normalizedLifetime));
				if (!particlePlaygroundTrail.IsDead() && j == particlePlaygroundTrail.trailPoints.Count - 1)
				{
					trailPoint.position = particlePlaygroundTrail.GetParticlePosition();
				}
				Vector3 vector = trailPoint.position;
				Vector3 vector2 = ((j >= particlePlaygroundTrail.trailPoints.Count - 1) ? (vector + (vector - particlePlaygroundTrail.trailPoints[j - 1].position)) : particlePlaygroundTrail.trailPoints[j + 1].position);
				Vector3 lhs = Vector3.up;
				switch (renderMode)
				{
				case TrailRenderMode.Vertical:
					lhs = Vector3.forward;
					break;
				case TrailRenderMode.Billboard:
					lhs = (_billboardTransformPosition - vector).normalized;
					break;
				}
				if (_localSpace)
				{
					vector = _localMatrix.MultiplyPoint3x4(vector);
					vector2 = _localMatrix.MultiplyPoint3x4(vector2);
				}
				Vector3 vector3 = ((renderMode == TrailRenderMode.CustomRenderScale) ? customRenderScale : Vector3.Cross(lhs, vector2 - vector).normalized);
				Vector3 vector4 = vector + vector3 * (trailPoint.width * 0.5f);
				Vector3 vector5 = vector - vector3 * (trailPoint.width * 0.5f);
				particlePlaygroundTrail.meshVerticesCache[j * 2] = vector4;
				particlePlaygroundTrail.meshVerticesCache[j * 2 + 1] = vector5;
				float x = ((uvMode != TrailUvMode.Lifetime) ? ((float)j * 1f / (float)(particlePlaygroundTrail.GetBirthIterator() - 1)) : normalizedLifetime);
				ref Vector2 reference = ref particlePlaygroundTrail.meshUvsCache[j * 2];
				reference = new Vector2(x, 0f);
				ref Vector2 reference2 = ref particlePlaygroundTrail.meshUvsCache[j * 2 + 1];
				reference2 = new Vector2(x, 1f);
				if (colorMode == TrailColorMode.Lifetime)
				{
					Color32 color = EvaluateColor(normalizedLifetime);
					color.a = (byte)((float)(int)color.a * pointArrayAlpha.Evaluate((float)j * 1f / (float)(particlePlaygroundTrail.GetBirthIterator() - 1)));
					particlePlaygroundTrail.SetColor(j, color);
				}
				else
				{
					particlePlaygroundTrail.SetColor(j, EvaluateColor(i, j));
				}
			}
		}
	}
}
