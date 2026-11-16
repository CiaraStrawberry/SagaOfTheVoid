using UnityEngine;

public class Capital_Support : _Capital
{
	public Capital_Support()
	{
		ShipClass = eShipClass.Support;
		ShipMesh = "ScifiCruiserCerberusHullNoHangar.fbx";
		ShipMaterialRoot = "ScifiCruiserCerberus";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 0f);
		component.radius = 12f;
		component.height = 40f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
