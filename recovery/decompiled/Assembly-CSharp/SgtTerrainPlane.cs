using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Terrain Plane")]
public class SgtTerrainPlane : MonoBehaviour
{
	[Tooltip("The rows/columns of the collider plane")]
	[Range(1f, 10f)]
	public int Detail = 1;

	[Tooltip("The size of the plane in world space")]
	public float Size = 1f;

	[SerializeField]
	private Mesh mesh;

	[SerializeField]
	private MeshCollider meshCollider;

	[NonSerialized]
	private Vector3[] positions;

	[NonSerialized]
	private int[] indices;

	protected virtual void FixedUpdate()
	{
		if (Detail > 0)
		{
			Vector3 position = transform.position;
			float num = Size * Size;
			Vector3 bestPoint = default;
			SgtTerrain sgtTerrain = null;
			float num2 = float.PositiveInfinity;
			for (int num3 = SgtTerrain.AllTerrains.Count - 1; num3 >= 0; num3--)
			{
				SgtTerrain sgtTerrain2 = SgtTerrain.AllTerrains[num3];
				Vector3 worldPoint = sgtTerrain2.GetWorldPoint(position);
				float num4 = Vector3.SqrMagnitude(position - worldPoint);
				if (num4 < num && num4 < num2)
				{
					bestPoint = worldPoint;
					num2 = num4;
					sgtTerrain = sgtTerrain2;
				}
			}
			if (sgtTerrain != null)
			{
				Build(sgtTerrain, bestPoint);
				return;
			}
		}
		Pool();
	}

	protected virtual void OnDestroy()
	{
		Pool();
	}

	private void Build(SgtTerrain terrain, Vector3 bestPoint)
	{
		if (meshCollider == null)
		{
			GameObject gameObject = new GameObject("Plane");
			meshCollider = gameObject.AddComponent<MeshCollider>();
		}
		if (mesh == null)
		{
			mesh = SgtObjectPool<Mesh>.Pop() ?? new Mesh();
			mesh.name = "Plane";
		}
		int detail = Detail;
		int num = Detail + 1;
		int num2 = num * num;
		int num3 = detail * detail * 6;
		Quaternion quaternion = Quaternion.Inverse(terrain.transform.rotation) * Quaternion.LookRotation(bestPoint);
		float magnitude = bestPoint.magnitude;
		float v = SgtHelper.UniformScale(terrain.transform.lossyScale);
		float num4 = Size * SgtHelper.Reciprocal(v);
		float num5 = num4 * 2f / (float)Detail;
		if (positions == null || positions.Length != num2)
		{
			positions = new Vector3[num2];
		}
		for (int i = 0; i <= Detail; i++)
		{
			for (int j = 0; j <= Detail; j++)
			{
				int num6 = j + i * num;
				Vector3 localPoint = quaternion * new Vector3((float)j * num5 - num4, (float)i * num5 - num4, magnitude);
				ref Vector3 reference = ref positions[num6];
				reference = terrain.GetLocalPoint(localPoint);
			}
		}
		if (indices == null || indices.Length != num3)
		{
			indices = new int[num3];
			for (int k = 0; k < detail; k++)
			{
				for (int l = 0; l < detail; l++)
				{
					int num7 = (l + k * detail) * 6;
					int num8 = l + k * num;
					indices[num7] = num8;
					indices[num7 + 1] = num8 + 1;
					indices[num7 + 2] = num8 + num;
					indices[num7 + 3] = num8 + num + 1;
					indices[num7 + 4] = num8 + num;
					indices[num7 + 5] = num8 + 1;
				}
			}
			mesh.Clear();
		}
		mesh.vertices = positions;
		mesh.triangles = indices;
		meshCollider.sharedMesh = mesh;
		meshCollider.transform.SetParent(terrain.transform, worldPositionStays: false);
	}

	private void Pool()
	{
		if (meshCollider != null)
		{
			SgtHelper.Destroy(meshCollider.gameObject);
			meshCollider = null;
		}
		if (mesh != null)
		{
			mesh.Clear(keepVertexLayout: false);
			mesh = SgtObjectPool<Mesh>.Add(mesh);
		}
	}
}
