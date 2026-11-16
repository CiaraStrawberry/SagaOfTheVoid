using System.Collections;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
	public GameObject Parent1;

	private GameObject forwards;

	public bool scriptedstart;

	private MeshRenderer rend;

	private float speed = 50f;

	private bool move;

	public GameObject parent;

	private void Start()
	{
		Parent1 = transform.parent.gameObject;
		Parent1.transform.localRotation *= Quaternion.Euler(0f, 180f, 0f);
		transform.parent = null;
		StartCoroutine(MyCoroutine2());
		StartCoroutine(MyCoroutine());
		if (scriptedstart)
		{
			StartCoroutine(MyCoroutine());
		}
	}

	private IEnumerator MyCoroutine()
	{
		yield return new WaitForSeconds(5f);
		Object.Destroy(gameObject);
	}

	private IEnumerator MyCoroutine2()
	{
		yield return new WaitForSeconds(1f);
		move = true;
	}
}
