using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParticlePlayground;

[ExecuteInEditMode]
public class PlaygroundRecorder : MonoBehaviour
{
	public bool multithreading = true;

	public float keyframeInterval = 0.1f;

	public float playbackSpeed = 1f;

	public float playHead;

	public bool loopPlayback = true;

	public bool fadeIn = true;

	public bool sizeIn = true;

	public bool skipInterpolationOnEndFrames = true;

	public bool localSpaceOnPlayback = true;

	[HideInInspector]
	public PlaygroundParticlesC playgroundSystem;

	[HideInInspector]
	public PlaygroundRecorderData recorderData;

	[NonSerialized]
	public List<RecordedFrame> recordedFrames;

	private bool _inPlayback;

	private bool _isReplaying;

	private bool _isRecording;

	private bool _hasRecorderData;

	private bool _hasPlaygroundSystem;

	private bool _hasEditedRecordData;

	private float _recordingStarted;

	private float _lastRecordedFrameTime;

	private int _recordingStartFrame;

	private int _recordingEndFrame;

	private ParticleSystem.Particle[] _playbackParticles;

	private ParticleSystemSimulationSpace _previousSimulationSpace;

	private object locker = new object();

	private void OnEnable()
	{
		if (playgroundSystem == null)
		{
			playgroundSystem = GetComponent<PlaygroundParticlesC>();
		}
		if (Application.isPlaying && recordedFrames == null)
		{
			if (recorderData != null)
			{
				_hasRecorderData = true;
				if (multithreading)
				{
					LoadAsync();
				}
				else
				{
					Load();
				}
			}
			else
			{
				recordedFrames = new List<RecordedFrame>();
				_hasEditedRecordData = true;
			}
		}
		_hasPlaygroundSystem = playgroundSystem != null;
	}

	private void OnDisable()
	{
		if (_hasPlaygroundSystem)
		{
			playgroundSystem.inPlayback = false;
			playgroundSystem.calculate = false;
		}
	}

	public bool HasRecordedFrames()
	{
		return recordedFrames != null && recordedFrames.Count > 0;
	}

	public bool IsRecording()
	{
		return _isRecording;
	}

	public bool IsReplaying()
	{
		return _isReplaying;
	}

	public bool IsInPlayback()
	{
		return _inPlayback;
	}

	public int FrameCount()
	{
		if (recordedFrames == null)
		{
			return -1;
		}
		return (recordedFrames.Count - 1 >= 0) ? (recordedFrames.Count - 1) : 0;
	}

	public int GetFrameAtTime(float normalizedTime)
	{
		if (recordedFrames == null || recordedFrames.Count == 0)
		{
			return 0;
		}
		return Mathf.Clamp(Mathf.FloorToInt((float)(recordedFrames.Count - 1) * Mathf.Clamp01(normalizedTime)), 0, recordedFrames.Count - 1);
	}

	public float GetFloatingFrameAtTime(float normalizedTime)
	{
		if (recordedFrames == null || recordedFrames.Count < 1)
		{
			return 0f;
		}
		return (float)(recordedFrames.Count - 1) * Mathf.Clamp01(normalizedTime);
	}

	public float GetTimeAtFrame(int frame)
	{
		frame = Mathf.Clamp(frame, 0, recordedFrames.Count);
		return (float)frame * 1f / (float)(recordedFrames.Count - 1);
	}

	public float GetKeyframeIntervalAtFrame(int frame)
	{
		if (recordedFrames == null || frame < 0 || frame >= recordedFrames.Count)
		{
			return 0f;
		}
		return recordedFrames[frame].keyframeInterval;
	}

	public float RecordingStarted()
	{
		return _recordingStarted;
	}

	public float GetLastRecordedFrameTime()
	{
		return _lastRecordedFrameTime;
	}

	public void StartRecording()
	{
		if (!(playgroundSystem == null))
		{
			playgroundSystem.inPlayback = false;
			if (_isRecording)
			{
				StopRecording();
			}
			_isRecording = true;
			_isReplaying = false;
			_inPlayback = false;
			_recordingStarted = Time.realtimeSinceStartup;
			if (Application.isPlaying)
			{
				StartCoroutine(RecordInternal(keyframeInterval));
			}
		}
	}

