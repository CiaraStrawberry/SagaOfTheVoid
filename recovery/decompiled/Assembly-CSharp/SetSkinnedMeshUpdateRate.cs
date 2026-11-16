using ParticlePlayground;
using UnityEngine;

public class SetSkinnedMeshUpdateRate : MonoBehaviour
{
	private void Start()
	{
		PlaygroundC.skinnedUpdateRate = 2;
	}
}
