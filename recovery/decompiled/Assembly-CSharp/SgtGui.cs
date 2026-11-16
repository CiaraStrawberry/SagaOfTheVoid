using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("Space Graphics Toolkit/SGT GUI")]
public class SgtGui : MonoBehaviour
{
	[Tooltip("The text that appears at the top of the screen")]
	[Multiline]
	public string Header;

	[Tooltip("The text that appears at the bottom of the screen")]
	[Multiline]
	public string Footer;

	private float counter;

	private int frames;

	private float fps;

	private static GUIStyle whiteStyle;

	private static GUIStyle blackStyle;

	protected virtual void Update()
	{
		if (Application.isPlaying)
		{
			counter += Time.deltaTime;
			frames++;
			if (counter >= 1f)
			{
				fps = (float)frames / counter;
				counter = 0f;
				frames = 0;
			}
		}
	}

	protected virtual void OnGUI()
	{
		Rect position = new Rect(5f, 50f, 100f, 50f);
		Rect position2 = new Rect(5f, 105f, 100f, 50f);
		Rect position3 = new Rect(5f, 160f, 100f, 50f);
		if (GUI.Button(position, "Reload"))
		{
			LoadLevel(GetCurrentLevel());
		}
		if (GUI.Button(position2, "Prev"))
		{
			int num = GetCurrentLevel() - 1;
			if (num < 0)
			{
				num = GetLevelCount() - 1;
			}
			LoadLevel(num);
		}
		if (GUI.Button(position3, "Next"))
		{
			int num2 = GetCurrentLevel() + 1;
			if (num2 >= GetLevelCount())
			{
				num2 = 0;
			}
			LoadLevel(num2);
		}
		if (fps > 0f)
		{
			DrawText("FPS: " + fps.ToString("0"), TextAnchor.UpperLeft);
		}
		if (!string.IsNullOrEmpty(Header))
		{
			DrawText(Header, TextAnchor.UpperCenter, 150);
		}
		if (!string.IsNullOrEmpty(Footer))
		{
			DrawText(Footer, TextAnchor.LowerCenter, 150);
		}
	}

	private static int GetCurrentLevel()
	{
		return SceneManager.GetActiveScene().buildIndex;
	}

	private static int GetLevelCount()
	{
		return SceneManager.sceneCountInBuildSettings;
	}

	private static void LoadLevel(int index)
	{
		SceneManager.LoadScene(index);
	}

	private static void DrawText(string text, TextAnchor anchor, int offsetX = 15, int offsetY = 15)
	{
		if (!string.IsNullOrEmpty(text))
		{
			if (whiteStyle == null || blackStyle == null)
			{
				whiteStyle = new GUIStyle();
				whiteStyle.fontSize = 20;
				whiteStyle.fontStyle = FontStyle.Bold;
				whiteStyle.wordWrap = true;
				whiteStyle.normal = new GUIStyleState();
				whiteStyle.normal.textColor = Color.white;
				blackStyle = new GUIStyle();
				blackStyle.fontSize = 20;
				blackStyle.fontStyle = FontStyle.Bold;
				blackStyle.wordWrap = true;
				blackStyle.normal = new GUIStyleState();
				blackStyle.normal.textColor = Color.black;
			}
			whiteStyle.alignment = anchor;
			blackStyle.alignment = anchor;
			float width = Screen.width;
			float height = Screen.height;
			Rect position = new Rect(0f, 0f, width, height);
			position.xMin += offsetX;
			position.xMax -= offsetX;
			position.yMin += offsetY;
			position.yMax -= offsetY;
			position.x++;
			GUI.Label(position, text, blackStyle);
			position.x -= 2f;
			GUI.Label(position, text, blackStyle);
			position.x++;
			position.y++;
			GUI.Label(position, text, blackStyle);
			position.y -= 2f;
			GUI.Label(position, text, blackStyle);
			position.y++;
			GUI.Label(position, text, whiteStyle);
		}
	}
}
