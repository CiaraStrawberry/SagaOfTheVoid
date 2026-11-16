using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("")]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SgtTerrainLevel : MonoBehaviour
{
	[Tooltip("The terrain face this belongs to")]
	public SgtTerrainFace Face;

	[Tooltip("The index of this LOD level")]
	public int Index;

	[NonSerialized]
	public bool Marked;

	[NonSerialized]
	private long size;

	[NonSerialized]
	private SgtRectL basicRegion;

	[NonSerialized]
	private SgtRectL outerRegion;

	[NonSerialized]
	private SgtRectL innerRegion;

	[NonSerialized]
	private Mesh mesh;

	[NonSerialized]
	private Mesh colliderMesh;

	[NonSerialized]
	private MeshFilter meshFilter;

	[NonSerialized]
	private MeshRenderer meshRenderer;

	[NonSerialized]
	private MeshCollider meshCollider;

	[NonSerialized]
	private Vector3[] positions;

	[NonSerialized]
	private Vector3[] normals;

	[NonSerialized]
	private Vector2[] coords1;

	[NonSerialized]
	private Vector2[] coords2;

	[NonSerialized]
	private static List<int> indices = new List<int>();

	public Mesh Mesh => mesh;

	public void UpdateMaterials()
	{
		if (!(Face != null))
		{
			return;
		}
		SgtTerrain terrain = Face.Terrain;
		if (terrain != null)
		{
			if (meshRenderer == null)
			{
				meshRenderer = GetComponent<MeshRenderer>();
			}
			meshRenderer.sharedMaterials = terrain.GetMaterials(this);
		}
	}

	public void Dirty()
	{
		basicRegion.Clear();
		innerRegion.Clear();
		outerRegion.Clear();
	}

	public void PoolMeshNow()
	{
		if (mesh != null)
		{
			mesh.Clear(keepVertexLayout: false);
			mesh = SgtObjectPool<Mesh>.Add(mesh);
		}
	}

	public void PoolColliderMeshNow(bool destroyCollider = true)
	{
		if (colliderMesh != null)
		{
			if (destroyCollider && meshCollider != null)
			{
				meshCollider = SgtHelper.Destroy(meshCollider);
			}
			colliderMesh.Clear(keepVertexLayout: false);
			colliderMesh = SgtObjectPool<Mesh>.Add(colliderMesh);
		}
	}

	public Mesh GetMesh()
	{
		if (mesh == null)
		{
			mesh = SgtObjectPool<Mesh>.Pop() ?? new Mesh();
			mesh.name = "Terrain";
		}
		if (meshFilter == null)
		{
			meshFilter = GetComponent<MeshFilter>();
		}
		meshFilter.sharedMesh = mesh;
		return mesh;
	}

	public Mesh GetColliderMesh()
	{
		if (colliderMesh == null)
		{
			colliderMesh = SgtObjectPool<Mesh>.Pop() ?? new Mesh();
			colliderMesh.name = "Terrain (Capped)";
		}
		return colliderMesh;
	}

	public static int Wrap(long x, long y, long size)
	{
		x %= size;
		if (x < 0)
		{
			x += size;
		}
		y %= size;
		if (y < 0)
		{
			y += size;
		}
		return (int)(x + y * size);
	}

	public void Write(SgtTerrain.Ring ring)
	{
		if (ring.Outer == outerRegion && ring.Inner == innerRegion)
		{
			return;
		}
		size = ring.Limit + 3;
		SgtTerrain terrain = Face.Terrain;
		long num = size * size;
		SgtRectL expanded = ring.Outer.GetExpanded(1L);
		if (positions == null || positions.Length != num)
		{
			basicRegion.Clear();
			outerRegion.Clear();
			positions = new Vector3[num];
			normals = new Vector3[num];
			coords1 = new Vector2[num];
			coords2 = new Vector2[num];
		}
		SgtVector3D sgtVector3D = ring.ExtentsX / ring.Detail;
		SgtVector3D sgtVector3D2 = ring.ExtentsY / ring.Detail;
		for (long num2 = expanded.minY; num2 <= expanded.maxY; num2++)
		{
			for (long num3 = expanded.minX; num3 <= expanded.maxX; num3++)
			{
				if (!basicRegion.Contains(num3, num2))
				{
					int num4 = Wrap(num3, num2, size);
					SgtVector3D localPoint = ring.Center + num3 * sgtVector3D + num2 * sgtVector3D2;
					double num5 = ((double)num3 / (double)ring.Detail + 1.0) / 2.0;
					double num6 = ((double)num2 / (double)ring.Detail + 1.0) / 2.0;
					double num7 = (double)num3 / (double)ring.Limit;
					double num8 = (double)num2 / (double)ring.Limit;
					float localHeight = terrain.GetLocalHeight(localPoint);
					SgtVector3D normalized = localPoint.normalized;
					SgtVector3D sgtVector3D3 = normalized * localHeight;
					ref Vector3 reference = ref positions[num4];
					reference = (Vector3)sgtVector3D3;
					ref Vector2 reference2 = ref coords1[num4];
					reference2 = new Vector2((float)num5, (float)num6);
					ref Vector2 reference3 = ref coords2[num4];
					reference3 = new Vector2((float)num7, (float)num8);
				}
			}
		}
		SgtTerrainLevel lastLevel = Face.LastLevel;
		WriteNormals(ring.Outer, outerRegion);
		if (lastLevel != null)
		{
			if (ring.Outer.minX > -ring.Detail)
			{
				WriteParentNormals(lastLevel, ring.Outer.minX, ring.Outer.minY, ring.Outer.minX, ring.Outer.maxY);
			}
			if (ring.Outer.maxX < ring.Detail)
			{
				WriteParentNormals(lastLevel, ring.Outer.maxX, ring.Outer.minY, ring.Outer.maxX, ring.Outer.maxY);
			}
			if (ring.Outer.minY > -ring.Detail)
			{
				WriteParentNormals(lastLevel, ring.Outer.minX, ring.Outer.minY, ring.Outer.maxX, ring.Outer.minY);
			}
			if (ring.Outer.maxY < ring.Detail)
			{
				WriteParentNormals(lastLevel, ring.Outer.minX, ring.Outer.maxY, ring.Outer.maxX, ring.Outer.maxY);
			}
		}
		WriteIndices(ring.Outer, ring.Inner, ring.Detail, size);
		Mesh mesh = GetMesh();
		mesh.Clear();
		mesh.vertices = positions;
		mesh.normals = normals;
		mesh.uv = coords1;
		mesh.uv2 = coords2;
		mesh.SetTriangles(indices, 0);
		mesh.bounds = terrain.GetBounds();
		if (terrain.MaxColliderDepth > Index)
		{
			if (meshCollider == null)
			{
				meshCollider = GetComponent<MeshCollider>();
			}
			if (meshCollider == null)
			{
				meshCollider = gameObject.AddComponent<MeshCollider>();
			}
			if (terrain.MaxColliderDepth == Index + 1 && ring.Inner.SizeX > 0 && ring.Inner.SizeY > 0)
			{
				Mesh mesh2 = GetColliderMesh();
				WriteIndices(ring.Outer, default, ring.Detail, size);
				mesh2.Clear();
				mesh2.vertices = positions;
				mesh2.SetTriangles(indices, 0);
				meshCollider.sharedMesh = mesh2;
			}
			else
			{
				PoolColliderMeshNow(destroyCollider: false);
				meshCollider.sharedMesh = mesh;
			}
		}
		else
		{
			PoolColliderMeshNow();
		}
		basicRegion = expanded;
		outerRegion = ring.Outer;
		innerRegion = ring.Inner;
		Face.LastLevel = this;
		if (terrain.OnCalculateLevel != null)
		{
			terrain.OnCalculateLevel(this, ring);
		}
	}

	public static SgtTerrainLevel Create(string name, int layer, Transform parent)
	{
		return SgtComponentPool<SgtTerrainLevel>.Pop(parent, name, layer);
	}

	public static SgtTerrainLevel Pool(SgtTerrainLevel level)
	{
		if (level != null)
		{
			level.Face = null;
			level.PoolMeshNow();
			level.PoolColliderMeshNow();
			SgtComponentPool<SgtTerrainLevel>.Add(level);
		}
		return null;
	}

	public static SgtTerrainLevel MarkForDestruction(SgtTerrainLevel level)
	{
		if (level != null)
		{
			level.Face = null;
			level.gameObject.SetActive(value: true);
		}
		return null;
	}

	protected virtual void OnEnable()
	{
		Dirty();
		UpdateMaterials();
	}

	protected virtual void OnDestroy()
	{
		PoolMeshNow();
	}

	protected virtual void Update()
	{
		if (Face == null)
		{
			Pool(this);
		}
	}

	private void WriteNormals(SgtRectL outer, SgtRectL inner)
	{
		inner.ClampTo(outer);
		WriteNormals(outer.minX, inner.maxY, outer.maxX, outer.maxY);
		WriteNormals(outer.minX, outer.minY, outer.maxX, inner.maxY);
		WriteNormals(outer.minX, inner.minX, inner.minX, inner.maxY);
		WriteNormals(inner.maxX, inner.minX, inner.maxX, inner.maxY);
	}

	private void WriteNormals(long minX, long minY, long maxX, long maxY)
	{
		for (long num = minY; num <= maxY; num++)
		{
			for (long num2 = minX; num2 <= maxX; num2++)
			{
				int num3 = Wrap(num2, num, size);
				Vector3 vector = positions[Wrap(num2 - 1, num, size)];
				Vector3 vector2 = positions[Wrap(num2 + 1, num, size)];
				Vector3 vector3 = positions[Wrap(num2, num - 1, size)];
				Vector3 vector4 = positions[Wrap(num2, num + 1, size)];
				Vector3 vector5 = -Vector3.Cross(vector2 - vector, vector4 - vector3);
				ref Vector3 reference = ref normals[num3];
				reference = vector5.normalized;
			}
		}
	}

	private void WriteParentNormals(SgtTerrainLevel parent, long minX, long minY, long maxX, long maxY)
	{
		for (long num = minY; num <= maxY; num++)
		{
			for (long num2 = minX; num2 <= maxX; num2++)
			{
				int num3 = Wrap(num2, num, size);
				double num4 = (double)num2 / 2.0;
				double num5 = (double)num / 2.0;
				Vector3 vector = parent.normals[Wrap((long)num4, (long)num5, parent.size)];
				normals[num3] = vector;
			}
		}
	}

	private void WriteIndices(SgtRectL outer, SgtRectL inner, long detail, long size)
	{
		SgtHelper.ClearCapacity(indices, 1024);
		for (long num = outer.minY; num < outer.maxY; num++)
		{
			for (long num2 = outer.minX; num2 < outer.maxX; num2++)
			{
				if (num2 >= inner.minX && num2 < inner.maxX && num >= inner.minY && num < inner.maxY)
				{
					continue;
				}
				long num3 = num2;
				long num4 = num;
				long num5 = num2 + 1;
				long num6 = num;
				long num7 = num2;
				long num8 = num + 1;
				long num9 = num2 + 1;
				long num10 = num + 1;
				if (num == outer.minY && num > -detail)
				{
					if (num2 % 2 == 0)
					{
						num5--;
					}
					else
					{
						num3--;
					}
				}
				if (num == outer.maxY - 1 && num < detail - 1)
				{
					if (num2 % 2 == 0)
					{
						num9++;
					}
					else
					{
						num7++;
					}
				}
				if (num2 == outer.minX && num2 > -detail)
				{
					if (num % 2 == 0)
					{
						num8--;
					}
					else
					{
						num4--;
					}
				}
				if (num2 == outer.maxX - 1 && num2 < detail - 1)
				{
					if (num % 2 == 0)
					{
						num10++;
					}
					else
					{
						num6++;
					}
				}
				int item = Wrap(num3, num4, size);
				int item2 = Wrap(num5, num6, size);
				int item3 = Wrap(num7, num8, size);
				int item4 = Wrap(num9, num10, size);
				indices.Add(item);
				indices.Add(item3);
				indices.Add(item2);
				indices.Add(item4);
				indices.Add(item2);
				indices.Add(item3);
			}
		}
	}

	private static float Area(Vector3 a, Vector3 b, Vector3 c)
	{
		return Vector3.Cross(a - b, a - c).magnitude;
	}
}
