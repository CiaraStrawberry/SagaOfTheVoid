using Microsoft.Xna.Framework;

namespace TrueSync.Physics2D;

public abstract class AbstractForceController : Controller
{
	public enum DecayModes
	{
		None,
		Step,
		Linear,
		InverseSquare,
		Curve
	}

	public enum ForceTypes
	{
		Point,
		Line,
		Area
	}

	public enum TimingModes
	{
		Switched,
		Triggered,
		Curve
	}

	public Curve DecayCurve;

	public ForceTypes ForceType;

	protected TSRandom Randomize;

	public Curve StrengthCurve;

	public FP Strength { get; set; }

	public TSVector2 Position { get; set; }

	public FP MaximumSpeed { get; set; }

	public FP MaximumForce { get; set; }

	public TimingModes TimingMode { get; set; }

	public FP ImpulseTime { get; private set; }

	public FP ImpulseLength { get; set; }

	public bool Triggered { get; private set; }

	public FP Variation { get; set; }

	public DecayModes DecayMode { get; set; }

	public FP DecayStart { get; set; }

	public FP DecayEnd { get; set; }

	public AbstractForceController()
		: base(ControllerType.AbstractForceController)
	{
		Enabled = true;
		Strength = 1f;
		Position = new TSVector2(0, 0);
		MaximumSpeed = 100f;
		TimingMode = TimingModes.Switched;
		ImpulseTime = 0f;
		ImpulseLength = 1f;
		Triggered = false;
		StrengthCurve = new Curve();
		Variation = 0f;
		Randomize = TSRandom.New(1234);
		DecayMode = DecayModes.None;
		DecayCurve = new Curve();
		DecayStart = 0f;
		DecayEnd = 0f;
		StrengthCurve.Keys.Add(new CurveKey(0, 5));
		StrengthCurve.Keys.Add(new CurveKey(0.1f, 5));
		StrengthCurve.Keys.Add(new CurveKey(0.2f, -4));
		StrengthCurve.Keys.Add(new CurveKey(1f, 0));
	}

	public AbstractForceController(TimingModes mode)
		: base(ControllerType.AbstractForceController)
	{
		TimingMode = mode;
		switch (mode)
		{
		case TimingModes.Switched:
			Enabled = true;
			break;
		case TimingModes.Triggered:
			Enabled = false;
			break;
		case TimingModes.Curve:
			Enabled = false;
			break;
		}
	}

	protected FP GetDecayMultiplier(Body body)
	{
		FP magnitude = (body.Position - Position).magnitude;
		switch (DecayMode)
		{
		case DecayModes.None:
			return 1f;
		case DecayModes.Step:
			if (magnitude < DecayEnd)
			{
				return 1f;
			}
			return 0f;
		case DecayModes.Linear:
			if (magnitude < DecayStart)
			{
				return 1f;
			}
			if (magnitude > DecayEnd)
			{
				return 0f;
			}
			return DecayEnd - DecayStart / magnitude - DecayStart;
		case DecayModes.InverseSquare:
			if (magnitude < DecayStart)
			{
				return 1f;
			}
			return 1f / ((magnitude - DecayStart) * (magnitude - DecayStart));
		case DecayModes.Curve:
			if (magnitude < DecayStart)
			{
				return 1f;
			}
			return DecayCurve.Evaluate(magnitude - DecayStart);
		default:
			return 1f;
		}
	}

	public void Trigger()
	{
		Triggered = true;
		ImpulseTime = 0;
	}

	public override void Update(FP dt)
	{
		switch (TimingMode)
		{
		case TimingModes.Switched:
			if (Enabled)
			{
				ApplyForce(dt, Strength);
			}
			break;
		case TimingModes.Triggered:
			if (Enabled && Triggered)
			{
				if (ImpulseTime < ImpulseLength)
				{
					ApplyForce(dt, Strength);
					ImpulseTime += dt;
				}
				else
				{
					Triggered = false;
				}
			}
			break;
		case TimingModes.Curve:
			if (Enabled && Triggered)
			{
				if (ImpulseTime < ImpulseLength)
				{
					ApplyForce(dt, Strength * StrengthCurve.Evaluate(ImpulseTime));
					ImpulseTime += dt;
				}
				else
				{
					Triggered = false;
				}
			}
			break;
		}
	}

	public abstract void ApplyForce(FP dt, FP strength);
}
