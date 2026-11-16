using System;
using System.Collections;
using System.Collections.Generic;
using PlaygroundSplines;
using UnityEngine;

namespace ParticlePlayground;

[RequireComponent(typeof(ParticleSystem))]
[ExecuteInEditMode]
public class PlaygroundParticlesC : MonoBehaviour
{
	[HideInInspector]
	public SOURCEC source;

	[HideInInspector]
	public int activeState;

	[HideInInspector]
	public bool emit = true;

	[HideInInspector]
	public bool loop = true;

	[HideInInspector]
	public bool clearParticlesOnEmissionStop;

	[HideInInspector]
	public bool disableOnDone;

	[HideInInspector]
	public ONDONE disableOnDoneRoutine;

	[HideInInspector]
	public int updateRate = 1;

	[HideInInspector]
	public bool calculate = true;

	[HideInInspector]
	public bool calculateDeltaMovement = true;

	[HideInInspector]
	public float deltaMovementStrength = 10f;

	[HideInInspector]
	public float minDeltaMovementStrength;

	[HideInInspector]
	public VALUEMETHOD deltaMovementStrengthValueMethod;

	[HideInInspector]
	public bool worldObjectUpdateVertices;

	[HideInInspector]
	public bool worldObjectUpdateNormals;

	[HideInInspector]
	public NEARESTNEIGHBORORIGINMETHOD nearestNeighborOriginMethod;

	[HideInInspector]
	public int nearestNeighborOrigin;

	[HideInInspector]
	public Vector3 nearestNeighborOriginVector3;

	[HideInInspector]
	public Transform nearestNeighborOriginTransform;

	[HideInInspector]
	public int particleCount;

	[HideInInspector]
	public float emissionRate = 1f;

	[HideInInspector]
	public OVERFLOWMODEC overflowMode;

	[HideInInspector]
	public Vector3 overflowOffset;

	[HideInInspector]
	public bool applySourceScatter;

	[HideInInspector]
	public Vector3 sourceScatterMin;

	[HideInInspector]
	public Vector3 sourceScatterMax;

	[HideInInspector]
	public Vector3 scatterScale = new Vector3(1f, 1f, 1f);

	[HideInInspector]
	public MINMAXVECTOR3METHOD sourceScatterMethod;

	[HideInInspector]
	public SORTINGC sorting;

	[HideInInspector]
	public AnimationCurve lifetimeSorting;

	[HideInInspector]
	public float sizeMin = 1f;

	[HideInInspector]
	public float sizeMax = 1f;

	[HideInInspector]
	public float scale = 1f;

	[HideInInspector]
	public float initialRotationMin;

	[HideInInspector]
	public float initialRotationMax;

	[HideInInspector]
	public bool separateRotationAxes;

	[HideInInspector]
	public Vector3 initialRotationMin3d;

	[HideInInspector]
	public Vector3 initialRotationMax3d;

	[HideInInspector]
	public float rotationSpeedMin;

	[HideInInspector]
	public float rotationSpeedMax;

	[HideInInspector]
	public Vector3 rotationSpeedMin3d;

	[HideInInspector]
	public Vector3 rotationSpeedMax3d;

	[HideInInspector]
	public bool rotateTowardsDirection;

	[HideInInspector]
	public Vector3 rotationNormal = -Vector3.forward;

	[HideInInspector]
	public VALUEMETHOD lifetimeValueMethod;

	[HideInInspector]
	public float lifetime;

	[HideInInspector]
	public float lifetimeMin;

	[HideInInspector]
	public float lifetimeOffset;

	[HideInInspector]
	public float lifetimeEmission = 1f;

	[HideInInspector]
	public float minShurikenLifetime = 0.08f;

	[HideInInspector]
	public bool applyLifetimeSize = true;

	[HideInInspector]
	public AnimationCurve lifetimeSize;

	[HideInInspector]
	public bool applyParticleArraySize;

	[HideInInspector]
	public AnimationCurve particleArraySize;

	[HideInInspector]
	public bool transitionBackToSource;

	[HideInInspector]
	public AnimationCurve transitionBackToSourceAmount;

	[HideInInspector]
	public bool onlySourcePositioning;

	[HideInInspector]
	public bool onlyLifetimePositioning;

	[HideInInspector]
	public Vector3AnimationCurveC lifetimePositioning;

	[HideInInspector]
	public bool applyLifetimePositioningTimeScale;

	[HideInInspector]
	public bool applyLifetimePositioningPositionScale;

	[HideInInspector]
	public AnimationCurve lifetimePositioningTimeScale;

	[HideInInspector]
	public AnimationCurve lifetimePositioningPositionScale;

	[HideInInspector]
	public float lifetimePositioningScale = 1f;

	[HideInInspector]
	public bool lifetimePositioningUsesSourceDirection;

	[HideInInspector]
	public bool applyLifetimeVelocity;

	[HideInInspector]
	public Vector3AnimationCurveC lifetimeVelocity;

	[HideInInspector]
	public float lifetimeVelocityScale = 1f;

	[HideInInspector]
	public bool applyInitialVelocity;

	[HideInInspector]
	public Vector3 initialVelocityMin;

	[HideInInspector]
	public Vector3 initialVelocityMax;

	[HideInInspector]
	public MINMAXVECTOR3METHOD initialVelocityMethod;

	[HideInInspector]
	public bool applyInitialLocalVelocity;

	[HideInInspector]
	public Vector3 initialLocalVelocityMin;

	[HideInInspector]
	public Vector3 initialLocalVelocityMax;

	[HideInInspector]
	public MINMAXVECTOR3METHOD initialLocalVelocityMethod;

	[HideInInspector]
	public bool applyInitialVelocityShape;

	[HideInInspector]
	public Vector3AnimationCurveC initialVelocityShape;

	[HideInInspector]
	public float initialVelocityShapeScale = 1f;

	[HideInInspector]
	public bool applyVelocityBending;

	[HideInInspector]
	public Vector3 velocityBending;

	[HideInInspector]
	public VELOCITYBENDINGTYPEC velocityBendingType;

	[HideInInspector]
	public Vector3 gravity;

	[HideInInspector]
	public float maxVelocity = 100f;

	[HideInInspector]
	public PlaygroundAxisConstraintsC axisConstraints = new PlaygroundAxisConstraintsC();

	[HideInInspector]
	public float damping;

	[HideInInspector]
	public float velocityScale = 1f;

	[HideInInspector]
	public Gradient lifetimeColor;

	[HideInInspector]
	public List<PlaygroundGradientC> lifetimeColors = new List<PlaygroundGradientC>();

	[HideInInspector]
	public COLORSOURCEC colorSource;

	[HideInInspector]
	public bool sourceUsesLifetimeAlpha;

	[HideInInspector]
	public COLORMETHOD colorMethod;

	[HideInInspector]
	public bool arrayColorUsesAlpha;

	[HideInInspector]
	public Gradient arrayColorAlpha;

	[HideInInspector]
	public bool applyLocalSpaceMovementCompensation = true;

	[HideInInspector]
	public bool applyRandomSizeOnRebirth = true;

	[HideInInspector]
	public bool applyRandomInitialVelocityOnRebirth = true;

	[HideInInspector]
	public bool applyRandomRotationOnRebirth = true;

	[HideInInspector]
	public bool applyRandomScatterOnRebirth;

	[HideInInspector]
	public bool applyInitialColorOnRebirth;

	[HideInInspector]
	public bool applyRandomLifetimeOnRebirth = true;

	[HideInInspector]
	public bool applyDeltaOnRebirth = true;

	[HideInInspector]
	public bool calculateManipulatorOnRebirth;

	[HideInInspector]
	public bool pauseCalculationWhenInvisible;

	[HideInInspector]
	public bool calculationTriggerSizeGizmo;

	[HideInInspector]
	public Transform calculationTriggerTransform;

	[HideInInspector]
	public Vector3 calculationTriggerSize = new Vector3(1f, 1f, 1f);

	[HideInInspector]
	public Vector3 calculationTriggerOffset = default;

	[HideInInspector]
	public bool forceVisibilityWhenOutOfFrustrum = true;

	[HideInInspector]
	public bool syncPositionsOnMainThread;

	[HideInInspector]
	public bool applyLockPosition;

	[HideInInspector]
	public bool applyLockRotation;

	[HideInInspector]
	public bool applyLockScale;

	[HideInInspector]
	public bool lockPositionIsLocal;

	[HideInInspector]
	public bool lockRotationIsLocal;

	[HideInInspector]
	public Vector3 lockPosition = Vector3.zero;

	[HideInInspector]
	public Vector3 lockRotation = Vector3.zero;

	[HideInInspector]
	public Vector3 lockScale = new Vector3(1f, 1f, 1f);

	[HideInInspector]
	public bool applyMovementCompensationLifetimeStrength;

	[HideInInspector]
	public AnimationCurve movementCompensationLifetimeStrength;

	[HideInInspector]
	public bool applyParticleMask;

	[HideInInspector]
	public int particleMask;

	[HideInInspector]
	public float particleMaskTime;

	[HideInInspector]
	public MASKSORTINGC particleMaskSorting;

	[HideInInspector]
	public float stretchSpeed = 1f;

	[HideInInspector]
	public bool applyStretchStartDirection;

	[HideInInspector]
	public Vector3 stretchStartDirection = Vector3.zero;

	[HideInInspector]
	public bool applyLifetimeStretching;

	[HideInInspector]
	public AnimationCurve stretchLifetime;

	[HideInInspector]
	public ThreadMethodLocal threadMethod;

	[HideInInspector]
	public bool multithreadedStartup = true;

	[HideInInspector]
	public int scriptedEmissionIndex;

	[HideInInspector]
	public Vector3 scriptedEmissionPosition;

	[HideInInspector]
	public Vector3 scriptedEmissionVelocity;

	[HideInInspector]
	public Color scriptedEmissionColor = Color.white;

	[HideInInspector]
	private float scriptedLifetime;

	[HideInInspector]
	public bool collision;

	[HideInInspector]
	public bool affectRigidbodies = true;

	[HideInInspector]
	public bool inverseRigidbodyCollision;

	[HideInInspector]
	public float mass = 0.01f;

	[HideInInspector]
	public float collisionRadius = 1f;

	[HideInInspector]
	public LayerMask collisionMask;

	[HideInInspector]
	public List<Transform> collisionExclusion;

	[HideInInspector]
	public float lifetimeLoss;

	[HideInInspector]
	public float bounciness = 0.5f;

	[HideInInspector]
	public Vector3 bounceRandomMin;

	[HideInInspector]
	public Vector3 bounceRandomMax;

	[HideInInspector]
	public List<PlaygroundColliderC> colliders;

	[HideInInspector]
	public COLLISIONTYPEC collisionType;

	[HideInInspector]
	public float minCollisionDepth;

	[HideInInspector]
	public float maxCollisionDepth;

	[HideInInspector]
	public bool stickyCollisions;

	[HideInInspector]
	public float stickyCollisionsSurfaceOffset;

	[HideInInspector]
	public LayerMask stickyCollisionsMask = -1;

	[HideInInspector]
	public bool collisionPrecision;

	[HideInInspector]
	public bool forceCollisionCaching;

	[HideInInspector]
	public bool maskedParticlesBypassCollision = true;

	public List<ParticleStateC> states = new List<ParticleStateC>();

	[HideInInspector]
	public List<PlaygroundSpline> splines;

	[HideInInspector]
	public float splineTimeOffset;

	[HideInInspector]
	public bool treatAsOneSpline;

	private int splineIndex;

	[HideInInspector]
	public WorldObject worldObject = new WorldObject();

	[HideInInspector]
	public SkinnedWorldObject skinnedWorldObject = new SkinnedWorldObject();

	[HideInInspector]
	public bool forceSkinnedMeshUpdateOnMainThread;

	[HideInInspector]
	public Transform sourceTransform;

	[HideInInspector]
	public List<PlaygroundTransformC> sourceTransforms;

	[HideInInspector]
	public bool treatAsOneTransform;

	private int transformIndex;

	[HideInInspector]
	public PlaygroundParticlesC otherParticleSource;

	[HideInInspector]
	public SOURCEBIRTHMETHOD otherParticleSourceMethod;

	[HideInInspector]
	public PaintObjectC paint;

	[HideInInspector]
	public ParticleProjectionC projection;

	public List<ManipulatorObjectC> manipulators;

	[HideInInspector]
	public List<PlaygroundEventC> events;

	[NonSerialized]
	public PlaygroundCache playgroundCache = new PlaygroundCache();

	[NonSerialized]
	public ParticleSystem.Particle[] particleCache;

	[NonSerialized]
	public CollisionCache collisionCache;

	[HideInInspector]
	public List<PlaygroundSave> snapshots = new List<PlaygroundSave>();

	[HideInInspector]
	public bool loadFromStart;

	[HideInInspector]
	public int loadFrom;

	[HideInInspector]
	public bool loadTransition;

	[HideInInspector]
	public TRANSITIONTYPEC loadTransitionType;

	[HideInInspector]
	public float loadTransitionTime = 1f;

	[HideInInspector]
	public PlaygroundCache snapshotData;

	[HideInInspector]
	public float timeOfSnapshot;

	public bool isSnapshot;

	[HideInInspector]
	public ParticleSystem shurikenParticleSystem;

	[HideInInspector]
	public int particleSystemId;

	[HideInInspector]
	public GameObject particleSystemGameObject;

	[HideInInspector]
	public Transform particleSystemTransform;

	[HideInInspector]
	public Renderer particleSystemRenderer;

	[HideInInspector]
	public ParticleSystemRenderer particleSystemRenderer2;

	[HideInInspector]
	public List<PlaygroundParticlesC> eventControlledBy = new List<PlaygroundParticlesC>();

	private SimplexNoise turbulenceSimplex;

	[HideInInspector]
	public TURBULENCETYPE turbulenceType;

	[HideInInspector]
	public float turbulenceStrength = 10f;

	[HideInInspector]
	public float turbulenceScale = 1f;

	[HideInInspector]
	public float turbulenceTimeScale = 1f;

	[HideInInspector]
	public bool turbulenceApplyLifetimeStrength;

	[HideInInspector]
	public AnimationCurve turbulenceLifetimeStrength;

	[HideInInspector]
	public bool prewarm;

	[HideInInspector]
	public float prewarmTime = 1f;

	[HideInInspector]
	public int prewarmCycles = 16;

	[HideInInspector]
	public float particleTimescale = 1f;

	[SerializeField]
	private int _randomSeed;

	[HideInInspector]
	public bool randomSeedOnEnable = true;

	private int thisLayer;

	private bool isPrewarming;

	private bool inTransition;

	private int previousParticleCount = -1;

	private float previousEmissionRate = 1f;

	private bool renderModeStretch;

	private float previousSizeMin;

	private float previousSizeMax;

	private float previousInitialRotationMin;

	private float previousInitialRotationMax;

	private float previousRotationSpeedMin;

	private float previousRotationSpeedMax;

	private bool previousSeparateRotationAxes;

	private Vector3 previousInitialRotationMin3d;

	private Vector3 previousInitialRotationMax3d;

	private Vector3 previousRotationSpeedMin3d;

	private Vector3 previousRotationSpeedMax3d;

	private Vector3 previousVelocityMin;

	private Vector3 previousVelocityMax;

	private Vector3 previousLocalVelocityMin;

	private Vector3 previousLocalVelocityMax;

	private MINMAXVECTOR3METHOD previousLocalVelocityMethod;

	private MINMAXVECTOR3METHOD previousVelocityMethod;

	private SORTINGC previousSorting;

	private int previousNearestNeighborOrigin;

	private NEARESTNEIGHBORORIGINMETHOD previousNearestNeighborOriginMethod;

	private Vector3 previousNearestNeighborOriginVector3;

	private Transform previousNearestNeighborOriginTransform;

	private Vector3 nearestNeighborOriginTransformPosition;

	private MASKSORTINGC previousMaskSorting;

	private VALUEMETHOD previousLifetimeValueMethod;

	private float previousLifetime;

	private float previousLifetimeMin;

	private float previousLifetimeEmission;

	private bool previousEmission = true;

	private bool previousLoop;

	private float emissionStopped;

	private bool queueEmissionHalt;

	private bool hasEmitted;

	private int lifetimeColorId;

	private System.Random internalRandom01;

	private float lastTimeUpdated;

	private PlaygroundEventParticle eventParticle = new PlaygroundEventParticle();

	[NonSerialized]
	public bool cameFromNonCalculatedFrame;

	[NonSerialized]
	public bool cameFromNonEmissionFrame = true;

	[NonSerialized]
	public float localTime;

	[NonSerialized]
	public float localDeltaTime;

	[NonSerialized]
	public int previousActiveState;

	[NonSerialized]
	public float simulationStarted;

	[NonSerialized]
	public bool inPlayback;

	[NonSerialized]
	public bool loopExceeded;

	[NonSerialized]
	public int loopExceededOnParticle;

	[NonSerialized]
	public bool isReadyForThreadedCalculations;

	[NonSerialized]
	public bool hasActiveParticles = true;

	[NonSerialized]
	public bool isDoneThread = true;

	[NonSerialized]
	public bool threadHadNoActiveParticles;

	private bool hasEventManipulatorLocal;

	private bool hasEventManipulatorGlobal;

	private bool hasSeveralManipulatorEvents;

	private bool hasGlobalAffectingManipulators;

	private Quaternion particleSystemRotation;

	private Quaternion particleSystemInverseRotation;

	private int psTransformNum = -1;

	private Vector3 manipulatorFix = new Vector3(0f, 0.0001f, 0f);

	private PlaygroundParticlesC thisInstance;

	private float t;

	private bool cancelDeltaPositioningOnSync;

	private Quaternion stCompensationRot;

	private Quaternion stCompensationRotPrev;

	private Quaternion stRot;

	private Vector3 stDir;

	private bool localSpace;

	private bool overflow;

	private bool skinnedWorldObjectReady;

	private bool stateReadyForTextureColor;

	private int manipulatorEventCount;

	private bool hasEvent;

	private bool hasTimerEvent;

	private bool hasCollisionCache;

	private ParticleSystem.MainModule shurikenMainModule;

	private int reportUpdateStepper = 100;

	private bool isReportingBadUpdateRate;

	private bool isSettingLifetime;

	private bool isSettingParticleTime;

	private bool isSettingParticleCount;

	private bool prevLoop;

	private SOURCEC prevSource;

	private float realSimulationTime;

	private Action _particleCalculationAction;

	private bool isSaving;

	private bool isLoading;

	private bool transitionAvailable;

	private bool abortTransition;

	private bool isYieldRefreshing;

	private bool initialized;

	[HideInInspector]
	public int randomSeed
	{
		get
		{
			return _randomSeed;
		}
		set
		{
			_randomSeed = value;
			RefreshSystemRandom(_randomSeed);
		}
	}

	public bool IsDoneThread
	{
		get
		{
			return isDoneThread;
		}
		set
		{
			isDoneThread = value;
		}
	}

	public float LastTimeUpdated
	{
		get
		{
			return lastTimeUpdated;
		}
		set
		{
			lastTimeUpdated = value;
		}
	}

	public float LocalDeltaTime
	{
		get
		{
			return localDeltaTime;
		}
		set
		{
			localDeltaTime = value;
			t = value;
		}
	}

	public void CopyTo(PlaygroundParticlesC playgroundParticles)
	{
		playgroundParticles.source = source;
		playgroundParticles.activeState = activeState;
		playgroundParticles.emit = emit;
		playgroundParticles.loop = loop;
		playgroundParticles.clearParticlesOnEmissionStop = clearParticlesOnEmissionStop;
		playgroundParticles.disableOnDone = disableOnDone;
		playgroundParticles.disableOnDoneRoutine = disableOnDoneRoutine;
		playgroundParticles.updateRate = updateRate;
		playgroundParticles.calculate = calculate;
		playgroundParticles.calculateDeltaMovement = calculateDeltaMovement;
		playgroundParticles.deltaMovementStrength = deltaMovementStrength;
		playgroundParticles.minDeltaMovementStrength = minDeltaMovementStrength;
		playgroundParticles.deltaMovementStrengthValueMethod = deltaMovementStrengthValueMethod;
		playgroundParticles.worldObjectUpdateVertices = worldObjectUpdateVertices;
		playgroundParticles.worldObjectUpdateNormals = worldObjectUpdateNormals;
		playgroundParticles.nearestNeighborOrigin = nearestNeighborOrigin;
		playgroundParticles.nearestNeighborOriginMethod = nearestNeighborOriginMethod;
		playgroundParticles.nearestNeighborOriginTransform = nearestNeighborOriginTransform;
		playgroundParticles.nearestNeighborOriginVector3 = nearestNeighborOriginVector3;
		playgroundParticles.particleCount = particleCount;
		playgroundParticles.emissionRate = emissionRate;
		playgroundParticles.overflowMode = overflowMode;
		playgroundParticles.overflowOffset = overflowOffset;
		playgroundParticles.applySourceScatter = applySourceScatter;
		playgroundParticles.scatterScale = scatterScale;
		playgroundParticles.sourceScatterMin = sourceScatterMin;
		playgroundParticles.sourceScatterMax = sourceScatterMax;
		playgroundParticles.sourceScatterMethod = sourceScatterMethod;
		playgroundParticles.sorting = sorting;
		playgroundParticles.lifetimeSorting = new AnimationCurve(lifetimeSorting.keys);
		playgroundParticles.sizeMin = sizeMin;
		playgroundParticles.sizeMax = sizeMax;
		playgroundParticles.scale = scale;
		playgroundParticles.initialRotationMin = initialRotationMin;
		playgroundParticles.initialRotationMax = initialRotationMax;
		playgroundParticles.rotationSpeedMin = rotationSpeedMin;
		playgroundParticles.rotationSpeedMax = rotationSpeedMax;
		playgroundParticles.separateRotationAxes = separateRotationAxes;
		playgroundParticles.initialRotationMin3d = initialRotationMin3d;
		playgroundParticles.initialRotationMax3d = initialRotationMax3d;
		playgroundParticles.rotationSpeedMin3d = rotationSpeedMin3d;
		playgroundParticles.rotationSpeedMax3d = rotationSpeedMax3d;
		playgroundParticles.rotateTowardsDirection = rotateTowardsDirection;
		playgroundParticles.rotationNormal = rotationNormal;
		playgroundParticles.lifetime = lifetime;
		playgroundParticles.lifetimeValueMethod = lifetimeValueMethod;
		playgroundParticles.lifetimeMin = lifetimeMin;
		playgroundParticles.lifetimeEmission = lifetimeEmission;
		playgroundParticles.lifetimeOffset = lifetimeOffset;
		playgroundParticles.minShurikenLifetime = minShurikenLifetime;
		playgroundParticles.applyLifetimeSize = applyLifetimeSize;
		playgroundParticles.lifetimeSize = new AnimationCurve(lifetimeSize.keys);
		playgroundParticles.applyParticleArraySize = applyParticleArraySize;
		playgroundParticles.particleArraySize = new AnimationCurve(particleArraySize.keys);
		playgroundParticles.transitionBackToSource = transitionBackToSource;
		playgroundParticles.transitionBackToSourceAmount = new AnimationCurve(transitionBackToSourceAmount.keys);
		playgroundParticles.onlySourcePositioning = onlySourcePositioning;
		playgroundParticles.onlyLifetimePositioning = onlyLifetimePositioning;
		playgroundParticles.lifetimePositioning = lifetimePositioning.Clone();
		playgroundParticles.lifetimePositioningScale = lifetimePositioningScale;
		playgroundParticles.lifetimePositioningUsesSourceDirection = lifetimePositioningUsesSourceDirection;
		playgroundParticles.lifetimePositioningTimeScale = new AnimationCurve(lifetimePositioningTimeScale.keys);
		playgroundParticles.lifetimePositioningPositionScale = new AnimationCurve(lifetimePositioningPositionScale.keys);
		playgroundParticles.applyLifetimePositioningTimeScale = applyLifetimePositioningTimeScale;
		playgroundParticles.applyLifetimePositioningPositionScale = applyLifetimePositioningPositionScale;
		playgroundParticles.axisConstraints = axisConstraints.Clone();
		playgroundParticles.applyLifetimeVelocity = applyLifetimeVelocity;
		playgroundParticles.lifetimeVelocity = lifetimeVelocity.Clone();
		playgroundParticles.lifetimeVelocityScale = lifetimeVelocityScale;
		playgroundParticles.applyInitialVelocity = applyInitialVelocity;
		playgroundParticles.initialVelocityMin = initialVelocityMin;
		playgroundParticles.initialVelocityMax = initialVelocityMax;
		playgroundParticles.initialVelocityMethod = initialVelocityMethod;
		playgroundParticles.applyInitialLocalVelocity = applyInitialLocalVelocity;
		playgroundParticles.initialLocalVelocityMin = initialLocalVelocityMin;
		playgroundParticles.initialLocalVelocityMax = initialLocalVelocityMax;
		playgroundParticles.initialLocalVelocityMethod = initialLocalVelocityMethod;
		playgroundParticles.applyVelocityBending = applyVelocityBending;
		playgroundParticles.velocityBending = velocityBending;
		playgroundParticles.velocityBendingType = velocityBendingType;
		playgroundParticles.applyInitialVelocityShape = applyInitialVelocityShape;
		playgroundParticles.initialVelocityShape = initialVelocityShape.Clone();
		playgroundParticles.initialVelocityShapeScale = initialVelocityShapeScale;
		playgroundParticles.gravity = gravity;
		playgroundParticles.damping = damping;
		playgroundParticles.velocityScale = velocityScale;
		playgroundParticles.maxVelocity = maxVelocity;
		playgroundParticles.lifetimeColor.SetKeys(lifetimeColor.colorKeys, lifetimeColor.alphaKeys);
		playgroundParticles.colorSource = colorSource;
		playgroundParticles.sourceUsesLifetimeAlpha = sourceUsesLifetimeAlpha;
		playgroundParticles.colorMethod = colorMethod;
		playgroundParticles.arrayColorUsesAlpha = arrayColorUsesAlpha;
		playgroundParticles.arrayColorAlpha.SetKeys(arrayColorAlpha.colorKeys, arrayColorAlpha.alphaKeys);
		playgroundParticles.applyLocalSpaceMovementCompensation = applyLocalSpaceMovementCompensation;
		playgroundParticles.applyRandomSizeOnRebirth = applyRandomSizeOnRebirth;
		playgroundParticles.applyRandomInitialVelocityOnRebirth = applyRandomInitialVelocityOnRebirth;
		playgroundParticles.applyRandomRotationOnRebirth = applyRandomRotationOnRebirth;
		playgroundParticles.applyRandomScatterOnRebirth = applyRandomScatterOnRebirth;
		playgroundParticles.applyInitialColorOnRebirth = applyInitialColorOnRebirth;
		playgroundParticles.applyRandomLifetimeOnRebirth = applyRandomLifetimeOnRebirth;
		playgroundParticles.applyDeltaOnRebirth = applyDeltaOnRebirth;
		playgroundParticles.calculateManipulatorOnRebirth = calculateManipulatorOnRebirth;
		playgroundParticles.pauseCalculationWhenInvisible = pauseCalculationWhenInvisible;
		playgroundParticles.calculationTriggerSize = calculationTriggerSize;
		playgroundParticles.calculationTriggerOffset = calculationTriggerOffset;
		playgroundParticles.calculationTriggerTransform = calculationTriggerTransform;
		playgroundParticles.calculationTriggerSizeGizmo = calculationTriggerSizeGizmo;
		playgroundParticles.forceVisibilityWhenOutOfFrustrum = forceVisibilityWhenOutOfFrustrum;
		playgroundParticles.syncPositionsOnMainThread = syncPositionsOnMainThread;
		playgroundParticles.applyLockPosition = applyLockPosition;
		playgroundParticles.applyLockRotation = applyLockRotation;
		playgroundParticles.applyLockScale = applyLockScale;
		playgroundParticles.lockPositionIsLocal = lockPositionIsLocal;
		playgroundParticles.lockRotationIsLocal = lockRotationIsLocal;
		playgroundParticles.lockPosition = lockPosition;
		playgroundParticles.lockRotation = lockRotation;
		playgroundParticles.lockScale = lockScale;
		playgroundParticles.applyMovementCompensationLifetimeStrength = applyMovementCompensationLifetimeStrength;
		playgroundParticles.movementCompensationLifetimeStrength = new AnimationCurve(movementCompensationLifetimeStrength.keys);
		playgroundParticles.applyParticleMask = applyParticleMask;
		playgroundParticles.particleMask = particleMask;
		playgroundParticles.particleMaskTime = particleMaskTime;
		playgroundParticles.particleMaskSorting = particleMaskSorting;
		playgroundParticles.stretchSpeed = stretchSpeed;
		playgroundParticles.applyStretchStartDirection = applyStretchStartDirection;
		playgroundParticles.applyLifetimeStretching = applyLifetimeStretching;
		playgroundParticles.stretchLifetime = new AnimationCurve(stretchLifetime.keys);
		playgroundParticles.threadMethod = threadMethod;
		playgroundParticles.multithreadedStartup = multithreadedStartup;
		playgroundParticles.scriptedEmissionIndex = scriptedEmissionIndex;
		playgroundParticles.scriptedEmissionPosition = scriptedEmissionPosition;
		playgroundParticles.scriptedEmissionVelocity = scriptedEmissionVelocity;
		playgroundParticles.scriptedEmissionColor = scriptedEmissionColor;
		playgroundParticles.collision = collision;
		playgroundParticles.affectRigidbodies = affectRigidbodies;
		playgroundParticles.inverseRigidbodyCollision = inverseRigidbodyCollision;
		playgroundParticles.mass = mass;
		playgroundParticles.collisionRadius = collisionRadius;
		playgroundParticles.collisionMask = collisionMask;
		playgroundParticles.bounciness = bounciness;
		playgroundParticles.lifetimeLoss = lifetimeLoss;
		playgroundParticles.bounceRandomMin = bounceRandomMin;
		playgroundParticles.bounceRandomMax = bounceRandomMax;
		playgroundParticles.collisionType = collisionType;
		playgroundParticles.minCollisionDepth = minCollisionDepth;
		playgroundParticles.maxCollisionDepth = maxCollisionDepth;
		playgroundParticles.stickyCollisions = stickyCollisions;
		playgroundParticles.stickyCollisionsSurfaceOffset = stickyCollisionsSurfaceOffset;
		playgroundParticles.stickyCollisionsMask = stickyCollisionsMask;
		playgroundParticles.collisionPrecision = collisionPrecision;
		playgroundParticles.forceCollisionCaching = forceCollisionCaching;
		playgroundParticles.maskedParticlesBypassCollision = maskedParticlesBypassCollision;
		playgroundParticles.collisionExclusion = new List<Transform>();
		for (int i = 0; i < collisionExclusion.Count; i++)
		{
			playgroundParticles.collisionExclusion.Add(collisionExclusion[i]);
		}
		playgroundParticles.colliders = new List<PlaygroundColliderC>();
		for (int j = 0; j < colliders.Count; j++)
		{
			playgroundParticles.colliders.Add(colliders[j].Clone());
		}
		playgroundParticles.states = new List<ParticleStateC>();
		for (int k = 0; k < states.Count; k++)
		{
			playgroundParticles.states.Add(states[k].Clone());
		}
		playgroundParticles.splines = new List<PlaygroundSpline>();
		for (int l = 0; l < splines.Count; l++)
		{
			playgroundParticles.splines.Add(splines[l]);
		}
		playgroundParticles.splineTimeOffset = splineTimeOffset;
		playgroundParticles.treatAsOneSpline = treatAsOneSpline;
		playgroundParticles.worldObject = worldObject.Clone();
		playgroundParticles.skinnedWorldObject = skinnedWorldObject.Clone();
		playgroundParticles.forceSkinnedMeshUpdateOnMainThread = forceSkinnedMeshUpdateOnMainThread;
		playgroundParticles.sourceTransform = sourceTransform;
		playgroundParticles.sourceTransforms = new List<PlaygroundTransformC>();
		for (int m = 0; m < sourceTransforms.Count; m++)
		{
			playgroundParticles.sourceTransforms.Add(sourceTransforms[m].Clone());
		}
		playgroundParticles.treatAsOneTransform = treatAsOneTransform;
		playgroundParticles.paint = paint.Clone();
		playgroundParticles.projection = projection.Clone();
		playgroundParticles.manipulators = new List<ManipulatorObjectC>();
		for (int n = 0; n < manipulators.Count; n++)
		{
			playgroundParticles.manipulators.Add(manipulators[n].Clone());
		}
		playgroundParticles.events = new List<PlaygroundEventC>();
		for (int num = 0; num < events.Count; num++)
		{
			playgroundParticles.events.Add(events[num].Clone());
		}
		playgroundParticles.lifetimeColors = new List<PlaygroundGradientC>();
		for (int num2 = 0; num2 < lifetimeColors.Count; num2++)
		{
			playgroundParticles.lifetimeColors.Add(new PlaygroundGradientC());
			lifetimeColors[num2].CopyTo(playgroundParticles.lifetimeColors[num2]);
		}
		playgroundParticles.turbulenceType = turbulenceType;
		playgroundParticles.turbulenceApplyLifetimeStrength = turbulenceApplyLifetimeStrength;
		playgroundParticles.turbulenceLifetimeStrength = new AnimationCurve(turbulenceLifetimeStrength.keys);
		playgroundParticles.turbulenceScale = turbulenceScale;
		playgroundParticles.turbulenceStrength = turbulenceStrength;
		playgroundParticles.turbulenceTimeScale = turbulenceTimeScale;
		playgroundParticles.particleTimescale = particleTimescale;
		playgroundParticles._randomSeed = _randomSeed;
		playgroundParticles.randomSeedOnEnable = randomSeedOnEnable;
	}

	public void CopySaveDataTo(PlaygroundParticlesC playgroundParticles)
	{
		playgroundParticles.snapshots = new List<PlaygroundSave>();
		for (int i = 0; i < snapshots.Count; i++)
		{
			playgroundParticles.snapshots.Add(snapshots[i].Clone());
		}
	}

	public void Emit(bool setEmission)
	{
		emit = setEmission;
		if (emit)
		{
			simulationStarted = localTime;
			calculate = true;
			hasActiveParticles = true;
			threadHadNoActiveParticles = false;
			loopExceeded = false;
			loopExceededOnParticle = -1;
			if (thisInstance == null)
			{
				thisInstance = this;
			}
			particleSystemGameObject.SetActive(value: true);
			Emission(thisInstance, emission: true, callRestEmission: true);
		}
		else
		{
			emissionStopped = localTime;
			if (clearParticlesOnEmissionStop)
			{
				InactivateParticles();
			}
		}
		previousEmission = setEmission;
	}

	public int Emit()
	{
		source = SOURCEC.Script;
		int result = scriptedEmissionIndex;
		EmitProcedure(scriptedEmissionPosition, scriptedEmissionVelocity, scriptedEmissionColor);
		return result;
	}

	public int Emit(Vector3 givePosition)
	{
		source = SOURCEC.Script;
		int result = scriptedEmissionIndex;
		EmitProcedure(givePosition, scriptedEmissionVelocity, scriptedEmissionColor);
		return result;
	}

	public int Emit(Vector3 givePosition, Vector3 giveVelocity)
	{
		source = SOURCEC.Script;
		int result = scriptedEmissionIndex;
		EmitProcedure(givePosition, giveVelocity, scriptedEmissionColor);
		return result;
	}

	public int Emit(Vector3 givePosition, Vector3 giveVelocity, Color32 giveColor)
	{
		source = SOURCEC.Script;
		int result = scriptedEmissionIndex;
		EmitProcedure(givePosition, giveVelocity, giveColor);
		return result;
	}

	public void Emit(int quantity)
	{
		source = SOURCEC.Script;
		for (int i = 0; i < quantity; i++)
		{
			EmitProcedure(scriptedEmissionPosition, scriptedEmissionVelocity, scriptedEmissionColor);
		}
	}

	public void Emit(int quantity, Vector3 givePosition, Vector3 randomVelocityMin, Vector3 randomVelocityMax, Color32 giveColor)
	{
		source = SOURCEC.Script;
		for (int i = 0; i < quantity; i++)
		{
			EmitProcedure(givePosition, (!applyInitialVelocityShape) ? RandomRange(internalRandom01, randomVelocityMin, randomVelocityMax) : Vector3.Scale(RandomRange(internalRandom01, randomVelocityMin, randomVelocityMax), initialVelocityShape.Evaluate((float)i * 1f / ((float)quantity * 1f), initialVelocityShapeScale)), giveColor);
		}
	}

	public void Emit(int quantity, Vector3 randomPositionMin, Vector3 randomPositionMax, Vector3 randomVelocityMin, Vector3 randomVelocityMax, Color32 giveColor)
	{
		source = SOURCEC.Script;
		for (int i = 0; i < quantity; i++)
		{
			EmitProcedure(RandomRange(internalRandom01, randomPositionMin, randomPositionMax), (!applyInitialVelocityShape) ? RandomRange(internalRandom01, randomVelocityMin, randomVelocityMax) : Vector3.Scale(RandomRange(internalRandom01, randomVelocityMin, randomVelocityMax), initialVelocityShape.Evaluate((float)i * 1f / ((float)quantity * 1f), initialVelocityShapeScale)), giveColor);
		}
	}

	public int Emit(float giveLifetime)
	{
		source = SOURCEC.Script;
		int result = scriptedEmissionIndex;
		scriptedLifetime = giveLifetime;
		EmitProcedure(scriptedEmissionPosition, scriptedEmissionVelocity, scriptedEmissionColor);
		scriptedLifetime = 0f;
		return result;
	}

	public int Emit(Vector3 givePosition, float giveLifetime)
	{
		source = SOURCEC.Script;
		int result = scriptedEmissionIndex;
		scriptedLifetime = giveLifetime;
		EmitProcedure(givePosition, scriptedEmissionVelocity, scriptedEmissionColor);
		scriptedLifetime = 0f;
		return result;
	}

	public int Emit(Vector3 givePosition, Vector3 giveVelocity, float giveLifetime)
	{
		source = SOURCEC.Script;
		int result = scriptedEmissionIndex;
		scriptedLifetime = giveLifetime;
		EmitProcedure(givePosition, giveVelocity, scriptedEmissionColor);
		scriptedLifetime = 0f;
		return result;
	}

	public int Emit(Vector3 givePosition, Vector3 giveVelocity, float giveLifetime, Color32 giveColor)
	{
		source = SOURCEC.Script;
		int result = scriptedEmissionIndex;
		scriptedLifetime = giveLifetime;
		EmitProcedure(givePosition, giveVelocity, giveColor);
		scriptedLifetime = 0f;
		return result;
	}

	public void ThreadSafeEmit(Vector3 givePosition, Vector3 giveVelocity, Color32 giveColor)
	{
		EmitProcedure(givePosition, giveVelocity, giveColor);
	}

	public void ThreadSafeEmit(int quantity, Vector3 givePosition, Vector3 randomVelocityMin, Vector3 randomVelocityMax, Color32 giveColor)
	{
		for (int i = 0; i < quantity; i++)
		{
			EmitProcedure(givePosition, (!applyInitialVelocityShape) ? RandomRange(internalRandom01, randomVelocityMin, randomVelocityMax) : Vector3.Scale(RandomRange(internalRandom01, randomVelocityMin, randomVelocityMax), initialVelocityShape.Evaluate((float)i * 1f / ((float)quantity * 1f), initialVelocityShapeScale)), giveColor);
		}
	}

