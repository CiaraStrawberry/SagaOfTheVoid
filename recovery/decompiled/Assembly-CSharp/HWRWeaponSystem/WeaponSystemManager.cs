using UnityEngine;

namespace HWRWeaponSystem;

public class WeaponSystemManager : MonoBehaviour
{
	private void Awake()
	{
		WeaponSystem.Pool = (ObjectPoolManager)Object.FindObjectOfType(typeof(ObjectPoolManager));
	}

	private void Start()
	{
	}
}
