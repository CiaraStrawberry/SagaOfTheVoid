using System;

namespace Oculus.Platform.Models;

public class LivestreamingStatus
{
	public readonly bool IsPaused;

	public readonly bool LivestreamingEnabled;

	public readonly bool MicEnabled;

	public LivestreamingStatus(IntPtr o)
	{
		IsPaused = CAPI.ovr_LivestreamingStatus_GetIsPaused(o);
		LivestreamingEnabled = CAPI.ovr_LivestreamingStatus_GetLivestreamingEnabled(o);
		MicEnabled = CAPI.ovr_LivestreamingStatus_GetMicEnabled(o);
	}
}
