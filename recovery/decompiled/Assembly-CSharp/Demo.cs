using System;
using UnityEngine;

[Serializable]
public class Demo : MonoBehaviour
{
	public Texture2D[] Textures;

	private float _scale = 1f;

	private int _textureIndex;

	private byte _r;

	private byte _g = 128;

	private byte _b = byte.MaxValue;

	private byte _a = 128;

	private bool _walk;

	private void Start()
	{
		ApplyChanges();
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		GUILayout.BeginVertical();
		GUILayout.Space(16f);
		GUILayout.BeginHorizontal();
		GUILayout.Space(16f);
		GUILayout.BeginVertical("box", GUILayout.Width(200f));
		GUILayout.Label("Scale: " + _scale.ToString("0.00"));
		float scale = _scale;
		_scale = GUILayout.HorizontalSlider(_scale, 0.5f, 2f);
		byte r = _r;
		byte g = _g;
		byte b = _b;
		byte a = _a;
		GUILayout.Space(16f);
		GUILayout.Label("Color: (" + _r + "," + _g + "," + _b + "," + _a + ")");
		GUILayout.BeginHorizontal();
		GUILayout.Label("R", GUILayout.Width(24f));
		_r = (byte)GUILayout.HorizontalSlider((int)_r, 0f, 255f);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("G", GUILayout.Width(24f));
		_g = (byte)GUILayout.HorizontalSlider((int)_g, 0f, 255f);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("B", GUILayout.Width(24f));
		_b = (byte)GUILayout.HorizontalSlider((int)_b, 0f, 255f);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("A", GUILayout.Width(24f));
		_a = (byte)GUILayout.HorizontalSlider((int)_a, 0f, 255f);
		GUILayout.EndHorizontal();
		GUILayout.Space(16f);
		GUILayout.Label("Texture: " + _textureIndex);
		int textureIndex = _textureIndex;
		_textureIndex = (int)GUILayout.HorizontalSlider(_textureIndex, 0f, Textures.Length - 1);
		GUILayout.Space(16f);
		bool walk = _walk;
		_walk = GUILayout.Toggle(_walk, "Move Units");
		GUILayout.Space(16f);
		if (GUILayout.Button("Random"))
		{
			_scale = UnityEngine.Random.Range(0.75f, 1.5f);
			_r = (byte)UnityEngine.Random.Range(0, 255);
			_g = (byte)UnityEngine.Random.Range(0, 255);
			_b = (byte)UnityEngine.Random.Range(0, 255);
			_a = (byte)UnityEngine.Random.Range(64, 255);
			_textureIndex = UnityEngine.Random.Range(0, Textures.Length - 1);
		}
		if (GUILayout.Button("Reset"))
		{
			_scale = 1f;
			_r = 0;
			_g = 128;
			_b = byte.MaxValue;
			_a = 128;
			_textureIndex = 0;
			_walk = false;
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		if (r != _r || g != _g || b != _b || a != _a || textureIndex != _textureIndex || scale != _scale)
		{
			ApplyChanges();
		}
		if (walk == _walk)
		{
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Walker");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			Walker component = gameObject.GetComponent<Walker>();
			if (component == null)
			{
			}
		}
	}

	private void ApplyChanges()
	{
		Color32 color = new Color32(_r, _g, _b, _a);
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(SelectorHelper));
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object obj in array2)
		{
			SelectorHelper selectorHelper = obj as SelectorHelper;
			if (!(selectorHelper == null))
			{
				selectorHelper.Scale = _scale;
				selectorHelper.Color = color;
				selectorHelper.Texture = Textures[_textureIndex];
				selectorHelper.ApplySettings();
			}
		}
	}
}
