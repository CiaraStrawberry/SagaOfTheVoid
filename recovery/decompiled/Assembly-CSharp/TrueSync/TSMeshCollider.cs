using System.Collections.Generic;
using System.Linq;
using TrueSync.Physics3D;
using UnityEngine;

namespace TrueSync;

[AddComponentMenu("TrueSync/Physics/MeshCollider", 0)]
public class TSMeshCollider : TSCollider
{
	[SerializeField]
	private Mesh mesh;

	private List<TSVector> vertices;

	private List<TriangleVertexIndices> indices;

	public Mesh Mesh
	{
		get
		{
			return mesh;
		}
		set
		{
			mesh = value;
			vertices = GetVertices();
			indices = GetIndices();
			if (_body != null)
			{
				_body.Shape = CreateShape();
			}
		}
	}

	public List<TSVector> Vertices
	{
		get
		{
			if (vertices == null)
			{
				vertices = GetVertices();
			}
			return vertices;
		}
	}

	public List<TriangleVertexIndices> Indices
	{
		get
		{
			if (indices == null)
			{
				indices = GetIndices();
			}
			return indices;
		}
	}

	public void Reset()
	{
		if (mesh == null)
		{
			MeshFilter component = GetComponent<MeshFilter>();
			mesh = component.sharedMesh;
		}
	}

	public override Shape CreateShape()
	{
		Octree octree = new Octree(Vertices, Indices);
		return new TriangleMeshShape(octree);
	}

	private List<TriangleVertexIndices> GetIndices()
	{
		int[] triangles = mesh.triangles;
		List<TriangleVertexIndices> list = new List<TriangleVertexIndices>();
		for (int i = 0; i < triangles.Length; i += 3)
		{
			list.Add(new TriangleVertexIndices(triangles[i + 2], triangles[i + 1], triangles[i]));
		}
		return list;
	}

	private List<TSVector> GetVertices()
	{
		return mesh.vertices.Select((Vector3 p) => new TSVector(p.x * lossyScale.x, p.y * lossyScale.y, p.z * lossyScale.z)).ToList();
	}

	protected override Vector3 GetGizmosSize()
	{
		return lossyScale.ToVector();
	}

	protected override void DrawGizmos()
	{
		Gizmos.DrawWireMesh(mesh);
	}
}
