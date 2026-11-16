using System;
using UnityEngine;
using UnityEngine.VR;

[ExecuteInEditMode]
public class OVRCameraRig : MonoBehaviour
{
	public bool usePerEyeCameras;

	public bool useFixedUpdateForTracking;

	private bool _skipUpdate;

	private readonly string trackingSpaceName = "TrackingSpace";

	private readonly string trackerAnchorName = "TrackerAnchor";

	private readonly string eyeAnchorName = "EyeAnchor";

	private readonly string handAnchorName = "HandAnchor";

	private readonly string legacyEyeAnchorName = "Camera";

	private Camera _centerEyeCamera;

	private Camera _leftEyeCamera;

	private Camera _rightEyeCamera;

	public Camera leftEyeCamera => (!usePerEyeCameras) ? _centerEyeCamera : _leftEyeCamera;

	public Camera rightEyeCamera => (!usePerEyeCameras) ? _centerEyeCamera : _rightEyeCamera;

	public Transform trackingSpace { get; private set; }

	public Transform leftEyeAnchor { get; private set; }

	public Transform centerEyeAnchor { get; private set; }

	public Transform rightEyeAnchor { get; private set; }

	public Transform leftHandAnchor { get; private set; }

	public Transform rightHandAnchor { get; private set; }

	public Transform trackerAnchor { get; private set; }

	public event Action<OVRCameraRig> UpdatedAnchors;

	private void Awake()
	{
		_skipUpdate = true;
		EnsureGameObjectIntegrity();
	}

	private void Start()
	{
		UpdateAnchors();
	}

	private void FixedUpdate()
	{
		if (useFixedUpdateForTracking)
		{
			UpdateAnchors();
		}
	}

	private void Update()
	{
		_skipUpdate = false;
		if (!useFixedUpdateForTracking)
		{
			UpdateAnchors();
		}
	}

	private void UpdateAnchors()
	{
		EnsureGameObjectIntegrity();
		if (!Application.isPlaying)
		{
			return;
		}
		if (_skipUpdate)
		{
			centerEyeAnchor.FromOVRPose(OVRPose.identity, isLocal: true);
			leftEyeAnchor.FromOVRPose(OVRPose.identity, isLocal: true);
			rightEyeAnchor.FromOVRPose(OVRPose.identity, isLocal: true);
			return;
		}
		bool monoscopic = OVRManager.instance.monoscopic;
		OVRPose pose = OVRManager.tracker.GetPose();
		trackerAnchor.localRotation = pose.orientation;
		centerEyeAnchor.localRotation = InputTracking.GetLocalRotation(VRNode.CenterEye);
		leftEyeAnchor.localRotation = ((!monoscopic) ? InputTracking.GetLocalRotation(VRNode.LeftEye) : centerEyeAnchor.localRotation);
		rightEyeAnchor.localRotation = ((!monoscopic) ? InputTracking.GetLocalRotation(VRNode.RightEye) : centerEyeAnchor.localRotation);
		leftHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
		rightHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
		trackerAnchor.localPosition = pose.position;
		centerEyeAnchor.localPosition = InputTracking.GetLocalPosition(VRNode.CenterEye);
		leftEyeAnchor.localPosition = ((!monoscopic) ? InputTracking.GetLocalPosition(VRNode.LeftEye) : centerEyeAnchor.localPosition);
		rightEyeAnchor.localPosition = ((!monoscopic) ? InputTracking.GetLocalPosition(VRNode.RightEye) : centerEyeAnchor.localPosition);
		leftHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
		rightHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
		if (UpdatedAnchors != null)
		{
			UpdatedAnchors(this);
		}
	}

