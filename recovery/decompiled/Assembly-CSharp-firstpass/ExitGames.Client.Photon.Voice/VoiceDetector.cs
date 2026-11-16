namespace ExitGames.Client.Photon.Voice;

public interface VoiceDetector
{
	bool On { get; set; }

	float Threshold { get; set; }

	bool Detected { get; }

	int ActivityDelayMs { get; set; }
}
public abstract class VoiceDetector<T> : Processor<T>, VoiceDetector
{
	protected int activityDelay;

	protected int autoSilenceCounter;

	protected int valuesCountPerSec;

	protected int activityDelayValuesCount;

	public bool On { get; set; }

	public float Threshold { get; set; }

	public bool Detected { get; protected set; }

	public int ActivityDelayMs
	{
		get
		{
			return activityDelay;
		}
		set
		{
			activityDelay = value;
			activityDelayValuesCount = value * valuesCountPerSec / 1000;
		}
	}

	internal VoiceDetector(int samplingRate, int numChannels)
	{
		valuesCountPerSec = samplingRate * numChannels;
		Threshold = 0.01f;
		ActivityDelayMs = 500;
	}

	public abstract void process(T[] buf);
}
