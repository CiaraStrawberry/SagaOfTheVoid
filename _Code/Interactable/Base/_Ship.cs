using UnityEngine;
using System.Collections.Generic;
using EasyEditor;
using UnityEngine.UI;
using Apex.Steering;
using Apex.Units;
using Apex.Steering.Components;
using Forge3D;
using TrueSync;
using UnityEngine.SceneManagement;
using VRTK.Highlighters;

public class _Ship : TrueSyncBehaviour
{
    [Inspector(group = "StartupandDebugstuff")]
    [SerializeField]
    private GameObject HealthBar;
    public GameObject HealthBarinit;
    public Image healthbarinterface;
    public Transform startmovepos;
    private float _ActualMaxSpeed;
    [SerializeField]
    private TSVector MoveTarget;
    PhotonTransformView transformview;
    public bool isbot;
    public int spawnnum;
    public bool iscloacked;
    public bool isTracker;
    public List<Vector3> targetmesh = new List<Vector3>();
    public List<Vector3> startmeshpos = new List<Vector3>();
    private F3DPoolManager poolmanager;
    public TSTransform shiptarget;
    public List<_Weapon> weaponry = new List<_Weapon>();
    public GameObject objectsholder;
    public GameObject Healthbaronly;
    public Image Healthbaronlyinterface;
    public Sprite RedHealthBar;
    public Sprite GreenHealthBar;
    public Sprite YellowSprite;
    public Sprite WhiteSprite;
    public Sprite GreySprite;
    public Sprite BlueSprite;
    private Vector3 healthbarstartscale;
    public VRTK_OutlineObjectCopyHighlighter outline;
    public int startshield;
    private TSVector shiptargetposlast;
    private TSTransform shiptargettstransform;

    #region Settings
    /// <summary>
    /// Ship color
    /// </summary>
    public enum eShipColor
    {
        Green,
        Blue,
        White,
        Yellow,
        Grey,
        Red
        
    }

    [Inspector(displayHeader = true, group = "Engine Settings", groupDescription = "Settings for the engine", order = 1)]
    [SerializeField]
    private GameObject _ShipObject;
    /// <summary>
    /// Ship object that the model rendering takes place
    /// </summary>
    public GameObject ShipObject
    {
        get { return _ShipObject; }
        set { _ShipObject = value; }
    }

    [Inspector(group = "Engine Settings")]
    [SerializeField]
    private GameObject _ShieldObject;
    /// <summary>
    /// Shiled object that the shield model rendering takes place
    /// </summary>
    public GameObject ShieldObject
    {
        get { return _ShieldObject; }
        set { _ShieldObject = value; }
    }

    [Inspector(group = "Engine Settings")]
    [SerializeField]
    private GameObject _WeaponsObject;
    /// <summary>
    /// Weapons object that contains all weapons placements
    /// </summary>
    public GameObject WeaponsObject
    {
        get { return _WeaponsObject; }
        set { _WeaponsObject = value; }
    }

    [Inspector(group = "Engine Settings")]
    [SerializeField]
    private GameObject _EquipmentObject;
    /// <summary>
    /// Equipment object that contains all equipment placements
    /// </summary>
    public GameObject EquipmentObject
    {
        get { return _EquipmentObject; }
        set { _EquipmentObject = value; }
    }

    [Inspector(group = "Engine Settings")]
    [SerializeField]
    private GameObject _EnginesObject;
    /// <summary>
    /// Engines object that contains all engine placements
    /// </summary>
    public GameObject EnginesObject
    {
        get { return _EnginesObject; }
        set { _EnginesObject = value; }
    }

    [Inspector(group = "Engine Settings")]
    [SerializeField]
    private Canvas _UIObject;
    /// <summary>
    /// UI object for dynamic addition of game assets
    /// </summary>
    public Canvas UIObject
    {
        get { return _UIObject; }
        set { _UIObject = value; }
    }

    [Inspector(displayHeader = true, group = "Hull Settings", groupDescription = "Basic settings for the hull, these don't need to change", order = 2)]
    [SerializeField]
    private eHullType _HullType;
    /// <summary>
    /// Hull type for the ship
    /// </summary>
    public eHullType HullType
    {
        get { return _HullType; }
        set
        {
            _HullType = value;
            AdjustStats();
        }
    }

    [Inspector(group = "Hull Settings")]
    [SerializeField]
    private eShipClass _ShipClass;
    /// <summary>
    /// Class of the ship
    /// </summary>
    public eShipClass ShipClass
    {
        get { return _ShipClass; }
        set
        {
            _ShipClass = value;
            AdjustStats();
        }
    }

    [Inspector(group = "Hull Settings")]
    [SerializeField]
    private int _PointsToDeploy;
    /// <summary>
    /// Points to deploy this class
    /// </summary>
    public int PointsToDeploy
    {
        get { return _PointsToDeploy; }
    }

    [Inspector(displayHeader = true, group = "Basic Stats", groupDescription = "Stats regarding hit points and maneuverability", order = 3)]
    [SerializeField]
    private int _Armor;
    /// <summary>
    /// Armor points for the ship
    /// </summary>
    public int Armor
    {
        get { return _Armor; }
        set { _Armor = value; }
    }

    [Inspector(group = "Basic Stats")]
    [SerializeField]
    private int _Shield;
    /// <summary>
    /// Shield points for the ship
    /// </summary>
    public int Shield
    {
        get { return _Shield; }
        set { _Shield = value; }
    }

    [Inspector(group = "Basic Stats")]
    [Tooltip("The maximum speed, regardless of movement form.")]
    [SerializeField]
    private FP _MaxSpeed;
    /// <summary>
    /// The maximum speed, regardless of movement form.
    /// </summary>
    public FP MaxSpeed
    {
        get { return _MaxSpeed; }
        set { _MaxSpeed = value; }
    }

    [Inspector(group = "Basic Stats")]
    [Tooltip("The maximum angular acceleration rate of the unit (rads / s^2), i.e. how fast can the unit reach its desired turn speed.")]
    [SerializeField]
    private int _MaxAngularAcceleration;
    /// <summary>
    /// The maximum angular acceleration rate of the unit (rads / s^2), i.e. how fast can the unit reach its desired turn speed.
    /// </summary>
    public int MaxAngularAcceleration
    {
        get { return _MaxAngularAcceleration; }
        set { _MaxAngularAcceleration = value; }
    }

    [Inspector(group = "Basic Stats")]
    [Tooltip("The maximum turn speed of the unit (rads / s), i.e. how fast can the unit turn.")]
    [SerializeField]
    private FP _MaxAngularSpeed;
    
    /// <summary>
    /// The maximum turn speed of the unit (rads / s), i.e. how fast can the unit turn.
    /// </summary>
    public FP MaxAngularSpeed
    {
        get { return _MaxAngularSpeed; }
        set { _MaxAngularSpeed = value; }
    }