	private void EmitProcedure(Vector3 givePosition, Vector3 giveVelocity, Color32 giveColor)
	{
		scriptedEmissionIndex %= particleCount;
		scriptedEmissionPosition = givePosition;
		scriptedEmissionVelocity = giveVelocity;
		scriptedEmissionColor = giveColor;
		hasActiveParticles = true;
		threadHadNoActiveParticles = false;
		cameFromNonCalculatedFrame = false;
		cameFromNonEmissionFrame = false;
		playgroundCache.simulate[scriptedEmissionIndex] = true;
		Rebirth(thisInstance, scriptedEmissionIndex, internalRandom01);
		if (playgroundCache.lifetimeOffset.Length == particleCount)
		{
			ref Color32 reference = ref playgroundCache.initialColor[scriptedEmissionIndex];
			reference = scriptedEmissionColor;
			playgroundCache.lifetimeOffset[scriptedEmissionIndex] = 0f;
			playgroundCache.life[scriptedEmissionIndex] = 0f;
			playgroundCache.birth[scriptedEmissionIndex] = PlaygroundC.globalTime;
			if (scriptedLifetime == 0f)
			{
				playgroundCache.death[scriptedEmissionIndex] = playgroundCache.birth[scriptedEmissionIndex] + lifetime;
			}
			else
			{
				playgroundCache.death[scriptedEmissionIndex] = playgroundCache.birth[scriptedEmissionIndex] + scriptedLifetime;
			}
			playgroundCache.emission[scriptedEmissionIndex] = true;
			playgroundCache.scriptedColor[scriptedEmissionIndex] = giveColor;
			playgroundCache.isFirstLoop[scriptedEmissionIndex] = true;
			emit = true;
			previousEmission = true;
			simulationStarted = localTime;
			loopExceeded = false;
			loopExceededOnParticle = -1;
			scriptedEmissionIndex++;
			scriptedEmissionIndex %= particleCount;
		}
	}

	public bool IsAlive()
	{
		return calculate && hasActiveParticles;
	}

	public bool IsLocalSpace()
	{
		return localSpace;
	}

	public bool InTransition()
	{
		return inTransition;
	}

	public bool IsLoading()
	{
		return isLoading;
	}

	public bool IsSaving()
	{
		return isSaving;
	}

	public bool IsReady()
	{
		return initialized && !isPrewarming;
	}

	public bool Initialized()
	{
		return initialized && !isPrewarming;
	}

	public bool IsYieldRefreshing()
	{
		return isYieldRefreshing;
	}

	public bool IsSettingParticleTime()
	{
		return isSettingParticleTime;
	}

	public bool IsSkinnedWorldObjectReady()
	{
		return source == SOURCEC.SkinnedWorldObject && skinnedWorldObjectReady;
	}

	public bool HasTurbulence()
	{
		return calculate && !onlySourcePositioning && !onlyLifetimePositioning && turbulenceStrength > 0f && turbulenceType != TURBULENCETYPE.None;
	}

	public bool HasOverflow()
	{
		return overflow;
	}

	public bool HasGlobalManipulator()
	{
		return hasGlobalAffectingManipulators;
	}

	public bool HasCollisionCache()
	{
		return hasCollisionCache;
	}

	public bool HasCollided(int index)
	{
		if (index < 0 || index > particleCount - 1)
		{
			return false;
		}
		if (hasCollisionCache)
		{
			return collisionCache.hasCollided[index];
		}
		return false;
	}

	public Vector3 GetCollisionPosition(int index)
	{
		if (!hasCollisionCache || index < 0 || index > particleCount - 1)
		{
			return Vector3.zero;
		}
		return collisionCache.collisionPosition[index];
	}

	public Vector3 GetCollisionNormal(int index)
	{
		if (!hasCollisionCache || index < 0 || index > particleCount - 1)
		{
			return Vector3.zero;
		}
		return collisionCache.collisionNormal[index];
	}

	public Vector3 GetStickyPosition(int index)
	{
		if (!hasCollisionCache || index < 0 || index > particleCount - 1)
		{
			return Vector3.zero;
		}
		return collisionCache.stickyPosition[index];
	}

	public void SetSticky(int index, Vector3 position, Vector3 normal, float offset, Transform parent)
	{
		if (index >= 0 && index <= particleCount - 1)
		{
			if (!hasCollisionCache)
			{
				collisionCache = new CollisionCache(particleCount);
				hasCollisionCache = true;
			}
			collisionCache.SetSticky(index, position, normal, stickyCollisionsSurfaceOffset, parent);
		}
	}

	public void UpdateSticky(int index)
	{
		if (hasCollisionCache && index >= 0 && index <= particleCount - 1)
		{
			collisionCache.hasCollided[index] = true;
			collisionCache.UpdateStickyPosition(index);
			ref Vector3 reference = ref playgroundCache.position[index];
			reference = collisionCache.stickyPosition[index];
			particleCache[index].position = playgroundCache.position[index];
		}
	}

	public void ClearCollisions()
	{
		if (hasCollisionCache)
		{
			collisionCache.ClearCollisions();
		}
	}

	public void ClearCollisions(int index)
	{
		if (hasCollisionCache && index >= 0 && index <= particleCount - 1)
		{
			collisionCache.Reset(index);
		}
	}

	public Transform GetCollisionTransform(int index)
	{
		if (!hasCollisionCache || index < 0 || index > particleCount - 1)
		{
			return null;
		}
		return collisionCache.collisionTransform[index];
	}

	public int GetLayer()
	{
		return thisLayer;
	}

	public void RefreshSystemRandom()
	{
		internalRandom01 = new System.Random();
	}

	public void RefreshSystemRandom(int seed)
	{
		_randomSeed = seed;
		internalRandom01 = new System.Random(_randomSeed);
	}

	public void SetRandomSeed(int seed)
	{
		RefreshSystemRandom(seed);
	}

	public void RefreshRandomAll(int seed)
	{
		RefreshSystemRandom(seed);
		RefreshRandomAll();
	}

	public void RefreshRandomAll()
	{
		RefreshScatter();
		RefreshRandomSizes();
		RefreshRandomInitialRotations();
		RefreshRandomRotationSpeeds();
		RefreshRandomLocalVelocities();
		RefreshRandomVelocities();
	}

	public void RefreshRandomSizes()
	{
		SetSizeRandom(thisInstance, sizeMin, sizeMax);
	}

	public void RefreshRandomInitialRotations()
	{
		if (separateRotationAxes)
		{
			SetInitialRotationRandom(thisInstance, initialRotationMin3d, initialRotationMax3d);
		}
		else
		{
			SetInitialRotationRandom(thisInstance, initialRotationMin, initialRotationMax);
		}
	}

	public void RefreshRandomRotationSpeeds()
	{
		if (separateRotationAxes)
		{
			SetRotationRandom(thisInstance, rotationSpeedMin3d, rotationSpeedMax3d);
		}
		else
		{
			SetRotationRandom(thisInstance, rotationSpeedMin, rotationSpeedMax);
		}
	}

	public void RefreshRandomLocalVelocities()
	{
		SetLocalVelocityRandom(thisInstance, initialLocalVelocityMin, initialLocalVelocityMax);
	}

	public void RefreshRandomVelocities()
	{
		SetVelocityRandom(thisInstance, initialVelocityMin, initialVelocityMax);
	}

	public bool IsReportingBadUpdateRate()
	{
		if (isPrewarming || inTransition)
		{
			return false;
		}
		reportUpdateStepper--;
		if (reportUpdateStepper == 0)
		{
			reportUpdateStepper = 100;
			isReportingBadUpdateRate = localTime > 1f && localDeltaTime > 0.04f;
		}
		return isReportingBadUpdateRate;
	}

	public float GetDeltaTime()
	{
		return t;
	}

	public SimplexNoise GetSimplex()
	{
		if (turbulenceSimplex == null)
		{
			turbulenceSimplex = new SimplexNoise();
		}
		return turbulenceSimplex;
	}

	public void Kill(int p)
	{
		if (p < particleCount && p >= 0)
		{
			playgroundCache.changedByPropertyDeath[p] = true;
			playgroundCache.life[p] = lifetime;
			ref Vector3 reference = ref playgroundCache.position[p];
			reference = PlaygroundC.initialTargetPosition;
			particleCache[p].position = playgroundCache.position[p];
			playgroundCache.manipulatorId[p] = 0;
		}
	}

	public void KillAndSendManipulatorDeathEvents(int p)
	{
		if (p >= particleCount || p < 0)
		{
			return;
		}
		playgroundCache.changedByPropertyDeath[p] = true;
		playgroundCache.life[p] = lifetime;
		ref Vector3 reference = ref playgroundCache.position[p];
		reference = PlaygroundC.initialTargetPosition;
		particleCache[p].position = playgroundCache.position[p];
		if (playgroundCache.manipulatorId[p] == 0)
		{
			return;
		}
		for (int i = 0; i < manipulators.Count; i++)
		{
			if (manipulators[i].trackParticles && manipulators[i].ContainsParticle(particleSystemId, p))
			{
				playgroundCache.manipulatorId[p] = 0;
				manipulators[i].RemoveParticle(particleSystemId, p);
				if (manipulators[i].sendEventExit)
				{
					UpdateEventParticle(manipulators[i].manipulatorEventParticle, p);
					manipulators[i].SendParticleEventDeath();
				}
			}
		}
		if (playgroundCache.manipulatorId[p] != 0)
		{
			for (int j = 0; j < PlaygroundC.reference.manipulators.Count; j++)
			{
				if (PlaygroundC.reference.manipulators[j].trackParticles && PlaygroundC.reference.manipulators[j].ContainsParticle(particleSystemId, p))
				{
					PlaygroundC.reference.manipulators[j].RemoveParticle(particleSystemId, p);
					if (PlaygroundC.reference.manipulators[j].sendEventExit)
					{
						UpdateEventParticle(PlaygroundC.reference.manipulators[j].manipulatorEventParticle, p);
						PlaygroundC.reference.manipulators[j].SendParticleEventDeath();
					}
				}
			}
		}
		playgroundCache.manipulatorId[p] = 0;
	}

	public void SetNoForce(int p, bool noForce)
	{
		playgroundCache.noForce[p] = noForce;
	}

	public bool NoForce(int p)
	{
		return playgroundCache.noForce[p];
	}

	public void Translate(int p, Vector3 translation)
	{
		p = Mathf.Clamp(p, 0, playgroundCache.position.Length);
		playgroundCache.position[p] += translation;
		particleCache[p].position = playgroundCache.position[p];
		playgroundCache.isCalculatedThisFrame[p] = true;
	}

	public void ParticlePosition(int p, Vector3 position)
	{
		p = Mathf.Clamp(p, 0, playgroundCache.position.Length);
		playgroundCache.position[p] = position;
		particleCache[p].position = position;
		playgroundCache.previousParticlePosition[p] = position;
		playgroundCache.isCalculatedThisFrame[p] = true;
	}

	public void ParticleColor(int p, Color32 color)
	{
		p = Mathf.Clamp(p, 0, playgroundCache.color.Length);
		playgroundCache.changedByPropertyColor[p] = true;
		playgroundCache.color[p] = color;
		SetParticleColorInternal(p, playgroundCache.color[p]);
	}

	private void SetParticleColorInternal(int index, Color32 color)
	{
		particleCache[index].startColor = color;
	}

	private Color32 GetParticleColorInternal(int index)
	{
		return particleCache[index].startColor;
	}

	public void ParticleSize(int p, float size)
	{
		p = Mathf.Clamp(p, 0, playgroundCache.size.Length);
		playgroundCache.changedByPropertySize[p] = true;
		playgroundCache.size[p] = size;
		particleCache[p].startSize = size;
	}

	public void ParticleVelocity(int p, Vector3 velocity)
	{
		p = Mathf.Clamp(p, 0, playgroundCache.velocity.Length);
		playgroundCache.velocity[p] = velocity;
	}

	public void PositionToTransformPoint(int p, Vector3 position, Transform targetTransform)
	{
		ref Vector3 reference = ref playgroundCache.position[p];
		reference = targetTransform.TransformPoint(position);
	}

	public void PositionToInverseTransformPoint(int p, Vector3 position, Transform targetTransform)
	{
		ref Vector3 reference = ref playgroundCache.position[p];
		reference = targetTransform.InverseTransformPoint(position);
	}

	public Vector3 GetParticlePosition(int p)
	{
		return playgroundCache.position[p];
	}

	public Vector3 GetSourcePosition(int p)
	{
		return playgroundCache.targetPosition[p];
	}

	public void SetHasActiveParticles()
	{
		hasActiveParticles = true;
		threadHadNoActiveParticles = false;
	}

	public bool HasSeveralManipulatorEvents()
	{
		return hasSeveralManipulatorEvents;
	}

	public void ProtectParticleFromManipulator(int particle, ManipulatorObjectC manipulator)
	{
		if (manipulator.transform.Update())
		{
			playgroundCache.excludeFromManipulatorId[particle] = manipulator.transform.instanceID;
		}
	}

	public void RemoveParticleProtection(int particle)
	{
		playgroundCache.excludeFromManipulatorId[particle] = 0;
	}

	public bool IsSettingLifetime()
	{
		return isSettingLifetime;
	}

	public bool IsPrewarming()
	{
		return isPrewarming;
	}

	public bool IsParticleInsideManipulator(int particleId, ManipulatorObjectC manipulator)
	{
		return manipulator.Contains(playgroundCache.position[particleId], (!localSpace) ? manipulator.transform.position : manipulator.transform.localPosition);
	}

	public int GetSplineIndex()
	{
		return splineIndex;
	}

	public int GetTransformIndex()
	{
		return transformIndex;
	}

	public void RefreshMaskSorting()
	{
		playgroundCache.maskSorting = new int[particleCount];
		switch (particleMaskSorting)
		{
		case MASKSORTINGC.Linear:
		{
			for (int k = 0; k < playgroundCache.maskSorting.Length; k++)
			{
				playgroundCache.maskSorting[k] = k;
			}
			break;
		}
		case MASKSORTINGC.Reversed:
		{
			for (int j = 0; j < playgroundCache.maskSorting.Length; j++)
			{
				playgroundCache.maskSorting[j] = playgroundCache.maskSorting.Length - 1 - j;
			}
			break;
		}
		case MASKSORTINGC.Scrambled:
		{
			for (int i = 0; i < playgroundCache.maskSorting.Length; i++)
			{
				playgroundCache.maskSorting[i] = (playgroundCache.maskSorting[i] = i);
			}
			PlaygroundC.ShuffleArray(playgroundCache.maskSorting);
			break;
		}
		}
		previousMaskSorting = particleMaskSorting;
	}

	public void RefreshMaskSorting(int[] maskSortingArray)
	{
		playgroundCache.maskSorting = new int[particleCount];
		for (int i = 0; i < particleCount; i++)
		{
			if (i < maskSortingArray.Length)
			{
				playgroundCache.maskSorting[i] = maskSortingArray[i];
			}
			else
			{
				playgroundCache.maskSorting[i] = i;
			}
		}
		previousMaskSorting = particleMaskSorting;
	}

	public static PlaygroundParticlesC CreatePlaygroundParticles(Texture2D[] images, string name, Vector3 position, Quaternion rotation, Vector3 offset, float particleSize, float scale, Material material)
	{
		PlaygroundParticlesC playgroundParticlesC = CreateParticleObject(name, position, rotation, particleSize, material);
		int[] array = new int[images.Length];
		for (int i = 0; i < images.Length; i++)
		{
			array[i] = images[i].width * images[i].height;
		}
		playgroundParticlesC.particleCache = new ParticleSystem.Particle[array[PlaygroundC.Largest(array)]];
		OnCreatePlaygroundParticles(playgroundParticlesC);
		for (int i = 0; i < images.Length; i++)
		{
			playgroundParticlesC.states.Add(new ParticleStateC());
			playgroundParticlesC.states[playgroundParticlesC.states.Count - 1].ConstructParticles(images[i], scale, offset, "State 0", null);
		}
		return playgroundParticlesC;
	}

	public static void OnCreatePlaygroundParticles(PlaygroundParticlesC playgroundParticles)
	{
		playgroundParticles.playgroundCache = new PlaygroundCache();
		playgroundParticles.paint = new PaintObjectC();
		playgroundParticles.states = new List<ParticleStateC>();
		playgroundParticles.projection = new ParticleProjectionC();
		playgroundParticles.colliders = new List<PlaygroundColliderC>();
		playgroundParticles.particleSystemId = PlaygroundC.particlesQuantity - 1;
		playgroundParticles.projection.projectionTransform = playgroundParticles.particleSystemTransform;
		playgroundParticles.playgroundCache.initialSize = new float[playgroundParticles.particleCount];
		playgroundParticles.playgroundCache.initialSize = RandomFloat(playgroundParticles.playgroundCache.initialSize.Length, playgroundParticles.sizeMin, playgroundParticles.sizeMax, playgroundParticles.internalRandom01);
		playgroundParticles.previousParticleCount = playgroundParticles.particleCount;
		playgroundParticles.lifetimeSize = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
		playgroundParticles.shurikenParticleSystem.Emit(playgroundParticles.particleCount);
		playgroundParticles.shurikenParticleSystem.GetParticles(playgroundParticles.particleCache);
		for (int i = 0; i < playgroundParticles.particleCache.Length; i++)
		{
			playgroundParticles.playgroundCache.size[i] = playgroundParticles.playgroundCache.initialSize[i];
		}
		SetParticleCount(playgroundParticles, playgroundParticles.particleCount);
		if (PlaygroundC.reference != null)
		{
			PlaygroundC.particlesQuantity++;
			PlaygroundC.reference.particleSystems.Add(playgroundParticles);
			playgroundParticles.particleSystemId = PlaygroundC.particlesQuantity - 1;
		}
	}

	public static PlaygroundParticlesC CreateParticleObject(string name, Vector3 position, Quaternion rotation, float particleSize, Material material)
	{
		GameObject gameObject = PlaygroundC.ResourceInstantiate("Particle Playground System");
		PlaygroundParticlesC component = gameObject.GetComponent<PlaygroundParticlesC>();
		component.particleSystemGameObject = gameObject;
		component.particleSystemGameObject.name = name;
		component.shurikenParticleSystem = component.particleSystemGameObject.GetComponent<ParticleSystem>();
		component.particleSystemRenderer = component.shurikenParticleSystem.GetComponent<Renderer>();
		component.particleSystemRenderer2 = component.shurikenParticleSystem.GetComponent<Renderer>() as ParticleSystemRenderer;
		component.particleSystemTransform = component.particleSystemGameObject.transform;
		component.sourceTransform = component.particleSystemTransform;
		component.source = SOURCEC.Transform;
		component.particleSystemTransform.position = position;
		component.particleSystemTransform.rotation = rotation;
		if (PlaygroundC.reference.autoGroup && component.particleSystemTransform.parent == null)
		{
			component.particleSystemTransform.parent = PlaygroundC.referenceTransform;
		}
		if (component.particleSystemRenderer.sharedMaterial == null)
		{
			component.particleSystemRenderer.sharedMaterial = material;
		}
		return component;
	}

	public static WorldObject NewWorldObject(Transform meshTransform)
	{
		WorldObject worldObject = new WorldObject();
		if ((bool)meshTransform.GetComponentInChildren<MeshFilter>())
		{
			worldObject.transform = meshTransform;
			worldObject.Initialize();
		}
		else
		{
			Debug.Log("Could not find a mesh in " + meshTransform.name + ".");
		}
		return worldObject;
	}

	public static SkinnedWorldObject NewSkinnedWorldObject(Transform meshTransform)
	{
		SkinnedWorldObject skinnedWorldObject = new SkinnedWorldObject();
		if ((bool)meshTransform.GetComponentInChildren<SkinnedMeshRenderer>())
		{
			skinnedWorldObject.transform = meshTransform;
			skinnedWorldObject.Initialize();
		}
		else
		{
			Debug.Log("Could not find a skinned mesh in " + meshTransform.name + ".");
		}
		return skinnedWorldObject;
	}

	public static SkinnedWorldObject NewSkinnedWorldObject(Transform meshTransform, int downResolution)
	{
		SkinnedWorldObject skinnedWorldObject = NewSkinnedWorldObject(meshTransform);
		skinnedWorldObject.downResolution = downResolution;
		return skinnedWorldObject;
	}

	public static PaintObjectC NewPaintObject(PlaygroundParticlesC playgroundParticles)
	{
		PaintObjectC result = (playgroundParticles.paint = new PaintObjectC());
		playgroundParticles.paint.Initialize();
		return result;
	}

	public static ParticleProjectionC NewProjectionObject(PlaygroundParticlesC playgroundParticles)
	{
		ParticleProjectionC result = (playgroundParticles.projection = new ParticleProjectionC());
		playgroundParticles.projection.Initialize();
		return result;
	}

	public static ManipulatorObjectC NewManipulatorObject(MANIPULATORTYPEC type, LayerMask affects, Transform manipulatorTransform, float size, float strength, PlaygroundParticlesC playgroundParticles)
	{
		ManipulatorObjectC manipulatorObjectC = new ManipulatorObjectC();
		manipulatorObjectC.type = type;
		manipulatorObjectC.affects = affects;
		manipulatorObjectC.transform.transform = manipulatorTransform;
		manipulatorObjectC.size = size;
		manipulatorObjectC.strength = strength;
		manipulatorObjectC.bounds = new Bounds(Vector3.zero, new Vector3(size, size, size));
		manipulatorObjectC.property = new ManipulatorPropertyC();
		manipulatorObjectC.Update();
		if (playgroundParticles == null)
		{
			PlaygroundC.reference.manipulators.Add(manipulatorObjectC);
		}
		else
		{
			playgroundParticles.manipulators.Add(manipulatorObjectC);
		}
		return manipulatorObjectC;
	}

	public static void GetPosition(SkinnedWorldObject particleStateWorldObject, bool updateNormals)
	{
		if (updateNormals)
		{
			particleStateWorldObject.normals = particleStateWorldObject.mesh.normals;
		}
		Vector3[] vertices = particleStateWorldObject.mesh.vertices;
		BoneWeight[] weights = particleStateWorldObject.mesh.boneWeights;
		Matrix4x4[] bindposes = particleStateWorldObject.mesh.bindposes;
		Matrix4x4[] boneMatrices = new Matrix4x4[particleStateWorldObject.renderer.bones.Length];
		int i;
		for (i = 0; i < boneMatrices.Length; i++)
		{
			ref Matrix4x4 reference = ref boneMatrices[i];
			reference = particleStateWorldObject.renderer.bones[i].localToWorldMatrix * bindposes[i];
		}
		PlaygroundC.RunAsync(() =>
		{
			Matrix4x4 matrix4x = default;
			for (i = 0; i < vertices.Length; i++)
			{
				BoneWeight boneWeight = weights[i];
				Matrix4x4 matrix4x2 = boneMatrices[boneWeight.boneIndex0];
				Matrix4x4 matrix4x3 = boneMatrices[boneWeight.boneIndex1];
				Matrix4x4 matrix4x4 = boneMatrices[boneWeight.boneIndex2];
				Matrix4x4 matrix4x5 = boneMatrices[boneWeight.boneIndex3];
				for (int j = 0; j < 16; j++)
				{
					matrix4x[j] = matrix4x2[j] * boneWeight.weight0 + matrix4x3[j] * boneWeight.weight1 + matrix4x4[j] * boneWeight.weight2 + matrix4x5[j] * boneWeight.weight3;
				}
				ref Vector3 reference2 = ref vertices[i];
				reference2 = matrix4x.MultiplyPoint3x4(vertices[i]);
			}
			particleStateWorldObject.vertexPositions = vertices;
		});
	}

	public static void GetPosition(Vector3[] v3, WorldObject particleStateWorldObject)
	{
		if (particleStateWorldObject.meshFilter.sharedMesh != particleStateWorldObject.mesh)
		{
			particleStateWorldObject.mesh = particleStateWorldObject.meshFilter.sharedMesh;
		}
		v3 = particleStateWorldObject.mesh.vertices;
	}

	public static void GetProceduralPosition(Vector3[] v3, WorldObject particleStateWorldObject)
	{
		if (particleStateWorldObject.meshFilter.sharedMesh != particleStateWorldObject.mesh)
		{
			particleStateWorldObject.mesh = particleStateWorldObject.meshFilter.sharedMesh;
		}
		Vector3[] vertices = particleStateWorldObject.mesh.vertices;
		if (v3.Length != vertices.Length)
		{
			v3 = new Vector3[vertices.Length];
		}
		for (int i = 0; i < v3.Length; i++)
		{
			ref Vector3 reference = ref v3[i];
			reference = particleStateWorldObject.transform.TransformPoint(vertices[i % vertices.Length]);
		}
	}

	public static void GetNormals(Vector3[] v3, WorldObject particleStateWorldObject)
	{
		v3 = particleStateWorldObject.mesh.normals;
	}

	public static void SetSize(PlaygroundParticlesC playgroundParticles, float size)
	{
		for (int i = 0; i < playgroundParticles.particleCache.Length; i++)
		{
			playgroundParticles.playgroundCache.initialSize[i] = size;
			playgroundParticles.particleCache[i].startSize = size;
		}
	}

	public static void SetSizeRandom(PlaygroundParticlesC playgroundParticles, float sizeMinimum, float sizeMaximum)
	{
		playgroundParticles.playgroundCache.initialSize = RandomFloat(playgroundParticles.particleCache.Length, sizeMinimum, sizeMaximum, playgroundParticles.internalRandom01);
		playgroundParticles.sizeMin = sizeMinimum;
		playgroundParticles.sizeMax = sizeMaximum;
		playgroundParticles.previousSizeMin = playgroundParticles.sizeMin;
		playgroundParticles.previousSizeMax = playgroundParticles.sizeMax;
	}

	public static void SetRotationRandom(PlaygroundParticlesC playgroundParticles, float rotationMinimum, float rotationMaximum)
	{
		playgroundParticles.playgroundCache.rotationSpeed = RandomFloat(playgroundParticles.particleCache.Length, rotationMinimum, rotationMaximum, playgroundParticles.internalRandom01);
		if (playgroundParticles.playgroundCache.rotation == null || playgroundParticles.playgroundCache.rotation.Length != playgroundParticles.playgroundCache.rotationSpeed.Length)
		{
			playgroundParticles.playgroundCache.rotation = new float[playgroundParticles.playgroundCache.rotationSpeed.Length];
		}
		for (int i = 0; i < playgroundParticles.particleCache.Length; i++)
		{
			playgroundParticles.playgroundCache.rotation[i] = playgroundParticles.playgroundCache.initialRotation[i];
		}
		playgroundParticles.rotationSpeedMin = rotationMinimum;
		playgroundParticles.rotationSpeedMax = rotationMaximum;
		playgroundParticles.previousRotationSpeedMin = playgroundParticles.rotationSpeedMin;
		playgroundParticles.previousRotationSpeedMax = playgroundParticles.rotationSpeedMax;
	}

	public static void SetInitialRotationRandom(PlaygroundParticlesC playgroundParticles, float rotationMinimum, float rotationMaximum)
	{
		playgroundParticles.playgroundCache.initialRotation = RandomFloat(playgroundParticles.particleCache.Length, rotationMinimum, rotationMaximum, playgroundParticles.internalRandom01);
		if (playgroundParticles.playgroundCache.rotation == null || playgroundParticles.playgroundCache.rotation.Length != playgroundParticles.playgroundCache.initialRotation.Length)
		{
			playgroundParticles.playgroundCache.rotation = new float[playgroundParticles.playgroundCache.initialRotation.Length];
		}
		for (int i = 0; i < playgroundParticles.playgroundCache.initialRotation.Length; i++)
		{
			playgroundParticles.playgroundCache.rotation[i] = playgroundParticles.playgroundCache.initialRotation[i];
		}
		playgroundParticles.initialRotationMin = rotationMinimum;
		playgroundParticles.initialRotationMax = rotationMaximum;
		playgroundParticles.previousInitialRotationMin = playgroundParticles.initialRotationMin;
		playgroundParticles.previousInitialRotationMax = playgroundParticles.initialRotationMax;
	}

	public static void SetRotationRandom(PlaygroundParticlesC playgroundParticles, Vector3 rotationMinimum, Vector3 rotationMaximum)
	{
		playgroundParticles.playgroundCache.rotationSpeed3d = RandomVector3(playgroundParticles.particleCache.Length, rotationMinimum, rotationMaximum, playgroundParticles.internalRandom01);
		if (playgroundParticles.playgroundCache.rotation3d == null || playgroundParticles.playgroundCache.rotation3d.Length != playgroundParticles.playgroundCache.rotationSpeed3d.Length)
		{
			playgroundParticles.playgroundCache.rotation3d = new Vector3[playgroundParticles.playgroundCache.initialRotation3d.Length];
		}
		for (int i = 0; i < playgroundParticles.particleCache.Length; i++)
		{
			ref Vector3 reference = ref playgroundParticles.playgroundCache.rotation3d[i];
			reference = playgroundParticles.playgroundCache.initialRotation3d[i];
		}
		playgroundParticles.rotationSpeedMin3d = rotationMinimum;
		playgroundParticles.rotationSpeedMax3d = rotationMaximum;
		playgroundParticles.previousRotationSpeedMin3d = playgroundParticles.rotationSpeedMin3d;
		playgroundParticles.previousRotationSpeedMax3d = playgroundParticles.rotationSpeedMax3d;
	}

	public static void SetInitialRotationRandom(PlaygroundParticlesC playgroundParticles, Vector3 rotationMinimum, Vector3 rotationMaximum)
	{
		playgroundParticles.playgroundCache.initialRotation3d = RandomVector3(playgroundParticles.particleCache.Length, rotationMinimum, rotationMaximum, playgroundParticles.internalRandom01);
		if (playgroundParticles.playgroundCache.rotation3d == null || playgroundParticles.playgroundCache.rotation3d.Length != playgroundParticles.playgroundCache.initialRotation3d.Length)
		{
			playgroundParticles.playgroundCache.rotation3d = new Vector3[playgroundParticles.playgroundCache.initialRotation3d.Length];
		}
		for (int i = 0; i < playgroundParticles.particleCache.Length; i++)
		{
			ref Vector3 reference = ref playgroundParticles.playgroundCache.rotation3d[i];
			reference = playgroundParticles.playgroundCache.initialRotation3d[i];
		}
		playgroundParticles.initialRotationMin3d = rotationMinimum;
		playgroundParticles.initialRotationMax3d = rotationMaximum;
		playgroundParticles.previousInitialRotationMin3d = playgroundParticles.initialRotationMin3d;
		playgroundParticles.previousInitialRotationMax3d = playgroundParticles.initialRotationMax3d;
	}

	public static void SetVelocityRandom(PlaygroundParticlesC playgroundParticles, Vector3 velocityMinimum, Vector3 velocityMaximum)
	{
		playgroundParticles.playgroundCache.initialVelocity = new Vector3[playgroundParticles.particleCount];
		for (int i = 0; i < playgroundParticles.particleCount; i++)
		{
			if (playgroundParticles.initialVelocityMethod == MINMAXVECTOR3METHOD.Spherical)
			{
				ref Vector3 reference = ref playgroundParticles.playgroundCache.initialVelocity[i];
				reference = RandomRangeSpherical(playgroundParticles.internalRandom01, velocityMinimum.x, velocityMaximum.x);
			}
			else if (playgroundParticles.initialVelocityMethod == MINMAXVECTOR3METHOD.SphericalLinear)
			{
				ref Vector3 reference2 = ref playgroundParticles.playgroundCache.initialVelocity[i];
				reference2 = RandomRangeSpherical(playgroundParticles.internalRandom01, velocityMinimum.x, velocityMaximum.x, (float)i * 1f / ((float)playgroundParticles.particleCount * 1f));
			}
			else if (playgroundParticles.initialVelocityMethod == MINMAXVECTOR3METHOD.RectangularLinear)
			{
				ref Vector3 reference3 = ref playgroundParticles.playgroundCache.initialVelocity[i];
				reference3 = Vector3.Lerp(velocityMinimum, velocityMaximum, (float)i * 1f / ((float)playgroundParticles.particleCount * 1f));
			}
			else
			{
				ref Vector3 reference4 = ref playgroundParticles.playgroundCache.initialVelocity[i];
				reference4 = RandomRange(playgroundParticles.internalRandom01, velocityMinimum, velocityMaximum);
			}
		}
		playgroundParticles.initialVelocityMin = velocityMinimum;
		playgroundParticles.initialVelocityMax = velocityMaximum;
		playgroundParticles.previousVelocityMin = playgroundParticles.initialVelocityMin;
		playgroundParticles.previousVelocityMax = playgroundParticles.initialVelocityMax;
		playgroundParticles.previousVelocityMethod = playgroundParticles.initialVelocityMethod;
	}

	public static void SetLocalVelocityRandom(PlaygroundParticlesC playgroundParticles, Vector3 velocityMinimum, Vector3 velocityMaximum)
	{
		playgroundParticles.playgroundCache.initialLocalVelocity = new Vector3[playgroundParticles.particleCount];
		for (int i = 0; i < playgroundParticles.particleCount; i++)
		{
			if (playgroundParticles.initialLocalVelocityMethod == MINMAXVECTOR3METHOD.Spherical)
			{
				ref Vector3 reference = ref playgroundParticles.playgroundCache.initialLocalVelocity[i];
				reference = RandomRangeSpherical(playgroundParticles.internalRandom01, velocityMinimum.x, velocityMaximum.x);
			}
			else if (playgroundParticles.initialLocalVelocityMethod == MINMAXVECTOR3METHOD.SphericalLinear)
			{
				ref Vector3 reference2 = ref playgroundParticles.playgroundCache.initialLocalVelocity[i];
				reference2 = RandomRangeSpherical(playgroundParticles.internalRandom01, velocityMinimum.x, velocityMaximum.x, (float)i * 1f / ((float)playgroundParticles.particleCount * 1f));
			}
			else if (playgroundParticles.initialLocalVelocityMethod == MINMAXVECTOR3METHOD.RectangularLinear)
			{
				ref Vector3 reference3 = ref playgroundParticles.playgroundCache.initialLocalVelocity[i];
				reference3 = Vector3.Lerp(velocityMinimum, velocityMaximum, (float)i * 1f / ((float)playgroundParticles.particleCount * 1f));
			}
			else
			{
				ref Vector3 reference4 = ref playgroundParticles.playgroundCache.initialLocalVelocity[i];
				reference4 = RandomRange(playgroundParticles.internalRandom01, velocityMinimum, velocityMaximum);
			}
		}
		playgroundParticles.initialLocalVelocityMin = velocityMinimum;
		playgroundParticles.initialLocalVelocityMax = velocityMaximum;
		playgroundParticles.previousLocalVelocityMin = playgroundParticles.initialLocalVelocityMin;
		playgroundParticles.previousLocalVelocityMax = playgroundParticles.initialLocalVelocityMax;
		playgroundParticles.previousLocalVelocityMethod = playgroundParticles.initialLocalVelocityMethod;
	}

	public static void SetMaterial(PlaygroundParticlesC playgroundParticles, Material particleMaterial)
	{
		playgroundParticles.particleSystemRenderer.sharedMaterial = particleMaterial;
	}

	public static void SetAlpha(PlaygroundParticlesC playgroundParticles, float alpha)
	{
		for (int i = 0; i < playgroundParticles.particleCache.Length; i++)
		{
			Color color = playgroundParticles.GetParticleColorInternal(i);
			color.a = alpha;
			playgroundParticles.SetParticleColorInternal(i, color);
		}
	}

	public static void Translate(PlaygroundParticlesC playgroundParticles, Vector3 direction)
	{
		for (int i = 0; i < playgroundParticles.particleCache.Length; i++)
		{
			playgroundParticles.particleCache[i].position += direction;
		}
	}

	public static void Add(PlaygroundParticlesC playgroundParticles, ParticleStateC state)
	{
		playgroundParticles.states.Add(state);
		state.Initialize();
	}

	public static void Add(PlaygroundParticlesC playgroundParticles, Texture2D image, float scale, Vector3 offset, string stateName, Transform stateTransform)
	{
		playgroundParticles.states.Add(new ParticleStateC());
		playgroundParticles.states[playgroundParticles.states.Count - 1].ConstructParticles(image, scale, offset, stateName, stateTransform);
	}

	public static void Add(PlaygroundParticlesC playgroundParticles, Texture2D image, Texture2D depthmap, float depthmapStrength, float scale, Vector3 offset, string stateName, Transform stateTransform)
	{
		playgroundParticles.states.Add(new ParticleStateC());
		playgroundParticles.states[playgroundParticles.states.Count - 1].ConstructParticles(image, scale, offset, stateName, stateTransform);
		playgroundParticles.states[playgroundParticles.states.Count - 1].stateDepthmap = depthmap;
		playgroundParticles.states[playgroundParticles.states.Count - 1].stateDepthmapStrength = depthmapStrength;
	}

	public static void Destroy(PlaygroundParticlesC playgroundParticles)
	{
		Clear(playgroundParticles);
		UnityEngine.Object.DestroyImmediate(playgroundParticles.particleSystemGameObject);
		playgroundParticles = null;
	}

	public static void SetLifetime(PlaygroundParticlesC playgroundParticles, SORTINGC sorting, float time)
	{
		if (!playgroundParticles.enabled || playgroundParticles.isSettingLifetime || playgroundParticles.isSettingParticleCount)
		{
			return;
		}
		if (playgroundParticles.internalRandom01 == null)
		{
			playgroundParticles.RefreshSystemRandom();
		}
		playgroundParticles.isSettingLifetime = true;
		if (playgroundParticles.multithreadedStartup)
		{
			PlaygroundC.RunAsync(() =>
			{
				SetLifetimeAsyncFriendly(playgroundParticles, sorting, time);
			});
		}
		else
		{
			SetLifetimeAsyncFriendly(playgroundParticles, sorting, time);
		}
	}

