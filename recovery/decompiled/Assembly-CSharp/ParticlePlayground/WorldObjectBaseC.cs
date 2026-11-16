using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class WorldObjectBaseC
{
	public GameObject gameObject;

	[HideInInspector]
	public Transform transform;

	[HideInInspector]
	public Rigidbody rigidbody;

	[HideInInspector]
	public MeshFilter meshFilter;

	[HideInInspector]
	public Mesh mesh;

	[HideInInspector]
	public Vector3[] vertexPositions;

	[HideInInspector]
	public Vector3[] normals;

	[HideInInspector]
	public bool updateNormals;

	[NonSerialized]
	public int cachedId;

	[NonSerialized]
	public bool initialized;

	[HideInInspector]
	public Matrix4x4 transformMatrix = default;

	[HideInInspector]
	public ScaleMethod scaleMethod;

	public void UpdateMatrix(bool isLocal)
	{
		transformMatrix.SetTRS((!isLocal) ? transform.position : Vector3.zero, (!isLocal) ? transform.rotation : Quaternion.identity, (scaleMethod != ScaleMethod.Local) ? transform.lossyScale : transform.localScale);
	}
}
