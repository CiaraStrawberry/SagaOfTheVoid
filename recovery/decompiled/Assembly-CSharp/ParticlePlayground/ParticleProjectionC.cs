using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class ParticleProjectionC
{
	[HideInInspector]
	private Color32[] sourceColors;

	[HideInInspector]
	private Vector3[] sourcePositions;

	[HideInInspector]
	private Vector3[] targetPositions;

	[HideInInspector]
	private Vector3[] targetNormals;

	[HideInInspector]
	private bool[] hasProjected;

	[HideInInspector]
	private Transform[] targetParents;

	public Texture2D projectionTexture;

	public Vector2 projectionOrigin;

	public Transform projectionTransform;

	public Matrix4x4 projectionMatrix;

	public Vector3 projectionPosition;

	public Vector3 projectionDirection;

	public Quaternion projectionRotation;

	public float projectionDistance = 1000f;

	public float projectionScale = 0.1f;

	public LayerMask projectionMask = -1;

	public COLLISIONTYPEC collisionType;

	public float minDepth = -1f;

	public float maxDepth = 1f;

	public float surfaceOffset;

	public bool liveUpdate = true;

	public bool hasRefreshed;

	[HideInInspector]
	public bool initialized;

	[HideInInspector]
	public int colorLength;

	[HideInInspector]
	public int positionLength;

	public void Initialize()
	{
		if ((bool)projectionTexture)
		{
			Construct(projectionTexture, projectionTransform);
			if (!liveUpdate)
			{
				UpdateSource();
				Update();
			}
			initialized = true;
			hasRefreshed = false;
		}
	}

	public void Construct(Texture2D image, Transform transform)
	{
		List<Color32> list = new List<Color32>();
		List<Vector3> list2 = new List<Vector3>();
		sourceColors = PlaygroundC.GetPixels(image);
		bool flag = false;
		if ((bool)PlaygroundC.reference)
		{
			flag = PlaygroundC.reference.buildZeroAlphaPixels;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < sourceColors.Length; i++)
		{
			if (flag || sourceColors[i].a != 0)
			{
				list.Add(sourceColors[i]);
				list2.Add(new Vector3((float)num * projectionScale, (float)num2 * projectionScale, 0f));
			}
			num++;
			num %= image.width;
			if (num == 0 && i != 0)
			{
				num2++;
			}
		}
		sourceColors = list.ToArray();
		sourcePositions = list2.ToArray();
		list = null;
		list2 = null;
		targetPositions = new Vector3[sourcePositions.Length];
		targetNormals = new Vector3[sourcePositions.Length];
		targetParents = new Transform[sourcePositions.Length];
		hasProjected = new bool[sourcePositions.Length];
		positionLength = sourcePositions.Length;
		colorLength = sourceColors.Length;
		projectionTexture = image;
		projectionTransform = transform;
		projectionMatrix = default;
	}

	public void UpdateSource()
	{
		projectionPosition = projectionTransform.position;
		projectionDirection = projectionTransform.forward;
		projectionRotation = projectionTransform.rotation;
		projectionMatrix.SetTRS(projectionPosition, projectionRotation, projectionTransform.localScale);
	}

	public void Update()
	{
		for (int i = 0; i < positionLength; i++)
		{
			Update(i);
		}
	}

	public void Update(int index)
	{
		index %= positionLength;
		Vector3 vector = projectionMatrix.MultiplyPoint3x4(sourcePositions[index] + new Vector3(projectionOrigin.x, projectionOrigin.y, 0f));
		if (collisionType == COLLISIONTYPEC.Physics3D)
		{
			if (Physics.Raycast(vector, projectionDirection, out var hitInfo, projectionDistance, projectionMask))
			{
				ref Vector3 reference = ref targetPositions[index];
				reference = hitInfo.point + hitInfo.normal * surfaceOffset;
				ref Vector3 reference2 = ref targetNormals[index];
				reference2 = hitInfo.normal;
				targetParents[index] = hitInfo.transform;
				hasProjected[index] = true;
			}
			else
			{
				ref Vector3 reference3 = ref targetPositions[index];
				reference3 = PlaygroundC.initialTargetPosition;
				ref Vector3 reference4 = ref targetNormals[index];
				reference4 = Vector3.forward;
				hasProjected[index] = false;
				targetParents[index] = null;
			}
			return;
		}
		RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, projectionDirection, projectionDistance, projectionMask, minDepth, maxDepth);
		if (raycastHit2D.collider != null)
		{
			ref Vector3 reference5 = ref targetPositions[index];
			reference5 = raycastHit2D.point + raycastHit2D.normal * surfaceOffset;
			ref Vector3 reference6 = ref targetNormals[index];
			reference6 = raycastHit2D.normal;
			targetParents[index] = raycastHit2D.transform;
			hasProjected[index] = true;
		}
		else
		{
			ref Vector3 reference7 = ref targetPositions[index];
			reference7 = PlaygroundC.initialTargetPosition;
			ref Vector3 reference8 = ref targetNormals[index];
			reference8 = Vector3.forward;
			hasProjected[index] = false;
			targetParents[index] = null;
		}
	}

	public Color32 GetColor(int index)
	{
		index %= colorLength;
		return sourceColors[index];
	}

	public Vector3 GetPosition(int index)
	{
		index %= positionLength;
		return targetPositions[index];
	}

	public Vector3 GetNormal(int index)
	{
		index %= positionLength;
		return targetNormals[index];
	}

	public Transform GetParent(int index)
	{
		index %= positionLength;
		return targetParents[index];
	}

	public bool HasProjection(int index)
	{
		index %= positionLength;
		return hasProjected[index];
	}

	public ParticleProjectionC Clone()
	{
		ParticleProjectionC particleProjectionC = new ParticleProjectionC();
		if (sourceColors != null)
		{
			particleProjectionC.sourceColors = (Color32[])sourceColors.Clone();
		}
		if (sourcePositions != null)
		{
			particleProjectionC.sourcePositions = (Vector3[])sourcePositions.Clone();
		}
		if (targetPositions != null)
		{
			particleProjectionC.targetPositions = (Vector3[])targetPositions.Clone();
		}
		if (targetNormals != null)
		{
			particleProjectionC.targetNormals = (Vector3[])targetNormals.Clone();
		}
		if (hasProjected != null)
		{
			particleProjectionC.hasProjected = (bool[])hasProjected.Clone();
		}
		if (targetParents != null)
		{
			particleProjectionC.targetParents = (Transform[])targetParents.Clone();
		}
		particleProjectionC.projectionTexture = projectionTexture;
		particleProjectionC.projectionOrigin = projectionOrigin;
		particleProjectionC.projectionTransform = projectionTransform;
		particleProjectionC.projectionMatrix = projectionMatrix;
		particleProjectionC.projectionPosition = projectionPosition;
		particleProjectionC.projectionDirection = projectionDirection;
		particleProjectionC.projectionRotation = projectionRotation;
		particleProjectionC.projectionDistance = projectionDistance;
		particleProjectionC.projectionScale = projectionScale;
		particleProjectionC.projectionMask = projectionMask;
		particleProjectionC.collisionType = collisionType;
		particleProjectionC.minDepth = minDepth;
		particleProjectionC.maxDepth = maxDepth;
		particleProjectionC.surfaceOffset = surfaceOffset;
		particleProjectionC.liveUpdate = liveUpdate;
		particleProjectionC.hasRefreshed = hasRefreshed;
		particleProjectionC.initialized = initialized;
		particleProjectionC.colorLength = colorLength;
		particleProjectionC.positionLength = positionLength;
		return particleProjectionC;
	}
}