    [Inspector(group = "Basic Stats")]
    [Tooltip("The maximum acceleration rate of the unit (m / s^2), i.e. how fast can the unit reach its desired speed.")]
    [SerializeField]
    private int _MaxAcceleration;
    /// <summary>
    /// The maximum acceleration rate of the unit (m / s^2), i.e. how fast can the unit reach its desired speed.
    /// </summary>
    public int MaxAcceleration
    {
        get { return _MaxAcceleration; }
        set { _MaxAcceleration = value; }
    }

    [Inspector(group = "Basic Stats")]
    [Tooltip("The maximum deceleration rate of the unit (m / s^2), i.e. how fast can the unit slow down.")]
    [SerializeField]
    private int _MaxDeceleration;
    /// <summary>
    /// The maximum deceleration rate of the unit (m / s^2), i.e. how fast can the unit slow down.
    /// </summary>
    public int MaxDeceleration
    {
        get { return _MaxDeceleration; }
        set { _MaxDeceleration = value; }
    }

    [Inspector(group = "Basic Stats")]
    [Tooltip("The minimum speed ever, regardless of movement form. Any speed below this will mean a stop.")]
    [SerializeField]
    private int _MinimumSpeed;
    /// <summary>
    /// The minimum speed ever, regardless of movement form. Any speed below this will mean a stop.
    /// </summary>
    public int MinimumSpeed
    {
        get { return _MinimumSpeed; }
        set { _MinimumSpeed = value; }
    }

    [Inspector(group = "Basic Stats")]
    [SerializeField]
    private _Weapon[] _Weapons;
    /// <summary>
    /// Weapons assigned to the ship
    /// </summary>
    public _Weapon[] Weapons
    {
        get { return _Weapons; }
        set { _Weapons = value; }
    }

    [Inspector(displayHeader = true, group = "Model Settings", groupDescription = "Model settings", order = 4)]
    [SerializeField]
    private eShipColor _ShipColor = eShipColor.Blue;
    /// <summary>
    /// Ship color 
    /// </summary>
    public virtual eShipColor ShipColor
    {
        get { return _ShipColor; }
        set
        {
            _ShipColor = value;
            foreach (Transform t in transform)
            {
                if (t.name == "Ship")
                {
                    MeshRenderer mr = t.GetComponent<MeshRenderer>();
                    if (ColorBlack != null)
                    {
                        switch (_ShipColor)
                        {           
                            case eShipColor.Blue:
                                mr.material = ColorBlue;
                                break;
                            case eShipColor.Green:
                                mr.material = ColorGreen;
                                break;
                            case eShipColor.Grey:
                                mr.material = ColorGrey;
                                break;
                            case eShipColor.Red:
                                mr.material = ColorRed;
                                break;
                            case eShipColor.White:
                                mr.material = ColorWhite;
                                break;
                            case eShipColor.Yellow:
                                mr.material = ColorYellow;
                                break;

                        }
                    }
                }
            }

            // Apply the color
        }


    }

    [Inspector(group = "Model Settings")]
    [SerializeField]
    private Material _Black;
    /// <summary>
    /// Black color material
    /// </summary>
    public Material ColorBlack
    {
        get { return _Black; }
        set { _Black = value; }
    }

    [Inspector(group = "Model Settings")]
    [SerializeField]
    private Material _Blue;
    /// <summary>
    /// Blue color material
    /// </summary>
    public Material ColorBlue
    {
        get { return _Blue; }
        set { _Blue = value; }
    }

    [Inspector(group = "Model Settings")]
    [SerializeField]
    private Material _Green;
    /// <summary>
    /// Green color material
    /// </summary>
    public Material ColorGreen
    {
        get { return _Green; }
        set { _Green = value; }
    }

    [Inspector(group = "Model Settings")]
    [SerializeField]
    private Material _Grey;
    /// <summary>
    /// Grey color material
    /// </summary>
    public Material ColorGrey
    {
        get { return _Grey; }
        set { _Grey = value; }
    }

    [Inspector(group = "Model Settings")]
    [SerializeField]
    private Material _Red;
    /// <summary>
    /// Red color material
    /// </summary>
    public Material ColorRed
    {
        get { return _Red; }
        set { _Red = value; }
    }

    [Inspector(group = "Model Settings")]
    [SerializeField]
    private Material _White;
    /// <summary>
    /// White color material
    /// </summary>
    public Material ColorWhite
    {
        get { return _White; }
        set { _White = value; }
    }

    [Inspector(group = "Model Settings")]
    [SerializeField]
    private Material _Yellow;
    /// <summary>
    /// Yellow color material
    /// </summary>
    public Material ColorYellow
    {
        get { return _Yellow; }
        set { _Yellow = value; }
    }

    #endregion

    // Used for content creation
    internal string ShipMesh = "";
    internal string ShipMaterialRoot = "";
    internal string ShieldMaterial = "";
    public List<GameObject> meshes = new List<GameObject>();
    public List<MeshRenderCloakController> meshesscripts = new List<MeshRenderCloakController>();
    public Material shieldmaterial;
    private Vector3 correctPlayerPos;
    private Quaternion correctPlayerRot;

