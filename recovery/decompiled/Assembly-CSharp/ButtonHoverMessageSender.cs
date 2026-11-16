using UnityEngine;

public class ButtonHoverMessageSender : MonoBehaviour
{
	public GameObject Recipiant;

	public string Message;

	private void Start()
	{
	}

	public void SendMessage()
	{
		Recipiant.SendMessage(Message);
	}
}
