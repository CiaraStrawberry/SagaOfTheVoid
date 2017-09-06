using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class controls the state of the controller tooltips.
/// </summary>
public class ToolTipController : MonoBehaviour {

    /// <summary>
    /// Gives the gameobject to the mainmenu holder to allow it to maniplate this gameobject.
    /// </summary>
    void Awake ()
    {
        GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().TutorialObjs.Add(this.gameObject);
    }

    /// <summary>
    /// Forces the tooltips to turn off in the main menu.
    /// </summary>
    void OnEnable()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0 || UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1)
        {
            this.gameObject.SetActive(false);
        }
    }
}
