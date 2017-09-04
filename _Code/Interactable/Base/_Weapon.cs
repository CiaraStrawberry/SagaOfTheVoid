using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using Forge3D;

public class _Weapon : TrueSyncBehaviour
{
    private UnitMovementcommandcontroller unittargetcontrol;
    private _Ship parentscript;
    public _Ship.eShipColor team;
    public GameObject verticalmovechild;
    public Material shieldmatgam;
    public GameObject horrizontalmovething;
  //  private TSTransform transformTS;
    public FP chanceout;
    private F3DPoolManager poolmanager;
    public AudioClip FireSound;
    public AudioSource audiosource;
    public GameObject WorldBase;
    public GameObject lastfired;
    private int startcount;
    public FP debughitchance;
    public List<FiredContainer> timepassedonships = new List<FiredContainer>();
    public FP burstfirewait;
    public int shotsperburst;
    public int damage;
    public GameObject MuzzleFlash;
    public GameObject MuzzlePos;
    private int indexWeapon;
    private int turnspeed = 50;
    private FP timepassing;
    public FP timebetweenshots = 1;
    public GameObject Lazer_Proectile;
    [SerializeField]
    private FP m_Accuracy;
    public FP Accuracy { get {return m_Accuracy; } set {m_Accuracy = value; } }
    private TSRandom customran;
    private bool started;
    // type of weapons, each changes behavior depending on what you set it to.
    public enum guntype {
        Cannon,
        Missile,
        MiniGun,
        ShieldGenerator,
        Tracker,
        Cloak,
        HPUpgrade,
        EngineUpgrade,
        WeaponUpgrade,
        FighterGun,
        BomberGun,
        Dual_Role,
        CapitalMainGun,
        Lazer
    }
    public FP timetilnextshot;
    public int shotsfiredinthisburst;
    public FP timepassed;
    public int debugfiredcontainercount;
    public float debugwaittime;
    public int runnext;
    public int timepassedforrun;
    public int a;
    public string teststring;


    [SerializeField]
    private GameObject m_Cannon_Projectile;
    public GameObject Cannon_Projectile    {    get { return m_Cannon_Projectile; } set { m_Cannon_Projectile = value; } }

    [SerializeField]
    private GameObject m_Missile_Projectile;
    public GameObject Missile_Projectile { get { return m_Missile_Projectile; } set { m_Missile_Projectile = value; } }


    [SerializeField]
    private GameObject m_MiniGun_Projectile;
    public GameObject MiniGun_Projectile { get { return m_MiniGun_Projectile; } set { m_MiniGun_Projectile = value; } }



    [SerializeField]
    private guntype m_guntypemain;
    public guntype guntypemain { get { return m_guntypemain; } set { m_guntypemain = value; } }

    /// <summary>
    /// Type of weapon, will be used to determine how the projectiles work
    /// </summary>
    public eWeaponType WeaponType { get; set; }

    /// <summary>
    /// Minimum hull required for weapon
    /// </summary>
    public eHullType MinimumHull { get; set; }

    /// <summary>
    /// Classes allowed by weapon
    /// </summary>
    public eShipClass SupportedShipClass { get; set; }

    /// <summary>
    /// How many rounds in a magazine
    /// </summary>
    public int RoundInMagazine { get; set; }

    /// <summary>
    /// Rounds per salvo (3 round burst, etc)
    /// </summary>
    public int RoundsPerSalvo { get; set; }

    /// <summary>
    /// Time to reload magazine
    /// </summary>
    public System.TimeSpan ReloadTime { get; set; }

    /// <summary>
    /// Time it takes to fire the next salvo
    /// </summary>
    public FP TimeBetweenSalvos { get; set; }

    /// <summary>
    /// How much damage each round will do at optimal range
    /// </summary>
    public int DamagePerRound { get; set; }

    /// <summary>
    /// Tracking speed in radians
    /// </summary>
    public int TrackingSpeed { get; set; }

    /// <summary>
    /// Range of the round in meters
    /// </summary>
    [SerializeField]
    private int m_Range;
    public int Range { get { return m_Range; } set { m_Range = value; } }

    /// <summary>
    /// Optimal range of round in meters
    /// </summary>ange
    [SerializeField]
    private int m_OptimalRange;
    public int OptimalRange { get {return m_OptimalRange; } set {m_OptimalRange = value; } }

