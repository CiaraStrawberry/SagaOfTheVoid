using System.Collections;
using UnityEngine;

namespace Forge3D;

public class F3DWeaponRangeController : MonoBehaviour
{
	public F3DWeaponRangeCamera wpCamera;

	public Animator ShipAnimator;

	public Transform PassageDoorA;

	public Transform PassageDoorB;

	public Vector3 PassageDoorA_Offset;

	public Vector3 PassageDoorB_Offset;

	public F3DWeaponRangeLightController[] LightControllers;

	public static bool isObserverActive;

	public Transform KeyMap;

	public Transform KeyMapPanel;

	public AudioSource ShipEngine;

	private float shipEngineHumMaxDistance = 40f;

	private void Awake()
	{
	}

	private void Start()
	{
		StartCoroutine("PlaySequence");
	}

	private IEnumerator PlaySequence()
	{
		wpCamera.ResetCamera();
		ResetLightControllers();
		StartCoroutine(SwitchLight(3, state: true, 2f));
		StartCoroutine(SwitchLight(0, state: true, 5f));
		StartCoroutine(SetDoor(state: true, 3f));
		StartCoroutine(SetDoor(state: false, 4f));
		StartCoroutine(SetCameraState(F3DWeaponRangeCamera.WPCameraState.Entry, 0f));
		StartCoroutine(SetCameraState(F3DWeaponRangeCamera.WPCameraState.EffectsOrbit, 5f));
		StartCoroutine(SetObserverControl(state: true, 5f));
		yield return null;
	}

	private IEnumerator SetDoor(bool state, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (state)
		{
			for (float i = 0f; i <= 1f; i += 0.025f)
			{
				PassageDoorA.localPosition = PassageDoorA_Offset * i;
				PassageDoorB.localPosition = PassageDoorB_Offset * i;
				yield return new WaitForSeconds(0.01f);
			}
			yield break;
		}
		for (float i2 = 1f; i2 >= 0f; i2 -= 0.025f)
		{
			PassageDoorA.localPosition = PassageDoorA_Offset * i2;
			PassageDoorB.localPosition = PassageDoorB_Offset * i2;
			yield return new WaitForSeconds(0.01f);
		}
	}

	private void ResetLightControllers()
	{
		for (int i = 0; i < LightControllers.Length; i++)
		{
			LightControllers[i].SwitchLights(state: false);
		}
	}

	private IEnumerator SwitchLight(int index, bool state, float delay)
	{
		yield return new WaitForSeconds(delay);
		F3DWeaponRangeLightController[] lightControllers = LightControllers;
		foreach (F3DWeaponRangeLightController f3DWeaponRangeLightController in lightControllers)
		{
			if (f3DWeaponRangeLightController.ControllerID == index)
			{
				f3DWeaponRangeLightController.SwitchLights(state);
				break;
			}
		}
	}

	private IEnumerator SetCameraState(F3DWeaponRangeCamera.WPCameraState state, float delay)
	{
		yield return new WaitForSeconds(delay);
		wpCamera.cameraState = state;
	}

	private IEnumerator SetObserverControl(bool state, float delay)
	{
		yield return new WaitForSeconds(delay);
		isObserverActive = state;
	}

	private void StartShipAnimation()
	{
		ShipAnimator.SetBool("Dock", value: true);
	}

	private void Update()
	{
		ShipEngine.maxDistance = Mathf.Lerp(ShipEngine.maxDistance, shipEngineHumMaxDistance, Time.deltaTime);
		if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
		{
			KeyMap.gameObject.SetActive(value: true);
			KeyMapPanel.gameObject.SetActive(value: true);
		}
		else
		{
			KeyMap.gameObject.SetActive(value: false);
			KeyMapPanel.gameObject.SetActive(value: false);
		}
		if (isObserverActive)
		{
			if (wpCamera.cameraState != F3DWeaponRangeCamera.WPCameraState.EffectsOrbit && Input.GetKeyDown(KeyCode.Alpha1))
			{
				shipEngineHumMaxDistance = 40f;
				isObserverActive = false;
				StartCoroutine(SetObserverControl(state: true, 3f));
				StartCoroutine(SetDoor(state: true, 0f));
				StartCoroutine(SetDoor(state: false, 3f));
				StartCoroutine(SwitchLight(1, state: false, 0.5f));
				StartCoroutine(SwitchLight(2, state: false, 1f));
				StartCoroutine(SwitchLight(0, state: true, 3f));
				wpCamera.cameraState = F3DWeaponRangeCamera.WPCameraState.EffectsEntry;
				StartCoroutine(SetCameraState(F3DWeaponRangeCamera.WPCameraState.EffectsOrbit, 1.7f));
			}
			else if (wpCamera.cameraState != F3DWeaponRangeCamera.WPCameraState.HangarOrbit && Input.GetKeyDown(KeyCode.Alpha2))
			{
				shipEngineHumMaxDistance = 90f;
				StartShipAnimation();
				isObserverActive = false;
				StartCoroutine(SetObserverControl(state: true, 3f));
				StartCoroutine(SwitchLight(0, state: false, 0.5f));
				StartCoroutine(SwitchLight(1, state: true, 4f));
				StartCoroutine(SwitchLight(2, state: true, 5f));
				StartCoroutine(SetDoor(state: true, 0f));
				StartCoroutine(SetDoor(state: false, 3f));
				wpCamera.cameraState = F3DWeaponRangeCamera.WPCameraState.HangarEntry;
				StartCoroutine(SetCameraState(F3DWeaponRangeCamera.WPCameraState.HangarOrbit, 1.7f));
			}
		}
	}
}
