using System.Collections.Generic;

namespace TrueSync.Physics2D;

public class VelocityLimitController : Controller
{
	public bool LimitAngularVelocity = true;

	public bool LimitLinearVelocity = true;

	private List<Body> _bodies = new List<Body>();

	private FP _maxAngularSqared;

	private FP _maxAngularVelocity;

	private FP _maxLinearSqared;

	private FP _maxLinearVelocity;

	public FP MaxAngularVelocity
	{
		get
		{
			return _maxAngularVelocity;
		}
		set
		{
			_maxAngularVelocity = value;
			_maxAngularSqared = _maxAngularVelocity * _maxAngularVelocity;
		}
	}

	public FP MaxLinearVelocity
	{
		get
		{
			return _maxLinearVelocity;
		}
		set
		{
			_maxLinearVelocity = value;
			_maxLinearSqared = _maxLinearVelocity * _maxLinearVelocity;
		}
	}

	public VelocityLimitController()
		: base(ControllerType.VelocityLimitController)
	{
		MaxLinearVelocity = Settings.MaxTranslation;
		MaxAngularVelocity = Settings.MaxRotation;
	}

	public VelocityLimitController(FP maxLinearVelocity, FP maxAngularVelocity)
		: base(ControllerType.VelocityLimitController)
	{
		if (maxLinearVelocity == 0 || maxLinearVelocity == FP.MaxValue)
		{
			LimitLinearVelocity = false;
		}
		if (maxAngularVelocity == 0 || maxAngularVelocity == FP.MaxValue)
		{
			LimitAngularVelocity = false;
		}
		MaxLinearVelocity = maxLinearVelocity;
		MaxAngularVelocity = maxAngularVelocity;
	}

	public override void Update(FP dt)
	{
		foreach (Body body in _bodies)
		{
			if (!IsActiveOn(body))
			{
				continue;
			}
			if (LimitLinearVelocity)
			{
				FP fP = dt * body._linearVelocity.x;
				FP fP2 = dt * body._linearVelocity.y;
				FP fP3 = fP * fP + fP2 * fP2;
				if (fP3 > dt * _maxLinearSqared)
				{
					FP fP4 = FP.Sqrt(fP3);
					FP fP5 = _maxLinearVelocity / fP4;
					body._linearVelocity.x *= fP5;
					body._linearVelocity.y *= fP5;
				}
			}
			if (LimitAngularVelocity)
			{
				FP fP6 = dt * body._angularVelocity;
				if (fP6 * fP6 > _maxAngularSqared)
				{
					FP fP7 = _maxAngularVelocity / FP.Abs(fP6);
					body._angularVelocity *= fP7;
				}
			}
		}
	}

	public void AddBody(Body body)
	{
		_bodies.Add(body);
	}

	public void RemoveBody(Body body)
	{
		_bodies.Remove(body);
	}
}
