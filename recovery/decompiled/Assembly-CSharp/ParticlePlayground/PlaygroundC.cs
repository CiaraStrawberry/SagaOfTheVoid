using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using PlaygroundSplines;
using UnityEngine;

namespace ParticlePlayground;

[ExecuteInEditMode]
public class PlaygroundC : MonoBehaviour
{
	public static int meshQuantity;

	public static int particlesQuantity;

	public static float version = 3.1f;

	public static string specialVersion = " ";

	public static float globalTime;

	public static float lastTimeUpdated;

	public static float globalDeltaTime;

	public static float globalTimescale = 1f;

	public static Vector3 initialTargetPosition = new Vector3(0f, -999.99f, 0f);

	public static int skinnedUpdateRate = 1;

	public static bool triggerSceneRepaint = true;

	public static float collisionSleepVelocity = 0.01f;

	public static int unsafeAutomaticThreadFrames = 20;

	public static Plane[] frustumPlanes;

	private static bool particleEventBirthInitialized = false;

	private static bool particleEventDeathInitialized = false;

	private static bool particleEventCollisionInitialized = false;

	private static bool particleEventTimeInitialized = false;

	public static PlaygroundC reference;

	public static Transform referenceTransform;

	public static GameObject referenceGameObject;

	[SerializeField]
	public List<PlaygroundParticlesC> particleSystems = new List<PlaygroundParticlesC>();

	[SerializeField]
	public List<ManipulatorObjectC> manipulators = new List<ManipulatorObjectC>();

	[HideInInspector]
	public int paintMaxPositions = 10000;

	[HideInInspector]
	public bool calculate = true;

	[HideInInspector]
	public PIXELMODEC pixelFilterMode = PIXELMODEC.Pixel32;

	[HideInInspector]
	public bool autoGroup;

	[HideInInspector]
	public bool buildZeroAlphaPixels;

	[HideInInspector]
	public bool drawGizmos = true;

	[HideInInspector]
	public bool drawSourcePositions;

	[HideInInspector]
	public bool showShuriken;

	[HideInInspector]
	public bool paintToolbox = true;

	[HideInInspector]
	public float collisionPlaneScale = 0.1f;

	[HideInInspector]
	public bool showSnapshotsInHierarchy;

	[HideInInspector]
	public bool drawWireframe;

	[HideInInspector]
	public bool drawSplinePreview = true;

	[HideInInspector]
	public bool globalTimeScale = true;

	[HideInInspector]
	public ThreadPoolMethod threadPoolMethod = ThreadPoolMethod.PlaygroundPool;

	[HideInInspector]
	public ThreadMethod threadMethod = ThreadMethod.Automatic;

	[HideInInspector]
	public ThreadMethodComponent skinnedMeshThreadMethod;

	[HideInInspector]
	public ThreadMethodComponent turbulenceThreadMethod;

	[HideInInspector]
	public int maxThreads = 8;

	public static PlaygroundQueue<Action> playgroundPool;

	private bool isDoneThread = true;

	private bool isDoneThreadLocal = true;

	private bool isDoneThreadSkinned = true;

	private bool isDoneThreadTurbulence = true;

	private int threads;

	private int _prevMaxThreadCount;

	private static int processorCount;

	private static int activeThreads = 0;

	private static bool hasReference = false;

	private static bool hasPlaygroundPool = false;

	private static System.Random random = new System.Random();

	private static object locker = new object();

	private object lockerLocal = new object();

	private int frameCount;

	private bool isReady;

	private bool hasActiveParticleSystems;

	private bool hasLocalNoThreads;

	private bool hasLocalOnePerSystem;

	private bool hasLocalOneForAll;

	private bool hasActiveSkinnedMeshes;

	private bool hasActiveTurbulence;

	private bool isFirstUnsafeAutomaticFrames = true;

	private int threadAggregatorRuns;

	private List<int> activeSystems = new List<int>();

	private ThreadMethod previousThreadMethod;

	private ThreadMethodComponent previousSkinnedMeshThreadMethod;

	private ThreadMethodComponent previousTurbulenceThreadMethod;

	public static event OnPlaygroundParticle particleEventBirth;

	public static event OnPlaygroundParticle particleEventDeath;

	public static event OnPlaygroundParticle particleEventCollision;

	public static event OnPlaygroundParticle particleEventTime;

	public static void SendParticleEventBirth(PlaygroundEventParticle eventParticle)
	{
		if (particleEventBirthInitialized)
		{
			particleEventBirth(eventParticle);
		}
	}

	public static void SendParticleEventDeath(PlaygroundEventParticle eventParticle)
	{
		if (particleEventDeathInitialized)
		{
			particleEventDeath(eventParticle);
		}
	}

	public static void SendParticleEventCollision(PlaygroundEventParticle eventParticle)
	{
		if (particleEventCollisionInitialized)
		{
			particleEventCollision(eventParticle);
		}
	}

	public static void SendParticleEventTime(PlaygroundEventParticle eventParticle)
	{
		if (particleEventTimeInitialized)
		{
			particleEventTime(eventParticle);
		}
	}

	public static PlaygroundParticlesC Particle()
	{
		return ResourceInstantiate("Particle Playground System").GetComponent<PlaygroundParticlesC>();
	}

	public static PlaygroundParticlesC ParticleNew()
	{
		PlaygroundParticlesC playgroundParticlesC = PlaygroundParticlesC.CreateParticleObject("Particle Playground System " + particlesQuantity, Vector3.zero, Quaternion.identity, 1f, new Material(Shader.Find("Playground/Vertex Color")));
		playgroundParticlesC.particleCache = new ParticleSystem.Particle[playgroundParticlesC.particleCount];
		PlaygroundParticlesC.OnCreatePlaygroundParticles(playgroundParticlesC);
		return playgroundParticlesC;
	}

	public static PlaygroundParticlesC Particle(Texture2D image, string name, Vector3 position, Quaternion rotation, Vector3 offset, float particleSize, float scale, Material material)
	{
		return PlaygroundParticlesC.CreatePlaygroundParticles(new Texture2D[1] { image }, name, position, rotation, offset, particleSize, scale, material);
	}