    // Had to delete when moving scripts around, need to re add _ship_editor back for it to work probs
    // Happens on adding the script, create objects if they don't exist
    public virtual void Reset()
    {

        if (this.GetComponent<CapsuleCollider>() == null)
            this.gameObject.AddComponent<CapsuleCollider>();

        if (this.GetComponent<PhotonView>() == null)
            this.gameObject.AddComponent<PhotonView>();

        if (this.GetComponent<PhotonTransformView>() == null)
            this.gameObject.AddComponent<PhotonTransformView>();

        if (this.transform.Find("Ship") == null)
        {
            GameObject shp = new GameObject("Ship");

            // Set layer
            shp.layer = LayerMask.NameToLayer("UI");

            // Add components
            shp.AddComponent<MeshFilter>();
            shp.AddComponent<MeshRenderer>();
            shp.AddComponent<MeshCollider>();

            SetChildTransformTo(shp, this.transform);

            this.ShipObject = shp;
        }
 
        if (this.transform.Find("Weapons") == null)
        {
            GameObject weapons = new GameObject("Weapons");

            SetChildTransformTo(weapons, this.transform);

            this.WeaponsObject = weapons;
        }

        // Add the engines object
        if (this.transform.Find("Engines") == null)
        {
            GameObject engines = new GameObject("Engines");

            SetChildTransformTo(engines, this.transform);

            this.EnginesObject = engines;
        }

        // Add the equipment object
        if (this.transform.Find("Equipment") == null)
        {
            GameObject equipment = new GameObject("Equipment");

            SetChildTransformTo(equipment, this.transform);

            this.EquipmentObject = equipment;
        }
        //Add the healthbar
        if (this.transform.Find("HealthBar") == null)
        {
            HealthBar = Instantiate(HealthBarinit, transform.position, new Quaternion(0, 0, 0, 0));
            HealthBar.transform.parent = transform;
            HealthBar.transform.localPosition = new Vector3(0, 0, 0);

        }
        else
        {
            HealthBar = this.transform.Find("HealthBar").gameObject;
        }

        // Add the UI object
        if (this.transform.Find("UI") == null)
        {
            GameObject ui = new GameObject("UI");

            // Set layer
            ui.layer = LayerMask.NameToLayer("UI");

            // Add base components
            ui.AddComponent<Canvas>();
            ui.AddComponent<CanvasScaler>();

            SetChildTransformTo(ui, this.transform);

            RectTransform tmp = ui.GetComponent<RectTransform>();
            tmp.localPosition = Vector3.zero;
            tmp.pivot = new Vector2(0.5f, 0.5f);
            tmp.localRotation = Quaternion.Euler(90f, 0f, 0f);
            tmp.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            #region Add the bars

            GameObject bar1 = new GameObject("Bar");
            Image img = bar1.AddComponent<Image>();
            img.color = new Color32(163, 231, 255, 68);
            img.raycastTarget = false;

            bar1.transform.SetParent(ui.transform);

            tmp = bar1.GetComponent<RectTransform>();
            tmp.localPosition = new Vector3(0f, 0f, -20.75f);
            tmp.sizeDelta = new Vector2(1f, 25f);
            tmp.pivot = new Vector2(0.5f, 0.5f);
            tmp.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            tmp.localScale = Vector3.one;

            // Add the second bar
            GameObject bar2 = new GameObject("Bar");
            img = bar2.AddComponent<Image>();
            img.color = new Color32(163, 231, 255, 68);
            img.raycastTarget = false;

            bar2.transform.SetParent(bar1.transform);

            tmp = bar2.GetComponent<RectTransform>();
            tmp.localPosition = new Vector3(0f, 0f, 0f);
            tmp.sizeDelta = new Vector2(1f, 25f);
            tmp.pivot = new Vector2(0.5f, 0.5f);
            tmp.localRotation = Quaternion.Euler(0f, 90f, 0f);
            tmp.localScale = Vector3.one;

            #endregion

            #region Add the banner

            GameObject banner = new GameObject("Banner");
            banner.AddComponent<RectTransform>();
            banner.AddComponent<BillBoardMe>();

            // Add banner to the UI component
            banner.transform.SetParent(ui.transform);

            tmp = banner.GetComponent<RectTransform>();
            tmp.localPosition = new Vector3(0f, 0f, -35.13f);
            tmp.pivot = new Vector2(0.5f, 0.5f);
            tmp.localRotation = Quaternion.Euler(-60f, 0f, 0f);
            tmp.localScale = Vector3.one;

            // -- Add Hull Object
            GameObject hull = new GameObject("Hull");

            // Set as child
            hull.transform.SetParent(banner.transform);

            // Set the image details
            img = hull.AddComponent<Image>();

            #region determine sprite type
            /*
            switch (this.HullType)
            {
                case eHullType.SuperCapital:
                    img.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Content/Textures/UI/png/105.png");
                    break;
                case eHullType.Capital:
                    img.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Content/Textures/UI/png/106.png");
                    break;
                case eHullType.Destroyer:
                    img.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Content/Textures/UI/png/107.png");
                    break;
                case eHullType.Cruiser:
                    img.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Content/Textures/UI/png/108.png");
                    break;
                case eHullType.Frigate:
                    img.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Content/Textures/UI/png/109.png");
                    break;
                case eHullType.Corvette:
                    img.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Content/Textures/UI/png/110.png");
                    break;
                case eHullType.Light:
                    img.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Content/Textures/UI/png/101.png");
                    break;
            }
            */
            #endregion
            // So it turns out if you want to add this code to a build file, what you have to do is create an editor class that interfaces with "_ship" in editor, then add that class to a folder called "Editor"
            img.type = Image.Type.Simple;

            // Set the shadow details
            Shadow shdw = hull.AddComponent<Shadow>();
            shdw.effectDistance = new Vector2(0.1f, -0.1f);
            shdw.useGraphicAlpha = true;

            // Set rect defaults
            tmp = hull.GetComponent<RectTransform>();
            tmp.sizeDelta = new Vector2(5f, 5f);
            tmp.pivot = new Vector2(0.5f, 0.5f);
            tmp.localRotation = Quaternion.identity;
            tmp.localScale = Vector3.one;
            tmp.localPosition = new Vector3(-2.5f, 0f, 0.12f);

            // -- Add the class object
            GameObject clss = new GameObject("Class");
            clss.transform.SetParent(hull.transform);
            img = clss.AddComponent<Image>();

            #region determine sprite type
            /*
            switch (this.ShipClass)
            {
                case eShipClass.Tank:
                    img.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Content/Textures/UI/png/151.png");
                    break;
                case eShipClass.Defense:
                    img.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Content/Textures/UI/png/17.png");
                    break;
                case eShipClass.Offense:
                    img.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Content/Textures/UI/png/7.png");
                    break;
                case eShipClass.Support:
                    img.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_Content/Textures/UI/png/84.png");
                    break;
            }
            */
            #endregion

            img.type = Image.Type.Simple;

            // Set the shadow details
            shdw = clss.AddComponent<Shadow>();
            shdw.effectDistance = new Vector2(0.1f, -0.1f);
            shdw.useGraphicAlpha = true;

            // Set rect defaults
            tmp = clss.GetComponent<RectTransform>();
            tmp.sizeDelta = new Vector2(5f, 5f);
            tmp.pivot = new Vector2(0.5f, 0.5f);
            tmp.localRotation = Quaternion.identity;
            tmp.localScale = Vector3.one;
            tmp.localPosition = new Vector3(5.05f, 0f, 0f);

            #endregion

            this.UIObject = ui.GetComponent<Canvas>();

            // Update the movement stats
            AdjustStats();

            // Post updates to speed component
        }

    }

    private float startarmour;
    public CustomPathfinding fac;
    public GameObject World2;
    public GameObject Cannon_Projectile;
    public GameObject Missile_Projectile;
    public GameObject Lazer_Shot;
    public GameObject MiniGun_Projectile;
    public GameObject shipDestruction;
    public Forcefield forcefield;
    private eShipColor lasthitby;

    // recieve input of take health away from ship
    public void takeDamage(int damage,eShipColor oriteam,eHullType inputhull)
    {
        if (unittargetcontrol.crosslevelholder.tutorial == false)
        {
            lasthitby = oriteam;
            if (Shield > 0)
            {
                Shield -= damage;
                //forcefield.OnHit(pos);
            }
            else
            {
                if (HullType == eHullType.Corvette)
                {
                    if (Armor > startarmour * 0.6) Armor = Mathf.RoundToInt(startarmour * 0.5f);
                    else if (Armor > startarmour * 0.3) Armor = Mathf.RoundToInt(startarmour * 0.2f);
                    else Armor = -5;
                }
                else if (HullType == eHullType.Light)
                {
                    if (Armor > startarmour * 0.8) Armor = Mathf.RoundToInt(startarmour * 0.7f);
                    else if (Armor > startarmour * 0.6) Armor = Mathf.RoundToInt(startarmour * 0.5f);
                    else if (Armor > startarmour * 0.4) Armor = Mathf.RoundToInt(startarmour * 0.3f);
                    else if (Armor > startarmour * 0.2) Armor = Mathf.RoundToInt(startarmour * 0.1f);
                    else Armor = -5;
                }
                else Armor -= damage;

            }
            if (Shield < 0) Shield = -1;
        }
        if(inputhull == eHullType.Light && HullType == eHullType.Light)
        {
            fac.turnspeed = fac.turnspeed - 0.5;
        }
    }

