using UnityEngine;

namespace VRTK.Examples.Archery;

public class ArrowNotch : MonoBehaviour
{
	private GameObject arrow;

	private VRTK_InteractableObject obj;

	private void Start()
	{
		arrow = transform.Find("Arrow").gameObject;
		obj = GetComponent<VRTK_InteractableObject>();
	}

	private void OnTriggerEnter(Collider collider)
	{
		BowHandle componentInParent = collider.GetComponentInParent<BowHandle>();
		if (componentInParent != null && obj != null && componentInParent.aim.IsHeld() && obj.IsGrabbed())
		{
			componentInParent.nockSide = collider.transform;
			arrow.transform.SetParent(componentInParent.arrowNockingPoint);
			CopyNotchToArrow();
			collider.GetComponentInParent<BowAim>().SetArrow(arrow);
			Object.Destroy(gameObject);
		}
	}

	private void CopyNotchToArrow()
	{
		GameObject gameObject = Object.Instantiate(base.gameObject, transform.position, transform.rotation);
		gameObject.name = name;
		arrow.GetComponent<Arrow>().SetArrowHolder(gameObject);
		arrow.GetComponent<Arrow>().OnNock();
	}
}
