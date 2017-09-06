using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class sends a message to the desired recipiant if the player interacts with it or something.
/// </summary>
public class ButtonHoverMessageSender : MonoBehaviour {

    // Message Reciever
    public GameObject Recipiant;
    // Message string.
    public string Message;

	/// <summary>
    /// Send the desired Message.
    /// </summary>
	public void SendMessage ()
    {
        Recipiant.SendMessage(Message);
    }
}
