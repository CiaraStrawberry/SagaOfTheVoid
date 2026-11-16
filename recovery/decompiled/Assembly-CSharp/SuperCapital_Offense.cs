public class SuperCapital_Offense : _SuperCapital
{
	public SuperCapital_Offense()
	{
		ShipClass = eShipClass.Offense;
		ShipMesh = "ScifiCruiserArbiterHullNoHangar.fbx";
		ShipMaterialRoot = "ScifiCruiserArbiter";
	}

	public override void Reset()
	{
		base.Reset();
	}

	private new void Update()
	{
	}
}
