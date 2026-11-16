using System.Collections.Generic;
using UnityEngine;

namespace ParticlePlayground;

public class PlaygroundFollow : MonoBehaviour
{
	public PlaygroundParticlesC particles;

	public GameObject referenceObject;

	public float followerLifetime;

	public int cacheSize;

	public bool sendEvents;

	private TrailRenderer referenceTrailRenderer;

	private float trailTime;

	private List<PlaygroundFollower> followers = new List<PlaygroundFollower>();

	private List<PlaygroundFollower> waitingFollowers = new List<PlaygroundFollower>();

	private PlaygroundFollower[] referenceObjectsCache;

	private PlaygroundFollower[] queue = new PlaygroundFollower[0];

	private int cacheIndex;

	private PlaygroundEventC birthEvent;

	private PlaygroundEventC deathEvent;

	private Transform followerParent;

	public event OnPlaygroundFollower followerEventBirth;

	public event OnPlaygroundFollower followerEventDeath;

	private void Start()
	{
		if (referenceObject == null || particles == null)
		{
			return;
		}
		birthEvent = PlaygroundC.CreateEvent(particles);
		birthEvent.broadcastType = EVENTBROADCASTC.EventListeners;
		birthEvent.eventType = EVENTTYPEC.Birth;
		deathEvent = PlaygroundC.CreateEvent(particles);
		deathEvent.broadcastType = EVENTBROADCASTC.EventListeners;
		deathEvent.eventType = EVENTTYPEC.Death;
		birthEvent.particleEvent += OnParticleDidBirth;
		deathEvent.particleEvent += OnParticleDidDie;
		followerParent = new GameObject("Followers").transform;
		followerParent.parent = transform;
		referenceTrailRenderer = referenceObject.GetComponent<TrailRenderer>();
		if (referenceTrailRenderer != null)
		{
			trailTime = referenceTrailRenderer.time;
		}
		int num = ((!(followerLifetime <= 0f)) ? (Mathf.CeilToInt(Mathf.Abs(particles.lifetime - followerLifetime) + (followerLifetime - particles.lifetime)) + 2) : (Mathf.CeilToInt(Mathf.Abs(particles.lifetime - trailTime) + (trailTime - particles.lifetime)) + 2));
		if (particles.lifetime <= 1f)
		{
			num++;
		}
		referenceObjectsCache = new PlaygroundFollower[(cacheSize <= 0) ? (particles.particleCount + Mathf.CeilToInt(particles.particleCount * num)) : cacheSize];
		for (int i = 0; i < referenceObjectsCache.Length; i++)
		{
			GameObject gameObject = Object.Instantiate(referenceObject);
			referenceObjectsCache[i] = new PlaygroundFollower(gameObject.transform, gameObject, gameObject.GetComponent<TrailRenderer>(), 0f, 0);
			referenceObjectsCache[i].transform.parent = followerParent;
			if (referenceObjectsCache[i].trailRenderer != null)
			{
				referenceObjectsCache[i].trailRenderer.time = 0f;
			}
			referenceObjectsCache[i].gameObject.SetActive(value: false);
		}
	}

	private void OnParticleDidBirth(PlaygroundEventParticle particle)
	{
		waitingFollowers.Add(new PlaygroundFollower(null, null, null, (!(followerLifetime <= 0f)) ? followerLifetime : (particle.totalLifetime + trailTime), particle.particleId));
	}

	private void OnParticleDidDie(PlaygroundEventParticle particle)
	{
		int followerWithId = GetFollowerWithId(particle.particleId);
		if (followerWithId >= 0)
		{
			followers[followerWithId].enabled = false;
		}
	}

	private int GetFollowerWithId(int particleId)
	{
		float num = 999f;
		int result = -1;
		for (int i = 0; i < followers.Count; i++)
		{
			if (followers[i].particleId == particleId && followers[i].lifetime < num)
			{
				result = i;
			}
		}
		return result;
	}

	private void Update()
	{
		if (waitingFollowers.Count > 0)
		{
			queue = waitingFollowers.ToArray();
		}
	}

	private void LateUpdate()
	{
		UpdateFollowers();
	}

	private void UpdateFollowers()
	{
		for (int i = 0; i < followers.Count; i++)
		{
			if (followers[i].enabled)
			{
				followers[i].transform.position = particles.particleCache[followers[i].particleId].position;
			}
			followers[i].lifetime -= Time.deltaTime;
			if (followers[i].lifetime <= 0f)
			{
				RemoveFollower(i);
			}
		}
		if (queue.Length > 0 && queue.Length == waitingFollowers.Count)
		{
			int count = waitingFollowers.Count;
			PlaygroundFollower[] array = queue;
			foreach (PlaygroundFollower follower in array)
			{
				AddFollower(follower, followers.Count - 1);
			}
			if (count == waitingFollowers.Count)
			{
				waitingFollowers = new List<PlaygroundFollower>();
			}
			else
			{
				waitingFollowers.RemoveRange(0, count - 1);
			}
			queue = new PlaygroundFollower[0];
		}
	}

	private void AddFollower(PlaygroundFollower follower, int i)
	{
		if (follower != null)
		{
			followers.Add(follower.Clone());
			followers[followers.Count - 1].enabled = true;
			followers[followers.Count - 1].gameObject = referenceObjectsCache[cacheIndex].gameObject;
			followers[followers.Count - 1].gameObject.SetActive(value: true);
			followers[followers.Count - 1].transform = referenceObjectsCache[cacheIndex].transform;
			followers[followers.Count - 1].trailRenderer = referenceObjectsCache[cacheIndex].trailRenderer;
			followers[followers.Count - 1].particleId = follower.particleId;
			followers[followers.Count - 1].transform.position = particles.playgroundCache.position[followers[followers.Count - 1].particleId];
			if (followers[followers.Count - 1].trailRenderer != null)
			{
				followers[followers.Count - 1].trailRenderer.time = trailTime;
			}
			if (sendEvents && followerEventBirth != null)
			{
				followerEventBirth(followers[followers.Count - 1]);
			}
			NextCacheIndex();
		}
	}

	private void RemoveFollower(int i)
	{
		if (sendEvents && followerEventDeath != null)
		{
			followerEventDeath(followers[i]);
		}
		followers[i].enabled = false;
		if (followers[i].trailRenderer != null)
		{
			followers[i].trailRenderer.time = 0f;
		}
		followers[i].gameObject.SetActive(value: false);
		followers.RemoveAt(i);
	}

	private void NextCacheIndex()
	{
		cacheIndex = (cacheIndex + 1) % referenceObjectsCache.Length;
	}

	public PlaygroundFollower GetActiveFollower(int index)
	{
		index = Mathf.Clamp(index, 0, followers.Count);
		return followers[index];
	}

	public PlaygroundFollower GetCachedFollower(int index)
	{
		index = Mathf.Clamp(index, 0, referenceObjectsCache.Length);
		return referenceObjectsCache[index];
	}

	public int GetActiveFollowersCount()
	{
		return followers.Count;
	}

	public int GetCachedFollowersCount()
	{
		return referenceObjectsCache.Length;
	}
}
