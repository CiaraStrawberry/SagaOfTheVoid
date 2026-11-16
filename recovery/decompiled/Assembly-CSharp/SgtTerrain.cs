using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("Space Graphics Toolkit/SGT Terrain")]
public class SgtTerrain : MonoBehaviour
{
	public class Ring
	{
		public long Limit;

		public long Detail;

		public SgtRectL Inner;

		public SgtRectL Outer;

		public SgtVector3D Center;

		public SgtVector3D ExtentsX;

		public SgtVector3D ExtentsY;
	}

	public class Shell
	{
		public long Limit;

		public long Detail;

		public SgtBoundsL Inner;

		public SgtBoundsL Outer;
	}

	public delegate void CalculateHeightDelegate(SgtVector3D localPosition, ref float displacement);

	public delegate void CalculateNormalDelegate(SgtVector3D localPosition, ref Vector3 normal);

	public delegate void CalculateMaterialDelegate(SgtTerrainLevel level, ref Material material);

	public delegate void CalculateQuadDelegate(SgtTerrainLevel level, Ring ring);

	public static List<SgtTerrain> AllTerrains = new List<SgtTerrain>();

	[Tooltip("The base material for the terrain (can be overirdden by each face)")]
	public Material Material;

	[Tooltip("The atmosphere applied to this terrain")]
	public SgtAtmosphere Atmosphere;

	[Tooltip("The distance the bounds are shifted toward the current camera")]
	public float BoundsOffset;

	[Tooltip("The transform the LOD will be based around (e.g. your main camera)")]
	public Transform Target;

	[Tooltip("The base radius of the terrain")]
	public float Radius = 1f;

	[Tooltip("The detail of each LOD level (NOTE: If you set this too high with high LOD distances, you will get errors)")]
	[Range(1f, 25f)]
	public int Detail = 5;

	[Tooltip("The maximum LOD depth that colliders will be generated for (0 = none)")]
	public int MaxColliderDepth;

	[Tooltip("The LOD distances in local space, these should be sorted from high to low")]
	public List<double> Distances;

	public SgtTerrainFace NegativeX;

	public SgtTerrainFace NegativeY;

	public SgtTerrainFace NegativeZ;

	public SgtTerrainFace PositiveX;

	public SgtTerrainFace PositiveY;

	public SgtTerrainFace PositiveZ;

	public CalculateHeightDelegate OnCalculateHeight;

	public CalculateQuadDelegate OnCalculateLevel;

	public CalculateMaterialDelegate OnCalculateMaterial;

	private static List<Shell> shells = new List<Shell>();

	private static Material[] materials1 = new Material[1];

	private static Material[] materials2 = new Material[2];

	private SgtVector3D CalculateTarget()
	{
		SgtVector3D sgtVector3D = new SgtVector3D(transform.InverseTransformPoint(Target.position));
		if (sgtVector3D.sqrMagnitude > 0.0)
		{
			sgtVector3D *= InvCube(sgtVector3D.normalized).magnitude;
		}
		float localHeight = GetLocalHeight(sgtVector3D);
		if (localHeight > 0f)
		{
			sgtVector3D /= (double)localHeight;
		}
		return sgtVector3D;
	}

	public Material GetMaterial(SgtTerrainLevel level)
	{
		Material material = Material;
		if (level != null && level.Face != null && level.Face.Material != null)
		{
			material = level.Face.Material;
		}
		if (OnCalculateMaterial != null)
		{
			OnCalculateMaterial(level, ref material);
		}
		return material;
	}

	public Material[] GetMaterials(SgtTerrainLevel level)
	{
		Material material = GetMaterial(level);
		if (SgtHelper.Enabled(Atmosphere))
		{
			materials2[0] = material;
			materials2[1] = Atmosphere.InnerMaterial;
			return materials2;
		}
		materials1[0] = material;
		return materials1;
	}

	public float GetLocalHeight(SgtVector3D localPoint)
	{
		float displacement = Radius;
		if (OnCalculateHeight != null)
		{
			OnCalculateHeight(localPoint, ref displacement);
		}
		return displacement;
	}

	public float GetLocalHeight(Vector3 localPoint)
	{
		return GetLocalHeight(new SgtVector3D(localPoint));
	}

	public float GetWorldHeight(Vector3 worldPoint)
	{
		worldPoint = GetWorldPoint(worldPoint);
		return Vector3.Distance(worldPoint, transform.position);
	}

	public SgtVector3D GetLocalPoint(SgtVector3D localPoint)
	{
		return localPoint.normalized * GetLocalHeight(localPoint);
	}

	public Vector3 GetLocalPoint(Vector3 localPoint)
	{
		return localPoint.normalized * GetLocalHeight(localPoint);
	}

