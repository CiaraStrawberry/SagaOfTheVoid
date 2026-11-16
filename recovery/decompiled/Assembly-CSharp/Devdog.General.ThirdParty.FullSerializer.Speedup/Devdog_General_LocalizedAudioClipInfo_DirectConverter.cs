using System;
using System.Collections.Generic;
using Devdog.General.Localization;

namespace Devdog.General.ThirdParty.FullSerializer.Speedup;

public class Devdog_General_LocalizedAudioClipInfo_DirectConverter : fsDirectConverter<LocalizedAudioClipInfo>
{
	protected override fsResult DoSerialize(LocalizedAudioClipInfo model, Dictionary<string, fsData> serialized)
	{
		fsResult success = fsResult.Success;
		success += SerializeMember(serialized, null, "audioClip", model.audioClip);
		success += SerializeMember(serialized, null, "volume", model.volume);
		success += SerializeMember(serialized, null, "pitch", model.pitch);
		return success + SerializeMember(serialized, null, "loop", model.loop);
	}

	protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref LocalizedAudioClipInfo model)
	{
		fsResult success = fsResult.Success;
		LocalizedAudioClip value = model.audioClip;
		success += DeserializeMember<LocalizedAudioClip>(data, null, "audioClip", out value);
		model.audioClip = value;
		float value2 = model.volume;
		success += DeserializeMember<float>(data, null, "volume", out value2);
		model.volume = value2;
		float value3 = model.pitch;
		success += DeserializeMember<float>(data, null, "pitch", out value3);
		model.pitch = value3;
		bool value4 = model.loop;
		success += DeserializeMember<bool>(data, null, "loop", out value4);
		model.loop = value4;
		return success;
	}

	public override object CreateInstance(fsData data, Type storageType)
	{
		return new LocalizedAudioClipInfo();
	}
}
