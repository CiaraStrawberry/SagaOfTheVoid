namespace DynamicFogAndMist;

internal static class FOG_TYPE_Ext
{
	public static bool isPlus(this FOG_TYPE fogType)
	{
		return fogType == FOG_TYPE.DesktopFogPlusWithSkyHaze || fogType == FOG_TYPE.MobileFogSimple;
	}
}
