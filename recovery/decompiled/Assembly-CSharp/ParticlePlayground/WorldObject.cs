using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class WorldObject : WorldObjectBaseC
{
	[HideInInspector]
	public Renderer renderer;

	public void Initialize()
	{
		gameObject = transform.gameObject;
		rigidbody = gameObject.GetComponent<Rigidbody>();
		renderer = transform.GetComponentInChildren<Renderer>();
		meshFilter = transform.GetComponentInChildren<MeshFilter>();
		if (meshFilter != null)
		{
			mesh = meshFilter.sharedMesh;
			if (mesh != null)
			{
				vertexPositions = mesh.vertices;
				normals = mesh.normals;
				initialized = true;
			}
			else
			{
				initialized = false;
			}
		}
		cachedId = gameObject.GetInstanceID();
	}

	public void Update()
	{
		if (mesh != null)
		{
			vertexPositions = mesh.vertices;
			if (updateNormals)
			{
				normals = mesh.normals;
			}
		}
	}

	public WorldObject Clone()
	{
		WorldObject worldObject = new WorldObject();
		worldObject.gameObject = gameObject;
		worldObject.transform = transform;
		worldObject.rigidbody = rigidbody;
		worldObject.renderer = renderer;
		worldObject.meshFilter = meshFilter;
		worldObject.mesh = mesh;
		worldObject.updateNormals = updateNormals;
		return worldObject;
	}
}
