using UnityEngine;

namespace VRTK;

public abstract class SDK_BaseBoundaries : SDK_Base
{
	protected Transform cachedPlayArea;

	public abstract void InitBoundaries();

	public abstract Transform GetPlayArea();

	public abstract Vector3[] GetPlayAreaVertices();

	public abstract float GetPlayAreaBorderThickness();

	public abstract bool IsPlayAreaSizeCalibrated();

	public abstract bool GetDrawAtRuntime();

	public abstract void SetDrawAtRuntime(bool value);

	protected Transform GetSDKManagerPlayArea()
	{
		VRTK_SDKManager instance = VRTK_SDKManager.instance;
		if (instance != null && instance.loadedSetup.actualBoundaries != null)
		{
			cachedPlayArea = ((!instance.loadedSetup.actualBoundaries) ? null : instance.loadedSetup.actualBoundaries.transform);
			return cachedPlayArea;
		}
		return null;
	}
}