	public static void SetLifetimeAsyncFriendly(PlaygroundParticlesC playgroundParticles, SORTINGC sorting, float time)
	{
		playgroundParticles.lifetime = time;
		SetLifetimeSubtraction(playgroundParticles);
		playgroundParticles.playgroundCache.lifetimeOffset = new float[playgroundParticles.particleCount];
		int num = playgroundParticles.playgroundCache.lifetimeOffset.Length;
		if (playgroundParticles.source != SOURCEC.Script)
		{
			float num2 = playgroundParticles.lifetimeEmission;
			if (!playgroundParticles.loop)
			{
				num2 *= 0.95f;
			}
			switch (sorting)
			{
			case SORTINGC.Scrambled:
			{
				for (int i = 0; i < playgroundParticles.particleCount; i++)
				{
					if (num != playgroundParticles.playgroundCache.lifetimeOffset.Length)
					{
						playgroundParticles.isSettingLifetime = false;
						return;
					}
					playgroundParticles.playgroundCache.lifetimeOffset[i] = RandomRange(playgroundParticles.internalRandom01, 0f, playgroundParticles.lifetime * num2);
				}
				break;
			}
			case SORTINGC.ScrambledLinear:
			{
				for (int m = 0; m < playgroundParticles.particleCount; m++)
				{
					if (num != playgroundParticles.playgroundCache.lifetimeOffset.Length)
					{
						playgroundParticles.isSettingLifetime = false;
						return;
					}
					float num5 = (float)m * 1f / ((float)playgroundParticles.particleCount * 1f);
					playgroundParticles.playgroundCache.lifetimeOffset[m] = playgroundParticles.lifetime * num2 * num5;
				}
				for (int num6 = playgroundParticles.playgroundCache.lifetimeOffset.Length - 1; num6 > 0; num6--)
				{
					if (num != playgroundParticles.playgroundCache.lifetimeOffset.Length)
					{
						playgroundParticles.isSettingLifetime = false;
						return;
					}
					int num7 = playgroundParticles.internalRandom01.Next(0, num6);
					float num8 = playgroundParticles.playgroundCache.lifetimeOffset[num6];
					playgroundParticles.playgroundCache.lifetimeOffset[num6] = playgroundParticles.playgroundCache.lifetimeOffset[num7];
					playgroundParticles.playgroundCache.lifetimeOffset[num7] = num8;
				}
				break;
			}
			case SORTINGC.Linear:
			{
				for (int num15 = 0; num15 < playgroundParticles.particleCount; num15++)
				{
					if (num != playgroundParticles.playgroundCache.lifetimeOffset.Length)
					{
						playgroundParticles.isSettingLifetime = false;
						return;
					}
					float num16 = (float)num15 * 1f / ((float)playgroundParticles.particleCount * 1f);
					playgroundParticles.playgroundCache.lifetimeOffset[num15] = playgroundParticles.lifetime * num2 * num16;
				}
				break;
			}
			case SORTINGC.Reversed:
			{
				int num9 = 0;
				for (int num10 = playgroundParticles.particleCount - 1; num10 >= 0; num10--)
				{
					if (num != playgroundParticles.playgroundCache.lifetimeOffset.Length)
					{
						playgroundParticles.isSettingLifetime = false;
						return;
					}
					float num11 = (float)num9 * 1f / ((float)playgroundParticles.particleCount * 1f);
					num9++;
					playgroundParticles.playgroundCache.lifetimeOffset[num10] = playgroundParticles.lifetime * num2 * num11;
				}
				break;
			}
			case SORTINGC.NearestNeighborReversed:
			{
				playgroundParticles.nearestNeighborOrigin = Mathf.Clamp(playgroundParticles.nearestNeighborOrigin, 0, playgroundParticles.particleCount - 1);
				float[] array2 = new float[playgroundParticles.particleCount];
				float num12 = 0f;
				Vector3 vector2 = Vector3.zero;
				switch (playgroundParticles.nearestNeighborOriginMethod)
				{
				case NEARESTNEIGHBORORIGINMETHOD.SourcePoint:
					vector2 = playgroundParticles.playgroundCache.targetPosition[playgroundParticles.nearestNeighborOrigin % playgroundParticles.particleCount];
					break;
				case NEARESTNEIGHBORORIGINMETHOD.Vector3:
					vector2 = playgroundParticles.nearestNeighborOriginVector3;
					break;
				case NEARESTNEIGHBORORIGINMETHOD.Transform:
					vector2 = playgroundParticles.nearestNeighborOriginTransformPosition;
					break;
				}
				for (int n = 0; n < playgroundParticles.particleCount; n++)
				{
					if (num != playgroundParticles.playgroundCache.lifetimeOffset.Length)
					{
						playgroundParticles.isSettingLifetime = false;
						return;
					}
					array2[n % playgroundParticles.particleCount] = Vector3.SqrMagnitude(vector2 - playgroundParticles.playgroundCache.targetPosition[n % playgroundParticles.particleCount]);
					if (array2[n % playgroundParticles.particleCount] > num12)
					{
						num12 = array2[n % playgroundParticles.particleCount];
					}
				}
				if (num12 > 0f)
				{
					for (int num13 = 0; num13 < playgroundParticles.particleCount; num13++)
					{
						if (num != playgroundParticles.playgroundCache.lifetimeOffset.Length)
						{
							playgroundParticles.isSettingLifetime = false;
							return;
						}
						playgroundParticles.playgroundCache.lifetimeOffset[num13 % playgroundParticles.particleCount] = Mathf.Lerp(playgroundParticles.lifetime * (num2 - 0.001f), 0f, array2[num13 % playgroundParticles.particleCount] / num12);
					}
					break;
				}
				for (int num14 = 0; num14 < playgroundParticles.particleCount; num14++)
				{
					if (num != playgroundParticles.playgroundCache.lifetimeOffset.Length)
					{
						playgroundParticles.isSettingLifetime = false;
						return;
					}
					playgroundParticles.playgroundCache.lifetimeOffset[num14 % playgroundParticles.particleCount] = 0f;
				}
				break;
			}
			case SORTINGC.NearestNeighbor:
			{
				playgroundParticles.nearestNeighborOrigin = Mathf.Clamp(playgroundParticles.nearestNeighborOrigin, 0, playgroundParticles.particleCount - 1);
				float[] array = new float[playgroundParticles.particleCount];
				float num4 = 0f;
				Vector3 vector = Vector3.zero;
				switch (playgroundParticles.nearestNeighborOriginMethod)
				{
				case NEARESTNEIGHBORORIGINMETHOD.SourcePoint:
					vector = playgroundParticles.playgroundCache.targetPosition[playgroundParticles.nearestNeighborOrigin % playgroundParticles.particleCount];
					break;
				case NEARESTNEIGHBORORIGINMETHOD.Vector3:
					vector = playgroundParticles.nearestNeighborOriginVector3;
					break;
				case NEARESTNEIGHBORORIGINMETHOD.Transform:
					vector = playgroundParticles.nearestNeighborOriginTransformPosition;
					break;
				}
				for (int j = 0; j < playgroundParticles.particleCount; j++)
				{
					if (num != playgroundParticles.playgroundCache.lifetimeOffset.Length)
					{
						playgroundParticles.isSettingLifetime = false;
						return;
					}
					array[j % playgroundParticles.particleCount] = Vector3.SqrMagnitude(vector - playgroundParticles.playgroundCache.targetPosition[j % playgroundParticles.particleCount]);
					if (array[j % playgroundParticles.particleCount] > num4)
					{
						num4 = array[j % playgroundParticles.particleCount];
					}
				}
				if (num4 > 0f)
				{
					for (int k = 0; k < playgroundParticles.particleCount; k++)
					{
						if (num != playgroundParticles.playgroundCache.lifetimeOffset.Length)
						{
							playgroundParticles.isSettingLifetime = false;
							return;
						}
						playgroundParticles.playgroundCache.lifetimeOffset[k % playgroundParticles.particleCount] = Mathf.Lerp(0f, playgroundParticles.lifetime * (num2 - 0.001f), array[k % playgroundParticles.particleCount] / num4);
					}
					break;
				}
				for (int l = 0; l < playgroundParticles.particleCount; l++)
				{
					if (num != playgroundParticles.playgroundCache.lifetimeOffset.Length)
					{
						playgroundParticles.isSettingLifetime = false;
						return;
					}
					playgroundParticles.playgroundCache.lifetimeOffset[l % playgroundParticles.particleCount] = 0f;
				}
				break;
			}
			case SORTINGC.Custom:
			{
				for (int num3 = playgroundParticles.particleCount - 1; num3 >= 0; num3--)
				{
					if (num != playgroundParticles.playgroundCache.lifetimeOffset.Length)
					{
						playgroundParticles.isSettingLifetime = false;
						return;
					}
					playgroundParticles.playgroundCache.lifetimeOffset[num3] = playgroundParticles.lifetime * playgroundParticles.lifetimeEmission * playgroundParticles.lifetimeSorting.Evaluate((float)num3 * 1f / (float)playgroundParticles.particleCount * 1f);
				}
				break;
			}
			}
		}
		SetParticleTimeNow(playgroundParticles);
		SetEmissionRate(playgroundParticles);
		playgroundParticles.previousLifetime = playgroundParticles.lifetime;
		playgroundParticles.previousLifetimeEmission = playgroundParticles.lifetimeEmission;
		playgroundParticles.previousNearestNeighborOrigin = playgroundParticles.nearestNeighborOrigin;
		playgroundParticles.previousNearestNeighborOriginMethod = playgroundParticles.nearestNeighborOriginMethod;
		playgroundParticles.previousNearestNeighborOriginVector3 = playgroundParticles.nearestNeighborOriginVector3;
		playgroundParticles.previousNearestNeighborOriginTransform = playgroundParticles.nearestNeighborOriginTransform;
		playgroundParticles.previousSorting = playgroundParticles.sorting;
		playgroundParticles.isDoneThread = true;
		playgroundParticles.localDeltaTime = 0f;
		playgroundParticles.localTime = PlaygroundC.globalTime;
		playgroundParticles.lastTimeUpdated = PlaygroundC.globalTime;
		playgroundParticles.isSettingLifetime = false;
	}

	public static void SetLifetimeSubtraction(PlaygroundParticlesC playgroundParticles)
	{
		playgroundParticles.playgroundCache.lifetimeSubtraction = new float[playgroundParticles.particleCount];
		System.Random random = playgroundParticles.internalRandom01;
		for (int i = 0; i < playgroundParticles.playgroundCache.lifetimeSubtraction.Length; i++)
		{
			if (playgroundParticles.lifetimeValueMethod == VALUEMETHOD.Constant)
			{
				playgroundParticles.playgroundCache.lifetimeSubtraction[i] = 0f;
			}
			else
			{
				playgroundParticles.playgroundCache.lifetimeSubtraction[i] = RandomRange(random, 0f, playgroundParticles.lifetime - playgroundParticles.lifetimeMin);
			}
		}
		playgroundParticles.previousLifetimeMin = playgroundParticles.lifetimeMin;
		playgroundParticles.previousLifetimeValueMethod = playgroundParticles.lifetimeValueMethod;
	}

	public static void SetEmissionRate(PlaygroundParticlesC playgroundParticles)
	{
		if (playgroundParticles.playgroundCache.emission == null)
		{
			return;
		}
		float num = playgroundParticles.lifetime * playgroundParticles.emissionRate;
		int num2 = playgroundParticles.playgroundCache.emission.Length;
		bool flag = playgroundParticles.source != SOURCEC.Transform && (playgroundParticles.overflowOffset != Vector3.zero || (playgroundParticles.applySourceScatter && (playgroundParticles.sourceScatterMin != Vector3.zero || playgroundParticles.sourceScatterMax != Vector3.zero)));
		for (int i = 0; i < playgroundParticles.playgroundCache.emission.Length; i++)
		{
			if (num2 != playgroundParticles.playgroundCache.emission.Length || playgroundParticles.playgroundCache.lifetimeOffset.Length != num2)
			{
				return;
			}
			if (playgroundParticles.emissionRate != 0f && playgroundParticles.source != SOURCEC.Script)
			{
				if (playgroundParticles.sorting != SORTINGC.Burst || (playgroundParticles.sorting == SORTINGC.NearestNeighbor && flag) || (playgroundParticles.sorting == SORTINGC.NearestNeighborReversed && flag))
				{
					playgroundParticles.playgroundCache.emission[i] = playgroundParticles.playgroundCache.lifetimeOffset[i] >= playgroundParticles.lifetime - num && playgroundParticles.emit;
				}
				else
				{
					playgroundParticles.playgroundCache.emission[i] = playgroundParticles.emit && playgroundParticles.emissionRate > (float)i * 1f / ((float)num2 * 1f);
				}
			}
			else
			{
				playgroundParticles.playgroundCache.emission[i] = false;
			}
			if (playgroundParticles.playgroundCache.emission[i])
			{
				playgroundParticles.playgroundCache.rebirth[i] = true;
				playgroundParticles.playgroundCache.simulate[i] = true;
			}
			else if (playgroundParticles.source == SOURCEC.Script)
			{
				playgroundParticles.playgroundCache.rebirth[i] = false;
			}
		}
		playgroundParticles.previousEmissionRate = playgroundParticles.emissionRate;
		playgroundParticles.hasActiveParticles = true;
		playgroundParticles.threadHadNoActiveParticles = false;
	}

	public static void SetParticleTimeNow(PlaygroundParticlesC playgroundParticles)
	{
		if (playgroundParticles.playgroundCache == null || playgroundParticles.playgroundCache.lifetimeOffset == null || playgroundParticles.playgroundCache.lifetimeOffset.Length != playgroundParticles.particleCount || playgroundParticles.playgroundCache.life == null || playgroundParticles.playgroundCache.life.Length != playgroundParticles.particleCount)
		{
			return;
		}
		playgroundParticles.isSettingParticleTime = true;
		if (playgroundParticles.source != SOURCEC.Script)
		{
			float num = PlaygroundC.globalTime + playgroundParticles.lifetimeOffset;
			if (num <= 0f)
			{
				num = playgroundParticles.localTime;
			}
			for (int i = 0; i < playgroundParticles.particleCount; i++)
			{
				playgroundParticles.playgroundCache.birth[i] = num + playgroundParticles.playgroundCache.lifetimeOffset[i] - playgroundParticles.lifetime;
				playgroundParticles.playgroundCache.death[i] = playgroundParticles.playgroundCache.birth[i] + playgroundParticles.lifetime;
				playgroundParticles.playgroundCache.life[i] = playgroundParticles.playgroundCache.death[i] - playgroundParticles.playgroundCache.lifetimeOffset[i] - playgroundParticles.playgroundCache.birth[i];
				playgroundParticles.particleCache[i].startLifetime = playgroundParticles.lifetime;
				playgroundParticles.particleCache[i].startSize = 0f;
				ref Vector3 reference = ref playgroundParticles.playgroundCache.targetPosition[i];
				reference = PlaygroundC.initialTargetPosition;
				ref Vector3 reference2 = ref playgroundParticles.playgroundCache.previousTargetPosition[i];
				reference2 = PlaygroundC.initialTargetPosition;
				ref Vector3 reference3 = ref playgroundParticles.playgroundCache.position[i];
				reference3 = PlaygroundC.initialTargetPosition;
				playgroundParticles.playgroundCache.simulate[i] = true;
				playgroundParticles.playgroundCache.emission[i] = true;
				playgroundParticles.playgroundCache.isFirstLoop[i] = true;
				playgroundParticles.playgroundCache.isNonBirthed[i] = true;
				playgroundParticles.playgroundCache.isCalculatedThisFrame[i] = false;
				if (!playgroundParticles.isPrewarming)
				{
					playgroundParticles.playgroundCache.rebirth[i] = true;
				}
			}
			playgroundParticles.localTime = num;
			playgroundParticles.lastTimeUpdated = playgroundParticles.localTime;
			playgroundParticles.localDeltaTime = 0f;
			playgroundParticles.t = 0f;
			playgroundParticles.simulationStarted = playgroundParticles.localTime;
			playgroundParticles.cameFromNonEmissionFrame = true;
			playgroundParticles.hasActiveParticles = true;
			playgroundParticles.threadHadNoActiveParticles = false;
		}
		playgroundParticles.isSettingParticleTime = false;
	}

	public static void SetParticleTimeNowWithRestEmission(PlaygroundParticlesC playgroundParticles)
	{
		playgroundParticles.isSettingParticleTime = true;
		float num = playgroundParticles.localTime;
		float num2 = num - playgroundParticles.emissionStopped;
		if (playgroundParticles.loop && num2 < playgroundParticles.lifetime && num2 > 0f)
		{
			playgroundParticles.cameFromNonEmissionFrame = true;
			for (int i = 0; i < playgroundParticles.particleCount; i++)
			{
				playgroundParticles.playgroundCache.birthDelay[i] = num2;
				playgroundParticles.playgroundCache.rebirth[i] = true;
				playgroundParticles.playgroundCache.simulate[i] = true;
				playgroundParticles.playgroundCache.emission[i] = true;
			}
			playgroundParticles.isSettingParticleTime = false;
		}
		else
		{
			SetParticleTimeNow(playgroundParticles);
		}
	}

	public static Color32 GetColorAtLifetime(PlaygroundParticlesC playgroundParticles, float time)
	{
		return playgroundParticles.lifetimeColor.Evaluate(time / playgroundParticles.lifetime);
	}