	public void EnsureGameObjectIntegrity()
	{
		if (trackingSpace == null)
		{
			trackingSpace = ConfigureRootAnchor(trackingSpaceName);
		}
		if (leftEyeAnchor == null)
		{
			leftEyeAnchor = ConfigureEyeAnchor(trackingSpace, VRNode.LeftEye);
		}
		if (centerEyeAnchor == null)
		{
			centerEyeAnchor = ConfigureEyeAnchor(trackingSpace, VRNode.CenterEye);
		}
		if (rightEyeAnchor == null)
		{
			rightEyeAnchor = ConfigureEyeAnchor(trackingSpace, VRNode.RightEye);
		}
		if (leftHandAnchor == null)
		{
			leftHandAnchor = ConfigureHandAnchor(trackingSpace, OVRPlugin.Node.HandLeft);
		}
		if (rightHandAnchor == null)
		{
			rightHandAnchor = ConfigureHandAnchor(trackingSpace, OVRPlugin.Node.HandRight);
		}
		if (trackerAnchor == null)
		{
			trackerAnchor = ConfigureTrackerAnchor(trackingSpace);
		}
		if (_centerEyeCamera == null || _leftEyeCamera == null || _rightEyeCamera == null)
		{
			_centerEyeCamera = centerEyeAnchor.GetComponent<Camera>();
			_leftEyeCamera = leftEyeAnchor.GetComponent<Camera>();
			_rightEyeCamera = rightEyeAnchor.GetComponent<Camera>();
			if (_centerEyeCamera == null)
			{
				_centerEyeCamera = centerEyeAnchor.gameObject.AddComponent<Camera>();
				_centerEyeCamera.tag = "MainCamera";
			}
			if (_leftEyeCamera == null)
			{
				_leftEyeCamera = leftEyeAnchor.gameObject.AddComponent<Camera>();
				_leftEyeCamera.tag = "MainCamera";
			}
			if (_rightEyeCamera == null)
			{
				_rightEyeCamera = rightEyeAnchor.gameObject.AddComponent<Camera>();
				_rightEyeCamera.tag = "MainCamera";
			}
			_centerEyeCamera.stereoTargetEye = StereoTargetEyeMask.Both;
			_leftEyeCamera.stereoTargetEye = StereoTargetEyeMask.Left;
			_rightEyeCamera.stereoTargetEye = StereoTargetEyeMask.Right;
		}
		if (_centerEyeCamera.enabled == usePerEyeCameras || _leftEyeCamera.enabled == !usePerEyeCameras || _rightEyeCamera.enabled == !usePerEyeCameras)
		{
			_skipUpdate = true;
		}
		_centerEyeCamera.enabled = !usePerEyeCameras;
		_leftEyeCamera.enabled = usePerEyeCameras;
		_rightEyeCamera.enabled = usePerEyeCameras;
	}

	private Transform ConfigureRootAnchor(string name)
	{
		Transform transform = base.transform.Find(name);
		if (transform == null)
		{
			transform = new GameObject(name).transform;
		}
		transform.parent = base.transform;
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}

	private Transform ConfigureEyeAnchor(Transform root, VRNode eye)
	{
		string text = eye switch
		{
			VRNode.CenterEye => "Center", 
			VRNode.LeftEye => "Left", 
			_ => "Right", 
		} + eyeAnchorName;
		Transform transform = base.transform.Find(root.name + "/" + text);
		if (transform == null)
		{
			transform = base.transform.Find(text);
		}
		if (transform == null)
		{
			string text2 = legacyEyeAnchorName + eye;
			transform = base.transform.Find(text2);
		}
		if (transform == null)
		{
			transform = new GameObject(text).transform;
		}
		transform.name = text;
		transform.parent = root;
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}

	private Transform ConfigureHandAnchor(Transform root, OVRPlugin.Node hand)
	{
		string text = ((hand != OVRPlugin.Node.HandLeft) ? "Right" : "Left");
		string text2 = text + handAnchorName;
		Transform transform = base.transform.Find(root.name + "/" + text2);
		if (transform == null)
		{
			transform = base.transform.Find(text2);
		}
		if (transform == null)
		{
			transform = new GameObject(text2).transform;
		}
		transform.name = text2;
		transform.parent = root;
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}

	private Transform ConfigureTrackerAnchor(Transform root)
	{
		string text = trackerAnchorName;
		Transform transform = base.transform.Find(root.name + "/" + text);
		if (transform == null)
		{
			transform = new GameObject(text).transform;
		}
		transform.parent = root;
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}
}