	public void StartRecording(float keyframeInterval)
	{
		this.keyframeInterval = keyframeInterval;
		StartRecording();
	}

	public void StartRecording(float recordingLength, float keyframeInterval)
	{
		if (!(playgroundSystem == null))
		{
			StartCoroutine(StartRecordingInternal(recordingLength, keyframeInterval));
		}
	}

	public void RecordOneFrame()
	{
		RecFrame();
	}

	public void InsertOneFrame(int frame, FrameType frameType = FrameType.Middle)
	{
		InsertRecFrame(frame, frameType);
	}

	public void StopRecording()
	{
		if (_isRecording)
		{
			_isRecording = false;
			_recordingEndFrame = recordedFrames.Count - 1;
			CancelInvoke("RecFrame");
			if (recordedFrames.Count > 0)
			{
				_recordingStartFrame = Mathf.Clamp(_recordingStartFrame, 0, recordedFrames.Count);
				_recordingEndFrame = Mathf.Clamp(_recordingEndFrame, 0, recordedFrames.Count);
				recordedFrames[_recordingStartFrame].frameType = FrameType.Start;
				recordedFrames[_recordingEndFrame].frameType = FrameType.End;
			}
		}
	}

	public void ClearRecording()
	{
		_isReplaying = false;
		StopRecording();
		StopPlayback();
		recordedFrames = null;
		recordedFrames = new List<RecordedFrame>();
		if (recorderData != null)
		{
			recorderData.Clear();
		}
		_hasEditedRecordData = true;
		_recordingStartFrame = 0;
		_recordingEndFrame = 0;
		if (_hasPlaygroundSystem)
		{
			playgroundSystem.inPlayback = false;
			playgroundSystem.calculate = false;
		}
	}

	public void Play()
	{
		Play(playHead, playbackSpeed, loopPlayback);
	}

	public void Play(float speed)
	{
		playbackSpeed = speed;
		Play(playHead, speed, loopPlayback);
	}

	public void Play(float fromNormalizedTime, float speed, bool repeat)
	{
		if (_hasPlaygroundSystem)
		{
			if (!_isReplaying && localSpaceOnPlayback)
			{
				ParticleSystem.MainModule main = playgroundSystem.shurikenParticleSystem.main;
				_previousSimulationSpace = main.simulationSpace;
				main.simulationSpace = ParticleSystemSimulationSpace.Local;
			}
			playgroundSystem.inPlayback = true;
			playbackSpeed = speed;
			loopPlayback = repeat;
			playHead = fromNormalizedTime;
			_isReplaying = true;
			StopRecording();
			StartPlayback();
			if (playHead >= 1f)
			{
				playHead = 0f;
			}
			if (Application.isPlaying)
			{
				StartCoroutine(PlayRecordedFrames(playHead));
			}
		}
	}

	public void Pause()
	{
		if (_hasPlaygroundSystem)
		{
			playgroundSystem.inPlayback = true;
			_isReplaying = false;
			if (_isRecording)
			{
				StopRecording();
			}
			StartPlayback();
		}
	}

	public void Stop()
	{
		if (_hasPlaygroundSystem)
		{
			if (_isReplaying && localSpaceOnPlayback)
			{
				ParticleSystem.MainModule main = playgroundSystem.shurikenParticleSystem.main;
				main.simulationSpace = _previousSimulationSpace;
			}
			playgroundSystem.inPlayback = false;
			_isReplaying = false;
			if (_isRecording)
			{
				StopRecording();
			}
			StopPlayback();
		}
	}

	public void StopAndSerialize()
	{
		if (_hasPlaygroundSystem)
		{
			playgroundSystem.inPlayback = false;
			_isReplaying = false;
			if (_isRecording)
			{
				StopRecording();
				Serialize();
			}
			StopPlayback();
		}
	}

	public void Serialize()
	{
		if (_hasRecorderData && _hasEditedRecordData)
		{
			if (multithreading)
			{
				recorderData.SerializeAsync(recordedFrames);
			}
			else
			{
				recorderData.Serialize(recordedFrames);
			}
			_hasEditedRecordData = false;
		}
	}

