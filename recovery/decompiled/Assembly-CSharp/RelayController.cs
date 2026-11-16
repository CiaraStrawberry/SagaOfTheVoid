using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class RelayController : MonoBehaviour
{
	public InputRelay relay;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void ordermove(List<GameObject> selectedshipsin, TSVector positionin, GameObject inputgam, FP averagespeedin, bool waypoints)
	{
		relay.ordermove(selectedshipsin, positionin, inputgam, averagespeedin, waypoints);
	}

	public void ordershipspawn(int ship, TSVector pos, int spawnteam, int ViewIDin)
	{
		if ((bool)relay)
		{
			relay.ordershipspawn(ship, pos, spawnteam, ViewIDin);
		}
	}
}
