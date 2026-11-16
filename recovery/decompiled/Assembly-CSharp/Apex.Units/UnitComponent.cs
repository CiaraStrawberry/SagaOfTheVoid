using Apex.Common;
using Apex.Services;
using Apex.Steering;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Units;

[AddComponentMenu("Apex/Unit/Navigation/Unit", 1032)]
[ApexComponent("Unit Properties")]
public class UnitComponent : AttributedComponent, IUnitProperties, IPositioned, IHaveAttributes
{
	private Transform _transform;

	private bool _isSelected;

	private bool? _selectPending;

	private HeightNavigationCapabilities _effectiveHeightCapabilities;

	[SerializeField]
	[Tooltip("The determination factor is used to evaluate whether this unit separates or avoids other units. Units with lower determination are basically ignored.")]
	private int _determination = 1;

	[SerializeField]
	[Tooltip("Controls whether this unit is selectable via user input such as a mouse.")]
	private bool _isSelectable;

	[SerializeField]
	private HeightNavigationCapabilities _heightCapabilities = new HeightNavigationCapabilities
	{
		maxClimbHeight = 0.5f,
		maxDropHeight = 1f,
		maxSlopeAngle = 30f
	};

	[Tooltip("The visual used to indicate whether the unit is selected or not.")]
	public GameObject selectionVisual;

	[MinCheck(0f, label = "Radius", tooltip = "The radius of the unit.")]
	public float radius = 0.5f;

	[RangeX(0f, 360f, label = "Field of View", tooltip = "The field of view in degrees.")]
	public float fieldOfView = 200f;

	[MinCheck(0f, label = "Ground Offset", tooltip = "The distance the unit will hover above the ground as seen from the bottom of its collider/mesh.")]
	public float yAxisoffset;

	float IUnitProperties.radius => radius;

	float IUnitProperties.fieldOfView => fieldOfView;

	public Vector3 position => _transform.position;

	public Vector3 forward => _transform.forward;

	public bool isSelectable
	{
		get
		{
			return _isSelectable;
		}
		set
		{
			_isSelectable = value;
		}
	}

	public int determination
	{
		get
		{
			return _determination;
		}
		set
		{
			_determination = value;
		}
	}

	public bool isSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			_selectPending = null;
			if (_isSelected != value)
			{
				_isSelected = value;
				if (selectionVisual != null)
				{
					selectionVisual.SetActive(value);
				}
			}
		}
	}

	public float groundOffset => yAxisoffset;

	public float baseToPositionOffset { get; private set; }

	public float height { get; private set; }

	public Vector3 basePosition
	{
		get
		{
			Vector3 result = _transform.position;
			result.y -= baseToPositionOffset;
			return result;
		}
	}

	public HeightNavigationCapabilities heightNavigationCapability => _effectiveHeightCapabilities;

	public void RecalculateBasePosition()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		Collider component2 = GetComponent<Collider>();
		Renderer component3 = GetComponent<Renderer>();
		if (component != null && component.useGravity)
		{
			yAxisoffset = 0f;
		}
		float num;
		if (component2 != null)
		{
			ColliderMeasurement colliderMeasurement = new ColliderMeasurement(component2, inWorldSpace: true);
			num = colliderMeasurement.extents.y - colliderMeasurement.transformCenterOffset.y + yAxisoffset;
			height = colliderMeasurement.size.y + yAxisoffset;
			if (component != null && component.useGravity)
			{
				num -= 2f * component2.contactOffset;
			}
		}
		else if (component3 != null)
		{
			float num2 = position.y - component3.bounds.center.y;
			num = component3.bounds.extents.y + num2 + yAxisoffset;
			height = component3.bounds.size.y + yAxisoffset;
		}
		else
		{
			num = yAxisoffset;
		}
		baseToPositionOffset = num;
	}

	public void MarkSelectPending(bool pending)
	{
		if (pending != _selectPending)
		{
			_selectPending = pending;
			if (selectionVisual != null)
			{
				selectionVisual.SetActive(pending);
			}
		}
	}

	private void Awake()
	{
		this.WarnIfMultipleInstances<IUnitProperties>();
		_transform = transform;
		HeightStrategy heightStrategy = GameServices.heightStrategy;
		_effectiveHeightCapabilities = ((!heightStrategy.useGlobalHeightNavigationSettings) ? _heightCapabilities : heightStrategy.unitsHeightNavigationCapability);
		GameServices.gameStateManager.RegisterUnit(gameObject);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		_isSelected = true;
		isSelected = false;
		RecalculateBasePosition();
		Vector3 vector = basePosition;
		Vector3 origin = vector;
		origin.y += height;
		if (Physics.Raycast(origin, Vector3.down, out var hitInfo, float.PositiveInfinity, Layers.terrain))
		{
			float num = hitInfo.point.y - vector.y;
			if (num > 0f)
			{
				Vector3 vector2 = _transform.position;
				vector2.y += num;
				_transform.position = vector2;
			}
		}
	}

	private void OnDestroy()
	{
		GameServices.gameStateManager.UnregisterUnit(gameObject);
	}

	public void CloneFrom(UnitComponent unitComp)
	{
		_isSelectable = unitComp.isSelectable;
		selectionVisual = unitComp.selectionVisual;
		attributes = unitComp.attributes;
		_heightCapabilities = unitComp._heightCapabilities;
		_effectiveHeightCapabilities = unitComp._effectiveHeightCapabilities;
		radius = unitComp.radius;
		fieldOfView = unitComp.fieldOfView;
		baseToPositionOffset = unitComp.baseToPositionOffset;
		height = unitComp.height;
		yAxisoffset = unitComp.yAxisoffset;
		determination = unitComp.determination;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}