	public void Load()
	{
		if (recorderData != null)
		{
			recordedFrames = recorderData.CloneAsRecordedFrames();
			_hasEditedRecordData = true;
		}
		else
		{
			Debug.Log("No Playground Recorder Data to load from!", gameObject);
		}
	}

	public void LoadAsync()
	{
		if (recorderData == null)
		{
			Debug.Log("No Playground Recorder Data to load from!", gameObject);
			return;
		}
		PlaygroundC.RunAsync(() =>
		{
			recordedFrames = recorderData.CloneAsRecordedFrames();
			_hasEditedRecordData = true;
		});
	}

	public void Scrub(float normalizedTime)
	{
		if (!HasRecordedFrames())
		{
			return;
		}
		if (_isRecording)
		{
			StopRecording();
		}
		if (!_inPlayback)
		{
			StartPlayback();
		}
		if (multithreading)
		{
			PlaygroundC.RunAsync(() =>
			{
				lock (locker)
				{
					ScrubInternal(normalizedTime);
				}
			});
		}
		else
		{
			ScrubInternal(normalizedTime);
		}
	}

	public bool Trim(float leftTime, float rightTime)
	{
		if (recordedFrames.Count == 0)
		{
			return false;
		}
		int frameAtTime = GetFrameAtTime(leftTime);
		int frameAtTime2 = GetFrameAtTime(rightTime);
		bool flag = false;
		if (frameAtTime > 0)
		{
			recordedFrames.RemoveRange(0, frameAtTime);
			flag = true;
		}
		if (frameAtTime2 < recordedFrames.Count - 1)
		{
			int num = frameAtTime2 - frameAtTime;
			recordedFrames.RemoveRange(num, recordedFrames.Count - num);
			flag = true;
		}
		if (flag)
		{
			_hasEditedRecordData = true;
			Serialize();
		}
		return flag;
	}

	public void TrimInner(float leftTime, float rightTime)
	{
		if (recordedFrames.Count != 0)
		{
			int frameAtTime = GetFrameAtTime(leftTime);
			int frameAtTime2 = GetFrameAtTime(rightTime);
			recordedFrames.RemoveRange(frameAtTime, frameAtTime2 - frameAtTime);
			_hasEditedRecordData = true;
			Serialize();
		}
	}

	public void SetParticleSystemAsCurrentPlayback()
	{
		SetParticleSystemAsRecording(playHead);
	}

	public void SetParticleSystemAsRecording(float normalizedTime)
	{
		if (playgroundSystem == null || _playbackParticles == null)
		{
			return;
		}
		if (multithreading)
		{
			PlaygroundC.RunAsync(() =>
			{
				lock (locker)
				{
					SetParticleSystemAsRecordingInternal(normalizedTime);
				}
			});
		}
		else
		{
			SetParticleSystemAsRecordingInternal(normalizedTime);
		}
	}

	private void RecFrame()
	{
		if (!_isRecording)
		{
			return;
		}
		if (!playgroundSystem.calculate)
		{
			CancelInvoke("RecFrame");
			return;
		}
		_lastRecordedFrameTime = Time.realtimeSinceStartup;
		if (playgroundSystem.particleCache != null)
		{
			if (recordedFrames == null)
			{
				recordedFrames = new List<RecordedFrame>();
			}
			recordedFrames.Add(new RecordedFrame(playgroundSystem, keyframeInterval));
			_hasEditedRecordData = true;
		}
	}

	private void InsertRecFrame(int frame, FrameType frameType)
	{
		if (playgroundSystem.particleCache != null)
		{
			if (recordedFrames == null)
			{
				recordedFrames = new List<RecordedFrame>();
			}
			recordedFrames.Insert(frame, new RecordedFrame(playgroundSystem, keyframeInterval));
			recordedFrames[frame].frameType = frameType;
			_hasEditedRecordData = true;
		}
	}

	private void StartPlayback()
	{
		if (!_inPlayback && recordedFrames != null && recordedFrames.Count > 0 && playgroundSystem != null)
		{
			_inPlayback = true;
			playgroundSystem.calculate = false;
			playgroundSystem.inPlayback = true;
			if (recordedFrames[0].particles != null)
			{
				_playbackParticles = recordedFrames[0].CloneAsParticles();
			}
			StartCoroutine(Playback());
		}
	}

