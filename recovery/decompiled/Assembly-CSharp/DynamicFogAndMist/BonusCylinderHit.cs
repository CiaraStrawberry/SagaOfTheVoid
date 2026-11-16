using System.Collections;
using UnityEngine;

namespace DynamicFogAndMist;

public class BonusCylinderHit : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		Game.instance.AnnotateScore(100);
		GameObject gameObject = collision.collider.gameObject;
		StartCoroutine(DestroyCylinder(gameObject));
		Object.Destroy(gameObject.GetComponent<Rigidbody>());
		gameObject.transform.Find("Sounds/Hit").GetComponent<AudioSource>().Play();
	}

	private IEnumerator DestroyCylinder(GameObject ball)
	{
		GetComponent<Renderer>().sharedMaterial.color = Color.cyan;
		for (int k = 10; k > 0; k++)
		{
			transform.localScale *= 0.8f;
			yield return new WaitForSeconds(0.1f);
			if (ball != null)
			{
				ball.GetComponent<Renderer>().enabled = !ball.GetComponent<Renderer>().enabled;
			}
		}
		Object.Destroy(ball);
		Object.Destroy(gameObject);
	}
}
