using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Devodg.SciFiDesign.UI;

public class BackgroundMeshGenerator : MonoBehaviour
{
	public struct Triangle
	{
		public int a;

		public int b;

		public int c;
	}

	public int vertices = 20;

	public Color startColor;

	public Color endColor;

	public bool debug;

	private Material _material;

	private MeshRenderer _renderer;

	protected void Awake()
	{
		_material = new Material(Shader.Find("Devdog/UI/BackgroundMesh"));
		_renderer = GetOrAddComponent<MeshRenderer>();
		MeshFilter orAddComponent = GetOrAddComponent<MeshFilter>();
		orAddComponent.mesh = Generate2DVoronoiMesh(new Rect(0f, 0f, Screen.width, Screen.height), vertices);
		_renderer.material = _material;
		UnityEngine.Random.InitState(DateTime.Now.Second);
	}

	private T GetOrAddComponent<T>() where T : Component
	{
		T component = GetComponent<T>();
		if (component != null)
		{
			return component;
		}
		return gameObject.AddComponent<T>();
	}

	protected Mesh Generate2DVoronoiMesh(Rect inRect, int amount)
	{
		Mesh mesh = new Mesh();
		List<Vector3> list = new List<Vector3>(amount + 4);
		list.Add(Vector3.zero);
		list.Add(Vector3.up * Screen.height);
		list.Add(Vector3.right * Screen.width);
		list.Add(new Vector3(1f * (float)Screen.width, 1f * (float)Screen.height, 0f));
		for (int i = 0; i < amount - 4; i++)
		{
			Vector3 item = new Vector3(UnityEngine.Random.Range(inRect.x, inRect.x + inRect.width), UnityEngine.Random.Range(inRect.y, inRect.y + inRect.height), 0f);
			list.Add(item);
		}
		list = (from o in list
			orderby o.x, o.y
			select o).ToList();
		Vector3[] v = list.ToArray();
		if (debug)
		{
			for (int num = 0; num < list.Count; num++)
			{
				GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
				gameObject.transform.SetParent(transform);
				gameObject.transform.localPosition = list[num];
				gameObject.transform.localScale = Vector3.one * 10f;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.layer = base.gameObject.layer;
			}
		}
		DelaunayTriangulation(v);
		mesh.vertices = v;
		mesh.RecalculateNormals();
		return mesh;
	}

	protected void DelaunayTriangulation(Vector3[] v)
	{
		int num = v.Length / 2;
		Vector3[] array = v.Take(num).ToArray();
		Vector3[] array2 = v.Skip(num).Take(v.Length - num).ToArray();
		if (array.Length > 3)
		{
			Debug.Log("Delaunay left : " + array.Length);
			DelaunayTriangulation(array);
		}
		else
		{
			Debug.Log("<color=green>Triangulate left</color>: " + array.Length);
		}
		if (array2.Length > 3)
		{
			Debug.Log("Delaunay right : " + array.Length);
			DelaunayTriangulation(array2);
		}
		else
		{
			Debug.Log("<color=yellow>Triangulate right</color>: " + array2.Length);
		}
		for (int i = 0; i < array.Length; i++)
		{
		}
	}

	protected int GetClosestVertexIndex(Vector3[] vertices, int fromIndex)
	{
		Vector3 b = vertices[fromIndex];
		int result = -1;
		float num = 9999f;
		for (int i = 0; i < vertices.Length; i++)
		{
			float num2 = Vector3.Distance(vertices[i], b);
			if (num2 < num)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}
}
