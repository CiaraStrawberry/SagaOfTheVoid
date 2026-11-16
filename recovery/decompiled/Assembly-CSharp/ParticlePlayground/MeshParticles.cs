using System.Collections.Generic;
using UnityEngine;

namespace ParticlePlayground;

public class MeshParticles : PlaygroundParticlesC
{
	public static PlaygroundParticlesC CreateMeshParticles(Mesh[] meshes, Texture2D[] textures, Texture2D[] heightMap, string name, Vector3 position, Quaternion rotation, float particleScale, Vector3[] offsets, Material material)
	{
		PlaygroundParticlesC playgroundParticlesC = PlaygroundParticlesC.CreateParticleObject(name, position, rotation, particleScale, material);
		playgroundParticlesC.states = new List<ParticleStateC>();
		int[] array = new int[meshes.Length];
		for (int i = 0; i < textures.Length; i++)
		{
			array[i] = meshes[i].vertexCount;
		}
		playgroundParticlesC.particleCache = new ParticleSystem.Particle[array[PlaygroundC.Largest(array)]];
		playgroundParticlesC.shurikenParticleSystem.Emit(playgroundParticlesC.particleCache.Length);
		playgroundParticlesC.shurikenParticleSystem.GetParticles(playgroundParticlesC.particleCache);
		for (int i = 0; i < meshes.Length; i++)
		{
			playgroundParticlesC.states.Add(new ParticleStateC());
			playgroundParticlesC.states[0].ConstructParticles(meshes[i], textures[i], particleScale, offsets[i], "State " + i, null);
		}
		playgroundParticlesC.UpdateSystem();
		PlaygroundC.particlesQuantity++;
		return playgroundParticlesC;
	}

	public static void Add(PlaygroundParticlesC meshParticles, Mesh mesh, float scale, Vector3 offset, string stateName, Transform stateTransform)
	{
		meshParticles.states.Add(new ParticleStateC());
		meshParticles.states[meshParticles.states.Count - 1].ConstructParticles(mesh, scale, offset, stateName, stateTransform);
	}

	public static void Add(PlaygroundParticlesC meshParticles, Mesh mesh, Texture2D texture, float scale, Vector3 offset, string stateName, Transform stateTransform)
	{
		meshParticles.states.Add(new ParticleStateC());
		meshParticles.states[meshParticles.states.Count - 1].ConstructParticles(mesh, texture, scale, offset, stateName, stateTransform);
	}
}
