using UnityEngine;
using VRTK;
using simple;

public class VRTK_SDK_CONTROLLER_MANAGER : MonoBehaviour
{
	public bool issteamversion;

	public GameObject steamManager;

	public MainMenuValueHolder crosslevelholder;

	public VRTK_SDKManager stkman;

	public bool lefthandmode;

	private IOComponent FleetBuilder;

	private void Awake()
	{
		if (Object.FindObjectsOfType(GetType()).Length > 1)
		{
			Object.Destroy(gameObject);
		}
		if (steamManager == null)
		{
			steamManager = GameObject.Find("SteamManager");
		}
		if (!issteamversion)
		{
			Debug.Log("setupoculus");
			if ((bool)steamManager)
			{
				steamManager.SetActive(value: false);
			}
		}
		FleetBuilder = IORoot.findIO("fleet1");
		FleetBuilder.read();
		crosslevelholder = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
		steamManager = GameObject.Find("SteamManager");
		stkman = GetComponent<VRTK_SDKManager>();
		InvokeRepeating("Rep", 0f, 1f);
		stkman.LoadedSetupChanged += Load;
		if (FleetBuilder.get<bool>("LeftMode"))
		{
			GameObject scriptAliasLeftController = stkman.scriptAliasLeftController;
			stkman.scriptAliasLeftController = stkman.scriptAliasRightController;
			stkman.scriptAliasRightController = scriptAliasLeftController;
			FleetBuilder.read();
		}
	}

	private void Start()
	{
		Application.runInBackground = false;
	}

	private void Rep()
	{
	}

	private void Update()
	{
	}

	public void Load(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
	{
		crosslevelholder.loadedsetup = stkman.loadedSetup;
	}
}
