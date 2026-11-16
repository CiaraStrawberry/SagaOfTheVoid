using UnityEngine;

public class Frigate_Tank : _Frigate
{
	public Frigate_Tank()
	{
		ShipClass = eShipClass.Tank;
		ShipMesh = "ScifiFrigateWyrmHull.fbx";
		ShipMaterialRoot = "ScifiFrigateWyrm";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 0.51f);
		component.radius = 1.2f;
		component.height = 9f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
