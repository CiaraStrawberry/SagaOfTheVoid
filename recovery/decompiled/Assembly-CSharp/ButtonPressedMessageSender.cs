using UnityEngine;

public class ButtonPressedMessageSender : MonoBehaviour
{
	public string message;

	public GameObject recipiant;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider col)
	{
		if (!(col.name == "Controller (right)") && !(col.name == "Controller (left)") && !(col.name == "LeftController") && !(col.name == "RightController"))
		{
		}
	}

	public void sendmessage()
	{
		Debug.Log(message);
		if ((bool)recipiant)
		{
			recipiant.SendMessage(message);
		}
	}
}
