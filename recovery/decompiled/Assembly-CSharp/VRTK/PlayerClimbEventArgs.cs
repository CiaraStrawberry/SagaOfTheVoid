using System;
using UnityEngine;

namespace VRTK;

public struct PlayerClimbEventArgs
{
	[Obsolete("`PlayerClimbEventArgs.controllerIndex` has been replaced with `PlayerClimbEventArgs.controllerReference`. This parameter will be removed in a future version of VRTK.")]
	public uint controllerIndex;

	public VRTK_ControllerReference controllerReference;

	public GameObject target;
}
