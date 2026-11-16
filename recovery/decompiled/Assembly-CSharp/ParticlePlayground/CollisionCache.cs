using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class CollisionCache
{
	[HideInInspector]
	public bool[] hasCollided;

	[HideInInspector]
	public Vector3[] collisionPosition;

	[HideInInspector]
	public Vector3[] collisionNormal;

	[HideInInspector]
	public Transform[] collisionTransform;

	[HideInInspector]
	public GameObject[] collisionGameObject;

	[HideInInspector]
	public Vector3[] collisionTransformPosition;

	[HideInInspector]
	public Vector3[] stickyPosition;

	public CollisionCache(int amount)
	{
		hasCollided = new bool[amount];
		collisionPosition = new Vector3[amount];
		collisionNormal = new Vector3[amount];
		collisionTransform = new Transform[amount];
		collisionGameObject = new GameObject[amount];
		collisionTransformPosition = new Vector3[amount];
		stickyPosition = new Vector3[amount];
	}

	public void Set(int index, Vector3 position, Vector3 normal, Transform transform)
	{
		index %= hasCollided.Length;
		hasCollided[index] = true;
		collisionPosition[index] = position;
		collisionNormal[index] = normal;
		collisionTransform[index] = transform;
		collisionGameObject[index] = transform.gameObject;
	}

	public void SetSticky(int index, Vector3 position, Vector3 normal, float offset, Transform transform)
	{
		index %= hasCollided.Length;
		hasCollided[index] = true;
		collisionPosition[index] = position;
		collisionNormal[index] = normal;
		collisionTransform[index] = transform;
		collisionGameObject[index] = transform.gameObject;
		ref Vector3 reference = ref collisionTransformPosition[index];
		reference = transform.InverseTransformPoint(position + collisionNormal[index] * offset);
	}

	public void UpdateStickyPosition(int index)
	{
		index %= stickyPosition.Length;
		if (collisionGameObject[index] != null && collisionGameObject[index].activeSelf && collisionGameObject[index].activeInHierarchy)
		{
			ref Vector3 reference = ref stickyPosition[index];
			reference = collisionTransform[index].TransformPoint(collisionTransformPosition[index]);
		}
		else
		{
			hasCollided[index] = false;
		}
	}

	public void Reset(int index)
	{
		index %= hasCollided.Length;
		hasCollided[index] = false;
	}

	public void ClearCollisions()
	{
		for (int i = 0; i < hasCollided.Length; i++)
		{
			hasCollided[i] = false;
		}
	}

	public CollisionCache Clone()
	{
		CollisionCache collisionCache = new CollisionCache(collisionPosition.Length);
		collisionCache.hasCollided = (bool[])hasCollided.Clone();
		collisionCache.collisionPosition = (Vector3[])collisionPosition.Clone();
		collisionCache.collisionNormal = (Vector3[])collisionNormal.Clone();
		collisionCache.collisionTransform = (Transform[])collisionTransform.Clone();
		collisionCache.collisionTransformPosition = (Vector3[])collisionTransformPosition.Clone();
		collisionCache.stickyPosition = (Vector3[])stickyPosition.Clone();
		return collisionCache;
	}
}
