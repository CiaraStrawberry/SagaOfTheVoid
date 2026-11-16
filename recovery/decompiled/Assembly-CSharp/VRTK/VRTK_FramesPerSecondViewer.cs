using UnityEngine;
using UnityEngine.UI;

namespace VRTK;

public class VRTK_FramesPerSecondViewer : MonoBehaviour
{
	[Tooltip("Toggles whether the FPS text is visible.")]
	public bool displayFPS = true;

	[Tooltip("The frames per second deemed acceptable that is used as the benchmark to change the FPS text colour.")]
	public int targetFPS = 90;

	[Tooltip("The size of the font the FPS is displayed in.")]
	public int fontSize = 32;

	[Tooltip("The position of the FPS text within the headset view.")]
	public Vector3 position = Vector3.zero;

	[Tooltip("The colour of the FPS text when the frames per second are within reasonable limits of the Target FPS.")]
	public Color goodColor = Color.green;

	[Tooltip("The colour of the FPS text when the frames per second are falling short of reasonable limits of the Target FPS.")]
	public Color warnColor = Color.yellow;

	[Tooltip("The colour of the FPS text when the frames per second are at an unreasonable level of the Target FPS.")]
	public Color badColor = Color.red;

	protected const float updateInterval = 0.5f;

	protected int framesCount;

	protected float framesTime;

	protected Canvas canvas;

	protected Text text;

	protected virtual void OnEnable()
	{
		VRTK_SDKManager instance = VRTK_SDKManager.instance;
		if (instance != null)
		{
			instance.LoadedSetupChanged += LoadedSetupChanged;
		}
		canvas = transform.GetComponentInParent<Canvas>();
		text = GetComponent<Text>();
		if (canvas != null)
		{
			canvas.planeDistance = 0.5f;
		}
		if (text != null)
		{
			text.fontSize = fontSize;
			text.transform.localPosition = position;
		}
		SetCanvasCamera();
	}

	protected virtual void Update()
	{
		framesCount++;
		framesTime += Time.unscaledDeltaTime;
		if (!(framesTime > 0.5f))
		{
			return;
		}
		if (this.text != null)
		{
			if (displayFPS)
			{
				float num = (float)framesCount / framesTime;
				this.text.text = $"{num:F2} FPS";
				Text text = this.text;
				Color color;
				if (num > (float)(targetFPS - 5))
				{
					color = goodColor;
				}
				else
				{
					color = ((!(num > (float)(targetFPS - 30))) ? badColor : warnColor);
				}
				text.color = color;
			}
			else
			{
				this.text.text = string.Empty;
			}
		}
		framesCount = 0;
		framesTime = 0f;
	}

	protected virtual void LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
	{
		SetCanvasCamera();
	}

	protected virtual void SetCanvasCamera()
	{
		Transform transform = VRTK_DeviceFinder.HeadsetCamera();
		if (transform != null)
		{
			canvas.worldCamera = transform.GetComponent<Camera>();
		}
	}
}
