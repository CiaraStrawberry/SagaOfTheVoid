using UnityEngine;

public class Destroyer_Support : _Destroyer
{
	public Destroyer_Support()
	{
		ShipClass = eShipClass.Support;
		ShipMesh = "ScifiDestroyerOlympusHullNoHangar.fbx";
		ShipMaterialRoot = "ScifiDestroyerOlympus";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 0f);
		component.radius = 8f;
		component.height = 25f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
