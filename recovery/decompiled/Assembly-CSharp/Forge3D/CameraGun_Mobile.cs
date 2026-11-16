using UnityEngine;

namespace Forge3D;

public class CameraGun_Mobile : MonoBehaviour
{
	public GameObject BallPrefab;

	public AudioSource[] ShieldHitSFX;

	private float curTime;

	public float FireRate = 0.1f;

	private float hitPower;

	private void OnGUI()
	{
		GUI.Label(new Rect(5f, 5f, Screen.width, Screen.height), "Left Mouse Button - Throw\nRight Mouse Button - Raycast hit\nQ, E Keys- Hit Power: " + hitPower);
	}

	private void OnMouse()
	{
		if (Input.GetMouseButtonDown(0) && BallPrefab != null)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			GameObject gameObject = Object.Instantiate(BallPrefab, transform.position + transform.forward, Quaternion.identity);
			gameObject.GetComponent<Rigidbody>().AddForce(ray.direction * 55f, ForceMode.VelocityChange);
			gameObject.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f)));
		}
		if (!Input.GetMouseButton(1) || !(curTime >= FireRate))
		{
			return;
		}
		curTime = 0f;
		Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (!Physics.Raycast(ray2, out var hitInfo, 500f))
		{
			return;
		}
		Forcefield_Mobile component = hitInfo.transform.gameObject.GetComponent<Forcefield_Mobile>();
		if (!(component != null))
		{
			return;
		}
		component.OnHit(hitInfo.point);
		if (ShieldHitSFX.Length <= 0)
		{
			return;
		}
		int num = Random.Range(0, ShieldHitSFX.Length);
		for (int i = 0; i < 10; i++)
		{
			if (!ShieldHitSFX[num].isPlaying)
			{
				break;
			}
			num = Random.Range(0, ShieldHitSFX.Length);
		}
		ShieldHitSFX[num].transform.position = hitInfo.point;
		ShieldHitSFX[num].Play();
	}

	private void Update()
	{
		curTime += Time.deltaTime;
		OnMouse();
		if (Input.GetKey(KeyCode.E))
		{
			hitPower += 0.1f;
		}
		else if (Input.GetKey(KeyCode.Q))
		{
			hitPower -= 0.1f;
		}
	}
}
