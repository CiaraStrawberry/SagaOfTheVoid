using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// This class attaches to the player controller and checks if there is an object which has a set tag near the controller and if there are more then one, which is the closest.
/// </summary>
public class onTriggerEnterEnemy : MonoBehaviour
{
    // the nearest object.
    public GameObject focus;
    // the nearest object for the holotag.
    public GameObject holofocus;
    // the nearest of both.
    public GameObject closest;
    // colliders to see which object is inside which.
    public Collider[] cols;
    // the main tag to look for.
    public string maintag = "Pickup";
    // the holo tag to look for.
    public string holotag = "Hologram";
    // the size of the box to check.
    public float flo;
    // a box collider to use to check.
    private BoxCollider box;
    // all colliders inside the box.
    List<Collider> list = new List<Collider>();
    // colliders inside the box with the holo tag.
    List<GameObject> Holograms = new List<GameObject>();
    // has the game started?
    public bool ingamemode;
    // the root gameobject holding all other gameobjects in the world.
    public GameObject ObjectsHolder;
    // the Game Controller.
    private UnitMovementcommandcontroller unitcontrol;

    /// <summary>
    /// Initialise The Sceneloading.
    /// </summary>
    void Awake ()
    {
       SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    /// <summary>
    /// check which level is loaded and act upon it.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="scenemode"></param>
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
    {
      if(this != null && this.gameObject != null)  box = this.GetComponent<BoxCollider>();
        if (ingamemode == true) ObjectsHolder = GameObject.Find("Objects");
       
    }

    /// <summary>
    /// Update box sizes and check closest player.
    /// </summary>
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

    /// <summary>
    /// Get the _ship components of the selected ship and turn on its healthbar.
    /// </summary>
    void LateUpdate ()
    {
      if(closest)
      {
            _Ship closestship = closest.GetComponent<_Ship>();
          if(closestship != null) closestship.enablehealthbar(); 
      }
    }

    /// <summary>
    /// Find the closest player to the player controller with maths.
    /// </summary>
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
            for (int i = 0; i < colliders.Length; i++)
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
        else focus = null;
    }
}
