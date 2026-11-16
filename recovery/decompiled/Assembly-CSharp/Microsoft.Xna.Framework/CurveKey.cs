using System;
using TrueSync;

namespace Microsoft.Xna.Framework;

public class CurveKey : IEquatable<CurveKey>, IComparable<CurveKey>
{
	private CurveContinuity continuity;

	private FP position;

	private FP tangentIn;

	private FP tangentOut;

	private FP value;

	public CurveContinuity Continuity
	{
		get
		{
			return continuity;
		}
		set
		{
			continuity = value;
		}
	}

	public FP Position => position;

	public FP TangentIn
	{
		get
		{
			return tangentIn;
		}
		set
		{
			tangentIn = value;
		}
	}

	public FP TangentOut
	{
		get
		{
			return tangentOut;
		}
		set
		{
			tangentOut = value;
		}
	}

	public FP Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public CurveKey(FP position, FP value)
		: this(position, value, 0, 0, CurveContinuity.Smooth)
	{
	}

	public CurveKey(FP position, FP value, FP tangentIn, FP tangentOut)
		: this(position, value, tangentIn, tangentOut, CurveContinuity.Smooth)
	{
	}

	public CurveKey(FP position, FP value, FP tangentIn, FP tangentOut, CurveContinuity continuity)
	{
		this.position = position;
		this.value = value;
		this.tangentIn = tangentIn;
		this.tangentOut = tangentOut;
		this.continuity = continuity;
	}

	public int CompareTo(CurveKey other)
	{
		return position.CompareTo(other.position);
	}

	public bool Equals(CurveKey other)
	{
		return this == other;
	}

	public static bool operator !=(CurveKey a, CurveKey b)
	{
		return !(a == b);
	}

	public static bool operator ==(CurveKey a, CurveKey b)
	{
		if (object.Equals(a, null))
		{
			return object.Equals(b, null);
		}
		if (object.Equals(b, null))
		{
			return object.Equals(a, null);
		}
		return a.position == b.position && a.value == b.value && a.tangentIn == b.tangentIn && a.tangentOut == b.tangentOut && a.continuity == b.continuity;
	}

	public CurveKey Clone()
	{
		return new CurveKey(position, value, tangentIn, tangentOut, continuity);
	}

	public override bool Equals(object obj)
	{
		return obj is CurveKey && (CurveKey)obj == this;
	}

	public override int GetHashCode()
	{
		return position.GetHashCode() ^ value.GetHashCode() ^ tangentIn.GetHashCode() ^ tangentOut.GetHashCode() ^ continuity.GetHashCode();
	}
}
