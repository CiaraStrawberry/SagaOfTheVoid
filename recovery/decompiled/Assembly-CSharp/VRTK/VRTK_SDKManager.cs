using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.VR;

namespace VRTK;

public sealed class VRTK_SDKManager : MonoBehaviour
{
	public sealed class ScriptingDefineSymbolPredicateInfo
	{
		public readonly SDK_ScriptingDefineSymbolPredicateAttribute attribute;

		public readonly MethodInfo methodInfo;

		public ScriptingDefineSymbolPredicateInfo(SDK_ScriptingDefineSymbolPredicateAttribute attribute, MethodInfo methodInfo)
		{
			this.attribute = attribute;
			this.methodInfo = methodInfo;
		}
	}

	public struct LoadedSetupChangeEventArgs(VRTK_SDKSetup previousSetup, VRTK_SDKSetup currentSetup, string errorMessage)
	{
		public readonly VRTK_SDKSetup previousSetup = previousSetup;

		public readonly VRTK_SDKSetup currentSetup = currentSetup;

		public readonly string errorMessage = errorMessage;
	}

	public delegate void LoadedSetupChangeEventHandler(VRTK_SDKManager sender, LoadedSetupChangeEventArgs e);

	public static readonly Dictionary<Type, Type> SDKFallbackTypesByBaseType;

	private static VRTK_SDKManager _instance;

	[Tooltip("If this is true then the instance of the SDK Manager won't be destroyed on every scene load.")]
	public bool persistOnLoad;

	[Tooltip("Determines whether the scripting define symbols required by the installed SDKs are automatically added to and removed from the player settings.")]
	public bool autoManageScriptDefines = true;

	public List<SDK_ScriptingDefineSymbolPredicateAttribute> activeScriptingDefineSymbolsWithoutSDKClasses = new List<SDK_ScriptingDefineSymbolPredicateAttribute>();

	[Tooltip("A reference to the GameObject that contains any scripts that apply to the Left Hand Controller.")]
	public GameObject scriptAliasLeftController;

	[Tooltip("A reference to the GameObject that contains any scripts that apply to the Right Hand Controller.")]
	public GameObject scriptAliasRightController;

	[Tooltip("Determines whether the VR settings of the Player Settings are automatically adjusted to allow for all the used SDKs in the SDK Setups list below.")]
	public bool autoManageVRSettings = true;

	[Tooltip("Determines whether the SDK Setups list below is used whenever the SDK Manager is enabled. The first loadable Setup is then loaded.")]
	public bool autoLoadSetup = true;

	[Tooltip("The list of SDK Setups to choose from.")]
	public VRTK_SDKSetup[] setups = new VRTK_SDKSetup[0];

	private List<Behaviour> _behavioursToToggleOnLoadedSetupChange = new List<Behaviour>();

	private Dictionary<Behaviour, bool> _behavioursInitialState = new Dictionary<Behaviour, bool>();

	public static ReadOnlyCollection<ScriptingDefineSymbolPredicateInfo> AvailableScriptingDefineSymbolPredicateInfos { get; private set; }

	public static ReadOnlyCollection<VRTK_SDKInfo> AvailableSystemSDKInfos { get; private set; }

	public static ReadOnlyCollection<VRTK_SDKInfo> AvailableBoundariesSDKInfos { get; private set; }

	public static ReadOnlyCollection<VRTK_SDKInfo> AvailableHeadsetSDKInfos { get; private set; }

	public static ReadOnlyCollection<VRTK_SDKInfo> AvailableControllerSDKInfos { get; private set; }

	public static ReadOnlyCollection<VRTK_SDKInfo> InstalledSystemSDKInfos { get; private set; }

	public static ReadOnlyCollection<VRTK_SDKInfo> InstalledBoundariesSDKInfos { get; private set; }

	public static ReadOnlyCollection<VRTK_SDKInfo> InstalledHeadsetSDKInfos { get; private set; }

