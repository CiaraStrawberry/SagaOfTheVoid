using UnityEngine;

namespace HWRWeaponSystem;

public class ChainDamage : DamageBase
{
	public GameObject ChainObject;

	public int NumberChain = 5;

	public int Distance = 100;

	public float Direction = 0.5f;

	private bool actived;

	private void Awake()
	{
	}

	private void Start()
	{
		chainDamage();
		actived = true;
	}

	private void chainDamage()
	{
		int num = 0;
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
				if (!(array[j] != null))
				{
					continue;
				}
				Vector3 normalized = (array[j].transform.position - transform.position).normalized;
				float num3 = Vector3.Dot(normalized, transform.forward);
				float num4 = Vector3.Distance(array[j].transform.position, transform.position);
				if (!(num4 < num2))
				{
					continue;
				}
				if (num3 >= Direction && (bool)ChainObject && num <= NumberChain)
				{
					Quaternion rotation = Quaternion.LookRotation(array[j].transform.position - transform.position);
					GameObject gameObject = ((!(WeaponSystem.Pool != null)) ? Object.Instantiate(ChainObject, transform.position, rotation) : WeaponSystem.Pool.Instantiate(ChainObject, transform.position, rotation));
					DamageBase component = gameObject.GetComponent<DamageBase>();
					if ((bool)component)
					{
						component.TargetTag = TargetTag;
					}
					num++;
				}
				num2 = num4;
			}
		}
	}

	private void OnEnable()
	{
		if (actived)
		{
			chainDamage();
		}
	}
}
