using UnityEngine;

public class OnClickDisableObj : MonoBehaviour
{
	private void OnClick()
	{
		gameObject.SetActive(value: false);
	}
}
