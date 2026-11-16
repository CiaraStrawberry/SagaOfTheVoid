using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine;

namespace Devdog.General;

public class AudioManager : ManagerBase<AudioManager>
{
	private static AudioSource[] _audioSources;

	private static GameObject _audioSourceGameObject;

	private static Queue<AudioClipInfo> _audioQueue = new Queue<AudioClipInfo>();

	protected override void Awake()
	{
		base.Awake();
		StartCoroutine(WaitFramesAndEnable(5));
		enabled = false;
		_audioQueue = new Queue<AudioClipInfo>(ManagerBase<GeneralSettingsManager>.instance.settings.reserveAudioSources);
		CreateAudioSourcePool();
	}

	protected override void Start()
	{
		base.Start();
	}

	private void OnEnable()
	{
	}

	private IEnumerator WaitFramesAndEnable(int frames)
	{
		for (int i = 0; i < frames; i++)
		{
			yield return null;
		}
		enabled = true;
	}

	private void CreateAudioSourcePool()
	{
		_audioSources = new AudioSource[ManagerBase<GeneralSettingsManager>.instance.settings.reserveAudioSources];
		_audioSourceGameObject = new GameObject("_AudioSources");
		_audioSourceGameObject.transform.SetParent(transform);
		_audioSourceGameObject.transform.localPosition = Vector3.zero;
		for (int i = 0; i < _audioSources.Length; i++)
		{
			_audioSources[i] = _audioSourceGameObject.AddComponent<AudioSource>();
			_audioSources[i].outputAudioMixerGroup = ManagerBase<GeneralSettingsManager>.instance.settings.audioMixerGroup;
		}
	}

	protected void Update()
	{
		if (_audioQueue.Count > 0)
		{
			AudioSource nextAudioSource = GetNextAudioSource();
			AudioClipInfo info = _audioQueue.Dequeue();
			nextAudioSource.Play(info);
		}
	}

	private static AudioSource GetNextAudioSource()
	{
		AudioSource[] audioSources = _audioSources;
		foreach (AudioSource audioSource in audioSources)
		{
			if (!audioSource.isPlaying)
			{
				return audioSource;
			}
		}
		DevdogLogger.LogWarning("All sources taken, can't play audio clip...");
		return null;
	}

	public static void AudioPlayOneShot(AudioClipInfo clip)
	{
		if (clip != null && !(clip.audioClip == null))
		{
			if (ManagerBase<AudioManager>.instance == null)
			{
				DevdogLogger.LogWarning("AudioManager not found, yet trying to play an audio clip....");
			}
			if (!_audioQueue.Any((AudioClipInfo o) => o.audioClip == clip.audioClip))
			{
				_audioQueue.Enqueue(clip);
			}
		}
	}
}
