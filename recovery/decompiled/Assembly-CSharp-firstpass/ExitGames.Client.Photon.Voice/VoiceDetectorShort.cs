namespace ExitGames.Client.Photon.Voice;

public class VoiceDetectorShort : VoiceDetector<short>
{
	internal VoiceDetectorShort(int samplingRate, int numChannels)
		: base(samplingRate, numChannels)
	{
	}

	public override void process(short[] buffer)
	{
		if (On)
		{
			foreach (short num in buffer)
			{
				if ((float)num > Threshold)
				{
					Detected = true;
					autoSilenceCounter = 0;
				}
				else
				{
					autoSilenceCounter++;
				}
			}
			if (autoSilenceCounter > activityDelayValuesCount)
			{
				Detected = false;
			}
		}
		else
		{
			Detected = false;
		}
	}
}
