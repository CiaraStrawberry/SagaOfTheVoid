using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class MeshRenderCloakController : MonoBehaviour {
    public bool cloaked;
    public bool cloakedlast;
    public Material cloakedmat;
    private MeshRenderer mesh;
    private Material original;
    private UnitMovementcommandcontroller unitcontrol;
    private _Ship parent;
    private GameObject engines;
    public List<MeshRenderer> parentchildren = new List<MeshRenderer>();
    public bool distancecloacked;
    public CustomPathfinding parentpasfinding;
    private GameObject World;
    public bool started;
    public LineRenderController linerencontrol;
    // Use this for initialization
    public void specupdate ()
    {
        if (linerencontrol) linerencontrol.OnSyncedUpdatspec();
    }
    public void StartMain () {
        mesh = GetComponent<MeshRenderer>();
        original = mesh.material;
        unitcontrol = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
        parent = transform.parent.GetComponent<_Ship>();
        parentpasfinding = transform.parent.GetComponent<CustomPathfinding>();
        World = GameObject.Find("WorldScaleBase");
        if (transform.parent.Find("Engines")) engines = transform.parent.Find("Engines").gameObject;
        foreach (Transform t in transform.parent) if (t.gameObject != this.gameObject && t.tag == "Pickup") foreach(MeshRenderer mesh in t.GetComponentsInChildren<MeshRenderer>()) parentchildren.Add(mesh);
        started = true;
      if(transform.childCount > 0)  linerencontrol = transform.GetChild(0).GetComponent<LineRenderController>();
    }
    int timefromupdate;
	
	// Update is called once per frame
	void Update () {
        //  if(  timefromupdate++; 
    //    if (linerencontrol) linerencontrol.OnSyncedUpdate
	}
    void LateUpdate ()
    {
        if (started)
        {
            if (mesh.enabled == false) mesh.enabled = true;
            if (cloakedlast != cloaked)
            {
                if (cloaked) cloack();
                else if (distancecloacked == false)
                {
                    mesh.material = original;
                    mesh.enabled = true;
                    if (engines && parentpasfinding.currentlymoving == true) engines.SetActive(true);
                    foreach (MeshRenderer gam in parentchildren) gam.enabled = true;
                }
                cloakedlast = cloaked;
            }
            if (cloaked && engines && engines.GetActive() == true) engines.SetActive(false);

        }

        if(parentpasfinding == null) parentpasfinding = transform.parent.GetComponent<CustomPathfinding>();
        if (parentpasfinding && parentpasfinding.timepassedsincespawn == 0)
        {
            if (mesh == null) mesh = GetComponent<MeshRenderer>();
            if (mesh && mesh.enabled == true) mesh.enabled = false;
        }
        

       
    }
    public void cloack ()
    {
       if (unitcontrol && unitcontrol.alliesout() != null && unitcontrol.alliesout().Contains(parent.GetComponent<TSTransform>()))
       {
              mesh.material = cloakedmat;
              if (engines && parentpasfinding.currentlymoving == true) engines.SetActive(true);
       }
       else if (mesh)
       {
              mesh.enabled = false;
              if (engines) engines.SetActive(false);
       }

        foreach (MeshRenderer gam in parentchildren) gam.enabled = false;                       
    }
    public void uncloack ()
    {
        if(cloaked == false && mesh)
        {
            mesh.material = original;
            mesh.enabled = true;
            if (engines && parentpasfinding.currentlymoving == true) engines.SetActive(true);
            foreach (MeshRenderer gam in parentchildren) gam.enabled = true;

        }

    }
    public float test;
    public bool asigncloacked ()
    {
        TSTransform temp = checkclosest();
        if (temp != null)
        {
            cloaked = false;
            return false;
        }
        else {
            cloaked = true;
            return true;
        }

        
       
    }

    TSTransform checkclosest ()
    {
        TSTransform output = null;
        TSTransform[] objs = unitcontrol.targetsout(parent.ShipColor).ToArray();
        float distance = Mathf.Infinity;
        for (int i = 0; i < objs.Length; i++)
        {
            if(objs[i] != null && objs[i].gameObject != null)
            {
              float dis = Vector3.Distance(objs[i].transform.localPosition, transform.parent.localPosition);
              if (distance > dis)
              {
                distance = dis;
                if (dis < 2000) output = objs[i];
              }
            }
        
        }
       // if(output != null) test = Vector3.Distance(output.transform.localPosition, transform.parent.localPosition);
        return output;
    }
}
