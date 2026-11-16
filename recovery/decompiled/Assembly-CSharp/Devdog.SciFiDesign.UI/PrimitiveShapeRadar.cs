using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.SciFiDesign.UI;

[RequireComponent(typeof(RectTransform))]
public class PrimitiveShapeRadar : Graphic
{
	[Serializable]
	public class Element
	{
		public string name;

		public Color vertexColor = Color.white;

		[SerializeField]
		private float _statValue = 1f;

		public float statValue
		{
			get
			{
				return _statValue;
			}
			set
			{
				_statValue = value;
			}
		}

		public float aimValue { get; set; }

		public float startValue { get; set; }

		public float animOffset { get; set; }
	}

	public Element[] elements = new Element[0];

	public float scaleFactor = 1f;

	public Color centerColor = Color.white;

	public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public float animateSpeed = 1f;

	private IEnumerator _valuesCoroutine;

	public void AnimateInValues()
	{
		if (_valuesCoroutine != null)
		{
			StopCoroutine(_valuesCoroutine);
		}
		if (gameObject.activeInHierarchy)
		{
			_valuesCoroutine = _AnimateInValues();
			StartCoroutine(_valuesCoroutine);
			return;
		}
		Element[] array = elements;
		foreach (Element element in array)
		{
			element.statValue = element.startValue;
		}
	}

	private IEnumerator _AnimateInValues()
	{
		float offset = 0f;
		Element[] array = elements;
		foreach (Element element in array)
		{
			element.animOffset = UnityEngine.Random.value;
			element.startValue = element.statValue;
			offset = Mathf.Max(offset, element.animOffset);
		}
		float timer = 0f;
		while (timer < 1f + offset)
		{
			timer += Time.deltaTime * animateSpeed;
			float curvedTime = animationCurve.Evaluate(timer);
			Element[] array2 = elements;
			foreach (Element element2 in array2)
			{
				element2.statValue = Mathf.Lerp(element2.startValue, element2.aimValue, curvedTime);
			}
			UpdateElements();
			yield return null;
		}
	}

	public void UpdateElements()
	{
		UpdateGeometry();
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		Rect pixelAdjustedRect = GetPixelAdjustedRect();
		vh.Clear();
		vh.AddVert(new Vector3(pixelAdjustedRect.x + pixelAdjustedRect.width / 2f, pixelAdjustedRect.y + pixelAdjustedRect.height / 2f, 0f), centerColor, Vector2.zero);
		for (int i = 0; i < elements.Length; i++)
		{
			float f = (float)i / (float)elements.Length * (float)Math.PI * 2f;
			float num = Mathf.Cos(f) * elements[i].statValue * scaleFactor;
			float num2 = Mathf.Sin(f) * elements[i].statValue * scaleFactor;
			vh.AddVert(new Vector3(num * pixelAdjustedRect.x, num2 * pixelAdjustedRect.y), elements[i].vertexColor * color, Vector2.zero);
		}
		for (int j = 1; j < elements.Length; j++)
		{
			vh.AddTriangle(j, j + 1, 0);
		}
		vh.AddTriangle(elements.Length, 1, 0);
	}
}
