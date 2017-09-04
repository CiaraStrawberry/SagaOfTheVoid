using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;
public class MainMenuControllerController : MonoBehaviour
{
    private VRTK_ControllerEvents eventscon;
    LineRenderer lineren;
    public ButtonPressedMessageSender hitobj;
    public AudioClip audioahptic;
    public GameObject lastobj;
    private RightHand_triggerInstantOrder righthandinstant;
    private int framestiilnexthaptic;
    private int minhaplimit = 60;
    public GameObject ViveObject;
    private AudioSource audio;
    CrossLevelVariableHolder crossvar;
    // Use this for initialization
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
    
    // Update is called once per frame
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
    void TriggerHapticPulse ()
    {
       Debug.Log("hapticpulse");
        if(ViveObject == null ||  ViveObject.GetActive() == false)
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(VRTK_DeviceFinder.GetControllerIndex(this.gameObject)), audioahptic);
        }
    }
    void disablelineren()
    {
        if (this != null && this.gameObject != null && (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0 || UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6))
        {
            lineren.enabled = false;
        }

    }
    void trigger(object sender, ControllerInteractionEventArgs e)
    {

        if (hitobj && hitobj.gameObject != null)
        {
            audio.Play();
            hitobj.sendmessage();
        }
    }
}
