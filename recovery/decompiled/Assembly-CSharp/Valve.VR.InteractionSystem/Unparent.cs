using UnityEngine;

namespace Valve.VR.InteractionSystem;

public class Unparent : MonoBehaviour
{
	private Transform oldParent;

	private void Start()
	{
		oldParent = transform.parent;
		transform.parent = null;
		gameObject.name = oldParent.gameObject.name + "." + gameObject.name;
	}

	private void Update()
	{
		if (oldParent == null)
		{
			Object.Destroy(gameObject);
		}
	}

	public Transform GetOldParent()
	{
		return oldParent;
	}
}
