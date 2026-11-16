using System;
using UnityEngine;

[Serializable]
public class ScreenDemo : MonoBehaviour
{
	public GameObject screen1;

	public GameObject screen2;

	public GameObject screen3;

	private bool showMenu;

	private float textureScale;

	private float scrollSpeed;

	private Flicker screen1Flicker;

	private Flicker screen2Flicker;

	private Flicker screen3Flicker;

	private ScrollTexture screen1ScrollTexture;

	private ScrollTexture screen2ScrollTexture;

	private ScrollTexture screen3ScrollTexture;

	private Material screen1Scanlines;

	private Material screen2Scanlines;

	private Material screen3Scanlines;

	private float scanlinesAlpha;

	private float minOnDelay;

	private float maxOnDelay;

	private float minOffDelay;

	private float maxOffDelay;

	private float minFlickerAlpha;

	private float maxFlickerAlpha;

	public ScreenDemo()
	{
		textureScale = 1f;
		scrollSpeed = -0.05f;
		scanlinesAlpha = 0.5f;
		minOnDelay = 0.5f;
		maxOnDelay = 5f;
		minOffDelay = 0.01f;
		maxOffDelay = 0.02f;
		minFlickerAlpha = 0.6f;
		maxFlickerAlpha = 0.7f;
	}

	public virtual void Start()
	{
		screen1ScrollTexture = (ScrollTexture)screen1.transform.Find("Screen_Scanlines").GetComponent(typeof(ScrollTexture));
		screen2ScrollTexture = (ScrollTexture)screen2.transform.Find("Screen_Scanlines").GetComponent(typeof(ScrollTexture));
		screen3ScrollTexture = (ScrollTexture)screen3.transform.Find("Screen_Scanlines").GetComponent(typeof(ScrollTexture));
		screen1Flicker = (Flicker)screen1.GetComponent(typeof(Flicker));
		screen2Flicker = (Flicker)screen2.GetComponent(typeof(Flicker));
		screen3Flicker = (Flicker)screen3.GetComponent(typeof(Flicker));
		screen1Scanlines = screen1ScrollTexture.gameObject.GetComponent<Renderer>().material;
		screen2Scanlines = screen2ScrollTexture.gameObject.GetComponent<Renderer>().material;
		screen3Scanlines = screen3ScrollTexture.gameObject.GetComponent<Renderer>().material;
	}

