using System;
using System.IO;
using TrueSync;

public class ReplayRecordInfo : IComparable<ReplayRecordInfo>
{
	public DateTime creationDate;

	public string fileFullName;

	public int numberOfPlayers = 1;

	public ReplayRecordInfo(FileInfo fileInfo)
	{
		creationDate = fileInfo.CreationTime;
		fileFullName = fileInfo.FullName;
		string[] array = fileInfo.Name.Split('_');
		if (array.Length >= 3)
		{
			numberOfPlayers = int.Parse(array[2].Split('.')[0]);
		}
	}

	public void Load()
	{
		ReplayRecord.replayToLoad = ReplayUtils.GetReplayRecord(fileFullName);
	}

	public int CompareTo(ReplayRecordInfo other)
	{
		return other.creationDate.CompareTo(creationDate);
	}
}
