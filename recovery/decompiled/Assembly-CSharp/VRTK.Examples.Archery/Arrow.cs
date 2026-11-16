using UnityEngine;

namespace VRTK.Examples.Archery;

public class Arrow : MonoBehaviour
{
	public float maxArrowLife = 10f;

	[HideInInspector]
	public bool inFlight;

	private bool collided;

	private Rigidbody rigidBody;

	private GameObject arrowHolder;

	private Vector3 originalPosition;

	private Quaternion originalRotation;

	private Vector3 originalScale;

	public void SetArrowHolder(GameObject holder)
	{
		arrowHolder = holder;
		arrowHolder.SetActive(value: false);
	}

	public void OnNock()
	{
		collided = false;
		inFlight = false;
	}

	public void Fired()
	{
		DestroyArrow(maxArrowLife);
	}

	public void ResetArrow()
	{
		collided = true;
		inFlight = false;
		RecreateNotch();
		ResetTransform();
	}

	private void Start()
	{
		rigidBody = GetComponent<Rigidbody>();
		SetOrigns();
	}

	private void SetOrigns()
	{
		originalPosition = transform.localPosition;
		originalRotation = transform.localRotation;
		originalScale = transform.localScale;
	}

	private void FixedUpdate()
	{
		if (!collided)
		{
			transform.LookAt(transform.position + rigidBody.velocity);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (inFlight)
		{
			ResetArrow();
		}
	}

	private void RecreateNotch()
	{
		arrowHolder.transform.SetParent(null);
		arrowHolder.SetActive(value: true);
		transform.SetParent(arrowHolder.transform);
		GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<Collider>().enabled = false;
		arrowHolder.GetComponent<Rigidbody>().isKinematic = false;
	}

	private void ResetTransform()
	{
		arrowHolder.transform.position = transform.position;
		arrowHolder.transform.rotation = transform.rotation;
		transform.localPosition = originalPosition;
		transform.localRotation = originalRotation;
		transform.localScale = originalScale;
	}

	private void DestroyArrow(float time)
	{
		Object.Destroy(arrowHolder, time);
		Object.Destroy(gameObject, time);
	}
}