	public virtual void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			showMenu = !showMenu;
		}
	}

	public virtual void OnGUI()
	{
		if (!showMenu)
		{
			GUI.Label(new Rect(10f, 20f, 250f, 30f), "Press Tab to Toggle Screen Menu");
			return;
		}
		GUI.Label(new Rect(10f, 20f, 200f, 30f), "Scan Lines");
		GUI.Label(new Rect(10f, 50f, 200f, 30f), "Scan Lines Tiling Scale: " + textureScale);
		textureScale = GUI.HorizontalSlider(new Rect(10f, 70f, 200f, 30f), textureScale, 0.1f, 5f);
		textureScale = Mathf.Round(textureScale * 100f) / 100f;
		screen1Scanlines.mainTextureScale = new Vector2(textureScale, textureScale);
		screen2Scanlines.mainTextureScale = new Vector2(textureScale, textureScale);
		screen3Scanlines.mainTextureScale = new Vector2(textureScale, textureScale);
		GUI.Label(new Rect(10f, 90f, 200f, 30f), "Scroll Speed: " + scrollSpeed);
		scrollSpeed = GUI.HorizontalSlider(new Rect(10f, 110f, 200f, 30f), scrollSpeed, -0.5f, 0.5f);
		scrollSpeed = Mathf.Round(scrollSpeed * 100f) / 100f;
		screen1ScrollTexture.scrollYSpeed = scrollSpeed;
		screen2ScrollTexture.scrollYSpeed = scrollSpeed;
		screen3ScrollTexture.scrollYSpeed = scrollSpeed;
		GUI.Label(new Rect(10f, 130f, 200f, 30f), "Alpha: " + scanlinesAlpha);
		scanlinesAlpha = GUI.HorizontalSlider(new Rect(10f, 150f, 200f, 30f), scanlinesAlpha, 0f, 1f);
		scanlinesAlpha = Mathf.Round(scanlinesAlpha * 100f) / 100f;
		float a = scanlinesAlpha;
		Color color = screen1Scanlines.color;
		float num = (color.a = a);
		Color color2 = (screen1Scanlines.color = color);
		float a2 = scanlinesAlpha;
		Color color4 = screen2Scanlines.color;
		float num2 = (color4.a = a2);
		Color color5 = (screen2Scanlines.color = color4);
		float a3 = scanlinesAlpha;
		Color color7 = screen3Scanlines.color;
		float num3 = (color7.a = a3);
		Color color8 = (screen3Scanlines.color = color7);
		GUI.Label(new Rect(10f, 180f, 200f, 30f), "Screen Flicker");
		GUI.Label(new Rect(10f, 210f, 200f, 30f), "Minimum On Delay: " + minOnDelay);
		minOnDelay = GUI.HorizontalSlider(new Rect(10f, 230f, 200f, 30f), minOnDelay, 0.01f, 5f);
		minOnDelay = Mathf.Round(minOnDelay * 100f) / 100f;
		screen1Flicker.minOnDelay = minOnDelay;
		screen2Flicker.minOnDelay = minOnDelay;
		screen3Flicker.minOnDelay = minOnDelay;
		GUI.Label(new Rect(10f, 250f, 200f, 30f), "Maximum On Delay: " + maxOnDelay);
		maxOnDelay = GUI.HorizontalSlider(new Rect(10f, 270f, 200f, 30f), maxOnDelay, 0.01f, 5f);
		maxOnDelay = Mathf.Round(maxOnDelay * 100f) / 100f;
		screen1Flicker.maxOnDelay = maxOnDelay;
		screen2Flicker.maxOnDelay = maxOnDelay;
		screen3Flicker.maxOnDelay = maxOnDelay;
		GUI.Label(new Rect(10f, 290f, 200f, 30f), "Minimum Off Delay: " + minOffDelay);
		minOffDelay = GUI.HorizontalSlider(new Rect(10f, 310f, 200f, 30f), minOffDelay, 0.01f, 1f);
		minOffDelay = Mathf.Round(minOffDelay * 100f) / 100f;
		screen1Flicker.minOffDelay = minOffDelay;
		screen2Flicker.minOffDelay = minOffDelay;
		screen3Flicker.minOffDelay = minOffDelay;
		GUI.Label(new Rect(10f, 330f, 200f, 30f), "Maximum Off Delay: " + maxOffDelay);
		maxOffDelay = GUI.HorizontalSlider(new Rect(10f, 350f, 200f, 30f), maxOffDelay, 0.01f, 1f);
		maxOffDelay = Mathf.Round(maxOffDelay * 100f) / 100f;
		screen1Flicker.maxOffDelay = maxOffDelay;
		screen2Flicker.maxOffDelay = maxOffDelay;
		screen3Flicker.maxOffDelay = maxOffDelay;
		GUI.Label(new Rect(10f, 370f, 200f, 30f), "Minimum Alpha: " + minFlickerAlpha);
		minFlickerAlpha = GUI.HorizontalSlider(new Rect(10f, 390f, 200f, 30f), minFlickerAlpha, 0.1f, 1f);
		minFlickerAlpha = Mathf.Round(minFlickerAlpha * 100f) / 100f;
		screen1Flicker.minAlpha = minFlickerAlpha;
		screen2Flicker.minAlpha = minFlickerAlpha;
		screen3Flicker.minAlpha = minFlickerAlpha;
		GUI.Label(new Rect(10f, 410f, 200f, 30f), "Maximum Alpha: " + maxFlickerAlpha);
		maxFlickerAlpha = GUI.HorizontalSlider(new Rect(10f, 430f, 200f, 30f), maxFlickerAlpha, 0.1f, 1f);
		maxFlickerAlpha = Mathf.Round(maxFlickerAlpha * 100f) / 100f;
		screen1Flicker.maxAlpha = maxFlickerAlpha;
		screen2Flicker.maxAlpha = maxFlickerAlpha;
		screen3Flicker.maxAlpha = maxFlickerAlpha;
	}

	public virtual void Main()
	{
	}
}
