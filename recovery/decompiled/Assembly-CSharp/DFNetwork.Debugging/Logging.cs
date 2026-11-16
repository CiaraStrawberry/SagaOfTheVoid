using System;
using System.Collections.Generic;

namespace DFNetwork.Debugging;

public static class Logging
{
	public struct LogItem(DateTime stamp, string location, string message)
	{
		public DateTime LogTime = stamp;

		public string Location = location;

		public string Message = message;
	}

	public static Action<LogItem> OnNewLogItem;

	private static List<LogItem> LogItems = new List<LogItem>();

	public static List<LogItem> GetLogList => LogItems;

	public static void AddLogItem(string location, string message)
	{
		LogItem logItem = new LogItem(DateTime.Now, location, message);
		LogItems.Add(logItem);
		if (OnNewLogItem != null)
		{
			OnNewLogItem(logItem);
		}
	}
}