	public Vector3 GetWorldPoint(Vector3 worldPoint, float offset = 0f)
	{
		Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
		localPoint = GetLocalPoint(localPoint);
		Vector3 result = transform.TransformPoint(localPoint);
		if (offset != 0f)
		{
			result += (worldPoint - transform.position).normalized * offset;
		}
		return result;
	}

	public Vector3 GetLocalNormalFast(SgtVector3D localPoint, SgtVector3D right, SgtVector3D forward)
	{
		SgtVector3D localPoint2 = GetLocalPoint(localPoint);
		SgtVector3D localPoint3 = GetLocalPoint(localPoint + right);
		SgtVector3D localPoint4 = GetLocalPoint(localPoint + forward);
		return (Vector3)SgtVector3D.Cross(localPoint2 - localPoint3, localPoint4 - localPoint2).normalized;
	}

	public Vector3 GetLocalNormalFast2(SgtVector3D localPoint, SgtVector3D right, SgtVector3D forward)
	{
		SgtVector3D localPoint2 = GetLocalPoint(localPoint + right);
		SgtVector3D localPoint3 = GetLocalPoint(localPoint + forward);
		return (Vector3)SgtVector3D.Cross(localPoint - localPoint2, localPoint3 - localPoint).normalized;
	}

	public Vector3 GetLocalNormal(SgtVector3D localPoint, SgtVector3D right, SgtVector3D forward)
	{
		SgtVector3D localPoint2 = GetLocalPoint(localPoint - right);
		SgtVector3D localPoint3 = GetLocalPoint(localPoint + right);
		SgtVector3D localPoint4 = GetLocalPoint(localPoint - forward);
		SgtVector3D localPoint5 = GetLocalPoint(localPoint + forward);
		return (Vector3)SgtVector3D.Cross(localPoint2 - localPoint3, localPoint5 - localPoint4).normalized;
	}

	public Vector3 GetLocalNormal2(SgtVector3D localPoint, SgtVector3D right, SgtVector3D forward)
	{
		SgtVector3D localPoint2 = GetLocalPoint(localPoint - right);
		SgtVector3D localPoint3 = GetLocalPoint(localPoint + right);
		SgtVector3D localPoint4 = GetLocalPoint(localPoint - forward);
		SgtVector3D localPoint5 = GetLocalPoint(localPoint + forward);
		return (Vector3)SgtVector3D.Cross(localPoint2 - localPoint3, localPoint5 - localPoint4).normalized;
	}

	public Vector3 GetWorldNormal(Vector3 worldPoint)
	{
		return (transform.position - worldPoint).normalized;
	}

	public Vector3 GetWorldNormal(Vector3 worldPoint, Vector3 right, Vector3 forward)
	{
		Vector3 worldPoint2 = GetWorldPoint(worldPoint);
		Vector3 worldPoint3 = GetWorldPoint(worldPoint + right);
		Vector3 worldPoint4 = GetWorldPoint(worldPoint + forward);
		Vector3 lhs = worldPoint2 - worldPoint3;
		Vector3 rhs = worldPoint4 - worldPoint2;
		return Vector3.Cross(lhs, rhs);
	}

	public Vector3 GetWorldNormalFast(Vector3 worldPoint, Vector3 right, Vector3 forward)
	{
		Vector3 worldPoint2 = GetWorldPoint(worldPoint + right);
		Vector3 worldPoint3 = GetWorldPoint(worldPoint + forward);
		Vector3 lhs = worldPoint - worldPoint2;
		Vector3 rhs = worldPoint3 - worldPoint;
		return Vector3.Cross(lhs, rhs);
	}

	[ContextMenu("Dirty Meshes")]
	public void DirtyMeshes()
	{
		if (NegativeX != null)
		{
			NegativeX.Dirty();
		}
		if (NegativeY != null)
		{
			NegativeY.Dirty();
		}
		if (NegativeZ != null)
		{
			NegativeZ.Dirty();
		}
		if (PositiveX != null)
		{
			PositiveX.Dirty();
		}
		if (PositiveY != null)
		{
			PositiveY.Dirty();
		}
		if (PositiveZ != null)
		{
			PositiveZ.Dirty();
		}
	}

	[ContextMenu("Update Materials")]
	public void UpdateMaterials()
	{
		if (NegativeX != null)
		{
			NegativeX.UpdateMaterials();
		}
		if (NegativeY != null)
		{
			NegativeY.UpdateMaterials();
		}
		if (NegativeZ != null)
		{
			NegativeZ.UpdateMaterials();
		}
		if (PositiveX != null)
		{
			PositiveX.UpdateMaterials();
		}
		if (PositiveY != null)
		{
			PositiveY.UpdateMaterials();
		}
		if (PositiveZ != null)
		{
			PositiveZ.UpdateMaterials();
		}
	}

