using UnityEngine;

public class MoneyCheckScript : MonoBehaviour
{
	public GameObject ship;

	private _Ship shipscript;

	private TextMesh textmesh;

	private void Start()
	{
		shipscript = ship.GetComponent<_Ship>();
		textmesh = GetComponent<TextMesh>();
		textmesh.text = shipscript.PointsToDeploy.ToString();
	}

	private void Update()
	{
	}
}
