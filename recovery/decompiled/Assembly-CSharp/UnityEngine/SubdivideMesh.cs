using System.Collections.Generic;

namespace UnityEngine;

public class SubdivideMesh
{
	private static List<Vector3> vertices;

	private static List<Vector3> normals;

	private static List<int> indices;

	private static Dictionary<uint, int> newVectices;

	private static int GetNewVertex(int i1, int i2)
	{
		uint key = (uint)((i1 << 16) | i2);
		uint key2 = (uint)((i2 << 16) | i1);
		if (newVectices.ContainsKey(key2))
		{
			return newVectices[key2];
		}
		if (newVectices.ContainsKey(key))
		{
			return newVectices[key];
		}
		int count = vertices.Count;
		newVectices.Add(key, count);
		vertices.Add((vertices[i1] + vertices[i2]) * 0.5f);
		normals.Add((normals[i1] + normals[i2]).normalized);
		return count;
	}

	public static void Subdivide(Mesh mesh)
	{
		newVectices = new Dictionary<uint, int>();
		vertices = new List<Vector3>(mesh.vertices);
		normals = new List<Vector3>(mesh.normals);
		indices = new List<int>();
		int[] triangles = mesh.triangles;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			int num = triangles[i];
			int num2 = triangles[i + 1];
			int num3 = triangles[i + 2];
			int newVertex = GetNewVertex(num, num2);
			int newVertex2 = GetNewVertex(num2, num3);
			int newVertex3 = GetNewVertex(num3, num);
			indices.Add(num);
			indices.Add(newVertex);
			indices.Add(newVertex3);
			indices.Add(num2);
			indices.Add(newVertex2);
			indices.Add(newVertex);
			indices.Add(num3);
			indices.Add(newVertex3);
			indices.Add(newVertex2);
			indices.Add(newVertex);
			indices.Add(newVertex2);
			indices.Add(newVertex3);
		}
		mesh.vertices = vertices.ToArray();
		mesh.normals = normals.ToArray();
		mesh.triangles = indices.ToArray();
		newVectices = null;
		vertices = null;
		normals = null;
		indices = null;
	}
}