	public void UpdateMeshes()
	{
		if (Detail > 0)
		{
			if (NegativeX == null)
			{
				NegativeX = CreateFace(CubemapFace.NegativeX);
			}
			if (NegativeY == null)
			{
				NegativeY = CreateFace(CubemapFace.NegativeY);
			}
			if (NegativeZ == null)
			{
				NegativeZ = CreateFace(CubemapFace.NegativeZ);
			}
			if (PositiveX == null)
			{
				PositiveX = CreateFace(CubemapFace.PositiveX);
			}
			if (PositiveY == null)
			{
				PositiveY = CreateFace(CubemapFace.PositiveY);
			}
			if (PositiveZ == null)
			{
				PositiveZ = CreateFace(CubemapFace.PositiveZ);
			}
			UpdateShells();
			NegativeX.Mark();
			NegativeY.Mark();
			NegativeZ.Mark();
			PositiveX.Mark();
			PositiveY.Mark();
			PositiveZ.Mark();
			for (int i = 0; i < shells.Count; i++)
			{
				Shell shell = shells[i];
				long detail = shell.Detail;
				NegativeX.Write(new SgtBoundsL(-detail, -detail, -detail, -detail + 1, detail, detail), shell, 0, i);
				NegativeY.Write(new SgtBoundsL(-detail, -detail, -detail, detail, -detail + 1, detail), shell, 1, i);
				NegativeZ.Write(new SgtBoundsL(-detail, -detail, -detail, detail, detail, -detail + 1), shell, 2, i);
				PositiveX.Write(new SgtBoundsL(detail - 1, -detail, -detail, detail, detail, detail), shell, 3, i);
				PositiveY.Write(new SgtBoundsL(-detail, detail - 1, -detail, detail, detail, detail), shell, 4, i);
				PositiveZ.Write(new SgtBoundsL(-detail, -detail, detail - 1, detail, detail, detail), shell, 5, i);
			}
			NegativeX.Sweep();
			NegativeY.Sweep();
			NegativeZ.Sweep();
			PositiveX.Sweep();
			PositiveY.Sweep();
			PositiveZ.Sweep();
		}
	}

	public Bounds GetBounds()
	{
		Vector3 zero = Vector3.zero;
		Vector3 size = new Vector3(Radius * 2f, Radius * 2f, Radius * 2f);
		return new Bounds(zero, size);
	}

	public Bounds GetBounds(Camera camera)
	{
		Vector3 center = Vector3.zero;
		if (BoundsOffset != 0f)
		{
			center = transform.InverseTransformPoint(camera.transform.position).normalized * BoundsOffset;
		}
		Vector3 size = new Vector3(Radius * 2f, Radius * 2f, Radius * 2f);
		return new Bounds(center, size);
	}

	public static SgtVector3D Cube(SgtVector3D v)
	{
		return v.normalized;
	}

