using System;
using UnityEngine;

[Serializable]
public class TurnAndMove : MonoBehaviour
{
	public float angleX;

	public float angleY;

	public float angleZ;

	public TurnAndMove()
	{
		angleX = 45f;
		angleY = 45f;
		angleZ = 45f;
	}

	public virtual void Update()
	{
		transform.Rotate(new Vector3(1f, 0f, 0f) * angleX * Time.deltaTime);
	}

	public virtual void Main()
	{
	}
}
