using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroyOnLoad_perName : MonoBehaviour
{
	public static List<string> list = new List<string>();

	private void Awake()
	{
		if (list.Contains(name))
		{
			Object.Destroy(gameObject);
		}
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(gameObject);
		list.Add(name);
	}

	private void Update()
	{
	}
}
