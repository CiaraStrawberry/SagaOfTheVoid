using UnityEngine;

public class TutorialPositionController : MonoBehaviour
{
	private GameObject player;

	public bool tracking;

	private void Start()
	{
		if (!tracking)
		{
			player = GameObject.Find("ResizeObj");
		}
		else if (GameObject.Find("CenterEyeAnchor") != null)
		{
			player = GameObject.Find("CenterEyeAnchor");
		}
		else
		{
			player = GameObject.Find("Camera (eye)");
		}
	}

	private void Update()
	{
		if ((bool)player)
		{
			transform.position = player.transform.position;
			if (tracking)
			{
				transform.rotation = player.transform.rotation;
			}
		}
	}
}
