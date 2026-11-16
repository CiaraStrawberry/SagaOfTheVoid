using System;
using Apex.Common;
using Apex.Debugging;
using Apex.Messages;
using Apex.Services;
using UnityEngine;

namespace Apex.WorldGeometry;

[AddComponentMenu("Apex/Game World/Portals/Portal", 1019)]
[ApexComponent("Game World")]
public class GridPortalComponent : ExtendedMonoBehaviour, IHandleMessage<GridStatusMessage>
{
	[Flags]
	private enum ActiveGrids : byte
	{
		None = 0,
		One = 1,
		Two = 2,
		Both = One | Two
	}

	private class RuntimeGridPortalComponent : GridPortalComponent
	{
		internal void Configure(PortalType type, PortalDirection direction, Bounds portalOne, Bounds portalTwo, bool relativeToTransform, string portalName)
		{
			base.type = type;
			base.direction = direction;
			base.portalOne = portalOne;
			base.portalTwo = portalTwo;
			base.relativeToTransform = relativeToTransform;
			base.portalName = portalName;
			base.Awake();
			OnStartAndEnable();
		}

		protected override void Awake()
		{
		}
	}

	[Tooltip("The portal's name, which can be used to look up the portal through the GridManager.")]
	public string portalName;

	[Tooltip("Connector portals are mainly for connecting grids, but can also be sued inside one grid. Shortcuts are as there name suggests, short cuts and are evaluated differently.")]
	public PortalType type;

	[Tooltip("Two-way portals are usable from both end points, one-way portals are only usable from one end point.")]
	public PortalDirection direction;

	public Bounds portalOne;

	public Bounds portalTwo;

	[Tooltip("Controls whether the portal end points are seen as relative to their parent transform. IF relative they will move with the transform otherwise they will be static and remain where they were placed initially.")]
	public bool relativeToTransform;

	[Tooltip("Whether to always draw the visualization gizmos, even when the portal's gameobject is not selected.\nNote a Portal Visualizer can be added to the game world to control this for all portals.")]
	public bool drawGizmosAlways;

	[Tooltip("The Gizmo color of portal one.")]
	public Color portalOneColor = new Color(0f, 150f / 255f, 211f / 255f, 100f / 255f);

	[Tooltip("The Gizmo color of portal one.")]
	public Color portalTwoColor = new Color(0f, 211f / 255f, 150f / 255f, 100f / 255f);

	[Tooltip("The Gizmo color of the portal connection.")]
	public Color connectionColor = new Color(155f, 150f / 255f, 100f / 255f, 1f);

	[SerializeField]
	[AttributeProperty("Exclusive To", "If set only units that have one or more of the specified attributes can use the portal.")]
	private int _exclusiveTo;

	private GridPortal _portal;

	private ActiveGrids _activeGrids;

	public AttributeMask exclusiveTo
	{
		get
		{
			return _exclusiveTo;
		}
		set
		{
			_exclusiveTo = value;
			if (_portal != null)
			{
				_portal.exclusiveTo = value;
			}
		}
	}

	public GridPortal portal => _portal;

	private Bounds actualPortalOne => (!relativeToTransform) ? portalOne : new Bounds(transform.TransformPoint(portalOne.center), portalOne.size);

	private Bounds actualPortalTwo => (!relativeToTransform) ? portalTwo : new Bounds(transform.TransformPoint(portalTwo.center), portalTwo.size);

	public static GridPortalComponent Create<TAction>(GameObject host, PortalType type, Bounds portalOne, Bounds portalTwo, out TAction action) where TAction : Component, IPortalAction
	{
		return Create<TAction>(host, type, PortalDirection.Twoway, portalOne, portalTwo, relativeToTransform: false, string.Empty, out action);
	}

	public static GridPortalComponent Create<TAction>(GameObject host, PortalType type, Bounds portalOne, Bounds portalTwo, bool relativeToTransform, out TAction action) where TAction : Component, IPortalAction
	{
		return Create<TAction>(host, type, PortalDirection.Twoway, portalOne, portalTwo, relativeToTransform, string.Empty, out action);
	}

	public static GridPortalComponent Create<TAction>(GameObject host, PortalType type, Bounds portalOne, Bounds portalTwo, string portalName, out TAction action) where TAction : Component, IPortalAction
	{
		return Create<TAction>(host, type, PortalDirection.Twoway, portalOne, portalTwo, relativeToTransform: false, portalName, out action);
	}

	public static GridPortalComponent Create<TAction>(GameObject host, PortalType type, Bounds portalOne, Bounds portalTwo, bool relativeToTransform, string portalName, out TAction action) where TAction : Component, IPortalAction
	{
		return Create<TAction>(host, type, PortalDirection.Twoway, portalOne, portalTwo, relativeToTransform, portalName, out action);
	}

