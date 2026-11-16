using UnityEngine;

namespace Apex.Services;

public static class UnityServices
{
	public static IPhysics physics = new PhysicsWrapper();

	public static ILayerMask layers = new LayerMaskWrapper();

	public static ITime time = new TimeWrapper();

	public static IDebug debug = new DebugWrapper();

	public static Camera mainCamera
	{
		get
		{
			Camera main = Camera.main;
			if (main == null)
			{
				throw new MissingReferenceException("There is no main camera defined, please tag one camera as main.");
			}
			return main;
		}
	}
}
