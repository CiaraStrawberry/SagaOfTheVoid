using UnityEngine;
using UnityEngine.Events;

public class TriggerVolumeControllers : MonoBehaviour
{
	public UnityEvent theEvent;

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Finish")
		{
			theEvent.Invoke();
		}
	}
}
