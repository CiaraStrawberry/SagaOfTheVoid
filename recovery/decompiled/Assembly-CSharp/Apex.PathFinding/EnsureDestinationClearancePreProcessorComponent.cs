using Apex.Common;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding;

[AddComponentMenu("Apex/Game World/Request PreProcess: Ensure Destination Clearance", 1040)]
[ApexComponent("Game World")]
public class EnsureDestinationClearancePreProcessorComponent : MonoBehaviour, IRequestPreProcessor
{
	private struct MatchData
	{
		internal float unitRadius;

		internal float baseClearance;

		internal bool HasProperClearance(Cell c)
		{
			IHaveClearance haveClearance = (IHaveClearance)c;
			return haveClearance.clearance >= unitRadius;
		}

		internal bool IsBlocked(Cell c)
		{
			IHaveClearance haveClearance = (IHaveClearance)c;
			return haveClearance.clearance < baseClearance;
		}
	}

	private GridEvaluatorClosestMatch _eval = new GridEvaluatorClosestMatch(50);

	[SerializeField]
	[Tooltip("The priority of the pre processor in relation to other pre processors. Highest priority is executed first.")]
	private int _priority = 100;

	public int priority
	{
		get
		{
			return _priority;
		}
		set
		{
			_priority = value;
		}
	}

	public bool PreProcess(IPathRequest request)
	{
		IGrid grid = GridManager.instance.GetGrid(request.to);
		if (grid == null)
		{
			return false;
		}
		if (!(grid.GetCell(request.to, adjustToBounds: false) is IHaveClearance haveClearance))
		{
			return false;
		}
		float radius = request.requesterProperties.radius;
		if (haveClearance.clearance >= radius)
		{
			return false;
		}
		MatchData matchData = new MatchData
		{
			unitRadius = radius,
			baseClearance = haveClearance.clearance
		};
		Cell cell = _eval.ClosestMatch(grid, request.to, ((MatchData)matchData).HasProperClearance, ((MatchData)matchData).IsBlocked);
		if (cell != null)
		{
			request.to = cell.position;
			return true;
		}
		return false;
	}
}
