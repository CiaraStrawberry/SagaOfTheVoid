using UnityEngine;

public class MainMenuMoneyController : MonoBehaviour
{
	private TextMesh textmesh;

	private FleetBuilderController fleetparent;

	private void Start()
	{
		textmesh = GetComponent<TextMesh>();
		fleetparent = GameObject.Find("Fleet Builder Parent").GetComponent<FleetBuilderController>();
	}

	private void Update()
	{
		textmesh.text = fleetparent.currentmoney + " left \n out of " + fleetparent.limit;
	}
}
