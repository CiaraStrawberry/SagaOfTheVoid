using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

/// <summary>
/// This class allows the players controller to interact with the menu through VRTK
/// </summary>
public class MainMenuControllerController : MonoBehaviour
{
    // the class that sends the message that the player has performed an action.
    private VRTK_ControllerEvents eventscon;
    // the linerenderer that shoots out from teh players controller.
    LineRenderer lineren;
    // the object that the players controller is currently pointing out.
    public ButtonPressedMessageSender hitobj;
    // the haptic clip that is currently asigned to play when the players controller hovers over something.
    public AudioClip audioahptic;
    // the last object the player pointed at.
    public GameObject lastobj;
    // the custom script attached to the players right hand.
    private RightHand_triggerInstantOrder righthandinstant;
    // the amount of time til the haptic clip can played again.
    private int framestiilnexthaptic;
    // the minimum time between the player switching objects that a haptic can be played.
    // this prevents the controller from buzzing really quickly if the player moves alot.
    private int minhaplimit = 60;
    // the vive controller object.
    public GameObject ViveObject;
    // the haptic origin to play the clip from.
    private AudioSource audio;
    // the crosslevelvariable holder singleton.
    CrossLevelVariableHolder crossvar;
    
    /// <summary>
    ///  the class initialisation.
    /// </summary>
    void Start()
    {
        eventscon = GetComponent<VRTK_ControllerEvents>();
        eventscon.TriggerPressed += new ControllerInteractionEventHandler(trigger);
        lineren = GetComponent<LineRenderer>();
        lineren.startWidth = 0.002f;
        lineren.endWidth = 0.002f;
        lineren.enabled = false;
        righthandinstant = GetComponent<RightHand_triggerInstantOrder>();
        audio = GetComponent<AudioSource>();
    }
    
    /// <summary>
    /// the update function that performes the haptics and menu interactions.
    /// </summary>
    void Update()
    {
        
        framestiilnexthaptic++;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000))
        {
            ButtonPressedMessageSender hitscripttemp = hit.transform.gameObject.GetComponent<ButtonPressedMessageSender>();
            if (hitscripttemp != null)
            {
                if (hit.collider.gameObject != lastobj)
                {
                    TriggerHapticPulse();
                    lastobj = hit.collider.gameObject;
                }
                lineren.enabled = true;
                lineren.SetPosition(0, transform.position);
                lineren.SetPosition(1, hit.point);
                hitobj = hitscripttemp;
                ButtonHoverMessageSender hovertemp = hit.transform.gameObject.GetComponent<ButtonHoverMessageSender>();
                if (hovertemp != null) hovertemp.SendMessage();
            }
            else disablelineren();
        }
        else
        {
            hitobj = null;
            disablelineren();
        }

        
    }

    /// <summary>
    /// the function to play a haptic pulse if the player isnt using a vive. (haptics were broken on the vive) TODO: find out which the vive haptics sound wrong.
    /// </summary>
    void TriggerHapticPulse ()
    {
       Debug.Log("hapticpulse");
        if(ViveObject == null ||  ViveObject.GetActive() == false)
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(VRTK_DeviceFinder.GetControllerIndex(this.gameObject)), audioahptic);
        }
    }

    /// <summary>
    /// turn of the players pointer.
    /// </summary>
    void disablelineren()
    {
        if (this != null && this.gameObject != null && (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0 || UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6))
            lineren.enabled = false;
    }

    /// <summary>
    ///  the function that plays when the player pulls the trigger.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void trigger(object sender, ControllerInteractionEventArgs e)
    {
        if (hitobj && hitobj.gameObject != null)
        {
            audio.Play();
            hitobj.sendmessage();
        }
    }
}
