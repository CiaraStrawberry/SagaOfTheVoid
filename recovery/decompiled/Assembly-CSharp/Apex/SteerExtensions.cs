using Apex.Steering;
using Apex.Units;

namespace Apex;

public static class SteerExtensions
{
	public static void SetFormation<T>(this IGrouping<T> grouping, IFormation formation) where T : IGroupable<T>
	{
		int groupCount = grouping.groupCount;
		for (int i = 0; i < groupCount; i++)
		{
			grouping[i].SetFormation(formation);
		}
	}

	public static void ClearFormation<T>(this IGrouping<T> grouping) where T : IGroupable<T>
	{
		int groupCount = grouping.groupCount;
		for (int i = 0; i < groupCount; i++)
		{
			grouping[i].ClearFormation();
		}
	}
}
