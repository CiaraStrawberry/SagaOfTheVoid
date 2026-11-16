using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackHeadset : MonoBehaviour
{
	private PhotonView personalphotonview;

	public GameObject Cameraobj;

	public GameObject LeftControler;

	public GameObject RightControler;

	public GameObject LeftControllerLocal;

	public GameObject RightControllerLocal;

	private bool moving;

	private void Start()
	{
		personalphotonview = GetComponent<PhotonView>();
		if (personalphotonview.isMine)
		{
			if ((bool)GameObject.Find("CenterEyeAnchor"))
			{
				Cameraobj = GameObject.Find("CenterEyeAnchor");
			}
			else
			{
				Cameraobj = GameObject.Find("Camera (eye)");
			}
			PhotonNetwork.Instantiate("LeftController", new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f), 0);
			PhotonNetwork.Instantiate("RightController", new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f), 0);
		}
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		moving = true;
		if (personalphotonview.isMine)
		{
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				meshRenderer.enabled = false;
			}
		}
	}

	private void Update()
	{
		if (personalphotonview.isMine && Cameraobj != null)
		{
			transform.position = Cameraobj.transform.position;
			transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			transform.rotation = Cameraobj.transform.rotation;
		}
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		if (!(this != null) || !(gameObject != null))
		{
			return;
		}
		moving = true;
		if (personalphotonview.isMine)
		{
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				meshRenderer.enabled = false;
			}
		}
	}
}
