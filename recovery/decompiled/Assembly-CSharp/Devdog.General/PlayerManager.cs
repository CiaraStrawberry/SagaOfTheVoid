using System;

namespace Devdog.General;

public class PlayerManager : ManagerBase<PlayerManager>
{
	private Player _currentPlayer;

	public Player currentPlayer
	{
		get
		{
			return _currentPlayer;
		}
		set
		{
			Player arg = _currentPlayer;
			_currentPlayer = value;
			if (OnPlayerChanged != null)
			{
				OnPlayerChanged(arg, _currentPlayer);
			}
		}
	}

	public event Action<Player, Player> OnPlayerChanged;
}
