using System.Collections;
using ParticlePlayground;

namespace UnityEngine;

public class PlaygroundRecordingMultiExample : MonoBehaviour
{
	public PlaygroundMultiRecorder playgroundMultiRecorder;

	public float keyframeInterval = 0.1f;

	public bool recordFromStart = true;

	private Rect guiArea;

	private bool hoveringScrubSlider;

	private bool isScrubbing;

	private float playbackSliderValue;

	private IEnumerator Start()
	{
		guiArea = new Rect(10f, 10f, (float)Screen.width * 0.5f, 130f);
		if (!recordFromStart)
		{
			yield break;
		}
		for (int i = 0; i < playgroundMultiRecorder.playgroundRecorders.Length; i++)
		{
			while (!playgroundMultiRecorder.playgroundRecorders[i].playgroundSystem.IsReady())
			{
				yield return null;
			}
		}
		playgroundMultiRecorder.StartRecording(keyframeInterval);
	}

	private void Update()
	{
		if (hoveringScrubSlider && Input.GetMouseButton(0))
		{
			isScrubbing = true;
		}
		else if (Input.GetMouseButtonUp(0))
		{
			isScrubbing = false;
		}
		if (isScrubbing)
		{
			playgroundMultiRecorder.Scrub(playbackSliderValue);
		}
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(guiArea, "Playground Multi Recorder");
		GUILayout.Space(18f);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(playgroundMultiRecorder.playgroundRecorders[0].IsRecording() ? "Stop Recording" : "Record"))
		{
			if (!playgroundMultiRecorder.playgroundRecorders[0].IsRecording())
			{
				playgroundMultiRecorder.StartRecording(keyframeInterval);
			}
			else
			{
				playgroundMultiRecorder.StopRecording();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Time (" + ((float)playgroundMultiRecorder.playgroundRecorders[0].FrameCount() * keyframeInterval * playbackSliderValue).ToString("F1") + ")", GUILayout.MaxWidth(40f));
		playbackSliderValue = GUILayout.HorizontalSlider(playbackSliderValue, 0f, 1f);
		if (playgroundMultiRecorder.playgroundRecorders[0].IsRecording())
		{
			playbackSliderValue = 1f;
		}
		hoveringScrubSlider = Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