	public static ReadOnlyCollection<VRTK_SDKInfo> InstalledControllerSDKInfos { get; private set; }

	public static VRTK_SDKManager instance
	{
		get
		{
			if (_instance == null)
			{
				VRTK_SDKManager vRTK_SDKManager = VRTK_SharedMethods.FindEvenInactiveComponent<VRTK_SDKManager>();
				if (vRTK_SDKManager != null)
				{
					vRTK_SDKManager.CreateInstance();
				}
			}
			return _instance;
		}
	}

	public VRTK_SDKSetup loadedSetup { get; private set; }

	public ReadOnlyCollection<Behaviour> behavioursToToggleOnLoadedSetupChange { get; private set; }

	public event LoadedSetupChangeEventHandler LoadedSetupChanged;

	static VRTK_SDKManager()
	{
		SDKFallbackTypesByBaseType = new Dictionary<Type, Type>
		{
			{
				typeof(SDK_BaseSystem),
				typeof(SDK_FallbackSystem)
			},
			{
				typeof(SDK_BaseBoundaries),
				typeof(SDK_FallbackBoundaries)
			},
			{
				typeof(SDK_BaseHeadset),
				typeof(SDK_FallbackHeadset)
			},
			{
				typeof(SDK_BaseController),
				typeof(SDK_FallbackController)
			}
		};
		PopulateAvailableScriptingDefineSymbolPredicateInfos();
		PopulateAvailableAndInstalledSDKInfos();
	}

	public void AddBehaviourToToggleOnLoadedSetupChange(Behaviour behaviour)
	{
		if (!_behavioursToToggleOnLoadedSetupChange.Contains(behaviour))
		{
			_behavioursToToggleOnLoadedSetupChange.Add(behaviour);
			_behavioursInitialState.Add(behaviour, behaviour.enabled);
		}
		if (loadedSetup == null && behaviour.enabled)
		{
			behaviour.enabled = false;
		}
	}

	public void RemoveBehaviourToToggleOnLoadedSetupChange(Behaviour behaviour)
	{
		_behavioursToToggleOnLoadedSetupChange.Remove(behaviour);
	}

	public void TryLoadSDKSetup(int startIndex, bool tryToReinitialize, params VRTK_SDKSetup[] sdkSetups)
	{
		if (sdkSetups.Length == 0)
		{
			return;
		}
		if (startIndex < 0 || startIndex >= sdkSetups.Length)
		{
			VRTK_Logger.Fatal(new ArgumentOutOfRangeException("startIndex"));
			return;
		}
		sdkSetups = sdkSetups.ToList().GetRange(startIndex, sdkSetups.Length - startIndex).ToArray();
		foreach (VRTK_SDKSetup item in sdkSetups.Where((VRTK_SDKSetup setup) => !setup.isValid))
		{
			string text = string.Join("\n- ", item.GetSimplifiedErrorDescriptions());
			if (!string.IsNullOrEmpty(text))
			{
				text = "- " + text;
				VRTK_Logger.Warn($"Ignoring SDK Setup '{item.name}' because there are some errors with it:\n{text}");
			}
		}
		sdkSetups = sdkSetups.Where((VRTK_SDKSetup setup) => setup.isValid).ToArray();
		VRTK_SDKSetup vRTK_SDKSetup = loadedSetup;
		ToggleBehaviours(state: false);
		loadedSetup = null;
		if (vRTK_SDKSetup != null)
		{
			vRTK_SDKSetup.OnUnloaded(this);
		}
		if (!VRSettings.enabled || !sdkSetups[0].usedVRDeviceNames.Contains(VRSettings.loadedDeviceName))
		{
			if (!tryToReinitialize && !VRSettings.enabled && !string.IsNullOrEmpty(VRSettings.loadedDeviceName))
			{
				sdkSetups = sdkSetups.Where((VRTK_SDKSetup setup) => !setup.usedVRDeviceNames.Contains(VRSettings.loadedDeviceName)).ToArray();
			}
			VRTK_SDKSetup[] array = sdkSetups.Where((VRTK_SDKSetup setup) => setup.usedVRDeviceNames.Except(VRSettings.supportedDevices).Any()).ToArray();
			VRTK_SDKSetup[] array2 = array;
			foreach (VRTK_SDKSetup vRTK_SDKSetup2 in array2)
			{
				string arg = string.Join(", ", vRTK_SDKSetup2.usedVRDeviceNames.Except(VRSettings.supportedDevices).ToArray());
				VRTK_Logger.Warn($"Ignoring SDK Setup '{vRTK_SDKSetup2.name}' because the following VR device names are missing from the PlayerSettings:\n{arg}");
			}
			sdkSetups = sdkSetups.Except(array).ToArray();
			string[] prioritizedDeviceNameList = sdkSetups.SelectMany((VRTK_SDKSetup setup) => setup.usedVRDeviceNames).Distinct().Concat(new string[1] { "None" })
				.ToArray();
			VRSettings.LoadDeviceByName(prioritizedDeviceNameList);
		}
		StartCoroutine(FinishSDKSetupLoading(sdkSetups, vRTK_SDKSetup));
	}

