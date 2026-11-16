using System;
using UnityEngine;

namespace VRTK;

public struct ControllerInteractionEventArgs
{
	[Obsolete("`ControllerInteractionEventArgs.controllerIndex` has been replaced with `ControllerInteractionEventArgs.controllerReference`. This parameter will be removed in a future version of VRTK.")]
	public uint controllerIndex;

	public VRTK_ControllerReference controllerReference;

	public float buttonPressure;

	public Vector2 touchpadAxis;

	public float touchpadAngle;
}
