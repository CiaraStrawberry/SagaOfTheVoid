using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VRTK;

public class VRTK_SDKSetupSwitcher : MonoBehaviour
{
	protected enum ViewingState
	{
		Status,
		Selection
	}

	[Header("Fallback Objects")]
	[SerializeField]
	private Camera fallbackCamera;

	[SerializeField]
	private EventSystem eventSystem;

	[Header("Object References")]
	[SerializeField]
	private Text currentText;

	[SerializeField]
	private RectTransform statusPanel;

	[SerializeField]
	private RectTransform selectionPanel;

	[SerializeField]
	private Button switchButton;

	[SerializeField]
	private Button cancelButton;

	[SerializeField]
	private Button chooseButton;

	private VRTK_SDKManager sdkManager;

	private readonly List<GameObject> chooseButtonGameObjects = new List<GameObject>();

	protected virtual void Awake()
	{
		fallbackCamera.gameObject.SetActive(value: false);
		eventSystem.gameObject.SetActive(value: false);
		chooseButton.gameObject.SetActive(value: false);
	}

	protected virtual void OnEnable()
	{
		sdkManager = VRTK_SDKManager.instance;
		sdkManager.LoadedSetupChanged += OnLoadedSetupChanged;
		switchButton.onClick.AddListener(OnSwitchButtonClick);
		cancelButton.onClick.AddListener(OnCancelButtonClick);
		Show(ViewingState.Status);
	}

	protected virtual void OnDisable()
	{
		sdkManager.LoadedSetupChanged -= OnLoadedSetupChanged;
		switchButton.onClick.RemoveListener(OnSwitchButtonClick);
		cancelButton.onClick.RemoveListener(OnCancelButtonClick);
		Show(ViewingState.Status);
	}

	protected virtual void OnLoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
	{
		Show(ViewingState.Status);
	}

	protected virtual void OnSwitchButtonClick()
	{
		Show(ViewingState.Selection);
	}

	protected virtual void OnCancelButtonClick()
	{
		Show(ViewingState.Status);
	}

	protected virtual void Show(ViewingState viewingState)
	{
		switch (viewingState)
		{
		case ViewingState.Status:
			RemoveCreatedChooseButtons();
			UpdateCurrentText();
			selectionPanel.gameObject.SetActive(value: false);
			statusPanel.gameObject.SetActive(value: true);
			break;
		case ViewingState.Selection:
			AddSelectionButtons();
			selectionPanel.gameObject.SetActive(value: true);
			statusPanel.gameObject.SetActive(value: false);
			break;
		default:
			VRTK_Logger.Fatal(new ArgumentOutOfRangeException("viewingState", viewingState, null));
			return;
		}
		fallbackCamera.gameObject.SetActive(Camera.main == null || Camera.main == fallbackCamera);
		eventSystem.gameObject.SetActive(EventSystem.current == null || EventSystem.current == eventSystem);
	}

	protected virtual void UpdateCurrentText()
	{
		VRTK_SDKSetup loadedSetup = sdkManager.loadedSetup;
		currentText.text = ((!(loadedSetup == null)) ? loadedSetup.name : "None");
	}

	protected virtual void AddSelectionButtons()
	{
		VRTK_SDKSetup loadedSetup = sdkManager.loadedSetup;
		if (loadedSetup != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(chooseButton.gameObject, chooseButton.transform.parent);
			gameObject.GetComponentInChildren<Text>().text = "None";
			gameObject.name = "ChooseNoneButton";
			gameObject.SetActive(value: true);
			gameObject.GetComponent<Button>().onClick.AddListener(() =>
			{
				sdkManager.UnloadSDKSetup();
			});
			chooseButtonGameObjects.Add(gameObject);
		}
		VRTK_SDKSetup[] setups = sdkManager.setups;
		for (int num = 0; num < setups.Length; num++)
		{
			VRTK_SDKSetup vRTK_SDKSetup = setups[num];
			if (!(vRTK_SDKSetup == null) && !(vRTK_SDKSetup == loadedSetup))
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(chooseButton.gameObject, chooseButton.transform.parent);
				gameObject2.GetComponentInChildren<Text>().text = vRTK_SDKSetup.name;
				gameObject2.name = $"Choose{vRTK_SDKSetup.name}Button";
				gameObject2.SetActive(value: true);
				int indexCopy = num;
				Button component = gameObject2.GetComponent<Button>();
				component.onClick.AddListener(() =>
				{
					sdkManager.TryLoadSDKSetup(indexCopy, tryToReinitialize: true, setups);
				});
				ColorBlock colors = component.colors;
				colors.colorMultiplier = ((!vRTK_SDKSetup.isValid) ? 0.8f : 1f);
				component.colors = colors;
				chooseButtonGameObjects.Add(gameObject2);
			}
		}
	}

	protected virtual void RemoveCreatedChooseButtons()
	{
		chooseButtonGameObjects.ForEach(UnityEngine.Object.Destroy);
		chooseButtonGameObjects.Clear();
	}
}