    /// <summary>
    /// After optimal, damage decreases each meter by this %
    /// </summary>
    [SerializeField]
    private double m_DecayRate;
    public double DecayRate { get {return m_DecayRate ; } set { m_DecayRate = value; } }

    /// <summary>
    // deterministic start.
    /// <summary>
    public void StartMain(int inputint)
    {
        started = true;
        i = inputint;
        startcount = 0;
        customran = TSRandom.New(transform.parent.GetComponent<PhotonView>().viewID + inputint);
        parentscript = transform.parent.GetComponent<_Ship>();
        if (guntypemain == guntype.EngineUpgrade)  parentscript.MaxSpeed = parentscript.MaxSpeed * 1.15f;
        if (guntypemain == guntype.HPUpgrade) parentscript.Armor = Mathf.RoundToInt( parentscript.Armor * 1.15f);
        if (guntypemain == guntype.WeaponUpgrade)
        {
            foreach(Transform gun in transform.parent)
            {
                if (gun.tag == "Pickup")
                {
                    _Weapon gunwep = gun.GetComponent<_Weapon>();
                    gunwep.timebetweenshots = gunwep.timebetweenshots * 1.15;
                }
               
            }
        }
        if (guntypemain == guntype.Cloak) parentscript.iscloacked = true;
        if (guntypemain == guntype.Tracker) parentscript.isTracker = true;
        unittargetcontrol = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
        poolmanager = GameObject.Find("F3dPoolManager").GetComponent<F3DPoolManager>();
        team = parentscript.ShipColor;
        if (verticalmovechild == null && transform.childCount > 0 && transform.GetChild(0).childCount > 0) verticalmovechild = transform.GetChild(0).GetChild(0).gameObject;
        _Ship parent = transform.parent.GetComponent<_Ship>();
        if (Cannon_Projectile != null)   parent.Cannon_Projectile = Cannon_Projectile;
        if (Missile_Projectile != null) parent.Missile_Projectile = Missile_Projectile;
        if (MiniGun_Projectile != null) parent.MiniGun_Projectile = MiniGun_Projectile;
        if (Lazer_Proectile != null) parent.Lazer_Shot = Lazer_Proectile;
        parent.shieldmaterial = shieldmatgam;
        damage = Calculate_damage();
        audiosource = GetComponent<AudioSource>();
        WorldBase = GameObject.Find("WorldScaleBase");
        timepassed = 0;
    }


    /// <summary>
    /// Damage Per Shot
    /// </summary>
    public int Calculate_damage ()
    {
        int output = 0;
        if (guntypemain == guntype.Cannon) output = 800;
        if (guntypemain == guntype.Cannon && parentscript.HullType == eHullType.Capital) output = 275;
        if (guntypemain == guntype.Missile) output = 300;
        if (guntypemain == guntype.MiniGun) output = 20;
        if (guntypemain == guntype.ShieldGenerator) output = 300;
        if (guntypemain == guntype.FighterGun) output = 20;
        if (guntypemain == guntype.BomberGun) output = 200;
        if (guntypemain == guntype.Dual_Role) output = 50;
        if (guntypemain == guntype.CapitalMainGun) output = 2000;
        if (guntypemain == guntype.Lazer) output = 3000;
        return output;
    }


