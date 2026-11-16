using System.Threading;

namespace Apex.Utilities;

public class ThreadFactory : IThreadFactory
{
	public Thread CreateThread(string name, ParameterizedThreadStart proc)
	{
		Thread thread = new Thread(proc);
		thread.Name = name;
		return thread;
	}
}
