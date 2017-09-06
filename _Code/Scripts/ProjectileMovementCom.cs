using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Forge3D;

/// <summary>
/// this class handels the cosmetic movement of a projectile, it is not deterministic.
/// </summary>
public class ProjectileMovementCom : MonoBehaviour {

    // The Projectiles Target.
    public GameObject targetgam;
    // The prefab explosion to spawn.
    public GameObject Explosion;
    // the point at which the projectile was spawned.
    public Vector3 startpos;
    // the ceneter of the deterministic simulation origin.
    public Vector3 origin;
    // the time since the projectile was fired.
    public float timepassed;
    // is the projectile a hit or a miss?
    public bool ishit;
    // is the projectile a missile.
    public bool ismissile;
    // is the projectile a lazer?
    public bool islazer;
    // the origin Gameobject.
    public GameObject start;
    // the Target Gameobject.
    public GameObject end;
    // the LineRender trailing behind the projectile.
    public LineRenderer ren;
    // The world root object.
    private GameObject World;
    // the number of frames since the object spawned.
    public int framesin;
    // is the object fired?
    public bool started;
    // the speed of the projectile.
    public float speed = 16;
    // the impact effect.
    public Transform impact;
    // the material for the linerender.
    public Material missilematerail;
    // Last position of the projectile.
    private Vector3 lastendpos;
    // the size of the linerenderer.
    public float linerenmodifier;
    // the origin guntype.
    public _Weapon.guntype guntype;
    // the position the projectile is aiming at if it is a miss.
    public Vector3 mispos;

    /// <summary>
    /// reset everything up after coming out of the pool.
    /// </summary>
	public void Start()
    {
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
        if (transform.childCount > 0) impact = transform.GetChild(0);
        if (islazer) transform.localScale = new Vector3(1, 1, 1);
        if (ren && ren.material == null) Destroythisgam();
    }

    /// <summary>
    ///  the object is deactivated.
    /// </summary>
    public void endgam ()
    {
        started = false;
    }

    /// <summary>
    /// Update to update the stats of the object whilst in flight and destroy if required.
    /// </summary>
    void Update()
    {
        if (timepassed > 1 && ishit) turnspeed = turnspeed * 1.1f;
        if (islazer && SourceGun == null) F3DPoolManager.Pools["GeneratedPool"].Despawn(this.gameObject.transform);
        if (islazer) transform.localScale = new Vector3(1, 1, 1);
        if (framesin < 1) startpos = objpos.transform.InverseTransformPoint(transform.position);
        framesin++;
        if (framesin == 2 && islazer && ishit == false && targetgam != null)
        {
            mispos = targetgam.transform.localPosition + new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
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
                if (end.gameObject != null) lastendpos = end.transform.position;
                ren.SetPosition(0, SourceGun.transform.position);

                if (ishit == true && end.gameObject != null) ren.SetPosition(1, end.transform.position);
                else if (mispos != new Vector3(0, 0, 0)) ren.SetPosition(1, transform.parent.TransformPoint(mispos));
                else ren.SetPosition(1, SourceGun.transform.position);
                if (start == null && end == null)
                {
                    ren.SetPosition(0, new Vector3(0, 0, 0));
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
            }
            if (islazer && (timepassed > 3 || start.gameObject == null || start == null || end == null))
            {

                F3DPoolManager.Pools["GeneratedPool"].Despawn(this.gameObject.transform);
            }

        }
        if (timepassed > 15)
        {

            F3DPoolManager.Pools["GeneratedPool"].Despawn(this.gameObject.transform);
        }

    }

    /// <summary>
    /// Destroy the projectile and return it to a pool.
    /// </summary>
    void Destroythisgam()
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

    /// <summary>
    /// Create a linerender to start with
    /// </summary>
    /// <param name="startin">the start Gameobject of the line</param>
    /// <param name="endin">the end Gameobject of the line</param>
    /// <param name="lazermat">The lines material</param>
    /// <param name="linemod">the damage modifier of the line.</param>
    /// <param name="inputgun">The guntype that fired it.</param>
    /// <param name="Sourcegunin">The gun object that fired it.</param>
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
    
    /// <summary>
    /// Set this object to be a missile.
    /// </summary>
    public void isMissile()
    {
        ismissile = true;
    }
}
