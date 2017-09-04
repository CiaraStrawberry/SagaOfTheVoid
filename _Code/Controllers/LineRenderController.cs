using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class LineRenderController : TrueSyncBehaviour
{
    private LineRenderer linerender;
    public Vector3[] Positions;
    GameObject WorldParent;
    public int size = 36;
    public float timebetween = 0.06f;
    GameObject WorldBase;
    Vector3 lastscale;
    // Use this for initialization
    public bool started;
    public bool missile = false;
    public Vector3 lastposition;
    public Vector3[] positionsrealtive;
    public CustomPathfinding parent;
    public GameObject linetargetend;
	public void Start () {
        if (missile == true)
        {
            LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            lineRenderer.startColor = Color.blue;
            lineRenderer.endColor = Color.white;
        }
        i = 0;
        if(missile == false)
        {
            parent = transform.parent.parent.GetComponent<CustomPathfinding>();
        }
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
    int i;
	// Update is called once per frame
	void Update () {
        if (i == 0 && missile) Start();
        i++;
     //   if (WorldBase.transform.localScale != lastscale) Rep();
     //   lastscale = WorldBase.transform.localScale;
       if(missile  == true)
        {
            OnSyncedUpdate();
        }
        if(started == false)
        {
            linerender.startWidth = WorldBase.transform.localScale.x / 35000;
            if (missile) linerender.startWidth = linerender.startWidth *  0.25f;
        }
	}

 
    int skip;
    public override void OnSyncedUpdate()
    {
        if (missile == true) OnSyncedUpdatspec();
    }
    int skipthisrun;
    void skipthisrunadd ()
    {
        if (skipthisrun < 5) skipthisrun++;
    }
    void skipthisrunremove()
    {
        if (skipthisrun > 0)
        {
            lastskip = true;
            skipthisrun--;
        }
    }
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
            else
            {
                for (int i = 0; i < size; i++)
                {
                //    linerender.SetPosition(i, transform.position);
                    linerender.SetPosition(i, Vector3.Lerp(transform.position, linetargetend.transform.position,((float)i/size)));
                }
              //  linerender.SetPosition(size - 1, linetargetend.transform.position);
            }

        }
    }
    private bool lastskip;
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
