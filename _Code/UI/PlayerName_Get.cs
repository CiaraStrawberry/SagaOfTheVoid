using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerName_Get : MonoBehaviour {
    private string name;
    private bool count;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Example());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Load()
    {
        name = GameObject.Find("Controllers").transform.Find("JoinMultiplayer").GetComponent<JoinMultiplayerGame>().roomName;
        GetComponent<Text>().text = name;
    }
    IEnumerator Example()
    {
        yield return new WaitForSeconds(2);
        Load();
    }
}

