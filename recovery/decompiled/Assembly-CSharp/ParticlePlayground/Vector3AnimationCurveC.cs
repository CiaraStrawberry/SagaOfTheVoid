using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class Vector3AnimationCurveC
{
	public AnimationCurve x;

	public AnimationCurve y;

	public AnimationCurve z;

	public float xRepeat = 1f;

	public float yRepeat = 1f;

	public float zRepeat = 1f;

	public Vector3 Evaluate(float time)
	{
		return Evaluate(time, 1f);
	}

	public Vector3 Evaluate(float time, float scale)
	{
		return new Vector3(x.Evaluate(time * xRepeat % 1f), y.Evaluate(time * yRepeat % 1f), z.Evaluate(time * zRepeat % 1f)) * scale;
	}

	public bool HasKeys()
	{
		CheckNull();
		return x.keys.Length > 0 || y.keys.Length > 0 || z.keys.Length > 0;
	}

	public void SetKeyValues(int key, float value)
	{
		SetKeyValues(key, new Vector3(value, value, value), 0f, 0f);
	}

	public void SetKeyValues(int key, Vector3 value, float inTangent, float outTangent)
	{
		CheckNull();
		Keyframe[] keys = x.keys;
		Keyframe[] keys2 = y.keys;
		Keyframe[] keys3 = z.keys;
		if (key < x.keys.Length)
		{
			keys[key].value = value.x;
			keys[key].inTangent = inTangent;
			keys[key].outTangent = outTangent;
		}
		if (key < y.keys.Length)
		{
			keys2[key].value = value.y;
			keys2[key].inTangent = inTangent;
			keys2[key].outTangent = outTangent;
		}
		if (key < z.keys.Length)
		{
			keys3[key].value = value.z;
			keys3[key].inTangent = inTangent;
			keys3[key].outTangent = outTangent;
		}
		x.keys = keys;
		y.keys = keys2;
		z.keys = keys3;
	}

	public void Reset()
	{
		CheckNull();
		Keyframe[] array = new Keyframe[2];
		array[0].time = 0f;
		array[1].time = 1f;
		x.keys = array;
		y.keys = array;
		z.keys = array;
		xRepeat = 1f;
		yRepeat = 1f;
		zRepeat = 1f;
	}

	public void Reset1()
	{
		CheckNull();
		Keyframe[] array = new Keyframe[2];
		array[0].time = 0f;
		array[0].value = 1f;
		array[1].time = 1f;
		array[1].value = 1f;
		x.keys = array;
		y.keys = array;
		z.keys = array;
		xRepeat = 1f;
		yRepeat = 1f;
		zRepeat = 1f;
	}

	public void ResetWithMidKey()
	{
		CheckNull();
		Keyframe[] array = new Keyframe[3];
		array[0].time = 0f;
		array[1].time = 0.5f;
		array[2].time = 1f;
		x.keys = array;
		y.keys = array;
		z.keys = array;
		xRepeat = 1f;
		yRepeat = 1f;
		zRepeat = 1f;
	}

	public void CheckNull()
	{
		if (x == null)
		{
			x = new AnimationCurve();
		}
		if (y == null)
		{
			y = new AnimationCurve();
		}
		if (z == null)
		{
			z = new AnimationCurve();
		}
	}

	public Vector3AnimationCurveC Clone()
	{
		Vector3AnimationCurveC vector3AnimationCurveC = new Vector3AnimationCurveC();
		vector3AnimationCurveC.x = new AnimationCurve(x.keys);
		vector3AnimationCurveC.y = new AnimationCurve(y.keys);
		vector3AnimationCurveC.z = new AnimationCurve(z.keys);
		vector3AnimationCurveC.xRepeat = xRepeat;
		vector3AnimationCurveC.yRepeat = yRepeat;
		vector3AnimationCurveC.zRepeat = zRepeat;
		return vector3AnimationCurveC;
	}
}
