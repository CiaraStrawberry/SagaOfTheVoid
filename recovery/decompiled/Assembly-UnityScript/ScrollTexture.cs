using System;
using UnityEngine;

[Serializable]
public class ScrollTexture : MonoBehaviour
{
	public float scrollXSpeed;

	public float scrollYSpeed;

	private float offsetX;

	private float offsetY;

	public virtual void Update()
	{
		offsetX = Time.time * scrollXSpeed;
		offsetY = Time.time * scrollYSpeed;
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
	}

	public virtual void Main()
	{
	}
}
