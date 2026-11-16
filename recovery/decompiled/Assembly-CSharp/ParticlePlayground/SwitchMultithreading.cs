using UnityEngine;

namespace ParticlePlayground;

public class SwitchMultithreading : MonoBehaviour
{
	public static void NextParticleSystemMethod()
	{
		switch (PlaygroundC.reference.threadMethod)
		{
		case ThreadMethod.Automatic:
			PlaygroundC.reference.threadMethod = ThreadMethod.NoThreads;
			break;
		case ThreadMethod.NoThreads:
			PlaygroundC.reference.threadMethod = ThreadMethod.OnePerSystem;
			break;
		case ThreadMethod.OnePerSystem:
			PlaygroundC.reference.threadMethod = ThreadMethod.OneForAll;
			break;
		case ThreadMethod.OneForAll:
			PlaygroundC.reference.threadMethod = ThreadMethod.Automatic;
			break;
		}
	}

	public static void NextSkinnedMeshMethod()
	{
		switch (PlaygroundC.reference.skinnedMeshThreadMethod)
		{
		case ThreadMethodComponent.InsideParticleCalculation:
			PlaygroundC.reference.skinnedMeshThreadMethod = ThreadMethodComponent.OnePerSystem;
			break;
		case ThreadMethodComponent.OnePerSystem:
			PlaygroundC.reference.skinnedMeshThreadMethod = ThreadMethodComponent.OneForAll;
			break;
		case ThreadMethodComponent.OneForAll:
			PlaygroundC.reference.skinnedMeshThreadMethod = ThreadMethodComponent.InsideParticleCalculation;
			break;
		}
	}

	public static void NextTurbulenceMethod()
	{
		switch (PlaygroundC.reference.turbulenceThreadMethod)
		{
		case ThreadMethodComponent.InsideParticleCalculation:
			PlaygroundC.reference.turbulenceThreadMethod = ThreadMethodComponent.OnePerSystem;
			break;
		case ThreadMethodComponent.OnePerSystem:
			PlaygroundC.reference.turbulenceThreadMethod = ThreadMethodComponent.OneForAll;
			break;
		case ThreadMethodComponent.OneForAll:
			PlaygroundC.reference.turbulenceThreadMethod = ThreadMethodComponent.InsideParticleCalculation;
			break;
		}
	}

	public static void NextThreadPoolMethod()
	{
		switch (PlaygroundC.reference.threadPoolMethod)
		{
		case ThreadPoolMethod.PlaygroundPool:
			PlaygroundC.reference.threadPoolMethod = ThreadPoolMethod.ThreadPool;
			break;
		case ThreadPoolMethod.ThreadPool:
			PlaygroundC.reference.threadPoolMethod = ThreadPoolMethod.PlaygroundPool;
			break;
		}
	}

	public static void NextIndividualParticleSystemMethod(PlaygroundParticlesC p)
	{
		switch (p.threadMethod)
		{
		case ThreadMethodLocal.Inherit:
			p.threadMethod = ThreadMethodLocal.OnePerSystem;
			break;
		case ThreadMethodLocal.OnePerSystem:
			p.threadMethod = ThreadMethodLocal.OneForAll;
			break;
		case ThreadMethodLocal.OneForAll:
			p.threadMethod = ThreadMethodLocal.Inherit;
			break;
		}
	}
}
