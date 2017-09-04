using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Forge3D;

public class ProjectileMovementCom : MonoBehaviour {
    public GameObject targetgam;
    public GameObject Explosion;
    public Vector3 startpos;
    public Vector3 origin;
    public float dist1;
    public float dist2;
    public int damage;
    public float turnspeed = 90;
    public float timepassed;
    public bool ishit;
    public bool ismissile;
    public bool islazer;
    public GameObject start;
    public GameObject end;
    public LineRenderer ren;
    private GameObject World;
    private GameObject objpos ;
    public string debug;
    public int framesin;
    // Use this for initialization
    public bool started;
    public float speed = 16;
    public Transform impact;
    public Material missilematerail;
	public void Start () {
        if (GetComponent<LineRenderer>() != null && islazer == false) GetComponent<LineRenderer>().material = missilematerail;
        
        turnspeed = 130;
        if (ismissile == true) turnspeed = 220;
        objpos = GameObject.Find("Objects");
        timepassed = 0;
        World = GameObject.Find("WorldScaleBase");
        framesin = 0;
        started = true;
        if (islazer && ren && start.gameObject != null && end.gameObject != null)
        {
            ren.SetPosition(0, SourceGun.transform.position);
            ren.SetPosition(1, end.transform.position);
            ren.startWidth = 0.000015f * World.transform.localScale.x;
            ren.endWidth = 0.000015f * World.transform.localScale.x;
            ren.transform.parent = GameObject.Find("Working").transform;
        }
        if(transform.childCount > 0)
        {
            impact = transform.GetChild(0);
        }
        if (islazer) transform.localScale = new Vector3(1, 1, 1);
        if (ren && ren.material == null) Destroythisgam();
 
    }
    public void endgam ()
    {
        started = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (timepassed > 1 && ishit) turnspeed = turnspeed * 1.1f;
     //   if (start == null && end == null) F3DPoolManager.Pools["GeneratedPool"].Despawn(this.gameObject.transform);
    //    if (origin == new Vector3(0, 0, 0)) F3DPoolManager.Pools["GeneratedPool"].Despawn(this.gameObject.transform);
        if(islazer && SourceGun ==null) F3DPoolManager.Pools["GeneratedPool"].Despawn(this.gameObject.transform);
        if (islazer) transform.localScale = new Vector3(1, 1, 1);
        if (framesin < 1) startpos = objpos.transform.InverseTransformPoint(transform.position);
        framesin++;
        if(framesin == 2 && islazer && ishit == false && targetgam != null)
        {
            mispos = targetgam.transform.localPosition + new Vector3(Random.Range(-100,100), Random.Range(-100, 100), Random.Range(-100, 100));
        }
        timepassed += Time.deltaTime;
        if (ismissile && timepassed < 2) speed = 6;
        else speed = 22;
        if (ismissile && timepassed > 2) speed = 14;
        if (guntype == _Weapon.guntype.CapitalMainGun) speed = 40;
        if (started)
        {
         if (islazer == false)
         {
                if (targetgam && targetgam.gameObject != null)
                {
                        Vector3 relativePos = targetgam.transform.position - transform.position;
                        Quaternion rotation = Quaternion.LookRotation(relativePos);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turnspeed * Time.deltaTime);
                }
                else ishit = false;

                transform.localPosition += transform.forward * (speed);
                if (ishit)
                {

                    Vector3 relativePos = targetgam.transform.position - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos);
                    if (ismissile == false) transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turnspeed * Time.deltaTime);
                    
                    if (Vector3.Distance(objpos.transform.InverseTransformPoint(transform.position), objpos.transform.InverseTransformPoint(targetgam.transform.position)) < 40 || (timepassed > 5 && ismissile == false))
                    {
                        Destroythisgam();
                    }
                }
                else
                {
                    if (targetgam && targetgam.gameObject != null)
                    {
                        if (timepassed > 7 || (targetgam && targetgam.gameObject == null) || (Vector3.Distance(startpos, objpos.transform.InverseTransformPoint(transform.position)) > (Vector3.Distance(startpos, objpos.transform.InverseTransformPoint(targetgam.transform.position))) && ismissile == false) || (ismissile == true && (Vector3.Distance(startpos, objpos.transform.InverseTransformPoint(transform.position)) > Vector3.Distance(startpos, objpos.transform.InverseTransformPoint(targetgam.transform.position)) * 1.4)))
                            Destroythisgam();
                    }
                    else if (timepassed > 3) Destroythisgam();
                }

                
                

        }
      
        if (islazer && ren && start.gameObject != null)
        {
               if(end.gameObject != null) lastendpos = end.transform.position;
                ren.SetPosition(0, SourceGun.transform.position);

                if (ishit == true && end.gameObject != null) ren.SetPosition(1, end.transform.position);
                else if (mispos != new Vector3(0, 0, 0)) ren.SetPosition(1, transform.parent.TransformPoint(mispos));
                else ren.SetPosition(1, SourceGun.transform.position);
                if (start == null && end == null)
                {
                    ren.SetPosition(0, new Vector3(0,0,0));
                    ren.SetPosition(1, new Vector3(0, 0, 0));
                }
                if (guntype == _Weapon.guntype.Lazer)
                {
                    if (impact.gameObject.GetActive() == false) impact.gameObject.SetActive(true);
                    if (ishit && end.gameObject != null) impact.transform.position = end.transform.position;
                    else impact.transform.position = transform.parent.TransformPoint(mispos);
                    impact.transform.localScale = new Vector3(30, 30f, 30f);
                   
                }
                else
                {
                    if (impact.gameObject.GetActive() == true) impact.gameObject.SetActive(false);
                }
                    ren.startWidth = 0.000015f * World.transform.localScale.x * linerenmodifier;
            ren.endWidth = 0.000015f * World.transform.localScale.x * linerenmodifier;
          //  ren.transform.parent = GameObject.Find("Working").transform;

        }
       if (islazer && ( timepassed > 3 || start.gameObject == null || start == null || end == null)  )
       {
         
           F3DPoolManager.Pools["GeneratedPool"].Despawn(this.gameObject.transform);
       }

        }
        if (timepassed > 15)
        {
         
            F3DPoolManager.Pools["GeneratedPool"].Despawn(this.gameObject.transform);
        }

    }
    private Vector3 lastendpos;
    void Destroythisgam ()
    {
        if (islazer) mispos = new Vector3(0, 0, 0);
        if (Explosion && Explosion.transform)
        {
            Transform detonationtemp = F3DPoolManager.Pools["GeneratedPool"].Spawn(Explosion.transform);
            if (detonationtemp)
            {
                GameObject detonation = detonationtemp.gameObject;
                detonation.transform.parent = transform.parent;
                detonation.transform.position = transform.position;
                detonation.transform.localScale = new Vector3(10f, 10f, 10f);

            }
           
        }
     
        F3DPoolManager.Pools["GeneratedPool"].Despawn(this.gameObject.transform);
    }
    public float linerenmodifier;
    public _Weapon.guntype guntype;
    public Vector3 mispos;
    public void startlineren (GameObject startin, GameObject endin,Material lazermat,float linemod,_Weapon.guntype inputgun, GameObject Sourcegunin)
    {
        islazer = true;
        start = startin;
        end = endin;
        ren = gameObject.GetComponent<LineRenderer>();
        ren.enabled = true;
        if (lazermat) ren.material = lazermat;
        linerenmodifier = linemod;
        guntype = inputgun;
        SourceGun = Sourcegunin;
      

    }
    public GameObject SourceGun;
    public void isMissile()
    {
        ismissile = true;
    }
}
