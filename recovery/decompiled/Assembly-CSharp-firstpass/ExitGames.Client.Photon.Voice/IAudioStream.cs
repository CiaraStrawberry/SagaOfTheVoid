using System;

namespace ExitGames.Client.Photon.Voice;

public interface IAudioStream<T> : IAudioStreamBase, IDisposable
{
	bool GetData(T[] buffer);
}
