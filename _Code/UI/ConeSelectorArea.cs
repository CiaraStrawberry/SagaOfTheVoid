using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeSelectorArea : MonoBehaviour {
    private GameObject inside;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider col)
    {
        if (col.gameObject.GetComponent<_Ship>() != null)
        {
           transform.parent.parent.gameObject.SendMessage("addtolist", col.gameObject, SendMessageOptions.DontRequireReceiver);
           transform.parent.gameObject.SendMessage("addtolist", col.gameObject, SendMessageOptions.DontRequireReceiver);
        }
     
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.GetComponent<_Ship>() != null)
        {
            transform.parent.parent.gameObject.SendMessage("addtolist", col.gameObject, SendMessageOptions.DontRequireReceiver);
            transform.parent.gameObject.SendMessage("addtolist", col.gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }

}
