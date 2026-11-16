using UnityEngine;

public class KillParentOnStartScript : MonoBehaviour
{
	private void Start()
	{
		transform.parent = null;
	}

	private void Update()
	{
	}
}
