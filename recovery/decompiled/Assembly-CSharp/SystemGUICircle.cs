using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SystemGUICircle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public float Size;

	public bool Over;

	private bool visible;

	private GameObject[] planets;

	public void SetPlanets(int num)
	{
		planets = new GameObject[num];
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = new GameObject("Planet", typeof(Image));
			Image component = gameObject.GetComponent<Image>();
			component.transform.transform.SetParent(transform, worldPositionStays: false);
			component.rectTransform.sizeDelta = Vector3.one * Size * 0.3f;
			float f = (float)Math.PI * 2f / (float)num * (float)i;
			component.rectTransform.localPosition = new Vector3(Mathf.Sin(f) * (Size / 1.7f), Mathf.Cos(f) * (Size / 1.7f), 0f);
			component.sprite = GetComponent<Image>().sprite;
			component.color = GetComponent<Image>().color;
			planets[i] = gameObject;
		}
	}

	private void Update()
	{
		if (Over)
		{
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 3f * Size, 0.1f);
			for (int i = 0; i < planets.Length; i++)
			{
				float f = (float)Math.PI * 2f / (float)planets.Length * (float)i + Time.time;
				planets[i].transform.localPosition = new Vector3(Mathf.Sin(f) * (Size / 1.7f), Mathf.Cos(f) * (Size / 1.7f), 0f);
			}
		}
		else
		{
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * Size, 0.1f);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Over = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Over = false;
	}
}
