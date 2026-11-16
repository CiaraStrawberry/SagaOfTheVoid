using UnityEngine;

public class Cruiser_Offense : _Cruiser
{
	public Cruiser_Offense()
	{
		ShipClass = eShipClass.Offense;
		ShipMesh = "ScifiFrigateStarmasterHull.fbx";
		ShipMaterialRoot = "ScifiFrigateStarmaster";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 0f);
		component.radius = 4f;
		component.height = 12f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
