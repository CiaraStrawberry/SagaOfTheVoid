using System.Collections;
using UnityEngine;
using VRTK;

public class ControllerInteractable : MonoBehaviour
{
	private BoxCollider bcol;

	private bool triggerdown;

	private GameObject colliderObj;

	public GameObject heldobj;

	private bool respawnnow;

	private int childcountstart;

	private onTriggerEnterEnemy triggerCheck;

	public GameObject shipclose;

	public GameObject grabbedobject;

	private Vector3 startdisparity;

	private Quaternion relativerot;

	private GameObject childobj;

	private GameObject tempheld;

	private VRTK_ControllerEvents vrtkcontrollerevents;

	private void Start()
	{
		bcol = GetComponent<BoxCollider>();
		vrtkcontrollerevents = GetComponent<VRTK_ControllerEvents>();
		StartCoroutine(Example());
		triggerCheck = GetComponent<onTriggerEnterEnemy>();
		vrtkcontrollerevents.TriggerClicked += triggerdownmeth;
		vrtkcontrollerevents.TriggerUnclicked += triggerup;
	}

	public void triggerdownmeth(object sender, ControllerInteractionEventArgs e)
	{
		triggerdown = true;
		if ((bool)colliderObj)
		{
			Checkpickup(colliderObj);
			checkpickuptrigger(colliderObj);
		}
		else if ((bool)shipclose)
		{
			grab(shipclose);
		}
	}

	public void triggerup(object sender, ControllerInteractionEventArgs e)
	{
		triggerdown = false;
		if (respawnnow)
		{
			tempheld.GetComponent<WeaponBaseClass>().respawn();
			tempheld = null;
			respawnnow = false;
		}
		if ((bool)heldobj)
		{
			heldobj.GetComponent<WeaponBaseClass>().reset(0f);
			heldobj = null;
		}
		if ((bool)grabbedobject)
		{
			GameObject gameObject = grabbedobject.transform.parent.gameObject;
			grabbedobject.transform.parent = grabbedobject.transform.parent.parent;
			gameObject.transform.parent = grabbedobject.transform;
			grabbedobject = null;
		}
	}

	private void Update()
	{
		colliderObj = triggerCheck.focus;
		shipclose = triggerCheck.holofocus;
		if ((bool)grabbedobject && (bool)childobj)
		{
			childobj.transform.position = transform.position;
			childobj.transform.rotation = transform.rotation * relativerot;
		}
	}

	private void grab(GameObject grabbed)
	{
		grabbedobject = grabbed;
		startdisparity = grabbed.transform.position - transform.position;
		GameObject gameObject = grabbed.transform.Find("Transformparent").gameObject;
		gameObject.transform.parent = grabbed.transform.parent;
		grabbed.transform.parent = gameObject.transform;
		Vector3 position = grabbed.transform.position;
		gameObject.transform.position = transform.position;
		grabbed.transform.position = position;
		relativerot = Quaternion.Inverse(transform.rotation) * grabbed.transform.rotation;
		childobj = gameObject;
	}

	private void OnTriggerEnter(Collider col)
	{
		if (grabbedobject == null)
		{
			Checkpickup(col.gameObject);
		}
	}

	private void OnTriggerExit(Collider col)
	{
	}

	private bool Checkpickup(GameObject col)
	{
		bool result = false;
		WeaponAttachPoint component = col.GetComponent<WeaponAttachPoint>();
		if (component != null && (bool)heldobj && col.transform.childCount < 1 && ((component.equipment && heldobj.GetComponent<WeaponBaseClass>().equipment) || (component.special && heldobj.GetComponent<WeaponBaseClass>().special)))
		{
			bool flag = true;
			WeaponBaseClass component2 = heldobj.GetComponent<WeaponBaseClass>();
			if ((bool)heldobj.GetComponent<_Ship>())
			{
				if (GameObject.Find("Fleet Builder Parent").GetComponent<FleetBuilderController>().enough(component2.cost))
				{
					col.transform.parent.GetComponent<AttachPointsController>().AttachShip(heldobj);
					tempheld = heldobj;
					heldobj = null;
					respawnnow = true;
				}
			}
			else
			{
				heldobj.transform.parent = col.gameObject.transform;
				heldobj.transform.localPosition = new Vector3(0f, 0f, 0f);
				heldobj.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
				tempheld = heldobj;
				heldobj = null;
				respawnnow = true;
			}
		}
		return result;
	}

	private void checkpickuptrigger(GameObject col)
	{
		if (col.gameObject.tag == "Pickup" && triggerdown && transform.childCount < childcountstart + 1 && heldobj == null && grabbedobject == null)
		{
			if ((bool)col.transform.parent.GetComponent<WeaponAttachPoint>())
			{
				col.GetComponent<WeaponBaseClass>().reset(0f);
				return;
			}
			WeaponBaseClass component = col.GetComponent<WeaponBaseClass>();
			component.scaletosize();
			Debug.Log(transform.childCount < childcountstart + 1);
			col.transform.parent = transform;
			col.transform.localPosition = new Vector3(0f, 0f, 0f);
			component.disabletext();
			heldobj = col.gameObject;
		}
	}

	private IEnumerator Example()
	{
		yield return new WaitForSeconds(1f);
		childcountstart = transform.childCount;
	}
}
