using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using simple;

public class SettingsController : MonoBehaviour {
    private IOComponent FleetBuilder;
    public GameObject currentdisplay;
    public bool leftmode;
    private TextMesh currentdisplaytext;
    public string current;
    public GameObject leftbuttontext;
    private TextMesh leftbuttontexttext;
    public GameObject leftdisplaytext;
    private TextMesh leftdisplaytextext;
    private GameObject RightController;
    private GameObject LeftController;
    private GameObject leftcontrollerparent;
    private GameObject rightcontrollerparent;
    void Start ()
    {
        FleetBuilder = IORoot.findIO("fleet1");
        FleetBuilder.read();
        currentdisplaytext = currentdisplay.GetComponent<TextMesh>();
        retrievesetting();
        leftmode = retrievelefthandmode();
        leftbuttontexttext = leftbuttontext.GetComponent<TextMesh>();
        leftdisplaytextext = leftdisplaytext.GetComponent<TextMesh>();
  

    }
    int i;

    void Update ()
    {

    }
	// Use this for initialization
	public void High ()
    {
        setsetting("High");
        QualitySettings.SetQualityLevel(5,true);
    }

    public void Medium ()
    {
        setsetting("Medium");
        QualitySettings.SetQualityLevel(3, true);
    }

    public void Low ()
    {
        setsetting("Low");
        QualitySettings.SetQualityLevel(1, true);
    }

    public string retrievesetting() {
        string output = FleetBuilder.get<string>("Settings");
        FleetBuilder.read();
        Debug.Log(output);
        settext(output);
        current = output;
        updateleftdisplay();
        return output;
    }
    
    public void setsetting (string fair)
    {
        FleetBuilder.add("Settings",fair);
        FleetBuilder.write();
        settext(fair);
    }
    public void setlefthandmode (bool input)
    {
        FleetBuilder.add("LeftMode", input);
        FleetBuilder.write();
        leftmode = input;
        updateleftdisplay();
      
    }

    public bool retrievelefthandmode ()
    {
        return FleetBuilder.get<bool>("LeftMode"); 
    }

    public void settext (string input) {
        currentdisplaytext.text = input;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f); 
    }
    public void EnableLeftHandMode ()
    {
        if (leftmode == true) setlefthandmode(false);
        else setlefthandmode(true);
    }
    public void updateleftdisplay ()
    {
        leftbuttontexttext = leftbuttontext.GetComponent<TextMesh>();
        leftdisplaytextext = leftdisplaytext.GetComponent<TextMesh>();
        if (leftmode == true)
        {
            leftbuttontexttext.text = "disable left hand mode";
            leftdisplaytextext.text = "Left hand mode: enabled";

        }
        else
        {
             leftbuttontexttext.text = "enable left hand mode";
            leftdisplaytextext.text = "Left hand mode: disabled";
        }
      
    }
    public void returntomenu ()
    {
        GameObject.Find("Controllers").GetComponent<GalaxyController>().returntonormal();
    }
}
