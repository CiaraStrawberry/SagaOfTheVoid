using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrueSync;

[AddComponentMenu("")]
public class TrueSyncManager : MonoBehaviour
{
	private enum StartState
	{
		BEHAVIOR_INITIALIZED,
		FIRST_UPDATE,
		STARTED
	}

	private const float JitterTimeFactor = 0.001f;

	private const string serverSettingsAssetFile = "TrueSyncGlobalConfig";

	private StartState startState;

	public GameObject[] playerPrefabs;

	public static TrueSyncConfig _TrueSyncGlobalConfig;

	public static TrueSyncConfig TrueSyncCustomConfig;

	public TrueSyncConfig customConfig;

	private Dictionary<int, List<GameObject>> gameOjectsSafeMap = new Dictionary<int, List<GameObject>>();

	private AbstractLockstep lockstep;

	private FP lockedTimeStep;

	private List<TrueSyncManagedBehaviour> generalBehaviours = new List<TrueSyncManagedBehaviour>();

	private Dictionary<byte, List<TrueSyncManagedBehaviour>> behaviorsByPlayer;

	private CoroutineScheduler scheduler;

	private List<TrueSyncManagedBehaviour> queuedBehaviours = new List<TrueSyncManagedBehaviour>();

	private Dictionary<ITrueSyncBehaviour, TrueSyncManagedBehaviour> mapBehaviorToManagedBehavior = new Dictionary<ITrueSyncBehaviour, TrueSyncManagedBehaviour>();

	private FP time = 0;

	private static TrueSyncManager instance;

	private FP tsDeltaTime = 0;

	public static TrueSyncConfig TrueSyncGlobalConfig
	{
		get
		{
			if (_TrueSyncGlobalConfig == null)
			{
				_TrueSyncGlobalConfig = (TrueSyncConfig)Resources.Load("TrueSyncGlobalConfig", typeof(TrueSyncConfig));
			}
			return _TrueSyncGlobalConfig;
		}
	}

	public static FP DeltaTime
	{
		get
		{
			if (instance == null)
			{
				return 0;
			}
			return instance.lockedTimeStep;
		}
	}

	public static FP Time
	{
		get
		{
			if (instance == null || instance.lockstep == null)
			{
				return 0;
			}
			return instance.time;
		}
	}

	public static int Ticks
	{
		get
		{
			if (instance == null || instance.lockstep == null)
			{
				return 0;
			}
			return instance.lockstep.Ticks;
		}
	}

	public static int LastSafeTick
	{
		get
		{
			if (instance == null || instance.lockstep == null)
			{
				return 0;
			}
			return instance.lockstep.LastSafeTick;
		}
	}

	public static TSVector Gravity
	{
		get
		{
			if (instance == null)
			{
				return TSVector.zero;
			}
			return instance.ActiveConfig.gravity3D;
		}
	}

	public static List<TSPlayerInfo> Players
	{
		get
		{
			if (instance == null || instance.lockstep == null)
			{
				return null;
			}
			List<TSPlayerInfo> list = new List<TSPlayerInfo>();
			foreach (TSPlayer value in instance.lockstep.Players.Values)
			{
				if (!value.dropped)
				{
					list.Add(value.playerInfo);
				}
			}
			return list;
		}
	}

	public static TSPlayerInfo LocalPlayer
	{
		get
		{
			if (instance == null || instance.lockstep == null)
			{
				return null;
			}
			return instance.lockstep.LocalPlayer.playerInfo;
		}
	}

	public static TrueSyncConfig Config
	{
		get
		{
			if (instance == null)
			{
				return null;
			}
			return instance.ActiveConfig;
		}
	}

	private TrueSyncConfig ActiveConfig
	{
		get
		{
			if (TrueSyncCustomConfig != null)
			{
				customConfig = TrueSyncCustomConfig;
				TrueSyncCustomConfig = null;
			}
			if (customConfig != null)
			{
				return customConfig;
			}
			return TrueSyncGlobalConfig;
		}
	}

