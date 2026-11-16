using UnityEngine;

public class EngineScalingScript : MonoBehaviour
{
	public GameObject World;

	private Quaternion startquat;

	private Vector3 startscale;

	private void Start()
	{
		World = GameObject.Find("WorldScaleBase");
		startquat = transform.localRotation;
		startscale = transform.localScale;
	}

	private void Update()
	{
		if ((!(World.transform.localScale.x > 145f) || !(World.transform.localScale.x < 900f)) && (!(World.transform.localScale.x < 145f) || !(World.transform.localScale.x > 30f)) && !(World.transform.localScale.x > 900f))
		{
		}
	}
}
