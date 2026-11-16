using System;
using UnityEngine;

namespace VRTK;

public struct HeadsetControllerAwareEventArgs
{
	public RaycastHit raycastHit;

	[Obsolete("`HeadsetControllerAwareEventArgs.controllerIndex` has been replaced with `HeadsetControllerAwareEventArgs.controllerReference`. This parameter will be removed in a future version of VRTK.")]
	public uint controllerIndex;

	public VRTK_ControllerReference controllerReference;
}
