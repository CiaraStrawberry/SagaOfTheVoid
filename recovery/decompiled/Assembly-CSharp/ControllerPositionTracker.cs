using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerPositionTracker : MonoBehaviour
{
	public GameObject Tracker;

	public bool rightcontroller;

	private PhotonView locview;

	private GameObject ViveObj;

	private GameObject OculusObj;

	public bool vivetype;

	public bool moving;

	private void Start()
	{
		locview = GetComponent<PhotonView>();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Voice");
		foreach (GameObject gameObject in array)
		{
			if (gameObject.GetComponent<PhotonView>().owner.ID == locview.owner.ID)
			{
				transform.parent = gameObject.transform;
			}
		}
		if (locview.isMine)
		{
			if (rightcontroller && (bool)GameObject.Find("RightController"))
			{
				Tracker = GameObject.Find("RightController").transform.parent.gameObject;
			}
			else if ((bool)GameObject.Find("LeftController"))
			{
				Tracker = GameObject.Find("LeftController").transform.parent.gameObject;
			}
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				meshRenderer.enabled = false;
			}
		}
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		if (SceneManager.GetActiveScene().name == "mptest")
		{
			moving = false;
		}
		else
		{
			moving = true;
		}
		ViveObj = transform.Find("ViveModel").gameObject;
		OculusObj = transform.Find("OculusModel").gameObject;
		InvokeRepeating("Rep", 0f, 1f);
		if (locview.isMine)
		{
			if ((bool)GameObject.Find("CenterEyeAnchor"))
			{
				locview.RPC("setcontrollerType", PhotonTargets.All, false);
			}
			else
			{
				locview.RPC("setcontrollerType", PhotonTargets.All, true);
			}
		}
	}

	[PunRPC]
	private void setcontrollerType(bool vive)
	{
		vivetype = vive;
	}

	private void Update()
	{
		if (Tracker != null && moving)
		{
			transform.position = Tracker.transform.position;
			transform.rotation = Tracker.transform.rotation;
		}
	}

	private void Rep()
	{
		if (moving)
		{
			if (vivetype)
			{
				if (OculusObj.GetActive())
				{
					OculusObj.SetActive(value: false);
				}
				if (!ViveObj.GetActive())
				{
					ViveObj.SetActive(value: true);
				}
			}
			else
			{
				if (!OculusObj.GetActive())
				{
					OculusObj.SetActive(value: true);
				}
				if (ViveObj.GetActive())
				{
					ViveObj.SetActive(value: false);
				}
			}
		}
		if (Tracker == null && locview.isMine)
		{
			if (rightcontroller && (bool)GameObject.Find("RightController"))
			{
				Tracker = GameObject.Find("RightController").transform.parent.gameObject;
			}
			else if ((bool)GameObject.Find("LeftController"))
			{
				Tracker = GameObject.Find("LeftController").transform.parent.gameObject;
			}
		}
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		if (!(this != null) || !(gameObject != null))
		{
			return;
		}
		if (scene.name == "mptest")
		{
			moving = false;
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				meshRenderer.enabled = false;
			}
		}
		else
		{
			MeshRenderer[] componentsInChildren2 = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer2 in componentsInChildren2)
			{
				meshRenderer2.enabled = true;
			}
			moving = true;
		}
		if (locview.isMine)
		{
			MeshRenderer[] componentsInChildren3 = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer3 in componentsInChildren3)
			{
				meshRenderer3.enabled = false;
			}
		}
	}
}
