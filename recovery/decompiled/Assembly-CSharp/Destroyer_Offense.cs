using UnityEngine;

public class Destroyer_Offense : _Destroyer
{
	public Destroyer_Offense()
	{
		ShipClass = eShipClass.Offense;
		ShipMesh = "ScifiDestroyerFalconHullNoHangar.fbx";
		ShipMaterialRoot = "ScifiDestroyerFalcon";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 0f);
		component.radius = 7f;
		component.height = 20f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