	private void Awake()
	{
		TrueSyncConfig activeConfig = ActiveConfig;
		lockedTimeStep = activeConfig.lockedTimeStep;
		StateTracker.Init(activeConfig.rollbackWindow);
		TSRandom.Init();
		if (activeConfig.physics2DEnabled || activeConfig.physics3DEnabled)
		{
			PhysicsManager.New(activeConfig);
			PhysicsManager.instance.LockedTimeStep = lockedTimeStep;
			PhysicsManager.instance.Init();
		}
		StateTracker.AddTracking(this, "time");
	}

	private void Start()
	{
		instance = this;
		Application.runInBackground = true;
		ICommunicator communicator = null;
		if (!PhotonNetwork.connected || !PhotonNetwork.inRoom)
		{
			Debug.LogWarning("You are not connected to Photon. TrueSync will start in offline mode.");
		}
		else
		{
			communicator = new PhotonTrueSyncCommunicator(PhotonNetwork.networkingPeer);
		}
		TrueSyncConfig activeConfig = ActiveConfig;
		lockstep = AbstractLockstep.NewInstance(lockedTimeStep.AsFloat(), communicator, PhysicsManager.instance, activeConfig.syncWindow, activeConfig.panicWindow, activeConfig.rollbackWindow, OnGameStarted, OnGamePaused, OnGameUnPaused, OnGameEnded, OnPlayerDisconnection, OnStepUpdate, GetLocalData, ProvideInputData);
		if (ReplayRecord.replayMode == ReplayMode.LOAD_REPLAY)
		{
			ReplayPicker.replayToLoad.Load();
			ReplayRecord replayToLoad = ReplayRecord.replayToLoad;
			if (replayToLoad == null)
			{
				Debug.LogError("Replay Record can't be loaded");
				gameObject.SetActive(value: false);
				return;
			}
			lockstep.ReplayRecord = replayToLoad;
		}
		if (activeConfig.showStats)
		{
			gameObject.AddComponent<TrueSyncStats>().Lockstep = lockstep;
		}
		scheduler = new CoroutineScheduler(lockstep);
		if (ReplayRecord.replayMode != ReplayMode.LOAD_REPLAY)
		{
			if (communicator == null)
			{
				lockstep.AddPlayer(0, "Local_Player", isLocal: true);
			}
			else
			{
				List<PhotonPlayer> list = new List<PhotonPlayer>(PhotonNetwork.playerList);
				list.Sort(UnityUtils.playerComparer);
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					PhotonPlayer photonPlayer = list[i];
					lockstep.AddPlayer((byte)photonPlayer.ID, photonPlayer.NickName, photonPlayer.IsLocal);
				}
			}
		}
		TrueSyncBehaviour[] array = UnityEngine.Object.FindObjectsOfType<TrueSyncBehaviour>();
		int j = 0;
		for (int num = array.Length; j < num; j++)
		{
			generalBehaviours.Add(NewManagedBehavior(array[j]));
		}
		initBehaviors();
		initGeneralBehaviors(generalBehaviours, realOwnerId: false);
		PhysicsManager.instance.OnRemoveBody(OnRemovedRigidBody);
		startState = StartState.BEHAVIOR_INITIALIZED;
	}

	private TrueSyncManagedBehaviour NewManagedBehavior(ITrueSyncBehaviour trueSyncBehavior)
	{
		TrueSyncManagedBehaviour trueSyncManagedBehaviour = new TrueSyncManagedBehaviour(trueSyncBehavior);
		mapBehaviorToManagedBehavior[trueSyncBehavior] = trueSyncManagedBehaviour;
		return trueSyncManagedBehaviour;
	}

	private void initBehaviors()
	{
		behaviorsByPlayer = new Dictionary<byte, List<TrueSyncManagedBehaviour>>();
		IEnumerator<KeyValuePair<byte, TSPlayer>> enumerator = lockstep.Players.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TSPlayer value = enumerator.Current.Value;
			List<TrueSyncManagedBehaviour> list = new List<TrueSyncManagedBehaviour>();
			int i = 0;
			for (int num = playerPrefabs.Length; i < num; i++)
			{
				GameObject original = playerPrefabs[i];
				GameObject gameObject = UnityEngine.Object.Instantiate(original);
				InitializeGameObject(gameObject, gameObject.transform.localPosition.ToTSVector(), gameObject.transform.rotation.ToTSQuaternion());
				TrueSyncBehaviour[] componentsInChildren = gameObject.GetComponentsInChildren<TrueSyncBehaviour>();
				int j = 0;
				for (int num2 = componentsInChildren.Length; j < num2; j++)
				{
					TrueSyncBehaviour trueSyncBehaviour = componentsInChildren[j];
					trueSyncBehaviour.owner = value.playerInfo;
					trueSyncBehaviour.localOwner = lockstep.LocalPlayer.playerInfo;
					trueSyncBehaviour.numberOfPlayers = lockstep.Players.Count;
					TrueSyncManagedBehaviour trueSyncManagedBehaviour = NewManagedBehavior(trueSyncBehaviour);
					trueSyncManagedBehaviour.owner = trueSyncBehaviour.owner;
					trueSyncManagedBehaviour.localOwner = trueSyncBehaviour.localOwner;
					list.Add(trueSyncManagedBehaviour);
				}
			}
			behaviorsByPlayer.Add(value.ID, list);
		}
	}

	private void initGeneralBehaviors(IEnumerable<TrueSyncManagedBehaviour> behaviours, bool realOwnerId)
	{
		List<TSPlayer> list = new List<TSPlayer>(lockstep.Players.Values);
		List<TrueSyncManagedBehaviour> list2 = new List<TrueSyncManagedBehaviour>();
		IEnumerator<TrueSyncManagedBehaviour> enumerator = behaviours.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TrueSyncManagedBehaviour current = enumerator.Current;
			if (current.trueSyncBehavior is TrueSyncBehaviour)
			{
				TrueSyncBehaviour trueSyncBehaviour = (TrueSyncBehaviour)current.trueSyncBehavior;
				if (realOwnerId)
				{
					trueSyncBehaviour.ownerIndex = trueSyncBehaviour.owner.Id;
				}
				else if (trueSyncBehaviour.ownerIndex >= 0 && trueSyncBehaviour.ownerIndex < list.Count)
				{
					trueSyncBehaviour.ownerIndex = list[trueSyncBehaviour.ownerIndex].ID;
				}
				if (behaviorsByPlayer.ContainsKey((byte)trueSyncBehaviour.ownerIndex))
				{
					trueSyncBehaviour.owner = lockstep.Players[(byte)trueSyncBehaviour.ownerIndex].playerInfo;
					behaviorsByPlayer[(byte)trueSyncBehaviour.ownerIndex].Add(current);
					list2.Add(current);
				}
				else
				{
					trueSyncBehaviour.ownerIndex = -1;
				}
				trueSyncBehaviour.localOwner = lockstep.LocalPlayer.playerInfo;
				trueSyncBehaviour.numberOfPlayers = lockstep.Players.Count;
				current.owner = trueSyncBehaviour.owner;
				current.localOwner = trueSyncBehaviour.localOwner;
			}
		}
		int i = 0;
		for (int count = list2.Count; i < count; i++)
		{
			generalBehaviours.Remove(list2[i]);
		}
	}

	private void CheckQueuedBehaviours()
	{
		if (queuedBehaviours.Count > 0)
		{
			generalBehaviours.AddRange(queuedBehaviours);
			initGeneralBehaviors(queuedBehaviours, realOwnerId: true);
			int i = 0;
			for (int count = queuedBehaviours.Count; i < count; i++)
			{
				TrueSyncManagedBehaviour trueSyncManagedBehaviour = queuedBehaviours[i];
				trueSyncManagedBehaviour.SetGameInfo(lockstep.LocalPlayer.playerInfo, lockstep.Players.Count);
				trueSyncManagedBehaviour.OnSyncedStart();
			}
			queuedBehaviours.Clear();
		}
	}

	private void Update()
	{
		if (lockstep != null && startState != StartState.STARTED)
		{
			if (startState == StartState.BEHAVIOR_INITIALIZED)
			{
				startState = StartState.FIRST_UPDATE;
			}
			else if (startState == StartState.FIRST_UPDATE)
			{
				lockstep.RunSimulation(firstRun: true);
				startState = StartState.STARTED;
			}
		}
	}

	public static void RunSimulation()
	{
		if (instance != null && instance.lockstep != null)
		{
			instance.lockstep.RunSimulation(firstRun: false);
		}
	}

	public static void PauseSimulation()
	{
		if (instance != null && instance.lockstep != null)
		{
			instance.lockstep.PauseSimulation();
		}
	}

	public static void EndSimulation()
	{
		if (instance != null && instance.lockstep != null)
		{
			instance.lockstep.EndSimulation();
		}
	}

	public static void UpdateCoroutines()
	{
		if (instance != null && instance.lockstep != null)
		{
			instance.scheduler.UpdateAllCoroutines();
		}
	}

	public static void SyncedStartCoroutine(IEnumerator coroutine)
	{
		if (instance != null && instance.lockstep != null)
		{
			instance.scheduler.StartCoroutine(coroutine);
		}
	}

	public static GameObject SyncedInstantiate(GameObject prefab)
	{
		return SyncedInstantiate(prefab, prefab.transform.localPosition.ToTSVector(), prefab.transform.rotation.ToTSQuaternion());
	}

	public static GameObject SyncedInstantiate(GameObject prefab, TSVector position, TSQuaternion rotation)
	{
		if (instance != null && instance.lockstep != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, position.ToVector(), rotation.ToQuaternion());
			if (ReplayRecord.replayMode != ReplayMode.LOAD_REPLAY)
			{
				AddGameObjectOnSafeMap(gameObject);
			}
			MonoBehaviour[] componentsInChildren = gameObject.GetComponentsInChildren<MonoBehaviour>();
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				MonoBehaviour monoBehaviour = componentsInChildren[i];
				if (monoBehaviour is ITrueSyncBehaviour)
				{
					instance.queuedBehaviours.Add(instance.NewManagedBehavior((ITrueSyncBehaviour)monoBehaviour));
				}
			}
			InitializeGameObject(gameObject, position, rotation);
			return gameObject;
		}
		Debug.Log(instance.lockstep);
		return null;
	}

	private static void AddGameObjectOnSafeMap(GameObject go)
	{
		Dictionary<int, List<GameObject>> dictionary = instance.gameOjectsSafeMap;
		int key = Ticks + 1;
		if (!dictionary.ContainsKey(key))
		{
			dictionary.Add(key, new List<GameObject>());
		}
		dictionary[key].Add(go);
	}

	private static void CheckGameObjectsSafeMap()
	{
		Dictionary<int, List<GameObject>> dictionary = instance.gameOjectsSafeMap;
		int key = Ticks + 1;
		if (dictionary.ContainsKey(key))
		{
			List<GameObject> list = dictionary[key];
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				GameObject gameObject = list[i];
				if (gameObject != null)
				{
					Renderer component = gameObject.GetComponent<Renderer>();
					if (component != null)
					{
						component.enabled = false;
					}
					UnityEngine.Object.Destroy(gameObject);
				}
			}
			list.Clear();
		}
		dictionary.Remove(LastSafeTick);
	}

	private static void InitializeGameObject(GameObject go, TSVector position, TSQuaternion rotation)
	{
		ICollider[] componentsInChildren = go.GetComponentsInChildren<ICollider>();
		if (componentsInChildren != null)
		{
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				PhysicsManager.instance.AddBody(componentsInChildren[i]);
			}
		}
		TSTransform component = go.GetComponent<TSTransform>();
		if (component != null)
		{
			component.Initialize();
			component.position = position;
			component.rotation = rotation;
		}
		TSTransform[] componentsInChildren2 = go.GetComponentsInChildren<TSTransform>();
		if (componentsInChildren2 != null)
		{
			int j = 0;
			for (int num2 = componentsInChildren2.Length; j < num2; j++)
			{
				TSTransform tSTransform = componentsInChildren2[j];
				if (tSTransform != component)
				{
					tSTransform.Initialize();
				}
			}
		}
		TSTransform2D component2 = go.GetComponent<TSTransform2D>();
		if (component2 != null)
		{
			component2.Initialize();
			component2.position = new TSVector2(position.x, position.y);
			component2.rotation = rotation.ToQuaternion().eulerAngles.z;
		}
		TSTransform2D[] componentsInChildren3 = go.GetComponentsInChildren<TSTransform2D>();
		if (componentsInChildren3 == null)
		{
			return;
		}
		int k = 0;
		for (int num3 = componentsInChildren3.Length; k < num3; k++)
		{
			TSTransform2D tSTransform2D = componentsInChildren3[k];
			if (tSTransform2D != component2)
			{
				tSTransform2D.Initialize();
			}
		}
	}

	public static GameObject SyncedInstantiate(GameObject prefab, TSVector2 position, TSQuaternion rotation)
	{
		return SyncedInstantiate(prefab, new TSVector(position.x, position.y, 0), rotation);
	}

	public static void SyncedDestroy(GameObject gameObject)
	{
		if (!(instance != null) || instance.lockstep == null)
		{
			return;
		}
		SyncedDisableBehaviour(gameObject);
		TSCollider[] componentsInChildren = gameObject.GetComponentsInChildren<TSCollider>();
		if (componentsInChildren != null)
		{
			int i = 0;
			for (int num = componentsInChildren.Length; i < num; i++)
			{
				TSCollider tSCollider = componentsInChildren[i];
				DestroyTSRigidBody(tSCollider.gameObject, tSCollider.Body);
			}
		}
		TSCollider2D[] componentsInChildren2 = gameObject.GetComponentsInChildren<TSCollider2D>();
		if (componentsInChildren2 != null)
		{
			int j = 0;
			for (int num2 = componentsInChildren2.Length; j < num2; j++)
			{
				TSCollider2D tSCollider2D = componentsInChildren2[j];
				DestroyTSRigidBody(tSCollider2D.gameObject, tSCollider2D.Body);
			}
		}
	}

	public static void SyncedDisableBehaviour(GameObject gameObject)
	{
		MonoBehaviour[] componentsInChildren = gameObject.GetComponentsInChildren<MonoBehaviour>();
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			MonoBehaviour monoBehaviour = componentsInChildren[i];
			if (monoBehaviour is ITrueSyncBehaviour && instance.mapBehaviorToManagedBehavior.ContainsKey((ITrueSyncBehaviour)monoBehaviour))
			{
				instance.mapBehaviorToManagedBehavior[(ITrueSyncBehaviour)monoBehaviour].disabled = true;
			}
		}
	}

	private static void DestroyTSRigidBody(GameObject tsColliderGO, IBody body)
	{
		tsColliderGO.gameObject.SetActive(value: false);
		instance.lockstep.Destroy(body);
	}

	public static void RegisterITrueSyncBehaviour(ITrueSyncBehaviour trueSyncBehaviour)
	{
		if (instance != null && instance.lockstep != null)
		{
			instance.queuedBehaviours.Add(instance.NewManagedBehavior(trueSyncBehaviour));
		}
	}

	public static void RegisterIsReadyChecker(TrueSyncIsReady IsReadyChecker)
	{
		if (instance != null && instance.lockstep != null)
		{
			AbstractLockstep abstractLockstep = instance.lockstep;
			abstractLockstep.GameIsReady = (TrueSyncIsReady)Delegate.Combine(abstractLockstep.GameIsReady, IsReadyChecker);
		}
	}

	public static void RemovePlayer(int playerId)
	{
		if (!(instance != null) || instance.lockstep == null)
		{
			return;
		}
		List<TrueSyncManagedBehaviour> list = instance.behaviorsByPlayer[(byte)playerId];
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			TrueSyncManagedBehaviour trueSyncManagedBehaviour = list[i];
			trueSyncManagedBehaviour.disabled = true;
			TSCollider[] componentsInChildren = ((TrueSyncBehaviour)trueSyncManagedBehaviour.trueSyncBehavior).gameObject.GetComponentsInChildren<TSCollider>();
			if (componentsInChildren != null)
			{
				int j = 0;
				for (int num = componentsInChildren.Length; j < num; j++)
				{
					TSCollider tSCollider = componentsInChildren[j];
					if (!tSCollider.Body.TSDisabled)
					{
						DestroyTSRigidBody(tSCollider.gameObject, tSCollider.Body);
					}
				}
			}
			TSCollider2D[] componentsInChildren2 = ((TrueSyncBehaviour)trueSyncManagedBehaviour.trueSyncBehavior).gameObject.GetComponentsInChildren<TSCollider2D>();
			if (componentsInChildren2 == null)
			{
				continue;
			}
			int k = 0;
			for (int num2 = componentsInChildren2.Length; k < num2; k++)
			{
				TSCollider2D tSCollider2D = componentsInChildren2[k];
				if (!tSCollider2D.Body.TSDisabled)
				{
					DestroyTSRigidBody(tSCollider2D.gameObject, tSCollider2D.Body);
				}
			}
		}
	}

	private void FixedUpdate()
	{
		if (lockstep != null)
		{
			tsDeltaTime += (FP)UnityEngine.Time.deltaTime;
			if (tsDeltaTime >= lockedTimeStep - 0.001f)
			{
				tsDeltaTime = 0;
				instance.scheduler.UpdateAllCoroutines();
				lockstep.Update();
			}
		}
	}

	private InputDataBase ProvideInputData()
	{
		return new InputData();
	}

	private void GetLocalData(InputDataBase playerInputData)
	{
		TrueSyncInput.CurrentInputData = (InputData)playerInputData;
		if (behaviorsByPlayer.ContainsKey(playerInputData.ownerID))
		{
			List<TrueSyncManagedBehaviour> list = behaviorsByPlayer[playerInputData.ownerID];
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				TrueSyncManagedBehaviour trueSyncManagedBehaviour = list[i];
				if (trueSyncManagedBehaviour != null && !trueSyncManagedBehaviour.disabled)
				{
					trueSyncManagedBehaviour.OnSyncedInput();
				}
			}
		}
		TrueSyncInput.CurrentInputData = null;
	}

	private void OnStepUpdate(List<InputDataBase> allInputData)
	{
		time += lockedTimeStep;
		if (ReplayRecord.replayMode != ReplayMode.LOAD_REPLAY)
		{
			CheckGameObjectsSafeMap();
		}
		TrueSyncInput.SetAllInputs(null);
		int i = 0;
		for (int count = generalBehaviours.Count; i < count; i++)
		{
			TrueSyncManagedBehaviour trueSyncManagedBehaviour = generalBehaviours[i];
			if (trueSyncManagedBehaviour != null && !trueSyncManagedBehaviour.disabled)
			{
				trueSyncManagedBehaviour.OnPreSyncedUpdate();
				instance.scheduler.UpdateAllCoroutines();
			}
		}
		int j = 0;
		for (int count2 = allInputData.Count; j < count2; j++)
		{
			InputDataBase inputDataBase = allInputData[j];
			if (!behaviorsByPlayer.ContainsKey(inputDataBase.ownerID))
			{
				continue;
			}
			List<TrueSyncManagedBehaviour> list = behaviorsByPlayer[inputDataBase.ownerID];
			int k = 0;
			for (int count3 = list.Count; k < count3; k++)
			{
				TrueSyncManagedBehaviour trueSyncManagedBehaviour2 = list[k];
				if (trueSyncManagedBehaviour2 != null && !trueSyncManagedBehaviour2.disabled)
				{
					trueSyncManagedBehaviour2.OnPreSyncedUpdate();
					instance.scheduler.UpdateAllCoroutines();
				}
			}
		}
		TrueSyncInput.SetAllInputs(allInputData);
		TrueSyncInput.CurrentSimulationData = null;
		int l = 0;
		for (int count4 = generalBehaviours.Count; l < count4; l++)
		{
			TrueSyncManagedBehaviour trueSyncManagedBehaviour3 = generalBehaviours[l];
			if (trueSyncManagedBehaviour3 != null && !trueSyncManagedBehaviour3.disabled)
			{
				trueSyncManagedBehaviour3.OnSyncedUpdate();
				instance.scheduler.UpdateAllCoroutines();
			}
		}
		int m = 0;
		for (int count5 = allInputData.Count; m < count5; m++)
		{
			InputDataBase inputDataBase2 = allInputData[m];
			if (behaviorsByPlayer.ContainsKey(inputDataBase2.ownerID))
			{
				TrueSyncInput.CurrentSimulationData = (InputData)inputDataBase2;
				List<TrueSyncManagedBehaviour> list2 = behaviorsByPlayer[inputDataBase2.ownerID];
				int n = 0;
				for (int count6 = list2.Count; n < count6; n++)
				{
					TrueSyncManagedBehaviour trueSyncManagedBehaviour4 = list2[n];
					if (trueSyncManagedBehaviour4 != null && !trueSyncManagedBehaviour4.disabled)
					{
						trueSyncManagedBehaviour4.OnSyncedUpdate();
						instance.scheduler.UpdateAllCoroutines();
					}
				}
			}
			TrueSyncInput.CurrentSimulationData = null;
		}
		CheckQueuedBehaviours();
	}

	private void OnRemovedRigidBody(IBody body)
	{
		GameObject gameObject = PhysicsManager.instance.GetGameObject(body);
		if (gameObject != null)
		{
			List<TrueSyncBehaviour> behaviours = new List<TrueSyncBehaviour>(gameObject.GetComponentsInChildren<TrueSyncBehaviour>());
			RemoveFromTSMBList(queuedBehaviours, behaviours);
			RemoveFromTSMBList(generalBehaviours, behaviours);
			Dictionary<byte, List<TrueSyncManagedBehaviour>>.Enumerator enumerator = behaviorsByPlayer.GetEnumerator();
			while (enumerator.MoveNext())
			{
				List<TrueSyncManagedBehaviour> value = enumerator.Current.Value;
				RemoveFromTSMBList(value, behaviours);
			}
		}
	}

	private void RemoveFromTSMBList(List<TrueSyncManagedBehaviour> tsmbList, List<TrueSyncBehaviour> behaviours)
	{
		List<TrueSyncManagedBehaviour> list = new List<TrueSyncManagedBehaviour>();
		int i = 0;
		for (int count = tsmbList.Count; i < count; i++)
		{
			TrueSyncManagedBehaviour trueSyncManagedBehaviour = tsmbList[i];
			if (trueSyncManagedBehaviour.trueSyncBehavior is TrueSyncBehaviour && behaviours.Contains((TrueSyncBehaviour)trueSyncManagedBehaviour.trueSyncBehavior))
			{
				list.Add(trueSyncManagedBehaviour);
			}
		}
		int j = 0;
		for (int count2 = list.Count; j < count2; j++)
		{
			TrueSyncManagedBehaviour item = list[j];
			tsmbList.Remove(item);
		}
	}

	public static void CleanUp()
	{
		ResourcePool.CleanUpAll();
		StateTracker.CleanUp();
		instance = null;
	}

	private void OnPlayerDisconnection(byte playerId)
	{
		TrueSyncManagedBehaviour.OnPlayerDisconnection(generalBehaviours, behaviorsByPlayer, playerId);
	}

	private void OnGameStarted()
	{
		TrueSyncManagedBehaviour.OnGameStarted(generalBehaviours, behaviorsByPlayer);
		instance.scheduler.UpdateAllCoroutines();
		CheckQueuedBehaviours();
	}

	private void OnGamePaused()
	{
		TrueSyncManagedBehaviour.OnGamePaused(generalBehaviours, behaviorsByPlayer);
		instance.scheduler.UpdateAllCoroutines();
	}

	private void OnGameUnPaused()
	{
		TrueSyncManagedBehaviour.OnGameUnPaused(generalBehaviours, behaviorsByPlayer);
		instance.scheduler.UpdateAllCoroutines();
	}

	private void OnGameEnded()
	{
		TrueSyncManagedBehaviour.OnGameEnded(generalBehaviours, behaviorsByPlayer);
		instance.scheduler.UpdateAllCoroutines();
	}

	private void OnApplicationQuit()
	{
		EndSimulation();
	}
}
