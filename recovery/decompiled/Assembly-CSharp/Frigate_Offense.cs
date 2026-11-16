using UnityEngine;

public class Frigate_Offense : _Frigate
{
	public Frigate_Offense()
	{
		ShipClass = eShipClass.Offense;
		ShipMesh = "ScifiFrigatePredatorHull.fbx";
		ShipMaterialRoot = "ScifiFrigatePredator";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 0f);
		component.radius = 2.5f;
		component.height = 9f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