	public void UnloadSDKSetup(bool disableVR = true)
	{
		if (loadedSetup != null)
		{
			ToggleBehaviours(state: false);
		}
		VRTK_SDKSetup vRTK_SDKSetup = loadedSetup;
		loadedSetup = null;
		if (vRTK_SDKSetup != null)
		{
			vRTK_SDKSetup.OnUnloaded(this);
		}
		if (disableVR)
		{
			VRSettings.LoadDeviceByName("None");
			VRSettings.enabled = false;
		}
		if (vRTK_SDKSetup != null)
		{
			OnLoadedSetupChanged(new LoadedSetupChangeEventArgs(vRTK_SDKSetup, null, null));
		}
	}

	private void OnEnable()
	{
		behavioursToToggleOnLoadedSetupChange = _behavioursToToggleOnLoadedSetupChange.AsReadOnly();
		CreateInstance();
		if (!autoLoadSetup)
		{
			return;
		}
		int num = 0;
		if (VRSettings.enabled)
		{
			num = Array.FindIndex(setups, (VRTK_SDKSetup setup) => setup.usedVRDeviceNames.Contains(VRSettings.loadedDeviceName));
		}
		else
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			int num2 = Array.IndexOf(commandLineArgs, "-vrmode", 1);
			if (num2 != -1 && num2 + 1 < commandLineArgs.Length && commandLineArgs[num2 + 1].ToLowerInvariant() == "none")
			{
				num = Array.FindIndex(setups, (VRTK_SDKSetup setup) => setup.usedVRDeviceNames.All((string vrDeviceName) => vrDeviceName == "None"));
			}
		}
		num = ((num != -1) ? num : 0);
		TryLoadSDKSetup(num, tryToReinitialize: false, setups.ToArray());
	}

	private void OnDisable()
	{
		UnloadSDKSetup();
	}

	private void CreateInstance()
	{
		if (_instance == null)
		{
			_instance = this;
			VRTK_SDK_Bridge.InvalidateCaches();
			if (persistOnLoad && Application.isPlaying)
			{
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
		}
		else if (_instance != this)
		{
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	private void OnLoadedSetupChanged(LoadedSetupChangeEventArgs e)
	{
		LoadedSetupChanged?.Invoke(this, e);
	}

	private IEnumerator FinishSDKSetupLoading(VRTK_SDKSetup[] sdkSetups, VRTK_SDKSetup previousLoadedSetup)
	{
		yield return null;
		string loadedDeviceName = ((!string.IsNullOrEmpty(VRSettings.loadedDeviceName)) ? VRSettings.loadedDeviceName : "None");
		loadedSetup = sdkSetups.FirstOrDefault((VRTK_SDKSetup setup) => setup.usedVRDeviceNames.Contains(loadedDeviceName));
		if (loadedSetup == null)
		{
			UnloadSDKSetup();
			VRTK_Logger.Error("No SDK Setup from the provided list could be loaded.");
			OnLoadedSetupChanged(new LoadedSetupChangeEventArgs(previousLoadedSetup, null, "No SDK Setup from the provided list could be loaded."));
			yield break;
		}
		if (loadedSetup.usedVRDeviceNames.Except(new string[1] { "None" }).Any())
		{
			VRSettings.enabled = true;
			if (!VRDevice.isPresent)
			{
				int num = Array.IndexOf(sdkSetups, loadedSetup) + 1;
				string text = "An SDK Setup from the provided list could be loaded, but the device is not in working order.";
				ToggleBehaviours(state: false);
				loadedSetup = null;
				if (num < sdkSetups.Length && sdkSetups.Length - num > 0)
				{
					text += " Now retrying with the remaining SDK Setups from the provided list...";
					VRTK_Logger.Warn(text);
					OnLoadedSetupChanged(new LoadedSetupChangeEventArgs(previousLoadedSetup, null, text));
					TryLoadSDKSetup(num, tryToReinitialize: false, sdkSetups);
				}
				else
				{
					UnloadSDKSetup();
					text += " There are no other Setups in the provided list to try.";
					VRTK_Logger.Error(text);
					OnLoadedSetupChanged(new LoadedSetupChangeEventArgs(previousLoadedSetup, null, text));
				}
				yield break;
			}
		}
		loadedSetup.OnLoaded(this);
		ToggleBehaviours(state: true);
		OnLoadedSetupChanged(new LoadedSetupChangeEventArgs(previousLoadedSetup, loadedSetup, null));
	}

	private void ToggleBehaviours(bool state)
	{
		IEnumerable<Behaviour> enumerable = _behavioursToToggleOnLoadedSetupChange.ToList();
		if (!state)
		{
			enumerable = enumerable.Reverse();
		}
		foreach (Behaviour item in enumerable)
		{
			item.enabled = ((!state || !_behavioursInitialState.ContainsKey(item)) ? state : _behavioursInitialState[item]);
		}
	}

	private static void PopulateAvailableScriptingDefineSymbolPredicateInfos()
	{
		List<ScriptingDefineSymbolPredicateInfo> list = new List<ScriptingDefineSymbolPredicateInfo>();
		Type[] types = typeof(VRTK_SDKManager).Assembly.GetTypes();
		foreach (Type type in types)
		{
			for (int j = 0; j < type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Length; j++)
			{
				MethodInfo methodInfo = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)[j];
				SDK_ScriptingDefineSymbolPredicateAttribute[] array = (SDK_ScriptingDefineSymbolPredicateAttribute[])methodInfo.GetCustomAttributes(typeof(SDK_ScriptingDefineSymbolPredicateAttribute), inherit: false);
				if (array.Length != 0)
				{
					if (methodInfo.ReturnType != typeof(bool) || methodInfo.GetParameters().Length != 0)
					{
						VRTK_Logger.Fatal(new InvalidOperationException($"The method '{methodInfo.Name}' on '{type}' has '{typeof(SDK_ScriptingDefineSymbolPredicateAttribute)}' specified but its signature is wrong. The method must take no arguments and return bool."));
						return;
					}
					list.AddRange(array.Select((SDK_ScriptingDefineSymbolPredicateAttribute predicateAttribute) => new ScriptingDefineSymbolPredicateInfo(predicateAttribute, methodInfo)));
				}
			}
		}
		list.Sort((ScriptingDefineSymbolPredicateInfo x, ScriptingDefineSymbolPredicateInfo y) => string.Compare(x.attribute.symbol, y.attribute.symbol, StringComparison.Ordinal));
		AvailableScriptingDefineSymbolPredicateInfos = list.AsReadOnly();
	}

	private static void PopulateAvailableAndInstalledSDKInfos()
	{
		List<string> symbolsOfInstalledSDKs = (from predicateInfo in AvailableScriptingDefineSymbolPredicateInfos
			where (bool)predicateInfo.methodInfo.Invoke(null, null)
			select predicateInfo.attribute.symbol).ToList();
		List<VRTK_SDKInfo> list = new List<VRTK_SDKInfo>();
		List<VRTK_SDKInfo> list2 = new List<VRTK_SDKInfo>();
		List<VRTK_SDKInfo> list3 = new List<VRTK_SDKInfo>();
		List<VRTK_SDKInfo> list4 = new List<VRTK_SDKInfo>();
		List<VRTK_SDKInfo> list5 = new List<VRTK_SDKInfo>();
		List<VRTK_SDKInfo> list6 = new List<VRTK_SDKInfo>();
		List<VRTK_SDKInfo> list7 = new List<VRTK_SDKInfo>();
		List<VRTK_SDKInfo> list8 = new List<VRTK_SDKInfo>();
		PopulateAvailableAndInstalledSDKInfos<SDK_BaseSystem, SDK_FallbackSystem>(list, list5, symbolsOfInstalledSDKs);
		PopulateAvailableAndInstalledSDKInfos<SDK_BaseBoundaries, SDK_FallbackBoundaries>(list2, list6, symbolsOfInstalledSDKs);
		PopulateAvailableAndInstalledSDKInfos<SDK_BaseHeadset, SDK_FallbackHeadset>(list3, list7, symbolsOfInstalledSDKs);
		PopulateAvailableAndInstalledSDKInfos<SDK_BaseController, SDK_FallbackController>(list4, list8, symbolsOfInstalledSDKs);
		AvailableSystemSDKInfos = list.AsReadOnly();
		AvailableBoundariesSDKInfos = list2.AsReadOnly();
		AvailableHeadsetSDKInfos = list3.AsReadOnly();
		AvailableControllerSDKInfos = list4.AsReadOnly();
		InstalledSystemSDKInfos = list5.AsReadOnly();
		InstalledBoundariesSDKInfos = list6.AsReadOnly();
		InstalledHeadsetSDKInfos = list7.AsReadOnly();
		InstalledControllerSDKInfos = list8.AsReadOnly();
	}

	private static void PopulateAvailableAndInstalledSDKInfos<BaseType, FallbackType>(List<VRTK_SDKInfo> availableSDKInfos, List<VRTK_SDKInfo> installedSDKInfos, ICollection<string> symbolsOfInstalledSDKs) where BaseType : SDK_Base where FallbackType : BaseType
	{
		Type baseType = typeof(BaseType);
		Type fallbackType = SDKFallbackTypesByBaseType[baseType];
		availableSDKInfos.AddRange(VRTK_SDKInfo.Create<BaseType, FallbackType, FallbackType>());
		availableSDKInfos.AddRange((from type in baseType.Assembly.GetExportedTypes()
			where type.IsSubclassOf(baseType) && type != fallbackType && !type.IsAbstract
			select type).SelectMany(VRTK_SDKInfo.Create<BaseType, FallbackType>));
		availableSDKInfos.Sort((VRTK_SDKInfo x, VRTK_SDKInfo y) => (!x.description.describesFallbackSDK) ? string.Compare(x.description.prettyName, y.description.prettyName, StringComparison.Ordinal) : (-1));
		installedSDKInfos.AddRange(availableSDKInfos.Where((VRTK_SDKInfo info) =>
		{
			string symbol = info.description.symbol;
			return string.IsNullOrEmpty(symbol) || symbolsOfInstalledSDKs.Contains(symbol);
		}));
	}
}
