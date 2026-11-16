using UnityEngine;

public class TextFader : MonoBehaviour
{
	private GUIText gText;

	private Color startColor;

	private Color fadedColor;

	private void Start()
	{
		gText = GetComponent<GUIText>();
		startColor = gText.material.color;
		fadedColor = new Color(startColor.r, startColor.g, startColor.b, 0.5f);
	}

	private void Update()
	{
		gText.material.color = Color.Lerp(startColor, fadedColor, Mathf.PingPong(Time.time * 2f, 1f));
	}
}
