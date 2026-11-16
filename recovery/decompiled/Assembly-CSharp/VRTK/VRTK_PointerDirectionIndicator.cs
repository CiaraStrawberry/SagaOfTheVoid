using UnityEngine;

namespace VRTK;

public class VRTK_PointerDirectionIndicator : MonoBehaviour
{
	[Tooltip("If this is checked then the reported rotation will include the offset of the headset rotation in relation to the play area.")]
	public bool includeHeadsetOffset = true;

	protected VRTK_ControllerEvents controllerEvents;

	protected Transform playArea;

	protected Transform headset;

	public virtual void Initialize(VRTK_ControllerEvents events)
	{
		controllerEvents = events;
		playArea = VRTK_DeviceFinder.PlayAreaTransform();
		headset = VRTK_DeviceFinder.HeadsetTransform();
	}

	public virtual void SetPosition(bool active, Vector3 position)
	{
		transform.position = position;
		gameObject.SetActive(active);
	}

	public virtual Quaternion GetRotation()
	{
		float num = ((!includeHeadsetOffset) ? 0f : (playArea.eulerAngles.y - headset.eulerAngles.y));
		return Quaternion.Euler(0f, transform.localEulerAngles.y + num, 0f);
	}

	protected virtual void Awake()
	{
		gameObject.SetActive(value: false);
	}

	protected virtual void Update()
	{
		if (controllerEvents != null)
		{
			float touchpadAxisAngle = controllerEvents.GetTouchpadAxisAngle();
			float y = ((!(touchpadAxisAngle > 180f)) ? touchpadAxisAngle : (touchpadAxisAngle -= 360f)) + headset.eulerAngles.y;
			transform.localEulerAngles = new Vector3(0f, y, 0f);
		}
	}
}