	public static PlaygroundParticlesC Particle(Texture2D image)
	{
		return PlaygroundParticlesC.CreatePlaygroundParticles(new Texture2D[1] { image }, "Particle Playground System " + particlesQuantity, Vector3.zero, Quaternion.identity, Vector3.zero, 1f, 1f, new Material(Shader.Find("Playground/Vertex Color")));
	}

	public static PlaygroundParticlesC Particle(Texture2D[] images, string name, Vector3 position, Quaternion rotation, Vector3 offset, float particleSize, float scale, Material material)
	{
		return PlaygroundParticlesC.CreatePlaygroundParticles(images, name, position, rotation, offset, particleSize, scale, material);
	}

	public static PlaygroundParticlesC Particle(Texture2D[] images)
	{
		return PlaygroundParticlesC.CreatePlaygroundParticles(images, "Particle Playground System " + particlesQuantity, Vector3.zero, Quaternion.identity, Vector3.zero, 1f, 1f, new Material(Shader.Find("Playground/Vertex Color")));
	}

	public static PlaygroundParticlesC Particle(Mesh mesh, Texture2D texture, string name, Vector3 position, Quaternion rotation, float particleScale, Vector3 offset, Material material)
	{
		return MeshParticles.CreateMeshParticles(new Mesh[1] { mesh }, new Texture2D[1] { texture }, null, name, position, rotation, particleScale, new Vector3[1] { offset }, material);
	}

	public static PlaygroundParticlesC Particle(Mesh mesh, Texture2D texture)
	{
		return MeshParticles.CreateMeshParticles(new Mesh[1] { mesh }, new Texture2D[1] { texture }, null, "Particle Playground System " + particlesQuantity, Vector3.zero, Quaternion.identity, 1f, new Vector3[1] { Vector3.zero }, new Material(Shader.Find("Playground/Vertex Color")));
	}

	public static PlaygroundParticlesC Particle(Mesh[] meshes, Texture2D[] textures, string name, Vector3 position, Quaternion rotation, float particleScale, Vector3[] offsets, Material material)
	{
		return MeshParticles.CreateMeshParticles(meshes, textures, null, name, position, rotation, particleScale, offsets, material);
	}

	public static PlaygroundParticlesC Particle(Mesh[] meshes, Texture2D[] textures)
	{
		return MeshParticles.CreateMeshParticles(meshes, textures, null, "Particle Playground System " + particlesQuantity, Vector3.zero, Quaternion.identity, 1f, new Vector3[meshes.Length], new Material(Shader.Find("Playground/Vertex Color")));
	}

	public static int Emit(PlaygroundParticlesC playgroundParticles)
	{
		return playgroundParticles.Emit(playgroundParticles.scriptedEmissionPosition, playgroundParticles.scriptedEmissionVelocity, playgroundParticles.scriptedEmissionColor);
	}

	public static int Emit(PlaygroundParticlesC playgroundParticles, Vector3 position, Vector3 normal, Color color)
	{
		return playgroundParticles.Emit(position, normal, color);
	}

	public static void Emit(PlaygroundParticlesC playgroundParticles, bool setEmission)
	{
		playgroundParticles.Emit(setEmission);
	}

	public static void GetPosition(SkinnedWorldObject particleStateWorldObject, bool updateNormals)
	{
		PlaygroundParticlesC.GetPosition(particleStateWorldObject, updateNormals);
	}

	public static void GetPosition(Vector3[] vertices, WorldObject particleStateWorldObject)
	{
		PlaygroundParticlesC.GetPosition(vertices, particleStateWorldObject);
	}

	public static void GetNormals(Vector3[] normals, WorldObject particleStateWorldObject)
	{
		PlaygroundParticlesC.GetNormals(normals, particleStateWorldObject);
	}

	public static void SetAlpha(PlaygroundParticlesC playgroundParticles, float alpha)
	{
		PlaygroundParticlesC.SetAlpha(playgroundParticles, alpha);
	}

	public static void SetSize(PlaygroundParticlesC playgroundParticles, float size)
	{
		PlaygroundParticlesC.SetSize(playgroundParticles, size);
	}

	public static void Translate(PlaygroundParticlesC playgroundParticles, Vector3 direction)
	{
		PlaygroundParticlesC.Translate(playgroundParticles, direction);
	}

	public static void Add(PlaygroundParticlesC playgroundParticles, ParticleStateC state)
	{
		PlaygroundParticlesC.Add(playgroundParticles, state);
	}

	public static void Add(PlaygroundParticlesC playgroundParticles, Texture2D image, float scale, Vector3 offset, string stateName)
	{
		PlaygroundParticlesC.Add(playgroundParticles, image, scale, offset, stateName, null);
	}

	public static void Add(PlaygroundParticlesC playgroundParticles, Texture2D image, float scale, Vector3 offset, string stateName, Transform stateTransform)
	{
		PlaygroundParticlesC.Add(playgroundParticles, image, scale, offset, stateName, stateTransform);
	}

	public static void Add(PlaygroundParticlesC playgroundParticles, Texture2D image, Texture2D depthmap, float depthmapStrength, float scale, Vector3 offset, string stateName)
	{
		PlaygroundParticlesC.Add(playgroundParticles, image, depthmap, depthmapStrength, scale, offset, stateName, null);
	}

	public static void Add(PlaygroundParticlesC playgroundParticles, Texture2D image, Texture2D depthmap, float depthmapStrength, float scale, Vector3 offset, string stateName, Transform stateTransform)
	{
		PlaygroundParticlesC.Add(playgroundParticles, image, depthmap, depthmapStrength, scale, offset, stateName, stateTransform);
	}

	public static void Add(PlaygroundParticlesC playgroundParticles, Mesh mesh, float scale, Vector3 offset, string stateName)
	{
		MeshParticles.Add(playgroundParticles, mesh, scale, offset, stateName, null);
	}

	public static void Add(PlaygroundParticlesC playgroundParticles, Mesh mesh, float scale, Vector3 offset, string stateName, Transform stateTransform)
	{
		MeshParticles.Add(playgroundParticles, mesh, scale, offset, stateName, stateTransform);
	}

