using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using System.Linq;

/// <summary>
/// This class attaches to every ship and acts as a simlated agent to move around the world.
/// </summary>
public class CustomPathfinding : TrueSyncBehaviour {

    // the current target Position for this ship.
    [SerializeField]
    public TSVector Target;
    // The current Root Gameobject for everything in the world.
    public GameObject World;
    // the turnspeed determined by the connected _ship class.
    public FP turnspeed;
    // the current speed of this ship determined by the _ship class.
    [AddTracking]
    public FP speed = 0;
    // the maxspeed of this ship determined by the _ship class.
    public FP maxspeed;
    // the acceleration of this ship determined by the _ship class
    public int acceleration;
    // the _ship class to draw relevant data about the ship from.
    public _Ship shipscript;
    // is the ship idle?
    public bool moving = true;
    //is the ship accelerating?
    private bool accelerating;
    // the distance until the ship starts to decellerate.
    private int mindecdistance = 300;
    // the distance between the current ship and its movement target.
    [SerializeField]
    private FP distancetotarget;
    // the target for movement in local space.
    public TSVector localtarget;
    // the photontransformview assosiated with this gameobject.
    private PhotonTransformView transformview;
    // the TSTransform class assosiated with this gameobject.
    public TSTransform transformts;
    // has the game started?
    public bool started;
    // the game controller.
    public UnitMovementcommandcontroller unitcom;
    // the ships last position, so it can reset if flying outside the map.
    public Vector3 Lastpos;
    // the distance to the ships last position.
    public Vector3 thisposdif;
    // the position of every mesh gameobject connected to this one.
    public List<Vector3> startmeshpos = new List<Vector3>();
    // time until the ship is pushed away from the nearest ship, only turns every now and then for performance reasons.
    int waitforpushaway;
    // time passed since the ship has spawned.
    public float timepassedsincespawn;
    // The last direction toworods the closest gameobject to this gameobject.
    private TSQuaternion lastquaternion;
    // the closest gameobject to the gameobject.
    public TSTransform Closestoverall;
    // a list of all ships to chache.
    public List<TSTransform> allshipstemp = new List<TSTransform>();

    /// <summary>
    /// The initialisation function, transfers variables from the _shipscript and initialises everything else.
    /// </summary>
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

    /// <summary>
    /// Update the Ships current movement target.
    /// </summary>
    /// <param name="targetin"> the target position in worldspace</param>
    /// <param name="localtargetin"> the target positin in local space.</param>
	public void assigntarget (TSVector targetin,TSVector localtargetin)
    {
        Target = targetin;
        localtarget = localtargetin;
        Accelerate();
    }

   /// <summary>
   /// Deterministic update function that, well , deterministically updates everything.
   /// </summary>
    public void SpecUpdate()
    {
        timepassedsincespawn++;
        if (turnspeed < shipscript.MaxAngularSpeed) turnspeed = turnspeed + 0.003;
        else turnspeed = shipscript.MaxAngularSpeed;
        thisposdif = Lastpos - transform.position;
        Lastpos = transform.position;
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


    /// <summary>
    /// Turn the ship to look at a target.
    /// </summary>
    /// <param name="targetin"></param>
    void Look (TSVector targetin)
    {
        TSVector targetDir = localtarget - transformts.position;
        TSQuaternion  directiontarget = TSQuaternion.LookRotation(targetDir);
        TSQuaternion outputrot = TSQuaternion.Slerp(transformts.rotation, directiontarget, TrueSyncManager.DeltaTime * turnspeed);
        if(TSQuaternion.Angle(outputrot,lastquaternion) < (TrueSyncManager.DeltaTime * turnspeed) / 2) transformts.rotation = outputrot;
        lastquaternion = outputrot;
    }

    /// <summary>
    /// is the target infront of ship.
    /// </summary>
    /// <param name="targetpos">the target to check with</param>
    /// <returns></returns>
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

   /// <summary>
   /// Move the ship forwards by amount speedin
   /// </summary>
   /// <param name="speedin">the speed at which to move the ship forwards.</param>
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

    
   
    /// <summary>
    /// start speeding up, for smoooth movement.
    /// </summary>
    void Accelerate ()  { accelerating = true;    }

    /// <summary>
    ///  get closest ship to ensure nothing collides.
    /// </summary>
    /// <returns>Returns the TSTransform class attached to the closest ship</returns>
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
