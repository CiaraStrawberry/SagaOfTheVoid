using UnityEngine;

namespace Valve.VR.InteractionSystem;

public class HideOnHandFocusLost : MonoBehaviour
{
	private void OnHandFocusLost(Hand hand)
	{
		gameObject.SetActive(value: false);
	}
}
