using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class MathManipulatorProperty
{
	public MATHMANIPULATORTYPE type;

	public MATHMANIPULATORPROPERTY property;

	public MATHMANIPULATORCLAMP clamp;

	public float clampCeil = 10f;

	public float clampFloor;

	public float value = 1f;

	public Vector3 value3 = Vector3.up;

	public float rate = 1f;

	public Vector3 rate3 = Vector3.up;

	private Vector3 value3Position;

	public void Update(Vector3 position)
	{
		value3Position = position + value3;
	}

	public Vector3 Evaluate(Vector3 inValue, float time)
	{
		if (clamp == MATHMANIPULATORCLAMP.ClampInValue)
		{
			inValue = Clamp(inValue);
		}
		switch (type)
		{
		case MATHMANIPULATORTYPE.Sin:
		{
			Vector3 result = new Vector3((value3.x == 0f) ? inValue.x : ClampOut(inValue.x + Mathf.Sin(time * rate3.x) * value3.x), (value3.y == 0f) ? inValue.y : ClampOut(inValue.y + Mathf.Sin(time * rate3.y) * value3.y), (value3.z == 0f) ? inValue.z : ClampOut(inValue.z + Mathf.Sin(time * rate3.z) * value3.z));
			return result;
		}
		case MATHMANIPULATORTYPE.Cos:
			return Clamp(new Vector3((value3.x == 0f) ? inValue.x : ClampOut(inValue.x + Mathf.Cos(time * rate3.x) * value3.x), (value3.y == 0f) ? inValue.y : ClampOut(inValue.y + Mathf.Cos(time * rate3.y) * value3.y), (value3.z == 0f) ? inValue.z : ClampOut(inValue.z + Mathf.Cos(time * rate3.z) * value3.z)));
		case MATHMANIPULATORTYPE.Lerp:
			return Clamp(new Vector3((value3.x == 0f) ? inValue.x : Mathf.Lerp(inValue.x, ClampOut(value3.x), time * rate3.x), (value3.y == 0f) ? inValue.y : Mathf.Lerp(inValue.y, ClampOut(value3.y), time * rate3.y), (value3.z == 0f) ? inValue.z : Mathf.Lerp(inValue.z, ClampOut(value3.z), time * rate3.z)));
		case MATHMANIPULATORTYPE.Add:
			return inValue + ClampOut(Vector3.Scale(value3, rate3) * time);
		case MATHMANIPULATORTYPE.Subtract:
			return inValue - ClampOut(Vector3.Scale(value3, rate3) * time);
		default:
			return inValue;
		}
	}

	public Vector3 EvaluatePosition(Vector3 inValue, float time)
	{
		if (clamp == MATHMANIPULATORCLAMP.ClampInValue)
		{
			inValue = Clamp(inValue);
		}
		return type switch
		{
			MATHMANIPULATORTYPE.Sin => ClampPosition(new Vector3(inValue.x + Mathf.Sin(time * rate3.x) * value3.x, inValue.y + Mathf.Sin(time * rate3.y) * value3.y, inValue.z + Mathf.Sin(time * rate3.z) * value3.z)), 
			MATHMANIPULATORTYPE.Cos => ClampPosition(new Vector3(inValue.x + Mathf.Cos(time * rate3.x) * value3.x, inValue.y + Mathf.Cos(time * rate3.y) * value3.y, inValue.z + Mathf.Cos(time * rate3.z) * value3.z)), 
			MATHMANIPULATORTYPE.Lerp => ClampPosition(new Vector3(Mathf.Lerp(inValue.x, value3Position.x, time * rate3.x), Mathf.Lerp(inValue.y, value3Position.y, time * rate3.y), Mathf.Lerp(inValue.z, value3Position.z, time * rate3.z))), 
			MATHMANIPULATORTYPE.Add => ClampPosition(inValue + Vector3.Scale(value3, rate3) * time), 
			MATHMANIPULATORTYPE.Subtract => ClampPosition(inValue - Vector3.Scale(value3, rate3) * time), 
			_ => inValue, 
		};
	}

	public float Evaluate(float inValue, float time)
	{
		if (clamp == MATHMANIPULATORCLAMP.ClampInValue)
		{
			inValue = Clamp(inValue);
		}
		return type switch
		{
			MATHMANIPULATORTYPE.Sin => (value == 0f) ? inValue : ClampOut(Mathf.Sin(time * rate) * value), 
			MATHMANIPULATORTYPE.Cos => (value == 0f) ? inValue : ClampOut(Mathf.Cos(time * rate) * value), 
			MATHMANIPULATORTYPE.Lerp => (value == 0f) ? inValue : Mathf.Lerp(inValue, ClampOut(value), time * rate), 
			MATHMANIPULATORTYPE.Add => ClampOut(inValue + value * rate * time), 
			MATHMANIPULATORTYPE.Subtract => ClampOut(inValue - value * rate * time), 
			_ => inValue, 
		};
	}

	private float Clamp(float val)
	{
		if (clamp == MATHMANIPULATORCLAMP.None)
		{
			return val;
		}
		return Mathf.Clamp(val, clampFloor, clampCeil);
	}

	private Vector3 Clamp(Vector3 val)
	{
		if (clamp == MATHMANIPULATORCLAMP.None)
		{
			return val;
		}
		val = new Vector3(Mathf.Clamp(val.x, clampFloor, clampCeil), Mathf.Clamp(val.y, clampFloor, clampCeil), Mathf.Clamp(val.z, clampFloor, clampCeil));
		return val;
	}

	private Vector3 ClampPosition(Vector3 val)
	{
		if (clamp == MATHMANIPULATORCLAMP.None)
		{
			return val;
		}
		val = new Vector3(Mathf.Clamp(val.x, value3Position.x + clampFloor, value3Position.x + clampCeil), Mathf.Clamp(val.y, value3Position.y + clampFloor, value3Position.y + clampCeil), Mathf.Clamp(val.z, value3Position.z + clampFloor, value3Position.z + clampCeil));
		return val;
	}

	private float ClampOut(float val)
	{
		if (clamp != MATHMANIPULATORCLAMP.ClampOutValue)
		{
			return val;
		}
		return Clamp(val);
	}

	private Vector3 ClampOut(Vector3 val)
	{
		if (clamp != MATHMANIPULATORCLAMP.ClampOutValue)
		{
			return val;
		}
		return Clamp(val);
	}

	public MathManipulatorProperty Clone()
	{
		MathManipulatorProperty mathManipulatorProperty = new MathManipulatorProperty();
		mathManipulatorProperty.type = type;
		mathManipulatorProperty.property = property;
		mathManipulatorProperty.clamp = clamp;
		mathManipulatorProperty.clampFloor = clampFloor;
		mathManipulatorProperty.clampCeil = clampCeil;
		mathManipulatorProperty.value = value;
		mathManipulatorProperty.value3 = value3;
		mathManipulatorProperty.rate = rate;
		mathManipulatorProperty.rate3 = rate3;
		return mathManipulatorProperty;
	}
}
public enum MATHMANIPULATORPROPERTY
{
	Size,
	Rotation,
	Velocity,
	Position
}
