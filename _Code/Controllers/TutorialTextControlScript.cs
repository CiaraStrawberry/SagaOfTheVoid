using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTextControlScript : MonoBehaviour {
    public TextMesh tutorialText;

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

    // Use this for initialization
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
            if(gams.transform.parent.name == "RightHandAnchor")
            {
                rightrotate = gams.transform.Find("Rotate tooltip").gameObject;
            }
            if (gams.transform.parent.name == "LeftHandAnchor")
            {
                leftrotate = gams.transform.Find("Rotate tooltip").gameObject;
            }
            gams.SetActive(origin);
        }
    }

    // Update is called once per frame
   public void MoveToNext1(tutorialstateinput input)
    {
        switch (input) {

            case tutorialstateinput.LeftTouchpadPressed:setlevel(tutorialstateinput.LeftTouchpadPressed, LeftDragInstruction, tutorialstateinput.LeftTriggerDown,eControllerPart.lefttrigger); break;
            case tutorialstateinput.LeftTriggerDown: setlevel(tutorialstateinput.LeftTriggerDown, LeftGripInstruction, tutorialstateinput.leftGripDown, eControllerPart.leftgrip);  break;
            case tutorialstateinput.leftGripDown: setlevel(tutorialstateinput.leftGripDown, LeftMenuInstructions, tutorialstateinput.leftMenuDown, eControllerPart.leftmenu);  break;
            case tutorialstateinput.leftMenuDown: setlevel(tutorialstateinput.leftMenuDown, RightSelectallInstructions, tutorialstateinput.RightGripSelectall, eControllerPart.rightGrip);  break;
            case tutorialstateinput.RightGripSelectall: setlevel(tutorialstateinput.RightGripSelectall, RightDeSelectallInstructions, tutorialstateinput.RightGripdeselectall, eControllerPart.rightGrip); break;
            case tutorialstateinput.RightGripdeselectall: setlevel(tutorialstateinput.RightGripdeselectall, RightSelectionLineInstructions, tutorialstateinput.RightTouchpadtouch, eControllerPart.rightTouchPad);  break;

            case tutorialstateinput.RightTouchpadtouch: setlevel(tutorialstateinput.RightTouchpadtouch, RightOrderInstructions, tutorialstateinput.RightTrigger, eControllerPart.rightTouchPad); break;
            case tutorialstateinput.RightTrigger: setlevel(tutorialstateinput.RightTrigger, GiveAttackOrderInstructions, tutorialstateinput.RightAttackOrder, eControllerPart.rightTrigger);  break;
            case tutorialstateinput.RightAttackOrder: setlevel(tutorialstateinput.RightAttackOrder, RightMenuInstructions, tutorialstateinput.RightMenu, eControllerPart.rightMenu); break;
            case tutorialstateinput.RightMenu: setlevel(tutorialstateinput.RightMenu, RightBuyShipInstructions, tutorialstateinput.RightMenuShipBuy, eControllerPart.rightMenu);  break;
            case tutorialstateinput.RightMenuShipBuy: setlevel(tutorialstateinput.RightMenuShipBuy, RightGiveWayPointInstructions, tutorialstateinput.RightTriggerGiveWayPoint, eControllerPart.rightTouchPad);  break;
            case tutorialstateinput.RightTriggerGiveWayPoint: setlevel(tutorialstateinput.RightTriggerGiveWayPoint, RightGiveLongAttackOrderInstructions, tutorialstateinput.RightTriggerGiveLongAttackOrder, eControllerPart.rightTouchPad); break;

            case tutorialstateinput.RightTriggerGiveLongAttackOrder: StartCoroutine("LeaveGame"); break;
        }
    }
    public void MoveToNext(tutorialstateinput input)
    {
     
    }
    tutorialstateinput curstate = tutorialstateinput.startstate;
    void setlevel (tutorialstateinput inputstate,string inputstring,tutorialstateinput nextstate,eControllerPart inputpart)
    {
        if((int)inputstate == (int)curstate + 1)
        {
           tutorialText.text = inputstring;
            curstate = inputstate;
         showinput(inputpart);
        }
      

    }
    void setnextlevel ()
    {
        int temp = (int)curstate;
        MoveToNext1((tutorialstateinput)(curstate + 1));
    }
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
    IEnumerator LeaveGame ()
    {
        yield return new WaitForSeconds(3);
        PhotonNetwork.LoadLevel(0);
    }
}