	public static void SetColorAtLifetime(PlaygroundParticlesC playgroundParticles, float time)
	{
		Color32 color = playgroundParticles.lifetimeColor.Evaluate(time / playgroundParticles.lifetime);
		for (int i = 0; i < playgroundParticles.particleCount; i++)
		{
			playgroundParticles.SetParticleColorInternal(i, color);
		}
		playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache, playgroundParticles.particleCache.Length);
	}

	public static void SetColorWithLifetimeSorting(PlaygroundParticlesC playgroundParticles)
	{
		SetLifetime(playgroundParticles, playgroundParticles.sorting, playgroundParticles.lifetime);
		for (int i = 0; i < playgroundParticles.particleCount; i++)
		{
			Color32 color = playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[i] / playgroundParticles.lifetime);
			playgroundParticles.SetParticleColorInternal(i, color);
		}
		playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache, playgroundParticles.particleCache.Length);
	}

	public bool IsSettingParticleCount()
	{
		return isSettingParticleCount;
	}

	public static void SetParticleCount(PlaygroundParticlesC playgroundParticles, int amount)
	{
		if (playgroundParticles.isSettingParticleCount || playgroundParticles.isPrewarming)
		{
			return;
		}
		if (amount < 0)
		{
			amount = 0;
		}
		if (playgroundParticles.internalRandom01 == null)
		{
			playgroundParticles.RefreshSystemRandom();
		}
		playgroundParticles.particleCount = amount;
		playgroundParticles.previousParticleCount = amount;
		playgroundParticles.particleCache = new ParticleSystem.Particle[amount];
		playgroundParticles.shurikenParticleSystem.Emit(amount);
		playgroundParticles.shurikenParticleSystem.GetParticles(playgroundParticles.particleCache);
		playgroundParticles.collisionCache = null;
		playgroundParticles.inTransition = false;
		playgroundParticles.hasActiveParticles = true;
		playgroundParticles.threadHadNoActiveParticles = false;
		playgroundParticles.isSettingParticleCount = true;
		if (playgroundParticles.multithreadedStartup)
		{
			PlaygroundC.RunAsync(() =>
			{
				SetParticleCountAsyncFriendly(playgroundParticles, amount);
			}, ThreadPoolMethod.ThreadPool);
		}
		else
		{
			SetParticleCountAsyncFriendly(playgroundParticles, amount);
		}
	}

	private static void SetParticleCountAsyncFriendly(PlaygroundParticlesC playgroundParticles, int amount)
	{
		if (!playgroundParticles.isSettingParticleCount)
		{
			return;
		}
		if (playgroundParticles.playgroundCache == null)
		{
			playgroundParticles.playgroundCache = new PlaygroundCache();
		}
		playgroundParticles.playgroundCache.size = new float[amount];
		playgroundParticles.playgroundCache.birth = new float[amount];
		playgroundParticles.playgroundCache.death = new float[amount];
		playgroundParticles.playgroundCache.rebirth = new bool[amount];
		playgroundParticles.playgroundCache.birthDelay = new float[amount];
		playgroundParticles.playgroundCache.life = new float[amount];
		playgroundParticles.playgroundCache.lifetimeSubtraction = new float[amount];
		if (!playgroundParticles.separateRotationAxes)
		{
			playgroundParticles.playgroundCache.rotation = new float[amount];
		}
		else
		{
			playgroundParticles.playgroundCache.rotation3d = new Vector3[amount];
		}
		playgroundParticles.playgroundCache.lifetimeOffset = new float[amount];
		playgroundParticles.playgroundCache.emission = new bool[amount];
		playgroundParticles.playgroundCache.changedByProperty = new bool[amount];
		playgroundParticles.playgroundCache.changedByPropertyColor = new bool[amount];
		playgroundParticles.playgroundCache.changedByPropertyColorLerp = new bool[amount];
		playgroundParticles.playgroundCache.changedByPropertyColorKeepAlpha = new bool[amount];
		playgroundParticles.playgroundCache.changedByPropertySize = new bool[amount];
		playgroundParticles.playgroundCache.changedByPropertyTarget = new bool[amount];
		playgroundParticles.playgroundCache.changedByPropertyDeath = new bool[amount];
		playgroundParticles.playgroundCache.propertyTarget = new int[amount];
		playgroundParticles.playgroundCache.propertyId = new int[amount];
		playgroundParticles.playgroundCache.excludeFromManipulatorId = new int[amount];
		playgroundParticles.playgroundCache.propertyColorId = new int[amount];
		playgroundParticles.playgroundCache.manipulatorId = new int[amount];
		playgroundParticles.playgroundCache.color = new Color32[amount];
		playgroundParticles.playgroundCache.scriptedColor = new Color32[amount];
		playgroundParticles.playgroundCache.initialColor = new Color32[amount];
		playgroundParticles.playgroundCache.lifetimeColorId = new int[amount];
		playgroundParticles.playgroundCache.noForce = new bool[amount];
		playgroundParticles.playgroundCache.position = new Vector3[amount];
		playgroundParticles.playgroundCache.targetPosition = new Vector3[amount];
		playgroundParticles.playgroundCache.targetDirection = new Vector3[amount];
		playgroundParticles.playgroundCache.previousTargetPosition = new Vector3[amount];
		playgroundParticles.playgroundCache.previousParticlePosition = new Vector3[amount];
		playgroundParticles.playgroundCache.collisionParticlePosition = new Vector3[amount];
		playgroundParticles.playgroundCache.localSpaceMovementCompensation = new Vector3[amount];
		playgroundParticles.playgroundCache.scatterPosition = new Vector3[amount];
		playgroundParticles.playgroundCache.velocity = new Vector3[amount];
		playgroundParticles.playgroundCache.isMasked = new bool[amount];
		playgroundParticles.playgroundCache.maskAlpha = new float[amount];
		playgroundParticles.playgroundCache.isNonBirthed = new bool[amount];
		playgroundParticles.playgroundCache.isFirstLoop = new bool[amount];
		playgroundParticles.playgroundCache.simulate = new bool[amount];
		playgroundParticles.playgroundCache.isCalculatedThisFrame = new bool[amount];
		playgroundParticles.playgroundCache.maskSorting = null;
		for (int i = 0; i < amount; i++)
		{
			playgroundParticles.particleCache[i].startSize = 0f;
			playgroundParticles.playgroundCache.rebirth[i] = true;
			playgroundParticles.playgroundCache.simulate[i] = true;
			playgroundParticles.playgroundCache.isNonBirthed[i] = true;
			playgroundParticles.playgroundCache.isFirstLoop[i] = true;
			if (playgroundParticles.colorSource == COLORSOURCEC.LifetimeColors && playgroundParticles.lifetimeColors.Count > 0)
			{
				playgroundParticles.lifetimeColorId++;
				playgroundParticles.lifetimeColorId %= playgroundParticles.lifetimeColors.Count;
				playgroundParticles.playgroundCache.lifetimeColorId[i] = playgroundParticles.lifetimeColorId;
			}
		}
		SetSizeRandom(playgroundParticles, playgroundParticles.sizeMin, playgroundParticles.sizeMax);
		playgroundParticles.previousSizeMin = playgroundParticles.sizeMin;
		playgroundParticles.previousSizeMax = playgroundParticles.sizeMax;
		if (!playgroundParticles.separateRotationAxes)
		{
			playgroundParticles.playgroundCache.initialRotation = RandomFloat(amount, playgroundParticles.initialRotationMin, playgroundParticles.initialRotationMax, playgroundParticles.internalRandom01);
			playgroundParticles.playgroundCache.rotationSpeed = RandomFloat(amount, playgroundParticles.rotationSpeedMin, playgroundParticles.rotationSpeedMax, playgroundParticles.internalRandom01);
		}
		else
		{
			playgroundParticles.playgroundCache.initialRotation3d = RandomVector3(amount, playgroundParticles.initialRotationMin3d, playgroundParticles.initialRotationMax3d, playgroundParticles.internalRandom01);
			playgroundParticles.playgroundCache.rotationSpeed3d = RandomVector3(amount, playgroundParticles.rotationSpeedMin3d, playgroundParticles.rotationSpeedMax3d, playgroundParticles.internalRandom01);
		}
		playgroundParticles.previousInitialRotationMin = playgroundParticles.initialRotationMin;
		playgroundParticles.previousInitialRotationMax = playgroundParticles.initialRotationMax;
		playgroundParticles.previousRotationSpeedMin = playgroundParticles.rotationSpeedMin;
		playgroundParticles.previousRotationSpeedMax = playgroundParticles.rotationSpeedMax;
		playgroundParticles.previousInitialRotationMin3d = playgroundParticles.initialRotationMin3d;
		playgroundParticles.previousInitialRotationMax3d = playgroundParticles.initialRotationMax3d;
		playgroundParticles.previousRotationSpeedMin3d = playgroundParticles.rotationSpeedMin3d;
		playgroundParticles.previousRotationSpeedMax3d = playgroundParticles.rotationSpeedMax3d;
		SetVelocityRandom(playgroundParticles, playgroundParticles.initialVelocityMin, playgroundParticles.initialVelocityMax);
		SetLocalVelocityRandom(playgroundParticles, playgroundParticles.initialLocalVelocityMin, playgroundParticles.initialLocalVelocityMax);
		Emission(playgroundParticles, playgroundParticles.emit, callRestEmission: false);
		if (playgroundParticles.applySourceScatter)
		{
			playgroundParticles.RefreshScatter();
		}
		playgroundParticles.isDoneThread = true;
		playgroundParticles.isSettingParticleCount = false;
	}

	public bool UpdateSystem()
	{
		if (isYieldRefreshing || isLoading || playgroundCache == null || thisInstance == null || !particleSystemGameObject.activeInHierarchy || !particleSystemGameObject.activeSelf || !enabled || isSettingLifetime || isSettingParticleCount || isPrewarming || inPlayback)
		{
			return false;
		}
		if (Application.isPlaying && queueEmissionHalt && !IsAlive() && !loop && loopExceeded && hasEmitted)
		{
			if (disableOnDoneRoutine == ONDONE.Inactivate)
			{
				particleSystemGameObject.SetActive(value: false);
				return false;
			}
			UnityEngine.Object.DestroyImmediate(particleSystemGameObject);
			return false;
		}
		if (emit)
		{
			hasEmitted = true;
		}
		if (source != prevSource)
		{
			prevSource = source;
			return false;
		}
		if (particleCount != previousParticleCount)
		{
			SetParticleCount(thisInstance, particleCount);
			isDoneThread = true;
			if (particleCount > 0)
			{
				StartCoroutine(Boot());
			}
			return false;
		}
		if (emit != previousEmission)
		{
			Emit(emit);
			return false;
		}
		if (loop != previousLoop)
		{
			if (emit && loop)
			{
				Emit(setEmission: true);
			}
			previousLoop = loop;
			return false;
		}
		if (sizeMin != previousSizeMin || sizeMax != previousSizeMax)
		{
			SetSizeRandom(thisInstance, sizeMin, sizeMax);
		}
		bool flag = separateRotationAxes != previousSeparateRotationAxes;
		if (separateRotationAxes)
		{
			if (initialRotationMin3d != previousInitialRotationMin3d || initialRotationMax3d != previousInitialRotationMax3d || flag)
			{
				SetInitialRotationRandom(thisInstance, initialRotationMin3d, initialRotationMax3d);
			}
			if (rotationSpeedMin3d != previousRotationSpeedMin3d || rotationSpeedMax3d != previousRotationSpeedMax3d || flag)
			{
				SetRotationRandom(thisInstance, rotationSpeedMin3d, rotationSpeedMax3d);
			}
		}
		else
		{
			if (initialRotationMin != previousInitialRotationMin || initialRotationMax != previousInitialRotationMax || flag)
			{
				SetInitialRotationRandom(thisInstance, initialRotationMin, initialRotationMax);
			}
			if (rotationSpeedMin != previousRotationSpeedMin || rotationSpeedMax != previousRotationSpeedMax || flag)
			{
				SetRotationRandom(thisInstance, rotationSpeedMin, rotationSpeedMax);
			}
		}
		if (flag)
		{
			ParticleSystem.RotationOverLifetimeModule rotationOverLifetime = shurikenParticleSystem.rotationOverLifetime;
			rotationOverLifetime.separateAxes = separateRotationAxes;
			previousSeparateRotationAxes = separateRotationAxes;
		}
		if (applyInitialVelocity && (initialVelocityMin != previousVelocityMin || initialVelocityMax != previousVelocityMax || initialVelocityMethod != previousVelocityMethod || playgroundCache.initialVelocity == null || playgroundCache.initialVelocity.Length != particleCount))
		{
			SetVelocityRandom(thisInstance, initialVelocityMin, initialVelocityMax);
			return false;
		}
		if (applyInitialLocalVelocity && (initialLocalVelocityMin != previousLocalVelocityMin || initialLocalVelocityMax != previousLocalVelocityMax || initialLocalVelocityMethod != previousLocalVelocityMethod || playgroundCache.initialLocalVelocity == null || playgroundCache.initialLocalVelocity.Length != particleCount))
		{
			SetLocalVelocityRandom(thisInstance, initialLocalVelocityMin, initialLocalVelocityMax);
			return false;
		}
		if (previousLifetime != lifetime || (lifetimeValueMethod == VALUEMETHOD.Constant && previousLifetimeValueMethod != lifetimeValueMethod))
		{
			Start();
			return false;
		}
		if (previousNearestNeighborOriginMethod != nearestNeighborOriginMethod || ((sorting == SORTINGC.NearestNeighbor || sorting == SORTINGC.NearestNeighborReversed) && ((nearestNeighborOriginMethod == NEARESTNEIGHBORORIGINMETHOD.SourcePoint && previousNearestNeighborOrigin != nearestNeighborOrigin) || (nearestNeighborOriginMethod == NEARESTNEIGHBORORIGINMETHOD.Vector3 && previousNearestNeighborOriginVector3 != nearestNeighborOriginVector3) || (nearestNeighborOriginMethod == NEARESTNEIGHBORORIGINMETHOD.Transform && previousNearestNeighborOriginTransform != nearestNeighborOriginTransform))) || previousSorting != sorting || previousLifetimeEmission != lifetimeEmission)
		{
			Start();
			return false;
		}
		if (lifetimeValueMethod == VALUEMETHOD.RandomBetweenTwoValues && (previousLifetimeMin != lifetimeMin || previousLifetime != lifetime || previousLifetimeValueMethod != lifetimeValueMethod))
		{
			SetLifetimeSubtraction(thisInstance);
			previousLifetimeMin = lifetimeMin;
		}
		if (previousEmissionRate != emissionRate && source != SOURCEC.Script)
		{
			SetEmissionRate(thisInstance);
		}
		if (source == SOURCEC.State && activeState != previousActiveState)
		{
			if (states[activeState].positionLength > particleCount)
			{
				SetParticleCount(thisInstance, states[activeState].positionLength);
			}
			previousActiveState = activeState;
		}
		return true;
	}

	public void UpdateShuriken()
	{
		if (!isReadyForThreadedCalculations || !calculate || inPlayback || (source == SOURCEC.Spline && splines.Count == 0))
		{
			return;
		}
		if (collision)
		{
			Collisions(thisInstance);
		}
		if (!particleSystemRenderer.enabled)
		{
			return;
		}
		if (syncPositionsOnMainThread || collision || (forceSkinnedMeshUpdateOnMainThread && source == SOURCEC.SkinnedWorldObject && (onlySourcePositioning || onlyLifetimePositioning)))
		{
			if (particleCache.Length != playgroundCache.position.Length || particleCount != playgroundCache.isCalculatedThisFrame.Length)
			{
				return;
			}
			if (playgroundCache.life.Length == particleCount && playgroundCache.simulate.Length == particleCount)
			{
				bool flag = onlySourcePositioning && !cancelDeltaPositioningOnSync;
				float num = ((!(minShurikenLifetime < 1E-05f)) ? minShurikenLifetime : 1E-05f);
				float y = PlaygroundC.initialTargetPosition.y;
				for (int i = 0; i < particleCount; i++)
				{
					if (!playgroundCache.isCalculatedThisFrame[i])
					{
						continue;
					}
					if (flag || (playgroundCache.life[i] == 0f && playgroundCache.simulate[i] && !onlyLifetimePositioning))
					{
						particleCache[i].remainingLifetime = lifetime;
						ref Vector3 reference = ref playgroundCache.previousTargetPosition[i];
						reference = playgroundCache.targetPosition[i];
						SetSourcePosition(i);
						ref Vector3 reference2 = ref playgroundCache.position[i];
						reference2 = playgroundCache.targetPosition[i];
					}
					particleCache[i].position = playgroundCache.position[i];
					if (playgroundCache.life[i] != 0f)
					{
						float num2 = playgroundCache.death[i] - localTime;
						if (num2 < num)
						{
							num2 = num;
						}
						particleCache[i].remainingLifetime = num2;
					}
					particleCache[i].startSize = ((!(playgroundCache.maskAlpha[i] > 0f) || particleCache[i].position.y == y) ? 0f : playgroundCache.size[i]);
					if (separateRotationAxes)
					{
						particleCache[i].rotation3D = playgroundCache.rotation3d[i];
					}
					else
					{
						particleCache[i].rotation = playgroundCache.rotation[i];
					}
				}
			}
		}
		if (!inTransition && particleCache != null && particleCache.Length > 0 && calculate && hasActiveParticles && !isPrewarming)
		{
			shurikenParticleSystem.SetParticles(particleCache, particleCache.Length);
		}
		else if (particleCache.Length == 0 && hasActiveParticles)
		{
			shurikenParticleSystem.Clear();
			shurikenParticleSystem.Stop();
			hasActiveParticles = false;
			isDoneThread = true;
		}
		if (hasActiveParticles && calculate && (shurikenParticleSystem.isPaused || shurikenParticleSystem.isStopped))
		{
			shurikenParticleSystem.Play();
		}
	}

	public static void SetInitialTargetPosition(PlaygroundParticlesC playgroundParticles, Vector3 position, bool refreshParticleSystem)
	{
		for (int i = 0; i < playgroundParticles.particleCache.Length; i++)
		{
			playgroundParticles.playgroundCache.previousTargetPosition[i] = position;
			playgroundParticles.playgroundCache.targetPosition[i] = position;
			playgroundParticles.particleCache[i].startSize = 0f;
			playgroundParticles.particleCache[i].position = position;
			playgroundParticles.playgroundCache.position[i] = position;
			playgroundParticles.playgroundCache.collisionParticlePosition[i] = position;
		}
		if (refreshParticleSystem)
		{
			playgroundParticles.GetComponent<ParticleSystem>().SetParticles(playgroundParticles.particleCache, playgroundParticles.particleCache.Length);
		}
	}

	public static void Emission(PlaygroundParticlesC playgroundParticles, bool emission, bool callRestEmission)
	{
		playgroundParticles.previousEmission = emission;
		if (!emission)
		{
			return;
		}
		PlaygroundC.RunAsync(() =>
		{
			for (int i = 0; i < playgroundParticles.playgroundCache.rebirth.Length; i++)
			{
				playgroundParticles.playgroundCache.rebirth[i] = true;
				if (!callRestEmission)
				{
					playgroundParticles.playgroundCache.simulate[i] = true;
				}
			}
			if (callRestEmission)
			{
				SetParticleTimeNowWithRestEmission(playgroundParticles);
			}
			playgroundParticles.emissionStopped = 0f;
			playgroundParticles.hasActiveParticles = true;
			playgroundParticles.threadHadNoActiveParticles = false;
		});
	}

	public static float SignedAngle(Vector3 a, Vector3 b, Vector3 n)
	{
		return Vector3.Angle(a, b) * Mathf.Sign(Vector3.Dot(n.normalized, Vector3.Cross(a, b)));
	}

	public bool PrepareThreadedCalculations()
	{
		if (!enabled)
		{
			return false;
		}
		if (inPlayback)
		{
			return false;
		}
		if (pauseCalculationWhenInvisible)
		{
			if (0 == 0 && Camera.main != null && PlaygroundC.frustumPlanes != null)
			{
				bool flag = GeometryUtility.TestPlanesAABB(PlaygroundC.frustumPlanes, new Bounds((!(calculationTriggerTransform == null)) ? (calculationTriggerTransform.position + calculationTriggerOffset) : (particleSystemTransform.position + calculationTriggerOffset), calculationTriggerSize));
				if (!flag && calculate && !particleSystemRenderer.isVisible)
				{
					calculate = false;
					return false;
				}
				if (flag && !calculate)
				{
					calculate = true;
					if (prewarm)
					{
						Start();
						return false;
					}
					SetParticleTimeNow(thisInstance);
				}
			}
		}
		else if (!calculate)
		{
			calculate = true;
			if (prewarm)
			{
				Start();
				return false;
			}
			SetParticleTimeNow(thisInstance);
		}
		if (!calculate)
		{
			return false;
		}
		if (cameFromNonCalculatedFrame && hasActiveParticles)
		{
			StartCoroutine(Boot());
			cameFromNonCalculatedFrame = false;
			return false;
		}
		if (applyLockPosition)
		{
			if (lockPositionIsLocal)
			{
				particleSystemTransform.localPosition = lockPosition;
			}
			else
			{
				particleSystemTransform.position = lockPosition;
			}
		}
		if (applyLockRotation)
		{
			if (lockRotationIsLocal)
			{
				particleSystemTransform.localRotation = Quaternion.Euler(lockRotation);
			}
			else
			{
				particleSystemTransform.rotation = Quaternion.Euler(lockRotation);
			}
		}
		if (applyLockScale)
		{
			particleSystemTransform.localScale = lockScale;
		}
		hasActiveParticles = !threadHadNoActiveParticles || (emit && loop && particleCount > 0);
		if (!hasActiveParticles)
		{
			loopExceeded = true;
			if (disableOnDone)
			{
				queueEmissionHalt = true;
			}
		}
		realSimulationTime = particleTimescale * Time.timeScale;
		cancelDeltaPositioningOnSync = false;
		if (PlaygroundC.reference.IsDoneThread() && isDoneThread)
		{
			stRot = Quaternion.identity;
			stDir = default;
		}
		localSpace = shurikenMainModule.simulationSpace == ParticleSystemSimulationSpace.Local;
		overflow = overflowOffset != Vector3.zero;
		skinnedWorldObjectReady = false;
		renderModeStretch = particleSystemRenderer2.renderMode == ParticleSystemRenderMode.Stretch;
		particleSystemRotation = particleSystemTransform.rotation;
		particleSystemInverseRotation = particleSystemRotation;
		particleSystemInverseRotation.x = 0f - particleSystemInverseRotation.x;
		particleSystemInverseRotation.y = 0f - particleSystemInverseRotation.y;
		particleSystemInverseRotation.z = 0f - particleSystemInverseRotation.z;
		particleSystemInverseRotation.w = 0f - particleSystemInverseRotation.w;
		if (!onlySourcePositioning && !onlyLifetimePositioning && turbulenceType != TURBULENCETYPE.None && turbulenceType == TURBULENCETYPE.Simplex && turbulenceSimplex == null)
		{
			turbulenceSimplex = new SimplexNoise();
		}
		if (emit)
		{
			switch (source)
			{
			case SOURCEC.State:
				if (states.Count > 0)
				{
					if (activeState > states.Count - 1)
					{
						activeState = states.Count - 1;
					}
					if (activeState < 0)
					{
						activeState = 0;
					}
					if (!(states[activeState].stateTransform != null))
					{
						break;
					}
					if (isDoneThread)
					{
						if (!states[activeState].initialized)
						{
							states[activeState].Initialize();
						}
						stRot = states[activeState].stateTransform.rotation;
						states[activeState].UpdateMatrix(localSpace);
					}
					if (localSpace && (states[activeState].stateTransform.parent == particleSystemTransform || states[activeState].stateTransform == particleSystemTransform) && isDoneThread)
					{
						cancelDeltaPositioningOnSync = true;
						stRot = Quaternion.Euler(Vector3.zero);
					}
					break;
				}
				return false;
			case SOURCEC.Transform:
				if (sourceTransforms == null)
				{
					sourceTransforms = new List<PlaygroundTransformC>();
				}
				if (sourceTransforms.Count == 0)
				{
					sourceTransforms.Add(new PlaygroundTransformC());
					sourceTransforms[0].transform = ((!(sourceTransform != null)) ? particleSystemTransform : sourceTransform);
				}
				if (isDoneThread)
				{
					for (int k = 0; k < sourceTransforms.Count; k++)
					{
						if (!sourceTransforms[k].Update())
						{
							return false;
						}
						if (IsLocalSpace())
						{
							psTransformNum = -1;
							sourceTransforms[k].SetLocalPosition(particleSystemTransform);
							sourceTransforms[k].SetPostitionAsLocal();
							if (sourceTransforms[k].IsSameAs(particleSystemTransform))
							{
								sourceTransforms[k].SetZeroRotation();
								psTransformNum = k;
							}
						}
					}
				}
				if (isDoneThread)
				{
					stRot = sourceTransform.rotation;
				}
				if (localSpace && sourceTransform == particleSystemTransform && isDoneThread)
				{
					cancelDeltaPositioningOnSync = true;
					stRot = Quaternion.Euler(Vector3.zero);
				}
				break;
			case SOURCEC.WorldObject:
				if (worldObject.gameObject != null)
				{
					if (worldObject.gameObject.GetInstanceID() != worldObject.cachedId || !worldObject.initialized)
					{
						worldObject = NewWorldObject(worldObject.gameObject.transform);
					}
					if (worldObject.mesh != null)
					{
						if (isDoneThread)
						{
							stRot = worldObject.transform.transform.rotation;
						}
						if (localSpace && isDoneThread)
						{
							cancelDeltaPositioningOnSync = true;
							stRot = Quaternion.Euler(Vector3.zero);
						}
						worldObject.UpdateMatrix(localSpace);
						worldObject.updateNormals = worldObjectUpdateNormals;
						if (worldObjectUpdateVertices)
						{
							worldObject.Update();
						}
						break;
					}
					return false;
				}
				return false;
			case SOURCEC.SkinnedWorldObject:
				if (skinnedWorldObject.gameObject != null && skinnedWorldObject.gameObject.GetInstanceID() != skinnedWorldObject.cachedId)
				{
					skinnedWorldObject = NewSkinnedWorldObject(skinnedWorldObject.gameObject.transform, skinnedWorldObject.downResolution);
				}
				skinnedWorldObjectReady = skinnedWorldObject.gameObject != null && skinnedWorldObject.mesh != null;
				if (PlaygroundC.reference.skinnedMeshThreadMethod == ThreadMethodComponent.OneForAll && PlaygroundC.reference.IsFirstUnsafeAutomaticFrames() && !PlaygroundC.reference.IsDoneThreadSkinnedMeshes())
				{
					return false;
				}
				if (skinnedWorldObjectReady)
				{
					if (isDoneThread)
					{
						stRot = skinnedWorldObject.transform.rotation;
						stDir = skinnedWorldObject.transform.TransformDirection(overflowOffset);
					}
					skinnedWorldObject.updateNormals = worldObjectUpdateNormals;
					if (Time.frameCount % PlaygroundC.skinnedUpdateRate == 0)
					{
						if (worldObjectUpdateVertices)
						{
							skinnedWorldObject.MeshUpdate();
						}
						skinnedWorldObject.BoneUpdate();
						if (forceSkinnedMeshUpdateOnMainThread)
						{
							skinnedWorldObject.Update();
						}
					}
					break;
				}
				return false;
			case SOURCEC.Paint:
				if (paint.initialized)
				{
					if (isDoneThread)
					{
						stRot = particleSystemTransform.rotation;
					}
					if (paint.positionLength > 0)
					{
						for (int j = 0; j < particleCache.Length; j++)
						{
							paint.Update(j);
						}
						break;
					}
					return false;
				}
				paint.Initialize();
				return false;
			case SOURCEC.Projection:
				if (projection.projectionTexture != null && projection.projectionTransform != null)
				{
					if (!projection.initialized)
					{
						projection.Initialize();
					}
					stRot = projection.projectionTransform.rotation;
					if (localSpace)
					{
						shurikenMainModule.simulationSpace = ParticleSystemSimulationSpace.World;
					}
					if (projection.liveUpdate || !projection.hasRefreshed)
					{
						projection.UpdateSource();
						projection.Update();
						stDir = projection.projectionTransform.TransformDirection(overflowOffset);
						projection.hasRefreshed = true;
					}
					break;
				}
				return false;
			case SOURCEC.Spline:
				if (splines.Count > 0)
				{
					for (int i = 0; i < splines.Count; i++)
					{
						if (splines[i] == null || !splines[i].enabled || !splines[i].IsReady())
						{
							return false;
						}
					}
					if (isDoneThread)
					{
						stRot = particleSystemTransform.rotation;
						if (localSpace)
						{
							cancelDeltaPositioningOnSync = true;
							stRot = Quaternion.Euler(Vector3.zero);
						}
					}
					break;
				}
				return false;
			}
		}
		stateReadyForTextureColor = source == SOURCEC.State && states[activeState].stateTexture != null && states[activeState].colorLength > 0;
		hasEventManipulatorLocal = false;
		hasEventManipulatorGlobal = false;
		hasGlobalAffectingManipulators = false;
		manipulatorEventCount = 0;
		thisLayer = particleSystemGameObject.layer;
		for (int l = 0; l < manipulators.Count; l++)
		{
			manipulators[l].Update();
			manipulators[l].transform.SetLocalPosition(particleSystemTransform);
			manipulators[l].SetLocalTargetsPosition(particleSystemTransform);
			if (manipulators[l].trackParticles)
			{
				hasEventManipulatorLocal = true;
				manipulatorEventCount++;
			}
		}
		if (PlaygroundC.reference.HasEnabledGlobalManipulators())
		{
			for (int m = 0; m < PlaygroundC.reference.manipulators.Count; m++)
			{
				if ((PlaygroundC.reference.manipulators[m].affects.value & (1 << GetLayer())) != 0)
				{
					hasGlobalAffectingManipulators = true;
					PlaygroundC.reference.manipulators[m].transform.SetLocalPosition(particleSystemTransform);
					if (PlaygroundC.reference.manipulators[m].trackParticles && PlaygroundC.reference.threadMethod != ThreadMethod.NoThreads)
					{
						threadMethod = ThreadMethodLocal.OneForAll;
						hasEventManipulatorGlobal = true;
						manipulatorEventCount++;
					}
				}
			}
		}
		hasSeveralManipulatorEvents = manipulatorEventCount > 1;
		hasEvent = events.Count > 0;
		hasTimerEvent = false;
		if (hasEvent)
		{
			for (int n = 0; n < events.Count; n++)
			{
				events[n].Initialize();
				if (events[n].initializedTarget && events[n].broadcastType != EVENTBROADCASTC.EventListeners && !events[n].target.eventControlledBy.Contains(thisInstance))
				{
					events[n].target.eventControlledBy.Add(thisInstance);
				}
				if (events[n].eventType == EVENTTYPEC.Time && ((events[n].broadcastType != EVENTBROADCASTC.EventListeners && events[n].initializedTarget) || (events[n].broadcastType == EVENTBROADCASTC.EventListeners && events[n].initializedEvent)))
				{
					hasTimerEvent = events[n].UpdateTime();
				}
			}
		}
		hasCollisionCache = collisionCache != null;
		return true;
	}

	private void ThreadedDelegate()
	{
		if (!isDoneThread && isReadyForThreadedCalculations)
		{
			ThreadedCalculations(thisInstance);
		}
	}

	public static void NewCalculatedThread(PlaygroundParticlesC playgroundParticles)
	{
		if (playgroundParticles._particleCalculationAction == null)
		{
			playgroundParticles._particleCalculationAction = playgroundParticles.ThreadedDelegate;
		}
		if (playgroundParticles.isDoneThread && playgroundParticles.isReadyForThreadedCalculations && !playgroundParticles.IsYieldRefreshing())
		{
			playgroundParticles.isDoneThread = false;
			PlaygroundC.RunAsync(playgroundParticles._particleCalculationAction);
		}
	}

	public static void NewCalculatedThread(PlaygroundParticlesC[] playgroundParticles)
	{
		for (int i = 0; i < playgroundParticles.Length; i++)
		{
			if (playgroundParticles[i]._particleCalculationAction == null)
			{
				playgroundParticles[i]._particleCalculationAction = playgroundParticles[i].ThreadedDelegate;
			}
			if (playgroundParticles[i].isDoneThread && !playgroundParticles[i].IsYieldRefreshing() && playgroundParticles[i].threadMethod == ThreadMethodLocal.Inherit)
			{
				playgroundParticles[i].isDoneThread = false;
			}
			if (!playgroundParticles[i].isDoneThread && PlaygroundC.reference.IsDoneThread() && playgroundParticles[i].isReadyForThreadedCalculations && playgroundParticles[i].threadMethod == ThreadMethodLocal.Inherit)
			{
				PlaygroundC.RunAsync(playgroundParticles[i]._particleCalculationAction);
			}
		}
	}

	private Color32 GetParticleColor(int p, float normalizedLife, float normalizedP)
	{
		Color32 result = default;
		if (!playgroundCache.changedByPropertyColor[p] && !playgroundCache.changedByPropertyColorLerp[p])
		{
			switch (colorSource)
			{
			case COLORSOURCEC.Source:
				result = source switch
				{
					SOURCEC.Script => playgroundCache.scriptedColor[p], 
					SOURCEC.State => (!stateReadyForTextureColor) ? ((Color32)lifetimeColor.Evaluate((colorMethod != COLORMETHOD.Lifetime) ? normalizedP : normalizedLife)) : states[activeState].GetColor(p % states[activeState].colorLength), 
					SOURCEC.Projection => projection.GetColor(p), 
					SOURCEC.Paint => paint.GetColor(p), 
					_ => (Color32)lifetimeColor.Evaluate((colorMethod != COLORMETHOD.Lifetime) ? normalizedP : normalizedLife), 
				};
				if (sourceUsesLifetimeAlpha)
				{
					result.a = (byte)(255f * Mathf.Clamp(lifetimeColor.Evaluate((colorMethod != COLORMETHOD.Lifetime) ? normalizedP : normalizedLife).a, 0f, (int)result.a));
				}
				break;
			case COLORSOURCEC.LifetimeColor:
				result = lifetimeColor.Evaluate((colorMethod != COLORMETHOD.Lifetime) ? normalizedP : normalizedLife);
				break;
			case COLORSOURCEC.LifetimeColors:
				result = ((lifetimeColors.Count <= 0) ? ((Color32)lifetimeColor.Evaluate((colorMethod != COLORMETHOD.Lifetime) ? normalizedP : normalizedLife)) : ((Color32)lifetimeColors[playgroundCache.lifetimeColorId[p] % lifetimeColors.Count].gradient.Evaluate((colorMethod != COLORMETHOD.Lifetime) ? normalizedP : normalizedLife)));
				break;
			}
		}
		else
		{
			result = GetParticleColorInternal(p);
			if (playgroundCache.changedByPropertyColorKeepAlpha[p])
			{
				result.a = (byte)(255f * Mathf.Clamp(lifetimeColor.Evaluate((colorMethod != COLORMETHOD.Lifetime) ? normalizedP : normalizedLife).a, 0f, (int)result.a));
			}
		}
		if (colorMethod == COLORMETHOD.ParticleArray && arrayColorUsesAlpha)
		{
			result.a = (byte)(255f * arrayColorAlpha.Evaluate(normalizedLife).a);
		}
		if (applyParticleMask)
		{
			result.a = (byte)Mathf.Lerp(0f, (int)result.a, playgroundCache.maskAlpha[p]);
		}
		return result;
	}

	private void SetSourcePosition(int p)
	{
		switch (source)
		{
		case SOURCEC.State:
			if (!overflow)
			{
				ref Vector3 reference14 = ref playgroundCache.targetPosition[p];
				reference14 = states[activeState].GetLocalPosition(p);
				break;
			}
			switch (overflowMode)
			{
			case OVERFLOWMODEC.SourceTransform:
			{
				ref Vector3 reference17 = ref playgroundCache.targetPosition[p];
				reference17 = states[activeState].GetLocalPosition(p) + GetOverflow2(overflowOffset, p, states[activeState].positionLength);
				break;
			}
			case OVERFLOWMODEC.World:
			{
				ref Vector3 reference16 = ref playgroundCache.targetPosition[p];
				reference16 = states[activeState].GetLocalPosition(p) + GetOverflow2(overflowOffset, p, states[activeState].positionLength);
				break;
			}
			case OVERFLOWMODEC.SourcePoint:
			{
				ref Vector3 reference15 = ref playgroundCache.targetPosition[p];
				reference15 = states[activeState].GetLocalPosition(p) + GetOverflow2(overflowOffset, states[activeState].GetLocalNormal(p), p, states[activeState].positionLength);
				break;
			}
			}
			break;
		case SOURCEC.Transform:
			if (sourceTransforms.Count <= 0)
			{
				break;
			}
			if (treatAsOneTransform)
			{
				transformIndex = (int)((float)p * 1f / ((float)particleCount * 1f) * (float)sourceTransforms.Count) % sourceTransforms.Count;
			}
			else
			{
				transformIndex = p % sourceTransforms.Count;
			}
			if (transformIndex >= sourceTransforms.Count)
			{
				transformIndex = sourceTransforms.Count - 1;
			}
			if (!overflow)
			{
				ref Vector3 reference6 = ref playgroundCache.targetPosition[p];
				reference6 = sourceTransforms[transformIndex].position;
				break;
			}
			switch (overflowMode)
			{
			case OVERFLOWMODEC.SourceTransform:
			{
				ref Vector3 reference9 = ref playgroundCache.targetPosition[p];
				reference9 = sourceTransforms[transformIndex].transformMatrix.MultiplyPoint3x4(GetOverflow2(overflowOffset, (!treatAsOneTransform) ? (p / sourceTransforms.Count) : (p % (particleCount / sourceTransforms.Count)), 1));
				break;
			}
			case OVERFLOWMODEC.World:
			{
				ref Vector3 reference8 = ref playgroundCache.targetPosition[p];
				reference8 = sourceTransforms[transformIndex].position + GetOverflow2(overflowOffset, (!treatAsOneTransform) ? (p / sourceTransforms.Count) : (p % (particleCount / sourceTransforms.Count)), 1);
				break;
			}
			case OVERFLOWMODEC.SourcePoint:
			{
				ref Vector3 reference7 = ref playgroundCache.targetPosition[p];
				reference7 = sourceTransforms[transformIndex].transformMatrix.MultiplyPoint3x4(GetOverflow2(overflowOffset, (!treatAsOneTransform) ? (p / sourceTransforms.Count) : (p % (particleCount / sourceTransforms.Count)), 1));
				break;
			}
			}
			break;
		case SOURCEC.WorldObject:
			if (!overflow)
			{
				ref Vector3 reference18 = ref playgroundCache.targetPosition[p];
				reference18 = worldObject.transformMatrix.MultiplyPoint3x4(worldObject.vertexPositions[p % worldObject.vertexPositions.Length]);
				break;
			}
			switch (overflowMode)
			{
			case OVERFLOWMODEC.SourceTransform:
			{
				ref Vector3 reference21 = ref playgroundCache.targetPosition[p];
				reference21 = worldObject.transformMatrix.MultiplyPoint3x4(worldObject.vertexPositions[p % worldObject.vertexPositions.Length] + GetOverflow2(overflowOffset, p, worldObject.vertexPositions.Length));
				break;
			}
			case OVERFLOWMODEC.World:
			{
				ref Vector3 reference20 = ref playgroundCache.targetPosition[p];
				reference20 = worldObject.transformMatrix.MultiplyPoint3x4(worldObject.vertexPositions[p % worldObject.vertexPositions.Length]) + GetOverflow2(overflowOffset, p, worldObject.vertexPositions.Length);
				break;
			}
			case OVERFLOWMODEC.SourcePoint:
			{
				ref Vector3 reference19 = ref playgroundCache.targetPosition[p];
				reference19 = worldObject.transformMatrix.MultiplyPoint3x4(worldObject.vertexPositions[p % worldObject.vertexPositions.Length] + GetOverflow2(overflowOffset, worldObject.normals[p % worldObject.normals.Length], p, worldObject.vertexPositions.Length));
				break;
			}
			}
			break;
		case SOURCEC.SkinnedWorldObject:
		{
			int downResolution = skinnedWorldObject.downResolution;
			int num2 = Mathf.RoundToInt(p * downResolution) % skinnedWorldObject.vertexPositions.Length;
			if (!overflow)
			{
				ref Vector3 reference2 = ref playgroundCache.targetPosition[p];
				reference2 = skinnedWorldObject.vertexPositions[num2];
				break;
			}
			switch (overflowMode)
			{
			case OVERFLOWMODEC.SourceTransform:
			{
				ref Vector3 reference5 = ref playgroundCache.targetPosition[p];
				reference5 = skinnedWorldObject.vertexPositions[num2] + GetOverflow2(stDir, p, skinnedWorldObject.vertexPositions.Length / downResolution);
				break;
			}
			case OVERFLOWMODEC.World:
			{
				ref Vector3 reference4 = ref playgroundCache.targetPosition[p];
				reference4 = skinnedWorldObject.vertexPositions[num2] + GetOverflow2(overflowOffset, p, skinnedWorldObject.vertexPositions.Length / downResolution);
				break;
			}
			case OVERFLOWMODEC.SourcePoint:
			{
				ref Vector3 reference3 = ref playgroundCache.targetPosition[p];
				reference3 = skinnedWorldObject.vertexPositions[num2] + GetOverflow2(overflowOffset, skinnedWorldObject.normals[num2], p, skinnedWorldObject.vertexPositions.Length / downResolution);
				break;
			}
			}
			break;
		}
		case SOURCEC.Projection:
			if (!overflow)
			{
				ref Vector3 reference22 = ref playgroundCache.targetPosition[p];
				reference22 = projection.GetPosition(p);
				break;
			}
			switch (overflowMode)
			{
			case OVERFLOWMODEC.SourceTransform:
			{
				ref Vector3 reference25 = ref playgroundCache.targetPosition[p];
				reference25 = projection.GetPosition(p) + GetOverflow2(stDir, p, projection.positionLength);
				break;
			}
			case OVERFLOWMODEC.World:
			{
				ref Vector3 reference24 = ref playgroundCache.targetPosition[p];
				reference24 = projection.GetPosition(p) + GetOverflow2(overflowOffset, p, projection.positionLength);
				break;
			}
			case OVERFLOWMODEC.SourcePoint:
			{
				ref Vector3 reference23 = ref playgroundCache.targetPosition[p];
				reference23 = projection.GetPosition(p) + GetOverflow2(Vector3Scale(stDir, projection.GetNormal(p)), p, projection.positionLength);
				break;
			}
			}
			break;
		case SOURCEC.Paint:
			if (!overflow)
			{
				ref Vector3 reference10 = ref playgroundCache.targetPosition[p];
				reference10 = paint.GetPosition(p);
				break;
			}
			switch (overflowMode)
			{
			case OVERFLOWMODEC.SourceTransform:
			{
				ref Vector3 reference13 = ref playgroundCache.targetPosition[p];
				reference13 = paint.GetPosition(p) + GetOverflow2(paint.GetRotation(p) * overflowOffset, p, paint.positionLength);
				break;
			}
			case OVERFLOWMODEC.World:
			{
				ref Vector3 reference12 = ref playgroundCache.targetPosition[p];
				reference12 = paint.GetPosition(p) + GetOverflow2(overflowOffset, p, paint.positionLength);
				break;
			}
			case OVERFLOWMODEC.SourcePoint:
			{
				ref Vector3 reference11 = ref playgroundCache.targetPosition[p];
				reference11 = paint.GetPosition(p) + paint.GetRotation(p) * GetOverflow2(overflowOffset, paint.GetNormal(p), p, paint.positionLength);
				break;
			}
			}
			break;
		case SOURCEC.Spline:
		{
			int index = ((!treatAsOneSpline) ? (p % splines.Count) : ((int)((float)p * 1f / ((float)particleCount * 1f) * (float)splines.Count + splineTimeOffset) % splines.Count));
			float num = ((!treatAsOneSpline) ? ((float)p * 1f / ((float)particleCount * 1f) + splineTimeOffset) : ((float)(p * splines.Count) * 1f / ((float)particleCount * 1f) + splineTimeOffset));
			ref Vector3 reference = ref playgroundCache.targetPosition[p];
			reference = splines[index].GetPoint(num);
			break;
		}
		}
		if (applySourceScatter)
		{
			playgroundCache.targetPosition[p] += Vector3.Scale(playgroundCache.scatterPosition[p], scatterScale);
		}
	}

	public static void ThreadedCalculations(PlaygroundParticlesC playgroundParticles)
	{
		if (playgroundParticles.inPlayback)
		{
			playgroundParticles.isDoneThread = true;
			playgroundParticles.threadHadNoActiveParticles = false;
			return;
		}
		if (!playgroundParticles.isPrewarming)
		{
			playgroundParticles.localDeltaTime = (PlaygroundC.globalTime - playgroundParticles.lastTimeUpdated) * playgroundParticles.particleTimescale;
			playgroundParticles.localTime += playgroundParticles.localDeltaTime;
			playgroundParticles.lastTimeUpdated = PlaygroundC.globalTime;
		}
		playgroundParticles.t = playgroundParticles.localDeltaTime;
		if (playgroundParticles.particleCount <= 0 || playgroundParticles.playgroundCache.color.Length != playgroundParticles.particleCount || playgroundParticles.playgroundCache.targetPosition.Length != playgroundParticles.particleCount || playgroundParticles.playgroundCache.targetDirection.Length != playgroundParticles.particleCount || (playgroundParticles.source == SOURCEC.State && playgroundParticles.states[playgroundParticles.activeState].IsInitializing()) || playgroundParticles.isYieldRefreshing || !PlaygroundC.IsReady() || playgroundParticles.isSettingParticleTime)
		{
			playgroundParticles.isDoneThread = true;
			playgroundParticles.threadHadNoActiveParticles = false;
			return;
		}
		if (!PlaygroundC.reference.calculate || !playgroundParticles.calculate || playgroundParticles.inTransition || !playgroundParticles.hasActiveParticles)
		{
			if (playgroundParticles.source != SOURCEC.Script)
			{
				playgroundParticles.isDoneThread = true;
				playgroundParticles.cameFromNonCalculatedFrame = true;
			}
			else
			{
				playgroundParticles.isDoneThread = true;
			}
			return;
		}
		if (!playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning && playgroundParticles.turbulenceStrength > 0f && playgroundParticles.turbulenceType != TURBULENCETYPE.None && playgroundParticles.turbulenceType == TURBULENCETYPE.Simplex && playgroundParticles.turbulenceSimplex == null)
		{
			playgroundParticles.isDoneThread = true;
			playgroundParticles.threadHadNoActiveParticles = false;
			return;
		}
		float num = playgroundParticles.t;
		Matrix4x4 matrix4x = default;
		if (playgroundParticles.source == SOURCEC.State || playgroundParticles.source == SOURCEC.WorldObject || playgroundParticles.source == SOURCEC.SkinnedWorldObject)
		{
			matrix4x.SetTRS(Vector3.zero, playgroundParticles.stRot, new Vector3(1f, 1f, 1f));
		}
		bool flag = true;
		if (playgroundParticles.source == SOURCEC.Transform)
		{
			for (int i = 0; i < playgroundParticles.sourceTransforms.Count; i++)
			{
				playgroundParticles.sourceTransforms[i].UpdateMatrix();
			}
		}
		if (playgroundParticles.source == SOURCEC.SkinnedWorldObject && playgroundParticles.skinnedWorldObjectReady && !playgroundParticles.forceSkinnedMeshUpdateOnMainThread && PlaygroundC.reference.skinnedMeshThreadMethod == ThreadMethodComponent.InsideParticleCalculation)
		{
			playgroundParticles.skinnedWorldObject.Update();
		}
		int num2 = playgroundParticles.particleCache.Length;
		if (playgroundParticles.playgroundCache.lifetimeSubtraction.Length != num2)
		{
			SetLifetimeSubtraction(playgroundParticles);
		}
		if (playgroundParticles.playgroundCache.maskAlpha.Length != num2)
		{
			playgroundParticles.playgroundCache.maskAlpha = new float[num2];
			playgroundParticles.playgroundCache.isMasked = new bool[num2];
		}
		if (playgroundParticles.playgroundCache.manipulatorId.Length != num2)
		{
			playgroundParticles.playgroundCache.manipulatorId = new int[num2];
		}
		if (playgroundParticles.playgroundCache.excludeFromManipulatorId.Length != num2)
		{
			playgroundParticles.playgroundCache.excludeFromManipulatorId = new int[num2];
		}
		if (playgroundParticles.playgroundCache.noForce.Length != num2)
		{
			playgroundParticles.playgroundCache.noForce = new bool[num2];
		}
		if (playgroundParticles.playgroundCache.isNonBirthed.Length != num2)
		{
			playgroundParticles.playgroundCache.isNonBirthed = new bool[num2];
		}
		if (playgroundParticles.playgroundCache.isFirstLoop.Length != num2)
		{
			playgroundParticles.playgroundCache.isFirstLoop = new bool[num2];
		}
		if (playgroundParticles.playgroundCache.isCalculatedThisFrame.Length != num2)
		{
			playgroundParticles.playgroundCache.isCalculatedThisFrame = new bool[num2];
		}
		if (playgroundParticles.playgroundCache.simulate.Length != num2)
		{
			playgroundParticles.playgroundCache.simulate = new bool[num2];
			for (int j = 0; j < num2; j++)
			{
				playgroundParticles.playgroundCache.simulate[j] = true;
			}
		}
		if (!playgroundParticles.separateRotationAxes)
		{
			if (playgroundParticles.playgroundCache.initialRotation == null || playgroundParticles.playgroundCache.initialRotation.Length != num2)
			{
				playgroundParticles.playgroundCache.initialRotation = new float[num2];
			}
			if (playgroundParticles.playgroundCache.rotation == null || playgroundParticles.playgroundCache.rotation.Length != num2)
			{
				playgroundParticles.playgroundCache.rotation = new float[num2];
			}
			if (playgroundParticles.playgroundCache.rotationSpeed == null || playgroundParticles.playgroundCache.rotationSpeed.Length != num2)
			{
				playgroundParticles.playgroundCache.rotationSpeed = new float[num2];
			}
		}
		else
		{
			if (playgroundParticles.playgroundCache.initialRotation3d == null || playgroundParticles.playgroundCache.initialRotation3d.Length != num2)
			{
				playgroundParticles.playgroundCache.initialRotation3d = new Vector3[num2];
			}
			if (playgroundParticles.playgroundCache.rotation3d == null || playgroundParticles.playgroundCache.rotation3d.Length != num2)
			{
				playgroundParticles.playgroundCache.rotation3d = new Vector3[num2];
			}
			if (playgroundParticles.playgroundCache.rotationSpeed3d == null || playgroundParticles.playgroundCache.rotationSpeed3d.Length != num2)
			{
				playgroundParticles.playgroundCache.rotationSpeed3d = new Vector3[num2];
			}
		}
		bool flag2 = playgroundParticles.collision && playgroundParticles.lifetimeLoss > 0f;
		if (flag2 && (playgroundParticles.playgroundCache.lifetimeLoss == null || playgroundParticles.playgroundCache.lifetimeLoss.Length != num2))
		{
			playgroundParticles.playgroundCache.lifetimeLoss = new float[num2];
		}
		if (playgroundParticles.applyParticleMask && (playgroundParticles.playgroundCache.maskSorting == null || playgroundParticles.playgroundCache.maskSorting.Length != num2 || playgroundParticles.particleMaskSorting != playgroundParticles.previousMaskSorting))
		{
			playgroundParticles.RefreshMaskSorting();
		}
		Vector3 zero = Vector3.zero;
		Vector3 up = Vector3.up;
		float y = PlaygroundC.initialTargetPosition.y;
		for (int k = 0; k < playgroundParticles.particleCount; k++)
		{
			if (num2 != playgroundParticles.particleCache.Length || playgroundParticles.isYieldRefreshing || playgroundParticles.isSettingParticleTime || playgroundParticles.isSettingParticleCount)
			{
				playgroundParticles.cameFromNonEmissionFrame = false;
				playgroundParticles.isDoneThread = true;
				playgroundParticles.threadHadNoActiveParticles = false;
				return;
			}
			if (!playgroundParticles.playgroundCache.simulate[k])
			{
				if (playgroundParticles.playgroundCache.rebirth[k] && playgroundParticles.loop)
				{
					playgroundParticles.InactivateParticle(k);
				}
				continue;
			}
			flag = false;
			playgroundParticles.playgroundCache.isCalculatedThisFrame[k] = false;
			float num3 = playgroundParticles.playgroundCache.death[k] - playgroundParticles.playgroundCache.birth[k] - playgroundParticles.playgroundCache.lifetimeSubtraction[k];
			float num4 = ((!(playgroundParticles.lifetime > 0f)) ? 0f : Mathf.Clamp01(playgroundParticles.playgroundCache.life[k] / num3));
			float normalizedP = (float)k * 1f / ((float)playgroundParticles.particleCount * 1f);
			float num5 = 1f;
			if (playgroundParticles.applyLifetimePositioningTimeScale)
			{
				num5 = playgroundParticles.lifetimePositioningTimeScale.Evaluate(num4);
			}
			if (playgroundParticles.applyParticleMask)
			{
				int num6 = playgroundParticles.playgroundCache.maskSorting[k];
				if (num6 >= 0 && num6 < playgroundParticles.particleCount)
				{
					if (k < playgroundParticles.particleMask)
					{
						if (playgroundParticles.playgroundCache.maskAlpha[num6] <= 0f || playgroundParticles.particleMaskTime <= 0f)
						{
							playgroundParticles.playgroundCache.isMasked[num6] = true;
							playgroundParticles.playgroundCache.maskAlpha[num6] = 0f;
							playgroundParticles.particleCache[num6].startSize = 0f;
						}
						else
						{
							playgroundParticles.playgroundCache.maskAlpha[num6] -= 1f / playgroundParticles.particleMaskTime * playgroundParticles.localDeltaTime;
						}
					}
					else if (playgroundParticles.playgroundCache.maskAlpha[num6] >= 1f || playgroundParticles.particleMaskTime <= 0f)
					{
						playgroundParticles.playgroundCache.isMasked[num6] = false;
						playgroundParticles.playgroundCache.maskAlpha[num6] = 1f;
					}
					else
					{
						playgroundParticles.playgroundCache.maskAlpha[num6] += 1f / playgroundParticles.particleMaskTime * playgroundParticles.localDeltaTime;
					}
				}
			}
			else
			{
				playgroundParticles.playgroundCache.isMasked[k] = false;
				playgroundParticles.playgroundCache.maskAlpha[k] = 1f;
			}
			Color32 particleColor = playgroundParticles.GetParticleColor(k, num4, normalizedP);
			playgroundParticles.SetParticleColorInternal(k, particleColor);
			playgroundParticles.playgroundCache.color[k] = particleColor;
			if (playgroundParticles.emit)
			{
				if (!playgroundParticles.syncPositionsOnMainThread)
				{
					ref Vector3 reference = ref playgroundParticles.playgroundCache.previousTargetPosition[k];
					reference = playgroundParticles.playgroundCache.targetPosition[k];
				}
				switch (playgroundParticles.source)
				{
				case SOURCEC.State:
					if (!playgroundParticles.playgroundCache.rebirth[k] && !playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning)
					{
						break;
					}
					if (playgroundParticles.applyInitialLocalVelocity && !playgroundParticles.onlyLifetimePositioning)
					{
						ref Vector3 reference12 = ref playgroundParticles.playgroundCache.targetDirection[k];
						reference12 = matrix4x.MultiplyPoint3x4(Vector3Scale(playgroundParticles.playgroundCache.initialLocalVelocity[k], playgroundParticles.states[playgroundParticles.activeState].GetNormal(k)));
					}
					if (playgroundParticles.onlyLifetimePositioning)
					{
						ref Vector3 reference13 = ref playgroundParticles.playgroundCache.targetDirection[k];
						reference13 = matrix4x.MultiplyPoint3x4(Vector3Scale(playgroundParticles.lifetimePositioning.Evaluate(num4 * num5, playgroundParticles.lifetimePositioningScale), playgroundParticles.states[playgroundParticles.activeState].GetNormal(k)));
						if (playgroundParticles.applyLifetimePositioningPositionScale)
						{
							playgroundParticles.playgroundCache.targetDirection[k] *= playgroundParticles.lifetimePositioningPositionScale.Evaluate(num4);
						}
					}
					playgroundParticles.SetSourcePosition(k);
					if (playgroundParticles.localSpace && playgroundParticles.applyLocalSpaceMovementCompensation)
					{
						ref Vector3 reference14 = ref playgroundParticles.playgroundCache.localSpaceMovementCompensation[k];
						reference14 = playgroundParticles.playgroundCache.targetPosition[k] - playgroundParticles.playgroundCache.previousTargetPosition[k];
					}
					break;
				case SOURCEC.Transform:
					if (!playgroundParticles.playgroundCache.rebirth[k] && !playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning)
					{
						break;
					}
					if (playgroundParticles.applyInitialLocalVelocity && !playgroundParticles.onlyLifetimePositioning)
					{
						if (playgroundParticles.localSpace && playgroundParticles.transformIndex != playgroundParticles.psTransformNum)
						{
							ref Vector3 reference20 = ref playgroundParticles.playgroundCache.targetDirection[k];
							reference20 = playgroundParticles.particleSystemInverseRotation * playgroundParticles.sourceTransforms[playgroundParticles.transformIndex].rotation * playgroundParticles.playgroundCache.initialLocalVelocity[k];
						}
						else
						{
							ref Vector3 reference21 = ref playgroundParticles.playgroundCache.targetDirection[k];
							reference21 = playgroundParticles.sourceTransforms[playgroundParticles.transformIndex].rotation * playgroundParticles.playgroundCache.initialLocalVelocity[k];
						}
					}
					if (playgroundParticles.onlyLifetimePositioning)
					{
						ref Vector3 reference22 = ref playgroundParticles.playgroundCache.targetDirection[k];
						reference22 = playgroundParticles.sourceTransforms[playgroundParticles.transformIndex].rotation * playgroundParticles.lifetimePositioning.Evaluate(num4 * num5, playgroundParticles.lifetimePositioningScale);
						if (playgroundParticles.applyLifetimePositioningPositionScale)
						{
							playgroundParticles.playgroundCache.targetDirection[k] *= playgroundParticles.lifetimePositioningPositionScale.Evaluate(num4 * num5);
						}
					}
					playgroundParticles.SetSourcePosition(k);
					if (playgroundParticles.localSpace && playgroundParticles.applyLocalSpaceMovementCompensation)
					{
						ref Vector3 reference23 = ref playgroundParticles.playgroundCache.localSpaceMovementCompensation[k];
						reference23 = playgroundParticles.playgroundCache.targetPosition[k] - playgroundParticles.playgroundCache.previousTargetPosition[k];
					}
					break;
				case SOURCEC.WorldObject:
					if (!playgroundParticles.playgroundCache.rebirth[k] && !playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning)
					{
						break;
					}
					if (playgroundParticles.applyInitialLocalVelocity && !playgroundParticles.onlyLifetimePositioning)
					{
						ref Vector3 reference6 = ref playgroundParticles.playgroundCache.targetDirection[k];
						reference6 = matrix4x.MultiplyPoint3x4(Vector3Scale(playgroundParticles.playgroundCache.initialLocalVelocity[k], playgroundParticles.worldObject.normals[k % playgroundParticles.worldObject.normals.Length]));
					}
					if (playgroundParticles.onlyLifetimePositioning)
					{
						ref Vector3 reference7 = ref playgroundParticles.playgroundCache.targetDirection[k];
						reference7 = matrix4x.MultiplyPoint3x4(Vector3Scale(playgroundParticles.lifetimePositioning.Evaluate(num4 * num5, playgroundParticles.lifetimePositioningScale), playgroundParticles.worldObject.normals[k % playgroundParticles.worldObject.normals.Length]));
						if (playgroundParticles.applyLifetimePositioningPositionScale)
						{
							playgroundParticles.playgroundCache.targetDirection[k] *= playgroundParticles.lifetimePositioningPositionScale.Evaluate(num4);
						}
					}
					playgroundParticles.SetSourcePosition(k);
					if (playgroundParticles.localSpace && playgroundParticles.applyLocalSpaceMovementCompensation)
					{
						ref Vector3 reference8 = ref playgroundParticles.playgroundCache.localSpaceMovementCompensation[k];
						reference8 = playgroundParticles.playgroundCache.targetPosition[k] - playgroundParticles.playgroundCache.previousTargetPosition[k];
					}
					break;
				case SOURCEC.SkinnedWorldObject:
					if (!playgroundParticles.skinnedWorldObjectReady || (!playgroundParticles.playgroundCache.rebirth[k] && !playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning))
					{
						break;
					}
					if (playgroundParticles.applyInitialLocalVelocity && !playgroundParticles.onlyLifetimePositioning)
					{
						ref Vector3 reference15 = ref playgroundParticles.playgroundCache.targetDirection[k];
						reference15 = matrix4x.MultiplyPoint3x4(Vector3Scale(playgroundParticles.playgroundCache.initialLocalVelocity[k], playgroundParticles.skinnedWorldObject.normals[k % playgroundParticles.skinnedWorldObject.normals.Length]));
					}
					if (playgroundParticles.onlyLifetimePositioning)
					{
						ref Vector3 reference16 = ref playgroundParticles.playgroundCache.targetDirection[k];
						reference16 = matrix4x.MultiplyPoint3x4(Vector3Scale(playgroundParticles.lifetimePositioning.Evaluate(num4 * num5, playgroundParticles.lifetimePositioningScale), playgroundParticles.skinnedWorldObject.normals[k % playgroundParticles.skinnedWorldObject.normals.Length]));
						if (playgroundParticles.applyLifetimePositioningPositionScale)
						{
							playgroundParticles.playgroundCache.targetDirection[k] *= playgroundParticles.lifetimePositioningPositionScale.Evaluate(num4);
						}
					}
					playgroundParticles.SetSourcePosition(k);
					if (playgroundParticles.localSpace && playgroundParticles.applyLocalSpaceMovementCompensation)
					{
						ref Vector3 reference17 = ref playgroundParticles.playgroundCache.localSpaceMovementCompensation[k];
						reference17 = playgroundParticles.playgroundCache.targetPosition[k] - playgroundParticles.playgroundCache.previousTargetPosition[k];
					}
					break;
				case SOURCEC.Projection:
					if (!playgroundParticles.playgroundCache.rebirth[k] && !playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning)
					{
						break;
					}
					if (playgroundParticles.applyInitialLocalVelocity && !playgroundParticles.onlyLifetimePositioning)
					{
						ref Vector3 reference18 = ref playgroundParticles.playgroundCache.targetDirection[k];
						reference18 = Vector3Scale(playgroundParticles.projection.GetNormal(k), playgroundParticles.playgroundCache.initialLocalVelocity[k]);
					}
					if (playgroundParticles.onlyLifetimePositioning)
					{
						ref Vector3 reference19 = ref playgroundParticles.playgroundCache.targetDirection[k];
						reference19 = Vector3Scale(playgroundParticles.projection.GetNormal(k), playgroundParticles.lifetimePositioning.Evaluate(num4 * num5, playgroundParticles.lifetimePositioningScale));
						if (playgroundParticles.applyLifetimePositioningPositionScale)
						{
							playgroundParticles.playgroundCache.targetDirection[k] *= playgroundParticles.lifetimePositioningPositionScale.Evaluate(num4);
						}
					}
					playgroundParticles.SetSourcePosition(k);
					break;
				case SOURCEC.Paint:
					if (!playgroundParticles.playgroundCache.rebirth[k] && !playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning)
					{
						break;
					}
					if (playgroundParticles.applyInitialLocalVelocity && !playgroundParticles.onlyLifetimePositioning)
					{
						ref Vector3 reference9 = ref playgroundParticles.playgroundCache.targetDirection[k];
						reference9 = playgroundParticles.paint.GetRotation(k) * Vector3Scale(playgroundParticles.playgroundCache.initialLocalVelocity[k], playgroundParticles.paint.GetNormal(k));
					}
					if (playgroundParticles.onlyLifetimePositioning)
					{
						ref Vector3 reference10 = ref playgroundParticles.playgroundCache.targetDirection[k];
						reference10 = playgroundParticles.paint.GetRotation(k) * Vector3Scale(playgroundParticles.lifetimePositioning.Evaluate(num4 * num5, playgroundParticles.lifetimePositioningScale), playgroundParticles.paint.GetNormal(k));
						if (playgroundParticles.applyLifetimePositioningPositionScale)
						{
							playgroundParticles.playgroundCache.targetDirection[k] *= playgroundParticles.lifetimePositioningPositionScale.Evaluate(num4);
						}
					}
					playgroundParticles.SetSourcePosition(k);
					if (playgroundParticles.localSpace && playgroundParticles.applyLocalSpaceMovementCompensation)
					{
						ref Vector3 reference11 = ref playgroundParticles.playgroundCache.localSpaceMovementCompensation[k];
						reference11 = playgroundParticles.playgroundCache.targetPosition[k] - playgroundParticles.playgroundCache.previousTargetPosition[k];
					}
					break;
				case SOURCEC.Spline:
				{
					if (!playgroundParticles.playgroundCache.rebirth[k] && !playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning)
					{
						break;
					}
					if (playgroundParticles.treatAsOneSpline)
					{
						playgroundParticles.splineIndex = (int)((float)k * 1f / ((float)num2 * 1f) * (float)playgroundParticles.splines.Count + playgroundParticles.splineTimeOffset) % playgroundParticles.splines.Count;
					}
					else
					{
						playgroundParticles.splineIndex = k % playgroundParticles.splines.Count;
					}
					float num7 = ((!playgroundParticles.treatAsOneSpline) ? ((float)k * 1f / ((float)num2 * 1f) + playgroundParticles.splineTimeOffset) : ((float)(k * playgroundParticles.splines.Count) * 1f / ((float)num2 * 1f) + playgroundParticles.splineTimeOffset));
					if (playgroundParticles.applyInitialLocalVelocity && !playgroundParticles.onlyLifetimePositioning)
					{
						ref Vector3 reference2 = ref playgroundParticles.playgroundCache.targetDirection[k];
						reference2 = Vector3Scale(playgroundParticles.splines[playgroundParticles.splineIndex].GetDirection(num7), playgroundParticles.playgroundCache.initialLocalVelocity[k]);
					}
					if (playgroundParticles.onlyLifetimePositioning)
					{
						ref Vector3 reference3 = ref playgroundParticles.playgroundCache.targetDirection[k];
						reference3 = Vector3Scale(playgroundParticles.splines[playgroundParticles.splineIndex].GetDirection(num7), playgroundParticles.lifetimePositioning.Evaluate(num4 * num5, playgroundParticles.lifetimePositioningScale));
						if (playgroundParticles.applyLifetimePositioningPositionScale)
						{
							playgroundParticles.playgroundCache.targetDirection[k] *= playgroundParticles.lifetimePositioningPositionScale.Evaluate(num4 * num5);
						}
					}
					ref Vector3 reference4 = ref playgroundParticles.playgroundCache.targetPosition[k];
					reference4 = playgroundParticles.splines[playgroundParticles.splineIndex].GetPoint(num7);
					if (playgroundParticles.applySourceScatter)
					{
						playgroundParticles.playgroundCache.targetPosition[k] += Vector3.Scale(playgroundParticles.playgroundCache.scatterPosition[k], playgroundParticles.scatterScale);
					}
					if (playgroundParticles.localSpace && playgroundParticles.applyLocalSpaceMovementCompensation)
					{
						ref Vector3 reference5 = ref playgroundParticles.playgroundCache.localSpaceMovementCompensation[k];
						reference5 = playgroundParticles.playgroundCache.targetPosition[k] - playgroundParticles.playgroundCache.previousTargetPosition[k];
					}
					break;
				}
				}
			}
			if (playgroundParticles.playgroundCache.life[k] == 0f)
			{
				if (!playgroundParticles.onlyLifetimePositioning)
				{
					ref Vector3 reference24 = ref playgroundParticles.playgroundCache.position[k];
					reference24 = playgroundParticles.playgroundCache.targetPosition[k];
					if (playgroundParticles.applyDeltaOnRebirth && !playgroundParticles.cameFromNonEmissionFrame && playgroundParticles.localTime - playgroundParticles.emissionStopped > playgroundParticles.lifetime && playgroundParticles.playgroundCache.birthDelay[k] == 0f && !playgroundParticles.onlySourcePositioning)
					{
						float num8 = playgroundParticles.localTime - playgroundParticles.playgroundCache.birth[k];
						if (playgroundParticles.isPrewarming)
						{
							num8 *= 0.5f;
						}
						playgroundParticles.playgroundCache.position[k] += playgroundParticles.playgroundCache.velocity[k] * playgroundParticles.velocityScale * num8;
					}
				}
				playgroundParticles.playgroundCache.initialColor[k] = particleColor;
				if (!playgroundParticles.cameFromNonEmissionFrame && playgroundParticles.calculateDeltaMovement && !playgroundParticles.isPrewarming && playgroundParticles.source != SOURCEC.Script)
				{
					Vector3 vector = playgroundParticles.playgroundCache.targetPosition[k] - playgroundParticles.playgroundCache.previousTargetPosition[k];
					if (playgroundParticles.deltaMovementStrengthValueMethod == VALUEMETHOD.Constant)
					{
						playgroundParticles.playgroundCache.velocity[k] += vector * playgroundParticles.deltaMovementStrength / playgroundParticles.realSimulationTime;
					}
					else
					{
						playgroundParticles.playgroundCache.velocity[k] += vector * RandomRange(playgroundParticles.internalRandom01, playgroundParticles.deltaMovementStrength, playgroundParticles.minDeltaMovementStrength) / playgroundParticles.realSimulationTime;
					}
				}
				playgroundParticles.playgroundCache.birthDelay[k] = 0f;
			}
			if (playgroundParticles.playgroundCache.birth[k] > playgroundParticles.localTime)
			{
				playgroundParticles.particleCache[k].startSize = 0f;
				ref Vector3 reference25 = ref playgroundParticles.playgroundCache.position[k];
				reference25 = PlaygroundC.initialTargetPosition;
			}
			if (playgroundParticles.playgroundCache.rebirth[k])
			{
				if (playgroundParticles.playgroundCache.birth[k] + playgroundParticles.playgroundCache.life[k] <= playgroundParticles.localTime + num3 && (!playgroundParticles.playgroundCache.isNonBirthed[k] || playgroundParticles.onlyLifetimePositioning || playgroundParticles.onlySourcePositioning))
				{
					if (!playgroundParticles.playgroundCache.changedByPropertySize[k])
					{
						if (playgroundParticles.applyLifetimeSize && !playgroundParticles.applyParticleArraySize)
						{
							playgroundParticles.playgroundCache.size[k] = playgroundParticles.playgroundCache.initialSize[k] * playgroundParticles.lifetimeSize.Evaluate(num4) * playgroundParticles.scale;
						}
						else if (playgroundParticles.applyLifetimeSize && playgroundParticles.applyParticleArraySize)
						{
							playgroundParticles.playgroundCache.size[k] = playgroundParticles.playgroundCache.initialSize[k] * playgroundParticles.lifetimeSize.Evaluate(num4) * playgroundParticles.particleArraySize.Evaluate((float)k * 1f / ((float)playgroundParticles.particleCount * 1f)) * playgroundParticles.scale;
						}
						else if (playgroundParticles.applyParticleArraySize)
						{
							playgroundParticles.playgroundCache.size[k] = playgroundParticles.playgroundCache.initialSize[k] * playgroundParticles.particleArraySize.Evaluate((float)k * 1f / ((float)playgroundParticles.particleCount * 1f)) * playgroundParticles.scale;
						}
						else
						{
							playgroundParticles.playgroundCache.size[k] = playgroundParticles.playgroundCache.initialSize[k] * playgroundParticles.scale;
						}
					}
					if (!playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning)
					{
						if (playgroundParticles.hasCollisionCache && playgroundParticles.stickyCollisions && playgroundParticles.collisionCache.hasCollided[k] && !playgroundParticles.playgroundCache.isNonBirthed[k])
						{
							if (playgroundParticles.stickyCollisionsSurfaceOffset == 0f)
							{
								ref Vector3 reference26 = ref playgroundParticles.playgroundCache.position[k];
								reference26 = playgroundParticles.collisionCache.stickyPosition[k];
							}
							else
							{
								ref Vector3 reference27 = ref playgroundParticles.playgroundCache.position[k];
								reference27 = playgroundParticles.collisionCache.stickyPosition[k] + playgroundParticles.collisionCache.collisionNormal[k] * playgroundParticles.stickyCollisionsSurfaceOffset;
							}
						}
						else if (!playgroundParticles.playgroundCache.noForce[k] && playgroundParticles.playgroundCache.life[k] > 0f)
						{
							if (playgroundParticles.applyVelocityBending)
							{
								if (playgroundParticles.velocityBendingType == VELOCITYBENDINGTYPEC.SourcePosition)
								{
									playgroundParticles.playgroundCache.velocity[k] += Vector3.Reflect(new Vector3(playgroundParticles.playgroundCache.velocity[k].x * playgroundParticles.velocityBending.x, playgroundParticles.playgroundCache.velocity[k].y * playgroundParticles.velocityBending.y, playgroundParticles.playgroundCache.velocity[k].z * playgroundParticles.velocityBending.z), (playgroundParticles.playgroundCache.targetPosition[k] - playgroundParticles.playgroundCache.position[k]).normalized) * num;
								}
								else
								{
									playgroundParticles.playgroundCache.velocity[k] += Vector3.Reflect(new Vector3(playgroundParticles.playgroundCache.velocity[k].x * playgroundParticles.velocityBending.x, playgroundParticles.playgroundCache.velocity[k].y * playgroundParticles.velocityBending.y, playgroundParticles.playgroundCache.velocity[k].z * playgroundParticles.velocityBending.z), (playgroundParticles.playgroundCache.previousParticlePosition[k] - playgroundParticles.playgroundCache.position[k]).normalized) * num;
								}
							}
							ref Vector3 reference28 = ref playgroundParticles.playgroundCache.previousTargetPosition[k];
							reference28 = playgroundParticles.playgroundCache.targetPosition[k];
							if (playgroundParticles.localSpace && playgroundParticles.source == SOURCEC.Transform && playgroundParticles.transformIndex != playgroundParticles.psTransformNum)
							{
								playgroundParticles.playgroundCache.velocity[k] -= playgroundParticles.sourceTransforms[playgroundParticles.transformIndex].rotation * playgroundParticles.gravity * num;
							}
							else
							{
								playgroundParticles.playgroundCache.velocity[k] -= playgroundParticles.gravity * num;
							}
							if (playgroundParticles.applyLifetimeVelocity)
							{
								playgroundParticles.playgroundCache.velocity[k] += playgroundParticles.lifetimeVelocity.Evaluate(num4, playgroundParticles.lifetimeVelocityScale) * num;
							}
							if (playgroundParticles.HasTurbulence() && PlaygroundC.reference.turbulenceThreadMethod == ThreadMethodComponent.InsideParticleCalculation && !playgroundParticles.playgroundCache.noForce[k])
							{
								Turbulence(playgroundParticles, playgroundParticles.turbulenceSimplex, k, playgroundParticles.t, playgroundParticles.turbulenceType, playgroundParticles.turbulenceTimeScale, playgroundParticles.turbulenceScale / playgroundParticles.velocityScale, playgroundParticles.turbulenceStrength, playgroundParticles.turbulenceApplyLifetimeStrength, playgroundParticles.turbulenceLifetimeStrength);
							}
							if (playgroundParticles.playgroundCache.velocity[k].sqrMagnitude > playgroundParticles.maxVelocity)
							{
								ref Vector3 reference29 = ref playgroundParticles.playgroundCache.velocity[k];
								reference29 = Vector3.ClampMagnitude(playgroundParticles.playgroundCache.velocity[k], playgroundParticles.maxVelocity);
							}
							if (playgroundParticles.damping > 0f)
							{
								ref Vector3 reference30 = ref playgroundParticles.playgroundCache.velocity[k];
								reference30 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[k], zero, playgroundParticles.damping * num);
							}
							if (playgroundParticles.transitionBackToSource)
							{
								float num9 = playgroundParticles.transitionBackToSourceAmount.Evaluate(num4) * playgroundParticles.particleTimescale;
								ref Vector3 reference31 = ref playgroundParticles.playgroundCache.position[k];
								reference31 = Vector3.Lerp(playgroundParticles.playgroundCache.position[k], playgroundParticles.playgroundCache.targetPosition[k], num9);
								playgroundParticles.playgroundCache.velocity[k] *= 1f - num9;
							}
							if (playgroundParticles.axisConstraints.x)
							{
								playgroundParticles.playgroundCache.velocity[k].x = 0f;
							}
							if (playgroundParticles.axisConstraints.y)
							{
								playgroundParticles.playgroundCache.velocity[k].y = 0f;
							}
							if (playgroundParticles.axisConstraints.z)
							{
								playgroundParticles.playgroundCache.velocity[k].z = 0f;
							}
							ref Vector3 reference32 = ref playgroundParticles.playgroundCache.collisionParticlePosition[k];
							reference32 = playgroundParticles.playgroundCache.position[k];
							playgroundParticles.playgroundCache.position[k] += playgroundParticles.playgroundCache.velocity[k] * playgroundParticles.velocityScale * num;
							if (playgroundParticles.localSpace && playgroundParticles.applyLocalSpaceMovementCompensation)
							{
								if (!playgroundParticles.applyMovementCompensationLifetimeStrength)
								{
									playgroundParticles.playgroundCache.position[k] += playgroundParticles.playgroundCache.localSpaceMovementCompensation[k];
								}
								else
								{
									playgroundParticles.playgroundCache.position[k] += playgroundParticles.playgroundCache.localSpaceMovementCompensation[k] * playgroundParticles.movementCompensationLifetimeStrength.Evaluate(num4);
								}
							}
							if (playgroundParticles.renderModeStretch && playgroundParticles.realSimulationTime > 0f)
							{
								if (playgroundParticles.applyLifetimeStretching)
								{
									playgroundParticles.particleCache[k].velocity = Vector3.Slerp(playgroundParticles.particleCache[k].velocity, (playgroundParticles.playgroundCache.position[k] - playgroundParticles.playgroundCache.previousParticlePosition[k]) * playgroundParticles.stretchLifetime.Evaluate(playgroundParticles.playgroundCache.life[k] / num3) / playgroundParticles.realSimulationTime, num * playgroundParticles.stretchSpeed * playgroundParticles.realSimulationTime);
								}
								else if (playgroundParticles.stretchSpeed > 0f)
								{
									playgroundParticles.particleCache[k].velocity = Vector3.Slerp(playgroundParticles.particleCache[k].velocity, (playgroundParticles.playgroundCache.position[k] - playgroundParticles.playgroundCache.previousParticlePosition[k]) / playgroundParticles.realSimulationTime, num * playgroundParticles.stretchSpeed * playgroundParticles.realSimulationTime);
								}
								else
								{
									playgroundParticles.particleCache[k].velocity = playgroundParticles.stretchStartDirection;
								}
							}
						}
					}
					else
					{
						if (playgroundParticles.onlyLifetimePositioning && !playgroundParticles.onlySourcePositioning)
						{
							if (!playgroundParticles.playgroundCache.changedByPropertyTarget[k])
							{
								if (playgroundParticles.lifetimePositioningUsesSourceDirection && playgroundParticles.source != SOURCEC.Script)
								{
									ref Vector3 reference33 = ref playgroundParticles.playgroundCache.position[k];
									reference33 = playgroundParticles.playgroundCache.targetPosition[k] + playgroundParticles.playgroundCache.targetDirection[k];
								}
								else if (!playgroundParticles.applyLifetimePositioningPositionScale)
								{
									ref Vector3 reference34 = ref playgroundParticles.playgroundCache.position[k];
									reference34 = playgroundParticles.playgroundCache.targetPosition[k] + playgroundParticles.lifetimePositioning.Evaluate(num4 * num5, playgroundParticles.lifetimePositioningScale);
								}
								else
								{
									ref Vector3 reference35 = ref playgroundParticles.playgroundCache.position[k];
									reference35 = playgroundParticles.playgroundCache.targetPosition[k] + playgroundParticles.lifetimePositioning.Evaluate(num4 * num5, playgroundParticles.lifetimePositioningScale) * playgroundParticles.lifetimePositioningPositionScale.Evaluate(num4 * num5);
								}
							}
						}
						else if (playgroundParticles.source != SOURCEC.Script && !playgroundParticles.playgroundCache.isNonBirthed[k])
						{
							ref Vector3 reference36 = ref playgroundParticles.playgroundCache.position[k];
							reference36 = playgroundParticles.playgroundCache.targetPosition[k];
						}
						if (playgroundParticles.renderModeStretch && playgroundParticles.realSimulationTime > 0f)
						{
							if (playgroundParticles.applyLifetimeStretching)
							{
								playgroundParticles.particleCache[k].velocity = Vector3.Slerp(playgroundParticles.particleCache[k].velocity, (playgroundParticles.playgroundCache.position[k] - playgroundParticles.playgroundCache.previousParticlePosition[k]) / playgroundParticles.realSimulationTime, num * playgroundParticles.stretchSpeed) * playgroundParticles.stretchLifetime.Evaluate(playgroundParticles.playgroundCache.life[k] / num3);
							}
							else if (playgroundParticles.stretchSpeed > 0f)
							{
								playgroundParticles.particleCache[k].velocity = Vector3.Slerp(playgroundParticles.particleCache[k].velocity, (playgroundParticles.playgroundCache.position[k] - playgroundParticles.playgroundCache.previousParticlePosition[k]) / playgroundParticles.realSimulationTime, num * playgroundParticles.stretchSpeed);
							}
							else
							{
								playgroundParticles.particleCache[k].velocity = playgroundParticles.stretchStartDirection;
							}
						}
						ref Vector3 reference37 = ref playgroundParticles.playgroundCache.previousTargetPosition[k];
						reference37 = playgroundParticles.playgroundCache.targetPosition[k];
					}
					if (num != 0f)
					{
						if (playgroundParticles.separateRotationAxes)
						{
							playgroundParticles.playgroundCache.rotation3d[k] += playgroundParticles.playgroundCache.rotationSpeed3d[k] * num;
						}
						else if (!playgroundParticles.rotateTowardsDirection)
						{
							playgroundParticles.playgroundCache.rotation[k] += playgroundParticles.playgroundCache.rotationSpeed[k] * num;
						}
						else if (playgroundParticles.playgroundCache.life[k] != 0f && playgroundParticles.playgroundCache.position[k] - playgroundParticles.playgroundCache.previousParticlePosition[k] != Vector3.zero)
						{
							playgroundParticles.playgroundCache.rotation[k] = playgroundParticles.playgroundCache.initialRotation[k] + SignedAngle(up, playgroundParticles.playgroundCache.position[k] - playgroundParticles.playgroundCache.previousParticlePosition[k], playgroundParticles.rotationNormal);
						}
					}
					if (!playgroundParticles.syncPositionsOnMainThread && playgroundParticles.playgroundCache.life[k] > 0f)
					{
						if (playgroundParticles.separateRotationAxes)
						{
							playgroundParticles.particleCache[k].rotation3D = playgroundParticles.playgroundCache.rotation3d[k];
						}
						else
						{
							playgroundParticles.particleCache[k].rotation = playgroundParticles.playgroundCache.rotation[k];
						}
					}
					ref Vector3 reference38 = ref playgroundParticles.playgroundCache.previousParticlePosition[k];
					reference38 = playgroundParticles.playgroundCache.position[k];
					if (playgroundParticles.hasTimerEvent)
					{
						playgroundParticles.SendEvent(EVENTTYPEC.Time, k);
					}
				}
				else
				{
					playgroundParticles.particleCache[k].startSize = 0f;
					ref Vector3 reference39 = ref playgroundParticles.playgroundCache.position[k];
					reference39 = PlaygroundC.initialTargetPosition;
				}
				float num10 = (playgroundParticles.localTime - playgroundParticles.playgroundCache.birth[k]) / num3;
				if (playgroundParticles.playgroundCache.life[k] < playgroundParticles.playgroundCache.death[k] - playgroundParticles.playgroundCache.birth[k])
				{
					playgroundParticles.playgroundCache.life[k] = num3 * num10;
					if (flag2)
					{
						playgroundParticles.playgroundCache.life[k] += playgroundParticles.playgroundCache.lifetimeLoss[k];
						playgroundParticles.playgroundCache.lifetimeLoss[k] = 0f;
					}
					if (!playgroundParticles.syncPositionsOnMainThread)
					{
						playgroundParticles.particleCache[k].remainingLifetime = Mathf.Clamp(num3 * (1f - num10), playgroundParticles.minShurikenLifetime, playgroundParticles.playgroundCache.death[k] - playgroundParticles.playgroundCache.birth[k]);
					}
					if (playgroundParticles.lifetimeValueMethod == VALUEMETHOD.RandomBetweenTwoValues && playgroundParticles.playgroundCache.life[k] > num3)
					{
						if ((playgroundParticles.hasEvent || playgroundParticles.hasEventManipulatorGlobal || playgroundParticles.hasEventManipulatorLocal) && !playgroundParticles.playgroundCache.isNonBirthed[k])
						{
							SendDeathEvents(playgroundParticles, k);
						}
						playgroundParticles.particleCache[k].startSize = 0f;
						playgroundParticles.particleCache[k].velocity = zero;
						ref Vector3 reference40 = ref playgroundParticles.playgroundCache.position[k];
						reference40 = PlaygroundC.initialTargetPosition;
					}
				}
				else
				{
					if (!playgroundParticles.loop && !playgroundParticles.playgroundCache.isNonBirthed[k])
					{
						if (playgroundParticles.hasEvent || playgroundParticles.hasEventManipulatorGlobal || playgroundParticles.hasEventManipulatorLocal)
						{
							SendDeathEvents(playgroundParticles, k);
						}
						playgroundParticles.InactivateParticle(k);
						continue;
					}
					if (!playgroundParticles.loop && playgroundParticles.localTime > playgroundParticles.simulationStarted + (playgroundParticles.playgroundCache.death[k] - playgroundParticles.playgroundCache.birth[k]) - 0.01f)
					{
						playgroundParticles.loopExceeded = true;
						if (playgroundParticles.loopExceededOnParticle == k && num10 > 2f)
						{
							if (playgroundParticles.disableOnDone)
							{
								playgroundParticles.queueEmissionHalt = true;
							}
							playgroundParticles.threadHadNoActiveParticles = true;
							playgroundParticles.hasActiveParticles = false;
						}
						if (playgroundParticles.loopExceededOnParticle == -1)
						{
							playgroundParticles.loopExceededOnParticle = k;
						}
					}
					playgroundParticles.particleCache[k].velocity = zero;
					if ((playgroundParticles.hasEvent || playgroundParticles.hasEventManipulatorGlobal || playgroundParticles.hasEventManipulatorLocal) && !playgroundParticles.playgroundCache.isNonBirthed[k])
					{
						SendDeathEvents(playgroundParticles, k);
					}
					if (!(playgroundParticles.localTime >= playgroundParticles.playgroundCache.birth[k] + playgroundParticles.playgroundCache.birthDelay[k]) || playgroundParticles.loopExceeded || playgroundParticles.source == SOURCEC.Script || !playgroundParticles.emit)
					{
						playgroundParticles.InactivateParticle(k);
						continue;
					}
					if (!playgroundParticles.playgroundCache.changedByPropertyDeath[k] || (playgroundParticles.playgroundCache.changedByPropertyDeath[k] && playgroundParticles.localTime > playgroundParticles.playgroundCache.death[k]))
					{
						Rebirth(playgroundParticles, k, playgroundParticles.internalRandom01);
					}
					else
					{
						playgroundParticles.particleCache[k].velocity = zero;
						playgroundParticles.particleCache[k].startSize = 0f;
						ref Vector3 reference41 = ref playgroundParticles.playgroundCache.position[k];
						reference41 = PlaygroundC.initialTargetPosition;
					}
				}
				for (int l = 0; l < playgroundParticles.manipulators.Count; l++)
				{
					CalculateManipulator(playgroundParticles, playgroundParticles.manipulators[l], k, num, playgroundParticles.playgroundCache.life[k], playgroundParticles.playgroundCache.position[k], ((!playgroundParticles.localSpace) ? playgroundParticles.manipulators[l].transform.position : playgroundParticles.manipulators[l].transform.localPosition) + playgroundParticles.manipulatorFix, playgroundParticles.localSpace);
				}
				if (playgroundParticles.hasGlobalAffectingManipulators)
				{
					for (int m = 0; m < PlaygroundC.reference.manipulators.Count; m++)
					{
						if ((PlaygroundC.reference.manipulators[m].affects.value & (1 << playgroundParticles.GetLayer())) != 0)
						{
							CalculateManipulator(playgroundParticles, PlaygroundC.reference.manipulators[m], k, num, playgroundParticles.playgroundCache.life[k], playgroundParticles.playgroundCache.position[k], ((!playgroundParticles.localSpace) ? PlaygroundC.reference.manipulators[m].transform.position : PlaygroundC.reference.manipulators[m].transform.localPosition) + playgroundParticles.manipulatorFix, playgroundParticles.localSpace);
						}
					}
				}
				if (!playgroundParticles.syncPositionsOnMainThread)
				{
					playgroundParticles.particleCache[k].startSize = ((!(playgroundParticles.playgroundCache.maskAlpha[k] > 0f) || playgroundParticles.particleCache[k].position.y == y) ? 0f : playgroundParticles.playgroundCache.size[k]);
				}
			}
			else
			{
				playgroundParticles.InactivateParticle(k);
			}
			if (!playgroundParticles.syncPositionsOnMainThread)
			{
				playgroundParticles.particleCache[k].position = playgroundParticles.playgroundCache.position[k];
			}
			playgroundParticles.playgroundCache.isCalculatedThisFrame[k] = true;
		}
		if (playgroundParticles.isSettingParticleTime)
		{
			playgroundParticles.threadHadNoActiveParticles = false;
		}
		else
		{
			playgroundParticles.threadHadNoActiveParticles = (flag && !playgroundParticles.emit) || (flag && !playgroundParticles.loop);
		}
		playgroundParticles.cameFromNonEmissionFrame = false;
		playgroundParticles.isDoneThread = true;
		if (!playgroundParticles.HasTurbulence() || PlaygroundC.reference.turbulenceThreadMethod != ThreadMethodComponent.OnePerSystem)
		{
			return;
		}
		PlaygroundC.RunAsync(() =>
		{
			for (int n = 0; n < playgroundParticles.particleCount; n++)
			{
				if (!playgroundParticles.playgroundCache.noForce[n])
				{
					Turbulence(playgroundParticles, playgroundParticles.turbulenceSimplex, n, playgroundParticles.t, playgroundParticles.turbulenceType, playgroundParticles.turbulenceTimeScale, playgroundParticles.turbulenceScale / playgroundParticles.velocityScale, playgroundParticles.turbulenceStrength, playgroundParticles.turbulenceApplyLifetimeStrength, playgroundParticles.turbulenceLifetimeStrength);
				}
			}
		});
	}

	public static void SendDeathEvents(PlaygroundParticlesC playgroundParticles, int p)
	{
		if ((playgroundParticles.playgroundCache.life[p] > 0f || playgroundParticles.playgroundCache.changedByPropertyDeath[p]) && !playgroundParticles.playgroundCache.isNonBirthed[p] && (playgroundParticles.loop || (!playgroundParticles.loop && playgroundParticles.playgroundCache.isFirstLoop[p])))
		{
			playgroundParticles.SendEvent(EVENTTYPEC.Death, p);
		}
		if (playgroundParticles.hasEventManipulatorLocal)
		{
			for (int i = 0; i < playgroundParticles.manipulators.Count; i++)
			{
				if (playgroundParticles.manipulators[i].trackParticles && playgroundParticles.manipulators[i].RemoveParticle(playgroundParticles.particleSystemId, p) && playgroundParticles.manipulators[i].sendEventDeath)
				{
					playgroundParticles.UpdateEventParticle(playgroundParticles.manipulators[i].manipulatorEventParticle, p);
					playgroundParticles.manipulators[i].SendParticleEventDeath();
				}
			}
		}
		if (playgroundParticles.hasEventManipulatorGlobal)
		{
			for (int j = 0; j < PlaygroundC.reference.manipulators.Count; j++)
			{
				if (PlaygroundC.reference.manipulators[j].trackParticles && PlaygroundC.reference.manipulators[j].RemoveParticle(playgroundParticles.particleSystemId, p) && PlaygroundC.reference.manipulators[j].sendEventDeath)
				{
					playgroundParticles.UpdateEventParticle(PlaygroundC.reference.manipulators[j].manipulatorEventParticle, p);
					PlaygroundC.reference.manipulators[j].SendParticleEventDeath();
				}
			}
		}
		playgroundParticles.playgroundCache.isNonBirthed[p] = true;
	}

	public static void Turbulence(PlaygroundParticlesC playgroundParticles, SimplexNoise turbulenceSimplex, int p, float t, TURBULENCETYPE turbulenceType, float turbulenceTimeScale, float turbulenceScale, float turbulenceStrength, bool turbulenceApplyLifetimeStrength, AnimationCurve turbulenceLifetimeStrength)
	{
		if (!playgroundParticles.calculate || playgroundParticles.inPlayback)
		{
			return;
		}
		float num = 1f;
		float num2 = 1f;
		float num3 = 2f;
		float num4 = 3f;
		if (!playgroundParticles.playgroundCache.simulate[p] || playgroundParticles.playgroundCache.position[p].y == PlaygroundC.initialTargetPosition.y)
		{
			return;
		}
		if (turbulenceType == TURBULENCETYPE.Simplex)
		{
			if (turbulenceTimeScale > 0f)
			{
				if (playgroundParticles.turbulenceApplyLifetimeStrength)
				{
					num = playgroundParticles.turbulenceLifetimeStrength.Evaluate(Mathf.Clamp01(playgroundParticles.playgroundCache.life[p] / (playgroundParticles.playgroundCache.death[p] - playgroundParticles.playgroundCache.birth[p] - playgroundParticles.playgroundCache.lifetimeSubtraction[p])));
				}
				if (num > 0f)
				{
					if (!playgroundParticles.axisConstraints.x)
					{
						playgroundParticles.playgroundCache.velocity[p].x += (float)turbulenceSimplex.noise((playgroundParticles.playgroundCache.position[p].x + num2) * turbulenceScale, (playgroundParticles.playgroundCache.position[p].y + num3) * turbulenceScale, (playgroundParticles.playgroundCache.position[p].z + num4) * turbulenceScale, playgroundParticles.localTime * turbulenceTimeScale) * turbulenceStrength * t * num;
					}
					if (!playgroundParticles.axisConstraints.y)
					{
						playgroundParticles.playgroundCache.velocity[p].y += (float)turbulenceSimplex.noise((playgroundParticles.playgroundCache.position[p].y + num3) * turbulenceScale, (playgroundParticles.playgroundCache.position[p].x + num2) * turbulenceScale, (playgroundParticles.playgroundCache.position[p].z + num4) * turbulenceScale, playgroundParticles.localTime * turbulenceTimeScale) * turbulenceStrength * t * num;
					}
					if (!playgroundParticles.axisConstraints.z)
					{
						playgroundParticles.playgroundCache.velocity[p].z += (float)turbulenceSimplex.noise((playgroundParticles.playgroundCache.position[p].z + num4) * turbulenceScale, (playgroundParticles.playgroundCache.position[p].x + num2) * turbulenceScale, (playgroundParticles.playgroundCache.position[p].y + num3) * turbulenceScale, playgroundParticles.localTime * turbulenceTimeScale) * turbulenceStrength * t * num;
					}
				}
			}
			else if (num > 0f)
			{
				if (!playgroundParticles.axisConstraints.x)
				{
					playgroundParticles.playgroundCache.velocity[p].x += (float)turbulenceSimplex.noise((playgroundParticles.playgroundCache.position[p].x + num2) * turbulenceScale, (playgroundParticles.playgroundCache.position[p].y + num3) * turbulenceScale, (playgroundParticles.playgroundCache.position[p].z + num4) * turbulenceScale) * turbulenceStrength * t * num;
				}
				if (!playgroundParticles.axisConstraints.y)
				{
					playgroundParticles.playgroundCache.velocity[p].y += (float)turbulenceSimplex.noise((playgroundParticles.playgroundCache.position[p].y + num3) * turbulenceScale, (playgroundParticles.playgroundCache.position[p].x + num2) * turbulenceScale, (playgroundParticles.playgroundCache.position[p].z + num4) * turbulenceScale) * turbulenceStrength * t * num;
				}
				if (!playgroundParticles.axisConstraints.z)
				{
					playgroundParticles.playgroundCache.velocity[p].z += (float)turbulenceSimplex.noise((playgroundParticles.playgroundCache.position[p].z + num4) * turbulenceScale, (playgroundParticles.playgroundCache.position[p].x + num2) * turbulenceScale, (playgroundParticles.playgroundCache.position[p].y + num3) * turbulenceScale) * turbulenceStrength * t * num;
				}
			}
		}
		else
		{
			if (!(playgroundParticles.turbulenceStrength > 0f))
			{
				return;
			}
			if (playgroundParticles.turbulenceApplyLifetimeStrength)
			{
				num = playgroundParticles.turbulenceLifetimeStrength.Evaluate(Mathf.Clamp01(playgroundParticles.playgroundCache.life[p] / (playgroundParticles.playgroundCache.death[p] - playgroundParticles.playgroundCache.birth[p] - playgroundParticles.playgroundCache.lifetimeSubtraction[p])));
			}
			if (num > 0f)
			{
				if (!playgroundParticles.axisConstraints.x)
				{
					playgroundParticles.playgroundCache.velocity[p].x += (Mathf.PerlinNoise(playgroundParticles.localTime * turbulenceTimeScale, playgroundParticles.playgroundCache.position[p].z * turbulenceScale) - 0.5f) * turbulenceStrength * t * num;
				}
				if (!playgroundParticles.axisConstraints.y)
				{
					playgroundParticles.playgroundCache.velocity[p].y += (Mathf.PerlinNoise(playgroundParticles.localTime * turbulenceTimeScale, playgroundParticles.playgroundCache.position[p].x * turbulenceScale) - 0.5f) * turbulenceStrength * t * num;
				}
				if (!playgroundParticles.axisConstraints.z)
				{
					playgroundParticles.playgroundCache.velocity[p].z += (Mathf.PerlinNoise(playgroundParticles.localTime * turbulenceTimeScale, playgroundParticles.playgroundCache.position[p].y * turbulenceScale) - 0.5f) * turbulenceStrength * t * num;
				}
			}
		}
	}

	public void CalculateCollisions()
	{
		Collisions(thisInstance);
	}

	public static void Collisions(PlaygroundParticlesC playgroundParticles)
	{
		if (!playgroundParticles.calculate || playgroundParticles.inPlayback || playgroundParticles.onlySourcePositioning || playgroundParticles.onlyLifetimePositioning || !(playgroundParticles.collisionRadius > 0f) || !playgroundParticles.hasEmitted)
		{
			return;
		}
		Ray ray = default;
		bool flag = playgroundParticles.collisionType == COLLISIONTYPEC.Physics3D;
		bool flag2 = playgroundParticles.collisionExclusion != null && playgroundParticles.collisionExclusion.Count > 0;
		bool flag3 = playgroundParticles.bounceRandomMin != Vector3.zero || playgroundParticles.bounceRandomMax != Vector3.zero;
		bool flag4 = playgroundParticles.events.Count > 0;
		if ((playgroundParticles.stickyCollisions || playgroundParticles.forceCollisionCaching) && playgroundParticles.collisionCache == null)
		{
			playgroundParticles.collisionCache = new CollisionCache(playgroundParticles.particleCount);
		}
		if (playgroundParticles.collision && playgroundParticles.colliders.Count > 0)
		{
			for (int i = 0; i < playgroundParticles.colliders.Count; i++)
			{
				playgroundParticles.colliders[i].UpdatePlane();
			}
		}
		if (playgroundParticles.playgroundCache.noForce.Length != playgroundParticles.particleCount)
		{
			playgroundParticles.playgroundCache.noForce = new bool[playgroundParticles.particleCount];
		}
		if (playgroundParticles.playgroundCache.lifetimeSubtraction.Length != playgroundParticles.particleCount)
		{
			playgroundParticles.playgroundCache.lifetimeSubtraction = new float[playgroundParticles.particleCount];
		}
		if (playgroundParticles.playgroundCache.lifetimeLoss == null || playgroundParticles.playgroundCache.lifetimeLoss.Length != playgroundParticles.particleCount)
		{
			playgroundParticles.playgroundCache.lifetimeLoss = new float[playgroundParticles.particleCount];
		}
		for (int j = 0; j < playgroundParticles.particleCount; j++)
		{
			if (playgroundParticles.playgroundCache.life[j] == 0f || playgroundParticles.playgroundCache.life[j] >= playgroundParticles.playgroundCache.death[j] - playgroundParticles.playgroundCache.birth[j] - playgroundParticles.playgroundCache.lifetimeSubtraction[j] || playgroundParticles.playgroundCache.noForce[j] || (playgroundParticles.maskedParticlesBypassCollision && playgroundParticles.playgroundCache.isMasked[j]))
			{
				continue;
			}
			if (playgroundParticles.stickyCollisions && playgroundParticles.collisionCache.hasCollided[j])
			{
				playgroundParticles.collisionCache.UpdateStickyPosition(j);
				ref Vector3 reference = ref playgroundParticles.playgroundCache.position[j];
				reference = playgroundParticles.collisionCache.stickyPosition[j];
				continue;
			}
			for (int k = 0; k < playgroundParticles.colliders.Count; k++)
			{
				if (!playgroundParticles.colliders[k].enabled || !playgroundParticles.colliders[k].transform || playgroundParticles.colliders[k].plane.GetSide(playgroundParticles.playgroundCache.position[j]))
				{
					continue;
				}
				ray.origin = playgroundParticles.playgroundCache.position[j];
				ray.direction = playgroundParticles.colliders[k].plane.normal;
				if (playgroundParticles.colliders[k].plane.Raycast(ray, out var enter))
				{
					ref Vector3 reference2 = ref playgroundParticles.playgroundCache.position[j];
					reference2 = ray.GetPoint(enter);
				}
				Vector3 preEventVelocity = playgroundParticles.playgroundCache.velocity[j];
				if (!playgroundParticles.stickyCollisions)
				{
					if (playgroundParticles.forceCollisionCaching)
					{
						playgroundParticles.collisionCache.Set(j, playgroundParticles.playgroundCache.position[j], playgroundParticles.colliders[k].plane.normal, playgroundParticles.colliders[k].transform);
					}
				}
				else
				{
					playgroundParticles.collisionCache.SetSticky(j, playgroundParticles.playgroundCache.position[j], playgroundParticles.colliders[k].plane.normal, playgroundParticles.stickyCollisionsSurfaceOffset, playgroundParticles.colliders[k].transform);
				}
				ref Vector3 reference3 = ref playgroundParticles.playgroundCache.velocity[j];
				reference3 = Vector3.Reflect(playgroundParticles.playgroundCache.velocity[j], (!flag3) ? playgroundParticles.colliders[k].plane.normal : (playgroundParticles.colliders[k].plane.normal + RandomVector3(playgroundParticles.internalRandom01, playgroundParticles.bounceRandomMin, playgroundParticles.bounceRandomMax))) * playgroundParticles.bounciness;
				if (playgroundParticles.lifetimeLoss > 0f)
				{
					playgroundParticles.playgroundCache.lifetimeLoss[j] = playgroundParticles.lifetime * playgroundParticles.lifetimeLoss;
					playgroundParticles.playgroundCache.changedByPropertyDeath[j] = true;
				}
				if (flag4)
				{
					playgroundParticles.SendEvent(EVENTTYPEC.Collision, j, preEventVelocity, playgroundParticles.colliders[k].transform);
				}
			}
			if (playgroundParticles.playgroundCache.velocity[j].magnitude > PlaygroundC.collisionSleepVelocity)
			{
				if (flag)
				{
					if (!Physics.Raycast(playgroundParticles.playgroundCache.collisionParticlePosition[j], playgroundParticles.playgroundCache.position[j] - playgroundParticles.playgroundCache.collisionParticlePosition[j], out var hitInfo, (!playgroundParticles.collisionPrecision) ? ((Vector3.SqrMagnitude(playgroundParticles.playgroundCache.position[j] - playgroundParticles.playgroundCache.collisionParticlePosition[j]) + playgroundParticles.collisionRadius) * 2f) : ((Vector3.Distance(playgroundParticles.playgroundCache.position[j], playgroundParticles.playgroundCache.collisionParticlePosition[j]) + playgroundParticles.collisionRadius) * 2f), playgroundParticles.collisionMask) || (flag2 && playgroundParticles.collisionExclusion.Contains(hitInfo.transform)))
					{
						continue;
					}
					ref Vector3 reference4 = ref playgroundParticles.playgroundCache.position[j];
					reference4 = playgroundParticles.playgroundCache.collisionParticlePosition[j];
					if (!playgroundParticles.stickyCollisions)
					{
						if (playgroundParticles.forceCollisionCaching)
						{
							playgroundParticles.collisionCache.Set(j, hitInfo.point, hitInfo.normal, hitInfo.transform);
						}
					}
					else if ((playgroundParticles.stickyCollisionsMask.value & (1 << hitInfo.transform.gameObject.layer)) != 0)
					{
						playgroundParticles.collisionCache.SetSticky(j, hitInfo.point, hitInfo.normal, playgroundParticles.stickyCollisionsSurfaceOffset, hitInfo.transform);
					}
					Vector3 vector = playgroundParticles.playgroundCache.velocity[j];
					ref Vector3 reference5 = ref playgroundParticles.playgroundCache.velocity[j];
					reference5 = Vector3.Reflect(playgroundParticles.playgroundCache.velocity[j], (!flag3) ? hitInfo.normal : (hitInfo.normal + RandomVector3(playgroundParticles.internalRandom01, playgroundParticles.bounceRandomMin, playgroundParticles.bounceRandomMax))) * playgroundParticles.bounciness;
					if (playgroundParticles.lifetimeLoss > 0f)
					{
						playgroundParticles.playgroundCache.lifetimeLoss[j] = playgroundParticles.lifetime * playgroundParticles.lifetimeLoss;
						playgroundParticles.playgroundCache.changedByPropertyDeath[j] = true;
					}
					if (playgroundParticles.affectRigidbodies && (bool)hitInfo.rigidbody)
					{
						hitInfo.rigidbody.AddForceAtPosition(((!playgroundParticles.inverseRigidbodyCollision) ? vector : (-vector)) * playgroundParticles.mass, playgroundParticles.playgroundCache.position[j]);
					}
					if (flag4)
					{
						playgroundParticles.SendEvent(EVENTTYPEC.Collision, j, vector, hitInfo.transform, hitInfo.collider);
					}
					if (playgroundParticles.hasEventManipulatorLocal)
					{
						for (int l = 0; l < playgroundParticles.manipulators.Count; l++)
						{
							if (playgroundParticles.manipulators[l].trackParticles && playgroundParticles.manipulators[l].sendEventCollision && playgroundParticles.manipulators[l].Contains(playgroundParticles.playgroundCache.position[j], playgroundParticles.manipulators[l].transform.position))
							{
								playgroundParticles.UpdateEventParticle(playgroundParticles.manipulators[l].manipulatorEventParticle, j);
								playgroundParticles.manipulators[l].manipulatorEventParticle.collisionCollider = hitInfo.collider;
								playgroundParticles.manipulators[l].manipulatorEventParticle.collisionParticlePosition = hitInfo.point;
								playgroundParticles.manipulators[l].manipulatorEventParticle.collisionTransform = hitInfo.transform;
								playgroundParticles.manipulators[l].SendParticleEventCollision();
							}
						}
					}
					if (!playgroundParticles.hasEventManipulatorGlobal)
					{
						continue;
					}
					for (int m = 0; m < PlaygroundC.reference.manipulators.Count; m++)
					{
						if (PlaygroundC.reference.manipulators[m].trackParticles && PlaygroundC.reference.manipulators[m].sendEventCollision && PlaygroundC.reference.manipulators[m].Contains(playgroundParticles.playgroundCache.position[j], PlaygroundC.reference.manipulators[m].transform.position))
						{
							playgroundParticles.UpdateEventParticle(PlaygroundC.reference.manipulators[m].manipulatorEventParticle, j);
							PlaygroundC.reference.manipulators[m].manipulatorEventParticle.collisionCollider = hitInfo.collider;
							PlaygroundC.reference.manipulators[m].manipulatorEventParticle.collisionParticlePosition = hitInfo.point;
							PlaygroundC.reference.manipulators[m].manipulatorEventParticle.collisionTransform = hitInfo.transform;
							PlaygroundC.reference.manipulators[m].SendParticleEventCollision();
						}
					}
					continue;
				}
				RaycastHit2D raycastHit2D = Physics2D.Raycast(playgroundParticles.playgroundCache.collisionParticlePosition[j], playgroundParticles.playgroundCache.position[j] - playgroundParticles.playgroundCache.collisionParticlePosition[j], (!playgroundParticles.collisionPrecision) ? ((Vector3.SqrMagnitude(playgroundParticles.playgroundCache.position[j] - playgroundParticles.playgroundCache.collisionParticlePosition[j]) + playgroundParticles.collisionRadius) * 2f) : ((Vector3.Distance(playgroundParticles.playgroundCache.position[j], playgroundParticles.playgroundCache.collisionParticlePosition[j]) + playgroundParticles.collisionRadius) * 2f), playgroundParticles.collisionMask, playgroundParticles.minCollisionDepth, playgroundParticles.maxCollisionDepth);
				if (!(raycastHit2D.collider != null) || (flag2 && playgroundParticles.collisionExclusion.Contains(raycastHit2D.transform)))
				{
					continue;
				}
				ref Vector3 reference6 = ref playgroundParticles.playgroundCache.position[j];
				reference6 = playgroundParticles.playgroundCache.collisionParticlePosition[j];
				if (!playgroundParticles.stickyCollisions)
				{
					if (playgroundParticles.forceCollisionCaching)
					{
						playgroundParticles.collisionCache.Set(j, raycastHit2D.point, raycastHit2D.normal, raycastHit2D.transform);
					}
				}
				else if ((playgroundParticles.stickyCollisionsMask.value & (1 << raycastHit2D.transform.gameObject.layer)) != 0)
				{
					playgroundParticles.collisionCache.SetSticky(j, raycastHit2D.point, raycastHit2D.normal, playgroundParticles.stickyCollisionsSurfaceOffset, raycastHit2D.transform);
				}
				Vector3 vector2 = playgroundParticles.playgroundCache.velocity[j];
				ref Vector3 reference7 = ref playgroundParticles.playgroundCache.velocity[j];
				reference7 = Vector3.Reflect(playgroundParticles.playgroundCache.velocity[j], (!flag3) ? ((Vector3)raycastHit2D.normal) : ((Vector3)raycastHit2D.normal + RandomVector3(playgroundParticles.internalRandom01, playgroundParticles.bounceRandomMin, playgroundParticles.bounceRandomMax))) * playgroundParticles.bounciness;
				if (playgroundParticles.lifetimeLoss > 0f)
				{
					playgroundParticles.playgroundCache.lifetimeLoss[j] = playgroundParticles.lifetime * playgroundParticles.lifetimeLoss;
					playgroundParticles.playgroundCache.changedByPropertyDeath[j] = true;
				}
				if (playgroundParticles.affectRigidbodies && (bool)raycastHit2D.rigidbody)
				{
					raycastHit2D.rigidbody.AddForceAtPosition(((!playgroundParticles.inverseRigidbodyCollision) ? vector2 : (-vector2)) * playgroundParticles.mass, playgroundParticles.playgroundCache.position[j]);
				}
				if (flag4)
				{
					playgroundParticles.SendEvent(EVENTTYPEC.Collision, j, vector2, raycastHit2D.transform, raycastHit2D.collider);
				}
				if (playgroundParticles.hasEventManipulatorLocal)
				{
					for (int n = 0; n < playgroundParticles.manipulators.Count; n++)
					{
						if (playgroundParticles.manipulators[n].trackParticles && playgroundParticles.manipulators[n].sendEventCollision && playgroundParticles.manipulators[n].Contains(playgroundParticles.playgroundCache.position[j], playgroundParticles.manipulators[n].transform.position))
						{
							playgroundParticles.UpdateEventParticle(playgroundParticles.manipulators[n].manipulatorEventParticle, j);
							playgroundParticles.manipulators[n].manipulatorEventParticle.collisionCollider2D = raycastHit2D.collider;
							playgroundParticles.manipulators[n].manipulatorEventParticle.collisionParticlePosition = raycastHit2D.point;
							playgroundParticles.manipulators[n].manipulatorEventParticle.collisionTransform = raycastHit2D.transform;
							playgroundParticles.manipulators[n].SendParticleEventCollision();
						}
					}
				}
				if (!playgroundParticles.hasEventManipulatorGlobal)
				{
					continue;
				}
				for (int num = 0; num < PlaygroundC.reference.manipulators.Count; num++)
				{
					if (PlaygroundC.reference.manipulators[num].trackParticles && PlaygroundC.reference.manipulators[num].sendEventCollision && PlaygroundC.reference.manipulators[num].Contains(playgroundParticles.playgroundCache.position[j], PlaygroundC.reference.manipulators[num].transform.position))
					{
						playgroundParticles.UpdateEventParticle(PlaygroundC.reference.manipulators[num].manipulatorEventParticle, j);
						playgroundParticles.manipulators[num].manipulatorEventParticle.collisionCollider2D = raycastHit2D.collider;
						playgroundParticles.manipulators[num].manipulatorEventParticle.collisionParticlePosition = raycastHit2D.point;
						playgroundParticles.manipulators[num].manipulatorEventParticle.collisionTransform = raycastHit2D.transform;
						PlaygroundC.reference.manipulators[num].SendParticleEventCollision();
					}
				}
			}
			else
			{
				ref Vector3 reference8 = ref playgroundParticles.playgroundCache.velocity[j];
				reference8 = Vector3.zero;
			}
		}
	}

	public static Vector3 GetOverflow2(Vector3 overflow, int currentVal, int maxVal)
	{
		float num = currentVal / maxVal;
		return new Vector3(overflow.x * num, overflow.y * num, overflow.z * num);
	}

	public static Vector3 GetOverflow2(Vector3 overflow, Vector3 direction, int currentVal, int maxVal)
	{
		float num = currentVal / maxVal;
		return new Vector3(direction.x * overflow.x * num, direction.y * overflow.y * num, direction.z * overflow.z * num);
	}

	public static Vector3 Vector3Scale(Vector3 v1, Vector3 v2)
	{
		return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
	}

	public static void CalculateManipulator(PlaygroundParticlesC playgroundParticles, ManipulatorObjectC thisManipulator, int p, float t, float life, Vector3 particlePosition, Vector3 manipulatorPosition, bool localSpace)
	{
		if (!thisManipulator.enabled || !thisManipulator.transform.available || thisManipulator.strength == 0f || !thisManipulator.LifetimeFilter(life, playgroundParticles.lifetime) || !thisManipulator.ParticleFilter(p, playgroundParticles.particleCount))
		{
			return;
		}
		bool flag = thisManipulator.Contains((!localSpace) ? particlePosition : (playgroundParticles.particleSystemRotation * particlePosition), (!localSpace) ? manipulatorPosition : (playgroundParticles.particleSystemRotation * manipulatorPosition));
		if (flag && (playgroundParticles.playgroundCache.excludeFromManipulatorId[p] == thisManipulator.manipulatorId || (thisManipulator.nonAffectedParticles.Count > 0 && thisManipulator.ContainsNonAffectedParticle(playgroundParticles.particleSystemId, p))))
		{
			return;
		}
		if (thisManipulator.trackParticles)
		{
			if (flag)
			{
				if ((thisManipulator.trackingMethod == TrackingMethod.ManipulatorId && !thisManipulator.IsSameId(playgroundParticles.playgroundCache.manipulatorId[p])) || (thisManipulator.trackingMethod == TrackingMethod.ParticleId && !thisManipulator.ContainsParticle(playgroundParticles.particleSystemId, p)))
				{
					playgroundParticles.playgroundCache.manipulatorId[p] = thisManipulator.manipulatorId;
					thisManipulator.AddParticle(playgroundParticles.particleSystemId, p);
					if (thisManipulator.sendEventEnter)
					{
						playgroundParticles.UpdateEventParticle(thisManipulator.manipulatorEventParticle, p);
						thisManipulator.SendParticleEventEnter();
					}
				}
			}
			else if ((thisManipulator.trackingMethod == TrackingMethod.ManipulatorId && thisManipulator.IsSameId(playgroundParticles.playgroundCache.manipulatorId[p])) || (thisManipulator.trackingMethod == TrackingMethod.ParticleId && thisManipulator.ContainsParticle(playgroundParticles.particleSystemId, p)))
			{
				playgroundParticles.playgroundCache.manipulatorId[p] = 0;
				thisManipulator.RemoveParticle(playgroundParticles.particleSystemId, p);
				if (thisManipulator.sendEventExit)
				{
					playgroundParticles.UpdateEventParticle(thisManipulator.manipulatorEventParticle, p);
					thisManipulator.SendParticleEventExit();
				}
			}
		}
		float num = 0f;
		float num2 = ((!thisManipulator.applyParticleLifetimeStrength) ? thisManipulator.strength : (thisManipulator.strength * thisManipulator.particleLifetimeStrength.Evaluate((life - playgroundParticles.playgroundCache.lifetimeSubtraction[p]) / playgroundParticles.lifetime)));
		bool flag2 = thisManipulator.axisConstraints.HasConstraints();
		Vector3 vector = playgroundParticles.playgroundCache.velocity[p];
		if (!playgroundParticles.onlySourcePositioning && !playgroundParticles.playgroundCache.noForce[p])
		{
			if (thisManipulator.type == MANIPULATORTYPEC.Attractor)
			{
				if (flag)
				{
					num = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.Distance(manipulatorPosition, playgroundParticles.playgroundCache.position[p]) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference = ref playgroundParticles.playgroundCache.velocity[p];
					reference = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition - particlePosition) * (num2 / num), t * (num2 / num) / thisManipulator.strengthSmoothing);
				}
			}
			else if (thisManipulator.type == MANIPULATORTYPEC.AttractorGravitational)
			{
				if (flag)
				{
					num = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.Distance(manipulatorPosition, playgroundParticles.playgroundCache.position[p]) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference2 = ref playgroundParticles.playgroundCache.velocity[p];
					reference2 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition - particlePosition) * num2 / num, t / thisManipulator.strengthSmoothing);
				}
			}
			else if (thisManipulator.type == MANIPULATORTYPEC.Repellent)
			{
				if (flag)
				{
					num = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.Distance(manipulatorPosition, playgroundParticles.playgroundCache.position[p]) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference3 = ref playgroundParticles.playgroundCache.velocity[p];
					reference3 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (particlePosition - manipulatorPosition) * (num2 / num), t * (num2 / num) / thisManipulator.strengthSmoothing);
				}
			}
			else if (thisManipulator.type == MANIPULATORTYPEC.Vortex && flag)
			{
				num = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.Distance(manipulatorPosition, playgroundParticles.playgroundCache.position[p]) / thisManipulator.strengthDistanceEffect));
				ref Vector3 reference4 = ref playgroundParticles.playgroundCache.velocity[p];
				reference4 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition - particlePosition) * num2 / num - Vector3.Cross(thisManipulator.transform.up, manipulatorPosition - particlePosition) * num2 / num, t * num2 / num / thisManipulator.strengthSmoothing);
			}
		}
		if (thisManipulator.type == MANIPULATORTYPEC.Property)
		{
			num = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
			PropertyManipulator(playgroundParticles, thisManipulator, thisManipulator.property, num2, p, t, particlePosition, manipulatorPosition, num, localSpace, flag);
		}
		else if (thisManipulator.type == MANIPULATORTYPEC.Combined)
		{
			num = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
			for (int i = 0; i < thisManipulator.properties.Count; i++)
			{
				PropertyManipulator(playgroundParticles, thisManipulator, thisManipulator.properties[i], num2, p, t, particlePosition, manipulatorPosition, num, localSpace, flag);
			}
		}
		if (flag2)
		{
			ref Vector3 reference5 = ref playgroundParticles.playgroundCache.velocity[p];
			reference5 = new Vector3((!thisManipulator.axisConstraints.x) ? playgroundParticles.playgroundCache.velocity[p].x : vector.x, (!thisManipulator.axisConstraints.y) ? playgroundParticles.playgroundCache.velocity[p].y : vector.y, (!thisManipulator.axisConstraints.z) ? playgroundParticles.playgroundCache.velocity[p].z : vector.z);
			ref Vector3 reference6 = ref playgroundParticles.playgroundCache.position[p];
			reference6 = new Vector3((!thisManipulator.axisConstraints.x) ? playgroundParticles.playgroundCache.position[p].x : particlePosition.x, (!thisManipulator.axisConstraints.y) ? playgroundParticles.playgroundCache.position[p].y : particlePosition.y, (!thisManipulator.axisConstraints.z) ? playgroundParticles.playgroundCache.position[p].z : particlePosition.z);
		}
	}

	public static void PropertyManipulator(PlaygroundParticlesC playgroundParticles, ManipulatorObjectC thisManipulator, ManipulatorPropertyC thisManipulatorProperty, float manipulatorStrength, int p, float t, Vector3 particlePosition, Vector3 manipulatorPosition, float manipulatorDistance, bool localSpace, bool contains)
	{
		if (contains)
		{
			switch (thisManipulatorProperty.type)
			{
			case MANIPULATORPROPERTYTYPEC.Math:
				if (thisManipulatorProperty.mathProperty.type == MATHMANIPULATORTYPE.Sin || thisManipulatorProperty.mathProperty.type == MATHMANIPULATORTYPE.Cos)
				{
					t = playgroundParticles.localTime;
				}
				switch (thisManipulatorProperty.mathProperty.property)
				{
				case MATHMANIPULATORPROPERTY.Position:
				{
					ref Vector3 reference6 = ref playgroundParticles.playgroundCache.position[p];
					reference6 = thisManipulatorProperty.mathProperty.EvaluatePosition(playgroundParticles.playgroundCache.position[p], t);
					break;
				}
				case MATHMANIPULATORPROPERTY.Velocity:
				{
					ref Vector3 reference7 = ref playgroundParticles.playgroundCache.velocity[p];
					reference7 = thisManipulatorProperty.mathProperty.Evaluate(playgroundParticles.playgroundCache.velocity[p], t);
					break;
				}
				case MATHMANIPULATORPROPERTY.Rotation:
					if (!playgroundParticles.rotateTowardsDirection)
					{
						playgroundParticles.playgroundCache.rotation[p] = thisManipulatorProperty.mathProperty.Evaluate(playgroundParticles.playgroundCache.rotation[p], t) % 360f;
					}
					break;
				case MATHMANIPULATORPROPERTY.Size:
					playgroundParticles.playgroundCache.size[p] = thisManipulatorProperty.mathProperty.Evaluate(playgroundParticles.playgroundCache.size[p], t);
					playgroundParticles.playgroundCache.changedByPropertySize[p] = true;
					break;
				}
				break;
			case MANIPULATORPROPERTYTYPEC.Velocity:
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
				{
					ref Vector3 reference20 = ref playgroundParticles.playgroundCache.velocity[p];
					reference20 = ((!thisManipulatorProperty.useLocalRotation) ? thisManipulatorProperty.velocity : thisManipulatorProperty.localVelocity);
				}
				else
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference21 = ref playgroundParticles.playgroundCache.velocity[p];
					reference21 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (!thisManipulatorProperty.useLocalRotation) ? thisManipulatorProperty.velocity : thisManipulatorProperty.localVelocity, t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing);
				}
				break;
			case MANIPULATORPROPERTYTYPEC.AdditiveVelocity:
				manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
				playgroundParticles.playgroundCache.velocity[p] += ((!thisManipulatorProperty.useLocalRotation) ? (thisManipulatorProperty.velocity * (t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing)) : (thisManipulatorProperty.localVelocity * (t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing)));
				break;
			case MANIPULATORPROPERTYTYPEC.Color:
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
				{
					if (thisManipulatorProperty.keepColorAlphas)
					{
						Color color = thisManipulatorProperty.color;
						color.a = Mathf.Clamp(playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p] / playgroundParticles.lifetime).a, 0f, color.a);
						playgroundParticles.SetParticleColorInternal(p, color);
					}
					else
					{
						playgroundParticles.SetParticleColorInternal(p, thisManipulatorProperty.color);
					}
				}
				else
				{
					if (thisManipulatorProperty.keepColorAlphas)
					{
						manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
						Color color = thisManipulatorProperty.color;
						color.a = Mathf.Clamp(playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p] / playgroundParticles.lifetime).a, 0f, color.a);
						playgroundParticles.SetParticleColorInternal(p, Color.Lerp(playgroundParticles.particleCache[p].startColor, color, t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing));
					}
					else
					{
						playgroundParticles.SetParticleColorInternal(p, Color.Lerp(playgroundParticles.particleCache[p].startColor, thisManipulatorProperty.color, t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing));
					}
					playgroundParticles.playgroundCache.changedByPropertyColorLerp[p] = true;
				}
				playgroundParticles.playgroundCache.changedByPropertyColor[p] = true;
				if (thisManipulatorProperty.keepColorAlphas)
				{
					playgroundParticles.playgroundCache.changedByPropertyColorKeepAlpha[p] = true;
				}
				if (playgroundParticles.playgroundCache.propertyColorId[p] != thisManipulator.manipulatorId)
				{
					playgroundParticles.playgroundCache.propertyColorId[p] = thisManipulator.manipulatorId;
				}
				break;
			case MANIPULATORPROPERTYTYPEC.LifetimeColor:
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
				{
					playgroundParticles.SetParticleColorInternal(p, thisManipulatorProperty.lifetimeColor.Evaluate((!(playgroundParticles.lifetime > 0f)) ? 0f : (playgroundParticles.playgroundCache.life[p] / playgroundParticles.lifetime)));
				}
				else
				{
					playgroundParticles.SetParticleColorInternal(p, Color.Lerp(playgroundParticles.particleCache[p].startColor, thisManipulatorProperty.lifetimeColor.Evaluate((!(playgroundParticles.lifetime > 0f)) ? 0f : (playgroundParticles.playgroundCache.life[p] / playgroundParticles.lifetime)), t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing));
					playgroundParticles.playgroundCache.changedByPropertyColorLerp[p] = true;
				}
				playgroundParticles.playgroundCache.changedByPropertyColor[p] = true;
				if (playgroundParticles.playgroundCache.propertyColorId[p] != thisManipulator.manipulatorId)
				{
					playgroundParticles.playgroundCache.propertyColorId[p] = thisManipulator.manipulatorId;
				}
				break;
			case MANIPULATORPROPERTYTYPEC.Size:
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 0f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					if (manipulatorDistance == 0f)
					{
						playgroundParticles.playgroundCache.size[p] = thisManipulatorProperty.size;
					}
					else
					{
						playgroundParticles.playgroundCache.size[p] = thisManipulatorProperty.size + thisManipulatorProperty.size / (1f + manipulatorDistance / thisManipulator.strengthSmoothing) * (thisManipulator.strength * thisManipulatorProperty.strength);
					}
				}
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					playgroundParticles.playgroundCache.size[p] = Mathf.Lerp(playgroundParticles.playgroundCache.size[p], thisManipulatorProperty.size, t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing);
				}
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					playgroundParticles.playgroundCache.size[p] = Mathf.MoveTowards(playgroundParticles.playgroundCache.size[p], thisManipulatorProperty.size, t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing);
				}
				playgroundParticles.playgroundCache.changedByPropertySize[p] = true;
				break;
			case MANIPULATORPROPERTYTYPEC.Target:
				if (thisManipulatorProperty.targets.Count <= 0 || !thisManipulatorProperty.targets[thisManipulatorProperty.targetPointer].available)
				{
					break;
				}
				if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId)
				{
					playgroundParticles.playgroundCache.propertyTarget[p] = thisManipulatorProperty.targetPointer;
					thisManipulatorProperty.targetPointer++;
					thisManipulatorProperty.targetPointer %= thisManipulatorProperty.targets.Count;
					playgroundParticles.playgroundCache.propertyId[p] = thisManipulator.manipulatorId;
				}
				if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId || !thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.targets.Count].available)
				{
					break;
				}
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
				{
					ref Vector3 reference15 = ref playgroundParticles.playgroundCache.position[p];
					reference15 = ((!localSpace) ? thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.targets.Count].position : thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.targets.Count].localPosition);
				}
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference16 = ref playgroundParticles.playgroundCache.position[p];
					reference16 = ((!localSpace) ? Vector3.Lerp(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.targets.Count].position, t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing) : Vector3.Lerp(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.targets.Count].localPosition, t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing));
					if (thisManipulatorProperty.zeroVelocityStrength > 0f)
					{
						ref Vector3 reference17 = ref playgroundParticles.playgroundCache.velocity[p];
						reference17 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
					}
				}
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference18 = ref playgroundParticles.playgroundCache.position[p];
					reference18 = ((!localSpace) ? Vector3.MoveTowards(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.targets.Count].position, t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing) : Vector3.MoveTowards(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.targets.Count].localPosition, t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing));
					if (thisManipulatorProperty.zeroVelocityStrength > 0f)
					{
						ref Vector3 reference19 = ref playgroundParticles.playgroundCache.velocity[p];
						reference19 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
					}
				}
				playgroundParticles.playgroundCache.changedByPropertyTarget[p] = true;
				break;
			case MANIPULATORPROPERTYTYPEC.Death:
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
				{
					playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime;
				}
				else
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					playgroundParticles.playgroundCache.birth[p] -= t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing;
				}
				playgroundParticles.playgroundCache.changedByPropertyDeath[p] = true;
				break;
			case MANIPULATORPROPERTYTYPEC.Attractor:
				if (!playgroundParticles.onlySourcePositioning)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.Distance(manipulatorPosition, playgroundParticles.playgroundCache.position[p]) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference22 = ref playgroundParticles.playgroundCache.velocity[p];
					reference22 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition - particlePosition) * (thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance), t * (thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance) / thisManipulator.strengthSmoothing);
				}
				break;
			case MANIPULATORPROPERTYTYPEC.Gravitational:
				if (!playgroundParticles.onlySourcePositioning)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.Distance(manipulatorPosition, playgroundParticles.playgroundCache.position[p]) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference29 = ref playgroundParticles.playgroundCache.velocity[p];
					reference29 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition - particlePosition) * (thisManipulatorProperty.strength * manipulatorStrength) / manipulatorDistance, t / thisManipulator.strengthSmoothing);
				}
				break;
			case MANIPULATORPROPERTYTYPEC.Repellent:
				if (!playgroundParticles.onlySourcePositioning)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.Distance(manipulatorPosition, playgroundParticles.playgroundCache.position[p]) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference14 = ref playgroundParticles.playgroundCache.velocity[p];
					reference14 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (particlePosition - manipulatorPosition) * (thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance), t * (thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance) / thisManipulator.strengthSmoothing);
				}
				break;
			case MANIPULATORPROPERTYTYPEC.Vortex:
				if (!playgroundParticles.onlySourcePositioning)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.Distance(manipulatorPosition, playgroundParticles.playgroundCache.position[p]) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference23 = ref playgroundParticles.playgroundCache.velocity[p];
					reference23 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition - particlePosition) * (manipulatorStrength * thisManipulatorProperty.strength) / manipulatorDistance - Vector3.Cross(thisManipulator.transform.up, manipulatorPosition - particlePosition) * (manipulatorStrength * thisManipulatorProperty.strength) / manipulatorDistance, t * (manipulatorStrength * thisManipulatorProperty.strength) / manipulatorDistance / thisManipulator.strengthSmoothing);
				}
				break;
			case MANIPULATORPROPERTYTYPEC.Turbulence:
				if (!playgroundParticles.onlySourcePositioning && thisManipulatorProperty.turbulenceType != TURBULENCETYPE.None)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.Distance(manipulatorPosition, particlePosition) / thisManipulator.strengthDistanceEffect));
					Turbulence(playgroundParticles, thisManipulatorProperty.turbulenceSimplex, p, t / thisManipulator.strengthSmoothing, thisManipulatorProperty.turbulenceType, thisManipulatorProperty.turbulenceTimeScale, thisManipulatorProperty.turbulenceScale, thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance, thisManipulatorProperty.turbulenceApplyLifetimeStrength, thisManipulatorProperty.turbulenceLifetimeStrength);
				}
				break;
			case MANIPULATORPROPERTYTYPEC.MeshTarget:
				if (playgroundParticles.onlySourcePositioning || !thisManipulatorProperty.meshTarget.initialized)
				{
					break;
				}
				if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId)
				{
					playgroundParticles.playgroundCache.propertyTarget[p] = thisManipulatorProperty.targetSortingList[thisManipulatorProperty.targetPointer];
					thisManipulatorProperty.targetPointer++;
					thisManipulatorProperty.targetPointer %= thisManipulatorProperty.meshTarget.vertexPositions.Length;
					playgroundParticles.playgroundCache.propertyId[p] = thisManipulator.manipulatorId;
				}
				if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId)
				{
					break;
				}
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
				{
					ref Vector3 reference30 = ref playgroundParticles.playgroundCache.position[p];
					reference30 = thisManipulatorProperty.meshTargetMatrix.MultiplyPoint3x4(thisManipulatorProperty.meshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.meshTarget.vertexPositions.Length]);
					ref Vector3 reference31 = ref playgroundParticles.playgroundCache.velocity[p];
					reference31 = Vector3.zero;
				}
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
				{
					ref Vector3 reference32 = ref playgroundParticles.playgroundCache.position[p];
					reference32 = Vector3.Lerp(particlePosition, thisManipulatorProperty.meshTargetMatrix.MultiplyPoint3x4(thisManipulatorProperty.meshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.meshTarget.vertexPositions.Length]), t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing);
					if (thisManipulatorProperty.zeroVelocityStrength > 0f)
					{
						ref Vector3 reference33 = ref playgroundParticles.playgroundCache.velocity[p];
						reference33 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
					}
				}
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear)
				{
					ref Vector3 reference34 = ref playgroundParticles.playgroundCache.position[p];
					reference34 = Vector3.MoveTowards(particlePosition, thisManipulatorProperty.meshTargetMatrix.MultiplyPoint3x4(thisManipulatorProperty.meshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.meshTarget.vertexPositions.Length]), t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing);
					if (thisManipulatorProperty.zeroVelocityStrength > 0f)
					{
						ref Vector3 reference35 = ref playgroundParticles.playgroundCache.velocity[p];
						reference35 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
					}
				}
				playgroundParticles.playgroundCache.changedByPropertyTarget[p] = true;
				break;
			case MANIPULATORPROPERTYTYPEC.SkinnedMeshTarget:
				if (playgroundParticles.onlySourcePositioning || !thisManipulatorProperty.skinnedMeshTarget.initialized)
				{
					break;
				}
				if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId)
				{
					playgroundParticles.playgroundCache.propertyTarget[p] = thisManipulatorProperty.targetSortingList[thisManipulatorProperty.targetPointer];
					thisManipulatorProperty.targetPointer++;
					thisManipulatorProperty.targetPointer %= thisManipulatorProperty.skinnedMeshTarget.vertexPositions.Length;
					playgroundParticles.playgroundCache.propertyId[p] = thisManipulator.manipulatorId;
				}
				if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId)
				{
					break;
				}
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
				{
					ref Vector3 reference8 = ref playgroundParticles.playgroundCache.position[p];
					reference8 = thisManipulatorProperty.skinnedMeshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.skinnedMeshTarget.vertexPositions.Length];
					ref Vector3 reference9 = ref playgroundParticles.playgroundCache.velocity[p];
					reference9 = Vector3.zero;
				}
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference10 = ref playgroundParticles.playgroundCache.position[p];
					reference10 = Vector3.Lerp(particlePosition, thisManipulatorProperty.skinnedMeshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.skinnedMeshTarget.vertexPositions.Length], t * (thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance) / thisManipulator.strengthSmoothing);
					if (thisManipulatorProperty.zeroVelocityStrength > 0f)
					{
						ref Vector3 reference11 = ref playgroundParticles.playgroundCache.velocity[p];
						reference11 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
					}
				}
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference12 = ref playgroundParticles.playgroundCache.position[p];
					reference12 = Vector3.MoveTowards(particlePosition, thisManipulatorProperty.skinnedMeshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.skinnedMeshTarget.vertexPositions.Length], t * (thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance) / thisManipulator.strengthSmoothing);
					if (thisManipulatorProperty.zeroVelocityStrength > 0f)
					{
						ref Vector3 reference13 = ref playgroundParticles.playgroundCache.velocity[p];
						reference13 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
					}
				}
				playgroundParticles.playgroundCache.changedByPropertyTarget[p] = true;
				break;
			case MANIPULATORPROPERTYTYPEC.StateTarget:
			{
				if (playgroundParticles.onlySourcePositioning || !thisManipulatorProperty.stateTarget.initialized || thisManipulatorProperty.stateTarget.IsInitializing())
				{
					break;
				}
				if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId)
				{
					playgroundParticles.playgroundCache.propertyTarget[p] = thisManipulatorProperty.targetSortingList[thisManipulatorProperty.targetPointer % thisManipulatorProperty.targetSortingList.Length];
					thisManipulatorProperty.targetPointer++;
					thisManipulatorProperty.targetPointer %= thisManipulatorProperty.stateTarget.positionLength;
					playgroundParticles.playgroundCache.propertyId[p] = thisManipulator.manipulatorId;
				}
				if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId)
				{
					break;
				}
				Vector3 vector = thisManipulatorProperty.stateTarget.stateTransformMx.MultiplyPoint3x4(thisManipulatorProperty.stateTarget.GetPosition(playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.stateTarget.positionLength));
				if (thisManipulatorProperty.onlyPositionInRange && thisManipulator.shape != MANIPULATORSHAPEC.Infinite && (!thisManipulatorProperty.onlyPositionInRange || !thisManipulator.Contains(vector, manipulatorPosition)))
				{
					break;
				}
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
				{
					playgroundParticles.playgroundCache.position[p] = vector;
					ref Vector3 reference24 = ref playgroundParticles.playgroundCache.velocity[p];
					reference24 = Vector3.zero;
				}
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference25 = ref playgroundParticles.playgroundCache.position[p];
					reference25 = Vector3.Lerp(particlePosition, vector, t * (thisManipulatorProperty.strength * thisManipulator.strength / manipulatorDistance) / thisManipulator.strengthSmoothing);
					if (thisManipulatorProperty.zeroVelocityStrength > 0f)
					{
						ref Vector3 reference26 = ref playgroundParticles.playgroundCache.velocity[p];
						reference26 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
					}
				}
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference27 = ref playgroundParticles.playgroundCache.position[p];
					reference27 = Vector3.MoveTowards(particlePosition, vector, t * (thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance) / thisManipulator.strengthSmoothing);
					if (thisManipulatorProperty.zeroVelocityStrength > 0f)
					{
						ref Vector3 reference28 = ref playgroundParticles.playgroundCache.velocity[p];
						reference28 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
					}
				}
				playgroundParticles.playgroundCache.changedByPropertyTarget[p] = true;
				break;
			}
			case MANIPULATORPROPERTYTYPEC.SplineTarget:
			{
				if (playgroundParticles.onlySourcePositioning || !thisManipulatorProperty.SplineTargetIsReady())
				{
					break;
				}
				if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId)
				{
					playgroundParticles.playgroundCache.propertyId[p] = thisManipulator.manipulatorId;
				}
				if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId)
				{
					break;
				}
				float a = ((thisManipulatorProperty.splineTargetMethod != SPLINETARGETMETHOD.SplineTime) ? Mathf.Clamp01(playgroundParticles.playgroundCache.life[p] / (playgroundParticles.playgroundCache.death[p] - playgroundParticles.playgroundCache.birth[p] - playgroundParticles.playgroundCache.lifetimeSubtraction[p]) + thisManipulatorProperty.splineTimeOffset) : ((float)p * 1f / ((float)playgroundParticles.particleCount * 1f) + thisManipulatorProperty.splineTimeOffset));
				if (Mathf.Approximately(thisManipulatorProperty.splineTimeOffset, 0f) && Mathf.Approximately(a, 1f))
				{
					a = 0.9999f;
				}
				Vector3 point = thisManipulatorProperty.splineTarget.GetPoint(a);
				if (thisManipulatorProperty.onlyPositionInRange && thisManipulator.shape != MANIPULATORSHAPEC.Infinite && (!thisManipulatorProperty.onlyPositionInRange || !thisManipulator.Contains(point, manipulatorPosition)))
				{
					break;
				}
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
				{
					playgroundParticles.playgroundCache.position[p] = point;
					ref Vector3 reference = ref playgroundParticles.playgroundCache.velocity[p];
					reference = Vector3.zero;
				}
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference2 = ref playgroundParticles.playgroundCache.position[p];
					reference2 = Vector3.Lerp(particlePosition, point, t * (thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance) / thisManipulator.strengthSmoothing);
					if (thisManipulatorProperty.zeroVelocityStrength > 0f)
					{
						ref Vector3 reference3 = ref playgroundParticles.playgroundCache.velocity[p];
						reference3 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
					}
				}
				else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear)
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					ref Vector3 reference4 = ref playgroundParticles.playgroundCache.position[p];
					reference4 = Vector3.MoveTowards(particlePosition, point, t * (thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance) / thisManipulator.strengthSmoothing);
					if (thisManipulatorProperty.zeroVelocityStrength > 0f)
					{
						ref Vector3 reference5 = ref playgroundParticles.playgroundCache.velocity[p];
						reference5 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
					}
				}
				playgroundParticles.playgroundCache.changedByPropertyTarget[p] = true;
				break;
			}
			}
			playgroundParticles.playgroundCache.changedByProperty[p] = true;
			return;
		}
		if (thisManipulatorProperty.onlySizeInRange)
		{
			playgroundParticles.playgroundCache.changedByPropertySize[p] = false;
		}
		if (playgroundParticles.playgroundCache.propertyColorId[p] == thisManipulator.manipulatorId && (thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.Color || thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.LifetimeColor))
		{
			if (playgroundParticles.playgroundCache.changedByPropertyColorLerp[p] && thisManipulatorProperty.transition != MANIPULATORPROPERTYTRANSITIONC.None && thisManipulatorProperty.onlyColorInRange)
			{
				playgroundParticles.SetParticleColorInternal(p, Color.Lerp(playgroundParticles.particleCache[p].startColor, playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p] / playgroundParticles.lifetime), t * thisManipulatorProperty.strength * manipulatorStrength));
			}
			if (thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.LifetimeColor && !thisManipulatorProperty.onlyColorInRange)
			{
				if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
				{
					playgroundParticles.SetParticleColorInternal(p, thisManipulatorProperty.lifetimeColor.Evaluate((!(playgroundParticles.lifetime > 0f)) ? 0f : (playgroundParticles.playgroundCache.life[p] / playgroundParticles.lifetime)));
				}
				else
				{
					manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
					playgroundParticles.SetParticleColorInternal(p, Color.Lerp(playgroundParticles.particleCache[p].startColor, thisManipulatorProperty.lifetimeColor.Evaluate((!(playgroundParticles.lifetime > 0f)) ? 0f : (playgroundParticles.playgroundCache.life[p] / playgroundParticles.lifetime)), t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing));
				}
			}
			if (thisManipulatorProperty.onlyColorInRange)
			{
				playgroundParticles.playgroundCache.changedByPropertyColor[p] = false;
			}
		}
		if (playgroundParticles.onlySourcePositioning || thisManipulatorProperty.onlyPositionInRange || thisManipulator.shape == MANIPULATORSHAPEC.Infinite || thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
		{
			return;
		}
		if (thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.Target && thisManipulatorProperty.targets.Count > 0 && thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.targets.Count] != null && playgroundParticles.playgroundCache.changedByPropertyTarget[p] && thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.targets.Count].available && playgroundParticles.playgroundCache.propertyId[p] == thisManipulator.transform.GetInstanceID())
		{
			if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
			{
				ref Vector3 reference36 = ref playgroundParticles.playgroundCache.position[p];
				reference36 = ((!localSpace) ? Vector3.Lerp(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.targets.Count].position, t * (thisManipulatorProperty.strength * manipulatorStrength) / thisManipulator.strengthSmoothing) : Vector3.Lerp(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.targets.Count].localPosition, t * (thisManipulatorProperty.strength * manipulatorStrength) / thisManipulator.strengthSmoothing));
			}
			else
			{
				ref Vector3 reference37 = ref playgroundParticles.playgroundCache.position[p];
				reference37 = ((!localSpace) ? Vector3.MoveTowards(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.targets.Count].position, t * (thisManipulatorProperty.strength * manipulatorStrength) / thisManipulator.strengthSmoothing) : Vector3.MoveTowards(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.targets.Count].localPosition, t * (thisManipulatorProperty.strength * manipulatorStrength) / thisManipulator.strengthSmoothing));
			}
			if (thisManipulatorProperty.zeroVelocityStrength > 0f)
			{
				ref Vector3 reference38 = ref playgroundParticles.playgroundCache.velocity[p];
				reference38 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
			}
		}
		if (thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.MeshTarget && thisManipulatorProperty.meshTarget.initialized && playgroundParticles.playgroundCache.propertyId[p] == thisManipulator.manipulatorId)
		{
			if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
			{
				manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
				ref Vector3 reference39 = ref playgroundParticles.playgroundCache.position[p];
				reference39 = Vector3.Lerp(particlePosition, thisManipulatorProperty.meshTargetMatrix.MultiplyPoint3x4(thisManipulatorProperty.meshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.meshTarget.vertexPositions.Length]), t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing);
				if (thisManipulatorProperty.zeroVelocityStrength > 0f)
				{
					ref Vector3 reference40 = ref playgroundParticles.playgroundCache.velocity[p];
					reference40 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
				}
			}
			else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear)
			{
				manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
				ref Vector3 reference41 = ref playgroundParticles.playgroundCache.position[p];
				reference41 = Vector3.MoveTowards(particlePosition, thisManipulatorProperty.meshTargetMatrix.MultiplyPoint3x4(thisManipulatorProperty.meshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.meshTarget.vertexPositions.Length]), t * thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance / thisManipulator.strengthSmoothing);
				if (thisManipulatorProperty.zeroVelocityStrength > 0f)
				{
					ref Vector3 reference42 = ref playgroundParticles.playgroundCache.velocity[p];
					reference42 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
				}
			}
		}
		if (thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.SkinnedMeshTarget && thisManipulatorProperty.skinnedMeshTarget.initialized && playgroundParticles.playgroundCache.propertyId[p] == thisManipulator.manipulatorId)
		{
			if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
			{
				manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
				ref Vector3 reference43 = ref playgroundParticles.playgroundCache.position[p];
				reference43 = Vector3.Lerp(particlePosition, thisManipulatorProperty.skinnedMeshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.skinnedMeshTarget.vertexPositions.Length], t * (thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance) / thisManipulator.strengthSmoothing);
				if (thisManipulatorProperty.zeroVelocityStrength > 0f)
				{
					ref Vector3 reference44 = ref playgroundParticles.playgroundCache.velocity[p];
					reference44 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
				}
			}
			else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear)
			{
				manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
				ref Vector3 reference45 = ref playgroundParticles.playgroundCache.position[p];
				reference45 = Vector3.MoveTowards(particlePosition, thisManipulatorProperty.skinnedMeshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.skinnedMeshTarget.vertexPositions.Length], t * (thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance) / thisManipulator.strengthSmoothing);
				if (thisManipulatorProperty.zeroVelocityStrength > 0f)
				{
					ref Vector3 reference46 = ref playgroundParticles.playgroundCache.velocity[p];
					reference46 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
				}
			}
		}
		if (thisManipulatorProperty.type != MANIPULATORPROPERTYTYPEC.StateTarget || !thisManipulatorProperty.stateTarget.initialized || thisManipulatorProperty.stateTarget.IsInitializing() || playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId)
		{
			return;
		}
		if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
		{
			manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
			ref Vector3 reference47 = ref playgroundParticles.playgroundCache.position[p];
			reference47 = Vector3.Lerp(particlePosition, thisManipulatorProperty.stateTarget.stateTransformMx.MultiplyPoint3x4(thisManipulatorProperty.stateTarget.GetPosition(playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.stateTarget.positionLength)), t * (thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance) / thisManipulator.strengthSmoothing);
			if (thisManipulatorProperty.zeroVelocityStrength > 0f)
			{
				ref Vector3 reference48 = ref playgroundParticles.playgroundCache.velocity[p];
				reference48 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
			}
		}
		else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear)
		{
			manipulatorDistance = ((!(thisManipulator.strengthDistanceEffect > 0f)) ? 10f : (Vector3.SqrMagnitude(manipulatorPosition - particlePosition) / thisManipulator.strengthDistanceEffect));
			ref Vector3 reference49 = ref playgroundParticles.playgroundCache.position[p];
			reference49 = Vector3.MoveTowards(particlePosition, thisManipulatorProperty.stateTarget.stateTransformMx.MultiplyPoint3x4(thisManipulatorProperty.stateTarget.GetPosition(playgroundParticles.playgroundCache.propertyTarget[p] % thisManipulatorProperty.stateTarget.positionLength)), t * (thisManipulatorProperty.strength * manipulatorStrength / manipulatorDistance) / thisManipulator.strengthSmoothing);
			if (thisManipulatorProperty.zeroVelocityStrength > 0f)
			{
				ref Vector3 reference50 = ref playgroundParticles.playgroundCache.velocity[p];
				reference50 = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t * thisManipulatorProperty.zeroVelocityStrength);
			}
		}
	}

	public void RefreshScatter()
	{
		System.Random random = internalRandom01;
		for (int i = 0; i < particleCount; i++)
		{
			if (applySourceScatter)
			{
				if (sourceScatterMethod == MINMAXVECTOR3METHOD.Rectangular)
				{
					ref Vector3 reference = ref playgroundCache.scatterPosition[i];
					reference = RandomRange(random, sourceScatterMin, sourceScatterMax);
				}
				else if (sourceScatterMethod == MINMAXVECTOR3METHOD.RectangularLinear)
				{
					ref Vector3 reference2 = ref playgroundCache.scatterPosition[i];
					reference2 = Vector3.Lerp(sourceScatterMin, sourceScatterMax, (float)i * 1f / ((float)particleCount * 1f));
				}
				else if (sourceScatterMethod == MINMAXVECTOR3METHOD.Spherical)
				{
					ref Vector3 reference3 = ref playgroundCache.scatterPosition[i];
					reference3 = RandomRangeSpherical(random, sourceScatterMin.x, sourceScatterMax.x);
				}
				else if (sourceScatterMethod == MINMAXVECTOR3METHOD.SphericalLinear)
				{
					ref Vector3 reference4 = ref playgroundCache.scatterPosition[i];
					reference4 = RandomRangeSpherical(random, sourceScatterMin.x, sourceScatterMax.x, (float)i * 1f / ((float)particleCount * 1f));
				}
			}
			else
			{
				ref Vector3 reference5 = ref playgroundCache.scatterPosition[i];
				reference5 = Vector3.zero;
			}
		}
	}

	public static float RandomRange(System.Random random, float min, float max)
	{
		return min + (float)random.NextDouble() * (max - min);
	}

	public static Vector3 RandomRange(System.Random random, Vector3 min, Vector3 max)
	{
		return new Vector3(RandomRange(random, min.x, max.x), RandomRange(random, min.y, max.y), RandomRange(random, min.z, max.z));
	}

	public static Vector3 RandomRangeSpherical(System.Random random, float min, float max)
	{
		float f = (float)Math.PI * 2f * RandomRange(random, 0f, 1f);
		float f2 = Mathf.Asin(2f * RandomRange(random, 0f, 1f) - 1f);
		float num = RandomRange(random, min, max);
		return new Vector3(Mathf.Cos(f2) * Mathf.Sin(f) * num, Mathf.Cos(f2) * Mathf.Cos(f) * num, Mathf.Sin(f2) * num);
	}

	public static Vector3 RandomRangeSpherical(System.Random random, float min, float max, float step)
	{
		float f = (float)Math.PI * 2f * RandomRange(random, 0f, 1f);
		float f2 = Mathf.Asin(2f * RandomRange(random, 0f, 1f) - 1f);
		float num = Mathf.Lerp(min, max, step);
		return new Vector3(Mathf.Cos(f2) * Mathf.Sin(f) * num, Mathf.Cos(f2) * Mathf.Cos(f) * num, Mathf.Sin(f2) * num);
	}

	public static Vector3 RandomRangeSpherical(System.Random random, float min, float max, float sectorA, float sectorB)
	{
		if (sectorB < 0f)
		{
			sectorB = 0f;
		}
		float f = 2f * sectorA * (float)Math.PI * RandomRange(random, 0f, 1f);
		float f2 = Mathf.Asin(2f * sectorB * RandomRange(random, 0f, 1f) - 1f);
		float num = RandomRange(random, min, max);
		return new Vector3(Mathf.Cos(f2) * Mathf.Sin(f) * num, Mathf.Cos(f2) * Mathf.Cos(f) * num, Mathf.Sin(f2) * num);
	}

	public static Vector3 RandomRangeSpherical(System.Random random, float min, float max, float sectorA, float sectorB, float step)
	{
		if (sectorB < 0f)
		{
			sectorB = 0f;
		}
		float f = 2f * sectorA * (float)Math.PI * RandomRange(random, 0f, 1f);
		float f2 = Mathf.Asin(2f * sectorB * RandomRange(random, 0f, 1f) - 1f);
		float num = Mathf.Lerp(min, max, step);
		return new Vector3(Mathf.Cos(f2) * Mathf.Sin(f) * num, Mathf.Cos(f2) * Mathf.Cos(f) * num, Mathf.Sin(f2) * num);
	}

	public static float[] RandomFloat(int length, float min, float max, System.Random random)
	{
		float[] array = new float[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = RandomRange(random, min, max);
		}
		return array;
	}

	public static Vector3 RandomVector3(System.Random random, Vector3 v1, Vector3 v2)
	{
		return RandomRange(random, v1, v2);
	}

	public static Vector3[] RandomVector3(int length, Vector3 v1, Vector3 v2, System.Random random)
	{
		Vector3[] array = new Vector3[length];
		for (int i = 0; i < length; i++)
		{
			ref Vector3 reference = ref array[i];
			reference = RandomRange(random, v1, v2);
		}
		return array;
	}

	public void InactivateParticle(int particleId)
	{
		playgroundCache.simulate[particleId] = false;
		playgroundCache.rebirth[particleId] = false;
		playgroundCache.isNonBirthed[particleId] = true;
		playgroundCache.isFirstLoop[particleId] = true;
		playgroundCache.isCalculatedThisFrame[particleId] = false;
		ref Vector3 reference = ref playgroundCache.position[particleId];
		reference = PlaygroundC.initialTargetPosition;
		ref Vector3 reference2 = ref playgroundCache.targetPosition[particleId];
		reference2 = playgroundCache.position[particleId];
		ref Vector3 reference3 = ref playgroundCache.previousTargetPosition[particleId];
		reference3 = playgroundCache.targetPosition[particleId];
		ref Vector3 reference4 = ref playgroundCache.velocity[particleId];
		reference4 = Vector3.zero;
		particleCache[particleId].position = playgroundCache.position[particleId];
		particleCache[particleId].startSize = 0f;
		playgroundCache.changedByProperty[particleId] = false;
		playgroundCache.changedByPropertyColor[particleId] = false;
		playgroundCache.changedByPropertyColorLerp[particleId] = false;
		playgroundCache.changedByPropertyColorKeepAlpha[particleId] = false;
		playgroundCache.changedByPropertySize[particleId] = false;
		playgroundCache.changedByPropertyTarget[particleId] = false;
		playgroundCache.changedByPropertyDeath[particleId] = false;
		playgroundCache.propertyTarget[particleId] = 0;
		playgroundCache.propertyId[particleId] = 0;
		playgroundCache.propertyColorId[particleId] = 0;
		playgroundCache.manipulatorId[particleId] = 0;
	}

	public void InactivateParticles()
	{
		for (int i = 0; i < particleCount; i++)
		{
			InactivateParticle(i);
		}
		hasActiveParticles = false;
		threadHadNoActiveParticles = true;
		isDoneThread = true;
		if (particleCache != null && particleCache.Length > 0)
		{
			shurikenParticleSystem.SetParticles(particleCache, particleCache.Length);
		}
	}

	public static void Rebirth(PlaygroundParticlesC playgroundParticles, int p, System.Random random)
	{
		if (!playgroundParticles.hasActiveParticles)
		{
			return;
		}
		playgroundParticles.playgroundCache.rebirth[p] = playgroundParticles.source == SOURCEC.Script || (playgroundParticles.emit && (playgroundParticles.loop || playgroundParticles.playgroundCache.isNonBirthed[p]) && playgroundParticles.playgroundCache.emission[p]);
		playgroundParticles.playgroundCache.isFirstLoop[p] = playgroundParticles.playgroundCache.isNonBirthed[p];
		playgroundParticles.playgroundCache.isNonBirthed[p] = false;
		playgroundParticles.playgroundCache.life[p] = 0f;
		playgroundParticles.playgroundCache.birth[p] = playgroundParticles.playgroundCache.death[p];
		playgroundParticles.playgroundCache.death[p] += playgroundParticles.lifetime;
		ref Vector3 reference = ref playgroundParticles.playgroundCache.velocity[p];
		reference = Vector3.zero;
		playgroundParticles.playgroundCache.noForce[p] = false;
		if (playgroundParticles.hasCollisionCache)
		{
			playgroundParticles.collisionCache.Reset(p);
		}
		playgroundParticles.playgroundCache.changedByProperty[p] = false;
		playgroundParticles.playgroundCache.changedByPropertyColor[p] = false;
		playgroundParticles.playgroundCache.changedByPropertyColorLerp[p] = false;
		playgroundParticles.playgroundCache.changedByPropertyColorKeepAlpha[p] = false;
		playgroundParticles.playgroundCache.changedByPropertySize[p] = false;
		playgroundParticles.playgroundCache.changedByPropertyTarget[p] = false;
		playgroundParticles.playgroundCache.changedByPropertyDeath[p] = false;
		playgroundParticles.playgroundCache.propertyTarget[p] = 0;
		playgroundParticles.playgroundCache.propertyId[p] = 0;
		playgroundParticles.playgroundCache.propertyColorId[p] = 0;
		playgroundParticles.playgroundCache.manipulatorId[p] = 0;
		if (playgroundParticles.applyRandomSizeOnRebirth)
		{
			playgroundParticles.playgroundCache.initialSize[p] = RandomRange(random, playgroundParticles.sizeMin, playgroundParticles.sizeMax);
		}
		if (!playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning)
		{
			if (playgroundParticles.applyInitialVelocity)
			{
				if (playgroundParticles.applyRandomInitialVelocityOnRebirth)
				{
					if (playgroundParticles.initialVelocityMethod == MINMAXVECTOR3METHOD.Spherical)
					{
						ref Vector3 reference2 = ref playgroundParticles.playgroundCache.initialVelocity[p];
						reference2 = RandomRangeSpherical(random, playgroundParticles.initialVelocityMin.x, playgroundParticles.initialVelocityMax.x);
					}
					else if (playgroundParticles.initialVelocityMethod == MINMAXVECTOR3METHOD.SphericalLinear)
					{
						ref Vector3 reference3 = ref playgroundParticles.playgroundCache.initialVelocity[p];
						reference3 = RandomRangeSpherical(random, playgroundParticles.initialVelocityMin.x, playgroundParticles.initialVelocityMax.x, (float)p * 1f / ((float)playgroundParticles.particleCount * 1f));
					}
					else if (playgroundParticles.initialVelocityMethod == MINMAXVECTOR3METHOD.RectangularLinear)
					{
						ref Vector3 reference4 = ref playgroundParticles.playgroundCache.initialVelocity[p];
						reference4 = Vector3.Lerp(playgroundParticles.initialVelocityMin, playgroundParticles.initialVelocityMax, (float)p * 1f / ((float)playgroundParticles.particleCount * 1f));
					}
					else
					{
						ref Vector3 reference5 = ref playgroundParticles.playgroundCache.initialVelocity[p];
						reference5 = RandomRange(random, playgroundParticles.initialVelocityMin, playgroundParticles.initialVelocityMax);
					}
				}
				ref Vector3 reference6 = ref playgroundParticles.playgroundCache.velocity[p];
				reference6 = playgroundParticles.playgroundCache.initialVelocity[p];
				if (playgroundParticles.applyInitialVelocityShape)
				{
					ref Vector3 reference7 = ref playgroundParticles.playgroundCache.velocity[p];
					reference7 = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((float)p * 1f / ((float)playgroundParticles.particleCount * 1f), playgroundParticles.initialVelocityShapeScale));
				}
			}
			if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.source != SOURCEC.Script)
			{
				if (playgroundParticles.applyRandomInitialVelocityOnRebirth)
				{
					if (playgroundParticles.initialLocalVelocityMethod == MINMAXVECTOR3METHOD.Spherical)
					{
						ref Vector3 reference8 = ref playgroundParticles.playgroundCache.initialLocalVelocity[p];
						reference8 = RandomRangeSpherical(random, playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x);
					}
					else if (playgroundParticles.initialLocalVelocityMethod == MINMAXVECTOR3METHOD.SphericalLinear)
					{
						ref Vector3 reference9 = ref playgroundParticles.playgroundCache.initialLocalVelocity[p];
						reference9 = RandomRangeSpherical(random, playgroundParticles.initialLocalVelocityMin.x, playgroundParticles.initialLocalVelocityMax.x, (float)p * 1f / ((float)playgroundParticles.particleCount * 1f));
					}
					else if (playgroundParticles.initialLocalVelocityMethod == MINMAXVECTOR3METHOD.RectangularLinear)
					{
						ref Vector3 reference10 = ref playgroundParticles.playgroundCache.initialLocalVelocity[p];
						reference10 = Vector3.Lerp(playgroundParticles.initialLocalVelocityMin, playgroundParticles.initialLocalVelocityMax, (float)p * 1f / ((float)playgroundParticles.particleCount * 1f));
					}
					else
					{
						ref Vector3 reference11 = ref playgroundParticles.playgroundCache.initialLocalVelocity[p];
						reference11 = RandomRange(random, playgroundParticles.initialLocalVelocityMin, playgroundParticles.initialLocalVelocityMax);
					}
				}
				playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.playgroundCache.targetDirection[p];
				if (playgroundParticles.applyInitialVelocityShape)
				{
					ref Vector3 reference12 = ref playgroundParticles.playgroundCache.velocity[p];
					reference12 = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((float)p * 1f / ((float)playgroundParticles.particleCount * 1f), playgroundParticles.initialVelocityShapeScale));
				}
			}
			playgroundParticles.particleCache[p].velocity = ((!playgroundParticles.renderModeStretch || !playgroundParticles.applyStretchStartDirection) ? Vector3.zero : playgroundParticles.stretchStartDirection);
		}
		if (playgroundParticles.source == SOURCEC.Script)
		{
			if (!playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning)
			{
				playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.scriptedEmissionVelocity;
			}
			ref Vector3 reference13 = ref playgroundParticles.playgroundCache.targetPosition[p];
			reference13 = playgroundParticles.scriptedEmissionPosition;
			playgroundParticles.particleCache[p].velocity = ((!playgroundParticles.renderModeStretch || !playgroundParticles.applyStretchStartDirection) ? Vector3.zero : playgroundParticles.stretchStartDirection);
		}
		if (playgroundParticles.scriptedLifetime == 0f)
		{
			if (playgroundParticles.applyRandomLifetimeOnRebirth)
			{
				if (playgroundParticles.lifetimeValueMethod == VALUEMETHOD.Constant)
				{
					playgroundParticles.playgroundCache.lifetimeSubtraction[p] = 0f;
				}
				else
				{
					playgroundParticles.playgroundCache.lifetimeSubtraction[p] = playgroundParticles.lifetime - RandomRange(random, playgroundParticles.lifetimeMin, playgroundParticles.lifetime);
				}
			}
			if (!playgroundParticles.syncPositionsOnMainThread)
			{
				playgroundParticles.particleCache[p].remainingLifetime = playgroundParticles.lifetime;
			}
			playgroundParticles.particleCache[p].startLifetime = playgroundParticles.lifetime - playgroundParticles.playgroundCache.lifetimeSubtraction[p];
		}
		else
		{
			playgroundParticles.playgroundCache.lifetimeSubtraction[p] = 0f;
			if (!playgroundParticles.syncPositionsOnMainThread)
			{
				playgroundParticles.particleCache[p].remainingLifetime = playgroundParticles.scriptedLifetime;
			}
			playgroundParticles.particleCache[p].startLifetime = playgroundParticles.scriptedLifetime;
		}
		if (playgroundParticles.playgroundCache.rebirth[p])
		{
			playgroundParticles.SetParticleColorInternal(p, playgroundParticles.GetParticleColor(p, 0f, (float)p * 1f / ((float)playgroundParticles.particleCount * 1f)));
			if (playgroundParticles.applySourceScatter && playgroundParticles.source != SOURCEC.Script)
			{
				if (playgroundParticles.playgroundCache.scatterPosition[p] == Vector3.zero || playgroundParticles.applyRandomScatterOnRebirth)
				{
					if (playgroundParticles.sourceScatterMethod == MINMAXVECTOR3METHOD.Rectangular)
					{
						ref Vector3 reference14 = ref playgroundParticles.playgroundCache.scatterPosition[p];
						reference14 = RandomRange(random, playgroundParticles.sourceScatterMin, playgroundParticles.sourceScatterMax);
					}
					else if (playgroundParticles.sourceScatterMethod == MINMAXVECTOR3METHOD.RectangularLinear)
					{
						ref Vector3 reference15 = ref playgroundParticles.playgroundCache.scatterPosition[p];
						reference15 = Vector3.Lerp(playgroundParticles.sourceScatterMin, playgroundParticles.sourceScatterMax, (float)p * 1f / ((float)playgroundParticles.particleCount * 1f));
					}
					else if (playgroundParticles.sourceScatterMethod == MINMAXVECTOR3METHOD.SphericalLinear)
					{
						ref Vector3 reference16 = ref playgroundParticles.playgroundCache.scatterPosition[p];
						reference16 = RandomRangeSpherical(random, playgroundParticles.sourceScatterMin.x, playgroundParticles.sourceScatterMax.x, (float)p * 1f / ((float)playgroundParticles.particleCount * 1f));
					}
					else
					{
						ref Vector3 reference17 = ref playgroundParticles.playgroundCache.scatterPosition[p];
						reference17 = RandomRangeSpherical(random, playgroundParticles.sourceScatterMin.x, playgroundParticles.sourceScatterMax.x);
					}
				}
			}
			else
			{
				ref Vector3 reference18 = ref playgroundParticles.playgroundCache.scatterPosition[p];
				reference18 = Vector3.zero;
			}
			if (!playgroundParticles.onlyLifetimePositioning)
			{
				ref Vector3 reference19 = ref playgroundParticles.playgroundCache.position[p];
				reference19 = playgroundParticles.playgroundCache.targetPosition[p];
				if (!playgroundParticles.syncPositionsOnMainThread)
				{
					playgroundParticles.particleCache[p].position = playgroundParticles.playgroundCache.targetPosition[p];
				}
				ref Vector3 reference20 = ref playgroundParticles.playgroundCache.previousParticlePosition[p];
				reference20 = playgroundParticles.playgroundCache.targetPosition[p];
				ref Vector3 reference21 = ref playgroundParticles.playgroundCache.collisionParticlePosition[p];
				reference21 = playgroundParticles.playgroundCache.targetPosition[p];
			}
			else if (!playgroundParticles.onlySourcePositioning)
			{
				if (playgroundParticles.lifetimePositioningUsesSourceDirection && playgroundParticles.source != SOURCEC.Script)
				{
					ref Vector3 reference22 = ref playgroundParticles.playgroundCache.position[p];
					reference22 = playgroundParticles.playgroundCache.targetPosition[p] + playgroundParticles.playgroundCache.targetDirection[p];
				}
				else if (!playgroundParticles.applyLifetimePositioningPositionScale)
				{
					ref Vector3 reference23 = ref playgroundParticles.playgroundCache.position[p];
					reference23 = playgroundParticles.playgroundCache.targetPosition[p] + playgroundParticles.lifetimePositioning.Evaluate(0f, playgroundParticles.lifetimePositioningScale);
				}
				else
				{
					ref Vector3 reference24 = ref playgroundParticles.playgroundCache.position[p];
					reference24 = playgroundParticles.playgroundCache.targetPosition[p] + playgroundParticles.lifetimePositioning.Evaluate(0f, playgroundParticles.lifetimePositioningScale) * playgroundParticles.lifetimePositioningPositionScale.Evaluate(0f);
				}
				if (!playgroundParticles.syncPositionsOnMainThread)
				{
					playgroundParticles.particleCache[p].position = playgroundParticles.playgroundCache.targetPosition[p];
				}
				ref Vector3 reference25 = ref playgroundParticles.playgroundCache.previousParticlePosition[p];
				reference25 = playgroundParticles.playgroundCache.targetPosition[p];
				ref Vector3 reference26 = ref playgroundParticles.playgroundCache.collisionParticlePosition[p];
				reference26 = playgroundParticles.playgroundCache.targetPosition[p];
			}
			if (playgroundParticles.applyInitialColorOnRebirth)
			{
				playgroundParticles.SetParticleColorInternal(p, playgroundParticles.playgroundCache.initialColor[p]);
				ref Color32 reference27 = ref playgroundParticles.playgroundCache.color[p];
				reference27 = playgroundParticles.playgroundCache.initialColor[p];
			}
		}
		else
		{
			playgroundParticles.particleCache[p].position = PlaygroundC.initialTargetPosition;
		}
		if (playgroundParticles.applyRandomRotationOnRebirth)
		{
			if (playgroundParticles.separateRotationAxes)
			{
				ref Vector3 reference28 = ref playgroundParticles.playgroundCache.initialRotation3d[p];
				reference28 = RandomVector3(random, playgroundParticles.initialRotationMin3d, playgroundParticles.initialRotationMax3d);
				ref Vector3 reference29 = ref playgroundParticles.playgroundCache.rotationSpeed3d[p];
				reference29 = RandomVector3(random, playgroundParticles.rotationSpeedMin3d, playgroundParticles.rotationSpeedMax3d);
			}
			else if (!playgroundParticles.rotateTowardsDirection)
			{
				playgroundParticles.playgroundCache.initialRotation[p] = RandomRange(random, playgroundParticles.initialRotationMin, playgroundParticles.initialRotationMax);
				playgroundParticles.playgroundCache.rotationSpeed[p] = RandomRange(random, playgroundParticles.rotationSpeedMin, playgroundParticles.rotationSpeedMax);
			}
		}
		if (playgroundParticles.separateRotationAxes)
		{
			ref Vector3 reference30 = ref playgroundParticles.playgroundCache.rotation3d[p];
			reference30 = playgroundParticles.playgroundCache.initialRotation3d[p];
			if (!playgroundParticles.syncPositionsOnMainThread)
			{
				playgroundParticles.particleCache[p].rotation3D = playgroundParticles.playgroundCache.rotation3d[p];
			}
		}
		else
		{
			if (!playgroundParticles.rotateTowardsDirection)
			{
				playgroundParticles.playgroundCache.rotation[p] = playgroundParticles.playgroundCache.initialRotation[p];
			}
			else
			{
				Vector3 b = ((playgroundParticles.onlySourcePositioning || !playgroundParticles.onlyLifetimePositioning) ? playgroundParticles.playgroundCache.velocity[p] : (playgroundParticles.playgroundCache.position[p] + playgroundParticles.lifetimePositioning.Evaluate(0.01f, playgroundParticles.lifetimePositioningScale) - playgroundParticles.playgroundCache.position[p]));
				playgroundParticles.playgroundCache.rotation[p] = playgroundParticles.playgroundCache.initialRotation[p] + SignedAngle(Vector3.up, b, playgroundParticles.rotationNormal);
			}
			if (!playgroundParticles.syncPositionsOnMainThread)
			{
				playgroundParticles.particleCache[p].rotation = playgroundParticles.playgroundCache.rotation[p];
			}
		}
		if (playgroundParticles.applyLifetimeSize && !playgroundParticles.applyParticleArraySize)
		{
			playgroundParticles.playgroundCache.size[p] = playgroundParticles.playgroundCache.initialSize[p] * playgroundParticles.lifetimeSize.Evaluate(0f) * playgroundParticles.scale;
		}
		else if (playgroundParticles.applyLifetimeSize && playgroundParticles.applyParticleArraySize)
		{
			playgroundParticles.playgroundCache.size[p] = playgroundParticles.playgroundCache.initialSize[p] * playgroundParticles.lifetimeSize.Evaluate(0f) * playgroundParticles.particleArraySize.Evaluate((float)p * 1f / ((float)playgroundParticles.particleCount * 1f)) * playgroundParticles.scale;
		}
		else if (playgroundParticles.applyParticleArraySize)
		{
			playgroundParticles.playgroundCache.size[p] = playgroundParticles.playgroundCache.initialSize[p] * playgroundParticles.particleArraySize.Evaluate((float)p * 1f / ((float)playgroundParticles.particleCount * 1f)) * playgroundParticles.scale;
		}
		else
		{
			playgroundParticles.playgroundCache.size[p] = playgroundParticles.playgroundCache.initialSize[p] * playgroundParticles.scale;
		}
		if (!playgroundParticles.syncPositionsOnMainThread)
		{
			playgroundParticles.particleCache[p].startSize = ((!(playgroundParticles.playgroundCache.maskAlpha[p] > 0f)) ? 0f : playgroundParticles.playgroundCache.size[p]);
		}
		if (playgroundParticles.colorSource == COLORSOURCEC.LifetimeColors && playgroundParticles.lifetimeColors.Count > 0)
		{
			playgroundParticles.lifetimeColorId++;
			playgroundParticles.lifetimeColorId %= playgroundParticles.lifetimeColors.Count;
			playgroundParticles.playgroundCache.lifetimeColorId[p] = playgroundParticles.lifetimeColorId;
		}
		if (playgroundParticles.calculateManipulatorOnRebirth)
		{
			for (int i = 0; i < playgroundParticles.manipulators.Count; i++)
			{
				if (playgroundParticles.manipulators[i].transform != null)
				{
					CalculateManipulator(playgroundParticles, playgroundParticles.manipulators[i], p, playgroundParticles.t, playgroundParticles.playgroundCache.life[p], playgroundParticles.playgroundCache.position[p], ((!playgroundParticles.localSpace) ? playgroundParticles.manipulators[i].transform.position : playgroundParticles.manipulators[i].transform.localPosition) + playgroundParticles.manipulatorFix, playgroundParticles.localSpace);
				}
			}
		}
		if (playgroundParticles.events.Count > 0 && playgroundParticles.playgroundCache.rebirth[p])
		{
			playgroundParticles.SendEvent(EVENTTYPEC.Birth, p);
		}
		if (playgroundParticles.hasEventManipulatorLocal)
		{
			for (int j = 0; j < playgroundParticles.manipulators.Count; j++)
			{
				if (playgroundParticles.manipulators[j].trackParticles && playgroundParticles.manipulators[j].sendEventBirth && playgroundParticles.manipulators[j].Contains(playgroundParticles.playgroundCache.targetPosition[p], playgroundParticles.manipulators[j].transform.position))
				{
					playgroundParticles.UpdateEventParticle(playgroundParticles.manipulators[j].manipulatorEventParticle, p);
					playgroundParticles.manipulators[j].SendParticleEventBirth();
				}
			}
		}
		if (!playgroundParticles.hasEventManipulatorGlobal)
		{
			return;
		}
		for (int k = 0; k < PlaygroundC.reference.manipulators.Count; k++)
		{
			if (PlaygroundC.reference.manipulators[k].trackParticles && PlaygroundC.reference.manipulators[k].sendEventBirth && PlaygroundC.reference.manipulators[k].Contains(playgroundParticles.playgroundCache.targetPosition[p], PlaygroundC.reference.manipulators[k].transform.position))
			{
				playgroundParticles.UpdateEventParticle(PlaygroundC.reference.manipulators[k].manipulatorEventParticle, p);
				PlaygroundC.reference.manipulators[k].SendParticleEventBirth();
			}
		}
	}

	private void SendEvent(EVENTTYPEC eventType, int p)
	{
		SendEvent(eventType, p, playgroundCache.velocity[p], null, null, null);
	}

	private void SendEvent(EVENTTYPEC eventType, int p, Vector3 preEventVelocity)
	{
		SendEvent(eventType, p, preEventVelocity, null, null, null);
	}

	private void SendEvent(EVENTTYPEC eventType, int p, Vector3 preEventVelocity, Transform collisionTransform)
	{
		SendEvent(eventType, p, preEventVelocity, collisionTransform, null, null);
	}

	private void SendEvent(EVENTTYPEC eventType, int p, Vector3 preEventVelocity, Transform collisionTransform, Collider collisionCollider)
	{
		SendEvent(eventType, p, preEventVelocity, collisionTransform, collisionCollider, null);
	}

	private void SendEvent(EVENTTYPEC eventType, int p, Vector3 preEventVelocity, Transform collisionTransform, Collider2D collisionCollider2D)
	{
		SendEvent(eventType, p, preEventVelocity, collisionTransform, null, collisionCollider2D);
	}

	private void SendEvent(EVENTTYPEC eventType, int p, Vector3 preEventVelocity, Transform collisionTransform, Collider collisionCollider, Collider2D collisionCollider2D)
	{
		Vector3 givePosition = Vector3.zero;
		Vector3 vector = Vector3.zero;
		Color32 giveColor = Color.white;
		for (int i = 0; i < events.Count; i++)
		{
			if (!events[i].enabled || events[i].eventType != eventType)
			{
				continue;
			}
			PlaygroundEventC playgroundEventC = events[i];
			if (playgroundEventC.eventType == EVENTTYPEC.Collision && playgroundEventC.collisionThreshold > preEventVelocity.sqrMagnitude)
			{
				break;
			}
			switch (playgroundEventC.eventInheritancePosition)
			{
			case EVENTINHERITANCEC.User:
				givePosition = playgroundEventC.eventPosition;
				break;
			case EVENTINHERITANCEC.Particle:
				givePosition = ((eventType != EVENTTYPEC.Death) ? playgroundCache.position[p] : playgroundCache.previousParticlePosition[p]);
				break;
			case EVENTINHERITANCEC.Source:
				givePosition = playgroundCache.targetPosition[p];
				break;
			}
			switch (playgroundEventC.eventInheritanceVelocity)
			{
			case EVENTINHERITANCEC.User:
				if (playgroundEventC.eventEmitCount == 1)
				{
					if (playgroundEventC.velocityValueMethod != VALUEMETHOD.Constant)
					{
						if (playgroundEventC.velocityRandomMethod != MINMAXVECTOR3METHOD.Spherical)
						{
							if (playgroundEventC.velocityRandomMethod != MINMAXVECTOR3METHOD.SphericalLinear)
							{
								vector = ((playgroundEventC.velocityRandomMethod != MINMAXVECTOR3METHOD.RectangularLinear) ? RandomRange(internalRandom01, playgroundEventC.eventVelocityMin, playgroundEventC.eventVelocity) : Vector3.Lerp(playgroundEventC.eventVelocityMin, playgroundEventC.eventVelocity, (float)p * 1f / ((float)particleCount * 1f)));
							}
							else
							{
								vector = RandomRangeSpherical(internalRandom01, playgroundEventC.eventVelocityMin.x, playgroundEventC.eventVelocity.x, (float)p * 1f / ((float)particleCount * 1f));
							}
						}
						else
						{
							vector = RandomRangeSpherical(internalRandom01, playgroundEventC.eventVelocityMin.x, playgroundEventC.eventVelocity.x);
						}
					}
					else
					{
						vector = playgroundEventC.eventVelocity;
					}
				}
				break;
			case EVENTINHERITANCEC.Particle:
				vector = playgroundCache.velocity[p];
				break;
			case EVENTINHERITANCEC.Source:
				if (applyInitialLocalVelocity)
				{
					vector = playgroundCache.initialLocalVelocity[p];
				}
				if (applyInitialVelocity)
				{
					vector += playgroundCache.initialVelocity[p];
				}
				if (applyLifetimeVelocity)
				{
					vector += lifetimeVelocity.Evaluate(Mathf.Clamp01(playgroundCache.life[p] / lifetime), lifetimeVelocityScale);
				}
				if (applyInitialVelocityShape)
				{
					vector = Vector3.Scale(vector, initialVelocityShape.Evaluate((float)p * 1f / ((float)particleCount * 1f), initialVelocityShapeScale));
				}
				break;
			}
			vector *= playgroundEventC.velocityMultiplier;
			switch (playgroundEventC.eventInheritanceColor)
			{
			case EVENTINHERITANCEC.User:
				giveColor = playgroundEventC.eventColor;
				break;
			case EVENTINHERITANCEC.Particle:
				giveColor = playgroundCache.color[p];
				break;
			case EVENTINHERITANCEC.Source:
				giveColor = playgroundCache.initialColor[p];
				break;
			}
			if (playgroundEventC.broadcastType == EVENTBROADCASTC.EventListeners || playgroundEventC.broadcastType == EVENTBROADCASTC.Both)
			{
				UpdateEventParticle(eventParticle, p);
				eventParticle.collisionCollider = collisionCollider;
				eventParticle.collisionCollider2D = collisionCollider2D;
				eventParticle.collisionTransform = collisionTransform;
				playgroundEventC.SendParticleEvent(eventParticle);
				if (playgroundEventC.sendToManager)
				{
					switch (playgroundEventC.eventType)
					{
					case EVENTTYPEC.Birth:
						PlaygroundC.SendParticleEventBirth(eventParticle);
						break;
					case EVENTTYPEC.Death:
						PlaygroundC.SendParticleEventDeath(eventParticle);
						break;
					case EVENTTYPEC.Collision:
						PlaygroundC.SendParticleEventCollision(eventParticle);
						break;
					case EVENTTYPEC.Time:
						PlaygroundC.SendParticleEventTime(eventParticle);
						break;
					}
				}
			}
			if (!playgroundEventC.initializedTarget || (playgroundEventC.broadcastType != EVENTBROADCASTC.Target && playgroundEventC.broadcastType != EVENTBROADCASTC.Both))
			{
				continue;
			}
			int eventEmitCount = playgroundEventC.eventEmitCount;
			if (eventEmitCount == 1)
			{
				playgroundEventC.target.ThreadSafeEmit(givePosition, vector, giveColor);
				continue;
			}
			for (int j = 0; j < eventEmitCount; j++)
			{
				if (playgroundEventC.velocityRandomMethod != MINMAXVECTOR3METHOD.Spherical)
				{
					if (playgroundEventC.velocityRandomMethod != MINMAXVECTOR3METHOD.SphericalLinear)
					{
						vector = ((playgroundEventC.velocityRandomMethod != MINMAXVECTOR3METHOD.RectangularLinear) ? RandomRange(internalRandom01, playgroundEventC.eventVelocityMin, playgroundEventC.eventVelocity) : Vector3.Lerp(playgroundEventC.eventVelocityMin, playgroundEventC.eventVelocity, (float)j * 1f / ((float)eventEmitCount * 1f)));
					}
					else
					{
						vector = RandomRangeSpherical(internalRandom01, playgroundEventC.eventVelocityMin.x, playgroundEventC.eventVelocity.x, (float)j * 1f / ((float)eventEmitCount * 1f));
					}
				}
				else
				{
					vector = RandomRangeSpherical(internalRandom01, playgroundEventC.eventVelocityMin.x, playgroundEventC.eventVelocity.x);
				}
				vector *= playgroundEventC.velocityMultiplier;
				playgroundEventC.target.ThreadSafeEmit(givePosition, vector, giveColor);
			}
		}
	}

	public bool UpdateEventParticle(PlaygroundEventParticle eParticle, int p)
	{
		if (p >= particleCount || p < 0)
		{
			return false;
		}
		eParticle.particleSystemId = particleSystemId;
		eParticle.particleId = p;
		eParticle.birth = playgroundCache.birth[p];
		eParticle.birthDelay = playgroundCache.birthDelay[p];
		eParticle.changedByProperty = playgroundCache.changedByProperty[p];
		eParticle.changedByPropertyColor = playgroundCache.changedByPropertyColor[p];
		eParticle.changedByPropertyColorKeepAlpha = playgroundCache.changedByPropertyColorKeepAlpha[p];
		eParticle.changedByPropertyColorLerp = playgroundCache.changedByPropertyColorLerp[p];
		eParticle.changedByPropertyDeath = playgroundCache.changedByPropertyDeath[p];
		eParticle.changedByPropertySize = playgroundCache.changedByPropertySize[p];
		eParticle.changedByPropertyTarget = playgroundCache.changedByPropertyTarget[p];
		eParticle.collisionParticlePosition = playgroundCache.collisionParticlePosition[p];
		eParticle.color = playgroundCache.color[p];
		eParticle.scriptedColor = playgroundCache.scriptedColor[p];
		eParticle.death = playgroundCache.death[p];
		eParticle.emission = playgroundCache.emission[p];
		eParticle.initialColor = playgroundCache.initialColor[p];
		eParticle.initialLocalVelocity = playgroundCache.initialLocalVelocity[p];
		eParticle.initialRotation = (separateRotationAxes ? 0f : playgroundCache.initialRotation[p]);
		eParticle.initialRotation3d = ((!separateRotationAxes) ? Vector3.zero : playgroundCache.initialRotation3d[p]);
		eParticle.initialSize = playgroundCache.initialSize[p];
		eParticle.initialVelocity = playgroundCache.initialVelocity[p];
		eParticle.initialLocalVelocity = playgroundCache.initialLocalVelocity[p];
		eParticle.life = playgroundCache.life[p];
		eParticle.lifetimeColorId = playgroundCache.lifetimeColorId[p];
		eParticle.noForce = playgroundCache.noForce[p];
		eParticle.totalLifetime = playgroundCache.death[p] - playgroundCache.birth[p] - playgroundCache.lifetimeSubtraction[p];
		eParticle.lifetimeOffset = playgroundCache.lifetimeOffset[p];
		eParticle.localSpaceMovementCompensation = playgroundCache.localSpaceMovementCompensation[p];
		eParticle.position = playgroundCache.position[p];
		eParticle.previousParticlePosition = playgroundCache.previousParticlePosition[p];
		eParticle.previousTargetPosition = playgroundCache.previousTargetPosition[p];
		eParticle.propertyColorId = playgroundCache.propertyColorId[p];
		eParticle.propertyId = playgroundCache.propertyId[p];
		eParticle.excludeFromManipulatorId = playgroundCache.excludeFromManipulatorId[p];
		eParticle.propertyTarget = playgroundCache.propertyTarget[p];
		eParticle.rebirth = playgroundCache.rebirth[p];
		eParticle.rotation = (separateRotationAxes ? 0f : playgroundCache.rotation[p]);
		eParticle.rotation3d = ((!separateRotationAxes) ? Vector3.zero : playgroundCache.rotation3d[p]);
		eParticle.rotationSpeed = (separateRotationAxes ? 0f : playgroundCache.rotationSpeed[p]);
		eParticle.rotationSpeed3d = ((!separateRotationAxes) ? Vector3.zero : playgroundCache.rotationSpeed3d[p]);
		eParticle.scatterPosition = playgroundCache.scatterPosition[p];
		eParticle.size = playgroundCache.size[p];
		eParticle.targetDirection = playgroundCache.targetDirection[p];
		eParticle.targetPosition = playgroundCache.targetPosition[p];
		eParticle.velocity = playgroundCache.velocity[p];
		eParticle.isMasked = playgroundCache.isMasked[p];
		eParticle.maskAlpha = playgroundCache.maskAlpha[p];
		eParticle.isFirstLoop = playgroundCache.isFirstLoop[p];
		eParticle.isNonBirthed = playgroundCache.isNonBirthed[p];
		return true;
	}

	public void RemoveState(int i)
	{
		int num = activeState;
		num = num % states.Count - 1;
		if (num < 0)
		{
			num = 0;
		}
		states[num].Initialize();
		activeState = num;
		states.RemoveAt(i);
	}

	public static void Clear(PlaygroundParticlesC playgroundParticles)
	{
		playgroundParticles.inTransition = false;
		playgroundParticles.particleCache = new ParticleSystem.Particle[0];
		playgroundParticles.playgroundCache = null;
		playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache, 0);
		playgroundParticles.shurikenParticleSystem.Clear();
	}

	public void Save()
	{
		if (isSnapshot)
		{
			Debug.Log("A snapshot can't store snapshot data within itself.", gameObject);
		}
		else
		{
			SaveRoutine("New Snapshot " + (snapshots.Count + 1));
		}
	}

	public void Save(string saveName)
	{
		if (isSnapshot)
		{
			Debug.Log("A snapshot can't store snapshot data within itself.", gameObject);
		}
		else
		{
			SaveRoutine(saveName);
		}
	}

	public void SaveAsynchronous()
	{
		if (isSnapshot)
		{
			Debug.Log("A snapshot can't store snapshot data within itself.", gameObject);
		}
		else
		{
			StartCoroutine(SaveAsynchronousRoutine("New Snapshot " + (snapshots.Count + 1)));
		}
	}

	public void SaveAsynchronous(string saveName)
	{
		if (isSnapshot)
		{
			Debug.Log("A snapshot can't store snapshot data within itself.", gameObject);
		}
		else
		{
			StartCoroutine(SaveAsynchronousRoutine(saveName));
		}
	}

	private void SaveRoutine(string saveName)
	{
		isSaving = true;
		PlaygroundSave playgroundSave = new PlaygroundSave();
		playgroundSave.settings = PlaygroundC.Particle();
		playgroundSave.settings.isSnapshot = true;
		playgroundSave.Save(this);
		playgroundSave.settings.transform.parent = particleSystemTransform;
		playgroundSave.settings.transform.name = saveName;
		playgroundSave.settings.timeOfSnapshot = localTime;
		playgroundSave.name = saveName;
		playgroundSave.time = localTime;
		playgroundSave.particleCount = particleCount;
		playgroundSave.lifetime = lifetime;
		playgroundSave.version = PlaygroundC.version;
		snapshots.Add(playgroundSave);
		PlaygroundC.reference.particleSystems.Remove(playgroundSave.settings);
		isSaving = false;
	}

	private IEnumerator SaveAsynchronousRoutine(string saveName)
	{
		isSaving = true;
		PlaygroundSave data = new PlaygroundSave();
		data.settings = PlaygroundC.Particle();
		data.settings.isSnapshot = true;
		yield return null;
		data.SaveAsync(this);
		while (data.IsSaving())
		{
			yield return null;
		}
		data.settings.transform.parent = particleSystemTransform;
		data.settings.transform.name = saveName;
		data.settings.timeOfSnapshot = localTime;
		data.name = saveName;
		data.time = localTime;
		data.particleCount = particleCount;
		data.lifetime = lifetime;
		data.version = PlaygroundC.version;
		snapshots.Add(data);
		PlaygroundC.reference.particleSystems.Remove(data.settings);
		isSaving = false;
	}

	public void Load(int loadPointer)
	{
		if (snapshots.Count > 0)
		{
			loadPointer %= snapshots.Count;
			StartCoroutine(LoadRoutine(loadPointer, 0));
		}
		else
		{
			Debug.Log("No data to load from. Please use PlaygroundParticlesC.Save() to store a particle system's current state.", particleSystemGameObject);
		}
	}

	public void Load(string loadName)
	{
		if (snapshots.Count > 0)
		{
			for (int i = 0; i < snapshots.Count; i++)
			{
				if (snapshots[i].name == loadName.Trim())
				{
					StartCoroutine(LoadRoutine(i, 0));
					break;
				}
			}
		}
		else
		{
			Debug.Log("No data found with the name " + loadName + ".", particleSystemGameObject);
		}
	}

	public void LoadAndApplyMask(int loadPointer, int loadMask)
	{
		if (snapshots.Count > 0)
		{
			loadPointer %= snapshots.Count;
			StartCoroutine(LoadRoutine(loadPointer, loadMask));
		}
		else
		{
			Debug.Log("No data to load from. Please use PlaygroundParticlesC.Save() to store a particle system's current state.", particleSystemGameObject);
		}
	}

	private IEnumerator LoadRoutine(int loadPointer, int mask)
	{
		if (loadTransition && loadTransitionTime > 0f && snapshots[loadPointer].transitionMultiplier > 0f && transitionAvailable && !isYieldRefreshing)
		{
			if (snapshots[loadPointer].loadMaterial && !snapshots[loadPointer].setMaterialAfterTransition && snapshots[loadPointer].settings.particleSystemRenderer.sharedMaterial != null)
			{
				particleSystemRenderer.sharedMaterial = snapshots[loadPointer].settings.particleSystemRenderer.sharedMaterial;
			}
			if (mask > 0)
			{
				mask = Mathf.Clamp(mask, 0, snapshots[loadPointer].settings.particleCount);
				if (snapshots[loadPointer].settings.snapshotData.isMasked == null || snapshots[loadPointer].settings.snapshotData.isMasked.Length != snapshots[loadPointer].settings.particleCount)
				{
					snapshots[loadPointer].settings.snapshotData.isMasked = new bool[snapshots[loadPointer].settings.particleCount];
					snapshots[loadPointer].settings.snapshotData.maskAlpha = new float[snapshots[loadPointer].settings.particleCount];
				}
				snapshots[loadPointer].settings.applyParticleMask = true;
				snapshots[loadPointer].settings.particleMask = mask;
				snapshots[loadPointer].settings.RefreshMaskSorting();
				snapshots[loadPointer].settings.snapshotData.maskSorting = (int[])snapshots[loadPointer].settings.playgroundCache.maskSorting.Clone();
				for (int i = 0; i < snapshots[loadPointer].settings.particleCount; i++)
				{
					int num = snapshots[loadPointer].settings.snapshotData.maskSorting[i];
					snapshots[loadPointer].settings.snapshotData.isMasked[num] = i < mask;
					snapshots[loadPointer].settings.snapshotData.maskAlpha[num] = ((i >= mask) ? 1f : 0f);
				}
			}
			StartCoroutine(LoadTransition(loadPointer));
			while (inTransition)
			{
				yield return null;
			}
		}
		if (isLoading || inTransition || abortTransition)
		{
			yield break;
		}
		isLoading = true;
		int prevParticleCount = particleCount;
		if (prevParticleCount != snapshots[loadPointer].settings.particleCount)
		{
			SetParticleCount(thisInstance, snapshots[loadPointer].settings.particleCount);
			while (isSettingParticleCount)
			{
				yield return null;
			}
		}
		snapshots[loadPointer].Load(thisInstance);
		while (snapshots[loadPointer].IsLoading())
		{
			yield return null;
		}
		lastTimeUpdated = PlaygroundC.globalTime;
		localTime = PlaygroundC.globalTime;
		cameFromNonCalculatedFrame = false;
		cameFromNonEmissionFrame = false;
		if (snapshots[loadPointer].loadMode != 1)
		{
			if (loadFromStart && isYieldRefreshing)
			{
				yield return null;
			}
			float tos = (localTime = ((!(snapshots[loadPointer].settings.timeOfSnapshot <= 0f)) ? snapshots[loadPointer].settings.timeOfSnapshot : snapshots[loadPointer].time));
			for (int j = 0; j < particleCount; j++)
			{
				playgroundCache.birth[j] = tos + (playgroundCache.birth[j] - tos);
				playgroundCache.death[j] = tos + (playgroundCache.death[j] - tos);
				playgroundCache.size[j] = snapshots[loadPointer].settings.snapshotData.size[j];
				particleCache[j].startSize = playgroundCache.size[j];
			}
			lifetime = snapshots[loadPointer].settings.lifetime;
			previousLifetimeValueMethod = lifetimeValueMethod;
			previousLifetime = lifetime;
			previousLifetimeMin = lifetimeMin;
			previousLifetimeEmission = lifetimeEmission;
			previousNearestNeighborOrigin = nearestNeighborOrigin;
			previousNearestNeighborOriginMethod = nearestNeighborOriginMethod;
			previousNearestNeighborOriginVector3 = nearestNeighborOriginVector3;
			previousNearestNeighborOriginTransform = nearestNeighborOriginTransform;
			previousSorting = sorting;
			localDeltaTime = 0.001f;
			t = localDeltaTime;
		}
		else if (prevParticleCount != particleCount)
		{
			SetLifetime(thisInstance, sorting, lifetime);
			yield return null;
			while (isSettingLifetime)
			{
				yield return null;
			}
		}
		hasActiveParticles = true;
		threadHadNoActiveParticles = false;
		lastTimeUpdated = PlaygroundC.globalTime;
		cameFromNonCalculatedFrame = false;
		cameFromNonEmissionFrame = false;
		loopExceeded = false;
		loopExceededOnParticle = -1;
		isLoading = false;
		isDoneThread = true;
		transitionAvailable = true;
	}

	private IEnumerator LoadTransition(int loadPointer)
	{
		if (inTransition)
		{
			abortTransition = true;
			yield return null;
		}
		abortTransition = false;
		inTransition = true;
		float transitionStartTime = PlaygroundC.globalTime;
		float loadParticleLifetime = snapshots[loadPointer].settings.lifetime;
		int loadParticleCount = snapshots[loadPointer].settings.particleCount;
		bool liveParticles = snapshots[loadPointer].loadMode != 1;
		bool firstFrameDone = false;
		int transitionParticleCount;
		int currentParticleCount = (transitionParticleCount = particleCount);
		float initPosY = PlaygroundC.initialTargetPosition.y;
		PlaygroundCache loadSnapshotData = snapshots[loadPointer].settings.snapshotData;
		Color transparent = new Color(1f, 1f, 1f, 0f);
		INDIVIDUALTRANSITIONTYPEC thisSnapshotTransition = snapshots[loadPointer].transitionType;
		if (thisSnapshotTransition == INDIVIDUALTRANSITIONTYPEC.Inherit)
		{
			thisSnapshotTransition = (INDIVIDUALTRANSITIONTYPEC)(loadTransitionType + 1);
		}
		Vector3[] transitionPosition = (Vector3[])playgroundCache.position.Clone();
		Color32[] transitionColor = (Color32[])playgroundCache.color.Clone();
		float[] transitionSize = (float[])playgroundCache.size.Clone();
		float[] transitionRotation = (float[])playgroundCache.rotation.Clone();
		if (loadParticleCount > particleCount)
		{
			bool isResizing = true;
			PlaygroundC.RunAsync(() =>
			{
				Array.Resize(ref transitionPosition, loadParticleCount);
				Array.Resize(ref transitionColor, loadParticleCount);
				Array.Resize(ref transitionSize, loadParticleCount);
				Array.Resize(ref transitionRotation, loadParticleCount);
				Array.Resize(ref transitionRotation, loadParticleCount);
				Array.Resize(ref playgroundCache.position, loadParticleCount);
				Array.Resize(ref playgroundCache.color, loadParticleCount);
				Array.Resize(ref playgroundCache.size, loadParticleCount);
				Array.Resize(ref playgroundCache.rotation, loadParticleCount);
				for (int i = particleCount; i < loadParticleCount; i++)
				{
					ref Vector3 reference3 = ref transitionPosition[i];
					reference3 = transitionPosition[i % particleCount];
					transitionColor[i].a = 0;
				}
				isResizing = false;
			});
			while (isResizing)
			{
				yield return null;
			}
			particleCache = new ParticleSystem.Particle[loadParticleCount];
			shurikenParticleSystem.Emit(loadParticleCount);
			shurikenParticleSystem.GetParticles(particleCache);
			transitionParticleCount = loadParticleCount;
		}
		if (loadSnapshotData.lifetimeSubtraction == null || loadSnapshotData.lifetimeSubtraction.Length != loadParticleCount)
		{
			loadSnapshotData.lifetimeSubtraction = new float[loadParticleCount];
		}
		if (loadSnapshotData.isNonBirthed == null || loadSnapshotData.isNonBirthed.Length != loadParticleCount)
		{
			loadSnapshotData.isNonBirthed = new bool[loadParticleCount];
		}
		if (loadSnapshotData.isMasked == null || loadSnapshotData.isMasked.Length != loadParticleCount)
		{
			loadSnapshotData.isMasked = new bool[loadParticleCount];
		}
		for (int num = 0; num < loadParticleCount; num++)
		{
			if (loadSnapshotData.position[num].y == initPosY || loadSnapshotData.life[num] >= loadParticleLifetime - loadSnapshotData.lifetimeSubtraction[num])
			{
				ref Vector3 reference = ref loadSnapshotData.position[num];
				reference = loadSnapshotData.targetPosition[num];
				transitionColor[num].a = 0;
			}
			if (transitionPosition[num].y == initPosY || playgroundCache.life[num % particleCount] >= lifetime - playgroundCache.lifetimeSubtraction[num % particleCount])
			{
				ref Vector3 reference2 = ref transitionPosition[num];
				reference2 = loadSnapshotData.targetPosition[num];
				transitionColor[num].a = 0;
			}
		}
		while (PlaygroundC.globalTime < transitionStartTime + (loadTransitionTime * snapshots[loadPointer].transitionMultiplier - 0.001f) && inTransition && !abortTransition && loadTransition && loadTransitionTime * snapshots[loadPointer].transitionMultiplier > 0f && currentParticleCount == particleCount)
		{
			float currentTime = PlaygroundC.globalTime;
			PlaygroundC.RunAsync(() =>
			{
				float num3 = TransitionType(thisSnapshotTransition, (currentTime - transitionStartTime) / (loadTransitionTime * snapshots[loadPointer].transitionMultiplier));
				for (int i = 0; i < transitionParticleCount; i++)
				{
					if (!loadSnapshotData.isNonBirthed[i % loadParticleCount])
					{
						ref Vector3 reference3 = ref playgroundCache.position[i];
						reference3 = Vector3.Lerp(transitionPosition[i], (!liveParticles) ? loadSnapshotData.targetPosition[i % loadParticleCount] : loadSnapshotData.position[i % loadParticleCount], num3);
						if (!syncPositionsOnMainThread)
						{
							particleCache[i].position = playgroundCache.position[i];
						}
						if (!loadSnapshotData.isMasked[i % loadParticleCount])
						{
							ref Color32 reference4 = ref playgroundCache.color[i];
							reference4 = Color32.Lerp(transitionColor[i], loadSnapshotData.color[i % loadParticleCount], num3);
						}
						else if (playgroundCache.isMasked[i])
						{
							ref Color32 reference5 = ref playgroundCache.color[i];
							reference5 = transparent;
						}
						else
						{
							ref Color32 reference6 = ref playgroundCache.color[i];
							reference6 = Color32.Lerp(transitionColor[i], transparent, num3);
						}
						if (loadParticleCount < particleCount && i >= loadParticleCount)
						{
							playgroundCache.color[i].a = (byte)Mathf.Lerp((int)playgroundCache.color[i].a, 0f, num3);
						}
						SetParticleColorInternal(i, playgroundCache.color[i]);
						playgroundCache.size[i] = Mathf.Lerp(transitionSize[i], loadSnapshotData.size[i % loadParticleCount], num3);
						particleCache[i].startSize = playgroundCache.size[i];
						playgroundCache.rotation[i] = Mathf.Lerp(transitionRotation[i], loadSnapshotData.rotation[i % loadParticleCount], num3);
						particleCache[i].rotation = playgroundCache.rotation[i];
					}
					else
					{
						playgroundCache.color[i].a = (byte)Mathf.Lerp((int)playgroundCache.color[i].a, 0f, num3);
						SetParticleColorInternal(i, playgroundCache.color[i]);
					}
				}
			});
			yield return null;
			if (firstFrameDone && currentParticleCount == particleCount)
			{
				if (syncPositionsOnMainThread)
				{
					for (int num2 = 0; num2 < transitionParticleCount; num2++)
					{
						particleCache[num2].position = playgroundCache.position[num2];
					}
				}
				shurikenParticleSystem.SetParticles(particleCache, particleCache.Length);
			}
			firstFrameDone = true;
		}
		if (loadParticleCount != particleCount && abortTransition)
		{
			particleCache = new ParticleSystem.Particle[particleCount];
			shurikenParticleSystem.Emit(particleCount);
			shurikenParticleSystem.GetParticles(particleCache);
			shurikenParticleSystem.SetParticles(particleCache, particleCache.Length);
			Array.Resize(ref playgroundCache.position, particleCount);
			Array.Resize(ref playgroundCache.color, particleCount);
			Array.Resize(ref playgroundCache.size, particleCount);
			Array.Resize(ref playgroundCache.rotation, particleCount);
		}
		lastTimeUpdated = PlaygroundC.globalTime;
		cameFromNonCalculatedFrame = false;
		cameFromNonEmissionFrame = false;
		if (!abortTransition)
		{
			inTransition = false;
		}
	}

	private float TransitionType(INDIVIDUALTRANSITIONTYPEC thisTransitionType, float t)
	{
		return thisTransitionType switch
		{
			INDIVIDUALTRANSITIONTYPEC.Linear => t, 
			INDIVIDUALTRANSITIONTYPEC.EaseIn => Mathf.Lerp(0f, 1f, 1f - Mathf.Cos(t * (float)Math.PI * 0.5f)), 
			INDIVIDUALTRANSITIONTYPEC.EaseOut => Mathf.Lerp(0f, 1f, Mathf.Sin(t * (float)Math.PI * 0.5f)), 
			_ => t, 
		};
	}

	private void CheckReferences()
	{
		if (PlaygroundC.reference == null)
		{
			PlaygroundC.reference = UnityEngine.Object.FindObjectOfType<PlaygroundC>();
		}
		if (PlaygroundC.reference == null)
		{
			PlaygroundC.reference = PlaygroundC.ResourceInstantiate("Playground Manager").GetComponent<PlaygroundC>();
		}
		if (playgroundCache == null)
		{
			playgroundCache = new PlaygroundCache();
		}
		if (thisInstance == null)
		{
			thisInstance = this;
		}
		if (particleSystemGameObject == null)
		{
			particleSystemGameObject = gameObject;
			particleSystemTransform = transform;
			particleSystemRenderer = GetComponent<Renderer>();
			shurikenParticleSystem = particleSystemGameObject.GetComponent<ParticleSystem>();
			particleSystemRenderer2 = gameObject.GetComponent<ParticleSystem>().GetComponent<Renderer>() as ParticleSystemRenderer;
			shurikenMainModule = shurikenParticleSystem.main;
		}
	}

	public IEnumerator YieldedRefresh()
	{
		if (isSnapshot || isYieldRefreshing)
		{
			yield break;
		}
		bool okToLoadFromStart = true;
		if (isSettingParticleCount || isSettingLifetime)
		{
			okToLoadFromStart = false;
		}
		while (!PlaygroundC.IsReady())
		{
			yield return null;
		}
		while (isSettingParticleCount)
		{
			yield return null;
		}
		while (isSettingLifetime)
		{
			yield return null;
		}
		if (!prewarm && (sorting == SORTINGC.NearestNeighbor || sorting == SORTINGC.NearestNeighborReversed))
		{
			shurikenParticleSystem.Clear();
			if (source == SOURCEC.SkinnedWorldObject)
			{
				yield return null;
			}
			SetLifetime(thisInstance, SORTINGC.Burst, lifetime);
			while (isSettingLifetime)
			{
				yield return null;
			}
			isYieldRefreshing = false;
			StartCoroutine(Prewarm((!loop) ? 0f : prewarmTime, 0));
			yield return null;
			while (isPrewarming)
			{
				yield return null;
			}
			SetParticleTimeNow(thisInstance);
			initialized = true;
			yield break;
		}
		while (isSettingParticleCount || isSettingLifetime)
		{
			yield return null;
		}
		isYieldRefreshing = true;
		if (okToLoadFromStart && loadFromStart && snapshots.Count > 0)
		{
			SetLifetime(thisInstance, sorting, lifetime);
			Load(loadFrom);
			yield return null;
			isYieldRefreshing = false;
			initialized = true;
			yield break;
		}
		SetLifetime(thisInstance, sorting, lifetime);
		if (multithreadedStartup)
		{
			while (isSettingLifetime)
			{
				yield return null;
			}
		}
		transitionAvailable = true;
		hasActiveParticles = true;
		threadHadNoActiveParticles = false;
		isYieldRefreshing = false;
		emissionStopped = 0f;
		if (prewarm)
		{
			prewarmCycles = Mathf.Clamp(prewarmCycles, 0, 1000);
			StartCoroutine(Prewarm(prewarmTime, prewarmCycles));
		}
		while (isPrewarming)
		{
			yield return null;
		}
		initialized = true;
	}

	private IEnumerator Prewarm(float time, int resolution)
	{
		if (isPrewarming || isLoading)
		{
			yield break;
		}
		if (source == SOURCEC.SkinnedWorldObject || HasTurbulence())
		{
			yield return null;
		}
		isPrewarming = true;
		while (isSettingParticleCount || isSettingLifetime)
		{
			yield return null;
		}
		resolution = Mathf.Clamp(resolution, 0, 1000);
		int currentCycles = 0;
		float storedLocalTime = 0f;
		bool isPrewarmingAsync = false;
		isReadyForThreadedCalculations = PrepareThreadedCalculations();
		if (isReadyForThreadedCalculations)
		{
			localTime = PlaygroundC.globalTime;
			lastTimeUpdated = PlaygroundC.globalTime;
			if (sorting == SORTINGC.NearestNeighbor || sorting == SORTINGC.NearestNeighborReversed)
			{
				if (nearestNeighborOriginMethod == NEARESTNEIGHBORORIGINMETHOD.Transform && nearestNeighborOriginTransform != null)
				{
					nearestNeighborOriginTransformPosition = (localSpace ? particleSystemTransform.InverseTransformPoint(nearestNeighborOriginTransform.position) : nearestNeighborOriginTransform.position);
				}
				storedLocalTime = localTime;
				SetLifetime(thisInstance, SORTINGC.Burst, lifetime);
				while (isSettingLifetime)
				{
					yield return null;
				}
				localTime = storedLocalTime;
				for (int i = 0; i < particleCount; i++)
				{
					SetSourcePosition(i);
				}
				storedLocalTime = localTime;
				SetLifetime(thisInstance, sorting, lifetime);
				while (isSettingLifetime)
				{
					yield return null;
				}
				localTime = storedLocalTime;
				for (int j = 0; j < particleCount; j++)
				{
					SetSourcePosition(j);
				}
			}
			while (isSettingLifetime)
			{
				yield return null;
			}
			if (resolution > 0)
			{
				isPrewarmingAsync = true;
				PlaygroundC.RunAsync(() =>
				{
					for (; currentCycles <= resolution; currentCycles++)
					{
						PrewarmStepper(time, resolution);
					}
					if (syncPositionsOnMainThread && calculateDeltaMovement)
					{
						for (int k = 0; k < particleCount; k++)
						{
							ref Vector3 reference = ref playgroundCache.previousTargetPosition[k];
							reference = playgroundCache.targetPosition[k];
						}
					}
					isPrewarmingAsync = false;
				});
			}
		}
		while (isPrewarmingAsync)
		{
			yield return null;
		}
		isPrewarming = false;
	}

	private void PrewarmStepper(float time, int resolution)
	{
		localDeltaTime = lifetime * time / ((float)resolution * 1f);
		t = localDeltaTime;
		localTime += t;
		ThreadedCalculations(thisInstance);
		if (HasTurbulence() && PlaygroundC.reference.turbulenceThreadMethod == ThreadMethodComponent.OneForAll)
		{
			for (int i = 0; i < particleCount; i++)
			{
				Turbulence(thisInstance, turbulenceSimplex, i, t, turbulenceType, turbulenceTimeScale, turbulenceScale, turbulenceStrength, turbulenceApplyLifetimeStrength, turbulenceLifetimeStrength);
			}
		}
	}

	private void OnEnable()
	{
		if (isSnapshot || isLoading)
		{
			return;
		}
		thisInstance = this;
		thisLayer = particleSystemGameObject.layer;
		initialized = false;
		isYieldRefreshing = false;
		isSettingParticleCount = false;
		isSettingLifetime = false;
		hasActiveParticles = true;
		threadHadNoActiveParticles = false;
		isDoneThread = true;
		calculate = true;
		lastTimeUpdated = PlaygroundC.globalTime;
		localTime = lastTimeUpdated;
		emissionStopped = 0f;
		queueEmissionHalt = false;
		hasEmitted = false;
		if (internalRandom01 == null || randomSeedOnEnable)
		{
			if (randomSeedOnEnable)
			{
				_randomSeed = UnityEngine.Random.Range(0, 10000);
			}
			RefreshSystemRandom(_randomSeed);
		}
		if (shurikenParticleSystem == null)
		{
			shurikenParticleSystem = GetComponent<ParticleSystem>();
		}
		shurikenMainModule = shurikenParticleSystem.main;
		shurikenMainModule.startSizeMultiplier = 0f;
		if (shurikenParticleSystem.isPaused || shurikenParticleSystem.isStopped)
		{
			shurikenParticleSystem.Play();
		}
		previousLifetimeValueMethod = lifetimeValueMethod;
		previousLifetime = lifetime;
		previousLifetimeMin = lifetimeMin;
		previousEmission = emit;
		previousLoop = loop;
		previousNearestNeighborOrigin = nearestNeighborOrigin;
		previousNearestNeighborOriginMethod = nearestNeighborOriginMethod;
		previousNearestNeighborOriginVector3 = nearestNeighborOriginVector3;
		previousNearestNeighborOriginTransform = nearestNeighborOriginTransform;
		previousSorting = sorting;
		previousParticleCount = particleCount;
		previousLifetimeEmission = lifetimeEmission;
		prevSource = source;
		loopExceeded = false;
		loopExceededOnParticle = -1;
		stRot = Quaternion.identity;
		stDir = default;
		if (particleCache == null)
		{
			SetParticleCount(thisInstance, particleCount);
			return;
		}
		SetInitialTargetPosition(thisInstance, PlaygroundC.initialTargetPosition, refreshParticleSystem: true);
		hasCollisionCache = false;
		collisionCache = null;
		StartCoroutine(YieldedRefresh());
	}

	public void Start()
	{
		if (isSnapshot || !gameObject.activeInHierarchy || !gameObject.activeSelf)
		{
			return;
		}
		CheckReferences();
		if (thisInstance.enabled)
		{
			if (source == SOURCEC.State)
			{
				for (int i = 0; i < states.Count; i++)
				{
					states[i].Initialize();
				}
			}
			if (worldObject != null && worldObject.transform != null && worldObject.gameObject != null && worldObject.gameObject.activeInHierarchy)
			{
				worldObject.Initialize();
			}
			if (skinnedWorldObject != null && skinnedWorldObject.transform != null && skinnedWorldObject.gameObject != null && skinnedWorldObject.gameObject.activeInHierarchy)
			{
				skinnedWorldObject.Initialize();
			}
			if (projection != null && projection.projectionTexture != null)
			{
				projection.Initialize();
			}
			if (manipulators.Count > 0)
			{
				for (int j = 0; j < manipulators.Count; j++)
				{
					manipulators[j].Update();
				}
			}
		}
		if (PlaygroundC.reference != null)
		{
			if (isSnapshot)
			{
				return;
			}
			if (!PlaygroundC.reference.particleSystems.Contains(thisInstance))
			{
				PlaygroundC.reference.particleSystems.Add(thisInstance);
			}
			if (particleSystemTransform.parent == null && PlaygroundC.reference.autoGroup)
			{
				particleSystemTransform.parent = PlaygroundC.referenceTransform;
			}
		}
		if (splines != null && splines.Count > 0)
		{
			for (int k = 0; k < splines.Count; k++)
			{
				if (splines[k] != null)
				{
					splines[k].AddUser(particleSystemTransform);
				}
			}
		}
		eventControlledBy = new List<PlaygroundParticlesC>();
		StartCoroutine(Boot());
	}

	public IEnumerator Boot()
	{
		if (!particleSystemGameObject.activeInHierarchy || !particleSystemGameObject.activeSelf || isLoading || !enabled)
		{
			yield break;
		}
		while (PlaygroundC.reference == null)
		{
			yield return null;
		}
		if (PlaygroundC.reference.HasEnabledGlobalManipulators())
		{
			for (int i = 0; i < PlaygroundC.reference.manipulators.Count; i++)
			{
				if (PlaygroundC.reference.manipulators[i].trackParticles && (PlaygroundC.reference.manipulators[i].affects.value & (1 << GetLayer())) != 0 && PlaygroundC.reference.threadMethod != ThreadMethod.NoThreads)
				{
					threadMethod = ThreadMethodLocal.OneForAll;
				}
			}
		}
		localTime = PlaygroundC.globalTime;
		lastTimeUpdated = localTime;
		for (int j = 0; j < PlaygroundC.reference.particleSystems.Count; j++)
		{
			if (PlaygroundC.reference.particleSystems[j].particleSystemId == particleSystemId && PlaygroundC.reference.particleSystems[j] != this)
			{
				particleSystemId = PlaygroundC.reference.particleSystems.Count - 1;
				break;
			}
		}
		if (!isYieldRefreshing)
		{
			StartCoroutine(YieldedRefresh());
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < events.Count; i++)
		{
			if (events[i].target != null)
			{
				events[i].target.eventControlledBy.Remove(thisInstance);
			}
		}
		if (splines.Count > 0)
		{
			for (int j = 0; j < splines.Count; j++)
			{
				if (splines[j] != null)
				{
					splines[j].RemoveUser(particleSystemTransform);
				}
			}
		}
		if ((bool)PlaygroundC.reference)
		{
			PlaygroundC.reference.particleSystems.Remove(thisInstance);
		}
	}

	private void OnDisable()
	{
		initialized = false;
	}
}
