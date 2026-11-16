using UnityEngine;

public class F3DPlanet : MonoBehaviour
{
	public float RotationRate;

	public float OrbitRate;

	public Transform OrbitPoint;

	public bool ShowOrbit;

	private float distToOrbitPoint;

	private Vector3 orbitAxis;

	private Vector3 pointToPlanetDir;

	private void Start()
	{
		if ((bool)OrbitPoint)
		{
			distToOrbitPoint = Vector3.Distance(transform.position, OrbitPoint.position);
			orbitAxis = transform.up;
			pointToPlanetDir = Vector3.Normalize(transform.position - OrbitPoint.position);
		}
	}

	private void Update()
	{
		transform.rotation *= Quaternion.Euler(0f, RotationRate * Time.deltaTime, 0f);
		if ((bool)OrbitPoint)
		{
			pointToPlanetDir = Quaternion.AngleAxis(OrbitRate * Time.deltaTime, orbitAxis) * pointToPlanetDir;
			transform.position = OrbitPoint.position + pointToPlanetDir * distToOrbitPoint;
		}
	}
}
