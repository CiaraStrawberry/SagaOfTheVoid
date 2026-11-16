using UnityEngine;

namespace Forge3D;

public class F3DObserver : MonoBehaviour
{
	public Transform ShieldFlashLight;

	public Transform ShieldHit;

	public Transform[] ShieldHitFX;

	private float ReactSpeed;

	private float curTime;

	private void Update()
	{
		if (F3DWeaponRangeController.isObserverActive && Input.GetMouseButton(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hitInfo, 500f))
			{
				Forcefield componentInParent = hitInfo.transform.GetComponentInParent<Forcefield>();
				if (componentInParent != null)
				{
					float hitPower = Random.Range(-2f, 2f);
					componentInParent.OnHit(hitInfo.point, hitPower);
					ReactSpeed = componentInParent.ReactSpeed;
					if (curTime >= ReactSpeed)
					{
						Object.Instantiate(ShieldFlashLight, hitInfo.point + hitInfo.normal, Quaternion.identity);
						Object.Instantiate(ShieldHit, hitInfo.point + hitInfo.normal * 0.5f, Quaternion.identity);
						if (ShieldHitFX != null)
						{
							AudioSource component = Object.Instantiate(ShieldHitFX[Random.Range(0, ShieldHitFX.Length)], hitInfo.point, Quaternion.identity).GetComponent<AudioSource>();
							component.volume -= Random.Range(0f, 0.25f);
							component.pitch -= Random.Range(-0.03f, 0.03f);
							component.Play();
						}
						curTime = 0f;
					}
				}
			}
		}
		curTime += Time.deltaTime;
	}
}