	public static void Add(PlaygroundParticlesC playgroundParticles, Mesh mesh, Texture2D texture, float scale, Vector3 offset, string stateName)
	{
		MeshParticles.Add(playgroundParticles, mesh, texture, scale, offset, stateName, null);
	}

	public static void Add(PlaygroundParticlesC playgroundParticles, Mesh mesh, Texture2D texture, float scale, Vector3 offset, string stateName, Transform stateTransform)
	{
		MeshParticles.Add(playgroundParticles, mesh, texture, scale, offset, stateName, stateTransform);
	}

	public static PlaygroundColliderC AddCollider(PlaygroundParticlesC playgroundParticles)
	{
		PlaygroundColliderC playgroundColliderC = new PlaygroundColliderC();
		playgroundParticles.colliders.Add(playgroundColliderC);
		return playgroundColliderC;
	}

	public static PlaygroundColliderC AddCollider(PlaygroundParticlesC playgroundParticles, Transform transform)
	{
		PlaygroundColliderC playgroundColliderC = new PlaygroundColliderC();
		playgroundColliderC.transform = transform;
		playgroundParticles.colliders.Add(playgroundColliderC);
		return playgroundColliderC;
	}

	public static void SetParticleCount(PlaygroundParticlesC playgroundParticles, int amount)
	{
		PlaygroundParticlesC.SetParticleCount(playgroundParticles, amount);
	}

	public static void SetLifetime(PlaygroundParticlesC playgroundParticles, float time)
	{
		PlaygroundParticlesC.SetLifetime(playgroundParticles, playgroundParticles.sorting, time);
	}

	public static void SetMaterial(PlaygroundParticlesC playgroundParticles, Material particleMaterial)
	{
		PlaygroundParticlesC.SetMaterial(playgroundParticles, particleMaterial);
	}

	public static void Destroy(PlaygroundParticlesC playgroundParticles)
	{
		PlaygroundParticlesC.Destroy(playgroundParticles);
	}

	public static WorldObject WorldObject(Transform meshTransform)
	{
		return PlaygroundParticlesC.NewWorldObject(meshTransform);
	}

	public static SkinnedWorldObject SkinnedWorldObject(Transform meshTransform)
	{
		return PlaygroundParticlesC.NewSkinnedWorldObject(meshTransform);
	}

	public static ManipulatorObjectC ManipulatorObject(MANIPULATORTYPEC type, LayerMask affects, Transform manipulatorTransform, float size, float strength)
	{
		return PlaygroundParticlesC.NewManipulatorObject(type, affects, manipulatorTransform, size, strength, null);
	}

	public static ManipulatorObjectC ManipulatorObject(Transform manipulatorTransform)
	{
		LayerMask affects = -1;
		return PlaygroundParticlesC.NewManipulatorObject(MANIPULATORTYPEC.Attractor, affects, manipulatorTransform, 1f, 1f, null);
	}

	public static ManipulatorObjectC ManipulatorObject(MANIPULATORTYPEC type, LayerMask affects, Transform manipulatorTransform, float size, float strength, PlaygroundParticlesC playgroundParticles)
	{
		return PlaygroundParticlesC.NewManipulatorObject(type, affects, manipulatorTransform, size, strength, playgroundParticles);
	}

	public static ManipulatorObjectC ManipulatorObject(Transform manipulatorTransform, PlaygroundParticlesC playgroundParticles)
	{
		LayerMask affects = -1;
		return PlaygroundParticlesC.NewManipulatorObject(MANIPULATORTYPEC.Attractor, affects, manipulatorTransform, 1f, 1f, playgroundParticles);
	}

	public static ManipulatorObjectC GetManipulator(int i)
	{
		if (reference.manipulators.Count > 0 && reference.manipulators[i % reference.manipulators.Count] != null)
		{
			return reference.manipulators[i % reference.manipulators.Count];
		}
		return null;
	}

	public static ManipulatorObjectC GetManipulator(int i, PlaygroundParticlesC playgroundParticles)
	{
		if (playgroundParticles.manipulators.Count > 0 && playgroundParticles.manipulators[i % playgroundParticles.manipulators.Count] != null)
		{
			return playgroundParticles.manipulators[i % playgroundParticles.manipulators.Count];
		}
		return null;
	}

	public static List<PlaygroundEventParticle> GetManipulatorParticles(int manipulator, PlaygroundParticlesC playgroundParticles)
	{
		if (manipulator < 0 || manipulator >= playgroundParticles.manipulators.Count)
		{
			return null;
		}
		List<PlaygroundEventParticle> list = new List<PlaygroundEventParticle>();
		PlaygroundEventParticle playgroundEventParticle = new PlaygroundEventParticle();
		for (int i = 0; i < playgroundParticles.particleCount; i++)
		{
			if (playgroundParticles.manipulators[manipulator].Contains(playgroundParticles.playgroundCache.position[i], playgroundParticles.manipulators[manipulator].transform.position))
			{
				playgroundParticles.UpdateEventParticle(playgroundEventParticle, i);
				list.Add(playgroundEventParticle.Clone());
			}
		}
		return list;
	}

	public static List<PlaygroundEventParticle> GetManipulatorParticles(int manipulator)
	{
		if (manipulator < 0 || manipulator >= reference.manipulators.Count)
		{
			return null;
		}
		List<PlaygroundEventParticle> list = new List<PlaygroundEventParticle>();
		PlaygroundEventParticle playgroundEventParticle = new PlaygroundEventParticle();
		for (int i = 0; i < reference.particleSystems.Count; i++)
		{
			for (int j = 0; j < reference.particleSystems[i].particleCount; j++)
			{
				if (reference.particleSystems[i].manipulators[manipulator].Contains(reference.particleSystems[i].playgroundCache.position[j], reference.particleSystems[i].manipulators[manipulator].transform.position))
				{
					reference.particleSystems[i].UpdateEventParticle(playgroundEventParticle, j);
					list.Add(playgroundEventParticle.Clone());
				}
			}
		}
		return list;
	}

	public static Transform CreateTransform()
	{
		return new GameObject("Source Transform").transform;
	}

