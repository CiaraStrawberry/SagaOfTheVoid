using UnityEngine;

namespace DynamicFogAndMist;

public class Demo2 : MonoBehaviour
{
	private static GUIStyle labelStyle;

	private void OnGUI()
	{
		if (labelStyle == null)
		{
			labelStyle = new GUIStyle(GUI.skin.label);
			labelStyle.normal.textColor = Color.black;
		}
		Rect position = new Rect(10f, 10f, Screen.width - 20, 30f);
		GUI.Label(position, "Demo 2 scene: move with WASD. Advance to find the rotating sign which will make the fog disappear.", labelStyle);
	}
}
