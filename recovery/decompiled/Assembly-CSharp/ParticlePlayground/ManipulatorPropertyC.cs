using System;
using System.Collections.Generic;
using PlaygroundSplines;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class ManipulatorPropertyC
{
	public MANIPULATORPROPERTYTYPEC type;

	public MANIPULATORPROPERTYTRANSITIONC transition;

	public MathManipulatorProperty mathProperty = new MathManipulatorProperty();

	public Vector3 velocity;

	public Vector3 localVelocity;

	public Color color = new Color(1f, 1f, 1f, 1f);

	public Gradient lifetimeColor;

	public float size = 1f;

	public List<PlaygroundTransformC> targets = new List<PlaygroundTransformC>();

	public int targetPointer;

	public WorldObject meshTarget = new WorldObject();

	public SkinnedWorldObject skinnedMeshTarget = new SkinnedWorldObject();

	public bool meshTargetIsProcedural;

	public Matrix4x4 meshTargetMatrix = default;

	public ParticleStateC stateTarget = new ParticleStateC();

	public PlaygroundSpline splineTarget;

	public SPLINETARGETMETHOD splineTargetMethod;

	public float splineTimeOffset;

	private bool splineTargetIsReady;

	public bool useLocalRotation;

	public bool onlyColorInRange = true;

	public bool keepColorAlphas = true;

	public bool onlyPositionInRange = true;

	public bool onlySizeInRange;

	public float zeroVelocityStrength = 1f;

	public float strength = 1f;

	public bool unfolded = true;

	public TARGETSORTINGC targetSorting;

	[NonSerialized]
	public int[] targetSortingList;

	private TARGETSORTINGC previousTargetSorting;

	public TURBULENCETYPE turbulenceType = TURBULENCETYPE.Simplex;

	public float turbulenceTimeScale = 1f;

	public float turbulenceScale = 1f;

	public bool turbulenceApplyLifetimeStrength;

	public AnimationCurve turbulenceLifetimeStrength;

	public SimplexNoise turbulenceSimplex;

	public void Update()
	{
		if (type == MANIPULATORPROPERTYTYPEC.Target)
		{
			for (int i = 0; i < targets.Count; i++)
			{
				targets[i].Update();
			}
		}
		else if (type == MANIPULATORPROPERTYTYPEC.MeshTarget)
		{
			if (!(meshTarget.gameObject != null))
			{
				return;
			}
			if (meshTarget.cachedId != meshTarget.gameObject.GetInstanceID())
			{
				meshTarget = PlaygroundParticlesC.NewWorldObject(meshTarget.gameObject.transform);
			}
			if (meshTarget.mesh != null)
			{
				meshTargetMatrix.SetTRS(meshTarget.transform.position, meshTarget.transform.rotation, meshTarget.transform.localScale);
				if (meshTargetIsProcedural)
				{
					UpdateMeshTarget();
				}
			}
			if (meshTarget.initialized && (targetSortingList == null || targetSortingList.Length != meshTarget.vertexPositions.Length || previousTargetSorting != targetSorting))
			{
				TargetSorting();
			}
		}
		else if (type == MANIPULATORPROPERTYTYPEC.SkinnedMeshTarget)
		{
			if (!(skinnedMeshTarget.gameObject != null))
			{
				return;
			}
			if (skinnedMeshTarget.cachedId != skinnedMeshTarget.gameObject.GetInstanceID() || !skinnedMeshTarget.initialized)
			{
				skinnedMeshTarget = PlaygroundParticlesC.NewSkinnedWorldObject(skinnedMeshTarget.gameObject.transform);
			}
			if (skinnedMeshTarget.initialized)
			{
				if (meshTargetIsProcedural)
				{
					UpdateSkinnedMeshTarget();
				}
				skinnedMeshTarget.BoneUpdate();
				PlaygroundC.RunAsync(() =>
				{
					skinnedMeshTarget.Update();
				});
				if (targetSortingList == null || targetSortingList.Length != skinnedMeshTarget.vertexPositions.Length || previousTargetSorting != targetSorting)
				{
					TargetSorting();
				}
			}
		}
		else if (type == MANIPULATORPROPERTYTYPEC.StateTarget)
		{
			if (!stateTarget.initialized && (stateTarget.stateMesh != null || stateTarget.stateTexture != null) && stateTarget.stateTransform != null)
			{
				stateTarget.Initialize();
			}
			else if (stateTarget.initialized && stateTarget.stateTransform != null)
			{
				stateTarget.stateTransformMx.SetTRS(stateTarget.stateTransform.position, stateTarget.stateTransform.rotation, stateTarget.stateTransform.localScale);
			}
			if (stateTarget.initialized && (targetSortingList == null || targetSortingList.Length != stateTarget.positionLength || previousTargetSorting != targetSorting))
			{
				TargetSorting();
			}
		}
		else if (type == MANIPULATORPROPERTYTYPEC.SplineTarget)
		{
			splineTargetIsReady = splineTarget != null && splineTarget.IsReady();
		}
		else if (type == MANIPULATORPROPERTYTYPEC.Turbulence)
		{
			if (turbulenceSimplex == null)
			{
				turbulenceSimplex = new SimplexNoise();
			}
		}
		else if (type != MANIPULATORPROPERTYTYPEC.Math)
		{
		}
	}

	public void UpdateMathProperty(Vector3 manipulatorPosition)
	{
		if (mathProperty == null)
		{
			mathProperty = new MathManipulatorProperty();
		}
		mathProperty.Update(manipulatorPosition);
	}

	public void UpdateMeshTarget()
	{
		meshTarget.Update();
	}

	public void UpdateSkinnedMeshTarget()
	{
		skinnedMeshTarget.Update();
	}

	public void SetMeshTarget(GameObject gameObject)
	{
		meshTarget.gameObject = gameObject;
	}

	public void SetSkinnedMeshTarget(GameObject gameObject)
	{
		skinnedMeshTarget.gameObject = gameObject;
	}

	public bool SplineTargetIsReady()
	{
		return splineTargetIsReady;
	}

	public void TargetSorting()
	{
		if ((type == MANIPULATORPROPERTYTYPEC.MeshTarget && !meshTarget.initialized) || (type == MANIPULATORPROPERTYTYPEC.SkinnedMeshTarget && !skinnedMeshTarget.initialized) || (type == MANIPULATORPROPERTYTYPEC.StateTarget && !stateTarget.initialized))
		{
			return;
		}
		targetPointer = 0;
		if (type == MANIPULATORPROPERTYTYPEC.MeshTarget)
		{
			targetSortingList = new int[meshTarget.vertexPositions.Length];
		}
		else if (type == MANIPULATORPROPERTYTYPEC.SkinnedMeshTarget)
		{
			targetSortingList = new int[skinnedMeshTarget.vertexPositions.Length];
		}
		else if (type == MANIPULATORPROPERTYTYPEC.StateTarget)
		{
			targetSortingList = new int[stateTarget.positionLength];
		}
		switch (targetSorting)
		{
		case TARGETSORTINGC.Scrambled:
		{
			for (int i = 0; i < targetSortingList.Length; i++)
			{
				targetSortingList[i] = i;
			}
			PlaygroundC.ShuffleArray(targetSortingList);
			break;
		}
		case TARGETSORTINGC.Linear:
		{
			for (int j = 0; j < targetSortingList.Length; j++)
			{
				targetSortingList[j] = j;
			}
			break;
		}
		case TARGETSORTINGC.Reversed:
		{
			int num = 0;
			for (int num2 = targetSortingList.Length - 1; num2 > 0; num2--)
			{
				targetSortingList[num] = num2;
				num++;
			}
			break;
		}
		}
		previousTargetSorting = targetSorting;
	}

	public void SetLocalVelocity(Quaternion rotation, Vector3 newVelocity)
	{
		localVelocity = rotation * newVelocity;
	}

	public void SetLocalTargetsPosition(Transform otherTransform)
	{
		for (int i = 0; i < targets.Count; i++)
		{
			targets[i].SetLocalPosition(otherTransform);
		}
	}

	public bool UpdateTarget(int index)
	{
		if (targets.Count > 0)
		{
			index %= targets.Count;
			targets[index].Update();
		}
		return targets[index].available;
	}

	public ManipulatorPropertyC Clone()
	{
		ManipulatorPropertyC manipulatorPropertyC = new ManipulatorPropertyC();
		manipulatorPropertyC.type = type;
		manipulatorPropertyC.transition = transition;
		manipulatorPropertyC.velocity = velocity;
		manipulatorPropertyC.color = color;
		manipulatorPropertyC.lifetimeColor = lifetimeColor;
		manipulatorPropertyC.size = size;
		manipulatorPropertyC.useLocalRotation = useLocalRotation;
		manipulatorPropertyC.onlyColorInRange = onlyColorInRange;
		manipulatorPropertyC.keepColorAlphas = keepColorAlphas;
		manipulatorPropertyC.onlyPositionInRange = onlyPositionInRange;
		manipulatorPropertyC.zeroVelocityStrength = zeroVelocityStrength;
		manipulatorPropertyC.strength = strength;
		manipulatorPropertyC.targetPointer = targetPointer;
		manipulatorPropertyC.targets = new List<PlaygroundTransformC>();
		for (int i = 0; i < targets.Count; i++)
		{
			manipulatorPropertyC.targets.Add(targets[i].Clone());
		}
		manipulatorPropertyC.meshTarget = meshTarget.Clone();
		manipulatorPropertyC.skinnedMeshTarget = skinnedMeshTarget.Clone();
		manipulatorPropertyC.stateTarget = stateTarget.Clone();
		manipulatorPropertyC.turbulenceType = turbulenceType;
		manipulatorPropertyC.turbulenceApplyLifetimeStrength = turbulenceApplyLifetimeStrength;
		manipulatorPropertyC.turbulenceScale = turbulenceScale;
		manipulatorPropertyC.turbulenceTimeScale = turbulenceTimeScale;
		manipulatorPropertyC.turbulenceLifetimeStrength = new AnimationCurve(turbulenceLifetimeStrength.keys);
		return manipulatorPropertyC;
	}
}
