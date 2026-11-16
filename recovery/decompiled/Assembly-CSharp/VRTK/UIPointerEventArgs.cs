using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VRTK;

public struct UIPointerEventArgs
{
	[Obsolete("`UIPointerEventArgs.controllerIndex` has been replaced with `UIPointerEventArgs.controllerReference`. This parameter will be removed in a future version of VRTK.")]
	public uint controllerIndex;

	public VRTK_ControllerReference controllerReference;

	public bool isActive;

	public GameObject currentTarget;

	public GameObject previousTarget;

	public RaycastResult raycastResult;
}