	private void StopPlayback()
	{
		_inPlayback = false;
		if (_hasPlaygroundSystem)
		{
			playgroundSystem.calculate = true;
		}
	}

	private void ScrubInternal(float normalizedTime)
	{
		normalizedTime = Mathf.Clamp01(normalizedTime);
		int frameAtTime = GetFrameAtTime(normalizedTime);
		if (skipInterpolationOnEndFrames && normalizedTime < 1f && recordedFrames[frameAtTime].frameType == FrameType.End)
		{
			normalizedTime = Mathf.Clamp01(normalizedTime + (GetTimeAtFrame(frameAtTime + 1) - normalizedTime));
			playHead = normalizedTime;
			frameAtTime = GetFrameAtTime(normalizedTime);
		}
		int num = Mathf.Clamp(frameAtTime + 1, 0, recordedFrames.Count);
		int index = Mathf.Clamp(num + 1, 0, recordedFrames.Count);
		if (_playbackParticles == null || _playbackParticles.Length != recordedFrames[frameAtTime].particles.Length)
		{
			if (recordedFrames[frameAtTime].particles == null)
			{
				return;
			}
			_playbackParticles = recordedFrames[frameAtTime].CloneAsParticles();
		}
		if (frameAtTime >= recordedFrames.Count - 1 || (normalizedTime == 0f && !loopPlayback))
		{
			return;
		}
		float num2 = 1f - ((float)num - GetFloatingFrameAtTime(normalizedTime));
		for (int i = 0; i < _playbackParticles.Length; i++)
		{
			if (recordedFrames[frameAtTime].particles[i].lifetime < recordedFrames[num].particles[i].lifetime)
			{
				Color32 color = ((!fadeIn) ? default(Color32) : new Color32(recordedFrames[frameAtTime].particles[i].color.r, recordedFrames[frameAtTime].particles[i].color.g, recordedFrames[frameAtTime].particles[i].color.b, 0));
				_playbackParticles[i].position = Vector3.Lerp(recordedFrames[frameAtTime].particles[i].sourcePosition, recordedFrames[num].particles[i].position, num2);
				_playbackParticles[i].startSize = Mathf.Lerp(sizeIn ? 0f : recordedFrames[frameAtTime].particles[i].startingSize, recordedFrames[num].particles[i].size, num2);
				_playbackParticles[i].startColor = Color.Lerp(fadeIn ? color : recordedFrames[frameAtTime].particles[i].color, recordedFrames[num].particles[i].color, num2);
				_playbackParticles[i].rotation = Mathf.Lerp(recordedFrames[num].particles[i].rotation - recordedFrames[index].particles[i].rotation * num2, recordedFrames[num].particles[i].rotation, num2);
			}
			else
			{
				_playbackParticles[i].position = Vector3.Lerp(recordedFrames[frameAtTime].particles[i].position, recordedFrames[num].particles[i].position, num2);
				_playbackParticles[i].startSize = Mathf.Lerp(recordedFrames[frameAtTime].particles[i].size, recordedFrames[num].particles[i].size, num2);
				_playbackParticles[i].startColor = Color.Lerp(recordedFrames[frameAtTime].particles[i].color, recordedFrames[num].particles[i].color, num2);
				_playbackParticles[i].rotation = Mathf.Lerp(recordedFrames[frameAtTime].particles[i].rotation, recordedFrames[num].particles[i].rotation, num2);
				_playbackParticles[i].remainingLifetime = Mathf.Lerp(recordedFrames[frameAtTime].particles[i].lifetime, recordedFrames[num].particles[i].lifetime, num2);
			}
		}
	}

