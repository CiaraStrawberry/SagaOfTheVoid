using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the tutorial controller script, it allows the player to look through the games instruction on how to play.
/// </summary>
public class TutorialTextControlScript : MonoBehaviour {

    // this is the text mesh containing the data.
    public TextMesh tutorialText;


    /// <summary>
    ///  this enum contains all the possible tutorial states.
    /// </summary>
    public enum tutorialstateinput {
        startstate,
        LeftTouchpadPressed,
        LeftTriggerDown,
        leftGripDown,
        leftMenuDown,
        RightGripSelectall,
        RightGripdeselectall,
        RightTouchpadtouch,
        RightTrigger,
        RightAttackOrder,
        RightMenu,
        RightMenuShipBuy,
        RightTriggerGiveWayPoint,
        RightTriggerGiveLongAttackOrder
    }

    /// <summary>
    /// This enum shows all the controller parts the player could use.
    /// </summary>
    public enum eControllerPart {
        lefttouchpad,
        lefttrigger,
        leftgrip,
        leftmenu,
        rightTouchPad,
        rightTrigger,
        rightGrip,
        rightMenu
    }

    // these variables hold the strings giving the instructions to the player.

    [TextArea(3, 10)]
    public string startstring;
    [TextArea(3, 10)]
    public string LeftDragInstruction;
    [TextArea(3, 10)]
    public string LeftGripInstruction;
    [TextArea(3, 10)]
    public string LeftMenuInstructions;
    [TextArea(3, 10)]
    public string RightSelectallInstructions;
    [TextArea(3, 10)]
    public string RightDeSelectallInstructions;
    [TextArea(3, 10)]
    public string RightSelectionLineInstructions;
    [TextArea(3, 10)]
    public string RightOrderInstructions;
    [TextArea(3, 10)]
    public string GiveAttackOrderInstructions;
    [TextArea(3, 10)]
    public string RightMenuInstructions;
    [TextArea(3, 10)]
    public string RightBuyShipInstructions;
    [TextArea(3, 10)]
    public string RightGiveWayPointInstructions;
    [TextArea(3, 10)]
    public string RightGiveLongAttackOrderInstructions;

    // these gameobjects are things for the tooltips to point to.

    public GameObject RightTouchpad;
    public GameObject RightTrigger;
    public GameObject RightGrip;
    public GameObject RightMenu;
    public GameObject LeftTouchPad;
    public GameObject LeftTrigger;
    public GameObject LeftMenu;
    public GameObject LeftGrip;
    public GameObject rightrotate;
    public GameObject leftrotate;

    // this is the current state of the tutorial enum.
    tutorialstateinput curstate = tutorialstateinput.startstate;

    /// <summary>
    /// initialise everything to default state.
    /// </summary>
    void Start () {
        tutorialText.text = startstring;
        List<GameObject> tutobjs = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().TutorialObjs;
        foreach(GameObject gams in tutobjs)
        {
            bool origin = gams.GetActive();
            gams.SetActive(true);
            if(gams.transform.parent.name == "Controller (right)"|| gams.transform.parent.name == "RightHandAnchor")
            {
                RightTouchpad = gams.transform.Find("TouchPadTooltip").gameObject;
                RightTrigger = gams.transform.Find("TriggerTooltip").gameObject;
                RightMenu = gams.transform.Find("MenuToolTip").gameObject;
                RightGrip = gams.transform.Find("GripToolTip").gameObject;
            }
            else
            {
                LeftTouchPad = gams.transform.Find("TouchPadTooltip").gameObject;
                LeftTrigger = gams.transform.Find("TriggerTooltip").gameObject;
                LeftMenu = gams.transform.Find("MenuToolTip").gameObject;
                LeftGrip = gams.transform.Find("GripToolTip").gameObject;
            }
            if(gams.transform.parent.name == "RightHandAnchor") rightrotate = gams.transform.Find("Rotate tooltip").gameObject;
            if (gams.transform.parent.name == "LeftHandAnchor")   leftrotate = gams.transform.Find("Rotate tooltip").gameObject;
            gams.SetActive(origin);
        }
    }

