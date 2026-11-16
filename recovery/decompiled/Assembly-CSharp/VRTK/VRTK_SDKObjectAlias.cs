using UnityEngine;

namespace VRTK;

public class VRTK_SDKObjectAlias : MonoBehaviour
{
	public enum SDKObject
	{
		Boundary,
		Headset
	}

	[Tooltip("The specific SDK Object to child this GameObject to.")]
	public SDKObject sdkObject;

	protected VRTK_SDKManager sdkManager;

	protected virtual void OnEnable()
	{
		sdkManager = VRTK_SDKManager.instance;
		if (sdkManager != null)
		{
			sdkManager.LoadedSetupChanged += LoadedSetupChanged;
		}
		ChildToSDKObject();
	}

	protected virtual void OnDisable()
	{
		if (sdkManager != null)
		{
			sdkManager.LoadedSetupChanged -= LoadedSetupChanged;
		}
	}

	protected virtual void LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
	{
		if (sdkManager != null)
		{
			ChildToSDKObject();
		}
	}

	protected virtual void ChildToSDKObject()
	{
		Vector3 localPosition = transform.localPosition;
		Quaternion localRotation = transform.localRotation;
		Vector3 localScale = transform.localScale;
		Transform parent = null;
		switch (sdkObject)
		{
		case SDKObject.Boundary:
			parent = VRTK_DeviceFinder.PlayAreaTransform();
			break;
		case SDKObject.Headset:
			parent = VRTK_DeviceFinder.HeadsetTransform();
			break;
		}
		transform.SetParent(parent);
		transform.localPosition = localPosition;
		transform.localRotation = localRotation;
		transform.localScale = localScale;
	}
}
