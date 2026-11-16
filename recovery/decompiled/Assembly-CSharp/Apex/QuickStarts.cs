using System;
using Apex.Debugging;
using Apex.Input;
using Apex.LoadBalancing;
using Apex.PathFinding;
using Apex.Services;
using Apex.Steering;
using Apex.Steering.Behaviours;
using Apex.Steering.Components;
using Apex.Steering.HeightNavigation;
using Apex.Steering.VectorFields;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex;

public static class QuickStarts
{
	public static GameObject GameWorld(GameObject target)
	{
		GameObject gameObject = null;
		Component component = ComponentHelper.FindFirstComponentInScene<GameServicesInitializerComponent>();
		if (component == null)
		{
			component = ComponentHelper.FindFirstComponentInScene<LoadBalancerComponent>();
		}
		if (component != null)
		{
			gameObject = component.gameObject;
			Debug.Log("Existing game world found, updating that.");
		}
		else if (target != null)
		{
			gameObject = target;
		}
		else
		{
			gameObject = new GameObject("Game World");
			Debug.Log("No game world found, creating one.");
		}
		bool flag = gameObject.AddIfMissing<ApexComponentMaster>(entireScene: false, out var component2);
		gameObject.AddIfMissing<GameServicesInitializerComponent>(entireScene: true);
		gameObject.AddIfMissing<NavigationSettingsComponent>(entireScene: true);
		if (gameObject.AddIfMissing<GridComponent>(entireScene: true))
		{
			gameObject.AddIfMissing<GridVisualizer>(entireScene: false);
		}
		if (gameObject.AddIfMissing<LayerMappingComponent>(entireScene: true, out var component3))
		{
			EnsureLayers(component3);
		}
		gameObject.AddIfMissing<PathServiceComponent>(entireScene: true);
		gameObject.AddIfMissing<LoadBalancerComponent>(entireScene: true);
		ExtendGameWorld(gameObject);
		if (flag)
		{
			component2.ToggleAll();
		}
		return gameObject;
	}

	public static GameObject NavigatingUnit(GameObject target, bool ensureGameworld)
	{
		target.AddIfMissing<Rigidbody>(entireScene: false, out var component);
		bool flag = target.AddIfMissing<ApexComponentMaster>(entireScene: false, out var component2);
		target.AddIfMissing<HumanoidSpeedComponent>(entireScene: false);
		target.AddIfMissing<UnitComponent>(entireScene: false);
		target.AddIfMissing<SteerableUnitComponent>(entireScene: false);
		target.AddIfMissing<DefaultHeightNavigator>(entireScene: false);
		target.AddIfMissing<PathOptionsComponent>(entireScene: false);
		target.AddIfMissing<SteerToAlignWithVelocity>(entireScene: false);
		AddIfMissing<SteerForPathComponent>(target, 5);
		target.AddIfMissing<PathVisualizer>(entireScene: false);
		ExtendNavigatingUnit(target);
		if (flag)
		{
			component2.ToggleAll();
		}
		if (component != null)
		{
			component.constraints |= RigidbodyConstraints.FreezeRotation;
			component.useGravity = false;
		}
		if (ensureGameworld)
		{
			return GameWorld(null);
		}
		return null;
	}

	public static void NavigatingUnitOnPatrol(GameObject target, bool ensureGameworld)
	{
		GameObject gameObject = NavigatingUnit(target, ensureGameworld);
		if (target.AddIfMissing<PatrolComponent>(entireScene: false, out var component) && !(gameObject == null))
		{
			int num = gameObject.GetComponentsInChildren<PatrolPointsComponent>(includeInactive: true).Length + 1;
			GameObject gameObject2 = new GameObject("Patrol Route " + num);
			gameObject2.transform.parent = gameObject.transform;
			PatrolPointsComponent patrolPointsComponent = gameObject2.AddComponent<PatrolPointsComponent>();
			patrolPointsComponent.points = new Vector3[2]
			{
				target.transform.position,
				target.transform.position + Vector3.forward * 8f
			};
			component.route = patrolPointsComponent;
		}
	}

	public static void NavigatingUnitWandering(GameObject target, bool ensureGameworld)
	{
		NavigatingUnit(target, ensureGameworld);
		target.AddIfMissing<WanderComponent>(entireScene: false);
	}