	public static GridPortalComponent Create<TAction>(GameObject host, PortalType type, PortalDirection direction, Bounds portalOne, Bounds portalTwo, bool relativeToTransform, string portalName, out TAction action) where TAction : Component, IPortalAction
	{
		host.AddIfMissing<TAction>(out action);
		RuntimeGridPortalComponent runtimeGridPortalComponent = host.AddComponent<RuntimeGridPortalComponent>();
		runtimeGridPortalComponent.Configure(type, direction, portalOne, portalTwo, relativeToTransform, portalName);
		return runtimeGridPortalComponent;
	}

	public static GridPortalComponent Create<TAction>(GameObject host, PortalType type, Bounds portalOne, Bounds portalTwo) where TAction : Component, IPortalAction
	{
		return Create<TAction>(host, type, PortalDirection.Twoway, portalOne, portalTwo, relativeToTransform: false, string.Empty);
	}

	public static GridPortalComponent Create<TAction>(GameObject host, PortalType type, Bounds portalOne, Bounds portalTwo, bool relativeToTransform) where TAction : Component, IPortalAction
	{
		return Create<TAction>(host, type, PortalDirection.Twoway, portalOne, portalTwo, relativeToTransform, string.Empty);
	}

	public static GridPortalComponent Create<TAction>(GameObject host, PortalType type, Bounds portalOne, Bounds portalTwo, string portalName) where TAction : Component, IPortalAction
	{
		return Create<TAction>(host, type, PortalDirection.Twoway, portalOne, portalTwo, relativeToTransform: false, portalName);
	}

	public static GridPortalComponent Create<TAction>(GameObject host, PortalType type, Bounds portalOne, Bounds portalTwo, bool relativeToTransform, string portalName) where TAction : Component, IPortalAction
	{
		return Create<TAction>(host, type, PortalDirection.Twoway, portalOne, portalTwo, relativeToTransform, portalName);
	}

	public static GridPortalComponent Create<TAction>(GameObject host, PortalType type, PortalDirection direction, Bounds portalOne, Bounds portalTwo, bool relativeToTransform, string portalName) where TAction : Component, IPortalAction
	{
		host.AddIfMissing<TAction>(out var _);
		RuntimeGridPortalComponent runtimeGridPortalComponent = host.AddComponent<RuntimeGridPortalComponent>();
		runtimeGridPortalComponent.Configure(type, direction, portalOne, portalTwo, relativeToTransform, portalName);
		return runtimeGridPortalComponent;
	}

	public static GridPortalComponent CreateConnector(GameObject host, Bounds portalOne, Bounds portalTwo)
	{
		return CreateConnector(host, PortalDirection.Twoway, portalOne, portalTwo, relativeToTransform: false);
	}

	public static GridPortalComponent CreateConnector(GameObject host, Bounds portalOne, Bounds portalTwo, bool relativeToTransform)
	{
		return CreateConnector(host, PortalDirection.Twoway, portalOne, portalTwo, relativeToTransform);
	}

	public static GridPortalComponent CreateConnector(GameObject host, PortalDirection direction, Bounds portalOne, Bounds portalTwo, bool relativeToTransform)
	{
		RuntimeGridPortalComponent runtimeGridPortalComponent = host.AddComponent<RuntimeGridPortalComponent>();
		runtimeGridPortalComponent.Configure(PortalType.Connector, direction, portalOne, portalTwo, relativeToTransform, string.Empty);
		return runtimeGridPortalComponent;
	}

	internal bool Connects(GridComponent grid)
	{
		Bounds bounds = grid.bounds;
		return bounds.Contains(actualPortalOne.center) || bounds.Contains(actualPortalTwo.center);
	}

	internal bool Connects(IGrid grid)
	{
		Bounds bounds = grid.bounds;
		return bounds.Contains(actualPortalOne.center) || bounds.Contains(actualPortalTwo.center);
	}

	internal bool Connects(GridComponent gridOne, GridComponent gridTwo)
	{
		Bounds bounds = gridOne.bounds;
		Bounds bounds2 = gridTwo.bounds;
		return (bounds.Contains(actualPortalOne.center) || bounds.Contains(actualPortalTwo.center)) && (bounds2.Contains(actualPortalOne.center) || bounds2.Contains(actualPortalTwo.center));
	}

	internal bool Connects(IGrid gridOne, IGrid gridTwo)
	{
		Bounds bounds = gridOne.bounds;
		Bounds bounds2 = gridTwo.bounds;
		return (bounds.Contains(actualPortalOne.center) || bounds.Contains(actualPortalTwo.center)) && (bounds2.Contains(actualPortalOne.center) || bounds2.Contains(actualPortalTwo.center));
	}

