using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TrueSync;

[Serializable]
public class InputData : InputDataBase
{
	private enum Types : byte
	{
		Byte,
		String,
		Integer,
		FP,
		TSVector,
		TSVector2,
		ByteArray
	}

	[SerializeField]
	internal SerializableDictionaryByteString stringTable;

	[SerializeField]
	internal SerializableDictionaryByteByte byteTable;

	[SerializeField]
	internal SerializableDictionaryByteInt intTable;

	[SerializeField]
	internal SerializableDictionaryByteFP fpTable;

	[SerializeField]
	internal SerializableDictionaryByteByteArray byteArrayTable;

	[SerializeField]
	internal SerializableDictionaryByteTSVector tsVectorTable;

	[SerializeField]
	internal SerializableDictionaryByteTSVector2 tsVectorTable2;

	public int Count => stringTable.Count + byteTable.Count + intTable.Count + fpTable.Count + byteArrayTable.Count + tsVectorTable.Count + tsVectorTable2.Count;

	public InputData()
	{
		stringTable = new SerializableDictionaryByteString();
		byteTable = new SerializableDictionaryByteByte();
		intTable = new SerializableDictionaryByteInt();
		fpTable = new SerializableDictionaryByteFP();
		byteArrayTable = new SerializableDictionaryByteByteArray();
		tsVectorTable = new SerializableDictionaryByteTSVector();
		tsVectorTable2 = new SerializableDictionaryByteTSVector2();
	}