    // check if the ship has lost any health over time.
    public bool isshieldfull()
    {
        if (Shield >= startshield) return true;
        else return false;
    }

    // adds health to healed ship
    public void addShield(int damage)
    {
       if(HullType != eHullType.Light) Shield += damage;
        if (Shield > startshield) Shield = startshield;
    }

    // enables the healthbaronly on command
    public void enablehealthbar() { if (HealthBar && Healthbaronly.GetActive() == false) Healthbaronly.SetActive(true);  }

    // disables the healthbaronly on command
    public void disablehealthbar () 
    {
        if (Healthbaronly == null) Healthbaronly = transform.Find("HealthBarOnly").gameObject;
        if (Healthbaronly != null && Healthbaronly.GetActive() == true &&  LeftHandMenu != null && LeftHandMenu.GetActive() == false) Healthbaronly.SetActive(false);
    }

    // Initialising connected scripts and child classes.
    void Awake()
    {
        if(GameObject.Find("LeftController"))  LeftHandMenu = GameObject.Find("LeftController").GetComponent<LeftControl_Onehanddrag>().MenuObj;
        if (transform.Find("WarpJumpOut_green_gamma") && transform.Find("WarpJumpOut_green_gamma").Find("WarpJumpFX").Find("WarpJumpDistortion")) Destroy(transform.Find("WarpJumpOut_green_gamma").Find("WarpJumpFX").Find("WarpJumpDistortion").gameObject);
        foreach (Transform t in transform)   if (t.GetComponent<_Weapon>() != null) t.gameObject.SetActive(false);
    }

    // checks if there is room in the pool to spawn another shot and if so returns said shot (if there isn't the simulation still runs, it just doesnt create an effect)
    GameObject spawnshot (GameObject objecttospawn)
    {
        GameObject output = null;
        Transform temp = F3DPoolManager.Pools["GeneratedPool"].Spawn(objecttospawn.transform);
        if (temp) output = temp.gameObject;
        return output;
    }

    // Not actually firing a weapon on the network, just deterministically firing. Takes vairables from _Weapon that constitute how the shot is supposed to function and returns the fired effect.
    public GameObject FireWeaponNetwork(Vector3 target, bool hit, Vector3 source, GameObject id, int damage, Vector3 sourcelocal, _Weapon.guntype Type,GameObject sourceobj,_Weapon.guntype actualtype)
    {
        GameObject shot = null;
        if (Type == _Weapon.guntype.Cannon || Type == _Weapon.guntype.CapitalMainGun) shot = spawnshot(Cannon_Projectile);
        if (Type == _Weapon.guntype.Missile) shot = spawnshot(Missile_Projectile);
        if (Type == _Weapon.guntype.MiniGun) shot = spawnshot(MiniGun_Projectile);
        if (Type == _Weapon.guntype.ShieldGenerator || Type == _Weapon.guntype.Lazer) shot = spawnshot(Lazer_Shot);
        if (Type == _Weapon.guntype.BomberGun) shot = spawnshot(MiniGun_Projectile);
        if (shot == null) return null;

        shot.transform.position = source;
        shot.transform.parent = transform.parent.Find("Working");
        if (Type == _Weapon.guntype.Cannon) shot.transform.localScale = new Vector3(20, 20, 20);
        if(Type == _Weapon.guntype.Cannon && HullType == eHullType.Capital) shot.transform.localScale = new Vector3(10,10,10);
        if (Type == _Weapon.guntype.MiniGun) shot.transform.localScale = new Vector3(3, 3, 15);
        if (Type == _Weapon.guntype.BomberGun) shot.transform.localScale = new Vector3(6, 6, 6);
        if (Type == _Weapon.guntype.Missile) shot.transform.localScale = new Vector3(15, 15, 15);
        if (Type == _Weapon.guntype.CapitalMainGun) shot.transform.localScale = new Vector3(40, 40, 80);
        if (hit && (Type == _Weapon.guntype.Cannon || Type == _Weapon.guntype.MiniGun)) shot.transform.LookAt(target);
        if (Type == _Weapon.guntype.Missile) shot.transform.forward = sourceobj.transform.up;
        if (Type == _Weapon.guntype.Missile || Type == _Weapon.guntype.BomberGun) shot.GetComponent<ProjectileMovementCom>().isMissile();
        
        if (hit == false)
        {
            shot.transform.LookAt(target);
            shot.transform.eulerAngles = shot.transform.eulerAngles + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
        }

        if (id != null && Type != _Weapon.guntype.ShieldGenerator)
        {
            GameObject targetgam = id;
            ProjectileMovementCom mov = shot.GetComponent<ProjectileMovementCom>();
            mov.targetgam = targetgam;
            mov.origin = sourcelocal;
            mov.damage = damage;
            mov.ishit = hit;
            mov.guntype = actualtype;
        }

        if (Type == _Weapon.guntype.ShieldGenerator)
        {
           _Ship targetscript = id.GetComponent<_Ship>();
            Debug.Log(targetscript.isshieldfull());
            if (targetscript.isshieldfull() == false)
            {
                ProjectileMovementCom mov = shot.GetComponent<ProjectileMovementCom>();
                mov.startlineren(sourceobj, id, shieldmaterial,1,Type,sourceobj);
                targetscript.addShield(damage);
                mov.origin = sourcelocal;
                mov.damage = damage;
                mov.ishit = hit;
                mov.guntype = actualtype;

            }
            else
            {
                WeaponTarget = null;
                F3DPoolManager.Pools["GeneratedPool"].Despawn(shot.transform);
            }

        }
        if (Type == _Weapon.guntype.Lazer)
        {
            ProjectileMovementCom mov = shot.GetComponent<ProjectileMovementCom>();
            _Ship targetscript = id.GetComponent<_Ship>();
            mov.startlineren(sourceobj, id, shieldmaterial,3, Type, sourceobj);
            targetscript.takeDamage(damage,ShipColor,HullType);
        }
        return shot;
    }

   
    // disables ship meshes to simulate cloak.
    public void assigncloackfog(bool cloak)
    {
        foreach (Transform t in transform) if (t.name == "Ship")
        {
            MeshRenderCloakController meshcon = t.GetComponent<MeshRenderCloakController>();
            if (cloak) meshcon.cloack();
            else meshcon.uncloack();
            meshcon.distancecloacked = cloak;
        }
    }

