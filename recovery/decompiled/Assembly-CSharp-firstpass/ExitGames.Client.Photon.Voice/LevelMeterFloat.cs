namespace ExitGames.Client.Photon.Voice;

public class LevelMeterFloat : LevelMeter<float>
{
	internal LevelMeterFloat(int samplingRate, int numChannels)
		: base(samplingRate, numChannels)
	{
	}

	public override void process(float[] buf)
	{
		foreach (float num in buf)
		{
			float num2 = num;
			if (num2 < 0f)
			{
				num2 = 0f - num2;
			}
			ampSum = ampSum + num2 - buffer[prevValuesPtr];
			buffer[prevValuesPtr] = num2;
			if (ampPeak < num2)
			{
				ampPeak = num2;
			}
			if (prevValuesPtr == 0)
			{
				CurrentPeakAmp = ampPeak;
				ampPeak = 0f;
				accumAvgPeakAmpSum += CurrentPeakAmp;
				accumAvgPeakAmpCount++;
			}
			prevValuesPtr = (prevValuesPtr + 1) % bufferSize;
		}
	}
}