	public static Transform CreatePlaygroundTransform(PlaygroundParticlesC playgroundParticles)
	{
		Transform transform = CreateTransform();
		PlaygroundTransformC playgroundTransformC = new PlaygroundTransformC();
		playgroundTransformC.transform = transform;
		if (playgroundParticles.sourceTransforms.Count == 1 && playgroundParticles.sourceTransforms[0].transform == null)
		{
			playgroundParticles.sourceTransforms[0].transform = transform;
		}
		else
		{
			playgroundParticles.sourceTransforms.Add(playgroundTransformC);
		}
		transform.parent = playgroundParticles.particleSystemTransform;
		transform.localPosition = Vector3.zero;
		transform.name = transform.name + " " + playgroundParticles.sourceTransforms.Count;
		return transform;
	}

	public static PlaygroundSpline CreateSpline()
	{
		GameObject gameObject = new GameObject("Playground Spline", typeof(PlaygroundSpline));
		return (PlaygroundSpline)gameObject.GetComponent(typeof(PlaygroundSpline));
	}

	public static PlaygroundSpline CreateSpline(PlaygroundParticlesC playgroundParticles)
	{
		PlaygroundSpline playgroundSpline = CreateSpline();
		playgroundSpline.Reset();
		if (playgroundParticles.splines.Count == 1 && playgroundParticles.splines[0] == null)
		{
			playgroundParticles.splines[0] = playgroundSpline;
		}
		else
		{
			playgroundParticles.splines.Add(playgroundSpline);
		}
		playgroundSpline.transform.parent = playgroundParticles.particleSystemTransform;
		playgroundSpline.transform.localPosition = Vector3.zero;
		playgroundSpline.name = playgroundSpline.name + " " + playgroundParticles.splines.Count;
		playgroundSpline.AddUser(playgroundParticles.particleSystemTransform);
		return playgroundSpline;
	}

	public static PlaygroundSpline CreateSpline(ManipulatorPropertyC manipulatorProperty)
	{
		PlaygroundSpline playgroundSpline = CreateSpline();
		playgroundSpline.Reset();
		manipulatorProperty.splineTarget = playgroundSpline;
		return playgroundSpline;
	}

	public static PlaygroundEventC CreateEvent()
	{
		return new PlaygroundEventC();
	}

	public static PlaygroundEventC CreateEvent(PlaygroundParticlesC playgroundParticles)
	{
		if (playgroundParticles == null)
		{
			return null;
		}
		if (playgroundParticles.events == null)
		{
			playgroundParticles.events = new List<PlaygroundEventC>();
		}
		playgroundParticles.events.Add(new PlaygroundEventC());
		return playgroundParticles.events[playgroundParticles.events.Count - 1];
	}

	public static PlaygroundEventC GetEvent(int i, PlaygroundParticlesC playgroundParticles)
	{
		if (playgroundParticles.events.Count > 0 && playgroundParticles.events[i % playgroundParticles.events.Count] != null)
		{
			return playgroundParticles.events[i % playgroundParticles.events.Count];
		}
		return null;
	}

	public static void RemoveEvent(int i, PlaygroundParticlesC playgroundParticles)
	{
		i %= playgroundParticles.events.Count;
		if (playgroundParticles.events.Count <= 0 || playgroundParticles.events[i] == null)
		{
			return;
		}
		if (playgroundParticles.events[i].target != null)
		{
			for (int j = 0; j < playgroundParticles.events[i].target.eventControlledBy.Count; j++)
			{
				if (playgroundParticles.events[i].target.eventControlledBy[j] == playgroundParticles)
				{
					playgroundParticles.events[i].target.eventControlledBy.RemoveAt(j);
				}
			}
		}
		playgroundParticles.events.RemoveAt(i);
	}

	public static PlaygroundParticlesC GetParticles(int i)
	{
		if (reference.particleSystems.Count > 0 && reference.particleSystems[i % reference.particleSystems.Count] != null)
		{
			return reference.particleSystems[i % reference.particleSystems.Count];
		}
		return null;
	}

	public static ParticleProjectionC ParticleProjection(PlaygroundParticlesC playgroundParticles)
	{
		return PlaygroundParticlesC.NewProjectionObject(playgroundParticles);
	}

	public static PaintObjectC PaintObject(PlaygroundParticlesC playgroundParticles)
	{
		return PlaygroundParticlesC.NewPaintObject(playgroundParticles);
	}

	public static int Paint(PlaygroundParticlesC playgroundParticles, Vector3 position, Vector3 normal, Transform parent, Color32 color)
	{
		return playgroundParticles.paint.Paint(position, normal, parent, color);
	}

	public static void Paint(PaintObjectC paintObject, Vector3 position, Vector3 normal, Transform parent, Color32 color)
	{
		paintObject.Paint(position, normal, parent, color);
	}

	public static bool Erase(PlaygroundParticlesC playgroundParticles, Vector3 position, float radius)
	{
		return playgroundParticles.paint.Erase(position, radius);
	}

	public static bool Erase(PaintObjectC paintObject, Vector3 position, float radius)
	{
		return paintObject.Erase(position, radius);
	}

	public static bool Erase(PlaygroundParticlesC playgroundParticles, int index)
	{
		return playgroundParticles.paint.Erase(index);
	}

	public static void ClearPaint(PlaygroundParticlesC playgroundParticles)
	{
		if (playgroundParticles.paint != null)
		{
			playgroundParticles.paint.ClearPaint();
		}
	}

	public static int GetPaintPositionLength(PlaygroundParticlesC playgroundParticles)
	{
		return playgroundParticles.paint.positionLength;
	}

	public static void SetInitialTargetPosition(PlaygroundParticlesC playgroundParticles, Vector3 position)
	{
		PlaygroundParticlesC.SetInitialTargetPosition(playgroundParticles, position, refreshParticleSystem: true);
	}

	public static void Emission(PlaygroundParticlesC playgroundParticles, bool emit)
	{
		PlaygroundParticlesC.Emission(playgroundParticles, emit, callRestEmission: false);
	}

	public static void Emission(PlaygroundParticlesC playgroundParticles, bool emit, bool restEmission)
	{
		PlaygroundParticlesC.Emission(playgroundParticles, emit, restEmission);
	}

	public static void Clear(PlaygroundParticlesC playgroundParticles)
	{
		PlaygroundParticlesC.Clear(playgroundParticles);
	}

