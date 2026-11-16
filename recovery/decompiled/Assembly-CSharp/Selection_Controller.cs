using UnityEngine;

public class Selection_Controller : MonoBehaviour
{
	private GameObject HealthBar;

	private void Awake()
	{
		HealthBar = transform.Find("HealthBar").gameObject;
		HealthBar.SetActive(value: false);
	}

	public void enable()
	{
		if ((bool)HealthBar)
		{
			HealthBar.SetActive(value: true);
			return;
		}
		HealthBar = transform.Find("HealthBar").gameObject;
		HealthBar.SetActive(value: false);
	}

	public void Disable()
	{
		if ((bool)HealthBar)
		{
			HealthBar.SetActive(value: true);
			return;
		}
		HealthBar = transform.Find("HealthBar").gameObject;
		HealthBar.SetActive(value: false);
	}
}
