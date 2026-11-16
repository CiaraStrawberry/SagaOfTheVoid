using UnityEngine;

public class WeaponParentHolder : MonoBehaviour
{
	public GameObject Parentstart;

	private void Start()
	{
		Parentstart = transform.parent.gameObject;
		if (transform.parent.GetComponent<WeaponAttachPoint>() == null)
		{
			transform.parent = transform.parent.parent;
		}
	}

	private void Update()
	{
		if (Parentstart.gameObject == null)
		{
			Object.Destroy(gameObject);
		}
	}
}
