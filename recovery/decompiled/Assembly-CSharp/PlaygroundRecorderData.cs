using System;
using System.Collections.Generic;
using ParticlePlayground;
using UnityEngine;

[Serializable]
public class PlaygroundRecorderData : ScriptableObject
{
	[HideInInspector]
	public float version;

	public SerializedFrame[] serializedFrames;

	public void Serialize(List<RecordedFrame> recordedFrames)
	{
		version = PlaygroundC.version;
		serializedFrames = null;
		serializedFrames = new SerializedFrame[recordedFrames.Count];
		for (int i = 0; i < serializedFrames.Length; i++)
		{
			serializedFrames[i] = recordedFrames[i].CloneAsSerializedFrame();
		}
	}

	public void SerializeAsync(List<RecordedFrame> recordedFrames)
	{
		version = PlaygroundC.version;
		PlaygroundC.RunAsync(() =>
		{
			serializedFrames = null;
			serializedFrames = new SerializedFrame[recordedFrames.Count];
			for (int i = 0; i < serializedFrames.Length; i++)
			{
				serializedFrames[i] = recordedFrames[i].CloneAsSerializedFrame();
			}
		});
	}

	public List<RecordedFrame> CloneAsRecordedFrames()
	{
		if (serializedFrames == null)
		{
			return null;
		}
		List<RecordedFrame> list = new List<RecordedFrame>();
		for (int i = 0; i < serializedFrames.Length; i++)
		{
			list.Add(serializedFrames[i].CloneAsRecordedFrame());
		}
		return list;
	}

	public void Clear()
	{
		serializedFrames = null;
	}

	public static PlaygroundRecorderData New()
	{
		return ScriptableObject.CreateInstance<PlaygroundRecorderData>();
	}
}
