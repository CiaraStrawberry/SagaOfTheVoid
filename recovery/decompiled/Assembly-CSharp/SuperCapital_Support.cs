public class SuperCapital_Support : _SuperCapital
{
	public SuperCapital_Support()
	{
		ShipClass = eShipClass.Support;
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