    /// <summary>
    /// Send fire weapon call to _ship. this doesnt just send the weapontype enum since some weaponry i will want to send a different weapon type. 
    /// </summary>
    void fireweapon (Vector3 targetpos)
    {
        if (MuzzlePos == null) MuzzlePos = gameObject;
        bool ishittest = ishit(parentscript.WeaponTarget.shipscript);
        GameObject targetviewid = parentscript.WeaponTarget.gameObject ;
         if (guntypemain == guntype.Cannon) lastfired = parentscript.FireWeaponNetwork( targetpos,ishittest,getshotstartpos(),targetviewid,damage, getshotstartposlocal(), guntype.Cannon, MuzzlePos,guntypemain);
         if(guntypemain == guntype.Missile) lastfired = parentscript.FireWeaponNetwork(targetpos, ishittest, getshotstartpos(), targetviewid, damage, getshotstartposlocal(), guntype.Missile, MuzzlePos, guntypemain);
         if (guntypemain == guntype.MiniGun) lastfired = parentscript.FireWeaponNetwork(targetpos, ishittest, getshotstartpos(), targetviewid, damage, getshotstartposlocal(), guntype.MiniGun, MuzzlePos, guntypemain);
         if(guntypemain == guntype.ShieldGenerator) lastfired = parentscript.FireWeaponNetwork(targetpos, true, getshotstartpos(), targetviewid, damage, getshotstartposlocal(), guntype.ShieldGenerator, MuzzlePos, guntypemain);
        if (guntypemain == guntype.FighterGun) lastfired = parentscript.FireWeaponNetwork(targetpos, ishittest, getshotstartpos(), targetviewid, damage, getshotstartposlocal(), guntype.MiniGun, MuzzlePos, guntypemain);
        if (guntypemain == guntype.BomberGun) lastfired = parentscript.FireWeaponNetwork(targetpos, ishittest, getshotstartpos(), targetviewid, damage, getshotstartposlocal(), guntype.BomberGun, MuzzlePos, guntypemain);
        if (guntypemain == guntype.Dual_Role) lastfired = parentscript.FireWeaponNetwork(targetpos, ishittest, getshotstartpos(), targetviewid, damage, getshotstartposlocal(), guntype.MiniGun, MuzzlePos, guntypemain);
        if (guntypemain == guntype.CapitalMainGun) lastfired = parentscript.FireWeaponNetwork(targetpos, ishittest, getshotstartpos(), targetviewid, damage, getshotstartposlocal(), guntype.Cannon, MuzzlePos, guntypemain);
        if (guntypemain == guntype.Lazer) lastfired = parentscript.FireWeaponNetwork(targetpos, ishittest, getshotstartpos(), targetviewid, damage, getshotstartposlocal(), guntype.Lazer, MuzzlePos, guntypemain);
        if (audiosource && FireSound && this.gameObject.GetActive() == true) { audiosource.clip = FireSound; audiosource.PlayOneShot(FireSound,audiosource.volume); }
        if (ishittest == true && guntypemain != guntype.ShieldGenerator) timepassedonships.Add(new FiredContainer(hittime(parentscript.fac.transformts.position, parentscript.WeaponTarget.position,(22 * 20)), damage, parentscript.WeaponTarget.gameObject));
        if (MuzzleFlash) {
            Transform flash= F3DPoolManager.Pools["GeneratedPool"].Spawn(MuzzleFlash.transform);
            if (flash)
            {
                flash.transform.parent = WorldBase.transform.Find("World").Find("Objects").Find("Working");
                flash.transform.localScale = new Vector3(100, 100, 100);
                flash.transform.position = transform.position;
            }
        
        }  
    }

    /// <summary>
    ///Check if the Vector3 target is infront of start
    /// </summary>
    public static bool AngleDir(Transform start, Vector3 target) {
        var relativePoint = start.InverseTransformPoint(target);
        if (relativePoint.z < 0.0)
            return false;
        else if (relativePoint.z > 0.0)
            return true;
        else
            return true;
    }
    /// <summary>
    // gets the barrel of the muzzle or infront enough it that the projectile will not collide with it.
    /// <summary>
    Vector3 getshotstartpos ()
    {
        if(parentscript.HullType == eHullType.Light || parentscript.HullType == eHullType.Corvette)
        {
            Vector3 output = transform.position;
            output = parentscript.meshes[Mathf.RoundToInt( Random.Range(0,parentscript.meshes.Count - 1))].transform.position;
            return output;
        }
        else    return transform.position;
    }

    /// <summary>
    // gets the barrel of the muzzle or infront enoughof it that the projectile will not collide with it in local space.
    /// <summary>
    Vector3 getshotstartposlocal()
    {
        if (parentscript.HullType == eHullType.Light || parentscript.HullType == eHullType.Corvette)
        {
            Vector3 output = transform.localPosition;
            output = parentscript.meshes[Mathf.RoundToInt(Random.Range(0, parentscript.meshes.Count - 1))].transform.localPosition;
            return output;
        }
        else return transform.localPosition;
    }

