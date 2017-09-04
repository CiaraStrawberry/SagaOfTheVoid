using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using simple;

public class FleetBuilderController : MonoBehaviour {
    public int limit;
    public int countthrough;
    private FleetNumControl guncontroller;
    private IOComponent FleetBuilder;
    public List<GameObject> Ships;
    public GameObject TurretHolder;         //16
    public GameObject display;
    private TextMesh displaycomponent;
    public List<GameObject> shipstodeploy = new List<GameObject>();
    public GameObject fleet1;
    public GameObject fleet2;
    public GameObject fleet3;
    public int currenttotal;
    public int currentmoney;
    public GameObject defaultparent;
    // Use this for initialization
    void Start()
    {
       // ShipList.Add(new ShipContainer(Ships[0], 0.7f, "Capital Offense", Ships[0].transform.position));
        checkupdate();
        FleetBuilder = IORoot.findIO("fleet1");
        FleetBuilder.read();
       // displaycomponent = display.GetComponent<TextMesh>();
       // updatedisplay();
        StartCoroutine(Example());
        guncontroller = GetComponent<FleetNumControl>();
        countthrough = 0;
        checkupdate();
    }
    void Update ()
    {
        
        if (fleet1.GetActive() == true) currenttotal = checkall(fleet1);
        else if (fleet2.GetActive() == true) currenttotal = checkall(fleet2);
        else if (fleet3.GetActive() == true) currenttotal = checkall(fleet3);
        currentmoney = limit - currenttotal;
        if (Input.GetKeyDown(KeyCode.R)) reset();
        if (Input.GetKeyDown(KeyCode.RightArrow)) Addone();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) gobackone();

    }

    int checkall(GameObject parent)
    {
        int output = 0;
        foreach (Transform child in parent.transform)
        {
            if (child.childCount > 0)
                output += child.GetChild(0).GetComponent<WeaponBaseClass>().cost;
        }
        return output;
    }
    public bool enough (int input)
    {
        bool output;
        if (input <= currentmoney)output = true;
        else output = false;
        return output;
    }

    public void quit()
    {
        //Save_current_Ship(ShipList[countthrough].ship);
        GameObject.Find("Controllers").GetComponent<GalaxyController>().returntonormal();
    }

//    void updatedisplay() { displaycomponent.text = Ships[countthrough].name; }

    public void reset()
    {
        Save_current_Ship(defaultparent.transform.Find(Ships[countthrough].name).gameObject);
        Deploy_Current_Fleet(Ships[countthrough]);
    }

    public void disableall() { foreach (GameObject ship in Ships) ship.SetActive(false); }
    public void enableall() { foreach (GameObject ship in Ships) ship.SetActive(true); }

    // Update is called once per frame
    public void Addone()
    {
        Save_current_Ship(Ships[countthrough]);
        countthrough++;
        if (countthrough > 2) countthrough = 0;
        checkupdate();
        //guncontroller.reset();
        Debug.Log(countthrough);
        Deploy_Current_Fleet(Ships[countthrough]);
       // updatedisplay();

    }

    public void gobackone()
    {
        Save_current_Ship(Ships[countthrough]);
        countthrough--;
        if (countthrough < 0) countthrough = 2;
        checkupdate();
       // guncontroller.reset();
        Deploy_Current_Fleet(Ships[countthrough]);
       // updatedisplay();

    }
    void checkupdate()
    {
        disableall();
        Ships[countthrough].SetActive(true);

    }

    GameObject Gameobjectget() { return Ships[countthrough]; }

    float sizeget() { return 1; }

    public List<GameObject> Children = new List<GameObject>();

    void Save_current_Ship(GameObject saveobject)
    {
        enableall();
        int i = 0;
        Children.Clear();
        Debug.Log(saveobject.transform.childCount);
        foreach (Transform child in saveobject.transform)
        {
            if (child.childCount > 0)
            Children.Add(child.GetChild(0).gameObject);
            else Children.Add(null);
        }
        
        foreach (GameObject stri in Children)
        {
            if (stri != null)
            {
                string fleetstring = saveobject.name + i;
                FleetBuilder.add(fleetstring, Children[i].name);
                Vector3 loc = Children[i].transform.localPosition;
                Vector3 rot = Children[i].transform.localEulerAngles;
                FleetBuilder.add(saveobject.name + "parent" + i, stri.transform.parent.name);
                FleetBuilder.write();
                FleetBuilder.add(saveobject.name + "num", i + 1);
                FleetBuilder.add(saveobject.name + "scalex" + i, stri.transform.parent.localScale.x);
                FleetBuilder.add(saveobject.name + "scaley" + i, stri.transform.parent.localScale.y);
                FleetBuilder.add(saveobject.name + "scalez" + i, stri.transform.parent.localScale.z);
            }
            else
            {
                string fleetstring = saveobject.name + i;
                FleetBuilder.add(fleetstring, "nothing");
                //Debug.Log(fleetstring);
            }
           
            i++;
            
        }
         FleetBuilder.write();
        checkupdate();
    }

    void Deploy_Current_Fleet(GameObject loadObj)
    {
        int o = FleetBuilder.get<int>(loadObj.name + "num");
        Debug.Log(o);
        Debug.Log(loadObj.name + "num");
        for (int a = 0; a < o; a++)
        {
            string objparent = FleetBuilder.get<string>(loadObj.name + a);
            float scalex = FleetBuilder.get<float>(loadObj.name + "scalex" + a);
            float scaley = FleetBuilder.get<float>(loadObj.name + "scaley" + a);
            float scalez = FleetBuilder.get<float>(loadObj.name + "scalez" + a);
            Debug.Log("Attachpoint " + (a + 1) + " " + objparent);
            if (objparent != "nothing")
            {
                GameObject gam = null;

                bool state = false;
                //  ShipListPage2(Frigates)
                for (int i = 0; i < shipstodeploy.Count; i++)
                {
                    if (shipstodeploy[i].name == objparent)
                    {
                        gam = shipstodeploy[i].GetComponent<WeaponBaseClass>().copy();
                        state = shipstodeploy[i].transform.parent.gameObject.GetActive();
                    }
                }
                
                if (gam)
                {
                    if (loadObj.transform.Find("Attachpoint " + (a + 1)).transform.childCount < 1)
                    {
                        gam.SetActive(true);
                        gam.transform.parent = loadObj.transform.Find("Attachpoint " + (a + 1));
                        gam.transform.localPosition = new Vector3(0, 0, 0);
                        gam.transform.localScale = new Vector3(100, 100, 100);
                        gam.transform.localRotation = new Quaternion(0, 0, 0, 0);
                        gam.GetComponent<WeaponBaseClass>().startget();
                        gam.transform.parent.gameObject.SetActive(state);
                        Debug.Log(gam);
                    }
                }




            }

        }
        FleetBuilder.write();
        string objparent1 = FleetBuilder.get<string>(loadObj.name + "0");
        Debug.Log(objparent1);
    }
    IEnumerator Example()
    {
        yield return new WaitForSeconds(0.5f);
        Deploy_Current_Fleet(Ships[countthrough]);
    }
  
}


