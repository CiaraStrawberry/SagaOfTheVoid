using System;

namespace ExitGames.Client.Photon.Voice;

internal class UnsupportedSampleTypeException : Exception
{
	public UnsupportedSampleTypeException(object o)
		: base("PhotonVoice: unsupported sample type: " + o.GetType())
	{
	}
}
