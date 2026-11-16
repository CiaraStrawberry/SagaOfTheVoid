using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal class RayDataComparer : IComparer<FP>
{
	int IComparer<FP>.Compare(FP a, FP b)
	{
		FP fP = a - b;
		if (fP > 0)
		{
			return 1;
		}
		if (fP < 0)
		{
			return -1;
		}
		return 0;
	}
}
