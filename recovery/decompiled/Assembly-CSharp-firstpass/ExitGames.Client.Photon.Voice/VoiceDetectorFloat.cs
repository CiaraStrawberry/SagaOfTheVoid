namespace ExitGames.Client.Photon.Voice;

public class VoiceDetectorFloat : VoiceDetector<float>
{
	internal VoiceDetectorFloat(int samplingRate, int numChannels)
		: base(samplingRate, numChannels)
	{
	}

	public override void process(float[] buffer)
	{
		if (On)
		{
			foreach (float num in buffer)
			{
				if (num > Threshold)
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
