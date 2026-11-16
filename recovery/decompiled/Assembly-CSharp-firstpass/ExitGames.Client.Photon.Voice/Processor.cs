namespace ExitGames.Client.Photon.Voice;

internal interface Processor<T>
{
	void process(T[] buf);
}