    /// <summary>
    ///  This function moves to inputted tutorial state.
    /// </summary>
    /// <param name="input"></param>
    public void MoveToNext(tutorialstateinput input)
    {
        switch (input)
        {

            case tutorialstateinput.LeftTouchpadPressed: setlevel(tutorialstateinput.LeftTouchpadPressed, LeftDragInstruction, tutorialstateinput.LeftTriggerDown, eControllerPart.lefttrigger); break;
            case tutorialstateinput.LeftTriggerDown: setlevel(tutorialstateinput.LeftTriggerDown, LeftGripInstruction, tutorialstateinput.leftGripDown, eControllerPart.leftgrip); break;
            case tutorialstateinput.leftGripDown: setlevel(tutorialstateinput.leftGripDown, LeftMenuInstructions, tutorialstateinput.leftMenuDown, eControllerPart.leftmenu); break;
            case tutorialstateinput.leftMenuDown: setlevel(tutorialstateinput.leftMenuDown, RightSelectallInstructions, tutorialstateinput.RightGripSelectall, eControllerPart.rightGrip); break;
            case tutorialstateinput.RightGripSelectall: setlevel(tutorialstateinput.RightGripSelectall, RightDeSelectallInstructions, tutorialstateinput.RightGripdeselectall, eControllerPart.rightGrip); break;
            case tutorialstateinput.RightGripdeselectall: setlevel(tutorialstateinput.RightGripdeselectall, RightSelectionLineInstructions, tutorialstateinput.RightTouchpadtouch, eControllerPart.rightTouchPad); break;

            case tutorialstateinput.RightTouchpadtouch: setlevel(tutorialstateinput.RightTouchpadtouch, RightOrderInstructions, tutorialstateinput.RightTrigger, eControllerPart.rightTouchPad); break;
            case tutorialstateinput.RightTrigger: setlevel(tutorialstateinput.RightTrigger, GiveAttackOrderInstructions, tutorialstateinput.RightAttackOrder, eControllerPart.rightTrigger); break;
            case tutorialstateinput.RightAttackOrder: setlevel(tutorialstateinput.RightAttackOrder, RightMenuInstructions, tutorialstateinput.RightMenu, eControllerPart.rightMenu); break;
            case tutorialstateinput.RightMenu: setlevel(tutorialstateinput.RightMenu, RightBuyShipInstructions, tutorialstateinput.RightMenuShipBuy, eControllerPart.rightMenu); break;
            case tutorialstateinput.RightMenuShipBuy: setlevel(tutorialstateinput.RightMenuShipBuy, RightGiveWayPointInstructions, tutorialstateinput.RightTriggerGiveWayPoint, eControllerPart.rightTouchPad); break;
            case tutorialstateinput.RightTriggerGiveWayPoint: setlevel(tutorialstateinput.RightTriggerGiveWayPoint, RightGiveLongAttackOrderInstructions, tutorialstateinput.RightTriggerGiveLongAttackOrder, eControllerPart.rightTouchPad); break;

            case tutorialstateinput.RightTriggerGiveLongAttackOrder: StartCoroutine("LeaveGame"); break;
        }
    }

    /// <summary>
    /// This allows you to set the tutorial state manually.
    /// </summary>
    /// <param name="inputstate"></param>
    /// <param name="inputstring"></param>
    /// <param name="nextstate"></param>
    /// <param name="inputpart"></param>
    void setlevel (tutorialstateinput inputstate,string inputstring,tutorialstateinput nextstate,eControllerPart inputpart)
    {
        if((int)inputstate == (int)curstate + 1)
        {
           tutorialText.text = inputstring;
            curstate = inputstate;
            showinput(inputpart);
        }
    }

    /// <summary>
    /// This moves to teh next tutorial state in the list.
    /// </summary>
    void setnextlevel ()
    {
        int temp = (int)curstate;
        MoveToNext1((tutorialstateinput)(curstate + 1));
    }

    /// <summary>
    /// shows all tooltip pointers.
    /// </summary>
    /// <param name="inputpart"></param>
    void showinput (eControllerPart inputpart)
    {
        disableall();
        RightTouchpad.transform.parent.gameObject.SetActive(true);
        LeftTouchPad.transform.parent.gameObject.SetActive(true);
        if (inputpart == eControllerPart.rightTouchPad) RightTouchpad.SetActive(true);
        if (inputpart == eControllerPart.rightTrigger) RightTrigger.SetActive(true);
        if (inputpart == eControllerPart.rightGrip) RightGrip.SetActive(true);
        if (inputpart == eControllerPart.rightMenu) RightMenu.SetActive(true);
        if (inputpart == eControllerPart.lefttouchpad) LeftTouchPad.SetActive(true);
        if (inputpart == eControllerPart.lefttrigger) LeftTrigger.SetActive(true);
        if (inputpart == eControllerPart.leftmenu) LeftMenu.SetActive(true);
        if (inputpart == eControllerPart.leftgrip) LeftGrip.SetActive(true);
    }
    
    /// <summary>
    /// Disables all tooltip pointers.
    /// </summary>
    void disableall()
    {
        if (RightTouchpad)
        {
            leftrotate.gameObject.SetActive(false);
            rightrotate.gameObject.SetActive(false);
            RightTouchpad.SetActive(false);
            RightTrigger.SetActive(false);
            RightGrip.SetActive(false);
            RightMenu.SetActive(false);
            LeftTouchPad.SetActive(false);
            LeftTrigger.SetActive(false);
            LeftMenu.SetActive(false);
            LeftGrip.SetActive(false);
        }
    
    }


    /// <summary>
    /// Ends the game and returns to the main menu.
    /// </summary>
    /// <returns></returns>
    IEnumerator LeaveGame ()
    {
        yield return new WaitForSeconds(3);
        PhotonNetwork.LoadLevel(0);
    }
}
