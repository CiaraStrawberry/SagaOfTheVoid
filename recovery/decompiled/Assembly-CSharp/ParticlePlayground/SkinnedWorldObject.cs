using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class SkinnedWorldObject : WorldObjectBaseC
{
	public int downResolution = 1;

	[HideInInspector]
	public SkinnedMeshRenderer renderer;

	private Transform[] boneTransforms;

	private BoneWeight[] weights;

	private Matrix4x4[] bindPoses;

	private Matrix4x4[] boneMatrices;

	private Vector3[] vertices;

	private Vector3[] localVertices;

	[HideInInspector]
	public bool isDoneThread = true;

	private Mesh bakedMesh;

	private Matrix4x4 vertexMatrix = default;

	private Action _updateAction;

	public void Initialize()
	{
		gameObject = transform.gameObject;
		cachedId = gameObject.GetInstanceID();
		rigidbody = gameObject.GetComponent<Rigidbody>();
		renderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();
		mesh = renderer.sharedMesh;
		if (mesh != null)
		{
			normals = mesh.normals;
			vertices = new Vector3[mesh.vertices.Length];
			localVertices = mesh.vertices;
			weights = mesh.boneWeights;
			boneTransforms = renderer.bones;
			bindPoses = mesh.bindposes;
			boneMatrices = new Matrix4x4[boneTransforms.Length];
			vertexPositions = vertices;
			initialized = true;
			Update();
		}
		else
		{
			Debug.Log("No mesh could be found in the assigned Skinned World Object. Make sure a mesh is assigned to your Skinned Mesh Renderer, if so - make sure the hierarchy under only have one Skinned Mesh Renderer component.", gameObject);
		}
	}

	public void MeshUpdate()
	{
		if (initialized)
		{
			if (boneMatrices != null && boneMatrices.Length > 0)
			{
				localVertices = mesh.vertices;
			}
			if (updateNormals)
			{
				normals = mesh.normals;
			}
		}
	}

	public void BoneUpdate()
	{
		if (!initialized)
		{
			return;
		}
		if (boneMatrices != null && boneMatrices.Length > 0)
		{
			for (int i = 0; i < boneMatrices.Length; i++)
			{
				if (boneTransforms[i] != null)
				{
					ref Matrix4x4 reference = ref boneMatrices[i];
					reference = boneTransforms[i].localToWorldMatrix * bindPoses[i];
				}
			}
		}
		else
		{
			if (bakedMesh == null)
			{
				bakedMesh = new Mesh();
			}
			renderer.BakeMesh(bakedMesh);
			localVertices = bakedMesh.vertices;
			vertexMatrix.SetTRS(renderer.transform.position, renderer.transform.rotation, renderer.transform.localScale);
		}
	}

	public void Update()
	{
		if (initialized)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				if (boneMatrices.Length > 0)
				{
					Matrix4x4 matrix4x = boneMatrices[weights[i].boneIndex0];
					Matrix4x4 matrix4x2 = boneMatrices[weights[i].boneIndex1];
					Matrix4x4 matrix4x3 = boneMatrices[weights[i].boneIndex2];
					Matrix4x4 matrix4x4 = boneMatrices[weights[i].boneIndex3];
					for (int j = 0; j < 16; j++)
					{
						vertexMatrix[j] = matrix4x[j] * weights[i].weight0 + matrix4x2[j] * weights[i].weight1 + matrix4x3[j] * weights[i].weight2 + matrix4x4[j] * weights[i].weight3;
					}
				}
				ref Vector3 reference = ref vertices[i];
				reference = vertexMatrix.MultiplyPoint3x4(localVertices[i]);
			}
			vertexPositions = vertices;
		}
		isDoneThread = true;
	}

	public void UpdateOnNewThread()
	{
		if (_updateAction == null)
		{
			_updateAction = Update;
		}
		if (isDoneThread)
		{
			isDoneThread = false;
			PlaygroundC.RunAsync(_updateAction);
		}
	}

	public SkinnedWorldObject Clone()
	{
		SkinnedWorldObject skinnedWorldObject = new SkinnedWorldObject();
		skinnedWorldObject.downResolution = downResolution;
		skinnedWorldObject.gameObject = gameObject;
		skinnedWorldObject.transform = transform;
		skinnedWorldObject.rigidbody = rigidbody;
		skinnedWorldObject.renderer = renderer;
		skinnedWorldObject.meshFilter = meshFilter;
		skinnedWorldObject.mesh = mesh;
		skinnedWorldObject.updateNormals = updateNormals;
		return skinnedWorldObject;
	}
}
