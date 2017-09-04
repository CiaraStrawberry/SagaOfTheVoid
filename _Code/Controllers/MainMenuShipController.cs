using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using simple;

public class MainMenuShipController : MonoBehaviour
{
    public int countthrough;
    private GunController guncontroller;
    private IOComponent FleetBuilder;
    public List<GameObject> Ships;
    public List<ShipContainer> ShipList = new List<ShipContainer>();
    public GameObject TurretHolder;         //16
    public GameObject display;
    private TextMesh displaycomponent;
    public GameObject resetparent;
    // Use this for initialization
    public void Awake()
    {
        
        FleetBuilder = IORoot.findIO("fleet1");
        if(FleetBuilder) FleetBuilder.read();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) reset();
        if (Input.GetKeyDown(KeyCode.RightArrow)) Addone();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) gobackone();

    }
    public Vector3 scaleout ()
    {
        return  new Vector3 (ShipList[countthrough].size, ShipList[countthrough].size, ShipList[countthrough].size);
    }
    public void Start()
    {
        ShipList.Add(new ShipContainer(Ships[0], 0.7f, "Capital Offense", Ships[0].transform.position,false));
        ShipList.Add(new ShipContainer(Ships[1], 0.7f, "Capital Defense", Ships[1].transform.position, false));
        ShipList.Add(new ShipContainer(Ships[2], 0.7f, "Capital Support", Ships[2].transform.position, true));
        ShipList.Add(new ShipContainer(Ships[3], 0.7f, "Capital Tank", Ships[3].transform.position, false));
        ShipList.Add(new ShipContainer(Ships[4], 0.35f, "Cruiser Defense", Ships[4].transform.position, false));
        ShipList.Add(new ShipContainer(Ships[5], 0.35f, "Cruiser Offense", Ships[5].transform.position, false));
        ShipList.Add(new ShipContainer(Ships[6], 0.35f, "Cruiser Support", Ships[6].transform.position, true));
        ShipList.Add(new ShipContainer(Ships[7], 0.35f, "Cruiser Tank", Ships[7].transform.position, false));
        ShipList.Add(new ShipContainer(Ships[8], 0.4f, "Destroyer Defense", Ships[8].transform.position, false));
        ShipList.Add(new ShipContainer(Ships[9], 0.4f, "Destroyer Offense", Ships[9].transform.position, false));
        ShipList.Add(new ShipContainer(Ships[10], 0.4f,"Destroyer Support", Ships[10].transform.position, true));
        ShipList.Add(new ShipContainer(Ships[11], 0.4f,"Destroyer Tank", Ships[11].transform.position, false));
        ShipList.Add(new ShipContainer(Ships[12], 0.3f,"Frigate Defense", Ships[12].transform.position, false));
        ShipList.Add(new ShipContainer(Ships[13], 0.3f,"Frigate Offense", Ships[13].transform.position, false));
        ShipList.Add(new ShipContainer(Ships[14], 0.3f,"Frigate Support", Ships[14].transform.position, true));
        ShipList.Add(new ShipContainer(Ships[15], 0.3f,"Frigate Tank", Ships[15].transform.position, false));

        checkupdate();
        guncontroller = TurretHolder.GetComponent<GunController>();
        guncontroller.reset(sizeget());
        
        displaycomponent = display.GetComponent<TextMesh>();
        updatedisplay();
        StartCoroutine(Example());
        Deploy_Current_Ship(ShipList[countthrough].ship);
    }

    public void quit ()
    {
        Save_current_Ship(ShipList[countthrough].ship);
        GameObject.Find("Controllers").GetComponent<GalaxyController>().returntonormal();
    }

    void updatedisplay() { displaycomponent.text = ShipList[countthrough].namestring; }

    public void reset() {
        foreach (ShipContainer ship in ShipList) {
            ship.ship.transform.position = ship.startloc;
            ship.ship.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        Save_current_Ship(resetparent.transform.Find( ShipList[countthrough].ship.name).gameObject);
        Deploy_Current_Ship(ShipList[countthrough].ship);
    }

    public void disableall() { foreach (ShipContainer ship in ShipList) ship.ship.SetActive(false); }

    // Update is called once per frame
    public void Addone()
    {
        Save_current_Ship(ShipList[countthrough].ship);
        countthrough += 1;
        if (countthrough == 16) countthrough = 0;
        checkupdate();
        guncontroller.reset(sizeget());
        Deploy_Current_Ship(ShipList[countthrough].ship);
        ShipList[countthrough].ship.transform.localPosition = new Vector3(-0.1339f, 0.0317f, -0.259f);
        updatedisplay();
        
    }

    public void gobackone()
    {
        Save_current_Ship(ShipList[countthrough].ship);
        countthrough -= 1;
        if (countthrough == -1) countthrough = 15;
        checkupdate();
        guncontroller.reset(sizeget());
        Deploy_Current_Ship(ShipList[countthrough].ship);
        ShipList[countthrough].ship.transform.localPosition = new Vector3(-0.1339f,0.0317f,-0.259f);
        updatedisplay();

    }
    void checkupdate()
    {
        disableall();
        ShipList[countthrough].ship.SetActive(true);

    }

    GameObject Gameobjectget ()  { return ShipList[countthrough].ship; }
   
    float sizeget() {    return ShipList[countthrough].size;   }

    public List<GameObject> Children = new List<GameObject>();
    void Save_current_Ship(GameObject saveobject)
    {
        int i = 0;
        Children.Clear();
        if (saveobject.transform.Find("Weapons").childCount > 0 && saveobject.transform.Find("Weapons").Find("Attachpoint 1").childCount > 0)
            Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 1").GetChild(0).gameObject);
        else Children.Add(null);
        if (saveobject.transform.Find("Weapons").childCount > 1 && saveobject.transform.Find("Weapons").Find("Attachpoint 2").childCount > 0)
            Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 2").GetChild(0).gameObject);
        else Children.Add(null);
        if (saveobject.transform.Find("Weapons").childCount > 2 && saveobject.transform.Find("Weapons").Find("Attachpoint 3").childCount > 0)
            Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 3").GetChild(0).gameObject);
        else Children.Add(null);
        if (saveobject.transform.Find("Weapons").childCount > 3 && saveobject.transform.Find("Weapons").Find("Attachpoint 4").childCount > 0)
            Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 4").GetChild(0).gameObject);
        else Children.Add(null);
        if (saveobject.transform.Find("Weapons").childCount > 4 && saveobject.transform.Find("Weapons").Find("Attachpoint 5").childCount > 0)
            Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 5").GetChild(0).gameObject);
        else Children.Add(null);

        foreach (GameObject stri in Children)
        {
            if (stri != null) {
                
                string fleetstring = saveobject.name + i;
                FleetBuilder.add(fleetstring, Children[i].name);
                Vector3 loc = Children[i].transform.localPosition;
                Vector3 rot = Children[i].transform.localEulerAngles;
                FleetBuilder.add(saveobject.name + "parent" + i, stri.transform.parent.name);
                FleetBuilder.write();
                FleetBuilder.add(saveobject.name + "num", i + 1);
                FleetBuilder.add(saveobject.name + "scalex" + i, stri.transform.localScale.x);
                FleetBuilder.add(saveobject.name + "scaley" + i, stri.transform.localScale.y);
                FleetBuilder.add(saveobject.name + "scalez" + i, stri.transform.localScale.z);
                FleetBuilder.add(saveobject.name + "posx" + i, stri.transform.parent.localPosition.x);
                FleetBuilder.add(saveobject.name + "posy" + i, stri.transform.parent.localPosition.y);
                FleetBuilder.add(saveobject.name + "posz" + i, stri.transform.parent.localPosition.z);
                //Destroy(stri);
            }
            else
            {
                string fleetstring = saveobject.name + i;
                FleetBuilder.add(fleetstring, "nothing");
                Debug.Log("nothing");
            }
            FleetBuilder.write();
          
            i++;
        }
    }

    void Deploy_Current_Ship (GameObject loadObj)
    {
           if (ShipList[countthrough].special) guncontroller.supporton();
        else guncontroller.supportoff();
        int o = FleetBuilder.get<int>(loadObj.name +"num");
        Debug.Log(o);
        for (int a = 0; a < o; a++)
        {
            string objparent = FleetBuilder.get<string>(loadObj.name + a);
            float scalex = FleetBuilder.get<float>(loadObj.name + "scalex" + a);
            float scaley = FleetBuilder.get<float>(loadObj.name + "scaley" + a);
            float scalez = FleetBuilder.get<float>(loadObj.name + "scalez" + a);
            if (objparent != "nothing")
            {
                
                Debug.Log(objparent);
                GameObject gam = GameObject.Find("ShipParent").transform.Find("TurretHolder").Find(objparent).gameObject.GetComponent<WeaponBaseClass>().copy();
               if ( loadObj.transform.Find("Weapons").Find("Attachpoint " + (a + 1)).transform.childCount < 1 )
                {
                gam.transform.parent = loadObj.transform.Find("Weapons").Find("Attachpoint " + (a + 1));
                gam.transform.localPosition = new Vector3(0, 0, 0);
                gam.transform.localScale = new Vector3(scalex, scaley, scalez);
                gam.transform.localRotation = new Quaternion(0, 0, 0, 0);
                gam.GetComponent<WeaponBaseClass>().startget();
                
                }
               else
                {
                     foreach (Transform t in loadObj.transform.Find("Weapons").Find("Attachpoint " + (a + 1)).transform)
                    {
                        Destroy(t.gameObject);
                    }
                    gam.transform.parent = loadObj.transform.Find("Weapons").Find("Attachpoint " + (a + 1));
                    gam.transform.localPosition = new Vector3(0, 0, 0);
                    gam.transform.localScale = new Vector3(scalex, scaley, scalez);
                    gam.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    gam.GetComponent<WeaponBaseClass>().startget();

                }
            }
  
        }
        if (o == 0) {
            Save_current_Ship(resetparent.transform.Find(loadObj.name).gameObject);
            Deploy_Current_Ship(loadObj);
        }
                FleetBuilder.write();
        string objparent1 = FleetBuilder.get<string>(loadObj.name + "0");
        Debug.Log(objparent1);
     
    }

    void checkreset1(GameObject loadObj)
    {
        int o = FleetBuilder.get<int>(loadObj.name + "num");
        if (o == 0) Save_current_Ship(resetparent.transform.Find(loadObj.name).gameObject);
        
    }
    IEnumerator Example()
    {
        yield return new WaitForSeconds(0.5f);
        Deploy_Current_Ship(ShipList[countthrough].ship);
    }
    public  void checkresetall ()   {  foreach (ShipContainer ship in ShipList) checkreset1(ship.ship);}
    
}


public class ShipContainer
{
    public GameObject ship;
    public string namestring;
    public float size;
    public Vector3 startloc;
    public bool special;
    public ShipContainer (GameObject nam, float siz, string namestri, Vector3 startlo, bool specialin)
    {
        ship = nam;
        size = siz;
        namestring = namestri;
        startloc = startlo;
        special = specialin;
    }
        
}



