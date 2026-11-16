using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class PlaygroundSave
{
	[HideInInspector]
	public string name;

	[HideInInspector]
	public float time;

	[HideInInspector]
	public float lifetime;

	[HideInInspector]
	public int particleCount;

	[HideInInspector]
	public float version;

	[HideInInspector]
	public PlaygroundParticlesC settings;

	[HideInInspector]
	public PlaygroundTransformC transform;

	[HideInInspector]
	public Material material;

	[HideInInspector]
	public int loadMode;

	[HideInInspector]
	public bool loadTransform = true;

	[HideInInspector]
	public bool loadMaterial = true;

	[HideInInspector]
	public bool setMaterialAfterTransition = true;

	[HideInInspector]
	public float transitionMultiplier = 1f;

	[HideInInspector]
	public INDIVIDUALTRANSITIONTYPEC transitionType;

	[HideInInspector]
	public bool unfolded;

	private bool isLoading;

	private bool isSaving;

	public bool IsLoading()
	{
		return isLoading;
	}

	public bool IsSaving()
	{
		return isSaving;
	}

	public void Load(PlaygroundParticlesC loadTo)
	{
		if (loadMaterial && (setMaterialAfterTransition || !loadTo.loadTransition) && settings.particleSystemRenderer.sharedMaterial != null)
		{
			loadTo.particleSystemRenderer.sharedMaterial = settings.particleSystemRenderer.sharedMaterial;
		}
		isLoading = true;
		PlaygroundC.RunAsync(() =>
		{
			switch (loadMode)
			{
			case 0:
				settings.CopyTo(loadTo);
				loadTo.playgroundCache = settings.snapshotData.Clone();
				break;
			case 1:
				settings.CopyTo(loadTo);
				break;
			case 2:
				loadTo.playgroundCache = settings.snapshotData.Clone();
				break;
			default:
				settings.CopyTo(loadTo);
				loadTo.playgroundCache = settings.snapshotData.Clone();
				break;
			}
			isLoading = false;
		});
		if (loadTransform)
		{
			transform.GetFromTransform(loadTo.particleSystemTransform);
		}
	}

	public void Save(PlaygroundParticlesC playgroundParticles)
	{
		isSaving = true;
		transform = new PlaygroundTransformC();
		transform.SetFromTransform(playgroundParticles.particleSystemTransform);
		settings.particleSystemRenderer.sharedMaterial = playgroundParticles.particleSystemRenderer.sharedMaterial;
		playgroundParticles.CopyTo(settings);
		settings.snapshotData = playgroundParticles.playgroundCache.Clone();
		isSaving = false;
	}

	public void SaveAsync(PlaygroundParticlesC playgroundParticles)
	{
		isSaving = true;
		transform = new PlaygroundTransformC();
		transform.SetFromTransform(playgroundParticles.particleSystemTransform);
		settings.particleSystemRenderer.sharedMaterial = playgroundParticles.particleSystemRenderer.sharedMaterial;
		PlaygroundC.RunAsync(() =>
		{
			playgroundParticles.CopyTo(settings);
			settings.snapshotData = playgroundParticles.playgroundCache.Clone();
			isSaving = false;
		});
	}

	public PlaygroundSave Clone()
	{
		PlaygroundSave playgroundSave = new PlaygroundSave();
		settings.CopyTo(playgroundSave.settings);
		playgroundSave.name = name;
		playgroundSave.time = time;
		playgroundSave.lifetime = lifetime;
		playgroundSave.particleCount = particleCount;
		playgroundSave.loadMode = loadMode;
		playgroundSave.loadTransform = loadTransform;
		playgroundSave.loadMaterial = loadMaterial;
		playgroundSave.setMaterialAfterTransition = setMaterialAfterTransition;
		playgroundSave.material = material;
		playgroundSave.transitionMultiplier = transitionMultiplier;
		playgroundSave.transitionType = transitionType;
		playgroundSave.unfolded = unfolded;
		return playgroundSave;
	}
}