	public override void Serialize(List<byte> bytes)
	{
		byte item = (byte)Count;
		bytes.Add(item);
		Dictionary<byte, byte>.Enumerator enumerator = byteTable.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<byte, byte> current = enumerator.Current;
			bytes.Add(current.Key);
			bytes.Add(0);
			bytes.Add(current.Value);
		}
		Dictionary<byte, byte[]>.Enumerator enumerator2 = byteArrayTable.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			KeyValuePair<byte, byte[]> current2 = enumerator2.Current;
			bytes.Add(current2.Key);
			bytes.Add(6);
			Utils.GetBytes(current2.Value.Length, bytes);
			int i = 0;
			for (int num = current2.Value.Length; i < num; i++)
			{
				bytes.Add(current2.Value[i]);
			}
		}
		Dictionary<byte, int>.Enumerator enumerator3 = intTable.GetEnumerator();
		while (enumerator3.MoveNext())
		{
			KeyValuePair<byte, int> current3 = enumerator3.Current;
			bytes.Add(current3.Key);
			bytes.Add(2);
			Utils.GetBytes(current3.Value, bytes);
		}
		Dictionary<byte, string>.Enumerator enumerator4 = stringTable.GetEnumerator();
		while (enumerator4.MoveNext())
		{
			KeyValuePair<byte, string> current4 = enumerator4.Current;
			bytes.Add(current4.Key);
			bytes.Add(1);
			byte[] bytes2 = Encoding.ASCII.GetBytes(current4.Value);
			Utils.GetBytes(bytes2.Length, bytes);
			int j = 0;
			for (int num2 = bytes2.Length; j < num2; j++)
			{
				bytes.Add(bytes2[j]);
			}
		}
		Dictionary<byte, FP>.Enumerator enumerator5 = fpTable.GetEnumerator();
		while (enumerator5.MoveNext())
		{
			KeyValuePair<byte, FP> current5 = enumerator5.Current;
			bytes.Add(current5.Key);
			bytes.Add(3);
			Utils.GetBytes(current5.Value.RawValue, bytes);
		}
		Dictionary<byte, TSVector>.Enumerator enumerator6 = tsVectorTable.GetEnumerator();
		while (enumerator6.MoveNext())
		{
			KeyValuePair<byte, TSVector> current6 = enumerator6.Current;
			bytes.Add(current6.Key);
			bytes.Add(4);
			Utils.GetBytes(current6.Value.x.RawValue, bytes);
			Utils.GetBytes(current6.Value.y.RawValue, bytes);
			Utils.GetBytes(current6.Value.z.RawValue, bytes);
		}
		Dictionary<byte, TSVector2>.Enumerator enumerator7 = tsVectorTable2.GetEnumerator();
		while (enumerator7.MoveNext())
		{
			KeyValuePair<byte, TSVector2> current7 = enumerator7.Current;
			bytes.Add(current7.Key);
			bytes.Add(5);
			Utils.GetBytes(current7.Value.x.RawValue, bytes);
			Utils.GetBytes(current7.Value.y.RawValue, bytes);
		}
	}

	public override void Deserialize(byte[] data, ref int offset)
	{
		byte b = data[offset++];
		for (int i = 0; i < b; i++)
		{
			byte key = data[offset++];
			switch (data[offset++])
			{
			case 2:
			{
				int value4 = BitConverter.ToInt32(data, offset);
				AddInt(key, value4);
				offset += 4;
				break;
			}
			case 0:
			{
				byte value3 = data[offset++];
				AddByte(key, value3);
				break;
			}
			case 6:
			{
				int num2 = BitConverter.ToInt32(data, offset);
				offset += 4;
				byte[] array = new byte[num2];
				for (int j = 0; j < num2; j++)
				{
					array[j] = data[offset + j];
				}
				offset += num2;
				AddByteArray(key, array);
				break;
			}
			case 1:
			{
				int num = BitConverter.ToInt32(data, offset);
				offset += 4;
				string value2 = Encoding.ASCII.GetString(data, offset, num);
				offset += num;
				AddString(key, value2);
				break;
			}
			case 3:
			{
				FP value = FP.FromRaw(BitConverter.ToInt64(data, offset));
				AddFP(key, value);
				offset += 8;
				break;
			}
			case 4:
			{
				FP x2 = FP.FromRaw(BitConverter.ToInt64(data, offset));
				offset += 8;
				FP y2 = FP.FromRaw(BitConverter.ToInt64(data, offset));
				offset += 8;
				FP z = FP.FromRaw(BitConverter.ToInt64(data, offset));
				offset += 8;
				AddTSVector(key, new TSVector(x2, y2, z));
				break;
			}
			case 5:
			{
				FP x = FP.FromRaw(BitConverter.ToInt64(data, offset));
				offset += 8;
				FP y = FP.FromRaw(BitConverter.ToInt64(data, offset));
				offset += 8;
				AddTSVector2(key, new TSVector2(x, y));
				break;
			}
			}
		}
	}

	public override void CleanUp()
	{
		stringTable.Clear();
		byteTable.Clear();
		intTable.Clear();
		fpTable.Clear();
		byteArrayTable.Clear();
		tsVectorTable.Clear();
		tsVectorTable2.Clear();
	}

	public override void CopyFrom(InputDataBase fromBase)
	{
		InputData inputData = (InputData)fromBase;
		Dictionary<byte, string>.Enumerator enumerator = inputData.stringTable.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<byte, string> current = enumerator.Current;
			stringTable.Add(current.Key, current.Value);
		}
		Dictionary<byte, byte>.Enumerator enumerator2 = inputData.byteTable.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			KeyValuePair<byte, byte> current2 = enumerator2.Current;
			byteTable.Add(current2.Key, current2.Value);
		}
		Dictionary<byte, int>.Enumerator enumerator3 = inputData.intTable.GetEnumerator();
		while (enumerator3.MoveNext())
		{
			KeyValuePair<byte, int> current3 = enumerator3.Current;
			intTable.Add(current3.Key, current3.Value);
		}
		Dictionary<byte, FP>.Enumerator enumerator4 = inputData.fpTable.GetEnumerator();
		while (enumerator4.MoveNext())
		{
			KeyValuePair<byte, FP> current4 = enumerator4.Current;
			fpTable.Add(current4.Key, current4.Value);
		}
		Dictionary<byte, byte[]>.Enumerator enumerator5 = inputData.byteArrayTable.GetEnumerator();
		while (enumerator5.MoveNext())
		{
			KeyValuePair<byte, byte[]> current5 = enumerator5.Current;
			byteArrayTable.Add(current5.Key, current5.Value);
		}
		Dictionary<byte, TSVector>.Enumerator enumerator6 = inputData.tsVectorTable.GetEnumerator();
		while (enumerator6.MoveNext())
		{
			KeyValuePair<byte, TSVector> current6 = enumerator6.Current;
			tsVectorTable.Add(current6.Key, current6.Value);
		}
		Dictionary<byte, TSVector2>.Enumerator enumerator7 = inputData.tsVectorTable2.GetEnumerator();
		while (enumerator7.MoveNext())
		{
			KeyValuePair<byte, TSVector2> current7 = enumerator7.Current;
			tsVectorTable2.Add(current7.Key, current7.Value);
		}
	}

	public override bool EqualsData(InputDataBase otherBase)
	{
		InputData inputData = (InputData)otherBase;
		if (stringTable.Count != inputData.stringTable.Count || byteTable.Count != inputData.byteTable.Count || intTable.Count != inputData.intTable.Count || fpTable.Count != inputData.fpTable.Count || byteArrayTable.Count != inputData.byteArrayTable.Count || tsVectorTable.Count != inputData.tsVectorTable.Count || tsVectorTable2.Count != inputData.tsVectorTable2.Count)
		{
			return false;
		}
		if (!checkEqualsTable(this, inputData))
		{
			return false;
		}
		return true;
	}

	private static bool checkEqualsTable(InputData id1, InputData id2)
	{
		Dictionary<byte, string>.Enumerator enumerator = id1.stringTable.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<byte, string> current = enumerator.Current;
			if (!id2.stringTable.ContainsKey(current.Key) || current.Value != id2.stringTable[current.Key])
			{
				return false;
			}
		}
		Dictionary<byte, byte>.Enumerator enumerator2 = id1.byteTable.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			KeyValuePair<byte, byte> current2 = enumerator2.Current;
			if (!id2.byteTable.ContainsKey(current2.Key) || current2.Value != id2.byteTable[current2.Key])
			{
				return false;
			}
		}
		Dictionary<byte, int>.Enumerator enumerator3 = id1.intTable.GetEnumerator();
		while (enumerator3.MoveNext())
		{
			KeyValuePair<byte, int> current3 = enumerator3.Current;
			if (!id2.intTable.ContainsKey(current3.Key) || current3.Value != id2.intTable[current3.Key])
			{
				return false;
			}
		}
		Dictionary<byte, FP>.Enumerator enumerator4 = id1.fpTable.GetEnumerator();
		while (enumerator4.MoveNext())
		{
			KeyValuePair<byte, FP> current4 = enumerator4.Current;
			if (!id2.fpTable.ContainsKey(current4.Key) || current4.Value != id2.fpTable[current4.Key])
			{
				return false;
			}
		}
		Dictionary<byte, byte[]>.Enumerator enumerator5 = id1.byteArrayTable.GetEnumerator();
		while (enumerator5.MoveNext())
		{
			KeyValuePair<byte, byte[]> current5 = enumerator5.Current;
			if (!id2.byteArrayTable.ContainsKey(current5.Key) || current5.Value != id2.byteArrayTable[current5.Key])
			{
				return false;
			}
		}
		Dictionary<byte, TSVector>.Enumerator enumerator6 = id1.tsVectorTable.GetEnumerator();
		while (enumerator6.MoveNext())
		{
			KeyValuePair<byte, TSVector> current6 = enumerator6.Current;
			if (!id2.tsVectorTable.ContainsKey(current6.Key) || current6.Value != id2.tsVectorTable[current6.Key])
			{
				return false;
			}
		}
		Dictionary<byte, TSVector2>.Enumerator enumerator7 = id1.tsVectorTable2.GetEnumerator();
		while (enumerator7.MoveNext())
		{
			KeyValuePair<byte, TSVector2> current7 = enumerator7.Current;
			if (!id2.tsVectorTable2.ContainsKey(current7.Key) || current7.Value != id2.tsVectorTable2[current7.Key])
			{
				return false;
			}
		}
		return true;
	}

	internal bool IsEmpty()
	{
		return Count == 0;
	}

	internal void AddString(byte key, string value)
	{
		stringTable[key] = value;
	}

	internal void AddByte(byte key, byte value)
	{
		byteTable[key] = value;
	}

	internal void AddByteArray(byte key, byte[] value)
	{
		byte[] array = new byte[value.Length];
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			array[i] = value[i];
		}
		byteArrayTable[key] = array;
	}

	internal void AddInt(byte key, int value)
	{
		intTable[key] = value;
	}

	internal void AddFP(byte key, FP value)
	{
		fpTable[key] = value;
	}

	internal void AddTSVector(byte key, TSVector value)
	{
		tsVectorTable[key] = value;
	}

	internal void AddTSVector2(byte key, TSVector2 value)
	{
		tsVectorTable2[key] = value;
	}

	public string GetString(byte key)
	{
		if (!stringTable.ContainsKey(key))
		{
			return string.Empty;
		}
		return stringTable[key];
	}

	public byte GetByte(byte key)
	{
		if (!byteTable.ContainsKey(key))
		{
			return 0;
		}
		return byteTable[key];
	}

	public byte[] GetByteArray(byte key)
	{
		if (!byteArrayTable.ContainsKey(key))
		{
			return null;
		}
		return byteArrayTable[key];
	}

	public int GetInt(byte key)
	{
		if (!intTable.ContainsKey(key))
		{
			return 0;
		}
		return intTable[key];
	}

	public FP GetFP(byte key)
	{
		if (!fpTable.ContainsKey(key))
		{
			return 0;
		}
		return fpTable[key];
	}

	public TSVector GetTSVector(byte key)
	{
		if (!tsVectorTable.ContainsKey(key))
		{
			return TSVector.zero;
		}
		return tsVectorTable[key];
	}

	public TSVector2 GetTSVector2(byte key)
	{
		if (!tsVectorTable2.ContainsKey(key))
		{
			return TSVector2.zero;
		}
		return tsVectorTable2[key];
	}

	public bool HasString(byte key)
	{
		return stringTable.ContainsKey(key);
	}

	public bool HasByte(byte key)
	{
		return byteTable.ContainsKey(key);
	}

	public bool HasByteArray(byte key)
	{
		return byteArrayTable.ContainsKey(key);
	}

	public bool HasInt(byte key)
	{
		return intTable.ContainsKey(key);
	}

	public bool HasFP(byte key)
	{
		return fpTable.ContainsKey(key);
	}

	public bool HasTSVector(byte key)
	{
		return tsVectorTable.ContainsKey(key);
	}

	public bool HasTSVector2(byte key)
	{
		return tsVectorTable2.ContainsKey(key);
	}
}
