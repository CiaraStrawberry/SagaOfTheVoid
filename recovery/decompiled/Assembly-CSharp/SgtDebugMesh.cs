using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[AddComponentMenu("Space Graphics Toolkit/SGT Debug Mesh")]
public class SgtDebugMesh : MonoBehaviour
{
	[Tooltip("The scale of the normal andtangent lines")]
	public float DrawScale = 1f;

	[Tooltip("The color of the normals")]
	public Color TriangleColor = new Color(1f, 0f, 0f, 0.5f);

	[Tooltip("The color of the normals")]
	public Color NormalColor = new Color(0f, 1f, 0f, 0.5f);

	[Tooltip("The color of the tangents")]
	public Color TangentColor = new Color(0f, 0f, 1f, 0.5f);

	[NonSerialized]
	private MeshFilter meshFilter;
}
