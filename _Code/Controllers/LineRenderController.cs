using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;


/// <summary>
/// Turns an attached linerender into a trail renderer that works in local space.
/// This script is probably the second most worked on in the entire program. I had to write my own trail renderer that works exclusively in world space.
/// It was shockingly hard.
/// </summary>
public class LineRenderController : TrueSyncBehaviour
{
    // the linerenderer to manipulate.
    private LineRenderer linerender;
    // the vertex positions of the linerenderer.
    public Vector3[] Positions;
    // the world parent containing all relevant objects to the script.
    GameObject WorldParent;
    // the number of vertexes.
    public int size = 36;
    // the time between the vertex points updating.
    public float timebetween = 0.06f;
    // the root world gameobject.
    GameObject WorldBase;
    // the scale of the missile last frame.
    Vector3 lastscale;
    // Has the game started yet?
    public bool started;
    // is this a missile trail?
    public bool missile = false;
    // the Position of the missile last frame.
    public Vector3 lastposition;
    // the Ship referance that the missile came from.
    public CustomPathfinding parent;
    // badly named variable that counts updates since the start to allow functions to call after a set number of updates.
    int i;
    // counting int to allow functions to call every few frames. it is cosmetic so it doesnt matter that much if it desyncs.
    int skip;
    // does the same thing as above. I dont know why i have 3 variables that do the same thing.
    int skipthisrun;
    // did the last frame skip?
    private bool lastskip;

    /// <summary>
    /// initialise the trail.
    /// </summary>
	public void Start () {
        if (missile == true)
        {
            LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.startColor = Color.blue;
            lineRenderer.endColor = Color.white;
        }
        i = 0;
        if(missile == false) parent = transform.parent.parent.GetComponent<CustomPathfinding>();
        Positions = new Vector3[size];
        positionsrealtive = new Vector3[size];
        transform.localPosition = transform.localPosition + new Vector3(0,0,-5);
        linerender = GetComponent<LineRenderer>();
        WorldParent = GameObject.Find("Objects");
        WorldBase = GameObject.Find("WorldScaleBase");
        linerender.startWidth = 1;
        linerender.endWidth = 0;
        linerender.positionCount = Positions.Length;
        for (int i = 0; i < Positions.Length; i++) Positions[i] = WorldParent.transform.InverseTransformPoint(transform.position);
        for (int i = 0; i < Positions.Length; i++)  linerender.SetPosition(i, WorldParent.transform.TransformPoint(Positions[i]));
        linerender.startWidth = WorldBase.transform.localScale.x / 38000;
        Rep();
      
    }
   
	/// <summary>
    /// Updates the function b
    /// </summary>
	void Update () {
        if (i == 0 && missile) Start();
        i++;
        if(missile  == true) OnSyncedUpdate();
        if(started == false)
        {
            linerender.startWidth = WorldBase.transform.localScale.x / 35000;
            if (missile) linerender.startWidth = linerender.startWidth *  0.25f;
        }
	}

 
   /// <summary>
   /// Uses determinism to update if no a missile.
   /// </summary>
    public override void OnSyncedUpdate()
    {
        if (missile == true) OnSyncedUpdatspec();
    }
  
    /// <summary>
    /// skips every few updates in a really strange pattern.
    /// </summary>
    void skipthisrunadd ()
    {
        if (skipthisrun < 5) skipthisrun++;
    }

    /// <summary>
    /// skips updates.
    /// </summary>
    void skipthisrunremove()
    {
        if (skipthisrun > 0)
        {
            lastskip = true;
            skipthisrun--;
        }
    }

    /// <summary>
    /// The actual code that updates the line based on its position to the parent gameobject.
    /// </summary>
    public void OnSyncedUpdatspec()
    {
        if (parent == null || parent.timepassedsincespawn > 180)
        {
            started = true;
            if (this != null && linerender != null)
            {
                if (WorldBase == null) WorldBase = GameObject.Find("WorldScaleBase");
                if (WorldBase.transform.localScale != lastscale) skipthisrunadd();
                else  skipthisrunremove();
                
                if (skipthisrun != 0)
                {
                    lastscale = WorldBase.transform.localScale;
                    for (int i = 0; i < Positions.Length; i++) linerender.SetPosition(i, WorldParent.transform.TransformPoint(WorldParent.transform.InverseTransformPoint(transform.position) + positionsrealtive[i]));
                    if (missile) linerender.startWidth = WorldBase.transform.localScale.x / 12000 * 0.1f;
                    else linerender.startWidth = WorldBase.transform.localScale.x / 12000;
                    for (int i = 0; i < Positions.Length; i++) Positions[i] = (WorldParent.transform.InverseTransformPoint(transform.position) + positionsrealtive[i]);
                }
                else
                {
                    linerender.SetPosition(0, transform.position);
                    positionsrealtive[0] = new Vector3(0, 0, 0);
                    for (int i = positionsrealtive.Length - 1; i > 0; i--) positionsrealtive[i] = Positions[i] - WorldParent.transform.InverseTransformPoint(transform.position);
                    skip++;
                    if (skip == 3)
                    {
                        lastskip = false;
                        skip = 0;
                        Rep();
                    }
                }
                lastposition = transform.position;
            }
            else if (linerender == null && this != null)
            {
                Start();
            }
        }
        else if(missile == false)
        {
            if (linerender == null && this != null) Start();
            else  for (int i = 0; i < size; i++)   linerender.SetPosition(i, Vector3.Lerp(transform.position, linetargetend.transform.position,((float)i/size)));
        }
    }
  
    /// <summary>
    /// The thing that updates once a second to update relevant data.
    /// </summary>
   void Rep()
    {
        if(this != null)
        {
          if (WorldBase == null) WorldBase = GameObject.Find("WorldScaleBase");
          for (int i = Positions.Length - 1; i > 0; i--) Positions[i] = Positions[i - 1];
          Positions[0] = WorldParent.transform.InverseTransformPoint(transform.position);
          for (int i = Positions.Length - 1; i > 0; i--) if(Positions[i].x  == 0) Positions[i] = Positions[0];
          for (int i = 0; i < Positions.Length;i++) if (i > 0 && Positions[i] != Positions[i - 1] || Positions[i] == WorldParent.transform.InverseTransformPoint(transform.position)) linerender.SetPosition(i,WorldParent.transform.TransformPoint( Positions[i]));
          linerender.SetPosition(0, WorldParent.transform.TransformPoint(Positions[0]));
        }

    }
}
