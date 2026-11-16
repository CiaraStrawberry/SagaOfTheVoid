using UnityEngine;

namespace HWRWeaponSystem;

public class DamageBase : MonoBehaviour
{
	public GameObject Effect;

	[HideInInspector]
	public GameObject Owner;

	public int Damage = 20;

	[HideInInspector]
	public ObjectPool objectPool;

	public string[] TargetTag = new string[1] { "Enemy" };

	public string[] IgnoreTag;

	public bool DoDamageCheck(GameObject gob)
	{
		for (int i = 0; i < IgnoreTag.Length; i++)
		{
			if (IgnoreTag[i] == gob.tag)
			{
				return false;
			}
		}
		return true;
	}

	public void IgnoreSelf(GameObject owner)
	{
		if (!GetComponent<Collider>() || !owner || !owner.GetComponent<Collider>())
		{
			return;
		}
		Physics.IgnoreCollision(GetComponent<Collider>(), owner.GetComponent<Collider>());
		if ((bool)Owner.transform.root)
		{
			Collider[] componentsInChildren = Owner.transform.root.GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				Physics.IgnoreCollision(GetComponent<Collider>(), collider);
			}
		}
	}
}
