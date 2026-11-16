namespace ExitGames.Client.Photon.Voice;

public interface LevelMeter
{
	float CurrentAvgAmp { get; }

	float CurrentPeakAmp { get; }

	float AccumAvgPeakAmp { get; }

	void ResetAccumAvgPeakAmp();
}
public abstract class LevelMeter<T> : Processor<T>, LevelMeter
{
	protected float ampSum;

	protected float ampPeak;

	protected int bufferSize;

	protected float[] buffer;

	protected int prevValuesPtr;

	protected float accumAvgPeakAmpSum;

	protected int accumAvgPeakAmpCount;

	public float CurrentAvgAmp => ampSum / (float)bufferSize;

	public float CurrentPeakAmp { get; protected set; }

	public float AccumAvgPeakAmp => (accumAvgPeakAmpCount != 0) ? (accumAvgPeakAmpSum / (float)accumAvgPeakAmpCount) : 0f;

	internal LevelMeter(int samplingRate, int numChannels)
	{
		bufferSize = samplingRate * numChannels / 2;
		buffer = new float[bufferSize];
	}

	public void ResetAccumAvgPeakAmp()
	{
		accumAvgPeakAmpSum = 0f;
		accumAvgPeakAmpCount = 0;
	}

	public abstract void process(T[] buf);
}
