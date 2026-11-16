using UnityEngine;

public class Cruiser_Defense : _Cruiser
{
	public Cruiser_Defense()
	{
		ShipClass = eShipClass.Defense;
		ShipMesh = "ScifiDestroyerVanguardHullNoHangar.fbx";
		ShipMaterialRoot = "ScifiDestroyerVanguard";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 0f);
		component.radius = 2.5f;
		component.height = 16f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
