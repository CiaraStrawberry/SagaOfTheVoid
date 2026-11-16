using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
	public StarSystemManager starManager;

	public CanvasGroup SystemInfo;

	private void Start()
	{
	}

	private void Update()
	{
		SystemInfo.alpha = 0f;
		foreach (GameObject system in starManager.Systems)
		{
			if (system.GetComponent<SystemGUICircle>().Over)
			{
				SystemInfo.alpha = 1f;
				SystemInfo.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, system.transform.position);
				SystemInfo.transform.GetChild(0).GetComponent<Text>().text = system.GetComponent<StarSystem>().Name;
				SystemInfo.transform.GetChild(1).GetComponent<Text>().text = system.GetComponent<StarSystem>().Planets.ToString();
			}
		}
	}
}
