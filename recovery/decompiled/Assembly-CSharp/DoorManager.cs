using UnityEngine;

public class DoorManager : MonoBehaviour
{
	public Door door1;

	public Door door2;

	private void OnTriggerEnter()
	{
		if (door1 != null)
		{
			door1.OpenDoor();
		}
		if (door2 != null)
		{
			door2.OpenDoor();
		}
	}
}
