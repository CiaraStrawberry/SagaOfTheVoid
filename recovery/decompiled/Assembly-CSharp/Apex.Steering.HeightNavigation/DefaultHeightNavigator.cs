using Apex.Services;
using Apex.Utilities;
using UnityEngine;

namespace Apex.Steering.HeightNavigation;

[AddComponentMenu("Apex/Unit/Navigation/Default Height Navigator", 1026)]
[ApexComponent("Steering")]
public class DefaultHeightNavigator : MonoBehaviour, IHeightNavigator
{
	[SerializeField]
	[Tooltip("The gravity acceleration")]
	private float _gravity = -9.81f;

	[SerializeField]
	private ProviderType _providerType;

	private IUnitHeightProvider _heightProvider;

	private float _gravitationVelocity;

	[MinCheck(0f, tooltip = "Controls to what degree a unit will attempt to stick to the ground when moving down slopes.")]
	public float groundStickynessFactor = 1f;

	[Tooltip("The terminal velocity, i.e. maximum fall speed (m/s)")]
	public float terminalVelocity = 50f;

	public float gravity
	{
		get
		{
			return _gravity;
		}
		set
		{
			_gravity = value;
		}
	}

	public ProviderType provider
	{
		get
		{
			return _providerType;
		}
		set
		{
			_providerType = value;
			_heightProvider = ResolveProvider();
		}
	}

	public IUnitHeightProvider heightProvider
	{
		get
		{
			return _heightProvider;
		}
		set
		{
			_heightProvider = value;
		}
	}

	public HeightOutput GetHeightOutput(SteeringInput input, float effectiveMaxSpeed)
	{
		Vector3 vector = input.currentSpatialVelocity;
		float heightDelta = _heightProvider.GetHeightDelta(input);
		float deltaTime = input.deltaTime;
		float num = (0f - effectiveMaxSpeed) * deltaTime * groundStickynessFactor;
		bool flag = heightDelta >= num;
		if (!flag)
		{
			if (_gravitationVelocity > 0f - terminalVelocity)
			{
				_gravitationVelocity += _gravity * deltaTime;
			}
			if (_gravitationVelocity * deltaTime < heightDelta)
			{
				_gravitationVelocity = heightDelta / deltaTime;
			}
			vector.y += _gravitationVelocity;
		}
		else
		{
			_gravitationVelocity = 0f;
			if (Mathf.Abs(heightDelta) > 1E-06f)
			{
				vector.y += heightDelta / deltaTime;
				vector = Vector3.ClampMagnitude(vector, effectiveMaxSpeed);
			}
		}
		return new HeightOutput
		{
			finalVelocity = vector,
			isGrounded = flag
		};
	}

	public void CloneFrom(IHeightNavigator other)
	{
		DefaultHeightNavigator defaultHeightNavigator = other as DefaultHeightNavigator;
		if (!(defaultHeightNavigator == null))
		{
			_gravity = defaultHeightNavigator.gravity;
			_providerType = defaultHeightNavigator._providerType;
			groundStickynessFactor = defaultHeightNavigator.groundStickynessFactor;
			terminalVelocity = defaultHeightNavigator.terminalVelocity;
		}
	}

	private static ProviderType GetDefault(Collider c)
	{
		HeightSamplingMode heightMode = GameServices.heightStrategy.heightMode;
		if (heightMode == HeightSamplingMode.NoHeightSampling)
		{
			return ProviderType.ZeroHeight;
		}
		if (c is CapsuleCollider || c is SphereCollider)
		{
			return (heightMode != HeightSamplingMode.HeightMap) ? ProviderType.RaycastSphericalThreePoint : ProviderType.HeightMapSphericalThreePoint;
		}
		if (c is BoxCollider || c is MeshCollider)
		{
			return (heightMode != HeightSamplingMode.HeightMap) ? ProviderType.RaycastBoxThreePoint : ProviderType.HeightMapBoxThreePoint;
		}
		return ProviderType.ZeroHeight;
	}

	private void Awake()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if (component != null)
		{
			component.useGravity = false;
		}
		if (_heightProvider == null)
		{
			_heightProvider = ResolveProvider();
		}
	}

	private IUnitHeightProvider ResolveProvider()
	{
		Collider component = GetComponent<Collider>();
		if (_providerType == ProviderType.None)
		{
			_providerType = GetDefault(component);
		}
		switch (_providerType)
		{
		case ProviderType.CapsuleCast:
			return new CapsuleCastProvider(component);
		case ProviderType.SphereCast:
			return new SphereCastProvider(component);
		case ProviderType.RaycastSphericalThreePoint:
			return new RaycastSphericalThreePointProvider(component);
		case ProviderType.RaycastBoxThreePoint:
			return new RaycastBoxThreePointProvider(component);
		case ProviderType.RaycastBoxFivePoint:
			return new RaycastBoxFivePointProvider(component);
		case ProviderType.HeightMapSphericalThreePoint:
			return new HeightMapSphericalThreePointProvider(component);
		case ProviderType.HeightMapBoxThreePoint:
			return new HeightMapBoxThreePointProvider(component);
		case ProviderType.HeightMapBoxFivePoint:
			return new HeightMapBoxFivePointProvider(component);
		case ProviderType.ZeroHeight:
			return new ZeroHeightProvider();
		default:
		{
			IUnitHeightProviderFactory unitHeightProviderFactory = this.As<IUnitHeightProviderFactory>();
			if (unitHeightProviderFactory == null)
			{
				throw new MissingComponentException("A IUnitHeightProviderFactory implementing component is required when height sampling mode is set to custom.");
			}
			return unitHeightProviderFactory.Create(component);
		}
		}
	}
}
