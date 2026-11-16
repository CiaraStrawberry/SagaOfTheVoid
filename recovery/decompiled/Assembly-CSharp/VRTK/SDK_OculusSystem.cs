namespace VRTK;

[SDK_Description("Oculus", "VRTK_DEFINE_SDK_OCULUS", "Oculus", "Standalone", 0)]
public class SDK_OculusSystem : SDK_BaseSystem
{
	public override bool IsDisplayOnDesktop()
	{
		return false;
	}

	public override bool ShouldAppRenderWithLowResources()
	{
		return false;
	}

	public override void ForceInterleavedReprojectionOn(bool force)
	{
	}
}
