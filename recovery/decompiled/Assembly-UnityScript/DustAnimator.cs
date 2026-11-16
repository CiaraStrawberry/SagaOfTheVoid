using System;
using UnityEngine;

[Serializable]
public class DustAnimator : MonoBehaviour
{
	public float xSpeed;

	public float ySpeed;

	public virtual void Update()
	{
		float x = Time.time * xSpeed;
		float y = Time.time * ySpeed;
		GetComponent<Renderer>().material.mainTextureOffset = new Vector2(x, y);
	}

	public virtual void Main()
	{
	}
}
