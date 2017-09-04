using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHoverMessageSender : MonoBehaviour {
    public GameObject Recipiant;
    public string Message;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public void SendMessage ()
    {
        Recipiant.SendMessage(Message);
    }
}
