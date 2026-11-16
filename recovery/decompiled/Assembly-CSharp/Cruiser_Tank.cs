using UnityEngine;

public class Cruiser_Tank : _Cruiser
{
	public Cruiser_Tank()
	{
		ShipClass = eShipClass.Tank;
		ShipMesh = "ScifiFrigateRapierHull.fbx";
		ShipMaterialRoot = "ScifiFrigateRapier";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 0.62f);
		component.radius = 4f;
		component.height = 13f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
