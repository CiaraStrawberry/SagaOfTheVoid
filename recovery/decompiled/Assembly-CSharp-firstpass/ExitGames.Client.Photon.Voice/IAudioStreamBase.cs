using System;

namespace ExitGames.Client.Photon.Voice;

public interface IAudioStreamBase : IDisposable
{
	int SamplingRate { get; }
}
