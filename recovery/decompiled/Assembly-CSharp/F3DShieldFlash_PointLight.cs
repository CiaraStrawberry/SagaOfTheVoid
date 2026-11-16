using System.Collections;
using UnityEngine;

public class F3DShieldFlash_PointLight : MonoBehaviour
{
	public Light ShieldFlash;

	private void Start()
	{
		StartCoroutine("Despawn");
	}

	private IEnumerator Despawn()
	{
		yield return new WaitForSeconds(2f);
		Object.Destroy(gameObject);
	}

	private void Update()
	{
		ShieldFlash.intensity = Mathf.Lerp(ShieldFlash.intensity, 0f, Time.deltaTime * 7f);
	}
}
