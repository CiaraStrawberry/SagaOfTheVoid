using ParticlePlayground;
using UnityEngine;

public class PlaygroundManipulatorStrength : MonoBehaviour
{
	public float maxStrength = 3f;

	public float adjustmentSpeed = 2f;

	private ManipulatorObjectC manipulator;

	private void Start()
	{
		manipulator = PlaygroundC.GetManipulator(0, GetComponent<PlaygroundParticlesC>());
	}

	private void Update()
	{
		manipulator.strength = ((!Input.GetMouseButton(0)) ? Mathf.Lerp(manipulator.strength, 0f, adjustmentSpeed * Time.deltaTime) : Mathf.Lerp(manipulator.strength, maxStrength, adjustmentSpeed * Time.deltaTime));
	}
}