    // checks if it needs to uncloak.
    void cloakrep()
    {
        if (iscloacked)
        {
            float range = 10000;
            if (HullType == eHullType.Capital) range = 1500;
            if (HullType == eHullType.Destroyer) range = 1000;
            if (HullType == eHullType.Cruiser) range = 800;
            if (HullType == eHullType.Frigate) range = 1500;
            List<GameObject> ships = unittargetcontrol.teammembersout(ShipColor);
            foreach (GameObject ship in ships) if(ship && ship.gameObject != null) foreach (Transform t in ship.transform) if (t.name == "Ship") t.GetComponent<MeshRenderCloakController>().cloaked = false;
            foreach (GameObject ship in ships)
            {
                int i = 0;
                if (ship && ship.gameObject != null && Vector3.Distance(transform.localPosition, ship.transform.localPosition) < range && i < 3)
                {
                    i++;
                    foreach (Transform t in ship.transform) if (t.name == "Ship" && t.GetComponent<MeshRenderCloakController>().asigncloacked())
                        {
                             Transform shottemp = F3DPoolManager.Pools["GeneratedPool"].Spawn(Lazer_Shot.transform);
                             if (shottemp)
                             {
                                  GameObject shot = shottemp.gameObject;
                                  ProjectileMovementCom mov = shot.GetComponent<ProjectileMovementCom>();
                                  shot.transform.parent = transform.parent.Find("Working");
                                  mov.startlineren(gameObject, ship, shieldmaterial, 1,_Weapon.guntype.ShieldGenerator,gameObject);
                                  mov.ishit = true;
                             }
                        }
                }
            }
        }

        // moves fighters around to simulate flocking behavior.
        if ((HullType == eHullType.Light || HullType == eHullType.Corvette) && fac.started == true)
        {
            for (int i = 0; i < targetmesh.Count; i++)  targetmesh[i] = new Vector3((0.5f - Random.value) * 100, (0.5f - Random.value) * 100, (0.5f - Random.value) * 100);
        }
    }
  
    
    public bool updatedtest;
    public bool isTrackingimproved;
    public float timepassed1;
    private bool shown;
    public TSTransform WeaponTarget;

    void asignordertopathfinding (storedorder input)
    {
        if (input.shiptargettstransform != null && input.shiptargettstransform.gameObject != null)
        {
            TSVector finaltarget = input.shiptargettstransform.transform.position.ToTSVector();
            fac.assigntarget(finaltarget, input.shiptargettstransform.position);
            shiptargetposlast = input.shiptargettstransform.position;
        }
        else
        {
            TSVector finaltarget = World2.transform.Find("Objects").TransformPoint(input.WorldTarget.ToVector()).ToTSVector();
            fac.assigntarget(finaltarget, input.WorldTarget);
        }
    }

    public void Rep()
    {
        if (WeaponTarget != null && unittargetcontrol != null && weaponry[0].guntypemain != _Weapon.guntype.ShieldGenerator  && UnitMovementcommandcontroller.getteam(WeaponTarget.shipscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode) == UnitMovementcommandcontroller.getteam(ShipColor, unittargetcontrol.crosslevelholder.Gamemode)) WeaponTarget = null;
        if (WeaponTarget == null) WeaponTarget = FindTarget();
    }



    // gets the needed thickness of the selection outline based on the scale of the world.
    float getthicknesstoset ()
    {
       float a = World.transform.localScale.x / 1000;
        if (a < 0.03f) a = 0.03f;    
        return a;
    }

    // Update Function, is not called automatically for some reason, has to be called from inputrelay.
    public void Update()
    {
        storedordercount = StoredOrders.Count;
        if (HealthBar && healthbarstartscale != new Vector3(0, 0, 0) && (HealthBar.GetActive() == true || Healthbaronly.GetActive() == true))
        {
            if (World.transform.localScale.x > 60) HealthBar.transform.localScale = healthbarstartscale;
            if(World.transform.localScale.x < 40 && World.transform.localScale.x > 12) HealthBar.transform.localScale = ( 1 / (World.transform.localScale.x / 40)) * forcedhealthscale * 2f;
            else if (World.transform.localScale.x < 12) HealthBar.transform.localScale = 8 * forcedhealthscale *  0.7f;

            if (World.transform.localScale.x > 60) Healthbaronly.transform.localScale = healthbarstartscale;
            if (World.transform.localScale.x < 40 && World.transform.localScale.x > 12) Healthbaronly.transform.localScale = (1 / (World.transform.localScale.x / 40)) * forcedhealthscale * 2f;
            else if (World.transform.localScale.x < 12) Healthbaronly.transform.localScale = 8 * forcedhealthscale * 0.7f;

        }
        timepassedforrun++;
        if (runnext != 0 && timepassedforrun >= runnext)
        {
            Rep();
            runnext = 0;
        }
        timepassed1 = timepassed1 + Time.deltaTime;
        if (World && outline && outline.active == true)
        {
            outline.stencilOutline.SetFloat("_Thickness", getthicknesstoset());
            for(int i = 0; i < outline.listofcopiedmodels.Count; i++)
            {
                 if(outline.listoforiginalmodels[i] != null && outline.listoforiginalmodels[i].gameObject != null)   outline.listofcopiedmodels[i].transform.localPosition = outline.listoforiginalmodels[i].transform.localPosition;
                 else
                {
                    GameObject oridestroy = outline.listoforiginalmodels[i];
                    GameObject copydestroy = outline.listofcopiedmodels[i];
                    outline.listoforiginalmodels.Remove(oridestroy);
                    outline.listofcopiedmodels.Remove(copydestroy);
                    Destroy(copydestroy);

                }
            }
        }

        if (timepassed1 > 3 && shown == false)
        {
            shown = true;
            foreach (GameObject ship in meshes) if(ship.GetActive() == false) ship.SetActive(true);
            foreach( Transform t in transform)
            {
               if (t.GetComponent<_Weapon>() != null ) if (t.gameObject.GetActive() == false && meshes[0].GetActive() == true) t.gameObject.SetActive(true);
               if (t.name == "Engines") if (t.gameObject.GetActive() == false) t.gameObject.SetActive(true);
            }
         
            if (transform.Find("WarpJumpOut_green_gamma")) Destroy(transform.Find("WarpJumpOut_green_gamma").gameObject);
            if (transform.Find("PAR_SEBImpactA_1a_0000 (1)")) Destroy(transform.Find("PAR_SEBImpactA_1a_0000 (1)").gameObject);
            HealthBar.SetActive(true);
            Healthbaronly.SetActive(true);
            transform.Find("HealthBar").Find("GameObject").Find("barPanel").Find("Healthbar").GetComponent<Image>().sprite = getsprite();
            transform.Find("HealthBarOnly").Find("GameObject").Find("barPanel").Find("Healthbar").GetComponent<Image>().sprite = getsprite();
            HealthBar.SetActive(false);
            Healthbaronly.SetActive(false);        }
        else foreach (Transform t in transform) if (t.name == "Engines" && t.gameObject.GetActive() == false) t.gameObject.SetActive(false);       
        
        // fac is the custompathfinding script.
        if (fac)
        {
            updatedtest = true;
            fac.SpecUpdate();
            foreach (_Weapon weapon in weaponry) if( timepassed1 > 3) weapon.UpdateSpec();
            foreach (MeshRenderCloakController mesh in meshesscripts) if(mesh != null) mesh.specupdate();
            if (World2 == null)  World2 = GameObject.Find("World");
            if (TrueSyncManager.Time > 0)
            {
                if (shiptarget && shiptarget.gameObject == null)
                {
                    shiptarget = null;
                    shiptargettstransform = null;
                    MoveTarget = shiptargetposlast;
                }

                if (StoredOrders.Count == 0)  asignordertopathfinding(new storedorder(MoveTarget, averagespeed, null, shiptargettstransform));
                else asignordertopathfinding(StoredOrders[0]);
                
                if (HealthBar.GetActive() == true) healthbarinterface.fillAmount = (Armor + Shield) / startcombined;
                if(Healthbaronlyinterface.gameObject.GetActive() == true) Healthbaronlyinterface.fillAmount = (Armor + Shield) / startcombined;

                // kill ship if no health left.
                if (Armor < 0)
                {
                    if (unittargetcontrol.crosslevelholder.campaign == false && unittargetcontrol.team == lasthitby) unittargetcontrol.addmoney(PointsToDeploy / 3);
                    foreach (AIController aicon in unittargetcontrol.aicontrollers) if (aicon.team == lasthitby && aicon.ismission == false) aicon.addmoney(PointsToDeploy / 3);
                    if (HullType != eHullType.Light && HullType != eHullType.Corvette)
                    {
                        Transform temp2 = F3DPoolManager.Pools["GeneratedPool"].Spawn(shipDestruction.transform);
                        if (temp2)
                        {
                            GameObject temp = temp2.gameObject;
                            temp.transform.parent = GameObject.Find("Working").transform;
                            temp.transform.position = transform.position;
                            temp.transform.localScale = new Vector3(10, 10, 10) * hulltypemoderator(HullType);
                        }
                    }
                    Destroy(this.gameObject);
                }

                if (engines)
                {
                    if (fac.currentlymoving == true) { if (timepassed1 > 3) setenginesactive(true); }
                    else setenginesactive(false);
                }




                if (HullType == eHullType.Light || HullType == eHullType.Corvette)
                {
                    for (int i = 0; i < meshes.Count; i++)
                    {
                      if(meshes[i].gameObject != null)  meshes[i].transform.localPosition = Vector3.Lerp(meshes[i].transform.localPosition, targetmesh[i], Time.deltaTime * 0.01f);
                    }
                    if (HullType == eHullType.Light)
                    {
                        if (meshes.Count == 5 && Armor < startarmour * 0.8) removefromfighters(meshes[4]);
                        if (meshes.Count == 4 && Armor < startarmour * 0.6) removefromfighters(meshes[3]);
                        if (meshes.Count == 3 && Armor < startarmour * 0.4) removefromfighters(meshes[2]);
                        if (meshes.Count == 2 && Armor < startarmour * 0.2) removefromfighters(meshes[1]);
                    }
                    if (HullType == eHullType.Corvette)
                    {
                        if (meshes.Count == 5 && Armor < startarmour * 0.66) removefromfighters(meshes[2]);
                        if (meshes.Count == 4 && Armor < startarmour * 0.33) removefromfighters(meshes[1]);
                    }
                }
            }
        }
        disablehealthbar();
        if (LeftHandMenu && LeftHandMenu.GetActive() == true && Healthbaronly.GetActive() == false) Healthbaronly.SetActive(true);
    }
    // switch engine activeness status.
    void setenginesactive (bool input)
    {
        if (input == false) { if (engines.GetActive() == true) engines.SetActive(false); }
        else { if (engines.GetActive() == false) engines.SetActive(true); }
    }

