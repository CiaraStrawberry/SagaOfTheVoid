using System.Collections.Generic;
using Devdog.General;
using UnityEngine;

namespace Devdog.SciFiDesign;

public class CreateMeshPointTopology : MonoBehaviour
{
	[Required]
	public GameObject toCopy;

	public MeshTopology meshTopology = MeshTopology.Lines;

	private GameObject _copy;

	private void Awake()
	{
		_copy = Object.Instantiate(toCopy);
		MeshFilter component = _copy.GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		List<Vector3> list = new List<Vector3>(50000);
		int[] array = new int[50000];
		for (int i = 0; i < 50000; i++)
		{
			list.Add(Random.insideUnitSphere);
			array[i] = i;
		}
		mesh.SetVertices(list);
		mesh.SetIndices(array, meshTopology, 0);
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		component.mesh = mesh;
		_copy.transform.SetParent(toCopy.transform.parent);
		_copy.transform.position = toCopy.transform.position;
		_copy.transform.rotation = toCopy.transform.rotation;
		_copy.transform.localScale = toCopy.transform.localScale;
	}
}
