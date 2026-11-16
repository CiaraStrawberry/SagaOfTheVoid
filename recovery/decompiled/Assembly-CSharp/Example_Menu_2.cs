public class Example_Menu_2 : aMenuState
{
	public override void OnTransitionFrom()
	{
	}

	public override void OnActive()
	{
		gameObject.SetActive(value: true);
	}

	public override void OnTransitionTo()
	{
	}

	public override void OnDeactivate()
	{
		gameObject.SetActive(value: false);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