	public static void NavigatingUnitWithSelection(GameObject target, bool ensureGameworld)
	{
		GameObject gameObject = NavigatingUnit(target, ensureGameworld);
		target.AddIfMissing<UnitComponent>(entireScene: false, out var component);
		Transform transform = target.transform.Find("SelectionVisual");
		GameObject gameObject3;
		if (transform == null)
		{
			GameObject gameObject2 = Resources.Load<GameObject>("Prefabs/UnitSelectedCustom");
			if (gameObject2 == null)
			{
				gameObject2 = Resources.Load<GameObject>("Prefabs/UnitSelected");
			}
			gameObject3 = UnityEngine.Object.Instantiate(gameObject2);
			gameObject3.name = "SelectionVisual";
			transform = gameObject3.transform;
			transform.parent = target.transform;
			transform.localPosition = Vector3.zero;
			transform.localScale = new Vector3(2f, 2f, 2f);
		}
		else
		{
			gameObject3 = transform.gameObject;
		}
		gameObject3.SetActive(value: false);
		component.selectionVisual = gameObject3;
		component.isSelectable = true;
		if (gameObject == null)
		{
			return;
		}
		AddIfMissing<InputReceiverBasic, InputReceiverAttribute>(gameObject, globalSearch: true);
		if (gameObject.GetComponentInChildren<SelectionRectangleComponent>() == null)
		{
			GameObject gameObject4 = Resources.Load<GameObject>("Prefabs/SelectionRectCustom");
			if (gameObject4 == null)
			{
				gameObject4 = Resources.Load<GameObject>("Prefabs/SelectionRect");
			}
			GameObject gameObject5 = UnityEngine.Object.Instantiate(gameObject4);
			gameObject5.transform.parent = gameObject.transform;
		}
	}

	public static void BasicAvoidance(GameObject target, bool ensureGameworld)
	{
		NavigatingUnit(target, ensureGameworld);
		target.AddIfMissing<BasicScanner>(entireScene: false);
		AddIfMissing<SteerForBasicAvoidanceComponent>(target, 5);
	}

	public static GameObject Portal(GameObject target)
	{
		int num = target.GetComponentsInChildren<GridPortalComponent>(includeInactive: true).Length + 1;
		Transform transform = target.transform;
		target = new GameObject("Portal " + num);
		target.transform.parent = transform;
		target.transform.localPosition = Vector3.zero;
		target.AddComponent<GridPortalComponent>();
		return target;
	}

	public static GameObject PatrolRoute(GameObject target)
	{
		int num = target.GetComponentsInChildren<PatrolPointsComponent>(includeInactive: true).Length + 1;
		GameObject gameObject = new GameObject("Patrol Route " + num);
		gameObject.transform.parent = target.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.AddComponent<PatrolPointsComponent>();
		return gameObject;
	}

	private static bool AddIfMissing<T>(GameObject target, int priority) where T : SteeringComponent
	{
		if (target.AddIfMissing<T>(entireScene: false, out var component))
		{
			component.priority = priority;
			return true;
		}
		return false;
	}

	private static bool AddIfMissing<T, TMarkerAttribute>(GameObject target, bool globalSearch, out T component) where T : Component where TMarkerAttribute : Attribute
	{
		component = (T)null;
		UnityEngine.Object[] array = ((!globalSearch) ? target.GetComponents<MonoBehaviour>() : UnityEngine.Object.FindObjectsOfType(typeof(MonoBehaviour)));
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object obj in array2)
		{
			if (Attribute.IsDefined(obj.GetType(), typeof(TMarkerAttribute)))
			{
				return false;
			}
		}
		component = target.AddComponent<T>();
		return true;
	}

	private static bool AddIfMissing<T, TMarkerAttribute>(GameObject target, bool globalSearch) where T : Component where TMarkerAttribute : Attribute
	{
		T component;
		return AddIfMissing<T, TMarkerAttribute>(target, globalSearch, out component);
	}

	private static void EnsureLayers(LayerMappingComponent lmc)
	{
		Debug.LogWarning("Unable to set up default layers, please ensure that layer mapping is properly set.");
	}

	public static void VectorFieldNavigation(GameObject target, bool ensureGameworld)
	{
		GameObject gameObject = NavigatingUnit(target, ensureGameworld);
		AddIfMissing<SteerForVectorFieldComponent>(target, 5);
		AddIfMissing<SteerForFormationComponent>(target, 10);
		AddIfMissing<SteerForBlockedCellRepulsionComponent>(target, 20);
		AddIfMissing<SteerForContainmentComponent>(target, 20);
		target.AddIfMissing<SteeringController>(entireScene: false);
		if (!(gameObject == null))
		{
			gameObject.AddIfMissing<VectorFieldManagerComponent>(entireScene: true);
			if (gameObject.As<IUnitGroupingStrategyFactory>() == null)
			{
				gameObject.AddComponent<SteeringUnitGroupingStrategyFactory>();
			}
		}
	}

	private static void ExtendGameWorld(GameObject gameWorld)
	{
		gameWorld.AddIfMissing<VectorFieldManagerComponent>(entireScene: true);
		if (gameWorld.As<IUnitGroupingStrategyFactory>() == null)
		{
			gameWorld.AddComponent<SteeringUnitGroupingStrategyFactory>();
		}
	}

	private static void ExtendNavigatingUnit(GameObject target)
	{
		target.NukeSingle<BasicScanner>();
		target.NukeSingle<SteerForBasicAvoidanceComponent>();
		target.AddIfMissing<SteeringScanner>(entireScene: false);
		AddIfMissing<SteerForSeparationComponent>(target, 9);
		AddIfMissing<SteerForUnitAvoidanceComponent>(target, 15);
	}
}
