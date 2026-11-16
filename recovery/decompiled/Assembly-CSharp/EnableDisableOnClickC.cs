using UnityEngine;

public class EnableDisableOnClickC : MonoBehaviour
{
	public GameObject go;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			go.SetActive(!go.activeSelf);
		}
	}
}
