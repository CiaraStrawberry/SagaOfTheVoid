using UnityEngine;

namespace Forge3D;

public class SimpleGun : MonoBehaviour
{
	private void Update()
	{
		if (!Input.GetMouseButton(0))
		{
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out var hitInfo, 500f))
		{
			Forcefield componentInParent = hitInfo.transform.GetComponentInParent<Forcefield>();
			if (componentInParent != null)
			{
				float hitPower = Random.Range(-2f, 2f);
				componentInParent.OnHit(hitInfo.point, hitPower);
			}
		}
	}
}
