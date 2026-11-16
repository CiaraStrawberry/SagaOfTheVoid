public class SuperCapital_Tank : _SuperCapital
{
	public SuperCapital_Tank()
	{
		ShipClass = eShipClass.Tank;
		ShipMesh = string.Empty;
		ShipMaterialRoot = string.Empty;
	}

	public override void Reset()
	{
		base.Reset();
	}

	private new void Update()
	{
	}
}
