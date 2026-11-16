using System.Collections;
using ParticlePlayground;

namespace UnityEngine;

public class PlaygroundRecordingExample : MonoBehaviour
{
	public PlaygroundRecorder playgroundRecorder;

	public float keyframeInterval = 0.1f;

	public bool recordFromStart = true;

	private Rect guiArea;

	private bool hoveringScrubSlider;

	private bool isScrubbing;

	private float playbackSliderValue;

	private IEnumerator Start()
	{
		guiArea = new Rect(10f, 10f, (float)Screen.width * 0.5f, 130f);
		if (recordFromStart)
		{
			while (!playgroundRecorder.playgroundSystem.IsReady())
			{
				yield return null;
			}
			playgroundRecorder.StartRecording(keyframeInterval);
		}
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
			playgroundRecorder.Scrub(playbackSliderValue);
		}
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(guiArea, "Playground Recorder");
		GUILayout.Space(18f);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(playgroundRecorder.IsRecording() ? "Stop Recording" : "Record"))
		{
			if (!playgroundRecorder.IsRecording())
			{
				playgroundRecorder.StartRecording(keyframeInterval);
			}
			else
			{
				playgroundRecorder.StopRecording();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Time (" + ((float)playgroundRecorder.FrameCount() * keyframeInterval * playbackSliderValue).ToString("F1") + ")", GUILayout.MaxWidth(40f));
		playbackSliderValue = GUILayout.HorizontalSlider(playbackSliderValue, 0f, 1f);
		if (playgroundRecorder.IsRecording())
		{
			playbackSliderValue = 1f;
		}
		hoveringScrubSlider = Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
