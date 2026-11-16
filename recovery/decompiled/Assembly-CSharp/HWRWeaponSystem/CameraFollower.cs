using UnityEngine;

namespace HWRWeaponSystem;

public class CameraFollower : MonoBehaviour
{
	public GameObject Target;

	public Vector3 Offset;

	private void Start()
	{
	}

	private void Update()
	{
		if ((bool)Target)
		{
			transform.position = Vector3.Lerp(transform.position, Target.transform.position + Offset, Time.deltaTime * 10f);
		}
	}
}