    /// <summary>
    // travel time of the projectile for the simulation
    /// <summary>
    public FP hittime (TSVector start,TSVector end, FP speed)
    {
        FP a = TSVector.Distance(start, end) / speed ;
        if (guntypemain != guntype.Lazer) return a + 0.5;
        else return 3;
    }

 
    /// <summary>
    // determins if the shot hit the target.
    /// <summary>
    bool ishit (_Ship inputship)
    {
        bool output = false;
        if (guntypemain == guntype.Missile && inputship.HullType != eHullType.Light) output = true;
        else {    
             
            FP chance = TSMath.Round(100 * getmodifyer(inputship.HullType));
            chanceout = chance;
            if (parentscript.isTrackingimproved) chance = chance * 2;
            FP randomnum = customran.Next(0,100);
            
            if (chance > randomnum) output = true;
            else output = false;
            teststring = chance.ToString() + " is more? than " + randomnum.ToString() + " so it is a hit: " + output;
        }
        return output;

    }

    /// <summary>
    //gets the probability of a hit based on gun type and target  size.
    /// <summary>
    public FP getmodifyer (eHullType input)
    {
        FP output = 1;
      
          if(guntypemain == guntype.Cannon || guntypemain ==  guntype.CapitalMainGun)
           {
              if (input == eHullType.Light) output = 0.02;
              if (input == eHullType.Corvette) output = 0.05;
              if (input == eHullType.Frigate) output = 0.8;
              if (input == eHullType.Cruiser) output = 0.8;
              if (input == eHullType.Destroyer) output = 0.8;
              if (input == eHullType.Capital) output = 1;
          }
          if (guntypemain == guntype.MiniGun)
          {
            if (input == eHullType.Light) output = 0.1;
            if (input == eHullType.Corvette) output = 0.1;
            if (input == eHullType.Frigate) output = 0.8;
            if (input == eHullType.Cruiser) output = 0.8;
            if (input == eHullType.Destroyer) output = 0.8;
            if (input == eHullType.Capital) output = 1;
          }
          if (guntypemain == guntype.Missile)
          {
            if (input == eHullType.Light) output = 0.05;
            if (input == eHullType.Corvette) output = 0.5;
            if (input == eHullType.Frigate) output = 1;
            if (input == eHullType.Cruiser) output = 1;
            if (input == eHullType.Destroyer) output = 1;
            if (input == eHullType.Capital) output = 1;
          }
          if(guntypemain == guntype.FighterGun)
          {
            if (input == eHullType.Light) output = 0.1;
            if (input == eHullType.Corvette) output = 0.5;
            if (input == eHullType.Frigate) output = 1;
            if (input == eHullType.Cruiser) output = 1;
            if (input == eHullType.Destroyer) output = 1;
            if (input == eHullType.Capital) output = 1;
        }
        if (guntypemain == guntype.BomberGun)
        {
            if (input == eHullType.Light) output = 0.05;
            if (input == eHullType.Corvette) output = 0.5;
            if (input == eHullType.Frigate) output = 1;
            if (input == eHullType.Cruiser) output = 1;
            if (input == eHullType.Destroyer) output = 1;
            if (input == eHullType.Capital) output = 1;
        }
        if (guntypemain == guntype.Dual_Role)
        {
            if (input == eHullType.Light) output = 0.01;
            if (input == eHullType.Corvette) output = 0.5;
            if (input == eHullType.Frigate) output = 0.5;
            if (input == eHullType.Cruiser) output = 0.5;
            if (input == eHullType.Destroyer) output = 1;
            if (input == eHullType.Capital) output = 1;
        }
        if (guntypemain == guntype.Lazer)
        {
            if (input == eHullType.Light) output = 0.01;
            if (input == eHullType.Corvette) output = 0.02;
            if (input == eHullType.Frigate) output = 0.6;
            if (input == eHullType.Cruiser) output = 0.9;
            if (input == eHullType.Destroyer) output = 0.9;
            if (input == eHullType.Capital) output = 0.9;
        }
        if (guntypemain == guntype.ShieldGenerator) output = 1;
        debughitchance = output;
        return output;
    }

 
    /// <summary>
    // deterministic update
    /// <summary>
    public void UpdateSpec()
    {
        debugfiredcontainercount = timepassedonships.Count;
        List<FiredContainer> shipstoremove = new List<FiredContainer>();
        foreach (FiredContainer fired in timepassedonships)
        {
            fired.timestart += TrueSyncManager.DeltaTime;
            if (fired.timestart >= fired.timesince)
            {
                if(fired.Target.gameObject != null) fired.Target.GetComponent<_Ship>().takeDamage(damage,parentscript.ShipColor,parentscript.HullType);
                shipstoremove.Add(fired);
            } 
        }
        foreach(FiredContainer fired in shipstoremove)
        {
            timepassedonships.Remove(fired);
        }
        if (startcount == 0) team = parentscript.ShipColor;
        startcount++;
        
        if (this != null && TrueSyncManager.Time > 0)
        {
          if (verticalmovechild == null && transform.childCount > 0  && transform.GetChild(0) && transform.GetChild(0).childCount > 0 &&  transform.GetChild(0).GetChild(0)) verticalmovechild = transform.GetChild(0).GetChild(0).gameObject;
          if (parentscript.WeaponTarget && parentscript.WeaponTarget.gameObject == null) parentscript.WeaponTarget = null;
          if (parentscript.WeaponTarget && guntypemain != guntype.CapitalMainGun) look(parentscript.WeaponTarget.transform.position);
          
        //  if (parentscript.WeaponTarget == null) timepassing = 0;
          timepassing += TrueSyncManager.DeltaTime;
          if (timepassing >= (timetilnextshot) && parentscript.WeaponTarget)
          {
              timepassing = 0;
                if (guntypemain != guntype.ShieldGenerator) fireweaponchecknormal();
                else fireweaponcheckshield();
            
              
                shotsfiredinthisburst++;

                if (shotsfiredinthisburst == shotsperburst)
                {
                    timetilnextshot = burstfirewait;
                    shotsfiredinthisburst = 0;
                }
                else
                { 
                    timetilnextshot = timebetweenshots;
                }
                
          }
         }
        if(audiosource)
        {
            audiosource.minDistance = 0.003f * WorldBase.transform.localScale.x;
            audiosource.maxDistance = 0.02f * WorldBase.transform.localScale.x;
        }
   
    }
    public GameObject test;
    int i;
    /// <summary>
    // checks if target is in range and desired team.
    /// <summary>
    void fireweaponchecknormal()
    {
        if (TSVector.Distance(parentscript.WeaponTarget.GetComponent<TSTransform>().position, parentscript.fac.transformts.position) < Range && UnitMovementcommandcontroller.getteam(parentscript.WeaponTarget.shipscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode) != UnitMovementcommandcontroller.getteam(parentscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode)) fireweapon(parentscript.WeaponTarget.transform.position);
        else parentscript.WeaponTarget = null;
    }

