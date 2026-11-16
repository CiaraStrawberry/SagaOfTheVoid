using TrueSync;
using UnityEngine;

public class bliplinecontroller : MonoBehaviour
{
	public LineRenderer Lineren;

	public Vector3 targetgam;

	public TSTransform source;

	private void Start()
	{
		Lineren = GetComponent<LineRenderer>();
	}

	private void Update()
	{
		if (targetgam != new Vector3(0f, 0f, 0f))
		{
			Lineren.SetPosition(0, transform.position);
			Lineren.SetPosition(1, transform.parent.TransformPoint(targetgam));
		}
		else
		{
			Lineren.SetPosition(0, transform.position);
			Lineren.SetPosition(1, transform.position);
		}
	}
}
