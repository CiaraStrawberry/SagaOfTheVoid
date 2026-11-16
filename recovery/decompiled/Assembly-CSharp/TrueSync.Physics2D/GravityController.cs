using System.Collections.Generic;

namespace TrueSync.Physics2D;

public class GravityController : Controller
{
	public FP MinRadius { get; set; }

	public FP MaxRadius { get; set; }

	public FP Strength { get; set; }

	public GravityType GravityType { get; set; }

	public List<Body> Bodies { get; set; }

	public List<TSVector2> Points { get; set; }

	public GravityController(FP strength)
		: base(ControllerType.GravityController)
	{
		Strength = strength;
		MaxRadius = FP.MaxValue;
		GravityType = GravityType.DistanceSquared;
		Points = new List<TSVector2>();
		Bodies = new List<Body>();
	}

	public GravityController(FP strength, FP maxRadius, FP minRadius)
		: base(ControllerType.GravityController)
	{
		MinRadius = minRadius;
		MaxRadius = maxRadius;
		Strength = strength;
		GravityType = GravityType.DistanceSquared;
		Points = new List<TSVector2>();
		Bodies = new List<Body>();
	}

	public override void Update(FP dt)
	{
		TSVector2 force = TSVector2.zero;
		foreach (Body body in World.BodyList)
		{
			if (!IsActiveOn(body))
			{
				continue;
			}
			foreach (Body body2 in Bodies)
			{
				if (body == body2 || (body.IsStatic && body2.IsStatic) || !body2.Enabled)
				{
					continue;
				}
				TSVector2 tSVector = body2.Position - body.Position;
				FP fP = tSVector.LengthSquared();
				if (!(fP <= Settings.Epsilon) && !(fP > MaxRadius * MaxRadius) && !(fP < MinRadius * MinRadius))
				{
					switch (GravityType)
					{
					case GravityType.DistanceSquared:
						force = Strength / fP * body.Mass * body2.Mass * tSVector;
						break;
					case GravityType.Linear:
						force = Strength / FP.Sqrt(fP) * body.Mass * body2.Mass * tSVector;
						break;
					}
					body.ApplyForce(ref force);
				}
			}
			foreach (TSVector2 point in Points)
			{
				TSVector2 tSVector2 = point - body.Position;
				FP fP2 = tSVector2.LengthSquared();
				if (!(fP2 <= Settings.Epsilon) && !(fP2 > MaxRadius * MaxRadius) && !(fP2 < MinRadius * MinRadius))
				{
					switch (GravityType)
					{
					case GravityType.DistanceSquared:
						force = Strength / fP2 * body.Mass * tSVector2;
						break;
					case GravityType.Linear:
						force = Strength / FP.Sqrt(fP2) * body.Mass * tSVector2;
						break;
					}
					body.ApplyForce(ref force);
				}
			}
		}
	}

	public void AddBody(Body body)
	{
		Bodies.Add(body);
	}

	public void AddPoint(TSVector2 point)
	{
		Points.Add(point);
	}
}
