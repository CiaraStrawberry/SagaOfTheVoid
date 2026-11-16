using UnityEngine;
using UnityEngine.UI;

namespace VRTK;

public class SDK_InputSimulator : MonoBehaviour
{
	public enum MouseInputMode
	{
		Always,
		RequiresButtonPress
	}

	[Tooltip("Show control information in the upper left corner of the screen.")]
	public bool showControlHints = true;

	[Tooltip("Hide hands when disabling them.")]
	public bool hideHandsAtSwitch;

	[Tooltip("Reset hand position and rotation when enabling them.")]
	public bool resetHandsAtSwitch = true;

	[Tooltip("Whether mouse movement always acts as input or requires a button press.")]
	public MouseInputMode mouseMovementInput;

	[Tooltip("Lock the mouse cursor to the game window when the mouse movement key is pressed.")]
	public bool lockMouseToView = true;

	[Header("Adjustments")]
	[Tooltip("Adjust hand movement speed.")]
	public float handMoveMultiplier = 0.002f;

	[Tooltip("Adjust hand rotation speed.")]
	public float handRotationMultiplier = 0.5f;

	[Tooltip("Adjust player movement speed.")]
	public float playerMoveMultiplier = 5f;

	[Tooltip("Adjust player rotation speed.")]
	public float playerRotationMultiplier = 0.5f;

	[Header("Operation Key Bindings")]
	[Tooltip("Key used to enable mouse input if a button press is required.")]
	public KeyCode mouseMovementKey = KeyCode.Mouse1;

	[Tooltip("Key used to toggle control hints on/off.")]
	public KeyCode toggleControlHints = KeyCode.F1;

	[Tooltip("Key used to switch between left and righ hand.")]
	public KeyCode changeHands = KeyCode.Tab;

	[Tooltip("Key used to switch hands On/Off.")]
	public KeyCode handsOnOff = KeyCode.LeftAlt;

	[Tooltip("Key used to switch between positional and rotational movement.")]
	public KeyCode rotationPosition = KeyCode.LeftShift;

	[Tooltip("Key used to switch between X/Y and X/Z axis.")]
	public KeyCode changeAxis = KeyCode.LeftControl;

	[Header("Movement Key Bindings")]
	[Tooltip("Key used to move forward.")]
	public KeyCode moveForward = KeyCode.W;

	[Tooltip("Key used to move to the left.")]
	public KeyCode moveLeft = KeyCode.A;

	[Tooltip("Key used to move backwards.")]
	public KeyCode moveBackward = KeyCode.S;

	[Tooltip("Key used to move to the right.")]
	public KeyCode moveRight = KeyCode.D;

	[Header("Controller Key Bindings")]
	[Tooltip("Key used to simulate trigger button.")]
	public KeyCode triggerAlias = KeyCode.Mouse1;

	[Tooltip("Key used to simulate grip button.")]
	public KeyCode gripAlias = KeyCode.Mouse0;

	[Tooltip("Key used to simulate touchpad button.")]
	public KeyCode touchpadAlias = KeyCode.Q;

	[Tooltip("Key used to simulate button one.")]
	public KeyCode buttonOneAlias = KeyCode.E;

	[Tooltip("Key used to simulate button two.")]
	public KeyCode buttonTwoAlias = KeyCode.R;

	[Tooltip("Key used to simulate start menu button.")]
	public KeyCode startMenuAlias = KeyCode.F;

	[Tooltip("Key used to switch between button touch and button press mode.")]
	public KeyCode touchModifier = KeyCode.T;

	[Tooltip("Key used to switch between hair touch mode.")]
	public KeyCode hairTouchModifier = KeyCode.H;

	private bool isHand;

	private GameObject hintCanvas;

	private Text hintText;

	private Transform rightHand;

	private Transform leftHand;

	private Transform currentHand;

	private Vector3 oldPos;

	private Transform neck;

	private SDK_ControllerSim rightController;

	private SDK_ControllerSim leftController;

	private static GameObject cachedCameraRig;

	private static bool destroyed;
}
