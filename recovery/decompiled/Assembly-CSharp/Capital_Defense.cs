using Apex.Units;
using UnityEngine;

public class Capital_Defense : _Capital
{
	public Capital_Defense()
	{
		ShipClass = eShipClass.Defense;
		ShipMesh = "ScifiHeavyDestroyerWolfhoundHullNoHangar.fbx";
		ShipMaterialRoot = "ScifiHeavyDestroyerWolfhound";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 2.33f);
		component.radius = 12f;
		component.height = 40f;
		component.direction = 2;
		UnitComponent component2 = GetComponent<UnitComponent>();
		component2.fieldOfView = 200f;
		component2.radius = 12f;
		component2.determination = 1;
	}

	private new void Update()
	{
	}
}
