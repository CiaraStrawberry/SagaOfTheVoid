using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateMatchUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(Example());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Load() {
      //  name = GameObject.Find("Controllers").transform.FindChild("JoinMultiplayer").GetComponent<JoinMultiplayerGame>().roomName;

      //  GetComponent<Text>().text = name;
    }
    IEnumerator Example()
    {
        yield return new WaitForSeconds(2);
        Load();
    }

}
