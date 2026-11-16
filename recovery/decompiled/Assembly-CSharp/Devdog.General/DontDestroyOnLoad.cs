using UnityEngine;

namespace Devdog.General;

public class DontDestroyOnLoad : MonoBehaviour
{
	protected void Start()
	{
		Object.DontDestroyOnLoad(this);
	}
}
