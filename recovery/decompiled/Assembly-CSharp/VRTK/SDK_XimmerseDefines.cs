namespace VRTK;

public static class SDK_XimmerseDefines
{
	public const string ScriptingDefineSymbol = "VRTK_DEFINE_SDK_XIMMERSE";

	[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_SDK_XIMMERSE", "Standalone")]
	[SDK_ScriptingDefineSymbolPredicate("VRTK_DEFINE_SDK_XIMMERSE", "Android")]
	private static bool IsXimmerseAvailable()
	{
		return typeof(SDK_XimmerseDefines).Assembly.GetType("Ximmerse.InputSystem.XDevicePlugin") != null;
	}
}
