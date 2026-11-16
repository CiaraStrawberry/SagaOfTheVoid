using System;
using System.IO;
using UnityEngine;

namespace DFNetwork.Helpers;

public class Serializer
{
	public int index;

	private MemoryStream ms;

	private byte[] _bytes;

	public int GetByteLength => index;

	public Serializer()
	{
		Reset();
	}

	public void Reset()
	{
		index = 0;
		ms = new MemoryStream();
	}

	public void Serialize()
	{
		Reset();
	}

	public byte[] ToArray()
	{
		return ms.ToArray();
	}

	public void AddBits(bool val)
	{
		byte[] array = new byte[1];
		array = BitConverter.GetBytes(val);
		byte[] array2 = array;
		foreach (byte value in array2)
		{
			ms.WriteByte(value);
		}
		index++;
	}

	public void AddBits(int val)
	{
		byte[] array = new byte[4];
		array = BitConverter.GetBytes(val);
		byte[] array2 = array;
		foreach (byte value in array2)
		{
			ms.WriteByte(value);
		}
		index += 4;
	}

	public void AddBits(int[] val)
	{
		AddBits(val.Length);
		foreach (int val2 in val)
		{
			AddBits(val2);
		}
	}

	public void AddBits(short val)
	{
		byte[] array = new byte[2];
		array = BitConverter.GetBytes(val);
		byte[] array2 = array;
		foreach (byte value in array2)
		{
			ms.WriteByte(value);
		}
		index += 2;
	}

	public void AddBits(short[] val)
	{
		AddBits(val.Length);
		foreach (short val2 in val)
		{
			AddBits(val2);
		}
	}

	public void AddBits(float val)
	{
		AddBits(toFixed(val));
	}

	public void AddBits(float[] val)
	{
		AddBits(val.Length);
		foreach (float val2 in val)
		{
			AddBits(val2);
		}
	}

	public void AddBits(double val)
	{
		byte[] array = new byte[8];
		array = BitConverter.GetBytes(val);
		byte[] array2 = array;
		foreach (byte value in array2)
		{
			ms.WriteByte(value);
		}
		index += 8;
	}

	public void AddBits(char val)
	{
		byte[] array = new byte[2];
		array = BitConverter.GetBytes(val);
		byte[] array2 = array;
		foreach (byte value in array2)
		{
			ms.WriteByte(value);
		}
		index += 2;
	}

	public void AddBits(string val)
	{
		AddBits(val.Length);
		foreach (char val2 in val)
		{
			AddBits(val2);
		}
	}

	public void AddBits(string[] val)
	{
		AddBits(val.Length);
		foreach (string text in val)
		{
			AddBits(text.Length);
			string text2 = text;
			foreach (char val2 in text2)
			{
				AddBits(val2);
			}
		}
	}

	public void AddBits(Vector3 val)
	{
		AddBits(val.x);
		AddBits(val.y);
		AddBits(val.z);
	}

	public void AddBits(Vector3[] val)
	{
		AddBits(val.Length);
		for (int i = 0; i < val.Length; i++)
		{
			Vector3 vector = val[i];
			AddBits(vector.x);
			AddBits(vector.y);
			AddBits(vector.z);
		}
	}

	public void AddBits(Quaternion val)
	{
		AddBits(val.x);
		AddBits(val.y);
		AddBits(val.z);
		AddBits(val.w);
	}

	public void AddBits(Quaternion[] val)
	{
		AddBits(val.Length);
		for (int i = 0; i < val.Length; i++)
		{
			Quaternion quaternion = val[i];
			AddBits(quaternion.x);
			AddBits(quaternion.y);
			AddBits(quaternion.z);
			AddBits(quaternion.w);
		}
	}

	public static int toFixed(float val)
	{
		return (int)(val * 65536f);
	}

	public static float toFloat(int val)
	{
		return (float)val / 65536f;
	}
}
