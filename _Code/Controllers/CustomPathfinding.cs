using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using System.Linq;

public class CustomPathfinding : TrueSyncBehaviour {
    [SerializeField]
    public TSVector Target;
    public GameObject World;
    public FP turnspeed;
    [AddTracking]
    public FP speed = 0;
    public FP maxspeed;
    public int acceleration;
    public _Ship shipscript;
    public bool moving = true;
    private bool accelerating;
    private int mindecdistance = 300;
    [SerializeField]
    private FP distancetotarget;
    public TSVector localtarget;
    private PhotonTransformView transformview;
    public bool currentlymoving;
    public TSTransform transformts;
    public bool started;
    public float debug;
    public GameObject debuggam;
    public UnitMovementcommandcontroller unitcom;
    public Vector3 Lastpos;
    public Vector3 thisposdif;
    public List<Vector3> startmeshpos = new List<Vector3>();
    //public eHullType Debug;
    // Use this for initialization
    public void Start()
    {
        World = GameObject.Find("World");
        shipscript = GetComponent<_Ship>();
        acceleration = 4;
        turnspeed = shipscript.MaxAngularSpeed;
        transformts = GetComponent<TSTransform>();
        unitcom = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
        waitforpushaway = (int)shipscript.MaxAngularSpeed % 5;
        int i = 0;
        foreach (Transform gam in transform)
        {
            if (gam.name == "Ship")
            {
                startmeshpos.Add(gam.transform.localPosition);
                Vector3 starttrans = gam.transform.localPosition;
                starttrans.z = Mathf.Lerp(-3000, startmeshpos[i].z, 1);
                gam.transform.localPosition = starttrans;
                i++;
            }
        }
    }

    


    // asign move order
	public void assigntarget (TSVector targetin,TSVector localtargetin)
    {
        Target = targetin;
        localtarget = localtargetin;
        Accelerate();
    }

    int waitforpushaway;
    public float timepassedsincespawn;
    public float debugtest;
    // Update is called once per frame
    public void SpecUpdate()
    {
        timepassedsincespawn++;

        if (turnspeed < shipscript.MaxAngularSpeed) turnspeed = turnspeed + 0.003;
        else turnspeed = shipscript.MaxAngularSpeed;
        thisposdif = Lastpos - transform.position;
        Lastpos = transform.position;
       // Debug.DrawLine(transform.position, Target.ToVector());

        if(timepassedsincespawn <= 180)
        {
            int i= 0;
            foreach(GameObject ship in shipscript.meshes)
            {
                Vector3 starttrans = ship.transform.localPosition;
                starttrans.z = Mathf.Lerp(-3000,startmeshpos[i].z, (1 / ( 180 / timepassedsincespawn)));
                debugtest = (1 / (180 / timepassedsincespawn));
                ship.transform.localPosition = starttrans;
                i++;
            }

        }

        if (shipscript && timepassedsincespawn > 180)
        {
            distancetotarget = TSVector.Distance(transformts.position, localtarget);

            if (started == false)
            {
                maxspeed = shipscript.MaxSpeed * 30;
                started = true;
            }

            if (shipscript.WeaponTarget != null) maxspeed = shipscript.MaxSpeed * 30;
            else maxspeed = shipscript.averagespeed * 30;

            if (TrueSyncManager.Time > 0)
            {
                currentlymoving = false;

                if ( distancetotarget > 10 || shipscript.HullType == eHullType.Light || shipscript.HullType == eHullType.Corvette)
                {
                    if (localtarget != null && distancetotarget < mindecdistance)
                    {
                        if (speed > 0) speed = speed - acceleration / 2;
                    }
                    else
                    {
                        if (accelerating == true && speed < maxspeed) speed += acceleration;
                        else if (speed > maxspeed) accelerating = false;
                    }

                    if (speed > maxspeed) speed = maxspeed;

                    if (shipscript.HullType == eHullType.Light || shipscript.HullType == eHullType.Corvette) speed = maxspeed;

                    if (Target != new TSVector(0, 0, 0) && moving == true && distancetotarget > 150) Look(Target);

                    if ((Target != new TSVector(0, 0, 0) && moving == true) || shipscript.HullType == eHullType.Light || shipscript.HullType == eHullType.Corvette)    Move(speed); 
                }
                else if (shipscript.HullType != eHullType.Light && shipscript.HullType != eHullType.Corvette && speed > 0) Move(speed);
                
                if(distancetotarget < 400) shipscript.movetonextstoredorder();

            }
        }
        else  shipscript = GetComponent<_Ship>();

        waitforpushaway++;

        if ((Closestoverall && shipscript.HullType != eHullType.Light && shipscript.HullType != eHullType.Corvette) && TSVector.Distance(Closestoverall.position, transformts.position) < 300 && Closestoverall.shipscript.HullType != eHullType.Light && Closestoverall.shipscript.HullType != eHullType.Corvette)
        {
            TSVector forwards = transformts.position - Closestoverall.position;
            transformts.position = transformts.position + (forwards * 0.005);
        }

        if (waitforpushaway > 10 && shipscript.HullType != eHullType.Light)
        {
            waitforpushaway = 0;
            TSTransform closest = getclosesttarget();
            if (closest != null) Closestoverall = closest;
           
        }
    }

