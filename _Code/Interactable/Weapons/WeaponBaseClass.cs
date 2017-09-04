using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WeaponBaseClass : MonoBehaviour {
    private GameObject weapon;
    public GameObject Base;
    private Vector3 startpos;
    private Quaternion startrot;
    private Transform startparent;
    private Vector3 startscale;
    public TextMesh childtext;
    private float size = 1;
    private bool noparent;
    public int cost;
    private _Ship ship;
    public bool equipment;
    public bool special;
    // Use this for initialization
    void Awake()
    {
        startpos = transform.position;
        startrot = transform.rotation;
        startscale = transform.localScale;
        if (late == false) { startparent = transform.parent; }
    }
    void Start() {
      
        ship = GetComponent<_Ship>();
        if (ship) {
            cost = ship.PointsToDeploy;
        }
        if (transform.parent == null)
        {
            Destroy(gameObject);
        }
        else
        {
            if (transform.parent.parent.name != "Weapons"&& transform.parent.parent.name != "ShipListParent" && ship == null) transform.localScale = new Vector3(1,1,1);
            else if (transform.parent.parent.name == "Weapons")
            {
                transform.localScale = GameObject.Find("ShipParent").GetComponent<MainMenuShipController>().scaleout() * 100;
            }
            if (transform.Find("Description"))
            {
                childtext = transform.Find("Description").gameObject.GetComponent<TextMesh>();
            }
            
            if (late)
            {
                disabletext();
            }
            else
            {
                enabletext();
            }
        }

        
    }

    // Update is called once per frame
    void Update() {
        //  Debug.Log(transform.localScale);
        if (transform.parent.parent.name == "Weapons")
        {
            transform.localScale = GameObject.Find("ShipParent").GetComponent<MainMenuShipController>().scaleout() * 100;
        }
    }
    public bool valid ()
    {
        return true;
    }
    public void startdif(float sizein, GameObject parentin)
    {
        size = sizein;
        late = true;
        startparent = parentin.transform;
    }
    public void respawn()
    {
        if (Base.transform.parent.Find(name) == null)
        {
            GameObject gam = Instantiate(this.gameObject, Base.transform.position, Base.transform.rotation);
            gam.transform.parent = Base.transform.parent;
            if (ship) gam.transform.localScale = new Vector3(1, 1, 1);
            else gam.transform.localScale = startscale;
            gam.GetComponent<WeaponBaseClass>().size = size;
            gam.name = name;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    public GameObject copy ()
    {
        if (Base.transform.childCount < 1)
        {
            GameObject gam = Instantiate(this.gameObject, Base.transform.position, Base.transform.rotation);
            gam.transform.localScale = startscale;
              gam.GetComponent<WeaponBaseClass>().startdif(size, startparent.gameObject);
               gam.name = name;
            
          
            return gam;

        }
        else
        {
            return gameObject;
        }
    }
    public void reset(float sizeinput)
    {
        if ((Base.transform.childCount < 1 || transform.parent == Base.transform) && Base.transform.parent.Find(name) == null)
        {
            transform.position = startpos;
            transform.rotation = startrot;
            transform.parent = Base.transform.parent;
            if (sizeinput >= 0.01)
            {
                size = sizeinput;
            }
            transform.localScale = startscale;
            Debug.Log(startscale);
        }
        else
        {
            Destroy(gameObject);
        }
        enabletext();
        

    }
    public void enabletext ()
    {
        if (childtext)
        {
            childtext.gameObject.SetActive(true);
        }
        
    }
    public void disabletext ()
    {
        if (childtext)
        {
            childtext.gameObject.SetActive(false);
        }
        
    }
    public void scaletosize()
    {
            transform.localScale = new Vector3( size,size,size);  
    }
    private bool late;
    public void disabletextlate ()
    {
        late = true;
    }
    public void startget ()
    {
        if (transform.Find("Description"))
        {
            childtext = transform.Find("Description").gameObject.GetComponent<TextMesh>();
           late = true;
        }
        
    }

}
