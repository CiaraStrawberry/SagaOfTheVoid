using UnityEngine;

public class ShipBuilderLeft : MonoBehaviour
{
	private void Start()
	{
	}

	private void Load()
	{
		GameObject.Find("ShipParent").SendMessage("gobackone");
	}
}
