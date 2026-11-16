using UnityEngine;

namespace VRTK;

public class VRTK_ObjectTooltip : MonoBehaviour
{
	[Tooltip("The text that is displayed on the tooltip.")]
	public string displayText;

	[Tooltip("The size of the text that is displayed.")]
	public int fontSize = 14;

	[Tooltip("The size of the tooltip container where `x = width` and `y = height`.")]
	public Vector2 containerSize = new Vector2(0.1f, 0.03f);

	[Tooltip("An optional transform of where to start drawing the line from. If one is not provided the centre of the tooltip is used for the initial line position.")]
	public Transform drawLineFrom;

	[Tooltip("A transform of another object in the scene that a line will be drawn from the tooltip to, this helps denote what the tooltip is in relation to. If no transform is provided and the tooltip is a child of another object, then the parent object's transform will be used as this destination position.")]
	public Transform drawLineTo;

	[Tooltip("The width of the line drawn between the tooltip and the destination transform.")]
	public float lineWidth = 0.001f;

	[Tooltip("The colour to use for the text on the tooltip.")]
	public Color fontColor = Color.black;

	[Tooltip("The colour to use for the background container of the tooltip.")]
	public Color containerColor = Color.black;

	[Tooltip("The colour to use for the line drawn between the tooltip and the destination transform.")]
	public Color lineColor = Color.black;

	[Tooltip("If this is checked then the tooltip will be rotated so it always face the headset.")]
	public bool alwaysFaceHeadset;

	protected LineRenderer line;

	protected Transform headset;

	public virtual void ResetTooltip()
	{
		SetContainer();
		SetText("UITextFront");
		SetText("UITextReverse");
		SetLine();
		if (drawLineTo == null && transform.parent != null)
		{
			drawLineTo = transform.parent;
		}
	}

	public virtual void UpdateText(string newText)
	{
		displayText = newText;
		ResetTooltip();
	}

	protected virtual void Awake()
	{
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void OnEnable()
	{
		ResetTooltip();
		headset = VRTK_DeviceFinder.HeadsetTransform();
	}

	protected virtual void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void Update()
	{
		DrawLine();
		if (alwaysFaceHeadset)
		{
			transform.LookAt(headset);
		}
	}

	protected virtual void SetContainer()
	{
		base.transform.Find("TooltipCanvas").GetComponent<RectTransform>().sizeDelta = containerSize;
		Transform transform = base.transform.Find("TooltipCanvas/UIContainer");
		transform.GetComponent<RectTransform>().sizeDelta = containerSize;
	}

	protected virtual void SetText(string name)
	{
	}

	protected virtual void SetLine()
	{
		line = transform.Find("Line").GetComponent<LineRenderer>();
		line.material = Resources.Load("TooltipLine") as Material;
		line.material.color = lineColor;
		line.startColor = lineColor;
		line.endColor = lineColor;
		line.startWidth = lineWidth;
		line.endWidth = lineWidth;
		if (drawLineFrom == null)
		{
			drawLineFrom = transform;
		}
	}

	protected virtual void DrawLine()
	{
		if (drawLineTo != null)
		{
			line.SetPosition(0, drawLineFrom.position);
			line.SetPosition(1, drawLineTo.position);
		}
	}
}
