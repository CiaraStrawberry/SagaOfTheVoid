using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class ParticleStateC
{
	private Color32[] color;

	private Vector3[] position;

	private Vector3[] normals;

	public Texture2D stateTexture;

	public Texture2D stateDepthmap;

	public float stateDepthmapStrength = 1f;

	public Mesh stateMesh;

	public string stateName;

	public float stateScale = 1f;

	public Vector3 stateOffset;

	public Transform stateTransform;

	public Matrix4x4 stateTransformMx = default;

	public ScaleMethod stateScaleMethod;

	[NonSerialized]
	public bool initialized;

	[HideInInspector]
	public int colorLength;

	[HideInInspector]
	public int positionLength;

	[HideInInspector]
	public bool applyChromaKey;

	[HideInInspector]
	public Color32 chromaKey;

	[HideInInspector]
	public float chromaKeySpread;

	private bool isInitializing;

	public bool IsInitializing()
	{
		return isInitializing;
	}

	public void Initialize()
	{
		isInitializing = true;
		if (stateMesh == null && stateTexture != null)
		{
			ConstructParticles(stateTexture, stateScale, stateOffset, stateName, stateTransform);
		}
		else if (stateMesh != null && stateTexture != null)
		{
			ConstructParticles(stateMesh, stateTexture, stateScale, stateOffset, stateName, stateTransform);
		}
		else if (stateMesh != null && stateTexture == null)
		{
			ConstructParticles(stateMesh, stateScale, stateOffset, stateName, stateTransform);
		}
		else
		{
			Debug.Log("State Texture or State Mesh returned null. Please assign an object to State: " + stateName + ".");
		}
		initialized = position != null && position.Length > 0;
		isInitializing = false;
	}

	public void UpdateMatrix(bool isLocal)
	{
		stateTransformMx.SetTRS((!isLocal) ? stateTransform.position : Vector3.zero, (!isLocal) ? stateTransform.rotation : Quaternion.identity, (stateScaleMethod != ScaleMethod.Local) ? stateTransform.lossyScale : stateTransform.localScale);
	}

	public void ConstructParticles(Texture2D image, float scale, Vector3 offset, string newStateName, Transform newStateTransform)
	{
		List<Color32> list = new List<Color32>();
		List<Vector3> list2 = new List<Vector3>();
		color = PlaygroundC.GetPixels(image);
		bool flag = false;
		if ((bool)PlaygroundC.reference)
		{
			flag = PlaygroundC.reference.buildZeroAlphaPixels;
		}
		bool flag2 = stateDepthmap != null;
		Color32[] array = ((!flag2) ? null : PlaygroundC.GetPixels(stateDepthmap));
		int num = 0;
		int num2 = 0;
		float num3 = ((!(stateDepthmap != null)) ? 0f : ((float)array.Length * 1f / ((float)color.Length * 1f)));
		byte b = (byte)(chromaKeySpread * 255f);
		for (int i = 0; i < color.Length; i++)
		{
			if ((flag || color[i].a != 0) && (!applyChromaKey || (applyChromaKey && (color[i].r < chromaKey.r - b || color[i].r > chromaKey.r + b) && (color[i].g < chromaKey.g - b || color[i].g > chromaKey.g + b) && (color[i].b < chromaKey.b - b || color[i].b > chromaKey.b + b))))
			{
				list.Add(color[i]);
				if (flag2)
				{
					int num4 = Mathf.FloorToInt((float)i * 1f * num3);
					float z = ((float)(int)array[num4].r * 1f + (float)(int)array[num4].g * 1f + (float)(int)array[num4].b * 1f) / 3f * ((float)(int)array[num4].a * 1f / 255f) * stateDepthmapStrength / 255f;
					list2.Add(new Vector3(num, num2, z));
				}
				else
				{
					list2.Add(new Vector3(num, num2, 0f));
				}
			}
			num++;
			num %= image.width;
			if (num == 0 && i != 0)
			{
				num2++;
			}
		}
		color = list.ToArray();
		position = list2.ToArray();
		list = null;
		list2 = null;
		normals = new Vector3[position.Length];
		for (int j = 0; j < normals.Length; j++)
		{
			ref Vector3 reference = ref normals[j];
			reference = Vector3.forward;
		}
		stateTransform = newStateTransform;
		stateTexture = image;
		colorLength = color.Length;
		positionLength = position.Length;
		stateScale = scale;
		stateOffset = offset;
		stateName = newStateName;
		initialized = position.Length > 0;
	}

	public void ConstructParticles(Mesh mesh, Texture2D texture, float scale, Vector3 offset, string newStateName, Transform newStateTransform)
	{
		position = mesh.vertices;
		normals = mesh.normals;
		if (normals == null || normals.Length == 0)
		{
			normals = new Vector3[position.Length];
			for (int i = 0; i < normals.Length; i++)
			{
				ref Vector3 reference = ref normals[i];
				reference = Vector3.forward;
			}
		}
		Vector2[] uv = mesh.uv;
		color = new Color32[uv.Length];
		for (int j = 0; j < position.Length; j++)
		{
			ref Color32 reference2 = ref color[j];
			reference2 = texture.GetPixelBilinear(uv[j].x, uv[j].y);
		}
		stateMesh = mesh;
		stateTransform = newStateTransform;
		colorLength = color.Length;
		positionLength = position.Length;
		stateScale = scale;
		stateOffset = offset;
		stateName = newStateName;
		initialized = position.Length > 0;
	}

	public void ConstructParticles(Mesh mesh, float scale, Vector3 offset, string newStateName, Transform newStateTransform)
	{
		position = mesh.vertices;
		normals = mesh.normals;
		if (normals == null || normals.Length == 0)
		{
			normals = new Vector3[position.Length];
			for (int i = 0; i < normals.Length; i++)
			{
				ref Vector3 reference = ref normals[i];
				reference = Vector3.forward;
			}
		}
		Vector2[] uv = mesh.uv;
		color = new Color32[uv.Length];
		stateMesh = mesh;
		stateTransform = newStateTransform;
		colorLength = color.Length;
		positionLength = position.Length;
		stateScale = scale;
		stateOffset = offset;
		stateName = newStateName;
		initialized = position.Length > 0;
	}

	public Color32 GetColor(int index)
	{
		if (colorLength == 0)
		{
			return default;
		}
		index %= colorLength;
		return color[index];
	}

	public Vector3 GetPosition(int index)
	{
		if (positionLength == 0)
		{
			return default;
		}
		index %= positionLength;
		return (position[index] + stateOffset) * stateScale;
	}

	public Vector3 GetLocalPosition(int index)
	{
		if (positionLength == 0)
		{
			return default;
		}
		index %= positionLength;
		return stateTransformMx.MultiplyPoint3x4((position[index] + stateOffset) * stateScale);
	}

	public Vector3 GetNormal(int index)
	{
		if (positionLength == 0)
		{
			return default;
		}
		index %= positionLength;
		return normals[index];
	}

	public Vector3 GetLocalNormal(int index)
	{
		if (positionLength == 0)
		{
			return default;
		}
		index %= positionLength;
		return stateTransformMx.MultiplyPoint3x4(normals[index]);
	}

	public Color32[] GetColors()
	{
		return color.Clone() as Color32[];
	}

	public Vector3[] GetPositions()
	{
		return position.Clone() as Vector3[];
	}

	public Vector3[] GetNormals()
	{
		return normals.Clone() as Vector3[];
	}

	public void SetColor(int index, Color32 c)
	{
		color[index] = c;
	}

	public void SetPosition(int index, Vector3 v)
	{
		position[index] = v;
	}

	public void SetNormal(int index, Vector3 v)
	{
		normals[index] = v;
	}

	public Vector3 GetParentedPosition(int thisPosition)
	{
		thisPosition %= positionLength;
		return stateTransform.TransformPoint((position[thisPosition] + stateOffset) * stateScale);
	}

	public ParticleStateC Clone()
	{
		ParticleStateC particleStateC = new ParticleStateC();
		particleStateC.stateName = stateName;
		particleStateC.stateScale = stateScale;
		particleStateC.stateTexture = stateTexture;
		particleStateC.stateDepthmap = stateDepthmap;
		particleStateC.stateDepthmapStrength = stateDepthmapStrength;
		particleStateC.stateMesh = stateMesh;
		particleStateC.stateOffset = stateOffset;
		particleStateC.colorLength = colorLength;
		particleStateC.positionLength = positionLength;
		particleStateC.stateTransform = stateTransform;
		particleStateC.applyChromaKey = applyChromaKey;
		particleStateC.chromaKey = chromaKey;
		particleStateC.chromaKeySpread = chromaKeySpread;
		return particleStateC;
	}
}
