using UnityEngine;

public class ReturnMenuStep : MonoBehaviour
{
	private GalaxyController galControl;

	private void Start()
	{
		galControl = GameObject.Find("Controllers").GetComponent<GalaxyController>();
	}

	private void Update()
	{
	}

	private void Load()
	{
		galControl.returntonormal();
		Debug.Log("returntoMenu");
	}
}
