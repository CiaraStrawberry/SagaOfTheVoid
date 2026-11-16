using System;
using TrueSync;

namespace Microsoft.Xna.Framework;

public class Curve
{
	private CurveKeyCollection keys;

	private CurveLoopType postLoop;

	private CurveLoopType preLoop;

	public bool IsConstant => keys.Count <= 1;

	public CurveKeyCollection Keys => keys;

	public CurveLoopType PostLoop
	{
		get
		{
			return postLoop;
		}
		set
		{
			postLoop = value;
		}
	}

	public CurveLoopType PreLoop
	{
		get
		{
			return preLoop;
		}
		set
		{
			preLoop = value;
		}
	}

	public Curve()
	{
		keys = new CurveKeyCollection();
	}

	public void ComputeTangent(int keyIndex, CurveTangent tangentInType, CurveTangent tangentOutType)
	{
		throw new NotImplementedException();
	}

	public void ComputeTangent(int keyIndex, CurveTangent tangentType)
	{
		ComputeTangent(keyIndex, tangentType, tangentType);
	}

	public void ComputeTangents(CurveTangent tangentInType, CurveTangent tangentOutType)
	{
		throw new NotImplementedException();
	}

	public void ComputeTangents(CurveTangent tangentType)
	{
		ComputeTangents(tangentType, tangentType);
	}

	public Curve Clone()
	{
		Curve curve = new Curve();
		curve.keys = keys.Clone();
		curve.preLoop = preLoop;
		curve.postLoop = postLoop;
		return curve;
	}

	public FP Evaluate(FP position)
	{
		CurveKey curveKey = keys[0];
		CurveKey curveKey2 = keys[keys.Count - 1];
		if (position < curveKey.Position)
		{
			switch (PreLoop)
			{
			case CurveLoopType.Constant:
				return curveKey.Value;
			case CurveLoopType.Linear:
				return curveKey.Value - curveKey.TangentIn * (curveKey.Position - position);
			case CurveLoopType.Cycle:
			{
				int numberOfCycle = GetNumberOfCycle(position);
				FP position2 = position - numberOfCycle * (curveKey2.Position - curveKey.Position);
				return GetCurvePosition(position2);
			}
			case CurveLoopType.CycleOffset:
			{
				int numberOfCycle = GetNumberOfCycle(position);
				FP position2 = position - numberOfCycle * (curveKey2.Position - curveKey.Position);
				return GetCurvePosition(position2) + numberOfCycle * (curveKey2.Value - curveKey.Value);
			}
			case CurveLoopType.Oscillate:
			{
				int numberOfCycle = GetNumberOfCycle(position);
				FP position2 = (((float)numberOfCycle % 2f != 0f) ? (curveKey2.Position - position + curveKey.Position + numberOfCycle * (curveKey2.Position - curveKey.Position)) : (position - numberOfCycle * (curveKey2.Position - curveKey.Position)));
				return GetCurvePosition(position2);
			}
			}
		}
		else if (position > curveKey2.Position)
		{
			switch (PostLoop)
			{
			case CurveLoopType.Constant:
				return curveKey2.Value;
			case CurveLoopType.Linear:
				return curveKey2.Value + curveKey.TangentOut * (position - curveKey2.Position);
			case CurveLoopType.Cycle:
			{
				int numberOfCycle2 = GetNumberOfCycle(position);
				FP fP = position - numberOfCycle2 * (curveKey2.Position - curveKey.Position);
				return GetCurvePosition(fP);
			}
			case CurveLoopType.CycleOffset:
			{
				int numberOfCycle2 = GetNumberOfCycle(position);
				FP fP = position - numberOfCycle2 * (curveKey2.Position - curveKey.Position);
				return GetCurvePosition(fP) + numberOfCycle2 * (curveKey2.Value - curveKey.Value);
			}
			case CurveLoopType.Oscillate:
			{
				int numberOfCycle2 = GetNumberOfCycle(position);
				FP fP = position - numberOfCycle2 * (curveKey2.Position - curveKey.Position);
				fP = (((float)numberOfCycle2 % 2f != 0f) ? (curveKey2.Position - position + curveKey.Position + numberOfCycle2 * (curveKey2.Position - curveKey.Position)) : (position - numberOfCycle2 * (curveKey2.Position - curveKey.Position)));
				return GetCurvePosition(fP);
			}
			}
		}
		return GetCurvePosition(position);
	}

	private int GetNumberOfCycle(FP position)
	{
		FP fP = (position - keys[0].Position) / (keys[keys.Count - 1].Position - keys[0].Position);
		if (fP < 0f)
		{
			fP -= (FP)1;
		}
		return (int)(long)fP;
	}

	private FP GetCurvePosition(FP position)
	{
		CurveKey curveKey = keys[0];
		for (int i = 1; i < keys.Count; i++)
		{
			CurveKey curveKey2 = Keys[i];
			if (curveKey2.Position >= position)
			{
				if (curveKey.Continuity == CurveContinuity.Step)
				{
					if (position >= 1f)
					{
						return curveKey2.Value;
					}
					return curveKey.Value;
				}
				FP fP = (position - curveKey.Position) / (curveKey2.Position - curveKey.Position);
				FP fP2 = fP * fP;
				FP fP3 = fP2 * fP;
				return (2 * fP3 - 3 * fP2 + 1f) * curveKey.Value + (fP3 - 2 * fP2 + fP) * curveKey.TangentOut + (3 * fP2 - 2 * fP3) * curveKey2.Value + (fP3 - fP2) * curveKey2.TangentIn;
			}
			curveKey = curveKey2;
		}
		return 0f;
	}
}