    /// <summary>
    // checks if target is in range and the same team.
    /// <summary>
    void fireweaponcheckshield ()
    {
        if (TSVector.Distance(parentscript.WeaponTarget.GetComponent<TSTransform>().position, parentscript.fac.transformts.position) < Range && UnitMovementcommandcontroller.getteam(parentscript.WeaponTarget.shipscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode) == UnitMovementcommandcontroller.getteam(parentscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode)) fireweapon(parentscript.WeaponTarget.transform.position);
        else parentscript.WeaponTarget = null;
    }

    /// <summary>
    //move mesh to point at target.
    /// <summary>
    public void look (Vector3 target)
    {
        if (target != new Vector3(0, 0, 0))
        {
            Quaternion directiontarget = Quaternion.LookRotation(target - transform.position);
            Quaternion quo = Quaternion.Lerp(transform.rotation, directiontarget, Time.deltaTime * turnspeed);
            if (horrizontalmovething)
            {
                horrizontalmovething.transform.rotation = quo;
                horrizontalmovething.transform.localEulerAngles = new Vector3(0, horrizontalmovething.transform.localEulerAngles.y, 0);
            }
            if (verticalmovechild)
            {
                verticalmovechild.transform.rotation = quo;
                verticalmovechild.transform.localEulerAngles = new Vector3(verticalmovechild.transform.localEulerAngles.x, 0, 0);
            }

        }
    }

    /// <summary>
    // class to hold pending shots
    /// <summary>
    public class FiredContainer
    {
        public FP timestart= 0;
        public FP timesince;
        public FP damage;
        public GameObject Target;
        public FiredContainer (FP timesincein,FP damagein,GameObject Targetin)
        {
            timestart = timesincein;
            damage = damagein;
            Target = Targetin;
        }
    }

}
