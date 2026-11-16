using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class MeshRenderCloakController : MonoBehaviour
{
	public bool cloaked;

	public bool cloakedlast;

	public Material cloakedmat;

	private MeshRenderer mesh;

	private Material original;

	private UnitMovementcommandcontroller unitcontrol;

	private _Ship parent;

	private GameObject engines;

	public List<MeshRenderer> parentchildren = new List<MeshRenderer>();

	public bool distancecloacked;

	public CustomPathfinding parentpasfinding;

	private GameObject World;

	public bool started;

	public LineRenderController linerencontrol;

	private int timefromupdate;

	public float test;

	public void specupdate()
	{
		if ((bool)linerencontrol)
		{
			linerencontrol.OnSyncedUpdatspec();
		}
	}

	public void StartMain()
	{
		mesh = GetComponent<MeshRenderer>();
		original = mesh.material;
		unitcontrol = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
		parent = base.transform.parent.GetComponent<_Ship>();
		parentpasfinding = base.transform.parent.GetComponent<CustomPathfinding>();
		World = GameObject.Find("WorldScaleBase");
		if ((bool)base.transform.parent.Find("Engines"))
		{
			engines = base.transform.parent.Find("Engines").gameObject;
		}
		foreach (Transform item2 in base.transform.parent)
		{
			if (item2.gameObject != gameObject && item2.tag == "Pickup")
			{
				MeshRenderer[] componentsInChildren = item2.GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer item in componentsInChildren)
				{
					parentchildren.Add(item);
				}
			}
		}
		started = true;
		if (transform.childCount > 0)
		{
			linerencontrol = transform.GetChild(0).GetComponent<LineRenderController>();
		}
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		if (started)
		{
			if (!mesh.enabled)
			{
				mesh.enabled = true;
			}
			if (cloakedlast != cloaked)
			{
				if (cloaked)
				{
					cloack();
				}
				else if (!distancecloacked)
				{
					mesh.material = original;
					mesh.enabled = true;
					if ((bool)engines && parentpasfinding.currentlymoving)
					{
						engines.SetActive(value: true);
					}
					foreach (MeshRenderer parentchild in parentchildren)
					{
						parentchild.enabled = true;
					}
				}
				cloakedlast = cloaked;
			}
			if (cloaked && (bool)engines && engines.GetActive())
			{
				engines.SetActive(value: false);
			}
		}
		if (parentpasfinding == null)
		{
			parentpasfinding = transform.parent.GetComponent<CustomPathfinding>();
		}
		if ((bool)parentpasfinding && parentpasfinding.timepassedsincespawn == 0f)
		{
			if (mesh == null)
			{
				mesh = GetComponent<MeshRenderer>();
			}
			if ((bool)mesh && mesh.enabled)
			{
				mesh.enabled = false;
			}
		}
	}

	public void cloack()
	{
		if ((bool)unitcontrol && unitcontrol.alliesout() != null && unitcontrol.alliesout().Contains(parent.GetComponent<TSTransform>()))
		{
			mesh.material = cloakedmat;
			if ((bool)engines && parentpasfinding.currentlymoving)
			{
				engines.SetActive(value: true);
			}
		}
		else if ((bool)mesh)
		{
			mesh.enabled = false;
			if ((bool)engines)
			{
				engines.SetActive(value: false);
			}
		}
		foreach (MeshRenderer parentchild in parentchildren)
		{
			parentchild.enabled = false;
		}
	}

	public void uncloack()
	{
		if (cloaked || !mesh)
		{
			return;
		}
		mesh.material = original;
		mesh.enabled = true;
		if ((bool)engines && parentpasfinding.currentlymoving)
		{
			engines.SetActive(value: true);
		}
		foreach (MeshRenderer parentchild in parentchildren)
		{
			parentchild.enabled = true;
		}
	}

	public bool asigncloacked()
	{
		TSTransform tSTransform = checkclosest();
		if (tSTransform != null)
		{
			cloaked = false;
			return false;
		}
		cloaked = true;
		return true;
	}

	private TSTransform checkclosest()
	{
		TSTransform result = null;
		TSTransform[] array = unitcontrol.targetsout(parent.ShipColor).ToArray();
		float num = float.PositiveInfinity;
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] != null) || !(array[i].gameObject != null))
			{
				continue;
			}
			float num2 = Vector3.Distance(array[i].transform.localPosition, transform.parent.localPosition);
			if (num > num2)
			{
				num = num2;
				if (num2 < 2000f)
				{
					result = array[i];
				}
			}
		}
		return result;
	}
}
