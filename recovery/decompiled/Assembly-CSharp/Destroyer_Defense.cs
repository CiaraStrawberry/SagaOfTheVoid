using UnityEngine;

public class Destroyer_Defense : _Destroyer
{
	public Destroyer_Defense()
	{
		ShipClass = eShipClass.Defense;
		ShipMesh = "ScifiDestroyerTitanHullNoHangar.fbx";
		ShipMaterialRoot = "ScifiDestroyerTitan";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 0f);
		component.radius = 6f;
		component.height = 25f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
