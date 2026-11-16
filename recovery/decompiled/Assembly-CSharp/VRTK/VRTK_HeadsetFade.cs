using UnityEngine;

namespace VRTK;

[AddComponentMenu("VRTK/Scripts/Presence/VRTK_HeadsetFade")]
public class VRTK_HeadsetFade : MonoBehaviour
{
	protected Transform headset;

	protected bool isTransitioning;

	protected bool isFaded;

	public event HeadsetFadeEventHandler HeadsetFadeStart;

	public event HeadsetFadeEventHandler HeadsetFadeComplete;

	public event HeadsetFadeEventHandler HeadsetUnfadeStart;

	public event HeadsetFadeEventHandler HeadsetUnfadeComplete;

	public virtual void OnHeadsetFadeStart(HeadsetFadeEventArgs e)
	{
		if (HeadsetFadeStart != null)
		{
			HeadsetFadeStart(this, e);
		}
	}

	public virtual void OnHeadsetFadeComplete(HeadsetFadeEventArgs e)
	{
		if (HeadsetFadeComplete != null)
		{
			HeadsetFadeComplete(this, e);
		}
	}

	public virtual void OnHeadsetUnfadeStart(HeadsetFadeEventArgs e)
	{
		if (HeadsetUnfadeStart != null)
		{
			HeadsetUnfadeStart(this, e);
		}
	}

	public virtual void OnHeadsetUnfadeComplete(HeadsetFadeEventArgs e)
	{
		if (HeadsetUnfadeComplete != null)
		{
			HeadsetUnfadeComplete(this, e);
		}
	}

	public virtual bool IsFaded()
	{
		return isFaded;
	}

	public virtual bool IsTransitioning()
	{
		return isTransitioning;
	}

	public virtual void Fade(Color color, float duration)
	{
		isFaded = false;
		isTransitioning = true;
		VRTK_SDK_Bridge.HeadsetFade(color, duration);
		OnHeadsetFadeStart(SetHeadsetFadeEvent(headset, duration));
		CancelInvoke("UnfadeComplete");
		Invoke("FadeComplete", duration);
	}

	public virtual void Unfade(float duration)
	{
		isFaded = true;
		isTransitioning = true;
		VRTK_SDK_Bridge.HeadsetFade(Color.clear, duration);
		OnHeadsetUnfadeStart(SetHeadsetFadeEvent(headset, duration));
		CancelInvoke("FadeComplete");
		Invoke("UnfadeComplete", duration);
	}

	protected virtual void Awake()
	{
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void OnEnable()
	{
		headset = VRTK_DeviceFinder.HeadsetCamera();
		isTransitioning = false;
		isFaded = false;
		VRTK_SharedMethods.AddCameraFade();
		if (!VRTK_SDK_Bridge.HasHeadsetFade(headset))
		{
			VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_HeadsetFade", "compatible fade", "Camera"));
		}
	}

	protected virtual void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual HeadsetFadeEventArgs SetHeadsetFadeEvent(Transform currentTransform, float duration)
	{
		HeadsetFadeEventArgs result = default;
		result.timeTillComplete = duration;
		result.currentTransform = currentTransform;
		return result;
	}

	protected virtual void FadeComplete()
	{
		isFaded = true;
		isTransitioning = false;
		OnHeadsetFadeComplete(SetHeadsetFadeEvent(headset, 0f));
	}

	protected virtual void UnfadeComplete()
	{
		isFaded = false;
		isTransitioning = false;
		OnHeadsetUnfadeComplete(SetHeadsetFadeEvent(headset, 0f));
	}
}
