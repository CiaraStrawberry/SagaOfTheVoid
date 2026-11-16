using System;
using System.Collections.Generic;
using System.Text;

namespace TrueSync.Physics2D;

public class Path
{
	public List<TSVector2> ControlPoints;

	private FP _deltaT;

	public bool Closed { get; set; }

	public Path()
	{
		ControlPoints = new List<TSVector2>();
	}

	public Path(TSVector2[] vertices)
	{
		ControlPoints = new List<TSVector2>(vertices.Length);
		for (int i = 0; i < vertices.Length; i++)
		{
			Add(vertices[i]);
		}
	}

	public Path(IList<TSVector2> vertices)
	{
		ControlPoints = new List<TSVector2>(vertices.Count);
		for (int i = 0; i < vertices.Count; i++)
		{
			Add(vertices[i]);
		}
	}

	public int NextIndex(int index)
	{
		if (index == ControlPoints.Count - 1)
		{
			return 0;
		}
		return index + 1;
	}

	public int PreviousIndex(int index)
	{
		if (index == 0)
		{
			return ControlPoints.Count - 1;
		}
		return index - 1;
	}

	public void Translate(ref TSVector2 vector)
	{
		for (int i = 0; i < ControlPoints.Count; i++)
		{
			ControlPoints[i] = TSVector2.Add(ControlPoints[i], vector);
		}
	}

	public void Scale(ref TSVector2 value)
	{
		for (int i = 0; i < ControlPoints.Count; i++)
		{
			ControlPoints[i] = TSVector2.Multiply(ControlPoints[i], value);
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < ControlPoints.Count; i++)
		{
			stringBuilder.Append(ControlPoints[i].ToString());
			if (i < ControlPoints.Count - 1)
			{
				stringBuilder.Append(" ");
			}
		}
		return stringBuilder.ToString();
	}

	public Vertices GetVertices(int divisions)
	{
		Vertices vertices = new Vertices();
		FP fP = 1f / (float)divisions;
		for (FP fP2 = 0; fP2 < 1f; fP2 += fP)
		{
			vertices.Add(GetPosition(fP2));
		}
		return vertices;
	}

	public TSVector2 GetPosition(FP time)
	{
		if (ControlPoints.Count < 2)
		{
			throw new Exception("You need at least 2 control points to calculate a position.");
		}
		TSVector2 result;
		if (Closed)
		{
			Add(ControlPoints[0]);
			_deltaT = 1f / (float)(ControlPoints.Count - 1);
			int num = (int)(long)(time / _deltaT);
			int num2 = num - 1;
			if (num2 < 0)
			{
				num2 += ControlPoints.Count - 1;
			}
			else if (num2 >= ControlPoints.Count - 1)
			{
				num2 -= ControlPoints.Count - 1;
			}
			int num3 = num;
			if (num3 < 0)
			{
				num3 += ControlPoints.Count - 1;
			}
			else if (num3 >= ControlPoints.Count - 1)
			{
				num3 -= ControlPoints.Count - 1;
			}
			int num4 = num + 1;
			if (num4 < 0)
			{
				num4 += ControlPoints.Count - 1;
			}
			else if (num4 >= ControlPoints.Count - 1)
			{
				num4 -= ControlPoints.Count - 1;
			}
			int num5 = num + 2;
			if (num5 < 0)
			{
				num5 += ControlPoints.Count - 1;
			}
			else if (num5 >= ControlPoints.Count - 1)
			{
				num5 -= ControlPoints.Count - 1;
			}
			FP amount = (time - _deltaT * num) / _deltaT;
			result = TSVector2.CatmullRom(ControlPoints[num2], ControlPoints[num3], ControlPoints[num4], ControlPoints[num5], amount);
			RemoveAt(ControlPoints.Count - 1);
		}
		else
		{
			int num6 = (int)(long)(time / _deltaT);
			int num7 = num6 - 1;
			if (num7 < 0)
			{
				num7 = 0;
			}
			else if (num7 >= ControlPoints.Count - 1)
			{
				num7 = ControlPoints.Count - 1;
			}
			int num8 = num6;
			if (num8 < 0)
			{
				num8 = 0;
			}
			else if (num8 >= ControlPoints.Count - 1)
			{
				num8 = ControlPoints.Count - 1;
			}
			int num9 = num6 + 1;
			if (num9 < 0)
			{
				num9 = 0;
			}
			else if (num9 >= ControlPoints.Count - 1)
			{
				num9 = ControlPoints.Count - 1;
			}
			int num10 = num6 + 2;
			if (num10 < 0)
			{
				num10 = 0;
			}
			else if (num10 >= ControlPoints.Count - 1)
			{
				num10 = ControlPoints.Count - 1;
			}
			FP amount2 = (time - _deltaT * num6) / _deltaT;
			result = TSVector2.CatmullRom(ControlPoints[num7], ControlPoints[num8], ControlPoints[num9], ControlPoints[num10], amount2);
		}
		return result;
	}

	public TSVector2 GetPositionNormal(FP time)
	{
		FP time2 = time + 0.0001f;
		TSVector2 value = GetPosition(time);
		TSVector2 value2 = GetPosition(time2);
		TSVector2.Subtract(ref value, ref value2, out var result);
		TSVector2 value3 = default;
		value3.x = -result.y;
		value3.y = result.x;
		TSVector2.Normalize(ref value3, out value3);
		return value3;
	}

	public void Add(TSVector2 point)
	{
		ControlPoints.Add(point);
		_deltaT = 1f / (float)(ControlPoints.Count - 1);
	}

	public void Remove(TSVector2 point)
	{
		ControlPoints.Remove(point);
		_deltaT = 1f / (float)(ControlPoints.Count - 1);
	}

	public void RemoveAt(int index)
	{
		ControlPoints.RemoveAt(index);
		_deltaT = 1f / (float)(ControlPoints.Count - 1);
	}

	public FP GetLength()
	{
		List<TSVector2> vertices = GetVertices(ControlPoints.Count * 25);
		FP result = 0;
		for (int i = 1; i < vertices.Count; i++)
		{
			result += TSVector2.Distance(vertices[i - 1], vertices[i]);
		}
		if (Closed)
		{
			result += TSVector2.Distance(vertices[ControlPoints.Count - 1], vertices[0]);
		}
		return result;
	}

	public List<TSVector> SubdivideEvenly(int divisions)
	{
		List<TSVector> list = new List<TSVector>();
		FP length = GetLength();
		FP fP = length / divisions + 0.001f;
		FP fP2 = 0f;
		TSVector2 value = ControlPoints[0];
		TSVector2 position = GetPosition(fP2);
		while (fP * 0.5f >= TSVector2.Distance(value, position))
		{
			position = GetPosition(fP2);
			fP2 += (FP)0.0001f;
			if (fP2 >= 1f)
			{
				break;
			}
		}
		value = position;
		for (int i = 1; i < divisions; i++)
		{
			TSVector2 positionNormal = GetPositionNormal(fP2);
			FP z = FP.Atan2(positionNormal.y, positionNormal.x);
			list.Add(new TSVector(position.x, position.y, z));
			while (fP >= TSVector2.Distance(value, position))
			{
				position = GetPosition(fP2);
				fP2 += (FP)1E-05f;
				if (fP2 >= 1f)
				{
					break;
				}
			}
			if (fP2 >= 1f)
			{
				break;
			}
			value = position;
		}
		return list;
	}
}
