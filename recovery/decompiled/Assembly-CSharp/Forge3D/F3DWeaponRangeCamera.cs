using System;
using UnityEngine;

namespace Forge3D;

public class F3DWeaponRangeCamera : MonoBehaviour
{
	public enum WPCameraState
	{
		Idle,
		Entry,
		HangarOrbit,
		HangarEntry,
		Effects,
		EffectsEntry,
		EffectsOrbit
	}

	public enum CameraLerpType
	{
		EaseIn,
		EaseOut,
		SmooterSmooth
	}

	public Camera MainCamera;

	public Transform WPHangar;

	public Transform WPHangarCentral;

	public Transform WPStart;

	public Transform WPEffects;

	public Transform WPEffectsCentral;

	private float globalCamTranslationSpeed;

	private float globalCamLookSpeed;

	public float mouseLimit = 0.5f;

	public float CamTranslationSpeed;

	public float CamLookSpeed;

	public float CamOrbitTranslationSpeed = 0.5f;

	public float CamOrbitLookSpeed = 0.5f;

	public float EffectsCameraZoomMin;

	public float EffectsCameraZoomMax;

	public float HangarCameraZoomMin;

	public float HangarCameraZoomMax;

	public float EffectsMouseYLimitLow;

	public float EffectsMouseYLimitHigh;

	public float HangarMouseYLimitLow;

	public float HangarMouseYLimitHigh;

	private WPCameraState cameraState_;

	private Vector3 lastCameraPos;

	private float mouseX;

	private float mouseY;

	private float mouseWheel;

	private float mouseLimitMin;

	private float mouseLimitMax;

	private Vector3 overlapOffset;

	private bool overlapping;

	private Vector3 camVelocity = Vector3.zero;

	private float curOrbitTime;

	private float curOrbitLerpTime = 6f;

	private float t;

	public WPCameraState cameraState
	{
		get
		{
			return cameraState_;
		}
		set
		{
			curOrbitTime = 0f;
			t = 0f;
			mouseX = (mouseY = 0f);
			lastCameraPos = MainCamera.transform.position;
			switch (value)
			{
			case WPCameraState.HangarOrbit:
				mouseWheel = HangarCameraZoomMax;
				mouseLimitMin = HangarMouseYLimitLow;
				mouseLimitMax = HangarMouseYLimitHigh;
				break;
			case WPCameraState.EffectsOrbit:
				mouseWheel = EffectsCameraZoomMax;
				mouseLimitMin = EffectsMouseYLimitLow;
				mouseLimitMax = EffectsMouseYLimitHigh;
				break;
			}
			cameraState_ = value;
		}
	}

	private void Awake()
	{
		mouseWheel = EffectsCameraZoomMax;
	}

	public void ResetCamera()
	{
		MainCamera.transform.position = WPStart.position;
		MainCamera.transform.rotation = Quaternion.LookRotation(WPEffectsCentral.position - MainCamera.transform.position, Vector3.up);
	}

