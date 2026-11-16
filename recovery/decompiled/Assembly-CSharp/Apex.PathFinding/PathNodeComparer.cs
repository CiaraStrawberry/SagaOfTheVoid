using System.Collections.Generic;

namespace Apex.PathFinding;

public sealed class PathNodeComparer : IComparer<IPathNode>
{
	public int Compare(IPathNode x, IPathNode y)
	{
		int num = y.f.CompareTo(x.f);
		if (num == 0)
		{
			return y.h.CompareTo(x.h);
		}
		return num;
	}
}