	public static SgtVector3D InvCube(SgtVector3D v)
	{
		SgtVector3D sgtVector3D = new SgtVector3D(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z));
		if (sgtVector3D.x > sgtVector3D.y && sgtVector3D.x > sgtVector3D.z)
		{
			return v / sgtVector3D.x;
		}
		if (sgtVector3D.y > sgtVector3D.x && sgtVector3D.y > sgtVector3D.z)
		{
			return v / sgtVector3D.y;
		}
		return v / sgtVector3D.z;
	}

	public static SgtTerrain CreateTerrain(int layer = 0, Transform parent = null)
	{
		return CreateTerrain(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtTerrain CreateTerrain(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Terrain", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtTerrain>();
	}

	private void ClearShells()
	{
		for (int num = shells.Count - 1; num >= 0; num--)
		{
			SgtClassPool<Shell>.Add(shells[num]);
		}
		shells.Clear();
	}

	private Shell AddShell(long detail)
	{
		Shell shell = SgtClassPool<Shell>.Pop() ?? new Shell();
		shell.Detail = detail;
		shell.Inner = default;
		shells.Add(shell);
		return shell;
	}

	private long GetSize(double position, double distance, long detail)
	{
		position = Math.Abs(position) - 1.0;
		position /= distance;
		if (position >= 1.0)
		{
			return 0L;
		}
		position = position * position * position * position;
		position = Math.Cos(position * Math.PI / 2.0);
		return (long)Math.Ceiling(distance * (double)detail * position);
	}

	private void UpdateShells()
	{
		ClearShells();
		if (!(Target != null) || Detail <= 0)
		{
			return;
		}
		SgtVector3D vector = CalculateTarget();
		long num = Detail;
		Shell shell = AddShell(num);
		shell.Limit = num * 2;
		shell.Outer = new SgtBoundsL(-num, -num, -num, num, num, num);
		if (Distances == null)
		{
			return;
		}
		double num2 = double.PositiveInfinity;
		int num3 = CalculateAxis(vector);
		for (int i = 0; i < Distances.Count; i++)
		{
			double num4 = Distances[i];
			if (num4 > 0.0 && num4 < num2)
			{
				long num5 = (long)Math.Ceiling(num4 * (double)num);
				long num6 = (long)(vector.x * (double)num);
				long num7 = (long)(vector.y * (double)num);
				long num8 = (long)(vector.z * (double)num);
				long num9 = num5;
				long num10 = num5;
				long num11 = num5;
				switch (num3)
				{
				case 0:
					num10 = (num11 = GetSize(vector.x, num4, num));
					break;
				case 1:
					num9 = (num11 = GetSize(vector.y, num4, num));
					break;
				case 2:
					num9 = (num10 = GetSize(vector.z, num4, num));
					break;
				}
				shell.Inner = new SgtBoundsL(num6 - num9, num7 - num10, num8 - num11, num6 + num9, num7 + num10, num8 + num11);
				num *= 2;
				num5 *= 2;
				Shell shell2 = AddShell(num);
				shell2.Limit = Math.Min(num5 * 2, num * 2);
				shell2.Outer = shell.Inner.Double;
				shell = shell2;
				num2 = num4;
			}
		}
	}

	protected virtual void OnEnable()
	{
		AllTerrains.Add(this);
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(PreRender));
		if (NegativeX != null)
		{
			NegativeX.gameObject.SetActive(value: true);
		}
		if (NegativeY != null)
		{
			NegativeY.gameObject.SetActive(value: true);
		}
		if (NegativeZ != null)
		{
			NegativeZ.gameObject.SetActive(value: true);
		}
		if (PositiveX != null)
		{
			PositiveX.gameObject.SetActive(value: true);
		}
		if (PositiveY != null)
		{
			PositiveY.gameObject.SetActive(value: true);
		}
		if (PositiveZ != null)
		{
			PositiveZ.gameObject.SetActive(value: true);
		}
		UpdateMaterials();
	}

	protected virtual void Update()
	{
		UpdateMeshes();
	}

	protected virtual void OnDisable()
	{
		AllTerrains.Remove(this);
		Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(PreRender));
		if (NegativeX != null)
		{
			NegativeX.gameObject.SetActive(value: false);
		}
		if (NegativeY != null)
		{
			NegativeY.gameObject.SetActive(value: false);
		}
		if (NegativeZ != null)
		{
			NegativeZ.gameObject.SetActive(value: false);
		}
		if (PositiveX != null)
		{
			PositiveX.gameObject.SetActive(value: false);
		}
		if (PositiveY != null)
		{
			PositiveY.gameObject.SetActive(value: false);
		}
		if (PositiveZ != null)
		{
			PositiveZ.gameObject.SetActive(value: false);
		}
	}

	protected virtual void OnDestroy()
	{
		SgtTerrainFace.MarkForDestruction(NegativeX);
		SgtTerrainFace.MarkForDestruction(NegativeY);
		SgtTerrainFace.MarkForDestruction(NegativeZ);
		SgtTerrainFace.MarkForDestruction(PositiveX);
		SgtTerrainFace.MarkForDestruction(PositiveY);
		SgtTerrainFace.MarkForDestruction(PositiveZ);
	}

	private void PreRender(Camera camera)
	{
		Bounds bounds = GetBounds(camera);
		if (NegativeX != null)
		{
			NegativeX.UpdateBounds(bounds);
		}
		if (NegativeY != null)
		{
			NegativeY.UpdateBounds(bounds);
		}
		if (NegativeZ != null)
		{
			NegativeZ.UpdateBounds(bounds);
		}
		if (PositiveX != null)
		{
			PositiveX.UpdateBounds(bounds);
		}
		if (PositiveY != null)
		{
			PositiveY.UpdateBounds(bounds);
		}
		if (PositiveZ != null)
		{
			PositiveZ.UpdateBounds(bounds);
		}
	}

	private SgtTerrainFace CreateFace(CubemapFace side)
	{
		SgtTerrainFace sgtTerrainFace = SgtTerrainFace.Create(side.ToString(), gameObject.layer, transform);
		sgtTerrainFace.Terrain = this;
		sgtTerrainFace.Side = side;
		return sgtTerrainFace;
	}

	private int CalculateAxis(SgtVector3D vector)
	{
		vector.x = Math.Abs(vector.x);
		vector.y = Math.Abs(vector.y);
		vector.z = Math.Abs(vector.z);
		if (vector.y > vector.x && vector.y > vector.z)
		{
			return 1;
		}
		if (vector.z > vector.x && vector.z > vector.y)
		{
			return 2;
		}
		return 0;
	}
}
