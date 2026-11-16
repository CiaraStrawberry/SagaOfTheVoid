namespace ExitGames.Client.Photon.Voice;

public class LevelMeterShort : LevelMeter<short>
{
	internal LevelMeterShort(int samplingRate, int numChannels)
		: base(samplingRate, numChannels)
	{
	}

	public override void process(short[] buf)
	{
		foreach (short num in buf)
		{
			short num2 = num;
			if (num2 < 0)
			{
				num2 = (short)(-num2);
			}
			ampSum = ampSum + (float)num2 - buffer[prevValuesPtr];
			buffer[prevValuesPtr] = num2;
			if (ampPeak < (float)num2)
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
