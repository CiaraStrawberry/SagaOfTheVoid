using UnityEngine;

public class Selectionposcontroller : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		transform.position = transform.parent.position;
	}
}
