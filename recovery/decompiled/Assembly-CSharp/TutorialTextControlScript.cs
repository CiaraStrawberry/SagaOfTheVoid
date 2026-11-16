using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTextControlScript : MonoBehaviour
{
	public enum tutorialstateinput
	{
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

	public enum eControllerPart
	{
		lefttouchpad,
		lefttrigger,
		leftgrip,
		leftmenu,
		rightTouchPad,
		rightTrigger,
		rightGrip,
		rightMenu
	}

	public TextMesh tutorialText;

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

	private tutorialstateinput curstate;

	private void Start()
	{
		tutorialText.text = startstring;
		List<GameObject> tutorialObjs = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().TutorialObjs;
		foreach (GameObject item in tutorialObjs)
		{
			bool active = item.GetActive();
			item.SetActive(value: true);
			if (item.transform.parent.name == "Controller (right)" || item.transform.parent.name == "RightHandAnchor")
			{
				RightTouchpad = item.transform.Find("TouchPadTooltip").gameObject;
				RightTrigger = item.transform.Find("TriggerTooltip").gameObject;
				RightMenu = item.transform.Find("MenuToolTip").gameObject;
				RightGrip = item.transform.Find("GripToolTip").gameObject;
			}
			else
			{
				LeftTouchPad = item.transform.Find("TouchPadTooltip").gameObject;
				LeftTrigger = item.transform.Find("TriggerTooltip").gameObject;
				LeftMenu = item.transform.Find("MenuToolTip").gameObject;
				LeftGrip = item.transform.Find("GripToolTip").gameObject;
			}
			if (item.transform.parent.name == "RightHandAnchor")
			{
				rightrotate = item.transform.Find("Rotate tooltip").gameObject;
			}
			if (item.transform.parent.name == "LeftHandAnchor")
			{
				leftrotate = item.transform.Find("Rotate tooltip").gameObject;
			}
			item.SetActive(active);
		}
	}

	public void MoveToNext1(tutorialstateinput input)
	{
		switch (input)
		{
		case tutorialstateinput.LeftTouchpadPressed:
			setlevel(tutorialstateinput.LeftTouchpadPressed, LeftDragInstruction, tutorialstateinput.LeftTriggerDown, eControllerPart.lefttrigger);
			break;
		case tutorialstateinput.LeftTriggerDown:
			setlevel(tutorialstateinput.LeftTriggerDown, LeftGripInstruction, tutorialstateinput.leftGripDown, eControllerPart.leftgrip);
			break;
		case tutorialstateinput.leftGripDown:
			setlevel(tutorialstateinput.leftGripDown, LeftMenuInstructions, tutorialstateinput.leftMenuDown, eControllerPart.leftmenu);
			break;
		case tutorialstateinput.leftMenuDown:
			setlevel(tutorialstateinput.leftMenuDown, RightSelectallInstructions, tutorialstateinput.RightGripSelectall, eControllerPart.rightGrip);
			break;
		case tutorialstateinput.RightGripSelectall:
			setlevel(tutorialstateinput.RightGripSelectall, RightDeSelectallInstructions, tutorialstateinput.RightGripdeselectall, eControllerPart.rightGrip);
			break;
		case tutorialstateinput.RightGripdeselectall:
			setlevel(tutorialstateinput.RightGripdeselectall, RightSelectionLineInstructions, tutorialstateinput.RightTouchpadtouch, eControllerPart.rightTouchPad);
			break;
		case tutorialstateinput.RightTouchpadtouch:
			setlevel(tutorialstateinput.RightTouchpadtouch, RightOrderInstructions, tutorialstateinput.RightTrigger, eControllerPart.rightTouchPad);
			break;
		case tutorialstateinput.RightTrigger:
			setlevel(tutorialstateinput.RightTrigger, GiveAttackOrderInstructions, tutorialstateinput.RightAttackOrder, eControllerPart.rightTrigger);
			break;
		case tutorialstateinput.RightAttackOrder:
			setlevel(tutorialstateinput.RightAttackOrder, RightMenuInstructions, tutorialstateinput.RightMenu, eControllerPart.rightMenu);
			break;
		case tutorialstateinput.RightMenu:
			setlevel(tutorialstateinput.RightMenu, RightBuyShipInstructions, tutorialstateinput.RightMenuShipBuy, eControllerPart.rightMenu);
			break;
		case tutorialstateinput.RightMenuShipBuy:
			setlevel(tutorialstateinput.RightMenuShipBuy, RightGiveWayPointInstructions, tutorialstateinput.RightTriggerGiveWayPoint, eControllerPart.rightTouchPad);
			break;
		case tutorialstateinput.RightTriggerGiveWayPoint:
			setlevel(tutorialstateinput.RightTriggerGiveWayPoint, RightGiveLongAttackOrderInstructions, tutorialstateinput.RightTriggerGiveLongAttackOrder, eControllerPart.rightTouchPad);
			break;
		case tutorialstateinput.RightTriggerGiveLongAttackOrder:
			StartCoroutine("LeaveGame");
			break;
		}
	}

	public void MoveToNext(tutorialstateinput input)
	{
	}

	private void setlevel(tutorialstateinput inputstate, string inputstring, tutorialstateinput nextstate, eControllerPart inputpart)
	{
		if (inputstate == curstate + 1)
		{
			tutorialText.text = inputstring;
			curstate = inputstate;
			showinput(inputpart);
		}
	}

	private void setnextlevel()
	{
		int num = (int)curstate;
		MoveToNext1(curstate + 1);
	}

	private void showinput(eControllerPart inputpart)
	{
		disableall();
		RightTouchpad.transform.parent.gameObject.SetActive(value: true);
		LeftTouchPad.transform.parent.gameObject.SetActive(value: true);
		if (inputpart == eControllerPart.rightTouchPad)
		{
			RightTouchpad.SetActive(value: true);
		}
		if (inputpart == eControllerPart.rightTrigger)
		{
			RightTrigger.SetActive(value: true);
		}
		if (inputpart == eControllerPart.rightGrip)
		{
			RightGrip.SetActive(value: true);
		}
		if (inputpart == eControllerPart.rightMenu)
		{
			RightMenu.SetActive(value: true);
		}
		if (inputpart == eControllerPart.lefttouchpad)
		{
			LeftTouchPad.SetActive(value: true);
		}
		if (inputpart == eControllerPart.lefttrigger)
		{
			LeftTrigger.SetActive(value: true);
		}
		if (inputpart == eControllerPart.leftmenu)
		{
			LeftMenu.SetActive(value: true);
		}
		if (inputpart == eControllerPart.leftgrip)
		{
			LeftGrip.SetActive(value: true);
		}
	}

	private void disableall()
	{
		if ((bool)RightTouchpad)
		{
			leftrotate.gameObject.SetActive(value: false);
			rightrotate.gameObject.SetActive(value: false);
			RightTouchpad.SetActive(value: false);
			RightTrigger.SetActive(value: false);
			RightGrip.SetActive(value: false);
			RightMenu.SetActive(value: false);
			LeftTouchPad.SetActive(value: false);
			LeftTrigger.SetActive(value: false);
			LeftMenu.SetActive(value: false);
			LeftGrip.SetActive(value: false);
		}
	}

	private IEnumerator LeaveGame()
	{
		yield return new WaitForSeconds(3f);
		PhotonNetwork.LoadLevel(0);
	}
}
