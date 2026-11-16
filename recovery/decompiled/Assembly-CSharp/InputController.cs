using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
	public aInput InputImplementation;

	public Action<float> OnScaleStart { get; set; }

	public Action<float, float> OnScale { get; set; }

	public Action<float> OnScaleEnd { get; set; }

	public Action<Vector3> OnStartPan { get; set; }

	public Action<Vector3> OnPan { get; set; }

	public Action<Vector3> OnEndPan { get; set; }

	public GameObject GetLeftPaddle => InputImplementation.GetLeftPaddle();

	public GameObject GetRightPaddle => InputImplementation.GetrightPaddle();
}
