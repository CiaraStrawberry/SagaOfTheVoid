using UnityEngine;

public class Destroyer_Tank : _Destroyer
{
	public Destroyer_Tank()
	{
		ShipClass = eShipClass.Tank;
		ShipMesh = "ScifiDestroyerPhoenixHullNoHangar.fbx";
		ShipMaterialRoot = "ScifiDestroyerPhoenix";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 0f);
		component.radius = 5f;
		component.height = 20f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
