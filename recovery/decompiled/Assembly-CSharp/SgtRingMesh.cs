using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Ring Mesh")]
public class SgtRingMesh : MonoBehaviour
{
	[Tooltip("The ring this mesh will be applied to")]
	public SgtRing Ring;

	[Tooltip("The amount of segments this mesh is designed to be split into")]
	public int Segments = 8;

	[Tooltip("The amount of times the ring mesh is sliced around each segment")]
	public int SegmentDetail = 50;

	[Tooltip("The radius of the inner edge of the ring in local coordinates")]
	public float RadiusMin = 1f;

	[Tooltip("The radius of the outer edge of the ring in local coordinates")]
	public float RadiusMax = 2f;

	[Tooltip("The amount of times the ring mesh is sliced between the radius min and max")]
	public int RadiusDetail = 2;

	[Tooltip("The amount the mesh bounds should get pushed out by in local coordinates. This should be used with 8+ Segments")]
	public float BoundsShift;

	[NonSerialized]
	private Mesh generatedMesh;

	[SerializeField]
	[HideInInspector]
	private bool startCalled;

	public Mesh GeneratedMesh => generatedMesh;

	[ContextMenu("Update Mesh")]
	public void UpdateMesh()
	{
		if (Segments <= 0 || SegmentDetail <= 0 || RadiusDetail <= 0)
		{
			return;
		}
		if (generatedMesh == null)
		{
			generatedMesh = SgtHelper.CreateTempMesh("Ring Mesh (Generated)");
			UpdateApply();
		}
		int num = SegmentDetail + 1;
		int num2 = RadiusDetail + 1;
		int num3 = num * num2 * 2;
		Vector3[] array = new Vector3[num3];
		Vector2[] array2 = new Vector2[num3];
		Vector2[] array3 = new Vector2[num3];
		Color[] array4 = new Color[num3];
		int[] array5 = new int[SegmentDetail * RadiusDetail * 6];
		float num4 = (float)Math.PI * 2f / (float)Segments / (float)SegmentDetail;
		float num5 = 1f / (float)SegmentDetail;
		float num6 = 1f / (float)RadiusDetail;
		for (int i = 0; i < num; i++)
		{
			float f = num4 * (float)i;
			float num7 = Mathf.Sin(f);
			float num8 = Mathf.Cos(f);
			for (int j = 0; j < num2; j++)
			{
				int num9 = num2 * i + j;
				float num10 = num5 * (float)i;
				float num11 = num6 * (float)j;
				float num12 = Mathf.Lerp(RadiusMin, RadiusMax, num11);
				ref Vector3 reference = ref array[num9];
				reference = new Vector3(num7 * num12, 0f, num8 * num12);
				ref Color reference2 = ref array4[num9];
				reference2 = new Color(1f, 1f, 1f, 0f);
				ref Vector2 reference3 = ref array2[num9];
				reference3 = new Vector2(num11, num10);
				ref Vector2 reference4 = ref array3[num9];
				reference4 = new Vector2(num12, num10 * num12);
			}
		}
		for (int k = 0; k < SegmentDetail; k++)
		{
			for (int l = 0; l < RadiusDetail; l++)
			{
				int num13 = (k * RadiusDetail + l) * 6;
				int num14 = k * num2 + l;
				int num15 = num14 + num2;
				array5[num13] = num14;
				array5[num13 + 1] = num14 + 1;
				array5[num13 + 2] = num15;
				array5[num13 + 3] = num15 + 1;
				array5[num13 + 4] = num15;
				array5[num13 + 5] = num14 + 1;
			}
		}
		generatedMesh.Clear(keepVertexLayout: false);
		generatedMesh.vertices = array;
		generatedMesh.colors = array4;
		generatedMesh.uv = array2;
		generatedMesh.uv2 = array3;
		generatedMesh.triangles = array5;
		generatedMesh.RecalculateNormals();
		generatedMesh.RecalculateBounds();
		Bounds bounds = generatedMesh.bounds;
		generatedMesh.bounds = SgtHelper.NewBoundsCenter(bounds, bounds.center + bounds.center.normalized * BoundsShift);
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Ring != null && generatedMesh != null && Ring.Mesh != generatedMesh)
		{
			Ring.Mesh = generatedMesh;
			Ring.UpdateMesh();
		}
	}

	protected virtual void OnEnable()
	{
		if (startCalled)
		{
			CheckUpdateCalls();
		}
	}

	protected virtual void Start()
	{
		if (!startCalled)
		{
			startCalled = true;
			if (Ring == null)
			{
				Ring = GetComponent<SgtRing>();
			}
			CheckUpdateCalls();
		}
	}

	protected virtual void OnDestroy()
	{
		if (generatedMesh != null)
		{
			generatedMesh.Clear(keepVertexLayout: false);
			SgtObjectPool<Mesh>.Add(generatedMesh);
		}
	}

	private void CheckUpdateCalls()
	{
		if (generatedMesh == null)
		{
			UpdateMesh();
		}
		UpdateApply();
	}
}
