using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowNumPlayers : MonoBehaviour {

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

       // GetComponent<Text>().text = GameObject.Find("Controllers").transform.FindChild("JoinMultiplayer").GetComponent<JoinMultiplayerGame>().getrooms().ToString();
        GetComponent<Text>().text = "return";
    }
    IEnumerator Example()
    {
        yield return new WaitForSeconds(2);
        Load();
    }
}