    private TSQuaternion lastquaternion;

    public TSTransform Closestoverall;

    // turn ship towords target
    void Look (TSVector targetin)
    {
        TSVector targetDir = localtarget - transformts.position;
        TSQuaternion  directiontarget = TSQuaternion.LookRotation(targetDir);
        TSQuaternion outputrot = TSQuaternion.Slerp(transformts.rotation, directiontarget, TrueSyncManager.DeltaTime * turnspeed);
        if(TSQuaternion.Angle(outputrot,lastquaternion) < (TrueSyncManager.DeltaTime * turnspeed) / 2) transformts.rotation = outputrot;
        lastquaternion = outputrot;
    }

    // is the target infront of ship.
    bool isInFront(TSVector targetpos)
    {
        return TSVector.Dot(transformts.forward, targetpos) > 0;
    }

    // is the target infront of ship.
    bool FrontTest()
    {
        TSVector fwd = transformts.forward;
        TSVector vec = localtarget - transformts.position;
        vec.Normalize();
        FP ang = TSMath.Acos(TSVector.Dot(fwd, vec)) * Mathf.Rad2Deg;
        if (ang <= 45.0f)
            return true;

        return false;
    }

    // move ship forwards
    void Move (FP speedin)
    {
        if (FrontTest() == true || shipscript.HullType == eHullType.Light)
        {
            TSVector oripos = transformts.position;
            TSVector forward = transformts.forward;

            if (moving) transformts.position = transformts.position + (transformts.forward * (speedin / 60));
            if (moving && speedin > acceleration) currentlymoving = true;
            if (transformts.position.x > 20000 || transformts.position.x > 20000 || transformts.position.x > 20000) transformts.position = oripos;
        }
    }

    
    public List<TSTransform> allshipstemp = new List<TSTransform>();

    // start speeding up, for smoooth movement.
    void Accelerate ()  { accelerating = true;    }

    // get closest ship to ensure nothing collides.
    TSTransform getclosesttarget()
    {
        if (unitcom)
        {
            List<TSTransform> output = new List<TSTransform>();
            
          
            if (unitcom.allshipststransform.Length != 0) {
                output = unitcom.allshipststransform .ToList();
                allshipstemp = output;
            }
            else output = allshipstemp;
            if(output.Contains(transformts)) output.Remove(transformts);
            
           
            if (output.Count != 0)
            {
                List<TSTransform> removable = new List<TSTransform>();
                foreach (TSTransform gam in output)
                {
                    if (gam == null || gam.gameObject == null) removable.Add(gam);
                }
                foreach (TSTransform gam in removable) output.Remove(gam);
                output = output.OrderBy(x => TSVector.Distance(transformts.position, x.position)).ToList();
                if (output.Count > 0) return output[0];
                else return null;
            }
            else return null;
        }
        else return null;
    }
}