	public static void RefreshScatter(PlaygroundParticlesC playgroundParticles)
	{
		playgroundParticles.RefreshScatter();
	}

	public static PlaygroundParticlesC InstantiatePreset(string presetName)
	{
		GameObject gameObject = ResourceInstantiate("Presets/" + presetName);
		PlaygroundParticlesC component = gameObject.GetComponent<PlaygroundParticlesC>();
		if (component != null)
		{
			if (reference == null)
			{
				reference = ResourceInstantiate("Playground Manager").GetComponent<PlaygroundC>();
			}
			if ((bool)reference)
			{
				if (reference.autoGroup && component.particleSystemTransform.parent == null)
				{
					component.particleSystemTransform.parent = referenceTransform;
				}
				particlesQuantity++;
				component.particleSystemId = particlesQuantity - 1;
			}
			gameObject.name = presetName;
			return component;
		}
		if (gameObject.name.Contains("Presets/"))
		{
			gameObject.name = gameObject.name.Remove(0, 8);
		}
		return null;
	}

	public static PlaygroundParticlesC InstantiatePreset(string categoryName, string presetName)
	{
		return InstantiatePreset(categoryName + "/" + presetName);
	}

	public static bool HasReference()
	{
		return hasReference;
	}

	public static bool HasPlaygroundPool()
	{
		return hasPlaygroundPool;
	}

	public bool IsDoneThread()
	{
		return isDoneThread;
	}

	public bool IsDoneThreadTurbulence()
	{
		return isDoneThreadTurbulence;
	}

	public bool IsDoneThreadSkinnedMeshes()
	{
		return isDoneThreadSkinned;
	}

	public bool IsFirstUnsafeAutomaticFrames()
	{
		return isFirstUnsafeAutomaticFrames;
	}

