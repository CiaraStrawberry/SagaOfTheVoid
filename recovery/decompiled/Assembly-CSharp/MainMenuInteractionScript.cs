using UnityEngine;
using Valve.VR;

public class MainMenuInteractionScript : MonoBehaviour
{
	public Material linemat;

	private LineRenderer lineren;

	private SteamVR_TrackedObject trackedObj;

	private EVRButtonId triggerButton = EVRButtonId.k_EButton_Axis1;

	private GameObject inside;

	public LayerMask lay;

	public bool runnow;

	private SteamVR_Controller.Device controller => SteamVR_Controller.Input((int)trackedObj.index);

	private void Start()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		lineren = gameObject.AddComponent<LineRenderer>();
		lineren.startWidth = 0.01f;
		lineren.endWidth = 0.01f;
		lineren.material = linemat;
	}

	private void Update()
	{
		lineren.SetPosition(0, transform.position);
		if (!runnow)
		{
			RaycastHit hitInfo;
			if (inside == null)
			{
				if (Physics.Raycast(transform.position, transform.forward, out hitInfo, (int)lay))
				{
					lineren.SetPosition(1, hitInfo.point);
				}
				else
				{
					lineren.SetPosition(1, transform.position + transform.forward * 100f);
				}
			}
			else
			{
				lineren.SetPosition(1, transform.position);
			}
			if (controller.GetPressDown(triggerButton) && Physics.Raycast(transform.position, transform.forward, out hitInfo, (int)lay) && inside == null)
			{
				hitInfo.transform.gameObject.SendMessage("Load", SendMessageOptions.DontRequireReceiver);
			}
			if (!Physics.Raycast(transform.position, transform.forward, out hitInfo))
			{
				return;
			}
			if ((bool)GameObject.Find("GalaxyParent"))
			{
				if (GameObject.Find("GalaxyParent").transform.localScale.x > 5.9f)
				{
					hitInfo.transform.gameObject.SendMessage("up", SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				hitInfo.transform.gameObject.SendMessage("up", SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			lineren.SetPosition(1, transform.position);
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Pickup" || col.tag == "Ship")
		{
			inside = col.gameObject;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.gameObject == inside)
		{
			inside = null;
		}
	}
}
