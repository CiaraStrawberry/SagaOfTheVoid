using UnityEngine;

public class WeaponAttachPoint : MonoBehaviour
{
	public bool equipment;

	private MeshRenderer meshren;

	public bool special;

	private void Start()
	{
		meshren = GetComponent<MeshRenderer>();
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider col)
	{
		if (!(col.tag == "Pickup"))
		{
		}
	}
}