	protected virtual void Awake()
	{
		if (GridManager.instance.GetGrid(actualPortalOne.center) != null)
		{
			_activeGrids |= ActiveGrids.One;
		}
		if (GridManager.instance.GetGrid(actualPortalTwo.center) != null)
		{
			_activeGrids |= ActiveGrids.Two;
		}
		if (_activeGrids == ActiveGrids.Both)
		{
			Initialize();
		}
		GridManager.instance.RegisterPortalComponent(this);
		GameServices.messageBus.Subscribe(this);
	}

	protected override void OnStartAndEnable()
	{
		if (_portal != null)
		{
			_portal.enabled = true;
		}
	}

	private void OnDisable()
	{
		if (_portal != null)
		{
			_portal.enabled = false;
		}
	}

	private void OnDestroy()
	{
		GridManager.instance.UnregisterPortalComponent(this);
		GameServices.messageBus.Unsubscribe(this);
	}

	private void OnDrawGizmos()
	{
		if (enabled && (drawGizmosAlways || (PortalVisualizer.instance != null && PortalVisualizer.instance.drawAllPortals)))
		{
			DrawVisualization();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!drawGizmosAlways && (!(PortalVisualizer.instance != null) || !PortalVisualizer.instance.drawAllPortals))
		{
			DrawVisualization();
		}
	}

	private void DrawVisualization()
	{
		Bounds bounds = actualPortalOne;
		Bounds bounds2 = actualPortalTwo;
		Gizmos.color = portalOneColor;
		Gizmos.DrawCube(bounds.center, bounds.size);
		Gizmos.color = portalTwoColor;
		Gizmos.DrawCube(bounds2.center, bounds2.size);
		if (!Application.isPlaying || (_portal != null && _portal.enabled))
		{
			Gizmos.color = connectionColor;
			Gizmos.DrawLine(bounds.center, bounds2.center);
			if (direction == PortalDirection.Twoway)
			{
				Gizmos.DrawSphere(bounds.center, 0.5f);
				Gizmos.DrawSphere(bounds2.center, 0.5f);
			}
			else
			{
				Gizmos.DrawSphere(bounds.center, 0.5f);
			}
		}
	}

	private void Initialize()
	{
		IPortalAction portalAction;
		if (type != PortalType.Connector)
		{
			portalAction = this.As<IPortalAction>();
			if (portalAction == null)
			{
				IPortalActionFactory portalActionFactory = this.As<IPortalActionFactory>();
				if (portalActionFactory != null)
				{
					portalAction = portalActionFactory.Create();
				}
			}
			if (portalAction == null)
			{
				Debug.LogError("A portal must have an accompanying portal action component, please add one.");
				enabled = false;
				return;
			}
		}
		else
		{
			portalAction = PortalConnectorAction.instance;
		}
		if (portalOne.size.sqrMagnitude == 0f || portalTwo.size.sqrMagnitude == 0f)
		{
			Debug.LogError("A portal's end points must have a size greater than 0.");
			enabled = false;
			return;
		}
		try
		{
			_portal = GridPortal.Create(portalName, type, direction, actualPortalOne, actualPortalTwo, exclusiveTo, portalAction);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
			enabled = false;
		}
	}

	void IHandleMessage<GridStatusMessage>.Handle(GridStatusMessage message)
	{
		Bounds gridBounds = message.gridBounds;
		ActiveGrids activeGrids = _activeGrids;
		ActiveGrids activeGrids2 = ActiveGrids.None;
		if (gridBounds.Contains(actualPortalOne.center))
		{
			activeGrids2 |= ActiveGrids.One;
		}
		if (gridBounds.Contains(actualPortalTwo.center))
		{
			activeGrids2 |= ActiveGrids.Two;
		}
		if (activeGrids2 == ActiveGrids.None)
		{
			return;
		}
		switch (message.status)
		{
		case GridStatusMessage.StatusCode.DisableComplete:
			_activeGrids &= (ActiveGrids)(byte)(~(int)activeGrids2);
			break;
		case GridStatusMessage.StatusCode.InitializationComplete:
			_activeGrids |= activeGrids2;
			break;
		}
		if (_activeGrids == activeGrids)
		{
			return;
		}
		if (_activeGrids == ActiveGrids.Both)
		{
			Initialize();
			if (_portal != null)
			{
				_portal.enabled = enabled;
			}
		}
		else if (_portal != null)
		{
			GridManager.instance.UnregisterPortal(_portal.name);
			_portal.enabled = false;
			_portal = null;
		}
	}
}
