using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TutorialController : MonoBehaviour {
    public GameObject LookControls;
    public GameObject OrderControls;
    public GameObject GameObjectives;
    private GameObject movebase;
    private Vector3 startpos;
    // Use this for initialization
    void Start() {
        LookControls.SetActive(true);
        OrderControls.SetActive(false);
        GameObjectives.SetActive(false);
        movebase = GameObject.Find("VRTK_SDK");
        startpos =transform.position - movebase.transform.position;
    }
    void Update ()
    {
        transform.position = movebase.transform.position + startpos;
    }
    public void MoveNext ()
    {
        if (LookControls.GetActive() == true)
        {
            OrderControls.SetActive(true);
            LookControls.SetActive(false);
        }
        else if (OrderControls.GetActive() == true)
        {
            OrderControls.SetActive(false);
            GameObjectives.SetActive(true);
        }
        else if (GameObjectives.GetActive() == true)
        {
            GameObjectives.SetActive(false);
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel(0);
        }
    }
}
