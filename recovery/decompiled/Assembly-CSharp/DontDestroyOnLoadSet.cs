using UnityEngine;

public class DontDestroyOnLoadSet : MonoBehaviour
{
	private void Start()
	{
		Object.DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
	}
}
