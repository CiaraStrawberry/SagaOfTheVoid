using UnityEngine;
using UnityEngine.SceneManagement;

public class Input_UI_Control_Script : MonoBehaviour
{
	public SceneField changescene;

	private bool isup;

	private Vector3 startpos;

	private GameObject galaxy;

	private Vector3 Startme;

	private Vector3 startposme;

	public GameObject parent;

	private void Start()
	{
		galaxy = GameObject.Find("Controllers");
		startpos = transform.parent.localPosition;
		startposme = transform.position;
		parent = transform.parent.gameObject;
		Startme = transform.localPosition;
	}

	private void Load()
	{
		SceneManager.LoadSceneAsync(changescene, LoadSceneMode.Single);
	}

	private void Update()
	{
		if ((bool)parent)
		{
			if (!isup)
			{
				parent.transform.localPosition = startpos;
			}
			else
			{
				parent.transform.localPosition = startpos + new Vector3(0f, 0.01f, 0f);
			}
			isup = false;
		}
	}

	private void up()
	{
		isup = true;
		if ((bool)parent)
		{
			transform.position = parent.transform.position;
			transform.parent = null;
		}
	}

	public void state()
	{
	}
}
