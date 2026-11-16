using System.Threading;

namespace Apex.Utilities;

public interface IThreadFactory
{
	Thread CreateThread(string name, ParameterizedThreadStart proc);
}
