namespace VRTK;

[SDK_Description("Fallback", null, null, null, 0)]
public class SDK_FallbackSystem : SDK_BaseSystem
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
