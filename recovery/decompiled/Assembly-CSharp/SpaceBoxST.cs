using UnityEngine;

[AddComponentMenu("Rendering/SpaceBox ST")]
public class SpaceBoxST : MonoBehaviour
{
	private MeshFilter _meshFilter;

	private Mesh _mesh;

	private Transform _spaceBoxST;

	public Material material;

	private void Start()
	{
		_spaceBoxST = new GameObject("SpaceBoxST").transform;
		_spaceBoxST.gameObject.hideFlags = HideFlags.HideAndDontSave;
		_spaceBoxST.parent = transform;
		_spaceBoxST.localPosition = Vector3.zero;
		_meshFilter = _spaceBoxST.gameObject.GetComponent<MeshFilter>();
		if (_meshFilter == null)
		{
			_meshFilter = _spaceBoxST.gameObject.AddComponent<MeshFilter>();
		}
		if (_spaceBoxST.gameObject.GetComponent<MeshRenderer>() == null)
		{
			_spaceBoxST.gameObject.AddComponent<MeshRenderer>();
		}
		_meshFilter.mesh = Create();
		_spaceBoxST.gameObject.GetComponent<Renderer>().material = material;
	}

	private void LateUpdate()
	{
		_spaceBoxST.transform.rotation = Quaternion.identity;
	}

	private void Update()
	{
	}

	private Mesh Create(float length = 1f)
	{
		Mesh mesh = new Mesh();
		Vector3 vector = new Vector3((0f - length) * 0.5f, (0f - length) * 0.5f, length * 0.5f);
		Vector3 vector2 = new Vector3(length * 0.5f, (0f - length) * 0.5f, length * 0.5f);
		Vector3 vector3 = new Vector3(length * 0.5f, (0f - length) * 0.5f, (0f - length) * 0.5f);
		Vector3 vector4 = new Vector3((0f - length) * 0.5f, (0f - length) * 0.5f, (0f - length) * 0.5f);
		Vector3 vector5 = new Vector3((0f - length) * 0.5f, length * 0.5f, length * 0.5f);
		Vector3 vector6 = new Vector3(length * 0.5f, length * 0.5f, length * 0.5f);
		Vector3 vector7 = new Vector3(length * 0.5f, length * 0.5f, (0f - length) * 0.5f);
		Vector3 vector8 = new Vector3((0f - length) * 0.5f, length * 0.5f, (0f - length) * 0.5f);
		Vector3[] array = new Vector3[24]
		{
			vector, vector2, vector3, vector4, vector8, vector5, vector, vector4, vector5, vector6,
			vector2, vector, vector7, vector8, vector4, vector3, vector6, vector7, vector3, vector2,
			vector8, vector7, vector6, vector5
		};
		Vector3 up = Vector3.up;
		Vector3 down = Vector3.down;
		Vector3 forward = Vector3.forward;
		Vector3 back = Vector3.back;
		Vector3 left = Vector3.left;
		Vector3 right = Vector3.right;
		Vector3[] normals = new Vector3[24]
		{
			down, down, down, down, left, left, left, left, forward, forward,
			forward, forward, back, back, back, back, right, right, right, right,
			up, up, up, up
		};
		Vector2[] array2 = new Vector2[array.Length];
		float num = 8f;
		float num2 = num / 4096f;
		float num3 = 3f;
		float num4 = 2f;
		float num5 = (4096f - num * 2f) / num3 / 4096f;
		float num6 = (4096f - num * 4f) / num4 / 4096f;
		array2 = new Vector2[24]
		{
			new Vector2(num2 + num5 * 2f, num2 * 3f + num6),
			new Vector2(num2 + num5 * 2f, num2 * 3f + num6 * 2f),
			new Vector2(num2 + num5 * 3f, num2 * 3f + num6 * 2f),
			new Vector2(num2 + num5 * 3f, num2 * 3f + num6),
			new Vector2(num2 + num5 * 2f, num2 + num6),
			new Vector2(num2 + num5 * 3f, num2 + num6),
			new Vector2(num2 + num5 * 3f, num2),
			new Vector2(num2 + num5 * 2f, num2),
			new Vector2(num2 + num5, num2 * 3f + num6),
			new Vector2(num2 + num5, num2 * 3f + num6 * 2f),
			new Vector2(num2 + num5 + num5, num2 * 3f + num6 * 2f),
			new Vector2(num2 + num5 + num5, num2 * 3f + num6),
			new Vector2(num2 + num5, num2 + num6),
			new Vector2(num2 + num5 + num5, num2 + num6),
			new Vector2(num2 + num5 + num5, num2),
			new Vector2(num2 + num5, num2),
			new Vector2(num2, num2 + num6),
			new Vector2(num2 + num5, num2 + num6),
			new Vector2(num2 + num5, num2),
			new Vector2(num2, num2),
			new Vector2(num2, num2 * 3f + num6),
			new Vector2(num2, num2 * 3f + num6 * 2f),
			new Vector2(num2 + num5, num2 * 3f + num6 * 2f),
			new Vector2(num2 + num5, num2 * 3f + num6)
		};
		int[] triangles = new int[36]
		{
			0, 1, 3, 1, 2, 3, 4, 5, 7, 5,
			6, 7, 8, 9, 11, 9, 10, 11, 12, 13,
			15, 13, 14, 15, 16, 17, 19, 17, 18, 19,
			20, 21, 23, 21, 22, 23
		};
		mesh.vertices = array;
		mesh.normals = normals;
		mesh.uv = array2;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		return mesh;
	}
}
