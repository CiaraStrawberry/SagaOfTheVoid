using UnityEngine;

public class Capital_Tank : _Capital
{
	public Capital_Tank()
	{
		ShipClass = eShipClass.Tank;
		ShipMesh = "ScifiBattleshipPrometheusHullNoHangar.fbx";
		ShipMaterialRoot = "ScifiBattleshipPrometheus";
	}

	public override void Reset()
	{
		base.Reset();
		CapsuleCollider component = GetComponent<CapsuleCollider>();
		component.center = new Vector3(0f, 0f, 0f);
		component.radius = 5f;
		component.height = 45f;
		component.direction = 2;
	}

	private new void Update()
	{
	}
}
