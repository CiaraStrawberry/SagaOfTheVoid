using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class SelectorHelper : MonoBehaviour
{
	public float Scale;

	public bool Rotate;

	public float DegreesPerSecond;

	public Texture2D Texture;

	public Color Color;

	[SerializeField]
	[HideInInspector]
	private float _origScale;

	[SerializeField]
	[HideInInspector]
	private bool _origRotate;

	[SerializeField]
	[HideInInspector]
	private float _origDegreesPerSecond;

	[SerializeField]
	[HideInInspector]
	private Texture2D _origTexture;

	[SerializeField]
	[HideInInspector]
	private Color _origColor;

	public void Reset()
	{
		Scale = 1f;
		Rotate = true;
		DegreesPerSecond = 15f;
	}

	public void Start()
	{
		ApplySettings();
	}

	public void Update()
	{
		if (SettingsHaveChanged())
		{
			ApplySettings();
		}
	}

	public void ApplySettings()
	{
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.sharedMaterial.SetColor("_Color", Color);
			renderer.sharedMaterial.SetTexture("_MainTex", Texture);
		}
		Transform transform = base.transform.Find("Quad");
		if (transform != null)
		{
			transform.localScale = new Vector3(Scale, Scale, Scale);
		}
		Transform transform2 = base.transform.Find("Quad2");
		if (transform2 != null)
		{
			Vector3 vector = Vector3.one;
			SelectorBase component = transform2.GetComponent<SelectorBase>();
			if (component != null)
			{
				vector = component.BaseScale;
			}
			transform2.localScale = new Vector3(Scale * vector.x, Scale * vector.y, Scale * vector.z);
		}
		Projector[] componentsInChildren2 = gameObject.GetComponentsInChildren<Projector>();
		Projector[] array2 = componentsInChildren2;
		foreach (Projector projector in array2)
		{
			projector.material.SetColor("_Color", Color);
			projector.material.SetTexture("_ShadowTex", Texture);
			projector.orthographicSize = Scale;
		}
		Transform transform3 = base.transform.Find("Particle System");
		if (transform3 != null)
		{
			transform3.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", Color);
			ParticleSystem component2 = transform3.GetComponent<ParticleSystem>();
			component2.startSize = 3f * Scale;
		}
		SaveOriginalSettings();
	}

	private bool SettingsHaveChanged()
	{
		return Math.Abs(_origScale - Scale) > 0.001f || _origRotate != Rotate || Math.Abs(_origDegreesPerSecond - DegreesPerSecond) > 0.001f || _origTexture != Texture || _origColor != Color;
	}

	private void SaveOriginalSettings()
	{
		_origScale = Scale;
		_origRotate = Rotate;
		_origDegreesPerSecond = DegreesPerSecond;
		_origTexture = Texture;
		_origColor = Color;
	}
}
