using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Sine Starfield")]
public class SgtSineStarfield : MonoBehaviour
{
	[Tooltip("The starfield we will turn into a sine wave")]
	public SgtCustomStarfield Starfield;

	[Tooltip("The base angle of the sine wave")]
	public float Angle;

	[Tooltip("The amount of degrees in the sine wave each star will appear at")]
	public float AngleStep = 5f;

	[Tooltip("The amount Angle changes per second")]
	public float AnglePerSecond = 5f;

	[Tooltip("The base position of the sine wave")]
	public Vector3 Position;

	[Tooltip("The amount of translation between each star")]
	public Vector3 PositionStep = Vector3.forward;

	[Tooltip("The maximum amplitude of the sine wave")]
	public Vector3 Amplitude = Vector3.up;

	protected virtual void Update()
	{
		if (Application.isPlaying)
		{
			Angle += AnglePerSecond * Time.deltaTime;
		}
		if (!(Starfield != null))
		{
			return;
		}
		List<SgtPointStar> stars = Starfield.Stars;
		if (stars == null)
		{
			return;
		}
		float num = Angle;
		Vector3 position = Position;
		for (int i = 0; i < stars.Count; i++)
		{
			SgtPointStar sgtPointStar = stars[i];
			if (sgtPointStar != null)
			{
				sgtPointStar.Position = position + Amplitude * Mathf.Sin(num * ((float)Math.PI / 180f));
				sgtPointStar.Radius = 0.05f;
				sgtPointStar.Color = Color.white;
				num += AngleStep;
				position += PositionStep;
			}
		}
		Starfield.UpdateMeshesAndModels();
	}
}
