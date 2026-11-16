using UnityEngine;

namespace Forge3D;

public class F3DPlayerTurretController : MonoBehaviour
{
	private RaycastHit hitInfo;

	public F3DTurret turret;

	private bool isFiring;

	public F3DFXController fxController;

	private void Update()
	{
		CheckForTurn();
		CheckForFire();
	}

	private void CheckForFire()
	{
		if (!isFiring && Input.GetKeyDown(KeyCode.Mouse0))
		{
			isFiring = true;
			fxController.Fire();
		}
		if (isFiring && Input.GetKeyUp(KeyCode.Mouse0))
		{
			isFiring = false;
			fxController.Stop();
		}
	}

	private void CheckForTurn()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hitInfo, 500f))
		{
			turret.SetNewTarget(hitInfo.point);
		}
	}
}
