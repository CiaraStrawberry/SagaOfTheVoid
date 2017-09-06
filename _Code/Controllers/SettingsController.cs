using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using simple;

/// <summary>
/// The script that controls the data relevant to changing the settings.
/// </summary>
public class SettingsController : MonoBehaviour {

    // the script used to save everything to the hard drive.
    private IOComponent FleetBuilder;
    // is left hand mode already enabled?
    public bool leftmode;
    // the textmesh to tell you what settings you have enabled currently.
    private TextMesh currentdisplaytext;
    // the current string telling you what settings you have enabled.
    public string current;
    //the current text on the left hand mode button.
    private TextMesh leftbuttontexttext;
    // the current text on the text below the left hand button.
    private TextMesh leftdisplaytextext;

    /// <summary>
    /// initialise everything.
    /// </summary>
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
   
    /// <summary>
    /// Enable High settings.
    /// </summary>
	public void High ()
    {
        setsetting("High");
        QualitySettings.SetQualityLevel(5,true);
    }

    /// <summary>
    /// Enable Medium Settings.
    /// </summary>
    public void Medium ()
    {
        setsetting("Medium");
        QualitySettings.SetQualityLevel(3, true);
    }

    /// <summary>
    /// Enable Low Settings.
    /// </summary>
    public void Low ()
    {
        setsetting("Low");
        QualitySettings.SetQualityLevel(1, true);
    }

    /// <summary>
    ///  Get the current settings from the hard drive.
    /// </summary>
    /// <returns>the settings string</returns>
    public string retrievesetting() {
        string output = FleetBuilder.get<string>("Settings");
        FleetBuilder.read();
        Debug.Log(output);
        settext(output);
        current = output;
        updateleftdisplay();
        return output;
    }
    
    /// <summary>
    /// set the settings to a string.
    /// </summary>
    /// <param name="fair"></param>
    public void setsetting (string fair)
    {
        FleetBuilder.add("Settings",fair);
        FleetBuilder.write();
        settext(fair);
    }

    /// <summary>
    /// Set the left hand mode on or off.
    /// </summary>
    /// <param name="input"></param>
    public void setlefthandmode (bool input)
    {
        FleetBuilder.add("LeftMode", input);
        FleetBuilder.write();
        leftmode = input;
        updateleftdisplay();
      
    }

    /// <summary>
    /// gets the left hand mode before starting.
    /// </summary>
    /// <returns>the left hand mode.</returns>
    public bool retrievelefthandmode ()
    {
        return FleetBuilder.get<bool>("LeftMode"); 
    }

    /// <summary>
    /// Change the current display text to a set string.
    /// </summary>
    /// <param name="input"></param>
    public void settext (string input) {
        currentdisplaytext.text = input;
    }

    /// <summary>
    /// Wait 0.5 seconds.
    /// </summary>
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f); 
    }

    /// <summary>
    /// Enable or disable left hand mode.
    /// </summary>
    public void EnableLeftHandMode ()
    {
        if (leftmode == true) setlefthandmode(false);
        else setlefthandmode(true);
    }
    
    /// <summary>
    /// Update the text based on relevant data.
    /// </summary>
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

    public void returntomenu ()  {  GameObject.Find("Controllers").GetComponent<GalaxyController>().returntonormal(); }

  }
