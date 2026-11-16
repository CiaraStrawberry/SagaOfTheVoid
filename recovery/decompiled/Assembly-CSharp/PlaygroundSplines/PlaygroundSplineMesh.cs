using UnityEngine;

namespace PlaygroundSplines;

[ExecuteInEditMode]
public class PlaygroundSplineMesh : MonoBehaviour
{
	public PlaygroundSpline spline;

	[Range(2f, 1000f)]
	public int points = 100;

	[Range(0.01f, 100f)]
	public float width = 1f;

	public bool noise;

	[Range(0.01f, 100f)]
	public float noiseStrength = 1f;

	public Vector2 noiseScale = new Vector2(1f, 1f);

	public bool noiseDistribution;

	public AnimationCurve noiseDistributionL;

	public AnimationCurve noiseDistributionR;

	public bool followSurface;

	public bool followSurfaceRotation;

	public float surfaceOffset = 0.1f;

	public Vector3 meshUpDirection = Vector3.up;

	public Vector3 surfaceDirection = Vector3.down;

	private int prevPoints;

	private float prevWidth;

	private bool prevNoise;

	private float prevNoiseStrength;

	private Vector2 prevNoiseScale;

	private bool prevNoiseDistribution;

	private bool prevFollowSurface;

	private bool prevFollowSurfaceRotation;

	private float prevSurfaceOffset;

	private Vector3 prevMeshUpDirection;

	private Vector3 prevSurfaceDirection;

	private void OnEnable()
	{
		if (noiseDistributionL == null || noiseDistributionL.keys.Length == 0)
		{
			SetAnimationCurveVals(ref noiseDistributionL);
		}
		if (noiseDistributionR == null || noiseDistributionR.keys.Length == 0)
		{
			SetAnimationCurveVals(ref noiseDistributionR);
		}
		if (GetComponent<Renderer>() == null)
		{
			gameObject.AddComponent<MeshRenderer>();
		}
		if (spline == null)
		{
			spline = GetComponent<PlaygroundSpline>();
		}
		if (spline != null)
		{
			BuildSplineMesh(spline, points, width);
		}
		SetVals();
	}

	private void Update()
	{
		if (NeedsUpdate())
		{
			if (spline != null)
			{
				BuildSplineMesh(spline, points, width);
			}
			SetVals();
		}
	}

	private bool NeedsUpdate()
	{
		return prevPoints != points || prevWidth != width || prevNoise != noise || (noise && (prevNoiseScale != noiseScale || prevNoiseStrength != noiseStrength || prevNoiseDistribution != noiseDistribution)) || prevFollowSurface != followSurface || prevFollowSurfaceRotation != followSurfaceRotation || prevSurfaceOffset != surfaceOffset || prevMeshUpDirection != meshUpDirection || prevSurfaceDirection != surfaceDirection;
	}

	private void SetVals()
	{
		prevPoints = points;
		prevWidth = width;
		prevNoise = noise;
		prevNoiseStrength = noiseStrength;
		prevNoiseScale = noiseScale;
		prevNoiseDistribution = noiseDistribution;
		prevFollowSurface = followSurface;
		prevFollowSurfaceRotation = followSurfaceRotation;
		prevSurfaceOffset = surfaceOffset;
		prevMeshUpDirection = meshUpDirection;
		prevSurfaceDirection = surfaceDirection;
	}

	private void SetAnimationCurveVals(ref AnimationCurve curve)
	{
		Keyframe[] array = new Keyframe[2];
		array[0].time = 0f;
		array[1].time = 1f;
		array[0].value = 1f;
		array[1].value = 1f;
		curve.keys = array;
	}

	public void BuildSplineMesh(PlaygroundSpline spline, int points, float width)
	{
		if (points < 2)
		{
			points = 2;
		}
		int num = points * 2;
		MeshFilter meshFilter = ((!(GetComponent<MeshFilter>() != null)) ? gameObject.AddComponent<MeshFilter>() : GetComponent<MeshFilter>());
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[(points - 1) * 6];
		Vector3 normal = meshUpDirection;
		for (int i = 0; i < points; i++)
		{
			float num2 = (float)i * 1f / (float)(points - 1);
			float num3 = ((float)i * 1f + 1f) / (float)(points - 1);
			if (num2 >= 1f && !spline.Loop)
			{
				num2 = 0.9999f;
			}
			if (num3 >= 1f && !spline.Loop)
			{
				num3 = 0.99999f;
			}
			Vector3 vector = spline.GetPoint(num2);
			Vector3 vector2 = spline.GetPoint(num3);
			if (followSurface || followSurfaceRotation)
			{
				if (Physics.Raycast(vector, surfaceDirection, out var hitInfo))
				{
					if (followSurfaceRotation)
					{
						normal = hitInfo.normal;
					}
					if (followSurface)
					{
						vector = hitInfo.point + hitInfo.normal * surfaceOffset;
					}
				}
				if (followSurface && Physics.Raycast(vector2, surfaceDirection, out hitInfo))
				{
					vector2 = hitInfo.point + hitInfo.normal * surfaceOffset;
				}
			}
			float num4 = ((!noise) ? 0f : (Mathf.PerlinNoise(num2 % 1f * noiseScale.x, 0f) * noiseStrength));
			float num5 = ((!noise) ? 0f : (Mathf.PerlinNoise(num2 % 1f * noiseScale.y, 0f) * noiseStrength));
			if (noise && noiseDistribution)
			{
				num4 *= noiseDistributionL.Evaluate(num2);
				num5 *= noiseDistributionR.Evaluate(num2);
			}
			Vector3 normalized = Vector3.Cross(normal, vector2 - vector).normalized;
			Vector3 vector3 = vector + normalized * (width / 2f + num4);
			Vector3 vector4 = vector - normalized * (width / 2f + num5);
			Debug.DrawLine(vector3, vector4);
			array[i * 2] = vector3;
			array[i * 2 + 1] = vector4;
			ref Vector2 reference = ref array2[i * 2];
			reference = new Vector2(num2, 0f);
			ref Vector2 reference2 = ref array2[i * 2 + 1];
			reference2 = new Vector2(num2, 1f);
			if (i > 0)
			{
				int num6 = (i - 1) * 6;
				int num7 = i * 2;
				array3[num6] = num7 - 2;
				array3[num6 + 1] = num7 - 1;
				array3[num6 + 2] = num7;
				array3[num6 + 3] = num7;
				array3[num6 + 4] = num7 - 1;
				array3[num6 + 5] = num7 + 1;
			}
		}
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;
	}
}