	public bool HasEnabledGlobalManipulators()
	{
		if (manipulators.Count > 0)
		{
			for (int i = 0; i < manipulators.Count; i++)
			{
				if (manipulators[i].enabled && manipulators[i].transform != null && manipulators[i].transform.transform != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static Color32[] GetPixels(Texture2D image)
	{
		if (image == null)
		{
			return null;
		}
		if ((bool)PlaygroundC.reference && PlaygroundC.reference.pixelFilterMode == PIXELMODEC.Bilinear)
		{
			Color32[] array = new Color32[image.width * image.height];
			for (int i = 0; i < image.height; i++)
			{
				for (int j = 0; j < image.width; j++)
				{
					ref Color32 reference = ref array[i * image.width + j];
					reference = image.GetPixelBilinear((float)j * 1f / (float)image.width, (float)i * 1f / (float)image.height);
				}
			}
			return array;
		}
		return image.GetPixels32();
	}

	public static Vector3 Offset(PLAYGROUNDORIGINC origin, int imageWidth, int imageHeight, float meshScale)
	{
		Vector3 zero = Vector3.zero;
		switch (origin)
		{
		case PLAYGROUNDORIGINC.TopLeft:
			zero.y = (float)(-imageHeight) * meshScale;
			break;
		case PLAYGROUNDORIGINC.TopCenter:
			zero.y = (float)(-imageHeight) * meshScale;
			zero.x = (float)(-imageWidth) * meshScale / 2f;
			break;
		case PLAYGROUNDORIGINC.TopRight:
			zero.y = (float)(-imageHeight) * meshScale;
			zero.x = (float)(-imageWidth) * meshScale;
			break;
		case PLAYGROUNDORIGINC.MiddleLeft:
			zero.y = (float)(-imageHeight) * meshScale / 2f;
			break;
		case PLAYGROUNDORIGINC.MiddleCenter:
			zero.y = (float)(-imageHeight) * meshScale / 2f;
			zero.x = (float)(-imageWidth) * meshScale / 2f;
			break;
		case PLAYGROUNDORIGINC.MiddleRight:
			zero.y = (float)(-imageHeight) * meshScale / 2f;
			zero.x = (float)(-imageWidth) * meshScale;
			break;
		case PLAYGROUNDORIGINC.BottomCenter:
			zero.x = (float)(-imageWidth) * meshScale / 2f;
			break;
		case PLAYGROUNDORIGINC.BottomRight:
			zero.x = (float)(-imageWidth) * meshScale;
			break;
		}
		return zero;
	}

	public static Vector3[] RandomVector3(int length, Vector3 min, Vector3 max)
	{
		Vector3[] array = new Vector3[length];
		for (int i = 0; i < length; i++)
		{
			ref Vector3 reference = ref array[i];
			reference = new Vector3(PlaygroundParticlesC.RandomRange(random, min.x, max.x), PlaygroundParticlesC.RandomRange(random, min.y, max.y), PlaygroundParticlesC.RandomRange(random, min.z, max.z));
		}
		return array;
	}

	public static float[] RandomFloat(int length, float min, float max)
	{
		float[] array = new float[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = PlaygroundParticlesC.RandomRange(random, min, max);
		}
		return array;
	}

	public static void ShuffleArray(float[] arr)
	{
		for (int num = arr.Length - 1; num > 0; num--)
		{
			int num2 = random.Next(0, num);
			float num3 = arr[num];
			arr[num] = arr[num2];
			arr[num2] = num3;
		}
	}

	public static void ShuffleArray(int[] arr)
	{
		for (int num = arr.Length - 1; num > 0; num--)
		{
			int num2 = random.Next(0, num);
			int num3 = arr[num];
			arr[num] = arr[num2];
			arr[num2] = num3;
		}
	}

	public static int Largest(int[] compare)
	{
		int num = 0;
		for (int i = 0; i < compare.Length; i++)
		{
			if (compare[i] > num)
			{
				num = i;
			}
		}
		return num;
	}

	public static int CountZeroAlphasInTexture(Texture2D image)
	{
		int num = 0;
		Color32[] pixels = image.GetPixels32();
		for (int i = 0; i < pixels.Length; i++)
		{
			if (pixels[i].a == 0)
			{
				num++;
			}
		}
		return num;
	}

	public static GameObject ResourceInstantiate(string n)
	{
		GameObject gameObject = Resources.Load(n) as GameObject;
		if (!gameObject)
		{
			return null;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		gameObject2.name = n;
		return gameObject2;
	}

	public static void TimeReset()
	{
		globalDeltaTime = 0f;
		globalTime = Time.timeSinceLevelLoad;
		lastTimeUpdated = globalTime;
	}

	public IEnumerator InitializePlayground()
	{
		if (reference != null && reference != this)
		{
			yield return null;
			Debug.Log("There can only be one instance of the Playground Manager in the scene.");
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					transform.parent = null;
				}
			}
			finally
			{
				IDisposable disposable2;
				IDisposable disposable = (disposable2 = enumerator as IDisposable);
				if (disposable2 != null)
				{
					disposable.Dispose();
				}
			}
			UnityEngine.Object.DestroyImmediate(gameObject);
			yield break;
		}
		if (reference == null)
		{
			reference = this;
		}
		CheckEvents();
		random = new System.Random();
		TimeReset();
		for (int i = 0; i < particleSystems.Count; i++)
		{
			if (particleSystems[i] != null)
			{
				particleSystems[i].particleSystemId = i;
				continue;
			}
			particleSystems.RemoveAt(i);
			i--;
		}
		particlesQuantity = particleSystems.Count;
		frameCount = 0;
		threadAggregatorRuns = 0;
		isReady = true;
		isDoneThread = true;
		isDoneThreadLocal = true;
		isDoneThreadSkinned = true;
		isDoneThreadTurbulence = true;
		SetMaxThreads(maxThreads);
	}

	public static bool IsReady()
	{
		return reference.isReady;
	}

	public static void SetTime()
	{
		if (!Application.isPlaying || !reference.globalTimeScale)
		{
			globalTime += (Time.realtimeSinceStartup - lastTimeUpdated) * globalTimescale;
		}
		else
		{
			globalTime += Time.deltaTime * globalTimescale;
		}
		globalDeltaTime = globalTime - lastTimeUpdated;
		lastTimeUpdated = globalTime;
	}

	public static void SetMaxThreads(int threadCount)
	{
		if (HasReference())
		{
			reference.maxThreads = Mathf.Clamp(threadCount, 1, 128);
			processorCount = SystemInfo.processorCount;
			reference.threads = Mathf.Clamp((processorCount <= 1) ? 1 : (processorCount - 1), 1, reference.maxThreads);
			reference._prevMaxThreadCount = reference.maxThreads;
			if (reference.threadPoolMethod == ThreadPoolMethod.PlaygroundPool && playgroundPool != null)
			{
				playgroundPool.SetThreadPool(reference.maxThreads);
			}
		}
	}

	private static void CheckEvents()
	{
		particleEventBirthInitialized = particleEventBirth != null;
		particleEventDeathInitialized = particleEventDeath != null;
		particleEventCollisionInitialized = particleEventCollision != null;
		particleEventTimeInitialized = particleEventTime != null;
	}

	private void OnEnable()
	{
		referenceTransform = transform;
		referenceGameObject = gameObject;
		StartCoroutine(InitializePlayground());
	}

	private void Start()
	{
		SetTime();
	}

	private void LateUpdate()
	{
		activeThreads = 0;
		if (activeSystems.Count > 0)
		{
			activeSystems.Clear();
		}
		frameCount++;
		isFirstUnsafeAutomaticFrames = frameCount < unsafeAutomaticThreadFrames || threadAggregatorRuns < unsafeAutomaticThreadFrames;
		if (reference != this)
		{
			InitializePlayground();
			return;
		}
		SetTime();
		CheckEvents();
		hasReference = reference != null;
		hasPlaygroundPool = playgroundPool != null;
		if (threadPoolMethod == ThreadPoolMethod.PlaygroundPool && maxThreads != _prevMaxThreadCount)
		{
			SetMaxThreads(maxThreads);
		}
		for (int i = 0; i < manipulators.Count; i++)
		{
			manipulators[i].Update();
		}
		if (particleSystems.Count == 0)
		{
			return;
		}
		hasActiveParticleSystems = false;
		hasLocalNoThreads = false;
		hasLocalOnePerSystem = false;
		hasLocalOneForAll = false;
		hasActiveSkinnedMeshes = false;
		hasActiveTurbulence = false;
		if (previousThreadMethod != threadMethod)
		{
			isDoneThread = true;
			previousThreadMethod = threadMethod;
		}
		if (threadMethod != ThreadMethod.OneForAll)
		{
			isDoneThread = true;
		}
		if (previousSkinnedMeshThreadMethod != skinnedMeshThreadMethod)
		{
			isDoneThreadSkinned = true;
			previousSkinnedMeshThreadMethod = skinnedMeshThreadMethod;
		}
		if (previousTurbulenceThreadMethod != turbulenceThreadMethod)
		{
			isDoneThreadTurbulence = true;
			previousTurbulenceThreadMethod = turbulenceThreadMethod;
		}
		Camera main = Camera.main;
		bool flag = false;
		bool flag2 = false;
		int count = particleSystems.Count;
		for (int j = 0; j < particleSystems.Count; j++)
		{
			if (particleSystems[j] == null)
			{
				particleSystems.RemoveAt(j);
				j--;
				if (j < 0)
				{
					j = 0;
				}
			}
			else if (!particleSystems[j].isSnapshot && particleSystems[j].shurikenParticleSystem != null && particleSystems[j].particleSystemGameObject.activeInHierarchy && particleSystems[j].Initialized() && !particleSystems[j].IsSettingParticleCount() && !particleSystems[j].IsSettingLifetime() && !particleSystems[j].IsYieldRefreshing() && !particleSystems[j].InTransition() && !particleSystems[j].IsLoading() && !particleSystems[j].IsPrewarming() && !particleSystems[j].IsSettingParticleTime() && !particleSystems[j].inPlayback)
			{
				if (Time.frameCount % particleSystems[j].updateRate != 0)
				{
					for (int k = 0; k < particleSystems[j].particleCount; k++)
					{
						particleSystems[j].particleCache[k].remainingLifetime = particleSystems[j].playgroundCache.death[k] - particleSystems[j].playgroundCache.birth[k] - particleSystems[j].playgroundCache.lifetimeSubtraction[k];
					}
					particleSystems[j].isReadyForThreadedCalculations = false;
					continue;
				}
				if (count > particleSystems.Count)
				{
					if (j <= 0)
					{
						hasActiveParticleSystems = false;
						return;
					}
					j--;
					count = particleSystems.Count;
				}
				else if (count < particleSystems.Count)
				{
					count = particleSystems.Count;
				}
				if (particleSystems.Count == 0 || j >= particleSystems.Count)
				{
					hasActiveParticleSystems = false;
					return;
				}
				if (!flag && particleSystems[j].pauseCalculationWhenInvisible && main != null && main.enabled)
				{
					frustumPlanes = GeometryUtility.CalculateFrustumPlanes(main);
					flag = true;
				}
				if ((particleSystems[j].onlySourcePositioning || particleSystems[j].onlyLifetimePositioning) && !particleSystems[j].UpdateSystem())
				{
					continue;
				}
				if (particleSystems.Count > 0 && j < particleSystems.Count)
				{
					particleSystems[j].isReadyForThreadedCalculations = particleSystems[j].PrepareThreadedCalculations();
					if (particleSystems[j].IsAlive())
					{
						hasActiveParticleSystems = true;
						if (particleSystems[j].onlySourcePositioning || particleSystems[j].onlyLifetimePositioning)
						{
							flag2 = true;
						}
						if (threadMethod == ThreadMethod.Automatic)
						{
							activeSystems.Add(j);
						}
						if (!particleSystems[j].forceSkinnedMeshUpdateOnMainThread && particleSystems[j].IsSkinnedWorldObjectReady() && particleSystems[j].calculate)
						{
							hasActiveSkinnedMeshes = true;
						}
						if (particleSystems[j].HasTurbulence() && particleSystems[j].calculate)
						{
							hasActiveTurbulence = true;
						}
						switch (particleSystems[j].threadMethod)
						{
						case ThreadMethodLocal.NoThreads:
							hasLocalNoThreads = true;
							break;
						case ThreadMethodLocal.OnePerSystem:
							hasLocalOnePerSystem = true;
							break;
						case ThreadMethodLocal.OneForAll:
							hasLocalOneForAll = true;
							break;
						}
					}
					else
					{
						particleSystems[j].isReadyForThreadedCalculations = false;
					}
				}
				else if (particleSystems.Count > 0 && j < particleSystems.Count && particleSystems[j] != null)
				{
					particleSystems[j].isReadyForThreadedCalculations = false;
				}
			}
			else if (particleSystems[j] != null)
			{
				particleSystems[j].isReadyForThreadedCalculations = false;
				if (particleSystems[j].IsPrewarming() && particleSystems[j].HasTurbulence())
				{
					hasActiveTurbulence = true;
					hasActiveParticleSystems = true;
				}
			}
		}
		if (!hasActiveParticleSystems)
		{
			return;
		}
		ThreadAggregator();
		if (!flag2)
		{
			return;
		}
		for (int l = 0; l < particleSystems.Count; l++)
		{
			if (particleSystems[l] != null && (particleSystems[l].onlySourcePositioning || particleSystems[l].onlyLifetimePositioning) && particleSystems[l].isReadyForThreadedCalculations && !particleSystems[l].InTransition() && !particleSystems[l].IsLoading() && particleSystems[l].IsReady())
			{
				particleSystems[l].UpdateShuriken();
			}
		}
	}

	private void Update()
	{
		int count = particleSystems.Count;
		for (int i = 0; i < particleSystems.Count; i++)
		{
			if (count > particleSystems.Count)
			{
				if (i <= 0)
				{
					hasActiveParticleSystems = false;
					break;
				}
				i--;
				count = particleSystems.Count;
			}
			else if (count < particleSystems.Count)
			{
				count = particleSystems.Count;
			}
			if (particleSystems.Count == 0 || i >= particleSystems.Count)
			{
				hasActiveParticleSystems = false;
				break;
			}
			if (particleSystems[i] != null && !particleSystems[i].onlySourcePositioning && !particleSystems[i].onlyLifetimePositioning && particleSystems[i].UpdateSystem() && particleSystems[i].isReadyForThreadedCalculations && !particleSystems[i].InTransition() && !particleSystems[i].IsLoading() && !particleSystems[i].IsSettingParticleTime() && particleSystems[i].IsReady() && !particleSystems[i].inPlayback)
			{
				particleSystems[i].UpdateShuriken();
			}
		}
	}

	public static int ActiveThreads()
	{
		return activeThreads;
	}

	public static int ProcessorCount()
	{
		return processorCount;
	}

	private void ThreadAggregator()
	{
		if (hasActiveSkinnedMeshes)
		{
			switch (skinnedMeshThreadMethod)
			{
			case ThreadMethodComponent.OnePerSystem:
			{
				for (int num = 0; num < particleSystems.Count; num++)
				{
					if (!particleSystems[num].forceSkinnedMeshUpdateOnMainThread && particleSystems[num].IsSkinnedWorldObjectReady())
					{
						particleSystems[num].skinnedWorldObject.UpdateOnNewThread();
					}
				}
				break;
			}
			case ThreadMethodComponent.OneForAll:
				if (!isDoneThreadSkinned)
				{
					break;
				}
				isDoneThreadSkinned = false;
				RunAsync(() =>
				{
					if (!isDoneThreadSkinned)
					{
						for (int i = 0; i < particleSystems.Count; i++)
						{
							if (!particleSystems[i].forceSkinnedMeshUpdateOnMainThread && particleSystems[i].IsSkinnedWorldObjectReady())
							{
								particleSystems[i].skinnedWorldObject.Update();
							}
						}
						isDoneThreadSkinned = true;
					}
				});
				break;
			}
		}
		if (hasActiveTurbulence && turbulenceThreadMethod == ThreadMethodComponent.OneForAll && isDoneThreadTurbulence)
		{
			isDoneThreadTurbulence = false;
			RunAsync(() =>
			{
				if (!isDoneThreadTurbulence)
				{
					for (int i = 0; i < particleSystems.Count; i++)
					{
						if (particleSystems[i].HasTurbulence())
						{
							for (int j = 0; j < particleSystems[i].particleCount; j++)
							{
								if (!particleSystems[i].playgroundCache.noForce[j])
								{
									PlaygroundParticlesC.Turbulence(particleSystems[i], particleSystems[i].GetSimplex(), j, particleSystems[i].GetDeltaTime(), particleSystems[i].turbulenceType, particleSystems[i].turbulenceTimeScale, particleSystems[i].turbulenceScale / particleSystems[i].velocityScale, particleSystems[i].turbulenceStrength, particleSystems[i].turbulenceApplyLifetimeStrength, particleSystems[i].turbulenceLifetimeStrength);
								}
							}
						}
						isDoneThreadTurbulence = true;
					}
				}
			});
		}
		if (hasLocalNoThreads)
		{
			for (int num2 = 0; num2 < particleSystems.Count; num2++)
			{
				if (particleSystems[num2].isReadyForThreadedCalculations && particleSystems[num2].threadMethod == ThreadMethodLocal.NoThreads)
				{
					PlaygroundParticlesC.ThreadedCalculations(particleSystems[num2]);
				}
			}
		}
		if (hasLocalOnePerSystem)
		{
			for (int num3 = 0; num3 < particleSystems.Count; num3++)
			{
				if (particleSystems[num3].threadMethod == ThreadMethodLocal.OnePerSystem)
				{
					PlaygroundParticlesC.NewCalculatedThread(particleSystems[num3]);
				}
			}
		}
		if ((hasLocalOneForAll || (isFirstUnsafeAutomaticFrames && threadMethod == ThreadMethod.Automatic)) && isDoneThreadLocal)
		{
			isDoneThreadLocal = false;
			RunAsync(() =>
			{
				if (isDoneThreadLocal)
				{
					return;
				}
				lock (lockerLocal)
				{
					for (int i = 0; i < particleSystems.Count; i++)
					{
						if (particleSystems[i].isReadyForThreadedCalculations && (particleSystems[i].threadMethod == ThreadMethodLocal.OneForAll || isFirstUnsafeAutomaticFrames))
						{
							particleSystems[i].IsDoneThread = true;
							PlaygroundParticlesC.ThreadedCalculations(particleSystems[i]);
						}
					}
					isDoneThreadLocal = true;
				}
			});
		}
		switch (threadMethod)
		{
		case ThreadMethod.NoThreads:
		{
			for (int num7 = 0; num7 < particleSystems.Count; num7++)
			{
				if (particleSystems[num7].isReadyForThreadedCalculations && particleSystems[num7].threadMethod == ThreadMethodLocal.Inherit)
				{
					PlaygroundParticlesC.ThreadedCalculations(particleSystems[num7]);
				}
			}
			break;
		}
		case ThreadMethod.OnePerSystem:
		{
			for (int num6 = 0; num6 < particleSystems.Count; num6++)
			{
				if (particleSystems[num6].threadMethod == ThreadMethodLocal.Inherit && particleSystems[num6].isReadyForThreadedCalculations)
				{
					PlaygroundParticlesC.NewCalculatedThread(particleSystems[num6]);
				}
			}
			break;
		}
		case ThreadMethod.OneForAll:
			if (!isDoneThread)
			{
				break;
			}
			isDoneThread = false;
			RunAsync(() =>
			{
				if (isDoneThread)
				{
					return;
				}
				lock (locker)
				{
					for (int i = 0; i < particleSystems.Count; i++)
					{
						if (particleSystems[i].isReadyForThreadedCalculations && particleSystems[i].threadMethod == ThreadMethodLocal.Inherit)
						{
							PlaygroundParticlesC.ThreadedCalculations(particleSystems[i]);
						}
					}
					isDoneThread = true;
				}
			});
			break;
		case ThreadMethod.Automatic:
		{
			if (isFirstUnsafeAutomaticFrames)
			{
				break;
			}
			maxThreads = Mathf.Clamp(maxThreads, 1, 128);
			threads = Mathf.Clamp((processorCount <= 1) ? 1 : (processorCount - 1), 1, maxThreads);
			if (activeSystems.Count <= threads)
			{
				for (int num4 = 0; num4 < particleSystems.Count; num4++)
				{
					if (particleSystems[num4].isReadyForThreadedCalculations && particleSystems[num4].threadMethod == ThreadMethodLocal.Inherit)
					{
						PlaygroundParticlesC.NewCalculatedThread(particleSystems[num4]);
					}
				}
				break;
			}
			float f = (float)activeSystems.Count * 1f / (float)threads * 1f;
			playgroundPool.actionsPerThread = Mathf.RoundToInt(f);
			for (int num5 = 0; num5 < particleSystems.Count; num5++)
			{
				if (particleSystems[num5].isReadyForThreadedCalculations && particleSystems[num5].threadMethod == ThreadMethodLocal.Inherit)
				{
					PlaygroundParticlesC.NewCalculatedThread(particleSystems[num5]);
				}
			}
			break;
		}
		}
		if (threadMethod != ThreadMethod.NoThreads && activeThreads == 0)
		{
			activeThreads = 1;
		}
		threadAggregatorRuns++;
	}

	public static void RunAsync(Action a, ThreadPoolMethod threadPoolMethod)
	{
		if (threadPoolMethod == ThreadPoolMethod.ThreadPool)
		{
			lock (locker)
			{
				ThreadPool.QueueUserWorkItem(RunAction, a);
				return;
			}
		}
		RunAsyncPlaygroundPool(a);
	}

	public static void RunAsync(Action a)
	{
		if (!HasReference() || reference.threadPoolMethod == ThreadPoolMethod.ThreadPool)
		{
			lock (locker)
			{
				ThreadPool.QueueUserWorkItem(RunAction, a);
				return;
			}
		}
		RunAsyncPlaygroundPool(a);
	}

	public static void RunAsyncPlaygroundPool(Action a)
	{
		if (!HasPlaygroundPool())
		{
			playgroundPool = new PlaygroundQueue<Action>((!HasReference()) ? (SystemInfo.processorCount * 2) : reference.maxThreads, RunAction);
			hasPlaygroundPool = true;
		}
		playgroundPool.EnqueueTask(a);
	}

	private static void RunAction(object a)
	{
		try
		{
			((Action)a)();
		}
		catch
		{
		}
	}
}