	private void OrbitCamera(Vector3 origin, Vector3 forward, float zoomMin, float zoomMax, bool checkOverlap = false)
	{
		if (F3DWeaponRangeController.isObserverActive && Input.GetKey(KeyCode.Mouse1))
		{
			mouseX -= Mathf.Clamp(Input.GetAxis("Mouse X"), 0f - mouseLimit, mouseLimit);
			mouseY += Mathf.Clamp(Input.GetAxis("Mouse Y"), 0f - mouseLimit, mouseLimit);
		}
		mouseWheel -= Input.GetAxis("Mouse ScrollWheel") * 5f;
		mouseWheel = Mathf.Clamp(mouseWheel, zoomMin, zoomMax);
		mouseY = Mathf.Clamp(mouseY, mouseLimitMin, mouseLimitMax);
		if (checkOverlap && Physics.CheckSphere(MainCamera.transform.position, 5f))
		{
			overlapping = true;
			overlapOffset = Vector3.SmoothDamp(overlapOffset, forward * 15f, ref camVelocity, 1f);
			mouseWheel += overlapOffset.magnitude * Time.deltaTime * 2f;
		}
		else
		{
			overlapping = false;
			overlapOffset = Vector3.SmoothDamp(overlapOffset, Vector3.zero, ref camVelocity, 2f);
		}
		mouseWheel = Mathf.Clamp(mouseWheel, zoomMin, zoomMax);
		Vector3 vector = forward * mouseWheel;
		vector = Vector3.ClampMagnitude(vector, 20f);
		vector = Quaternion.AngleAxis(mouseX, Vector3.up) * vector;
		Vector3 axis = Vector3.Cross(vector, Vector3.up);
		vector = Quaternion.AngleAxis(mouseY, axis) * vector;
		MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, origin + vector, Time.deltaTime * globalCamTranslationSpeed);
		MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, Quaternion.LookRotation(origin - MainCamera.transform.position, Vector3.up), Time.deltaTime * globalCamLookSpeed);
		Debug.DrawLine(WPEffectsCentral.position, WPEffectsCentral.position + vector);
	}

	private void TickLerp(CameraLerpType type)
	{
		curOrbitTime += Time.deltaTime;
		if (curOrbitTime >= curOrbitLerpTime)
		{
			curOrbitTime = curOrbitLerpTime;
		}
		t = curOrbitTime / curOrbitLerpTime;
		switch (type)
		{
		case CameraLerpType.SmooterSmooth:
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			break;
		case CameraLerpType.EaseIn:
			t = 1f - Mathf.Cos(t * (float)Math.PI * 0.5f);
			break;
		case CameraLerpType.EaseOut:
			t = Mathf.Sin(t * (float)Math.PI * 0.5f);
			break;
		}
	}

	private void FixedUpdate()
	{
	}

	private void OnDrawGizmos()
	{
		if (overlapping)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(MainCamera.transform.position, 5f);
		}
	}

	private void LateUpdate()
	{
		switch (cameraState_)
		{
		case WPCameraState.HangarEntry:
			curOrbitLerpTime = 2f;
			TickLerp(CameraLerpType.SmooterSmooth);
			MainCamera.transform.position = Vector3.Lerp(lastCameraPos, WPEffects.position, t);
			MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, Quaternion.LookRotation(WPHangar.forward, Vector3.up), t);
			break;
		case WPCameraState.HangarOrbit:
			curOrbitLerpTime = 4f;
			TickLerp(CameraLerpType.SmooterSmooth);
			globalCamTranslationSpeed = Mathf.Lerp(0.5f, CamOrbitTranslationSpeed, t);
			globalCamLookSpeed = Mathf.Lerp(0.5f, CamOrbitLookSpeed, t);
			OrbitCamera(WPHangarCentral.position, WPHangarCentral.forward, HangarCameraZoomMin, HangarCameraZoomMax, checkOverlap: true);
			break;
		case WPCameraState.EffectsEntry:
			curOrbitLerpTime = 2f;
			TickLerp(CameraLerpType.SmooterSmooth);
			MainCamera.transform.position = Vector3.Lerp(lastCameraPos, WPHangar.position, t);
			MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, Quaternion.LookRotation(WPEffects.forward, Vector3.up), t);
			break;
		case WPCameraState.EffectsOrbit:
			curOrbitLerpTime = 2f;
			TickLerp(CameraLerpType.SmooterSmooth);
			globalCamTranslationSpeed = Mathf.Lerp(0.5f, CamOrbitTranslationSpeed, t);
			globalCamLookSpeed = Mathf.Lerp(0.5f, CamOrbitLookSpeed, t);
			OrbitCamera(WPEffectsCentral.position, WPEffectsCentral.forward, EffectsCameraZoomMin, EffectsCameraZoomMax);
			break;
		case WPCameraState.Entry:
			curOrbitLerpTime = 5f;
			TickLerp(CameraLerpType.SmooterSmooth);
			MainCamera.transform.position = Vector3.Lerp(WPStart.position, WPEffects.position, t);
			MainCamera.transform.rotation = Quaternion.Lerp(MainCamera.transform.rotation, Quaternion.LookRotation(WPEffectsCentral.position - MainCamera.transform.position, Vector3.up), t);
			break;
		case WPCameraState.Idle:
		case WPCameraState.Effects:
			break;
		}
	}
}
