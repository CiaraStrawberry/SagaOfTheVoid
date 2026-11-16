using System;
using UnityEngine;

namespace VRTK;

public struct DestinationMarkerEventArgs
{
	public float distance;

	public Transform target;

	public RaycastHit raycastHit;

	public Vector3 destinationPosition;

	public Quaternion? destinationRotation;

	public bool forceDestinationPosition;

	public bool enableTeleport;

	[Obsolete("`DestinationMarkerEventArgs.controllerIndex` has been replaced with `DestinationMarkerEventArgs.controllerReference`. This parameter will be removed in a future version of VRTK.")]
	public uint controllerIndex;

	public VRTK_ControllerReference controllerReference;
}
