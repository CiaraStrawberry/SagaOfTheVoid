namespace TrueSync.Physics2D;

public class SimpleWindForce : AbstractForceController
{
	public TSVector2 Direction { get; set; }

	public FP Divergence { get; set; }

	public bool IgnorePosition { get; set; }

	public override void ApplyForce(FP dt, FP strength)
	{
		foreach (Body body in World.BodyList)
		{
			FP decayMultiplier = GetDecayMultiplier(body);
			if (!(decayMultiplier != 0))
			{
				continue;
			}
			TSVector2 tSVector;
			if (ForceType == ForceTypes.Point)
			{
				tSVector = body.Position - Position;
			}
			else
			{
				Direction.Normalize();
				tSVector = Direction;
				if (tSVector.magnitude == 0)
				{
					tSVector = new TSVector2(0, 1);
				}
			}
			if (Variation != 0)
			{
				FP fP = TSRandom.value * TSMath.Clamp(Variation, 0, 1);
				tSVector.Normalize();
				body.ApplyForce(tSVector * strength * decayMultiplier * fP);
			}
			else
			{
				tSVector.Normalize();
				body.ApplyForce(tSVector * strength * decayMultiplier);
			}
		}
	}
}
