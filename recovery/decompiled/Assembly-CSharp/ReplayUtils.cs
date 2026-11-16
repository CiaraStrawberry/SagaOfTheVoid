using System;
using System.Collections.Generic;
using System.IO;
using TrueSync;
using UnityEngine;

public class ReplayUtils
{
	public static string replayContext;

	private const string REPLAY_FOLDER = "replays";

	public static void Init()
	{
		ReplayRecord.ReplayRecordSave = (ReplayRecordSave)Delegate.Combine(ReplayRecord.ReplayRecordSave, new ReplayRecordSave(SaveRecord));
	}

	private static void SaveRecord(byte[] replayRecord, int numberOfPlayers)
	{
		string fullName = CheckReplayFolder().FullName;
		try
		{
			File.WriteAllBytes($"{fullName}/replay_{DateTime.Now.Ticks}_{numberOfPlayers}.tsr", replayRecord);
		}
		catch (Exception)
		{
		}
	}

	public static List<ReplayRecordInfo> GetContextRecords()
	{
		List<ReplayRecordInfo> list = new List<ReplayRecordInfo>();
		FileInfo[] files = CheckReplayFolder().GetFiles("*.tsr");
		foreach (FileInfo fileInfo in files)
		{
			ReplayRecordInfo replayRecordInfo = new ReplayRecordInfo(fileInfo);
			if (replayRecordInfo != null)
			{
				list.Add(replayRecordInfo);
			}
		}
		list.Sort();
		return list;
	}

	public static ReplayRecord GetReplayRecord(string fileFullName)
	{
		ReplayRecord result = null;
		try
		{
			byte[] replayContent = File.ReadAllBytes(fileFullName);
			result = ReplayRecord.GetReplayRecord(replayContent);
		}
		catch (Exception)
		{
		}
		return result;
	}

	public static void ClearAllReplayRecords()
	{
		FileInfo[] files = CheckReplayFolder().GetFiles("*.tsr");
		foreach (FileInfo fileInfo in files)
		{
			fileInfo.Delete();
		}
	}

	private static DirectoryInfo CheckReplayFolder()
	{
		string text = null;
		text = ((replayContext == null || replayContext.Trim().Length <= 0) ? string.Format("{0}/{1}", Application.persistentDataPath, "replays") : string.Format("{0}/{1}/{2}", Application.persistentDataPath, "replays", replayContext));
		DirectoryInfo directoryInfo = new DirectoryInfo(text);
		if (!directoryInfo.Exists)
		{
			directoryInfo.Create();
		}
		return directoryInfo;
	}
}
