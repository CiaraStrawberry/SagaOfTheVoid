using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class onTriggerEnterEnemy : MonoBehaviour
{
	public GameObject focus;

	public GameObject holofocus;

	public GameObject closest;

	public Collider[] cols;

	public string maintag = "Pickup";

	public string holotag = "Hologram";

	public float flo;

	private BoxCollider box;

	private List<Collider> list = new List<Collider>();

	private List<GameObject> Holograms = new List<GameObject>();

	public bool ingamemode;

	public GameObject ObjectsHolder;

	private UnitMovementcommandcontroller unitcontrol;

	private void Awake()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
	{
		if (this != null && gameObject != null)
		{
			box = GetComponent<BoxCollider>();
		}
		if (ingamemode)
		{
			ObjectsHolder = GameObject.Find("Objects");
		}
	}

	private void Update()
	{
		if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1 && box != null)
		{
			flo = box.size.x * 2f;
			maintag = "Ship";
		}
		else if (box != null)
		{
			if ((bool)box)
			{
				flo = box.size.x;
			}
			maintag = "Pickup";
		}
		FindClosestPlayer();
	}

	private void LateUpdate()
	{
		if ((bool)closest)
		{
			_Ship component = closest.GetComponent<_Ship>();
			if (component != null)
			{
				component.enablehealthbar();
			}
		}
	}

	public void FindClosestPlayer()
	{
		if ((bool)box && transform.parent.Find("MovePos") != null)
		{
			Collider[] array = Physics.OverlapBox(transform.parent.Find("MovePos").position, new Vector3(flo * transform.lossyScale.x / 2f, flo * transform.lossyScale.x / 2f, flo * transform.lossyScale.x / 2f));
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].gameObject.tag == maintag)
				{
					list.Add(array[i]);
				}
			}
		}
		else if (transform.parent.Find("MovePos") != null && ObjectsHolder != null)
		{
			Collider[] array2 = Physics.OverlapBox(transform.parent.Find("MovePos").position, new Vector3(flo * ObjectsHolder.transform.lossyScale.x / 2f, flo * ObjectsHolder.transform.lossyScale.x / 2f, flo * ObjectsHolder.transform.lossyScale.x / 2f));
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j].gameObject.tag == maintag)
				{
					list.Add(array2[j]);
				}
			}
		}
		GameObject[] array3 = Holograms.ToArray();
		Holograms.Clear();
		if (array3.Length != 0)
		{
			float num = float.PositiveInfinity;
			for (int k = 0; k < array3.Length; k++)
			{
				float sqrMagnitude = (array3[k].transform.position - transform.parent.Find("MovePos").position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					closest = array3[k].gameObject;
					num = sqrMagnitude;
				}
			}
			holofocus = closest;
		}
		if (closest == null)
		{
			holofocus = null;
		}
		closest = null;
		Collider[] array4 = list.ToArray();
		list.Clear();
		if (array4.Length != 0)
		{
			float num2 = float.PositiveInfinity;
			for (int l = 0; l < array4.Length; l++)
			{
				float sqrMagnitude2 = (array4[l].transform.position - transform.parent.Find("MovePos").position).sqrMagnitude;
				if (sqrMagnitude2 < num2)
				{
					closest = array4[l].gameObject;
					num2 = sqrMagnitude2;
				}
			}
			focus = closest;
		}
		else
		{
			focus = null;
		}
	}
}
