using UnityEngine;

namespace Devdog.SciFiDesign.UI;

public class Rotate : MonoBehaviour
{
	public Vector3 rotationAxis;

	protected void Update()
	{
		transform.Rotate(rotationAxis * Time.deltaTime);
	}
}
