using UnityEngine;

namespace TrueSync;

[AddComponentMenu("TrueSync/Physics/TSMaterial", 22)]
public class TSMaterial : MonoBehaviour
{
	public FP friction = 0.25f;

	public FP restitution = 0;
}
