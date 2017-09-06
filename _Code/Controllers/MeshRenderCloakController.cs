using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

/// <summary>
/// This function turns the cloack on a ship on or off depending on the situation.
/// </summary>
public class MeshRenderCloakController : MonoBehaviour {

    // is clocked this frame?
    public bool cloaked;
    // was cloaked last frame?
    public bool cloakedlast;
    // the material to switch to when cloaked.
    public Material cloakedmat;
    // the material to switch to when uncloaked.
    private Material original;
    // the mesh to apply the material to.
    private MeshRenderer mesh;
    // the game controller.
    private UnitMovementcommandcontroller unitcontrol;
    // the parent class script.
    private _Ship parent;
    // the engines to disable when cloacked.
    private GameObject engines;
    // the child meshes to switch material on.
    public List<MeshRenderer> parentchildren = new List<MeshRenderer>();
    // distance traveled since cloak started
    public bool distancecloacked;
    // the pathfinding script attached to the main _ship script.
    public CustomPathfinding parentpasfinding;
    // the world root.
    private GameObject World;
    // has the game started?
    public bool started;
    // the custom trail rendererer script.
    public LineRenderController linerencontrol;
    // time since the game started.
    int timefromupdate;

    /// <summary>
    /// Update on the trail renderer.
    /// </summary>
    public void specupdate ()
    {
        if (linerencontrol) linerencontrol.OnSyncedUpdatspec();
    }

    /// <summary>
    /// initialise on deterministic start.
    /// </summary>
    public void StartMain()
    {
        mesh = GetComponent<MeshRenderer>();
        original = mesh.material;
        unitcontrol = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
        parent = transform.parent.GetComponent<_Ship>();
        parentpasfinding = transform.parent.GetComponent<CustomPathfinding>();
        World = GameObject.Find("WorldScaleBase");
        if (transform.parent.Find("Engines")) engines = transform.parent.Find("Engines").gameObject;
        foreach (Transform t in transform.parent) if (t.gameObject != this.gameObject && t.tag == "Pickup") foreach (MeshRenderer mesh in t.GetComponentsInChildren<MeshRenderer>()) parentchildren.Add(mesh);
        started = true;
        if (transform.childCount > 0) linerencontrol = transform.GetChild(0).GetComponent<LineRenderController>();
    }
   
	/// <summary>
    /// Update the cloak status.
    /// </summary>
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

    /// <summary>
    /// Enable the cloak.
    /// </summary>
    public void cloack()
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

    /// <summary>
    /// disable the cloak.
    /// </summary>
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

    /// <summary>
    /// Check if cloak should be turned off.
    /// </summary>
    /// <returns>the result of the check.</returns>
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

    /// <summary>
    /// get the closest Gameobject.
    /// </summary>
    /// <returns>the closest gameobject</returns>
    TSTransform checkclosest ()
    {
        TSTransform output = null;
        TSTransform[] objs = unitcontrol.targetsout(parent.ShipColor).ToArray();
        float distance = Mathf.Infinity;
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i] != null && objs[i].gameObject != null)
            {
                float dis = Vector3.Distance(objs[i].transform.localPosition, transform.parent.localPosition);
                if (distance > dis)
                {
                    distance = dis;
                    if (dis < 2000) output = objs[i];
                }
            }
        }
        return output;
    }
}
