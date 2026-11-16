using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General;

public class IgnoreRangeTriggerRangeHandler : MonoBehaviour, ITriggerRangeHandler
{
	public IEnumerable<Player> GetPlayersInRange()
	{
		return new Player[0];
	}

	public bool IsPlayerInRange(Player target)
	{
		return true;
	}
}
