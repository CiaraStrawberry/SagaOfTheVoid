using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Flare Mesh")]
public class SgtFlareMesh : MonoBehaviour
{
	[Tooltip("The flare this mesh will be applied to")]
	public SgtFlare Flare;

	[Tooltip("The amount of points used to make the flare mesh")]
	public int Detail = 512;

	[Tooltip("The base radius of the flare in local space")]
	public float Radius = 2f;

	[Tooltip("Deform the flare based on cosine wave?")]
	public bool Wave;

	[Tooltip("The strength of the wave in local space")]
	public float WaveStrength = 5f;

	[Tooltip("The amount of wave peaks")]
	public int WavePoints = 4;

	[Tooltip("The sharpness of the waves")]
	public float WavePower = 5f;

	[Tooltip("The angle offset of the waves")]
	public float WavePhase;

	[Tooltip("Deform the flare based on noise?")]
	public bool Noise;

	[Tooltip("The strength of the noise in local space")]
	public float NoiseStrength = 5f;

	[Tooltip("The amount of noise points")]
	public int NoisePoints = 50;

	[Tooltip("The sharpness of the noise")]
	public float NoisePower = 5f;

	[Tooltip("The angle offset of the noise")]
	public float NoisePhase;

	[Tooltip("The random seed used for the random noise")]
	[SgtSeed]
	public int NoiseSeed;

	[NonSerialized]
	private Mesh generatedMesh;

	[SerializeField]
	[HideInInspector]
	private bool startCalled;

	private static List<float> points = new List<float>();

	public Mesh GeneratedMesh => generatedMesh;

	[ContextMenu("Update Mesh")]
	public void UpdateMesh()
	{
		if (Detail <= 2)
		{
			return;
		}
		if (generatedMesh == null)
		{
			generatedMesh = SgtHelper.CreateTempMesh("Flare Mesh (Generated)");
			UpdateApply();
		}
		int num = Detail + 1;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[Detail * 3];
		float num2 = (float)Math.PI * 2f / (float)Detail;
		float num3 = 0f;
		if (Noise && NoisePoints > 0)
		{
			SgtHelper.BeginRandomSeed(NoiseSeed);
			points.Clear();
			for (int i = 0; i < NoisePoints; i++)
			{
				points.Add(UnityEngine.Random.value);
			}
			num3 = (float)NoisePoints / (float)Detail;
			SgtHelper.EndRandomSeed();
		}
		for (int j = 0; j < Detail; j++)
		{
			int num4 = j + 1;
			float num5 = num2 * (float)j;
			float num6 = Mathf.Sin(num5);
			float num7 = Mathf.Cos(num5);
			float num8 = Radius;
			if (Wave)
			{
				float f = (num5 + WavePhase * ((float)Math.PI / 180f)) * (float)WavePoints;
				num8 += Mathf.Pow(Mathf.Cos(f) * 0.5f + 0.5f, WavePower * WavePower) * WaveStrength;
			}
			if (Noise && NoisePoints > 0)
			{
				float num9 = (float)j * num3;
				int num10 = (int)num9;
				float t = num9 % 1f;
				float a = points[num10 % NoisePoints];
				float b = points[(num10 + 1) % NoisePoints];
				float c = points[(num10 + 2) % NoisePoints];
				float d = points[(num10 + 3) % NoisePoints];
				num8 += SgtHelper.CubicInterpolate(a, b, c, d, t) * NoiseStrength;
			}
			ref Vector3 reference = ref array[num4];
			reference = new Vector3(num6 * num8, num7 * num8, 0f);
			ref Vector2 reference2 = ref array2[num4];
			reference2 = new Vector2(1f, 0f);
		}
		for (int k = 0; k < Detail; k++)
		{
			int num11 = k * 3;
			int num12 = k + 1;
			int num13 = k + 2;
			if (num13 >= num)
			{
				num13 = 1;
			}
			array3[num11] = 0;
			array3[num11 + 1] = num12;
			array3[num11 + 2] = num13;
		}
		generatedMesh.Clear(keepVertexLayout: false);
		generatedMesh.vertices = array;
		generatedMesh.uv = array2;
		generatedMesh.triangles = array3;
		generatedMesh.RecalculateNormals();
		generatedMesh.RecalculateBounds();
	}

	[ContextMenu("Update Apply")]
	public void UpdateApply()
	{
		if (Flare != null)
		{
			Flare.Mesh = generatedMesh;
			Flare.UpdateMesh();
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
			if (Flare == null)
			{
				Flare = GetComponent<SgtFlare>();
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
