using System.Collections.Generic;
using UnityEngine;

namespace TrueSync;

public class TrueSyncInput
{
	private static List<InputData> currentAllInputsData = new List<InputData>();

	private static Dictionary<byte, InputData> currentAllInputsDataMap = new Dictionary<byte, InputData>();

	private static InputData currentInputData;

	private static InputData currentSimulationData;

	public static InputData CurrentInputData
	{
		set
		{
			currentInputData = value;
		}
	}

	public static InputData CurrentSimulationData
	{
		get
		{
			return currentSimulationData;
		}
		set
		{
			currentSimulationData = value;
		}
	}

	public static void SetString(byte key, string value)
	{
		if (currentInputData != null)
		{
			currentInputData.AddString(key, value);
		}
	}

	public static void SetByte(byte key, byte value)
	{
		if (currentInputData != null)
		{
			currentInputData.AddByte(key, value);
		}
	}

	public static void SetByteArray(byte key, byte[] value)
	{
		if (currentInputData != null)
		{
			currentInputData.AddByteArray(key, value);
		}
	}

	public static void SetBool(byte key, bool value)
	{
		if (currentInputData != null)
		{
			currentInputData.AddByte(key, (byte)(value ? 1u : 0u));
		}
	}

	public static void SetInt(byte key, int value)
	{
		if (currentInputData != null)
		{
			currentInputData.AddInt(key, value);
		}
	}

	public static void SetFP(byte key, FP value)
	{
		if (currentInputData != null)
		{
			currentInputData.AddFP(key, value);
		}
	}

	public static void SetTSVector(byte key, TSVector value)
	{
		if (currentInputData != null)
		{
			currentInputData.AddTSVector(key, value);
		}
	}

	public static void SetTSVector2(byte key, TSVector2 value)
	{
		if (currentInputData != null)
		{
			currentInputData.AddTSVector2(key, value);
		}
	}

	public static void SetTSVector(byte key, Vector3 value)
	{
		SetTSVector(key, value.ToTSVector());
	}

	public static void SetTSVector2(byte key, Vector2 value)
	{
		SetTSVector2(key, value.ToTSVector2());
	}

	public static void SetAllInputs(List<InputDataBase> allInputData)
	{
		currentAllInputsData.Clear();
		currentAllInputsDataMap.Clear();
		if (allInputData != null)
		{
			int i = 0;
			for (int count = allInputData.Count; i < count; i++)
			{
				InputData inputData = (InputData)allInputData[i];
				currentAllInputsData.Add(inputData);
				currentAllInputsDataMap.Add(inputData.ownerID, inputData);
			}
		}
	}

	public static List<InputData> GetAllInputs()
	{
		return currentAllInputsData;
	}

	public static InputData GetPlayerInput(byte ownerId)
	{
		if (!currentAllInputsDataMap.ContainsKey(ownerId))
		{
			return null;
		}
		return currentAllInputsDataMap[ownerId];
	}

	private static void LogInvalidKeyAccess()
	{
		Debug.LogWarning("You can't access an input in a TrueSyncBehaviour that has no player owner.");
	}

	public static string GetString(byte key)
	{
		if (currentSimulationData == null)
		{
			LogInvalidKeyAccess();
			return null;
		}
		return currentSimulationData.GetString(key);
	}

	public static string GetString(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.GetString(key);
	}

	public static byte GetByte(byte key)
	{
		if (currentSimulationData == null)
		{
			LogInvalidKeyAccess();
			return 0;
		}
		return currentSimulationData.GetByte(key);
	}

	public static byte GetByte(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.GetByte(key) ?? 0;
	}

	public static byte[] GetByteArray(byte key)
	{
		if (currentSimulationData == null)
		{
			LogInvalidKeyAccess();
			return null;
		}
		return currentSimulationData.GetByteArray(key);
	}

	public static byte[] GetByteArray(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.GetByteArray(key);
	}

	public static bool GetBool(byte key)
	{
		if (currentSimulationData == null)
		{
			LogInvalidKeyAccess();
			return false;
		}
		return currentSimulationData.GetByte(key) == 1;
	}

	public static bool GetBool(byte ownerId, byte key)
	{
		InputData playerInput = GetPlayerInput(ownerId);
		if (playerInput == null)
		{
			return false;
		}
		return playerInput.GetByte(key) == 1;
	}

	public static int GetInt(byte key)
	{
		if (currentSimulationData == null)
		{
			LogInvalidKeyAccess();
			return 0;
		}
		return currentSimulationData.GetInt(key);
	}

	public static int GetInt(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.GetInt(key) ?? 0;
	}

	public static FP GetFP(byte key)
	{
		if (currentSimulationData == null)
		{
			LogInvalidKeyAccess();
			return FP.Zero;
		}
		return currentSimulationData.GetFP(key);
	}

	public static FP GetFP(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.GetFP(key) ?? FP.Zero;
	}

	public static TSVector GetTSVector(byte key)
	{
		if (currentSimulationData == null)
		{
			LogInvalidKeyAccess();
			return TSVector.zero;
		}
		return currentSimulationData.GetTSVector(key);
	}

	public static TSVector GetTSVector(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.GetTSVector(key) ?? TSVector.zero;
	}

	public static TSVector2 GetTSVector2(byte key)
	{
		if (currentSimulationData == null)
		{
			LogInvalidKeyAccess();
			return TSVector2.zero;
		}
		return currentSimulationData.GetTSVector2(key);
	}

	public static TSVector2 GetTSVector2(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.GetTSVector2(key) ?? TSVector2.zero;
	}

	public static bool HasString(byte key)
	{
		if (currentSimulationData == null)
		{
			return false;
		}
		return currentSimulationData.HasString(key);
	}

	public static bool HasString(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.HasString(key) ?? false;
	}

	public static bool HasByte(byte key)
	{
		if (currentSimulationData == null)
		{
			return false;
		}
		return currentSimulationData.HasByte(key);
	}

	public static bool HasByte(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.HasByte(key) ?? false;
	}

	public static bool HasByteArray(byte key)
	{
		if (currentSimulationData == null)
		{
			return false;
		}
		return currentSimulationData.HasByteArray(key);
	}

	public static bool HasByteArray(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.HasByteArray(key) ?? false;
	}

	public static bool HasBool(byte key)
	{
		if (currentSimulationData == null)
		{
			return false;
		}
		return currentSimulationData.HasByte(key);
	}

	public static bool HasBool(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.HasByte(key) ?? false;
	}

	public static bool HasInt(byte key)
	{
		if (currentSimulationData == null)
		{
			return false;
		}
		return currentSimulationData.HasInt(key);
	}

	public static bool HasInt(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.HasInt(key) ?? false;
	}

	public static bool HasFP(byte key)
	{
		if (currentSimulationData == null)
		{
			return false;
		}
		return currentSimulationData.HasFP(key);
	}

	public static bool HasFP(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.HasFP(key) ?? false;
	}

	public static bool HasTSVector(byte key)
	{
		if (currentSimulationData == null)
		{
			return false;
		}
		return currentSimulationData.HasTSVector(key);
	}

	public static bool HasTSVector(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.HasTSVector(key) ?? false;
	}

	public static bool HasTSVector2(byte key)
	{
		if (currentSimulationData == null)
		{
			return false;
		}
		return currentSimulationData.HasTSVector2(key);
	}

	public static bool HasTSVector2(byte ownerId, byte key)
	{
		return GetPlayerInput(ownerId)?.HasTSVector2(key) ?? false;
	}
}
