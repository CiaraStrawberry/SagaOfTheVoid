using ParticlePlayground;
using UnityEngine;

public class PlaygroundBrushPresetC : MonoBehaviour
{
	public string presetName = "Brush";

	public Texture2D texture;

	public float scale = 1f;

	public BRUSHDETAILC detail;

	public float distance = 10000f;

	public float spacing = 0.1f;

	public bool exceedMaxStopsPaint;
}
