using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class allows you to enable the controller tooltips.
/// </summary>
public class HelpButtonSender : MonoBehaviour {

    /// <summary>
    /// This function enables the controller tooltips if they are already open and closes them if not.
    /// </summary>
    public void Help()
    {
        if (GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().TutorialObjs[0].GetActive() == true) sethelpactive(false);
        else sethelpactive(true);
    }

    /// <summary>
    /// This function does the actual coding of enabling or disabling the tooltips.
    /// </summary>
    /// <param name="activetoset"></param>
    void sethelpactive(bool activetoset) {
        foreach (GameObject help in GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().TutorialObjs.ToArray())
            if (help != null && help.gameObject != null)
            {
                foreach (Transform t in help.GetComponentInChildren<Transform>(true)) help.SetActive(true);
                help.SetActive(activetoset);
            }
    }
    
    
}
