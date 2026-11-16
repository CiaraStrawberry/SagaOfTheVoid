namespace VRTK;

[SDK_Description("Simulator", null, null, "Standalone", 0)]
public class SDK_SimSystem : SDK_BaseSystem
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
