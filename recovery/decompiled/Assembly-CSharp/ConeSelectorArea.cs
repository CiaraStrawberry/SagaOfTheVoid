using UnityEngine;

public class ConeSelectorArea : MonoBehaviour
{
	private GameObject inside;

	private void Start()
	{
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.GetComponent<_Ship>() != null)
		{
			transform.parent.parent.gameObject.SendMessage("addtolist", col.gameObject, SendMessageOptions.DontRequireReceiver);
			transform.parent.gameObject.SendMessage("addtolist", col.gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.gameObject.GetComponent<_Ship>() != null)
		{
			transform.parent.parent.gameObject.SendMessage("addtolist", col.gameObject, SendMessageOptions.DontRequireReceiver);
			transform.parent.gameObject.SendMessage("addtolist", col.gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}
}
