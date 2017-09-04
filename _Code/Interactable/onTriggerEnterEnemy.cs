using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class onTriggerEnterEnemy : MonoBehaviour
{

    // This script sorts through all objects in the collider and gets the closest, kinda inefficant but the result is great.
    public GameObject focus;
    public GameObject holofocus;
    public GameObject closest;
    public Collider[] cols;
    public string maintag = "Pickup";
    public string holotag = "Hologram";
    public float flo;
    private BoxCollider box;
    List<Collider> list = new List<Collider>();
    List<GameObject> Holograms = new List<GameObject>();
    public bool ingamemode;
    public GameObject ObjectsHolder;
    private UnitMovementcommandcontroller unitcontrol;
    // Use this for initialization

    void Awake ()
    {
       SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
    {
      if(this != null && this.gameObject != null)  box = this.GetComponent<BoxCollider>();
        if (ingamemode == true) ObjectsHolder = GameObject.Find("Objects");
       
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && box != null)
        {
            flo = box.size.x * 2;
            maintag = "Ship";
        }
        else if(box != null)
        { 
           if(box) flo = box.size.x;
            maintag = "Pickup";
        }
        FindClosestPlayer();
     
        
    }
    void LateUpdate ()
    {
      if(closest)
      {
            _Ship closestship = closest.GetComponent<_Ship>();
          if(closestship != null) closestship.enablehealthbar(); 
      }
    }

    public void FindClosestPlayer()
    {
        if (box && transform.parent.Find("MovePos") != null)
        {
            Collider[] colliders2 = Physics.OverlapBox(transform.parent.Find("MovePos").position, new Vector3(flo * transform.lossyScale.x / 2, flo * transform.lossyScale.x / 2, flo * transform.lossyScale.x / 2));
            for (int i = 0; i < colliders2.Length; i++)
            {
                if (colliders2[i].gameObject.tag == maintag)
                {
                    list.Add(colliders2[i]);
                }
            }
        }
        else if (transform.parent.Find("MovePos") != null && ObjectsHolder != null)
        {
            Collider[] colliders2 = Physics.OverlapBox(transform.parent.Find("MovePos").position, new Vector3(flo * ObjectsHolder.transform.lossyScale.x / 2, flo * ObjectsHolder.transform.lossyScale.x / 2, flo * ObjectsHolder.transform.lossyScale.x / 2));
            for (int i = 0; i < colliders2.Length; i++)
            {
                if (colliders2[i].gameObject.tag == maintag)
                {
                    list.Add(colliders2[i]);
                }
            }
        }



        GameObject[] colliders3 = Holograms.ToArray(); ;
        Holograms.Clear();
         if (colliders3.Length != 0)
         {
            float distance = Mathf.Infinity;
            for (int i = 0; i < colliders3.Length; i++)
            {
               Vector3 diff = colliders3[i].transform.position - transform.parent.Find("MovePos").position;
               float curDistance = diff.sqrMagnitude;
               if (curDistance < distance)
               {
                  closest = colliders3[i].gameObject;
                  distance = curDistance;
               }
               
             }
             holofocus = closest;
          }
        if (closest == null) holofocus = null;
        closest = null;







        Collider[] colliders = list.ToArray(); ;
           list.Clear();
        if (colliders.Length != 0)
        {
            float distance;
        distance = Mathf.Infinity;
        for (int i = 0; i < colliders.Length ; i++)
        {
               
                Vector3 diff = colliders[i].transform.position - transform.parent.Find("MovePos").position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = colliders[i].gameObject;
                    distance = curDistance;
                }

           
           
        }
        focus = closest;
            

        }
        else
        {
            focus = null;
        }

     
    }

}
