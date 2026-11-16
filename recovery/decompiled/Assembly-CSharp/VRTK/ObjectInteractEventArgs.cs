using System;
using UnityEngine;

namespace VRTK;

public struct ObjectInteractEventArgs
{
	[Obsolete("`ObjectInteractEventArgs.controllerIndex` has been replaced with `ObjectInteractEventArgs.controllerReference`. This parameter will be removed in a future version of VRTK.")]
	public uint controllerIndex;

	public VRTK_ControllerReference controllerReference;

	public GameObject target;
}
