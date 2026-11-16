using UnityEngine;

public class ToggleControllerVisuals : MonoBehaviour
{
	public GameObject[] theControllersL;

	public GameObject[] theControllersR;

	private int curIndex;

	private void Start()
	{
		ChangeIndex(3);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			curIndex++;
			if (curIndex == theControllersL.Length)
			{
				curIndex = 0;
			}
		}
	}

	public void ChangeIndex(int index)
	{
		curIndex = index;
		ToggleVisuals();
	}

	public void ToggleVisuals()
	{
		for (int i = 0; i < theControllersL.Length; i++)
		{
			if (theControllersL[i].activeInHierarchy)
			{
				theControllersL[i].SetActive(value: false);
			}
			if (theControllersR[i].activeInHierarchy)
			{
				theControllersR[i].SetActive(value: false);
			}
		}
		theControllersL[curIndex].SetActive(value: true);
		theControllersR[curIndex].SetActive(value: true);
	}
}
