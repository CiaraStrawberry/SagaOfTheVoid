using UnityEngine;

namespace VRTK.Examples;

public class ExampleSceneSimulatorCameraRigMover : MonoBehaviour
{
	protected virtual void Awake()
	{
		transform.Find("VRSimulatorCameraRig").position -= transform.position;
	}
}
