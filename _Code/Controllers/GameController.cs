using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    /// <summary>
    /// The VR object in the scene
    /// </summary>
    public GameObject VrObject;

    /// <summary>
    /// Current active menu
    /// </summary>
    private aGameState _CurrentState;

    /// <summary>
    /// Default menu to transition to
    /// </summary>
    public aGameState DefaultState;

    /// <summary>
    /// Container to put spawned objects into.  used to size and do other things
    /// </summary>
    public GameObject WorldContainerObject;

    /// <summary>
    /// First spawn point, always faces 0.0.0
    /// </summary>
    /// 

    [SerializeField]
    private Vector3 SpawnPoint1local;
    [SerializeField]
    private Vector3 SpawnPoint2local;
    [SerializeField]
    private Vector3 SpawnPoint3local;
    [SerializeField]
    private Vector3 SpawnPoint4local;
    [SerializeField]
    private Vector3 SpawnPoint5local;
    [SerializeField]
    private Vector3 SpawnPoint6local;

    [SerializeField]
    private Vector3 SpawnPoint1localfreeforall;
    [SerializeField]
    private Vector3 SpawnPoint2localfreeforall;
    [SerializeField]
    private Vector3 SpawnPoint3localfreeforall;
    [SerializeField]
    private Vector3 SpawnPoint4localfreeforall;
    [SerializeField]
    private Vector3 SpawnPoint5localfreeforall;
    [SerializeField]
    private Vector3 SpawnPoint6localfreeforall;

    [HideInInspector]
    public Vector3 SpawnPoint1;
    [HideInInspector]
    public Vector3 SpawnPoint2;
    [HideInInspector]
    public Vector3 SpawnPoint3;
    [HideInInspector]
    public Vector3 SpawnPoint4;
    [HideInInspector]
    public Vector3 SpawnPoint5;
    [HideInInspector]
    public Vector3 SpawnPoint6;

    /// <summary>
    /// Cached menu items
    /// </summary>
    public List<aGameState> CachedStates;

    /// <summary>
    /// Transition to another menu
    /// </summary>
    /// <param name="go">Menu to transition to, or null to remove menu</param>
    public void TransitionTo(aGameState go) {

        // Notify new menu that we are switching, if no new menu, deactive the other
        if (go != null) {
            go.OnTransitionTo();
        } else if (_CurrentState != null) {
            _CurrentState.OnTransitionFrom();
            _CurrentState.OnDeactivate();
            _CurrentState = null;
        }

        // Transition from previous if needed
        if (_CurrentState != null) {
            _CurrentState.OnTransitionFrom();
        }

        // Set active
        go.OnActive();

        // Set current
        _CurrentState = go;

    }

    public void convertspawnpoints (CrossLevelVariableHolder.gamemode input)
    {
        if( input == CrossLevelVariableHolder.gamemode.TeamDeathMatch)
        {
           GameObject World = GameObject.Find("Objects");
           SpawnPoint1 = World.transform.TransformPoint(SpawnPoint1local);
           SpawnPoint2 = World.transform.TransformPoint(SpawnPoint2local);
           SpawnPoint3 = World.transform.TransformPoint(SpawnPoint3local);
           SpawnPoint4 = World.transform.TransformPoint(SpawnPoint4local);
           SpawnPoint5 = World.transform.TransformPoint(SpawnPoint5local);
           SpawnPoint6 = World.transform.TransformPoint(SpawnPoint6local);
        }
        if(input == CrossLevelVariableHolder.gamemode.FreeForAll)
        {
            GameObject World = GameObject.Find("Objects");
            SpawnPoint1 = World.transform.TransformPoint(SpawnPoint1localfreeforall);
            SpawnPoint2 = World.transform.TransformPoint(SpawnPoint2localfreeforall);
            SpawnPoint3 = World.transform.TransformPoint(SpawnPoint3localfreeforall);
            SpawnPoint4 = World.transform.TransformPoint(SpawnPoint4localfreeforall);
            SpawnPoint5 = World.transform.TransformPoint(SpawnPoint5localfreeforall);
            SpawnPoint6 = World.transform.TransformPoint(SpawnPoint6localfreeforall);
        }
        if(input == CrossLevelVariableHolder.gamemode.CapitalShip)
        {
            GameObject World = GameObject.Find("Objects");
            SpawnPoint1 = World.transform.TransformPoint(SpawnPoint1local);
            SpawnPoint2 = World.transform.TransformPoint(SpawnPoint2local);
            SpawnPoint3 = World.transform.TransformPoint(SpawnPoint3local);
            SpawnPoint4 = World.transform.TransformPoint(SpawnPoint4local);
            SpawnPoint5 = World.transform.TransformPoint(SpawnPoint5local);
            SpawnPoint6 = World.transform.TransformPoint(SpawnPoint6local);
        }
  
    }
    // Use this for initialization
    void Awake ()
    {
        
    }

    void Start() {
        
        // Make sure that SOV folder exists
        if (!System.IO.Directory.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\SOV\\")) {
            System.IO.Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\SOV\\");
        }
        
        if (DefaultState != null) {

            //_CurrentMenu = Instantiate(DefaultMenu, this.transform) as GameObject;
            _CurrentState = DefaultState;//Instantiate<aGameState>(DefaultState);

            // Determine if we need to resize
            _CurrentState.transform.parent = this.transform;

            // Transition to
            _CurrentState.OnTransitionTo();

            // Set active
            _CurrentState.OnActive();

        }

    }

    // Update is called once per frame
    void Update() {
        //convertspawnpoints();
    }

}
