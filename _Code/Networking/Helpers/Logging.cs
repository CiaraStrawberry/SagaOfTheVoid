using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DFNetwork.Debugging {
		
	public static class Logging {
		
		/// <summary>
		/// Event for when a new log item is added
		/// </summary>
		public static System.Action<LogItem> OnNewLogItem;
		
		/// <summary>
		/// Log item struct
		/// </summary>
		public struct LogItem {
			
			public System.DateTime LogTime;
			public string Location;
			public string Message;
			
			public LogItem(System.DateTime stamp, string location, string message) {
				
				LogTime = stamp;
				Location = location;
				Message = message;
				
			}
			
		}
		
		private static List<LogItem> LogItems = new List<LogItem>();
		
		/// <summary>
		/// Adds the log item.
		/// </summary>
		/// <param name='location'>
		/// Location.
		/// </param>
		/// <param name='message'>
		/// Message.
		/// </param>
		public static void AddLogItem(string location, string message) {
			
			LogItem lg = new LogItem(System.DateTime.Now, location, message);
			
			LogItems.Add(lg);
			
			//Debug.Log(location + ": " + message);
			
			if (OnNewLogItem != null) { OnNewLogItem(lg); }
			
		}
		
		/// <summary>
		/// Gets the get log list.
		/// </summary>
		/// <value>
		/// The get log list.
		/// </value>
		public static List<LogItem> GetLogList {
			
			get { return LogItems; }
			
		}
		
	}
	
}