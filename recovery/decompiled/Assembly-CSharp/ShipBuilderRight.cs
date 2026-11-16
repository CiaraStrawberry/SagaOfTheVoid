using UnityEngine;

public class ShipBuilderRight : MonoBehaviour
{
	private void Start()
	{
	}

	private void Load()
	{
		GameObject.Find("ShipParent").SendMessage("Addone");
	}
}
