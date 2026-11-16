using UnityEngine;

public class Capital_Offense : _Capital
{
	public Capital_Offense()
	{
		ShipClass = eShipClass.Offense;
		ShipMesh = "ScifiCruiserKingswordHullNoHangar.fbx";
		ShipMaterialRoot = "ScifiCruiserKingsword";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 2.33f);
		component.radius = 7f;
		component.height = 47f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
