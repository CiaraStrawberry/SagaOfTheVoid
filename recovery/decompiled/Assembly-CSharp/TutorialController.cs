using UnityEngine;

public class TutorialController : MonoBehaviour
{
	public GameObject LookControls;

	public GameObject OrderControls;

	public GameObject GameObjectives;

	private GameObject movebase;

	private Vector3 startpos;

	private void Start()
	{
		LookControls.SetActive(value: true);
		OrderControls.SetActive(value: false);
		GameObjectives.SetActive(value: false);
		movebase = GameObject.Find("VRTK_SDK");
		startpos = transform.position - movebase.transform.position;
	}

	private void Update()
	{
		transform.position = movebase.transform.position + startpos;
	}

	public void MoveNext()
	{
		if (LookControls.GetActive())
		{
			OrderControls.SetActive(value: true);
			LookControls.SetActive(value: false);
		}
		else if (OrderControls.GetActive())
		{
			OrderControls.SetActive(value: false);
			GameObjectives.SetActive(value: true);
		}
		else if (GameObjectives.GetActive())
		{
			GameObjectives.SetActive(value: false);
			PhotonNetwork.LeaveRoom();
			PhotonNetwork.LoadLevel(0);
		}
	}
}
