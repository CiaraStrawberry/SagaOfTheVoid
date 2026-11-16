using UnityEngine;

namespace DynamicFogAndMist;

public class Demo1 : MonoBehaviour
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
		GUI.Label(position, "Demo 1 scene: windy mist fog style. Notice the subtle fog animation. To change look, select Main Camera and check image effect settings in inspector.", labelStyle);
	}
}