    public int storedordercount;
    private GameObject LeftHandMenu;
    
    // TODO: Allow fighters to respawn on heal.
    // reduces the number of fighter meshes corresponding to ship group health.
    void removefromfighters(GameObject input)
    {
        meshes.Remove(input);
        Destroy(input);
    }


    UnitMovementcommandcontroller unittargetcontrol;
    public GameObject engines;
    private float startcombined;
    public FP averagespeed;
    public List<storedorder> StoredOrders = new List<storedorder>();

    // gives custompathfinding new move order or adds to list of orders if waypoint mode is set.
    public void asignMoveOrder(TSVector WorldTarget, GameObject Target,FP averagespeedin,bool addtoorderlist)
    {
        if (addtoorderlist == false)asignsinglemoveorder(WorldTarget,  Target,  averagespeedin);
        else StoredOrders.Add( new storedorder( WorldTarget, averagespeedin,  Target));
    }

    // asigns move order to custompathfinding.
    void asignsinglemoveorder (TSVector WorldTarget, GameObject Target, FP averagespeedin)
    {
            StoredOrders.Clear();
            MoveTarget = WorldTarget;
            if (Target != null) shiptarget = Target.GetComponent<TSTransform>();
            if (Target != null) shiptargettstransform = Target.GetComponent<TSTransform>();
            else
            {
                shiptarget = null;
                shiptargettstransform = null;

            }
            averagespeed = averagespeedin;
    }

    // moves to next order in list of order if waypoint mode enabled
    public void movetonextstoredorder () { if(StoredOrders.Count > 0) StoredOrders.RemoveAt(0); }
    
    // gets size of explosion based on hulltype
    private float hulltypemoderator(eHullType input)
    {
        float output = 1;
        if (input == eHullType.Capital) output = 3;
        if (input == eHullType.Destroyer) output = 2.5f;
        if (input == eHullType.Cruiser) output = 2f;
        if (input == eHullType.Frigate) output = 1.5f;
        if (input == eHullType.Corvette) output = 1f;
        if (input == eHullType.Light) output = 0.7f;
        return output;
    }

    public int runnext;
    public int timepassedforrun;
    public int a;
    public TSTransform[] Reptemp;

    //  gets all allied ships and thenwaits a deterministic random amount of time.
    public void callallreps()
    {
        List<TSTransform> temptemp = new List<TSTransform>();
        temptemp = unittargetcontrol.targetsout(ShipColor);
        if (temptemp.Count != 0)
        {
            if (weaponry.Count > 0 && weaponry[0].guntypemain != _Weapon.guntype.ShieldGenerator) Reptemp = temptemp.ToArray();
            else Reptemp = unittargetcontrol.alliesoutcustom(ShipColor).ToArray();
        }
        timepassedforrun = 0;
        runnext = (transform.GetComponent<PhotonView>().viewID % 1000) / 2;
        //  Rep();
    
    }

    public bool started;

    //Actual simulation start function, calls start in child classess too.
    // TODO: make this less of a solid block of code.
    public virtual void StartMain()
    {
        started = true;
        int i = 0;
        averagespeed = MaxSpeed * 30;
        foreach (Transform t in transform)
        {
            if (t.GetComponent<_Weapon>() != null)
            {
                weaponry.Add(t.GetComponent<_Weapon>());
                t.GetComponent<_Weapon>().StartMain(i);
                i++;
                t.gameObject.SetActive(false);
            }
        }
        if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1)   InvokeRepeating("cloakrep", TSRandom.RangeFP(0f, 1f).AsFloat(), 3);
        objectsholder = GameObject.Find("Objects");
        unittargetcontrol = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
        startshield = Shield;
        if (ShieldObject) forcefield = ShieldObject.GetComponent<Forcefield>();
        shipDestruction = Resources.Load("Explosion_001") as GameObject;
        startarmour = Armor;
        startcombined = Armor + Shield;
        if (transform.Find("Engines")) engines = transform.Find("Engines").gameObject;
        Healthbaronly = transform.Find("HealthBarOnly").gameObject;
        
