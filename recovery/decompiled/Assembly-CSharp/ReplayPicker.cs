using TrueSync;
using UnityEngine;

public class ReplayPicker : MonoBehaviour
{
	public static ReplayRecordInfo replayToLoad;

	public ReplayRecordInfo replayRecord;

	public void Pick()
	{
		replayToLoad = replayRecord;
		ReplayRecord.replayMode = ReplayMode.LOAD_REPLAY;
		Menu.instance.ReplayPanel_LoadLevel();
	}
}
