using System.Collections.Generic;
using System.Linq;
using Devdog.General.UI;
using UnityEngine;

namespace Devdog.SciFiDesign;

public class PointGridGenerator : MonoBehaviour
{
	public int rows = 10;

	public int cols = 10;

	public Vector2 spacing = Vector2.one;

	public Material material;

	public float movementEffect = 1f;

	public float maxMovementEffect = 0.2f;

	public float effectDegradeSpeed = -0.01f;

	private GameObject _gridObj;

	private MeshFilter _meshFilter;

	private MeshRenderer _meshRenderer;

	private Vector3 _mousePrevFrame;

	private float _mouseMovementSpeed;

	private void Start()
	{
		_gridObj = new GameObject();
		_gridObj.transform.SetParent(transform);
		UIUtility.ResetTransform(_gridObj.transform);
		_meshFilter = _gridObj.AddComponent<MeshFilter>();
		_meshRenderer = _gridObj.AddComponent<MeshRenderer>();
		_mousePrevFrame = Input.mousePosition;
		GeneratePoints();
	}

	protected void Update()
	{
		Vector3 vector = Input.mousePosition - _mousePrevFrame;
		vector.x /= Screen.width;
		vector.y /= Screen.height;
		_mouseMovementSpeed += (vector * movementEffect).magnitude;
		_mouseMovementSpeed -= effectDegradeSpeed * Time.deltaTime;
		_mouseMovementSpeed = Mathf.Clamp(_mouseMovementSpeed, 0f, maxMovementEffect);
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.x /= Screen.width;
		mousePosition.y /= Screen.height;
		material.SetFloat("_MouseMovementSpeed", _mouseMovementSpeed);
		material.SetVector("_MousePosition", mousePosition);
		_mousePrevFrame = Input.mousePosition;
	}

	public void GeneratePoints()
	{
		Mesh mesh = new Mesh();
		List<Vector3> list = new List<Vector3>(rows * cols);
		List<Vector2> list2 = new List<Vector2>(rows * cols);
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				list.Add(new Vector3((float)i * spacing.x, (float)j * spacing.y, 0f));
				list2.Add(new Vector2((float)i / (float)rows, (float)j / (float)cols));
			}
		}
		mesh.SetVertices(list);
		mesh.SetTriangles(new int[0], 0);
		mesh.SetIndices(Enumerable.Range(0, rows * cols).ToArray(), MeshTopology.Points, 0);
		mesh.SetUVs(0, list2);
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		_meshFilter.mesh = mesh;
		_meshRenderer.material = material;
	}
}
