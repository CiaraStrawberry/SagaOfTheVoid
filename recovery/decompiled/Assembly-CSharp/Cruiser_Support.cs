using UnityEngine;

public class Cruiser_Support : _Cruiser
{
	public Cruiser_Support()
	{
		ShipClass = eShipClass.Support;
		ShipMesh = "ScifiFrigateBarracudaHullB.fbx";
		ShipMaterialRoot = "ScifiFrigateBarracuda";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 0.93f);
		component.radius = 5f;
		component.height = 11f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
