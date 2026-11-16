using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General.ThirdParty.FullSerializer.Speedup;

public class Devdog_General_MotionInfo_DirectConverter : fsDirectConverter<MotionInfo>
{
	protected override fsResult DoSerialize(MotionInfo model, Dictionary<string, fsData> serialized)
	{
		fsResult success = fsResult.Success;
		success += SerializeMember(serialized, null, "motion", model.motion);
		return success + SerializeMember(serialized, null, "speed", model.speed);
	}

	protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref MotionInfo model)
	{
		fsResult success = fsResult.Success;
		Motion value = model.motion;
		success += DeserializeMember<Motion>(data, null, "motion", out value);
		model.motion = value;
		float value2 = model.speed;
		success += DeserializeMember<float>(data, null, "speed", out value2);
		model.speed = value2;
		return success;
	}

	public override object CreateInstance(fsData data, Type storageType)
	{
		return new MotionInfo();
	}
}
