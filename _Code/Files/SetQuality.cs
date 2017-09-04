using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using simple;

public class SetQuality : MonoBehaviour
{
    // This code is the literal definition of "had too many revisions".

    private GameObject basehol;
    private IOComponent FleetBuilder;
    private GameObject Serializationmanager;
  
    // Use this for initialization

    void Awake()
    {
        FleetBuilder = IORoot.findIO("fleet1");

    }
    void Update()
    {
        //Debug.Log(FleetBuilder.get<int>("fleet1num"));
        //FleetBuilder.read();
    }
    void Start()
    {





     
        if (name == "high")
        {
            QualitySettings.SetQualityLevel(5);
        }
        if (name == "medium")
        {
            QualitySettings.SetQualityLevel(3);
        }
        if (name == "low")
        {
            QualitySettings.SetQualityLevel(1);
        }
        if (FleetBuilder == null)
            return;
      //  basehol = GameObject.FindWithTag("Hol");
        //GameObject linemanager = GameObject.FindWithTag("Man");
        //VariableStorage va = linemanager.GetComponent<VariableStorage>();
       

    }
    public void fleettest ()
    {
        int o = FleetBuilder.get<int>("fleet1num");
        FleetBuilder.read();
        Debug.Log(o);
      if (o == 0)
        {
  GameObject linemanager = GameObject.FindWithTag("Man");
            VariableStorage va = linemanager.GetComponent<VariableStorage>();
            va.reset();
        
        }
          
    }
   
    public void fleet3fun()
    {

        basehol = GameObject.FindGameObjectWithTag("Hol");
  

        foreach (Transform child in basehol.transform)
        {
            Destroy(child.gameObject);
        }

        int o = FleetBuilder.get<int>("fleet2num");
        FleetBuilder.read();
        Debug.Log(o);
        for (int a = 0; a < o; a++)
        {
            string fleettemp1 = FleetBuilder.get<string>("fleet3" + a);
            float fleettemp1locx = FleetBuilder.get<float>("fleet3locx" + a);
            float fleettemp1locy = FleetBuilder.get<float>("fleet3locy" + a);
            float fleettemp1locz = FleetBuilder.get<float>("fleet3locz" + a);
            float fleettemp1rotx = FleetBuilder.get<float>("fleet3rotx" + a);
            float fleettemp1roty = FleetBuilder.get<float>("fleet3roty" + a);
            float fleettemp1rotz = FleetBuilder.get<float>("fleet3rotz" + a);
            FleetBuilder.read();
            FleetBuilder.add("fleet3" + a, fleettemp1);
            FleetBuilder.write();
            GameObject temp = Resources.Load(fleettemp1) as GameObject;
            GameObject gam1 = Instantiate(temp, temp.transform.position, Quaternion.Euler(fleettemp1rotx, fleettemp1roty, fleettemp1rotz)) as GameObject;
            gam1.transform.parent = basehol.transform;
            gam1.transform.localPosition = new Vector3(fleettemp1locx, fleettemp1locy, fleettemp1locz);
            gam1.transform.localRotation = Quaternion.Euler(fleettemp1rotx, fleettemp1roty, fleettemp1rotz);
            gam1.name = fleettemp1;

        }
    }
    public List<GameObject> Children;
    public string ma;
    public void save(string fleet, GameObject saveobject)
    {

        Children.Clear();
  
        foreach (Transform child in saveobject.transform)
        {
            if (child.gameObject != saveobject)
            {
                
                Children.Add(child.gameObject);
            }
           
        }
        
        if (fleet == "fleet1")
        {
            int i = 0;
              foreach (GameObject stri in Children)
              {
                 string fleetstring = "fleet1" + i; 
                 FleetBuilder.add(fleetstring,Children[i].name);
                 Vector3 loc = Children[i].transform.localPosition;
                 Vector3 rot = Children[i].transform.localEulerAngles;
                 FleetBuilder.add("fleet1locx" + i, loc.x);
                 FleetBuilder.add("fleet1locy" + i, loc.y);
                 FleetBuilder.add("fleet1locz" + i, loc.z);
                 FleetBuilder.add("fleet1rotx" + i, rot.x);
                 FleetBuilder.add("fleet1roty" + i, rot.y);
                 FleetBuilder.add("fleet1rotz" + i, rot.z);
                 
                 FleetBuilder.write();
                 i++;
                 }
            FleetBuilder.add("fleet1num",i);

            FleetBuilder.write();
            FleetBuilder.read();
         

        }
        else
        {
            if (fleet == "fleet2")
            {
                int i = 0;
                foreach (GameObject stri in Children)
                {

                    string fleetstring = "fleet2" + i;
                    FleetBuilder.add(fleetstring, Children[i].name);
                    Vector3 loc = Children[i].transform.localPosition;
                    Vector3 rot = Children[i].transform.localEulerAngles;
                    FleetBuilder.add("fleet2locx" + i, loc.x);
                    FleetBuilder.add("fleet2locy" + i, loc.y);
                    FleetBuilder.add("fleet2locz" + i, loc.z);
                    FleetBuilder.add("fleet2rotx" + i, rot.x);
                    FleetBuilder.add("fleet2roty" + i, rot.y);
                    FleetBuilder.add("fleet2rotz" + i, rot.z);

                    FleetBuilder.write();
                    i++;
                }
                FleetBuilder.add("fleet2num", i);
                FleetBuilder.write();
                FleetBuilder.read();
             

            }
            else
            {
                if (fleet == "fleet3")
                {
                    int i = 0;
                    foreach (GameObject stri in Children)
                    {

                        string fleetstring = "fleet3" + i;
                        FleetBuilder.add(fleetstring, Children[i].name);
                        Vector3 loc = Children[i].transform.localPosition;
                        Vector3 rot = Children[i].transform.localEulerAngles;
                        FleetBuilder.add("fleet3locx" + i, loc.x);
                        FleetBuilder.add("fleet3locy" + i, loc.y);
                        FleetBuilder.add("fleet3locz" + i, loc.z);
                        FleetBuilder.add("fleet3rotx" + i, rot.x);
                        FleetBuilder.add("fleet3roty" + i, rot.y);
                        FleetBuilder.add("fleet3rotz" + i, rot.z);

                        FleetBuilder.write();
                        i++;
                    }
                    FleetBuilder.add("fleet3num", i);
                    FleetBuilder.write();
                    FleetBuilder.read();
                    fleet3fun();
                }
                else
                {
                    Debug.Log("null fleet");
                }
            }
        }
  

    }

    }

    

