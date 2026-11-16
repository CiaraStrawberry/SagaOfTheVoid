using System;
using System.IO;
using UnityEngine;

namespace DFNetwork.Helpers;

public class Deserializer
{
	public int index;

	private MemoryStream ms;

	private byte[] _bytes;

	public int GetByteLength => index;

	public Deserializer(byte[] data)
	{
		Reset();
		_bytes = data;
	}

	public void Reset()
	{
		index = 0;
		ms = new MemoryStream();
	}

	public byte[] ToArray()
	{
		return ms.ToArray();
	}

	public bool ReadNextBool()
	{
		bool result = BitConverter.ToBoolean(_bytes, index);
		index++;
		return result;
	}

	public int ReadNextInt()
	{
		int result = BitConverter.ToInt32(_bytes, index);
		index += 4;
		return result;
	}

	public int[] ReadNextIntArray()
	{
		int num = ReadNextInt();
		int[] array = new int[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadNextInt();
		}
		return array;
	}

	public short ReadNextShort()
	{
		short result = BitConverter.ToInt16(_bytes, index);
		index += 2;
		return result;
	}

	public short[] ReadNextShortArray()
	{
		int num = ReadNextInt();
		short[] array = new short[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadNextShort();
		}
		return array;
	}

	public float ReadNextFloat()
	{
		return toFloat(ReadNextInt());
	}

	public float[] ReadNextFloatArray()
	{
		int num = ReadNextInt();
		float[] array = new float[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadNextFloat();
		}
		return array;
	}

	public double ReadNextDouble()
	{
		double result = BitConverter.ToDouble(_bytes, index);
		index += 8;
		return result;
	}

	public char ReadNextChar()
	{
		char result = BitConverter.ToChar(_bytes, index);
		index += 2;
		return result;
	}

	public string ReadNextString()
	{
		int num = ReadNextInt();
		string text = string.Empty;
		for (int i = 0; i < num; i++)
		{
			text += ReadNextChar();
		}
		return text;
	}

	public string[] ReadNextStringArray()
	{
		int num = ReadNextInt();
		string[] array = new string[num];
		for (int i = 0; i < num; i++)
		{
			int num2 = ReadNextInt();
			string text = string.Empty;
			for (int j = 0; j < num2; j++)
			{
				text += ReadNextChar();
			}
			array[i] = text;
		}
		return array;
	}

	public Vector3 ReadNextVector3()
	{
		return new Vector3(ReadNextFloat(), ReadNextFloat(), ReadNextFloat());
	}

	public Vector3[] ReadNextVector3Array()
	{
		int num = ReadNextInt();
		Vector3[] array = new Vector3[num];
		for (int i = 0; i < num; i++)
		{
			ref Vector3 reference = ref array[i];
			reference = new Vector3(ReadNextFloat(), ReadNextFloat(), ReadNextFloat());
		}
		return array;
	}

	public Quaternion ReadNextQuaternion()
	{
		return new Quaternion(ReadNextFloat(), ReadNextFloat(), ReadNextFloat(), ReadNextFloat());
	}

	public Quaternion[] ReadNextQuaternionArray()
	{
		int num = ReadNextInt();
		Quaternion[] array = new Quaternion[num];
		for (int i = 0; i < num; i++)
		{
			ref Quaternion reference = ref array[i];
			reference = new Quaternion(ReadNextFloat(), ReadNextFloat(), ReadNextFloat(), ReadNextFloat());
		}
		return array;
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
