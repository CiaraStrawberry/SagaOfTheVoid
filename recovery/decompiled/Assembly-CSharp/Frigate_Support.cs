using UnityEngine;

public class Frigate_Support : _Frigate
{
	public Frigate_Support()
	{
		ShipClass = eShipClass.Support;
		ShipMesh = "ScifiFrigateBarracudaHullA.fbx";
		ShipMaterialRoot = "ScifiFrigateBarracuda";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 0.36f);
		component.radius = 2.5f;
		component.height = 8f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
