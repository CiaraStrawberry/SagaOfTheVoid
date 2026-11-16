using UnityEngine.VR;

public class OvrAvatarLocalDriver : OvrAvatarDriver
{
	private float voiceAmplitude;

	private ControllerPose GetControllerPose(OVRInput.Controller controller)
	{
		ovrAvatarButton ovrAvatarButton2 = (ovrAvatarButton)0;
		if (OVRInput.Get(OVRInput.Button.One, controller))
		{
			ovrAvatarButton2 |= ovrAvatarButton.One;
		}
		if (OVRInput.Get(OVRInput.Button.Two, controller))
		{
			ovrAvatarButton2 |= ovrAvatarButton.Two;
		}
		if (OVRInput.Get(OVRInput.Button.Start, controller))
		{
			ovrAvatarButton2 |= ovrAvatarButton.Three;
		}
		if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick, controller))
		{
			ovrAvatarButton2 |= ovrAvatarButton.Joystick;
		}
		ovrAvatarTouch ovrAvatarTouch2 = (ovrAvatarTouch)0;
		if (OVRInput.Get(OVRInput.Touch.One, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.One;
		}
		if (OVRInput.Get(OVRInput.Touch.Two, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.Two;
		}
		if (OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.Joystick;
		}
		if (OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.ThumbRest;
		}
		if (OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.Index;
		}
		if (!OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.Pointing;
		}
		if (!OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, controller))
		{
			ovrAvatarTouch2 |= ovrAvatarTouch.ThumbUp;
		}
		return new ControllerPose
		{
			buttons = ovrAvatarButton2,
			touches = ovrAvatarTouch2,
			joystickPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller),
			indexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller),
			handTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller),
			isActive = ((OVRInput.GetActiveController() & controller) != 0)
		};
	}

	private void Start()
	{
	}

	public override bool GetCurrentPose(out PoseFrame pose)
	{
		pose = new PoseFrame
		{
			voiceAmplitude = voiceAmplitude,
			headPosition = InputTracking.GetLocalPosition(VRNode.CenterEye),
			headRotation = InputTracking.GetLocalRotation(VRNode.CenterEye),
			handLeftPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch),
			handLeftRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch),
			handRightPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch),
			handRightRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch),
			controllerLeftPose = GetControllerPose(OVRInput.Controller.LTouch),
			controllerRightPose = GetControllerPose(OVRInput.Controller.RTouch)
		};
		return true;
	}
}