        foreach (Transform t in transform)
        {
            if (t.name == "Ship")
            {
                meshes.Add(t.gameObject);
                meshesscripts.Add(t.GetComponent<MeshRenderCloakController>());
                targetmesh.Add(new Vector3(0, 0, 0));
                startmeshpos.Add(t.localPosition);
            }
            if (t.name == "Engines") t.gameObject.SetActive(false);
        }
        outline = GetComponent<VRTK_OutlineObjectCopyHighlighter>();
        if (outline) outline.Initialise(Color.green);
        transformview = GetComponent<PhotonTransformView>();
        if (SceneManager.GetActiveScene().name != "MenuMain" && HullType != eHullType.Destroyer) transform.localScale = new Vector3(8, 8, 8);
        else if (SceneManager.GetActiveScene().name != "MenuMain") transform.localScale = new Vector3(12, 12, 12);
        World = GameObject.Find("WorldScaleBase");
        World2 = GameObject.Find("World");
        if (SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 4) GetComponent<TSTransform>().position = transform.localPosition.ToTSVector();
        fac = this.GetComponent<CustomPathfinding>();
        asignsinglemoveorder(GetComponent<TSTransform>().position, null,MaxSpeed);
        HealthBar = transform.Find("HealthBar").gameObject;
        healthbarinterface = transform.Find("HealthBar").Find("GameObject").Find("barPanel").Find("Healthbar").GetComponent<Image>();
        if( Healthbaronly) Healthbaronlyinterface = transform.Find("HealthBarOnly").Find("GameObject").Find("barPanel").Find("Healthbar").GetComponent<Image>();
        if(Healthbaronly) Healthbaronly.SetActive(false);
        if (HealthBar) HealthBar.SetActive(false);
        if (GameObject.Find("RightController") && GameObject.Find("RightController").GetComponent<RightHand_triggerInstantOrder>().selectedships.Contains(this.gameObject)) HealthBar.SetActive(true);
        ShipColor = getteambynumber(spawnnum);
        foreach (GameObject mesh in meshes)  mesh.GetComponent<MeshRenderCloakController>().StartMain();
        healthbarstartscale = HealthBar.transform.localScale;
        forcedhealthscale = new Vector3(0.02f, 0.02f, 0.02f);

    }

    // gets the spawn color based on the start number.
    public static eShipColor getteambynumber (int input)
    {
        eShipColor output = eShipColor.Blue;
        if (input == 1) output = eShipColor.Green;
        if (input == 2) output = eShipColor.Blue;
        if (input == 3) output = eShipColor.White;
        if (input == 4) output = eShipColor.Yellow;
        if (input == 5) output = eShipColor.Grey;
        if (input == 6) output = eShipColor.Red;
        return output;
    }

    private Vector3 forcedhealthscale;
    // gets the color of the healthbar, in freefor all it depends on team. in everything is it is red for enemy, green for owned ships and blue for allied.
    Sprite getsprite ()
    {
        if (unittargetcontrol.crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip || unittargetcontrol.crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch)
        {
            bool a = (UnitMovementcommandcontroller.getteam(ShipColor, unittargetcontrol.crosslevelholder.Gamemode) == UnitMovementcommandcontroller.getteam(unittargetcontrol.team, unittargetcontrol.crosslevelholder.Gamemode));
            if (a == true)
            {
                if (ShipColor == unittargetcontrol.team) return GreenHealthBar;
                else return BlueSprite;
            }
            else return RedHealthBar;
        }
        else
        {
            if (ShipColor == eShipColor.Blue) return BlueSprite;
            else if (ShipColor == eShipColor.Green) return GreenHealthBar;
            else if (ShipColor == eShipColor.Grey) return GreySprite;
            else if (ShipColor == eShipColor.Red) return RedHealthBar;
            else if (ShipColor == eShipColor.White) return WhiteSprite;
            else return YellowSprite;
        }
    }
    // Update is called once per frame
    private GameObject World;


    /// <summary>
    /// Method to update the stats easily when hull or class changes
    /// </summary>
    private void AdjustStats()
    {

    }

    /// <summary>
    /// Handle network serialization
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

        }
        else
        {
            // Network player, receive data
        }
    }

    /// <summary>
    /// Adds the child to the object, and defaults position / rotations / scales
    /// </summary>
    /// <param name="go"></param>
    private void SetChildTransformTo(GameObject child, Transform parent)
    {
        child.transform.SetParent(parent.transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;
    }
    // Use this for initialization
    public void enable()
    {
        if (outline == null) outline = GetComponent<VRTK_OutlineObjectCopyHighlighter>();
        outline.Highlight(Color.green);
        outline.active = true;
        outline.stencilOutline.SetFloat("_Thickness", getthicknesstoset());
        for (int i = 0; i < outline.listofcopiedmodels.Count; i++)
        {
            outline.listofcopiedmodels[i].transform.localPosition = outline.listoforiginalmodels[i].transform.localPosition;
        }
        /*
        if (HealthBar) HealthBar.SetActive(true);
        else
        {
            if (transform.Find("HealthBar"))
            {
                HealthBar = transform.Find("HealthBar").gameObject;
                HealthBar.SetActive(true);
            }

        }
        */

    }

    // disable selection indicator
    public void Disable()
    {
        if (outline == null) outline = GetComponent<VRTK_OutlineObjectCopyHighlighter>();
        outline.stencilOutline.SetFloat("_Thickness", getthicknesstoset());
        for (int i = 0; i < outline.listofcopiedmodels.Count; i++)
        {
            outline.listofcopiedmodels[i].transform.localPosition = outline.listoforiginalmodels[i].transform.localPosition;
        }
        outline.Unhighlight();
    }

    // Un-used class that would spawn weapons based on the ones stored as attached to that ship as dictated by the player.
    public void SpawnWeapons(string wep1, string wep2, string wep3, string wep4, string wep5, Vector3 wep1pos, Vector3 wep2pos, Vector3 wep3pos, Vector3 wep4pos, Vector3 wep5pos, Vector3 wep1scale, Vector3 wep2scale, Vector3 wep3scale, Vector3 wep4scale, Vector3 wep5scale, int spawnpos, bool bot, Vector3 spawnposmain)
    {
        isbot = bot;
        spawnnum = spawnpos;
        if (wep1 != null && wep1 != "")
        {
            GameObject spawn = Resources.Load("Turrets/" + wep1 + "InGame") as GameObject;
            GameObject spawned = Instantiate(spawn);
            spawned.transform.parent = transform;
            spawned.transform.localScale = wep1scale;
            spawned.transform.localPosition = wep1pos;
            spawned.transform.localEulerAngles = new Vector3(0, 0, 0);
        }

        if (wep2 != null && wep2 != "")
        {
            GameObject spawn = Resources.Load("Turrets/" + wep2 + "InGame") as GameObject;
            Debug.Log(wep2);
            GameObject spawned = Instantiate(spawn);
            spawned.transform.parent = transform;
            spawned.transform.localScale = wep2scale;
            spawned.transform.localPosition = wep2pos;
            spawned.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        if (wep3 != null && wep3 != "")
        {
            GameObject spawn = Resources.Load("Turrets/" + wep3 + "InGame") as GameObject;
            GameObject spawned = Instantiate(spawn);
            spawned.transform.parent = transform;
            spawned.transform.localScale = wep3scale;
            spawned.transform.localPosition = wep3pos;
            spawned.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        if (wep4 != null && wep4 != "")
        {
            GameObject spawn = Resources.Load("Turrets/" + wep4 + "InGame") as GameObject;
            GameObject spawned = Instantiate(spawn);
            spawned.transform.parent = transform;
            spawned.transform.localScale = wep4scale;
            spawned.transform.localPosition = wep4pos;
            spawned.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        if (wep5 != null && wep5 != "")
        {
            GameObject spawn = Resources.Load("Turrets/" + wep5 + "InGame") as GameObject;
            GameObject spawned = Instantiate(spawn);
            spawned.transform.parent = transform;
            spawned.transform.localScale = wep5scale;
            spawned.transform.localPosition = wep5pos;
            spawned.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        GetComponent<TSTransform>().position = spawnposmain.ToTSVector();
    }

    int i;

    // every type of weapon is most effective against a set ship type, this returns that preferance to allow better auto-targeting.
    public static eHullType getpreferance (_Weapon.guntype source)
    {
        eHullType preferance ;
        if (source == _Weapon.guntype.FighterGun) preferance = eHullType.Light;
        else if (source == _Weapon.guntype.BomberGun) preferance = eHullType.Capital;
        else if(source == _Weapon.guntype.Dual_Role) preferance = eHullType.Frigate;
        else if(source == _Weapon.guntype.MiniGun) preferance = eHullType.Light;
        else if (source == _Weapon.guntype.Cannon) preferance = eHullType.Capital;
        else if (source == _Weapon.guntype.Lazer) preferance = eHullType.Capital;
        else if (source == _Weapon.guntype.Missile) preferance = eHullType.Frigate;
        else if (source == _Weapon.guntype.CapitalMainGun) preferance = eHullType.Frigate;
        else preferance = eHullType.Frigate;
        return preferance;
    }

    // asigns target for weaponry to shoot at.
    public TSTransform FindTarget()
    {
        if (weaponry.Count > 0)
        {
            i++;
            if (weaponry.Count > 0 && weaponry[0].guntypemain == _Weapon.guntype.ShieldGenerator && WeaponTarget && WeaponTarget.GetComponent<_Ship>().isshieldfull() == true) WeaponTarget = null;
            if (i == 5) WeaponTarget = null;
            if (WeaponTarget && TSVector.Distance(WeaponTarget.GetComponent<TSTransform>().position, fac.transformts.position) > weaponry[0].Range) WeaponTarget = null;
            FP distance = 100000;
            if (weaponry[0].guntypemain != _Weapon.guntype.ShieldGenerator)
            {
                if (shiptarget != null && UnitMovementcommandcontroller.getteam(shiptarget.shipscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode) != UnitMovementcommandcontroller.getteam(ShipColor, unittargetcontrol.crosslevelholder.Gamemode) && (weaponry[0].guntypemain != _Weapon.guntype.BomberGun || shiptarget.shipscript.HullType != eHullType.Light) && (weaponry[0].guntypemain != _Weapon.guntype.Missile || shiptarget.shipscript.HullType != eHullType.Light)) WeaponTarget = shiptarget.GetComponent<TSTransform>();
            }
            else
            {
                if (shiptarget != null && UnitMovementcommandcontroller.getteam(shiptarget.shipscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode) == UnitMovementcommandcontroller.getteam(ShipColor, unittargetcontrol.crosslevelholder.Gamemode) && shiptarget != fac.transformts) WeaponTarget = shiptarget.GetComponent<TSTransform>();
            }
            TSTransform[] objs = Reptemp;
            eHullType preferance = getpreferance(weaponry[0].guntypemain);
            
            if (weaponry[0].guntypemain != _Weapon.guntype.ShieldGenerator)
            {
                if (WeaponTarget == null)
                {
                    for (int i = 0; i < objs.Length; i++)
                    {
                        if (objs[i] != null && objs[i].gameObject != null)
                        {

                            FP dis = TSVector.Distance(objs[i].position, fac.transformts.position);
                            if (distance > dis && objs[i] != this.transform.gameObject && objs[i].shipscript.HullType == preferance)
                            {
                                hitdebug = objs[i];
                                distance = dis;
                                if (distance < weaponry[0].Range) WeaponTarget = objs[i];
                            }
                        }
                    }
                }
                if (WeaponTarget == null)
                {
                    for (int i = 0; i < objs.Length; i++)
                    {
                        if (objs[i] != null && objs[i].gameObject != null )
                        {

                            FP dis = TSVector.Distance(objs[i].position, fac.transformts.position);
                            if (distance > dis && objs[i] != this.transform.gameObject && (weaponry[0].guntypemain != _Weapon.guntype.BomberGun || objs[i].shipscript.HullType != eHullType.Light) && (weaponry[0].guntypemain != _Weapon.guntype.Missile || objs[i].shipscript.HullType != eHullType.Light))
                            {
                                hitdebug = objs[i];
                                distance = dis;
                                if (distance < weaponry[0].Range) WeaponTarget = objs[i];
                            }
                        }
                    }
                }
            }
            else
            {
                if (WeaponTarget == null)
                {
                    for (int i = 0; i < objs.Length; i++)
                    {
                        if (objs[i] != null && objs[i].gameObject != null)
                        {

                            FP dis = TSVector.Distance(objs[i].position, fac.transformts.position);
                            if (distance > dis && objs[i] != this.transform.gameObject && objs[i].shipscript.isshieldfull() == false)
                            {
                                hitdebug = objs[i];
                                distance = dis;
                                if (distance < weaponry[0].Range) WeaponTarget = objs[i];
                            }
                        }
                    }
                }
            }
            return WeaponTarget;
        }
        else return null;
    }

    public TSTransform hitdebug;

    // waypoint order stored for later movement.
    public class storedorder
    {
        public TSVector WorldTarget;
        public FP averagespeed;
        public TSTransform shiptargettstransform;

        public storedorder(TSVector WorldTargetin,  FP averagespeedin,GameObject Targetin = null,TSTransform transforminput = null)
        {
            WorldTarget = WorldTargetin;
            averagespeed = averagespeedin;
            if(Targetin != null)
            {
                shiptargettstransform = Targetin.GetComponent<TSTransform>();
            }
            else
            {
                shiptargettstransform = transforminput;
            }
            
            
            
        }
    }
}
