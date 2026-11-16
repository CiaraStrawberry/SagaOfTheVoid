using UnityEngine;

namespace HWRWeaponSystem;

public class AILook : MonoBehaviour
{
	public string[] TargetTag = new string[1] { "Enemy" };

	public float Distance = 1000f;

	private int indexWeapon;

	public GameObject target;

	private WeaponController weapon;

	public bool test;

	private float timeAIattack;

	public bool missile;

	private GameObject child;

	private GameObject muzzle;

	private void Start()
	{
		weapon = GetComponent<WeaponController>();
		child = transform.GetChild(0).gameObject;
		muzzle = gameObject;
	}

	private void Update()
	{
		if ((bool)target)
		{
			if (target.gameObject == null)
			{
				target = null;
			}
		}
		else
		{
			target = null;
		}
		if ((bool)target)
		{
			Quaternion b = Quaternion.LookRotation(target.transform.position - transform.position);
			b.x = transform.rotation.x;
			b.z = transform.rotation.z;
			transform.rotation = Quaternion.Lerp(transform.rotation, b, Time.deltaTime * 3f);
			Quaternion b2 = Quaternion.LookRotation(target.transform.position - child.transform.position);
			Vector3 eulerAngles = Quaternion.Lerp(child.transform.localRotation, b2, Time.deltaTime * 10f).eulerAngles;
			eulerAngles.y = 0f;
			child.transform.LookAt(target.transform);
			Vector3 normalized = (target.transform.position - transform.position).normalized;
			float num = Vector3.Dot(normalized, transform.forward);
			if (num > 0.1f && (bool)weapon)
			{
				if (!missile)
				{
					Vector3 vector = target.transform.position - muzzle.transform.position;
					if (Physics.Raycast(muzzle.transform.position, vector, out var hitInfo, Distance, 1) && hitInfo.transform.gameObject == target)
					{
						weapon.LaunchWeapon(indexWeapon);
						weapon.target = target;
					}
					Debug.DrawRay(muzzle.transform.position, vector);
				}
				else
				{
					weapon.LaunchWeapon(indexWeapon);
					weapon.target = target;
				}
			}
			if (Time.time > timeAIattack + 3f)
			{
				target = null;
			}
			return;
		}
		for (int i = 0; i < TargetTag.Length; i++)
		{
			if (GameObject.FindGameObjectsWithTag(TargetTag[i]).Length <= 0)
			{
				continue;
			}
			GameObject[] array = GameObject.FindGameObjectsWithTag(TargetTag[i]);
			float num2 = Distance;
			for (int j = 0; j < array.Length; j++)
			{
				float num3 = Vector3.Distance(array[j].transform.position, transform.position);
				if (num2 > num3)
				{
					num2 = num3;
					target = array[j];
					if ((bool)weapon)
					{
						indexWeapon = Random.Range(0, weapon.WeaponLists.Length);
					}
					timeAIattack = Time.time;
				}
			}
		}
	}
}
