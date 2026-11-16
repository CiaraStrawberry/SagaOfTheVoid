public class SuperCapital_Defense : _SuperCapital
{
	public SuperCapital_Defense()
	{
		ShipClass = eShipClass.Defense;
		ShipMesh = "ScifiCruiserProtectorHullNoHangar.fbx";
		ShipMaterialRoot = "ScifiCruiserProtector";
	}

	public override void Reset()
	{
		base.Reset();
	}

	private new void Update()
	{
	}
}
