namespace TrueSync.Physics3D;

public class ContactSettings
{
	public enum MaterialCoefficientMixingType
	{
		TakeMaximum,
		TakeMinimum,
		UseAverage
	}

	internal FP maximumBias = 10;

	internal FP bias = 25 * FP.EN2;

	internal FP minVelocity = FP.EN3;

	internal FP allowedPenetration = FP.EN2;

	internal FP breakThreshold = FP.EN2;

	internal MaterialCoefficientMixingType materialMode = MaterialCoefficientMixingType.UseAverage;

	public FP MaximumBias
	{
		get
		{
			return maximumBias;
		}
		set
		{
			maximumBias = value;
		}
	}

	public FP BiasFactor
	{
		get
		{
			return bias;
		}
		set
		{
			bias = value;
		}
	}

	public FP MinimumVelocity
	{
		get
		{
			return minVelocity;
		}
		set
		{
			minVelocity = value;
		}
	}

	public FP AllowedPenetration
	{
		get
		{
			return allowedPenetration;
		}
		set
		{
			allowedPenetration = value;
		}
	}

	public FP BreakThreshold
	{
		get
		{
			return breakThreshold;
		}
		set
		{
			breakThreshold = value;
		}
	}

	public MaterialCoefficientMixingType MaterialCoefficientMixing
	{
		get
		{
			return materialMode;
		}
		set
		{
			materialMode = value;
		}
	}
}