	private void SetParticleSystemAsRecordingInternal(float normalizedTime)
	{
		normalizedTime = Mathf.Clamp01(normalizedTime);
		int frameAtTime = GetFrameAtTime(normalizedTime);
		int num = Mathf.Clamp(frameAtTime + 1, 0, recordedFrames.Count - 1);
		float t = 1f - ((float)num - GetFloatingFrameAtTime(normalizedTime));
		float timeStamp = recordedFrames[frameAtTime].timeStamp;
		float globalTime = PlaygroundC.globalTime;
		int num2 = playgroundSystem.playgroundCache.position.Length;
		for (int i = 0; i < _playbackParticles.Length; i++)
		{
			ref Vector3 reference = ref playgroundSystem.playgroundCache.position[i % num2];
			reference = Vector3.Lerp(recordedFrames[frameAtTime].particles[i].position, recordedFrames[num].particles[i].position, t);
			ref Vector3 reference2 = ref playgroundSystem.playgroundCache.velocity[i % num2];
			reference2 = Vector3.Lerp(recordedFrames[frameAtTime].particles[i].velocity, recordedFrames[num].particles[i].velocity, t);
			playgroundSystem.playgroundCache.size[i % num2] = Mathf.Lerp(recordedFrames[frameAtTime].particles[i].size, recordedFrames[num].particles[i].size, t);
			ref Color32 reference3 = ref playgroundSystem.playgroundCache.color[i % num2];
			reference3 = Color.Lerp(recordedFrames[frameAtTime].particles[i].color, recordedFrames[num].particles[i].color, t);
			playgroundSystem.playgroundCache.rotation[i % num2] = Mathf.Lerp(recordedFrames[frameAtTime].particles[i].rotation, recordedFrames[num].particles[i].rotation, t);
			playgroundSystem.playgroundCache.life[i % num2] = Mathf.Lerp(recordedFrames[frameAtTime].particles[i].playgroundLife, recordedFrames[num].particles[i].playgroundLife, t);
			playgroundSystem.playgroundCache.lifetimeSubtraction[i % num2] = Mathf.Lerp(recordedFrames[frameAtTime].particles[i].playgroundLifetimeSubtraction, recordedFrames[num].particles[i].playgroundLifetimeSubtraction, t);
			playgroundSystem.playgroundCache.birth[i % num2] = globalTime + (recordedFrames[frameAtTime].particles[i].playgroundStartLifetime - timeStamp);
			playgroundSystem.playgroundCache.death[i % num2] = globalTime + (recordedFrames[frameAtTime].particles[i].playgroundEndLifetime - timeStamp);
		}
		playgroundSystem.localTime = globalTime;
		playgroundSystem.LastTimeUpdated = globalTime;
		playgroundSystem.LocalDeltaTime = 0.001f;
		playgroundSystem.cameFromNonCalculatedFrame = false;
		playgroundSystem.cameFromNonEmissionFrame = false;
		playgroundSystem.loopExceeded = false;
		playgroundSystem.loopExceededOnParticle = -1;
		playgroundSystem.hasActiveParticles = true;
		StopAndSerialize();
	}

	private IEnumerator StartRecordingInternal(float recordingLength, float keyframeInterval)
	{
		_isReplaying = false;
		_inPlayback = false;
		_isRecording = true;
		playgroundSystem.inPlayback = false;
		StartCoroutine(RecordInternal(keyframeInterval));
		yield return new WaitForSeconds(recordingLength);
		StopRecording();
	}

	private IEnumerator RecordInternal(float keyframeInterval)
	{
		_recordingStartFrame = ((recordedFrames.Count > 0) ? (recordedFrames.Count - 1) : 0);
		while (_isRecording)
		{
			RecFrame();
			yield return new WaitForSeconds(keyframeInterval);
		}
	}

	private IEnumerator Playback()
	{
		while (_inPlayback)
		{
			if (_playbackParticles != null)
			{
				playgroundSystem.shurikenParticleSystem.SetParticles(_playbackParticles, _playbackParticles.Length);
			}
			yield return null;
		}
	}

	private IEnumerator PlayRecordedFrames(float fromNormalizedTime)
	{
		float t = fromNormalizedTime;
		while (_isReplaying)
		{
			t += playbackSpeed / ((float)FrameCount() * keyframeInterval) * Time.deltaTime;
			if (t > 1f)
			{
				t = 1f;
			}
			else if (t < 0f)
			{
				t = 0f;
			}
			Scrub(t);
			if (t == 1f)
			{
				if (loopPlayback)
				{
					t = 0f;
				}
				else
				{
					_isReplaying = false;
				}
			}
			else if (t == 0f)
			{
				if (loopPlayback)
				{
					t = 1f;
				}
				else
				{
					_isReplaying = false;
				}
			}
			playHead = t;
			yield return null;
		}
	}
}
