using System.Collections.Generic;

namespace Devdog.General;

public interface ITriggerRangeHandler
{
	IEnumerable<Player> GetPlayersInRange();

	bool IsPlayerInRange(Player player);
}
