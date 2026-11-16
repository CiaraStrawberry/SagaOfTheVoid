using UnityEngine;

namespace VRTK;

public abstract class SDK_Base : ScriptableObject
{
	public virtual void OnBeforeSetupLoad(VRTK_SDKSetup setup)
	{
	}

	public virtual void OnAfterSetupLoad(VRTK_SDKSetup setup)
	{
	}

	public virtual void OnBeforeSetupUnload(VRTK_SDKSetup setup)
	{
	}

	public virtual void OnAfterSetupUnload(VRTK_SDKSetup setup)
	{
	}
}
