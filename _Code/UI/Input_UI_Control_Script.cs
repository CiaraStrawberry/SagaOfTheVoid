using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Input_UI_Control_Script : MonoBehaviour {
    public SceneField changescene;
    private bool isup;
    private Vector3 startpos;
    private GameObject galaxy;
    private Vector3 Startme;
    private Vector3 startposme;
    public GameObject parent;
    // Use this for initialization
    void Start()
    {


        galaxy = GameObject.Find("Controllers");
        startpos = transform.parent.localPosition;
        startposme = transform.position;
        parent = transform.parent.gameObject;
        Startme = transform.localPosition;
    //    transform.parent = null;
    }
    // Use this for initialization
    void Load ()
    {
        SceneManager.LoadSceneAsync(changescene,LoadSceneMode.Single);
    }
    void Update()
    {
        if (parent)
        {
          //  if (isup == false) transform.position = 
            if (isup == false) parent.transform.localPosition = startpos;
            else parent.transform.localPosition = startpos + new Vector3(0, 0.01f, 0);
            isup = false;
          // transform.position = startposme;
        }
    }
    void up()
    {
        isup = true;
        if (parent)
        {
        transform.position = parent.transform.position;
        transform.parent = null;
        }
        else
        {

        }

    
    }
    public void state ()
    {

    }

}
