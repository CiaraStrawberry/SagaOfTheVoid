namespace EasyEditor;

public static class Settings
{
	public enum RenderPosition
	{
		Above,
		Below
	}

	public const int indentation = 1;

	public const bool foldable = false;

	public const RenderPosition commentRenderPosition = RenderPosition.Below;

	public const RenderPosition messageRenderPosition = RenderPosition.Below;

	public const bool inlineUnfolded = false;
}
