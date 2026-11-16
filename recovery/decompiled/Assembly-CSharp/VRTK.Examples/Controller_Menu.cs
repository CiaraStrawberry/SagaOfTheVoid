using UnityEngine;

namespace VRTK.Examples;

public class Controller_Menu : MonoBehaviour
{
	public GameObject menuObject;

	private GameObject clonedMenuObject;

	private bool menuInit;

	private bool menuActive;

	private void Start()
	{
		GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed += DoMenuOn;
		GetComponent<VRTK_ControllerEvents>().ButtonTwoReleased += DoMenuOff;
		menuInit = false;
		menuActive = false;
	}

	private void InitMenu()
	{
		clonedMenuObject = Object.Instantiate(menuObject, transform.position, Quaternion.identity);
		clonedMenuObject.SetActive(value: true);
		menuInit = true;
	}

	private void DoMenuOn(object sender, ControllerInteractionEventArgs e)
	{
		if (!menuInit)
		{
			InitMenu();
		}
		clonedMenuObject.SetActive(value: true);
		menuActive = true;
	}

	private void DoMenuOff(object sender, ControllerInteractionEventArgs e)
	{
		clonedMenuObject.SetActive(value: false);
		menuActive = false;
	}

	private void Update()
	{
		if (menuActive)
		{
			clonedMenuObject.transform.rotation = transform.rotation;
			clonedMenuObject.transform.position = transform.position;
		}
	}
}
