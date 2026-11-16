namespace TrueSync.Physics2D;

internal class TOIClone2D
{
	public DistanceProxy ProxyA = new DistanceProxy();

	public DistanceProxy ProxyB = new DistanceProxy();

	public Sweep SweepA;

	public Sweep SweepB;

	public FP TMax;

	public void Clone(TOIInput input)
	{
		ProxyA.Radius = input.ProxyA.Radius;
		ProxyA.Vertices.Clear();
		ProxyA.Vertices.AddRange(input.ProxyA.Vertices);
		ProxyB.Radius = input.ProxyB.Radius;
		ProxyB.Vertices.Clear();
		ProxyB.Vertices.AddRange(input.ProxyB.Vertices);
		SweepA = input.SweepA;
		SweepB = input.SweepB;
		TMax = input.TMax;
	}

	public void Restore(TOIInput input)
	{
		input.ProxyA.Radius = ProxyA.Radius;
		input.ProxyA.Vertices.Clear();
		input.ProxyA.Vertices.AddRange(ProxyA.Vertices);
		input.ProxyB.Radius = ProxyB.Radius;
		input.ProxyB.Vertices.Clear();
		input.ProxyB.Vertices.AddRange(ProxyB.Vertices);
		input.SweepA = SweepA;
		input.SweepB = SweepB;
		input.TMax = TMax;
	}
}
