using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPressedMessageSender : MonoBehaviour {
    public string message;
    public GameObject recipiant;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter(Collider col)
    {
        if (col.name == "Controller (right)" || col.name == "Controller (left)" || col.name == "LeftController" || col.name == "RightController")
        {
          //  recipiant.SendMessage(message);
        }

    }
    public void sendmessage ()
    {
        Debug.Log(message);
        if(recipiant)   recipiant.SendMessage(message);
    }
}
