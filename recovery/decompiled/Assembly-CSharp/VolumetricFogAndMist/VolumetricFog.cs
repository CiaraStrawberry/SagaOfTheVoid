using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace VolumetricFogAndMist;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Rendering/Volumetric Fog & Mist")]
[HelpURL("http://kronnect.com/taptapgo")]
public class VolumetricFog : MonoBehaviour
{
	private class FogOfWarClearance
	{
		public bool enabled;

		public int x;

		public int y;

		public float clearTime;

		public byte alphaSet;
	}

	public const string SKW_FOG_DISTANCE_ON = "FOG_DISTANCE_ON";

	public const string SKW_LIGHT_SCATTERING = "FOG_SCATTERING_ON";

	public const string SKW_FOG_AREA_BOX = "FOG_AREA_BOX";

	public const string SKW_FOG_AREA_SPHERE = "FOG_AREA_SPHERE";

	public const string SKW_FOG_VOID_BOX = "FOG_VOID_BOX";

	public const string SKW_FOG_VOID_SPHERE = "FOG_VOID_SPHERE";

	public const string SKW_FOG_HAZE_ON = "FOG_HAZE_ON";

	public const string SKW_FOG_OF_WAR_ON = "FOG_OF_WAR_ON";

	public const string SKW_FOG_BLUR = "FOG_BLUR_ON";

	private const float TIME_BETWEEN_TEXTURE_UPDATES = 0.2f;

	private static VolumetricFog _fog;

	[HideInInspector]
	public bool isDirty;

	[SerializeField]
	private FOG_PRESET _preset = FOG_PRESET.Mist;

	[SerializeField]
	private bool _useFogVolumes;

	[SerializeField]
	private bool _improveTransparency;

	[SerializeField]
	private bool _renderOpaque = true;

	[SerializeField]
	private GameObject _sun;

	[SerializeField]
	private bool _sunCopyColor = true;

	[SerializeField]
	private float _density = 1f;

	[SerializeField]
	private float _noiseStrength = 0.8f;

	[SerializeField]
	private float _distance;

	[SerializeField]
	private float _maxFogLength = 1000f;

	[SerializeField]
	private float _maxFogLengthFallOff;

	[SerializeField]
	private float _distanceFallOff;

	[SerializeField]
	private float _height = 4f;

	[SerializeField]
	private float _baselineHeight;

	[SerializeField]
	private bool _baselineRelativeToCamera;

	[SerializeField]
	private float _baselineRelativeToCameraDelay;

	[SerializeField]
	private float _noiseScale = 1f;

	[SerializeField]
	private float _alpha = 1f;

	[SerializeField]
	private Color _color = new Color(0.89f, 0.89f, 0.89f, 1f);

	[SerializeField]
	private Color _specularColor = new Color(1f, 1f, 0.8f, 1f);

	[SerializeField]
	private float _specularThreshold = 0.6f;

	[SerializeField]
	private float _specularIntensity = 0.2f;

	[SerializeField]
	private Vector3 _lightDirection = new Vector3(1f, 0f, -1f);

	[SerializeField]
	private float _lightIntensity = 0.2f;

	[SerializeField]
	private Color _lightColor = Color.white;

	[SerializeField]
	private float _speed = 0.01f;

	[SerializeField]
	private Vector3 _windDirection = new Vector3(-1f, 0f, 0f);

	[SerializeField]
	private Color _skyColor = new Color(0.89f, 0.89f, 0.89f, 1f);

	[SerializeField]
	private float _skyHaze = 50f;

	[SerializeField]
	private float _skySpeed = 0.3f;

	[SerializeField]
	private float _skyNoiseStrength = 0.1f;

	[SerializeField]
	private float _skyAlpha = 1f;

	[SerializeField]
	private float _skyDepth = 0.999f;

	[SerializeField]
	private GameObject _character;

	[SerializeField]
	private float _fogVoidFallOff = 1f;

	[SerializeField]
	private float _fogVoidRadius;

	[SerializeField]
	private Vector3 _fogVoidPosition = Vector3.zero;

	[SerializeField]
	private float _fogVoidDepth;

	[SerializeField]
	private float _fogVoidHeight;

	[SerializeField]
	private bool _fogVoidInverted;

	[SerializeField]
	private GameObject _fogAreaCenter;

	[SerializeField]
	private float _fogAreaFallOff = 1f;

	[SerializeField]
	private float _fogAreaRadius;

	[SerializeField]
	private Vector3 _fogAreaPosition = Vector3.zero;

	[SerializeField]
	private float _fogAreaDepth;

	[SerializeField]
	private float _fogAreaHeight;

	public const int MAX_POINT_LIGHTS = 6;

	[SerializeField]
	private GameObject[] _pointLights = new GameObject[6];

	[SerializeField]
	private float[] _pointLightRanges = new float[6];

	[SerializeField]
	private float[] _pointLightIntensities = new float[6] { 1f, 1f, 1f, 1f, 1f, 1f };

	[SerializeField]
	private float[] _pointLightIntensitiesMultiplier = new float[6] { 1f, 1f, 1f, 1f, 1f, 1f };

	[SerializeField]
	private Vector3[] _pointLightPositions = new Vector3[6];

	[SerializeField]
	private Color[] _pointLightColors = new Color[6]
	{
		new Color(1f, 1f, 0f, 1f),
		new Color(1f, 1f, 0f, 1f),
		new Color(1f, 1f, 0f, 1f),
		new Color(1f, 1f, 0f, 1f),
		new Color(1f, 1f, 0f, 1f),
		new Color(1f, 1f, 0f, 1f)
	};

	[SerializeField]
	private bool _pointLightTrackingAuto;

	[SerializeField]
	private int _pointLightTrackingCount;

	[SerializeField]
	private float _pointLightTrackingCheckInterval = 1f;

	[SerializeField]
	private int _downsampling = 1;

	[SerializeField]
	private bool _edgeImprove;

	[SerializeField]
	private float _edgeThreshold = 0.0005f;

	[SerializeField]
	private float _stepping = 12f;

	[SerializeField]
	private float _steppingNear = 1f;

	[SerializeField]
	private bool _dithering;

	[SerializeField]
	private float _ditherStrength = 0.75f;

	[SerializeField]
	private bool _jitter;

	[SerializeField]
	private float _jitterrStrength = 3f;

	[SerializeField]
	private bool _lightScatteringEnabled;

	[SerializeField]
	private float _lightScatteringSpread = 0.686f;

	[SerializeField]
	private int _lightScatteringSamples = 16;

	[SerializeField]
	private float _lightScatteringWeight = 2.3f;

	[SerializeField]
	private float _lightScatteringIllumination = 18f;

	[SerializeField]
	private float _lightScatteringDecay = 0.986f;

	[SerializeField]
	private float _lightScatteringExposure = 0.02f;

	[SerializeField]
	private float _lightScatteringJittering = 0.5f;

	[SerializeField]
	private bool _fogBlur;

	[SerializeField]
	private float _fogBlurDepth = 0.05f;

	private Material fogMat;

	private float initialFogAlpha;

	private float targetFogAlpha;

	private float initialSkyHazeAlpha;

	private float targetSkyHazeAlpha;

	private float transitionDuration;

	private float transitionStartTime;

	private float currentFogAlpha;

	private float currentSkyHazeAlpha;

	private RenderTexture depthTexture;

	private RenderTexture rtDest;

	private RenderTexture reducedDestination;

	private GameObject depthCamObj;

	private Camera mainCamera;

	private Camera depthCam;

	private Light sunLight;

	private Light[] pointLightComponents = new Light[6];

	private Texture2D adjustedTexture;

	private Color[] noiseColors;

	private Color[] adjustedColors;

	private float sunLightIntensity = 1f;

	private Vector2 oldSunPos;

	private float sunFade = 1f;

	private float oldBaselineRelativeCameraY;

	private List<string> shaderKeywords;

	private float lastTextureUpdate;

	private Vector3 windSpeedAcum;

	private float skyHazeSpeedAcum;

	private Material blurMat;

	private RenderBuffer[] mrt;

	private Light[] lastFoundLights;

	private Light[] lightBuffer;

	private GameObject[] currentLights = new GameObject[6];

	private float trackPointAutoLastTime;

	private float trackPointCheckNewLightsLastTime;

	private Shader depthShader;

	[SerializeField]
	private bool _fogOfWarEnabled;

	[SerializeField]
	private Vector3 _fogOfWarCenter;

	[SerializeField]
	private Vector3 _fogOfWarSize = new Vector3(1024f, 0f, 1024f);

	[SerializeField]
	private int _fogOfWarTextureSize = 256;

	[SerializeField]
	private float _fogOfWarRestoreDelay;

	[SerializeField]
	private float _fogOfWarRestoreDuration = 2f;

	private Texture2D fogOfWarTexture;

	private Color32[] fogOfWarColorBuffer;

	private List<FogOfWarClearance> fowClearanceList;

	public static VolumetricFog instance
	{
		get
		{
			if (_fog == null)
			{
				if (Camera.main != null)
				{
					_fog = Camera.main.GetComponent<VolumetricFog>();
				}
				if (_fog == null)
				{
					Camera[] allCameras = Camera.allCameras;
					foreach (Camera camera in allCameras)
					{
						_fog = camera.GetComponent<VolumetricFog>();
						if (_fog != null)
						{
							break;
						}
					}
				}
			}
			return _fog;
		}
	}

	public FOG_PRESET preset
	{
		get
		{
			return _preset;
		}
		set
		{
			if (value != _preset)
			{
				_preset = value;
				UpdatePreset();
				isDirty = true;
			}
		}
	}

	public bool useFogVolumes
	{
		get
		{
			return _useFogVolumes;
		}
		set
		{
			if (value != _useFogVolumes)
			{
				_useFogVolumes = value;
				isDirty = true;
			}
		}
	}

	public bool improveTransparency
	{
		get
		{
			return _improveTransparency;
		}
		set
		{
			if (value != _improveTransparency)
			{
				_improveTransparency = value;
				if (_improveTransparency)
				{
					renderOpaque = true;
				}
				isDirty = true;
			}
		}
	}

	public bool renderOpaque
	{
		get
		{
			return _renderOpaque;
		}
		set
		{
			if (value != _renderOpaque)
			{
				_renderOpaque = value;
				if (!_renderOpaque)
				{
					_improveTransparency = false;
				}
				UpdateRenderComponents();
				isDirty = true;
			}
		}
	}

	public GameObject sun
	{
		get
		{
			return _sun;
		}
		set
		{
			if (value != _sun)
			{
				_sun = value;
				UpdateSun();
			}
		}
	}

	public bool sunCopyColor
	{
		get
		{
			return _sunCopyColor;
		}
		set
		{
			if (value != _sunCopyColor)
			{
				_sunCopyColor = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float density
	{
		get
		{
			return _density;
		}
		set
		{
			if (value != _density)
			{
				_preset = FOG_PRESET.Custom;
				_density = value;
				UpdateMaterialProperties();
				UpdateTextureAlpha();
				UpdateTexture();
				isDirty = true;
			}
		}
	}

	public float noiseStrength
	{
		get
		{
			return _noiseStrength;
		}
		set
		{
			if (value != _noiseStrength)
			{
				_preset = FOG_PRESET.Custom;
				_noiseStrength = value;
				UpdateMaterialProperties();
				UpdateTextureAlpha();
				UpdateTexture();
				isDirty = true;
			}
		}
	}

	public float distance
	{
		get
		{
			return _distance;
		}
		set
		{
			if (value != _distance)
			{
				_preset = FOG_PRESET.Custom;
				_distance = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float maxFogLength
	{
		get
		{
			return _maxFogLength;
		}
		set
		{
			if (value != _maxFogLength)
			{
				_maxFogLength = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float maxFogLengthFallOff
	{
		get
		{
			return _maxFogLengthFallOff;
		}
		set
		{
			if (value != _maxFogLengthFallOff)
			{
				_maxFogLengthFallOff = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float distanceFallOff
	{
		get
		{
			return _distanceFallOff;
		}
		set
		{
			if (value != _distanceFallOff)
			{
				_preset = FOG_PRESET.Custom;
				_distanceFallOff = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float height
	{
		get
		{
			return _height;
		}
		set
		{
			if (value != _height)
			{
				_preset = FOG_PRESET.Custom;
				_height = Mathf.Max(value, 1E-05f);
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float baselineHeight
	{
		get
		{
			return _baselineHeight;
		}
		set
		{
			if (value != _baselineHeight)
			{
				_preset = FOG_PRESET.Custom;
				_baselineHeight = value;
				if (_fogAreaRadius > 0f)
				{
					_fogAreaPosition.y = _baselineHeight;
				}
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public bool baselineRelativeToCamera
	{
		get
		{
			return _baselineRelativeToCamera;
		}
		set
		{
			if (value != _baselineRelativeToCamera)
			{
				_preset = FOG_PRESET.Custom;
				_baselineRelativeToCamera = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float baselineRelativeToCameraDelay
	{
		get
		{
			return _baselineRelativeToCameraDelay;
		}
		set
		{
			if (value != _baselineRelativeToCameraDelay)
			{
				_baselineRelativeToCameraDelay = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float noiseScale
	{
		get
		{
			return _noiseScale;
		}
		set
		{
			if (value != _noiseScale)
			{
				_preset = FOG_PRESET.Custom;
				_noiseScale = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float alpha
	{
		get
		{
			return _alpha;
		}
		set
		{
			if (value != _alpha)
			{
				_preset = FOG_PRESET.Custom;
				_alpha = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public Color color
	{
		get
		{
			return _color;
		}
		set
		{
			if (value != _color)
			{
				_preset = FOG_PRESET.Custom;
				_color = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public Color specularColor
	{
		get
		{
			return _specularColor;
		}
		set
		{
			if (value != _specularColor)
			{
				_preset = FOG_PRESET.Custom;
				_specularColor = value;
				UpdateMaterialProperties();
				UpdateTexture();
				isDirty = true;
			}
		}
	}

	public float specularThreshold
	{
		get
		{
			return _specularThreshold;
		}
		set
		{
			if (value != _specularThreshold)
			{
				_preset = FOG_PRESET.Custom;
				_specularThreshold = value;
				UpdateTexture();
				isDirty = true;
			}
		}
	}

	public float specularIntensity
	{
		get
		{
			return _specularIntensity;
		}
		set
		{
			if (value != _specularIntensity)
			{
				_preset = FOG_PRESET.Custom;
				_specularIntensity = value;
				UpdateMaterialProperties();
				UpdateTexture();
				isDirty = true;
			}
		}
	}

	public Vector3 lightDirection
	{
		get
		{
			return _lightDirection;
		}
		set
		{
			if (value != _lightDirection)
			{
				_preset = FOG_PRESET.Custom;
				_lightDirection = value;
				UpdateMaterialProperties();
				UpdateTexture();
				isDirty = true;
			}
		}
	}

	public float lightIntensity
	{
		get
		{
			return _lightIntensity;
		}
		set
		{
			if (value != _lightIntensity)
			{
				_preset = FOG_PRESET.Custom;
				_lightIntensity = value;
				UpdateMaterialProperties();
				UpdateTexture();
				isDirty = true;
			}
		}
	}

	public Color lightColor
	{
		get
		{
			return _lightColor;
		}
		set
		{
			if (value != _lightColor)
			{
				_preset = FOG_PRESET.Custom;
				_lightColor = value;
				UpdateMaterialProperties();
				UpdateTexture();
				isDirty = true;
			}
		}
	}

	public float speed
	{
		get
		{
			return _speed;
		}
		set
		{
			if (value != _speed)
			{
				_preset = FOG_PRESET.Custom;
				_speed = value;
				UpdateWindSpeedQuick();
				isDirty = true;
			}
		}
	}

	public Vector3 windDirection
	{
		get
		{
			return _windDirection;
		}
		set
		{
			if (value != _windDirection)
			{
				_preset = FOG_PRESET.Custom;
				_windDirection = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public Color skyColor
	{
		get
		{
			return _skyColor;
		}
		set
		{
			if (value != _skyColor)
			{
				_preset = FOG_PRESET.Custom;
				_skyColor = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float skyHaze
	{
		get
		{
			return _skyHaze;
		}
		set
		{
			if (value != _skyHaze)
			{
				_preset = FOG_PRESET.Custom;
				_skyHaze = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float skySpeed
	{
		get
		{
			return _skySpeed;
		}
		set
		{
			if (value != _skySpeed)
			{
				_preset = FOG_PRESET.Custom;
				_skySpeed = value;
				UpdateWindSpeedQuick();
				isDirty = true;
			}
		}
	}

	public float skyNoiseStrength
	{
		get
		{
			return _skyNoiseStrength;
		}
		set
		{
			if (value != _skyNoiseStrength)
			{
				_preset = FOG_PRESET.Custom;
				_skyNoiseStrength = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float skyAlpha
	{
		get
		{
			return _skyAlpha;
		}
		set
		{
			if (value != _skyAlpha)
			{
				_preset = FOG_PRESET.Custom;
				_skyAlpha = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float skyDepth
	{
		get
		{
			return _skyDepth;
		}
		set
		{
			if (value != _skyDepth)
			{
				_skyDepth = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public GameObject character
	{
		get
		{
			return _character;
		}
		set
		{
			if (value != _character)
			{
				_character = value;
				isDirty = true;
				if (_fogVoidRadius < 20f)
				{
					fogVoidRadius = 20f;
				}
			}
		}
	}

	public float fogVoidFallOff
	{
		get
		{
			return _fogVoidFallOff;
		}
		set
		{
			if (value != _fogVoidFallOff)
			{
				_fogVoidFallOff = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float fogVoidRadius
	{
		get
		{
			return _fogVoidRadius;
		}
		set
		{
			if (value != _fogVoidRadius)
			{
				_fogVoidRadius = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public Vector3 fogVoidPosition
	{
		get
		{
			return _fogVoidPosition;
		}
		set
		{
			if (value != _fogVoidPosition)
			{
				_fogVoidPosition = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float fogVoidDepth
	{
		get
		{
			return _fogVoidDepth;
		}
		set
		{
			if (value != _fogVoidDepth)
			{
				_fogVoidDepth = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float fogVoidHeight
	{
		get
		{
			return _fogVoidHeight;
		}
		set
		{
			if (value != _fogVoidHeight)
			{
				_fogVoidHeight = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	[Obsolete("Fog Void inverted is now deprecated. Use Fog Area settings.")]
	public bool fogVoidInverted
	{
		get
		{
			return _fogVoidInverted;
		}
		set
		{
			_fogVoidInverted = value;
		}
	}

	public GameObject fogAreaCenter
	{
		get
		{
			return _fogAreaCenter;
		}
		set
		{
			if (value != _character)
			{
				_fogAreaCenter = value;
				isDirty = true;
			}
		}
	}

	public float fogAreaFallOff
	{
		get
		{
			return _fogAreaFallOff;
		}
		set
		{
			if (value != _fogAreaFallOff)
			{
				_fogAreaFallOff = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float fogAreaRadius
	{
		get
		{
			return _fogAreaRadius;
		}
		set
		{
			if (value != _fogAreaRadius)
			{
				_fogAreaRadius = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public Vector3 fogAreaPosition
	{
		get
		{
			return _fogAreaPosition;
		}
		set
		{
			if (value != _fogAreaPosition)
			{
				_fogAreaPosition = value;
				_baselineHeight = _fogAreaPosition.y;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float fogAreaDepth
	{
		get
		{
			return _fogAreaDepth;
		}
		set
		{
			if (value != _fogAreaDepth)
			{
				_fogAreaDepth = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float fogAreaHeight
	{
		get
		{
			return _fogAreaHeight;
		}
		set
		{
			if (value != _fogAreaHeight)
			{
				_fogAreaHeight = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public bool pointLightTrackAuto
	{
		get
		{
			return _pointLightTrackingAuto;
		}
		set
		{
			if (value != _pointLightTrackingAuto)
			{
				_pointLightTrackingAuto = value;
				TrackPointLights();
				isDirty = true;
			}
		}
	}

	public int pointLightTrackingCount
	{
		get
		{
			return _pointLightTrackingCount;
		}
		set
		{
			if (value != _pointLightTrackingCount)
			{
				_pointLightTrackingCount = Mathf.Clamp(value, 0, 6);
				TrackPointLights();
				isDirty = true;
			}
		}
	}

	public float pointLightTrackingCheckInterval
	{
		get
		{
			return _pointLightTrackingCheckInterval;
		}
		set
		{
			if (value != _pointLightTrackingCheckInterval)
			{
				_pointLightTrackingCheckInterval = value;
				TrackPointLights();
				isDirty = true;
			}
		}
	}

	public int downsampling
	{
		get
		{
			return _downsampling;
		}
		set
		{
			if (value != _downsampling)
			{
				_preset = FOG_PRESET.Custom;
				_downsampling = value;
				isDirty = true;
			}
		}
	}

	public bool edgeImprove
	{
		get
		{
			return _edgeImprove;
		}
		set
		{
			if (value != _edgeImprove)
			{
				_preset = FOG_PRESET.Custom;
				_edgeImprove = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float edgeThreshold
	{
		get
		{
			return _edgeThreshold;
		}
		set
		{
			if (value != _edgeThreshold)
			{
				_preset = FOG_PRESET.Custom;
				_edgeThreshold = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float stepping
	{
		get
		{
			return _stepping;
		}
		set
		{
			if (value != _stepping)
			{
				_preset = FOG_PRESET.Custom;
				_stepping = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float steppingNear
	{
		get
		{
			return _steppingNear;
		}
		set
		{
			if (value != _steppingNear)
			{
				_preset = FOG_PRESET.Custom;
				_steppingNear = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public bool dithering
	{
		get
		{
			return _dithering;
		}
		set
		{
			if (value != _dithering)
			{
				_dithering = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float ditherStrength
	{
		get
		{
			return _ditherStrength;
		}
		set
		{
			if (value != _ditherStrength)
			{
				_ditherStrength = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public bool jitter
	{
		get
		{
			return _jitter;
		}
		set
		{
			if (value != _jitter)
			{
				_jitter = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float jitterrStrength
	{
		get
		{
			return _jitterrStrength;
		}
		set
		{
			if (value != _jitterrStrength)
			{
				_jitterrStrength = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public bool lightScatteringEnabled
	{
		get
		{
			return _lightScatteringEnabled;
		}
		set
		{
			if (value != _lightScatteringEnabled)
			{
				_lightScatteringEnabled = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float lightScatteringSpread
	{
		get
		{
			return _lightScatteringSpread;
		}
		set
		{
			if (value != _lightScatteringSpread)
			{
				_lightScatteringSpread = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public int lightScatteringSamples
	{
		get
		{
			return _lightScatteringSamples;
		}
		set
		{
			if (value != _lightScatteringSamples)
			{
				_lightScatteringSamples = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float lightScatteringWeight
	{
		get
		{
			return _lightScatteringWeight;
		}
		set
		{
			if (value != _lightScatteringWeight)
			{
				_lightScatteringWeight = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float lightScatteringIllumination
	{
		get
		{
			return _lightScatteringIllumination;
		}
		set
		{
			if (value != _lightScatteringIllumination)
			{
				_lightScatteringIllumination = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float lightScatteringDecay
	{
		get
		{
			return _lightScatteringDecay;
		}
		set
		{
			if (value != _lightScatteringDecay)
			{
				_lightScatteringDecay = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float lightScatteringExposure
	{
		get
		{
			return _lightScatteringExposure;
		}
		set
		{
			if (value != _lightScatteringExposure)
			{
				_lightScatteringExposure = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float lightScatteringJittering
	{
		get
		{
			return _lightScatteringJittering;
		}
		set
		{
			if (value != _lightScatteringJittering)
			{
				_lightScatteringJittering = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public bool fogBlur
	{
		get
		{
			return _fogBlur;
		}
		set
		{
			if (value != _fogBlur)
			{
				_fogBlur = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float fogBlurDepth
	{
		get
		{
			return _fogBlurDepth;
		}
		set
		{
			if (value != _fogBlurDepth)
			{
				_fogBlurDepth = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public Camera fogCamera => mainCamera;

	public bool fogOfWarEnabled
	{
		get
		{
			return _fogOfWarEnabled;
		}
		set
		{
			if (value != _fogOfWarEnabled)
			{
				_fogOfWarEnabled = value;
				FogOfWarUpdateTexture();
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public Vector3 fogOfWarCenter
	{
		get
		{
			return _fogOfWarCenter;
		}
		set
		{
			if (value != _fogOfWarCenter)
			{
				_fogOfWarCenter = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public Vector3 fogOfWarSize
	{
		get
		{
			return _fogOfWarSize;
		}
		set
		{
			if (value != _fogOfWarSize && value.x > 0f && value.z > 0f)
			{
				_fogOfWarSize = value;
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public int fogOfWarTextureSize
	{
		get
		{
			return _fogOfWarTextureSize;
		}
		set
		{
			if (value != _fogOfWarTextureSize && value > 16)
			{
				_fogOfWarTextureSize = value;
				FogOfWarUpdateTexture();
				UpdateMaterialProperties();
				isDirty = true;
			}
		}
	}

	public float fogOfWarRestoreDelay
	{
		get
		{
			return _fogOfWarRestoreDelay;
		}
		set
		{
			if (value != _fogOfWarRestoreDelay)
			{
				_fogOfWarRestoreDelay = value;
				isDirty = true;
			}
		}
	}

	public float fogOfWarRestoreDuration
	{
		get
		{
			return _fogOfWarRestoreDuration;
		}
		set
		{
			if (value != _fogOfWarRestoreDuration)
			{
				_fogOfWarRestoreDuration = value;
				isDirty = true;
			}
		}
	}

	public Color32[] fogOfWarTextureData
	{
		get
		{
			return fogOfWarColorBuffer;
		}
		set
		{
			fogOfWarEnabled = true;
			fogOfWarColorBuffer = value;
			if (value != null && !(fogOfWarTexture == null) && value.Length == fogOfWarTexture.width * fogOfWarTexture.height)
			{
				fogOfWarTexture.SetPixels32(fogOfWarColorBuffer);
				fogOfWarTexture.Apply();
			}
		}
	}

	private void OnEnable()
	{
		if (_fogVoidInverted)
		{
			_fogVoidInverted = false;
			_fogAreaCenter = _character;
			_fogAreaDepth = _fogVoidDepth;
			_fogAreaFallOff = _fogVoidFallOff;
			_fogAreaHeight = _fogVoidHeight;
			_fogAreaPosition = _fogVoidPosition;
			_fogAreaRadius = _fogVoidRadius;
			_fogVoidRadius = 0f;
			_character = null;
		}
		if (_pointLights == null || _pointLights.Length < 6)
		{
			_pointLights = new GameObject[6];
		}
		if (_pointLightColors == null || _pointLightColors.Length < 6)
		{
			_pointLightColors = new Color[6];
		}
		if (_pointLightIntensities == null || _pointLightIntensities.Length < 6)
		{
			_pointLightIntensities = new float[6];
		}
		if (_pointLightIntensitiesMultiplier == null || _pointLightIntensitiesMultiplier.Length < 6)
		{
			_pointLightIntensitiesMultiplier = new float[6];
		}
		if (_pointLightPositions == null || _pointLightPositions.Length < 6)
		{
			_pointLightPositions = new Vector3[6];
		}
		if (_pointLightRanges == null || _pointLightRanges.Length < 6)
		{
			_pointLightRanges = new float[6];
		}
		if (fogMat == null)
		{
			Init();
		}
	}

	private void Init()
	{
		targetFogAlpha = -1f;
		targetSkyHazeAlpha = -1f;
		currentFogAlpha = _alpha;
		_skyColor.a = _skyAlpha;
		currentSkyHazeAlpha = _skyAlpha;
		fogMat = new Material(Shader.Find("VolumetricFogAndMist/VolumetricFog"));
		fogMat.hideFlags = HideFlags.DontSave;
		mainCamera = gameObject.GetComponent<Camera>();
		if (mainCamera.depthTextureMode == DepthTextureMode.None)
		{
			mainCamera.depthTextureMode = DepthTextureMode.Depth;
		}
		Texture2D texture2D = Resources.Load<Texture2D>("Textures/Noise3");
		noiseColors = texture2D.GetPixels();
		adjustedColors = new Color[noiseColors.Length];
		adjustedTexture = new Texture2D(texture2D.width, texture2D.height, TextureFormat.RGBA32, mipmap: false);
		adjustedTexture.hideFlags = HideFlags.DontSave;
		UpdateTextureAlpha();
		UpdateSun();
		if (_pointLightTrackingAuto)
		{
			TrackPointLights();
		}
		else
		{
			UpdatePointLights();
		}
		FogOfWarInit();
		if (fogOfWarTexture == null)
		{
			FogOfWarUpdateTexture();
		}
		UpdatePreset();
		oldBaselineRelativeCameraY = mainCamera.transform.position.y;
	}

	private void OnDestroy()
	{
		if (depthCamObj != null)
		{
			UnityEngine.Object.DestroyImmediate(depthCamObj);
			depthCamObj = null;
		}
		if (adjustedTexture != null)
		{
			UnityEngine.Object.DestroyImmediate(adjustedTexture);
			adjustedTexture = null;
		}
		if (fogMat != null)
		{
			UnityEngine.Object.DestroyImmediate(fogMat);
			fogMat = null;
		}
		CleanUpTextures();
	}

	public void DestroySelf()
	{
		DestroyRenderComponent<VolumetricFogPreT>();
		DestroyRenderComponent<VolumetricFogPosT>();
		UnityEngine.Object.DestroyImmediate(this);
	}

	private void Start()
	{
		currentFogAlpha = _alpha;
		currentSkyHazeAlpha = _skyAlpha;
	}

	private void Update()
	{
		if (targetFogAlpha >= 0f || targetSkyHazeAlpha >= 0f)
		{
			if (targetFogAlpha != currentFogAlpha || targetSkyHazeAlpha != currentSkyHazeAlpha)
			{
				if (transitionDuration > 0f)
				{
					currentFogAlpha = Mathf.Lerp(initialFogAlpha, targetFogAlpha, (Time.time - transitionStartTime) / transitionDuration);
					currentSkyHazeAlpha = Mathf.Lerp(initialSkyHazeAlpha, targetSkyHazeAlpha, (Time.time - transitionStartTime) / transitionDuration);
				}
				else
				{
					currentFogAlpha = targetFogAlpha;
					currentSkyHazeAlpha = targetSkyHazeAlpha;
				}
				fogMat.SetFloat("_FogAlpha", currentFogAlpha);
				UpdateSkyColor(currentSkyHazeAlpha);
			}
		}
		else if (currentFogAlpha != _alpha || currentSkyHazeAlpha != _skyAlpha)
		{
			if (transitionDuration > 0f)
			{
				currentFogAlpha = Mathf.Lerp(initialFogAlpha, _alpha, (Time.time - transitionStartTime) / transitionDuration);
				currentSkyHazeAlpha = Mathf.Lerp(initialSkyHazeAlpha, alpha, (Time.time - transitionStartTime) / transitionDuration);
			}
			else
			{
				currentFogAlpha = _alpha;
				currentSkyHazeAlpha = _skyAlpha;
			}
			fogMat.SetFloat("_FogAlpha", currentFogAlpha);
			UpdateSkyColor(currentSkyHazeAlpha);
		}
		if (_baselineRelativeToCamera)
		{
			UpdateMaterialHeights();
		}
		else if (_character != null)
		{
			_fogVoidPosition = _character.transform.position;
			UpdateMaterialHeights();
		}
		if (_fogAreaCenter != null)
		{
			_fogAreaPosition = _fogAreaCenter.transform.position;
			UpdateMaterialHeights();
		}
		if (_pointLightTrackingAuto && Time.time - trackPointAutoLastTime > _pointLightTrackingCheckInterval)
		{
			trackPointAutoLastTime = Time.time;
			TrackPointLights();
		}
		if (_sun != null)
		{
			bool flag = false;
			if (_sun.transform.forward != _lightDirection && (!Application.isPlaying || Time.time - lastTextureUpdate >= 0.2f))
			{
				flag = true;
			}
			if (sunLight != null && sunLight.color != _lightColor)
			{
				flag = true;
			}
			if (flag)
			{
				UpdateTexture();
			}
		}
		if (_fogOfWarEnabled)
		{
			FogOfWarUpdate();
		}
	}

	private void FixedUpdate()
	{
		UpdateWindSpeedQuick();
	}

	private void UpdateRenderComponents()
	{
		if (_renderOpaque)
		{
			DestroyRenderComponent<VolumetricFogPosT>();
			AssignRenderComponent<VolumetricFogPreT>();
		}
		else
		{
			DestroyRenderComponent<VolumetricFogPreT>();
			AssignRenderComponent<VolumetricFogPosT>();
		}
	}

	private void DestroyRenderComponent<T>() where T : IVolumetricFogRenderComponent
	{
		T[] componentsInChildren = GetComponentsInChildren<T>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].fog == this || componentsInChildren[i].fog == null)
			{
				componentsInChildren[i].DestroySelf();
			}
		}
	}

	private void AssignRenderComponent<T>() where T : Component, IVolumetricFogRenderComponent
	{
		T[] componentsInChildren = GetComponentsInChildren<T>(includeInactive: true);
		int num = -1;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].fog == this)
			{
				return;
			}
			if (componentsInChildren[i].fog == null)
			{
				num = i;
			}
		}
		if (num < 0)
		{
			T val = gameObject.AddComponent<T>();
			val.fog = this;
		}
		else
		{
			componentsInChildren[num].fog = this;
		}
	}

	internal void DoOnPreRender()
	{
		if (!enabled || !gameObject.activeSelf || !_improveTransparency || fogMat == null)
		{
			return;
		}
		CleanUpTextures();
		if (depthCam == null)
		{
			if (depthCamObj == null)
			{
				depthCamObj = new GameObject("DepthCamera");
				depthCam = depthCamObj.AddComponent<Camera>();
				depthCam.enabled = false;
				depthCamObj.hideFlags = HideFlags.HideAndDontSave;
			}
			else
			{
				depthCam = depthCamObj.GetComponent<Camera>();
				if (depthCam == null)
				{
					UnityEngine.Object.DestroyImmediate(depthCamObj);
					depthCamObj = null;
					return;
				}
			}
		}
		depthCam.CopyFrom(mainCamera);
		depthCam.depthTextureMode = DepthTextureMode.None;
		depthTexture = RenderTexture.GetTemporary(mainCamera.pixelWidth, mainCamera.pixelHeight, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		depthCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
		depthCam.clearFlags = CameraClearFlags.Color;
		depthCam.targetTexture = depthTexture;
		if (depthShader == null)
		{
			depthShader = Shader.Find("VolumetricFogAndMist/CopyDepth");
		}
		depthCam.RenderWithShader(depthShader, "RenderType");
		fogMat.SetTexture("_DepthTexture", depthTexture);
	}

	internal void DoOnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (_density == 0f || !enabled)
		{
			Graphics.Blit(source, destination);
			return;
		}
		fogMat.SetMatrix("_ClipToWorld", mainCamera.cameraToWorldMatrix * mainCamera.projectionMatrix.inverse);
		if (_sun != null && _lightScatteringEnabled)
		{
			UpdateScatteringData();
		}
		for (int i = 0; i < 6; i++)
		{
			Light light = pointLightComponents[i];
			if (light != null)
			{
				if (_pointLightColors[i] != light.color)
				{
					ref Color reference = ref _pointLightColors[i];
					reference = light.color;
					isDirty = true;
				}
				if (_pointLightRanges[i] != light.range)
				{
					_pointLightRanges[i] = light.range;
					isDirty = true;
				}
				if (_pointLightPositions[i] != light.transform.position)
				{
					ref Vector3 reference2 = ref _pointLightPositions[i];
					reference2 = light.transform.position;
					isDirty = true;
				}
				if (_pointLightIntensities[i] != light.intensity)
				{
					_pointLightIntensities[i] = light.intensity;
					isDirty = true;
				}
			}
			if (_pointLightRanges[i] * _pointLightIntensities[i] > 0f)
			{
				string text = i.ToString();
				fogMat.SetVector("_FogPointLightPosition" + text, _pointLightPositions[i] + Vector3.down * _baselineHeight);
				Vector3 vector = new Vector3(_pointLightColors[i].r, _pointLightColors[i].g, _pointLightColors[i].b);
				vector *= _pointLightIntensities[i] * 0.1f * _pointLightIntensitiesMultiplier[i] * (_pointLightRanges[i] * _pointLightRanges[i]);
				fogMat.SetVector("_FogPointLightColor" + text, vector);
			}
		}
		if ((float)_downsampling > 1f)
		{
			int scaledSize = GetScaledSize(source.width, _downsampling);
			int scaledSize2 = GetScaledSize(source.width, _downsampling);
			reducedDestination = RenderTexture.GetTemporary(scaledSize, scaledSize2, 0, RenderTextureFormat.ARGB32);
			RenderTextureFormat format = ((!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RFloat)) ? RenderTextureFormat.ARGBFloat : RenderTextureFormat.RFloat);
			RenderTexture temporary = RenderTexture.GetTemporary(scaledSize, scaledSize2, 0, format);
			if (_fogBlur)
			{
				RenderTexture temporary2 = RenderTexture.GetTemporary(scaledSize, scaledSize2, 0, RenderTextureFormat.ARGB32);
				Graphics.Blit(source, temporary2);
				SetBlurTexture(temporary2);
				RenderTexture.ReleaseTemporary(temporary2);
			}
			if (VRSettings.enabled || SystemInfo.supportedRenderTargetCount < 2)
			{
				Graphics.Blit(source, reducedDestination, fogMat, 4);
				Graphics.Blit(source, temporary, fogMat, 5);
			}
			else
			{
				fogMat.SetTexture("_MainTex", source);
				if (mrt == null)
				{
					mrt = new RenderBuffer[2];
				}
				ref RenderBuffer reference3 = ref mrt[0];
				reference3 = reducedDestination.colorBuffer;
				ref RenderBuffer reference4 = ref mrt[1];
				reference4 = temporary.colorBuffer;
				Graphics.SetRenderTarget(mrt, reducedDestination.depthBuffer);
				Graphics.Blit(null, fogMat, 2);
			}
			fogMat.SetTexture("_FogDownsampled", reducedDestination);
			fogMat.SetTexture("_DownsampledDepth", temporary);
			Graphics.Blit(source, destination, fogMat, 3);
			RenderTexture.ReleaseTemporary(temporary);
			RenderTexture.ReleaseTemporary(reducedDestination);
		}
		else
		{
			if (_fogBlur)
			{
				RenderTexture temporary3 = RenderTexture.GetTemporary(512, 512, 0, RenderTextureFormat.ARGB32);
				Graphics.Blit(source, temporary3);
				SetBlurTexture(temporary3);
				RenderTexture.ReleaseTemporary(temporary3);
			}
			Graphics.Blit(source, destination, fogMat, 0);
		}
		rtDest = destination;
	}

	private void SetBlurTexture(RenderTexture source)
	{
		if (blurMat == null)
		{
			Shader shader = Shader.Find("VolumetricFogAndMist/Blur");
			blurMat = new Material(shader);
			blurMat.hideFlags = HideFlags.DontSave;
		}
		if (!(blurMat == null))
		{
			blurMat.SetFloat("_BlurDepth", _fogBlurDepth);
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
			Graphics.Blit(source, temporary, blurMat, 0);
			RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
			Graphics.Blit(temporary, temporary2, blurMat, 1);
			blurMat.SetFloat("_BlurDepth", _fogBlurDepth * 2f);
			temporary.DiscardContents();
			Graphics.Blit(temporary2, temporary, blurMat, 0);
			temporary2.DiscardContents();
			Graphics.Blit(temporary, temporary2, blurMat, 1);
			fogMat.SetTexture("_BlurTex", temporary2);
			RenderTexture.ReleaseTemporary(temporary2);
			RenderTexture.ReleaseTemporary(temporary);
		}
	}

	internal void DoOnPostRender()
	{
		if (_density != 0f && enabled)
		{
			Graphics.Blit(rtDest, fogMat, 1);
		}
	}

	public string GetCurrentPresetName()
	{
		return Enum.GetName(typeof(FOG_PRESET), _preset);
	}

	private void UpdatePreset()
	{
		switch (_preset)
		{
		case FOG_PRESET.Clear:
			_density = 0f;
			_fogOfWarEnabled = false;
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.Mist:
			_skySpeed = 0.3f;
			_skyHaze = 15f;
			_skyNoiseStrength = 0.1f;
			_skyAlpha = 0.8f;
			_density = 0.3f;
			_noiseStrength = 0.6f;
			_noiseScale = 1f;
			_distance = 0f;
			_distanceFallOff = 0f;
			_height = 6f;
			_stepping = 8f;
			_steppingNear = 0f;
			_alpha = 1f;
			_color = new Color(0.89f, 0.89f, 0.89f, 1f);
			_skyColor = _color;
			_specularColor = new Color(1f, 1f, 0.8f, 1f);
			_specularIntensity = 0.1f;
			_specularThreshold = 0.6f;
			_lightColor = Color.white;
			_lightIntensity = 0.12f;
			_speed = 0.01f;
			_fogOfWarEnabled = false;
			_downsampling = 1;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.WindyMist:
			_skySpeed = 0.3f;
			_skyHaze = 25f;
			_skyNoiseStrength = 0.1f;
			_skyAlpha = 0.85f;
			_density = 0.3f;
			_noiseStrength = 0.5f;
			_noiseScale = 1.15f;
			_distance = 0f;
			_distanceFallOff = 0f;
			_height = 6.5f;
			_stepping = 10f;
			_steppingNear = 0f;
			_alpha = 1f;
			_color = new Color(0.89f, 0.89f, 0.89f, 1f);
			_skyColor = _color;
			_specularColor = new Color(1f, 1f, 0.8f, 1f);
			_specularIntensity = 0.1f;
			_specularThreshold = 0.6f;
			_lightColor = Color.white;
			_lightIntensity = 0f;
			_speed = 0.15f;
			_fogOfWarEnabled = false;
			_downsampling = 1;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.GroundFog:
			_skySpeed = 0.3f;
			_skyHaze = 0f;
			_skyNoiseStrength = 0.1f;
			_skyAlpha = 0.85f;
			_density = 0.6f;
			_noiseStrength = 0.479f;
			_noiseScale = 1.15f;
			_distance = 5f;
			_distanceFallOff = 1f;
			_height = 1.5f;
			_stepping = 8f;
			_steppingNear = 0f;
			_alpha = 0.95f;
			_color = new Color(0.89f, 0.89f, 0.89f, 1f);
			_skyColor = _color;
			_specularColor = new Color(1f, 1f, 0.8f, 1f);
			_specularIntensity = 0.2f;
			_specularThreshold = 0.6f;
			_lightColor = Color.white;
			_lightIntensity = 0.2f;
			_speed = 0.01f;
			_fogOfWarEnabled = false;
			_downsampling = 1;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.FrostedGround:
			_skySpeed = 0f;
			_skyHaze = 0f;
			_skyNoiseStrength = 0.729f;
			_skyAlpha = 0.55f;
			_density = 1f;
			_noiseStrength = 0.164f;
			_noiseScale = 1.81f;
			_distance = 0f;
			_distanceFallOff = 0f;
			_height = 0.5f;
			_stepping = 20f;
			_steppingNear = 50f;
			_alpha = 0.97f;
			_color = new Color(0.546f, 0.648f, 0.71f, 1f);
			_skyColor = _color;
			_specularColor = new Color(0.792f, 0.792f, 0.792f, 1f);
			_specularIntensity = 1f;
			_specularThreshold = 0.866f;
			_lightColor = new Color(0.972f, 0.972f, 0.972f, 1f);
			_lightIntensity = 0.743f;
			_speed = 0f;
			_fogOfWarEnabled = false;
			_downsampling = 1;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.FoggyLake:
			_skySpeed = 0.3f;
			_skyHaze = 40f;
			_skyNoiseStrength = 0.574f;
			_skyAlpha = 0.827f;
			_density = 1f;
			_noiseStrength = 0.03f;
			_noiseScale = 5.77f;
			_distance = 0f;
			_distanceFallOff = 0f;
			_height = 4f;
			_stepping = 6f;
			_steppingNear = 14.4f;
			_alpha = 1f;
			_color = new Color(0f, 0.96f, 1f, 1f);
			_skyColor = _color;
			_specularColor = Color.white;
			_lightColor = Color.white;
			_specularIntensity = 0.861f;
			_specularThreshold = 0.907f;
			_lightIntensity = 0.126f;
			_speed = 0f;
			_fogOfWarEnabled = false;
			_downsampling = 1;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.LowClouds:
			_skySpeed = 0.3f;
			_skyHaze = 60f;
			_skyNoiseStrength = 1f;
			_skyAlpha = 0.96f;
			_density = 1f;
			_noiseStrength = 0.7f;
			_noiseScale = 1f;
			_distance = 0f;
			_distanceFallOff = 0f;
			_height = 4f;
			_stepping = 12f;
			_steppingNear = 0f;
			_alpha = 0.98f;
			_color = new Color(0.89f, 0.89f, 0.89f, 1f);
			_skyColor = _color;
			_specularColor = new Color(1f, 1f, 0.8f, 1f);
			_specularIntensity = 0.15f;
			_specularThreshold = 0.6f;
			_lightColor = Color.white;
			_lightIntensity = 0.15f;
			_speed = 0.008f;
			_fogOfWarEnabled = false;
			_downsampling = 1;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.SeaClouds:
			_skySpeed = 0.3f;
			_skyHaze = 60f;
			_skyNoiseStrength = 1f;
			_skyAlpha = 0.96f;
			_density = 1f;
			_noiseStrength = 1f;
			_noiseScale = 1.5f;
			_distance = 0f;
			_distanceFallOff = 0f;
			_height = 12.4f;
			_stepping = 6f;
			_alpha = 0.98f;
			_color = new Color(0.89f, 0.89f, 0.89f, 1f);
			_skyColor = _color;
			_specularColor = new Color(1f, 1f, 0.8f, 1f);
			_specularIntensity = 0.259f;
			_specularThreshold = 0.6f;
			_lightColor = Color.white;
			_lightIntensity = 0.15f;
			_speed = 0.008f;
			_fogOfWarEnabled = false;
			_downsampling = 1;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.Fog:
			_skySpeed = 0.3f;
			_skyHaze = 144f;
			_skyNoiseStrength = 0.7f;
			_skyAlpha = 0.9f;
			_density = 0.35f;
			_noiseStrength = 0.3f;
			_noiseScale = 1f;
			_distance = 20f;
			_distanceFallOff = 0.7f;
			_height = 8f;
			_stepping = 8f;
			_steppingNear = 0f;
			_alpha = 0.97f;
			_color = new Color(0.89f, 0.89f, 0.89f, 1f);
			_skyColor = _color;
			_specularColor = new Color(1f, 1f, 0.8f, 1f);
			_specularIntensity = 0f;
			_specularThreshold = 0.6f;
			_lightColor = Color.white;
			_lightIntensity = 0f;
			_speed = 0.05f;
			_fogOfWarEnabled = false;
			_downsampling = 1;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.HeavyFog:
			_skySpeed = 0.05f;
			_skyHaze = 500f;
			_skyNoiseStrength = 0.96f;
			_skyAlpha = 1f;
			_density = 0.35f;
			_noiseStrength = 0.1f;
			_noiseScale = 1f;
			_distance = 20f;
			_distanceFallOff = 0.8f;
			_height = 18f;
			_stepping = 6f;
			_steppingNear = 0f;
			_alpha = 1f;
			_color = new Color(0.91f, 0.91f, 0.91f, 1f);
			_skyColor = _color;
			_specularColor = new Color(1f, 1f, 0.8f, 1f);
			_specularIntensity = 0f;
			_specularThreshold = 0.6f;
			_lightColor = Color.white;
			_lightIntensity = 0f;
			_speed = 0.015f;
			_fogOfWarEnabled = false;
			_downsampling = 1;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.SandStorm1:
			_skySpeed = 0.35f;
			_skyHaze = 388f;
			_skyNoiseStrength = 0.847f;
			_skyAlpha = 1f;
			_density = 0.487f;
			_noiseStrength = 0.758f;
			_noiseScale = 1.71f;
			_distance = 0f;
			_distanceFallOff = 0f;
			_height = 16f;
			_stepping = 6f;
			_steppingNear = 0f;
			_alpha = 1f;
			_color = new Color(0.505f, 0.505f, 0.505f, 1f);
			_skyColor = _color;
			_specularColor = new Color(1f, 1f, 0.8f, 1f);
			_specularIntensity = 0f;
			_specularThreshold = 0.6f;
			_lightColor = Color.white;
			_lightIntensity = 0f;
			_speed = 0.3f;
			_windDirection = Vector3.right;
			_fogOfWarEnabled = false;
			_downsampling = 1;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.Smoke:
			_skySpeed = 0.109f;
			_skyHaze = 10f;
			_skyNoiseStrength = 0.119f;
			_skyAlpha = 1f;
			_density = 1f;
			_noiseStrength = 0.767f;
			_noiseScale = 1.6f;
			_distance = 0f;
			_distanceFallOff = 0f;
			_height = 8f;
			_stepping = 12f;
			_steppingNear = 25f;
			_alpha = 1f;
			_color = new Color(0.125f, 0.125f, 0.125f, 1f);
			_skyColor = _color;
			_specularColor = new Color(1f, 1f, 1f, 1f);
			_specularIntensity = 0.575f;
			_specularThreshold = 0.6f;
			_lightColor = Color.white;
			_lightIntensity = 1f;
			_speed = 0.075f;
			_windDirection = Vector3.right;
			_fogOfWarEnabled = false;
			_downsampling = 1;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			_baselineHeight += 8f;
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.ToxicSwamp:
			_skySpeed = 0.062f;
			_skyHaze = 22f;
			_skyNoiseStrength = 0.694f;
			_skyAlpha = 1f;
			_density = 1f;
			_noiseStrength = 1f;
			_noiseScale = 1f;
			_distance = 0f;
			_distanceFallOff = 0f;
			_height = 2.5f;
			_stepping = 20f;
			_steppingNear = 50f;
			_alpha = 0.95f;
			_color = new Color(0.0238f, 0.175f, 0.109f, 1f);
			_skyColor = _color;
			_specularColor = new Color(0.593f, 0.625f, 0.207f, 1f);
			_specularIntensity = 0.735f;
			_specularThreshold = 0.6f;
			_lightColor = new Color(0.73f, 0.746f, 0.511f, 1f);
			_lightIntensity = 0.492f;
			_speed = 0.0003f;
			_windDirection = Vector3.right;
			_fogOfWarEnabled = false;
			_downsampling = 1;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.SandStorm2:
			_skySpeed = 0f;
			_skyHaze = 0f;
			_skyNoiseStrength = 0.729f;
			_skyAlpha = 0.55f;
			_density = 0.545f;
			_noiseStrength = 1f;
			_noiseScale = 3f;
			_distance = 0f;
			_distanceFallOff = 0f;
			_height = 12f;
			_stepping = 5f;
			_steppingNear = 19.6f;
			_alpha = 0.96f;
			_color = new Color(0.609f, 0.609f, 0.609f, 1f);
			_skyColor = _color;
			_specularColor = new Color(0.589f, 0.621f, 0.207f, 1f);
			_specularIntensity = 0.505f;
			_specularThreshold = 0.6f;
			_lightColor = new Color(0.726f, 0.742f, 0.507f, 1f);
			_lightIntensity = 0.581f;
			_speed = 0.168f;
			_windDirection = Vector3.right;
			_fogOfWarEnabled = false;
			_downsampling = 1;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			_fogVoidRadius = 0f;
			break;
		case FOG_PRESET.WorldEdge:
		{
			_skySpeed = 0.3f;
			_skyHaze = 60f;
			_skyNoiseStrength = 1f;
			_skyAlpha = 0.96f;
			_density = 1f;
			_noiseStrength = 1f;
			_noiseScale = 3f;
			_distance = 0f;
			_distanceFallOff = 0f;
			_height = 20f;
			_stepping = 6f;
			_alpha = 0.98f;
			_color = new Color(0.89f, 0.89f, 0.89f, 1f);
			_skyColor = _color;
			_specularColor = new Color(1f, 1f, 0.8f, 1f);
			_specularIntensity = 0.259f;
			_specularThreshold = 0.6f;
			_lightColor = Color.white;
			_lightIntensity = 0.15f;
			_speed = 0.03f;
			_fogOfWarEnabled = false;
			_downsampling = 2;
			_baselineRelativeToCamera = false;
			CheckWaterLevel(baseZero: false);
			Terrain activeTerrain = GetActiveTerrain();
			if (activeTerrain != null)
			{
				_fogVoidPosition = activeTerrain.transform.position + activeTerrain.terrainData.size * 0.5f;
				_fogVoidRadius = activeTerrain.terrainData.size.x * 0.45f;
				_fogVoidHeight = activeTerrain.terrainData.size.y;
				_fogVoidDepth = activeTerrain.terrainData.size.z * 0.45f;
				_fogVoidFallOff = 6f;
				_fogAreaRadius = 0f;
				_character = null;
				_fogAreaCenter = null;
				float x = activeTerrain.terrainData.size.x;
				if (mainCamera.farClipPlane < x)
				{
					mainCamera.farClipPlane = x;
				}
				if (_maxFogLength < x * 0.6f)
				{
					_maxFogLength = x * 0.6f;
				}
			}
			break;
		}
		}
		FogOfWarUpdateTexture();
		UpdateMaterialProperties();
		UpdateRenderComponents();
		UpdateTextureAlpha();
		UpdateTexture();
	}

	public void CheckWaterLevel(bool baseZero)
	{
		if (_baselineHeight > mainCamera.transform.position.y || baseZero)
		{
			_baselineHeight = 0f;
		}
		GameObject gameObject = GameObject.Find("Water");
		if (gameObject == null)
		{
			GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].layer == 4)
				{
					gameObject = array[i];
					break;
				}
			}
		}
		if (gameObject != null)
		{
			_renderOpaque = false;
			if (_baselineHeight < gameObject.transform.position.y)
			{
				_baselineHeight = gameObject.transform.position.y;
			}
		}
		UpdateMaterialHeights();
	}

	public static Terrain GetActiveTerrain()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain != null && activeTerrain.isActiveAndEnabled)
		{
			return activeTerrain;
		}
		for (int i = 0; i < Terrain.activeTerrains.Length; i++)
		{
			activeTerrain = Terrain.activeTerrains[i];
			if (activeTerrain != null && activeTerrain.isActiveAndEnabled)
			{
				return activeTerrain;
			}
		}
		return null;
	}

	private void UpdateMaterialHeights()
	{
		float num = _baselineHeight;
		Vector3 vector = _fogAreaPosition;
		if (_fogAreaRadius > 0f)
		{
			if (_fogAreaCenter != null)
			{
				num += _fogAreaCenter.transform.position.y;
			}
			vector.y = 0f;
		}
		if (_baselineRelativeToCamera)
		{
			oldBaselineRelativeCameraY += (mainCamera.transform.position.y - oldBaselineRelativeCameraY) * Mathf.Clamp01(1.001f - _baselineRelativeToCameraDelay);
			num += oldBaselineRelativeCameraY - 1f;
		}
		float w = 0.01f / _noiseScale;
		fogMat.SetVector("_FogData", new Vector4(num, _height, 1f / _density, w));
		fogMat.SetFloat("_FogSkyHaze", _skyHaze + num);
		Vector3 vector2 = _fogVoidPosition - num * Vector3.up;
		fogMat.SetVector("_FogVoidPosition", vector2);
		fogMat.SetVector("_FogAreaPosition", vector);
	}

	private void UpdateMaterialProperties()
	{
		if (fogMat == null)
		{
			return;
		}
		UpdateSkyColor(_skyAlpha);
		Vector4 value = new Vector4(1f / (_stepping + 1f), 1f / (1f + _steppingNear), _edgeThreshold, (!_dithering) ? 0f : (_ditherStrength * 0.1f));
		if (!_edgeImprove)
		{
			value.z = 0f;
		}
		fogMat.SetVector("_FogStepping", value);
		fogMat.SetFloat("_FogAlpha", currentFogAlpha);
		UpdateMaterialHeights();
		float num = 0.01f / _noiseScale;
		float w = _maxFogLength * _maxFogLengthFallOff + 1f;
		fogMat.SetVector("_FogDistance", new Vector4(num * num * _distance * _distance, _distanceFallOff * _distanceFallOff + 0.1f, _maxFogLength, w));
		fogMat.SetColor("_Color", _color * 2f);
		if (shaderKeywords == null)
		{
			shaderKeywords = new List<string>();
		}
		else
		{
			shaderKeywords.Clear();
		}
		if (_distance > 0f)
		{
			shaderKeywords.Add("FOG_DISTANCE_ON");
		}
		if (_fogVoidRadius > 0f && _fogVoidFallOff > 0f)
		{
			Vector4 value2 = new Vector4(1f / (1f + _fogVoidRadius), 1f / (1f + _fogVoidHeight), 1f / (1f + _fogVoidDepth), _fogVoidFallOff);
			if (_fogVoidHeight > 0f && _fogVoidDepth > 0f)
			{
				shaderKeywords.Add("FOG_VOID_BOX");
			}
			else
			{
				shaderKeywords.Add("FOG_VOID_SPHERE");
			}
			fogMat.SetVector("_FogVoidData", value2);
		}
		if (_fogAreaRadius > 0f && _fogAreaFallOff > 0f)
		{
			Vector4 value3 = new Vector4(1f / (1f + _fogAreaRadius), 1f / (1f + _fogAreaHeight), 1f / (1f + _fogAreaDepth), _fogAreaFallOff);
			if (_fogAreaHeight > 0f && _fogAreaDepth > 0f)
			{
				shaderKeywords.Add("FOG_AREA_BOX");
			}
			else
			{
				shaderKeywords.Add("FOG_AREA_SPHERE");
				value3.y = _fogAreaRadius * _fogAreaRadius;
				value3.x /= num;
				value3.z /= num;
			}
			fogMat.SetVector("_FogAreaData", value3);
		}
		if (_skyHaze > 0f && _skyAlpha > 0f)
		{
			shaderKeywords.Add("FOG_HAZE_ON");
		}
		if (_fogOfWarEnabled)
		{
			shaderKeywords.Add("FOG_OF_WAR_ON");
			fogMat.SetTexture("_FogOfWar", fogOfWarTexture);
			fogMat.SetVector("_FogOfWarCenter", _fogOfWarCenter);
			fogMat.SetVector("_FogOfWarSize", _fogOfWarSize);
			Vector3 vector = _fogOfWarCenter - 0.5f * _fogOfWarSize;
			fogMat.SetVector("_FogOfWarCenterAdjusted", new Vector3(vector.x / _fogOfWarSize.x, 1f, vector.z / _fogOfWarSize.z));
		}
		int num2 = -1;
		for (int i = 0; i < 6; i++)
		{
			if (_pointLights[i] != null || _pointLightRanges[i] * _pointLightIntensities[i] > 0f)
			{
				num2 = i;
			}
		}
		if (num2 >= 0)
		{
			shaderKeywords.Add("FOG_POINT_LIGHT" + num2);
		}
		if (_lightScatteringEnabled)
		{
			UpdateScatteringData();
			shaderKeywords.Add("FOG_SCATTERING_ON");
		}
		if (_fogBlur)
		{
			shaderKeywords.Add("FOG_BLUR_ON");
			fogMat.SetFloat("_FogBlurDepth", _fogBlurDepth);
		}
		fogMat.shaderKeywords = shaderKeywords.ToArray();
	}

	private void UpdateWindSpeedQuick()
	{
		float num = 0.01f / _noiseScale;
		windSpeedAcum += Time.deltaTime * _windDirection.normalized * _speed / num;
		Vector4 value = windSpeedAcum;
		value.w = ((!_jitter) ? 0f : _jitterrStrength);
		fogMat.SetVector("_FogWindDir", value);
		skyHazeSpeedAcum += Time.deltaTime * _skySpeed / 20f;
		fogMat.SetVector("_FogSkyData", new Vector4(_skyHaze, _skyNoiseStrength, skyHazeSpeedAcum, _skyDepth));
	}

	private void UpdateTextureAlpha()
	{
		if (adjustedColors != null)
		{
			float num = Mathf.Clamp(_noiseStrength, 0f, 0.95f);
			for (int i = 0; i < adjustedColors.Length; i++)
			{
				adjustedColors[i].a = (1f - noiseColors[i].b * num) * _density;
			}
		}
	}

	private void UpdateScatteringData()
	{
		if (_sun == null)
		{
			return;
		}
		Vector3 vector = mainCamera.WorldToViewportPoint(_sun.transform.forward * 10000f);
		if (vector.z < 0f)
		{
			Vector2 vector2 = new Vector2(vector.x, vector.y);
			if (vector2 != oldSunPos)
			{
				oldSunPos = vector2;
				fogMat.SetVector("_SunPosition", vector2);
				float num = Mathf.Clamp01(1f - _lightDirection.y);
				sunFade = Mathf.SmoothStep(1f, 0f, (vector2 - Vector2.one * 0.5f).magnitude * 0.5f) * num;
			}
			if (_lightScatteringEnabled && !fogMat.IsKeywordEnabled("FOG_SCATTERING_ON"))
			{
				fogMat.EnableKeyword("FOG_SCATTERING_ON");
			}
			fogMat.SetVector("_FogScatteringData", new Vector4(_lightScatteringSpread / (float)_lightScatteringSamples, _lightScatteringSamples, _lightScatteringExposure * sunFade, _lightScatteringWeight / (float)_lightScatteringSamples));
			fogMat.SetVector("_FogScatteringData2", new Vector3(_lightScatteringIllumination, _lightScatteringDecay, _lightScatteringJittering));
		}
		else if (fogMat.IsKeywordEnabled("FOG_SCATTERING_ON"))
		{
			fogMat.DisableKeyword("FOG_SCATTERING_ON");
		}
	}

	private void UpdateTexture()
	{
		if (fogMat == null)
		{
			return;
		}
		UpdateSkyColor(_skyAlpha);
		float num = (_lightIntensity + sunLightIntensity) * Mathf.Clamp01(1f - _lightDirection.y * 2f);
		Color a = RenderSettings.ambientLight * RenderSettings.ambientIntensity;
		Color color = Color.Lerp(a, _lightColor * num, num);
		Vector3 vector = new Vector3(0f - _lightDirection.x, 0f, 0f - _lightDirection.z).normalized * 0.3f;
		vector.y = ((!(_lightDirection.y > 0f)) ? (1f - Mathf.Clamp01(0f - _lightDirection.y)) : Mathf.Clamp01(1f - _lightDirection.y));
		int width = adjustedTexture.width;
		int num2 = Mathf.FloorToInt(vector.z * (float)width) * width;
		int num3 = (int)((float)num2 + vector.x * (float)width) + adjustedColors.Length;
		float num4 = 1.0001f - _specularThreshold;
		float num5 = vector.y / num4;
		Color color2 = _specularColor * (1f + _specularIntensity) * _specularIntensity;
		bool flag = false;
		for (int i = 0; i < adjustedColors.Length; i++)
		{
			int num6 = (i + num3) % adjustedColors.Length;
			float a2 = adjustedColors[i].a;
			float num7 = (a2 - adjustedColors[num6].a) * num5;
			if (num7 < 0f)
			{
				num7 = 0f;
			}
			else if (num7 > 1f)
			{
				num7 = 1f;
			}
			Color color3 = (color + color2 * num7) * 0.5f;
			color3.a = a2;
			if (i == 0)
			{
				if (!(adjustedColors[i] != color3))
				{
					break;
				}
				flag = true;
			}
			adjustedColors[i] = color3;
		}
		if (flag)
		{
			adjustedTexture.SetPixels(adjustedColors);
			adjustedTexture.Apply();
		}
		fogMat.SetTexture("_NoiseTex", adjustedTexture);
		lastTextureUpdate = Time.time;
	}

	private void CleanUpTextures()
	{
		if ((bool)depthTexture)
		{
			RenderTexture.ReleaseTemporary(depthTexture);
			depthTexture = null;
		}
	}

	private void UpdateSun()
	{
		if (_sun != null)
		{
			sunLight = _sun.GetComponent<Light>();
		}
		else
		{
			sunLight = null;
		}
	}

	private void UpdateSkyColor(float alpha)
	{
		if (fogMat == null)
		{
			return;
		}
		if (_sun != null)
		{
			if (_sun.transform.forward != _lightDirection)
			{
				_lightDirection = _sun.transform.forward;
			}
			if (sunLight != null)
			{
				if (sunLight.color != _lightColor && _sunCopyColor)
				{
					_lightColor = sunLight.color;
				}
				sunLightIntensity = sunLight.intensity;
			}
		}
		else
		{
			sunLightIntensity = 1f;
		}
		float num = (_lightIntensity + sunLightIntensity) * Mathf.Clamp01(1f - _lightDirection.y);
		_skyColor.a = alpha;
		Color value = num * _skyColor;
		fogMat.SetColor("_FogSkyColor", value);
	}

	private void UpdatePointLights()
	{
		for (int i = 0; i < _pointLights.Length; i++)
		{
			GameObject gameObject = _pointLights[i];
			if (gameObject != null)
			{
				pointLightComponents[i] = gameObject.GetComponent<Light>();
			}
			else
			{
				pointLightComponents[i] = null;
			}
		}
	}

	private int GetScaledSize(int size, float factor)
	{
		size = (int)((float)size / factor);
		size /= 4;
		if (size < 1)
		{
			size = 1;
		}
		return size * 4;
	}

	public void SetTargetAlpha(float newFogAlpha, float newSkyHazeAlpha, float duration)
	{
		if (_useFogVolumes)
		{
			initialFogAlpha = currentFogAlpha;
			initialSkyHazeAlpha = currentSkyHazeAlpha;
			targetFogAlpha = newFogAlpha;
			targetSkyHazeAlpha = newSkyHazeAlpha;
			transitionDuration = duration;
			transitionStartTime = Time.time;
		}
	}

	public void ClearTargetAlpha(float duration)
	{
		SetTargetAlpha(-1f, -1f, duration);
	}

	public GameObject GetPointLight(int index)
	{
		if (index < 0 || index > _pointLights.Length)
		{
			return null;
		}
		return _pointLights[index];
	}

	public void SetPointLight(int index, GameObject pointLight)
	{
		if (index >= 0 && index <= _pointLights.Length && _pointLights[index] != pointLight)
		{
			_pointLights[index] = pointLight;
			UpdatePointLights();
			UpdateMaterialProperties();
			isDirty = true;
		}
	}

	public float GetPointLightRange(int index)
	{
		if (index < 0 || index > _pointLightRanges.Length)
		{
			return 0f;
		}
		return _pointLightRanges[index];
	}

	public void SetPointLightRange(int index, float range)
	{
		if (index >= 0 && index <= _pointLightRanges.Length && range != _pointLightRanges[index])
		{
			_pointLightRanges[index] = range;
			UpdateMaterialProperties();
			isDirty = true;
		}
	}

	public float GetPointLightIntensity(int index)
	{
		if (index < 0 || index > _pointLightIntensities.Length)
		{
			return 0f;
		}
		return _pointLightIntensities[index];
	}

	public void SetPointLightIntensity(int index, float intensity)
	{
		if (index >= 0 && index <= _pointLightIntensities.Length && intensity != _pointLightIntensities[index])
		{
			_pointLightIntensities[index] = intensity;
			UpdateMaterialProperties();
			isDirty = true;
		}
	}

	public float GetPointLightIntensityMultiplier(int index)
	{
		if (index < 0 || index > _pointLightIntensitiesMultiplier.Length)
		{
			return 0f;
		}
		return _pointLightIntensitiesMultiplier[index];
	}

	public void SetPointLightIntensityMultiplier(int index, float intensityMultiplier)
	{
		if (index >= 0 && index <= _pointLightIntensitiesMultiplier.Length && intensityMultiplier != _pointLightIntensitiesMultiplier[index])
		{
			_pointLightIntensitiesMultiplier[index] = intensityMultiplier;
			UpdateMaterialProperties();
			isDirty = true;
		}
	}

	public Vector3 GetPointLightPosition(int index)
	{
		if (index < 0 || index > _pointLightPositions.Length)
		{
			return Vector3.zero;
		}
		return _pointLightPositions[index];
	}

	public void SetPointLightPosition(int index, Vector3 position)
	{
		if (index >= 0 && index <= _pointLightPositions.Length && position != _pointLightPositions[index])
		{
			_pointLightPositions[index] = position;
			UpdateMaterialProperties();
			isDirty = true;
		}
	}

	public Color GetPointLightColor(int index)
	{
		if (index < 0 || index > _pointLightColors.Length)
		{
			return Color.white;
		}
		return _pointLightColors[index];
	}

	public void SetPointLightColor(int index, Color color)
	{
		if (index >= 0 && index <= _pointLightColors.Length && color != _pointLightColors[index])
		{
			_pointLightColors[index] = color;
			UpdateMaterialProperties();
			isDirty = true;
		}
	}

	private void TrackNewLights()
	{
		lastFoundLights = UnityEngine.Object.FindObjectsOfType<Light>();
	}

	private void TrackPointLights()
	{
		if (!_pointLightTrackingAuto)
		{
			return;
		}
		if (lastFoundLights == null || !Application.isPlaying || Time.time - trackPointCheckNewLightsLastTime > 3f)
		{
			trackPointCheckNewLightsLastTime = Time.time;
			TrackNewLights();
		}
		int num = lastFoundLights.Length;
		if (lightBuffer == null || lightBuffer.Length != num)
		{
			lightBuffer = new Light[num];
		}
		for (int i = 0; i < num; i++)
		{
			lightBuffer[i] = lastFoundLights[i];
		}
		bool flag = false;
		for (int j = 0; j < 6; j++)
		{
			GameObject gameObject = null;
			if (j < _pointLightTrackingCount)
			{
				gameObject = GetNearestLight(lightBuffer);
			}
			_pointLights[j] = gameObject;
			_pointLightRanges[j] = 0f;
			if (currentLights[j] != gameObject)
			{
				currentLights[j] = gameObject;
				flag = true;
			}
		}
		if (flag)
		{
			UpdatePointLights();
			UpdateMaterialProperties();
		}
	}

	private GameObject GetNearestLight(Light[] lights)
	{
		float num = float.MaxValue;
		Vector3 position = mainCamera.transform.position;
		GameObject result = null;
		int num2 = -1;
		for (int i = 0; i < lights.Length; i++)
		{
			if (lights[i] == null || !lights[i].enabled || lights[i].type != LightType.Point)
			{
				continue;
			}
			GameObject gameObject = lights[i].gameObject;
			if (gameObject.activeSelf)
			{
				float sqrMagnitude = (gameObject.transform.position - position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = gameObject;
					num = sqrMagnitude;
					num2 = i;
				}
			}
		}
		if (num2 >= 0)
		{
			lights[num2] = null;
		}
		return result;
	}

	public static VolumetricFog CreateFogArea(Vector3 position, float radius)
	{
		return CreateFogArea(Camera.main.gameObject, position, radius);
	}

	public static VolumetricFog CreateFogArea(GameObject cameraGameObject, Vector3 position, float radius)
	{
		if (cameraGameObject == null)
		{
			return null;
		}
		VolumetricFog volumetricFog = cameraGameObject.AddComponent<VolumetricFog>();
		volumetricFog.preset = FOG_PRESET.SeaClouds;
		volumetricFog.fogAreaPosition = position;
		volumetricFog.fogAreaRadius = radius;
		volumetricFog.fogAreaHeight = 0f;
		volumetricFog.fogAreaDepth = 0f;
		volumetricFog.skyHaze = 0f;
		CreateFogAreaPlaceholder(spherical: true, position, radius, volumetricFog.height, radius, volumetricFog);
		return volumetricFog;
	}

	public static VolumetricFog CreateFogArea(Vector3 position, Vector3 boxSize)
	{
		return CreateFogArea(Camera.main.gameObject, position, boxSize);
	}

	public static VolumetricFog CreateFogArea(GameObject cameraGameObject, Vector3 position, Vector3 boxSize)
	{
		if (cameraGameObject == null)
		{
			return null;
		}
		VolumetricFog volumetricFog = cameraGameObject.AddComponent<VolumetricFog>();
		volumetricFog.preset = FOG_PRESET.SeaClouds;
		volumetricFog.fogAreaPosition = position;
		volumetricFog.fogAreaRadius = boxSize.x;
		volumetricFog.fogAreaHeight = boxSize.y;
		volumetricFog.fogAreaDepth = boxSize.z;
		volumetricFog.height = boxSize.y * 0.98f;
		volumetricFog.jitter = true;
		volumetricFog.skyHaze = 0f;
		CreateFogAreaPlaceholder(spherical: false, position, boxSize.x, boxSize.y, boxSize.z, volumetricFog);
		return volumetricFog;
	}

	private static void CreateFogAreaPlaceholder(bool spherical, Vector3 position, float radius, float height, float depth, VolumetricFog fog)
	{
		GameObject original = ((!spherical) ? Resources.Load<GameObject>("Prefabs/FogBoxArea") : Resources.Load<GameObject>("Prefabs/FogSphereArea"));
		GameObject gameObject = UnityEngine.Object.Instantiate(original);
		FogAreaCullingManager component = gameObject.GetComponent<FogAreaCullingManager>();
		if (component == null)
		{
			Debug.Log("FogAreaCullingManager missing script.");
			return;
		}
		component.fog = fog;
		component.transform.position = position;
		component.transform.localScale = new Vector3(radius, height, depth);
		component.UpdateFogAreaExtents();
	}

	public static void RemoveAllFogAreas()
	{
		RemoveAllFogAreas(Camera.main.gameObject);
	}

	public static void RemoveAllFogAreas(GameObject cameraGameObject)
	{
		if (cameraGameObject == null)
		{
			return;
		}
		VolumetricFogPreT[] componentsInChildren = cameraGameObject.GetComponentsInChildren<VolumetricFogPreT>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].fog.fogAreaRadius > 0f)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren[i]);
			}
		}
		VolumetricFogPosT[] componentsInChildren2 = cameraGameObject.GetComponentsInChildren<VolumetricFogPosT>(includeInactive: true);
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			if (componentsInChildren2[j].fog.fogAreaRadius > 0f)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren2[j]);
			}
		}
		VolumetricFog[] componentsInChildren3 = cameraGameObject.GetComponentsInChildren<VolumetricFog>(includeInactive: true);
		for (int k = 0; k < componentsInChildren3.Length; k++)
		{
			if (componentsInChildren3[k].fogAreaRadius > 0f)
			{
				UnityEngine.Object.DestroyImmediate(componentsInChildren3[k]);
			}
		}
	}

	private void FogOfWarInit()
	{
		fowClearanceList = new List<FogOfWarClearance>();
	}

	private void FogOfWarUpdateTexture()
	{
		if (_fogOfWarEnabled)
		{
			int scaledSize = GetScaledSize(_fogOfWarTextureSize, 1f);
			fogOfWarTexture = new Texture2D(scaledSize, scaledSize, TextureFormat.Alpha8, mipmap: false);
			fogOfWarTexture.hideFlags = HideFlags.DontSave;
			fogOfWarTexture.filterMode = FilterMode.Bilinear;
			fogOfWarTexture.wrapMode = TextureWrapMode.Clamp;
			ResetFogOfWar();
		}
	}

	private void FogOfWarUpdate()
	{
		if (!_fogOfWarEnabled || _fogOfWarRestoreDelay <= 0f)
		{
			return;
		}
		int count = fowClearanceList.Count;
		int width = fogOfWarTexture.width;
		bool flag = false;
		for (int i = 0; i < count; i++)
		{
			FogOfWarClearance fogOfWarClearance = fowClearanceList[i];
			if (!fogOfWarClearance.enabled)
			{
				continue;
			}
			float num = Time.time - fogOfWarClearance.clearTime - _fogOfWarRestoreDelay;
			if (num > 0f)
			{
				float value = ((!(_fogOfWarRestoreDuration <= 0f)) ? (num / _fogOfWarRestoreDuration) : 1f);
				value = Mathf.Clamp01(value);
				float num2 = Mathf.Lerp((int)fogOfWarClearance.alphaSet, 255f, value);
				int num3 = fogOfWarClearance.y * width + fogOfWarClearance.x;
				fogOfWarColorBuffer[num3].a = (byte)num2;
				fogOfWarTexture.SetPixel(fogOfWarClearance.x, fogOfWarClearance.y, fogOfWarColorBuffer[num3]);
				flag = true;
				if (value >= 1f)
				{
					fogOfWarClearance.enabled = false;
				}
			}
		}
		if (flag)
		{
			fogOfWarTexture.Apply();
		}
	}

	public void SetFogOfWarAlpha(Vector3 worldPosition, float radius, float fogNewAlpha)
	{
		if (fogOfWarTexture == null)
		{
			return;
		}
		float num = (worldPosition.x - _fogOfWarCenter.x) / _fogOfWarSize.x + 0.5f;
		if (num < 0f || num > 1f)
		{
			return;
		}
		float num2 = (worldPosition.z - _fogOfWarCenter.z) / _fogOfWarSize.z + 0.5f;
		if (num2 < 0f || num2 > 1f)
		{
			return;
		}
		int width = fogOfWarTexture.width;
		int num3 = fogOfWarTexture.height;
		int num4 = (int)(num * (float)width);
		int num5 = (int)(num2 * (float)num3);
		int num6 = num5 * width + num4;
		byte b = (byte)(fogNewAlpha * 255f);
		Color32 color = fogOfWarColorBuffer[num6];
		if (b == color.a)
		{
			return;
		}
		float num7 = radius / _fogOfWarSize.z;
		int num8 = Mathf.FloorToInt((float)num3 * num7);
		for (int i = num5 - num8; i <= num5 + num8; i++)
		{
			if (i <= 0 || i >= num3 - 1)
			{
				continue;
			}
			for (int j = num4 - num8; j <= num4 + num8; j++)
			{
				if (j > 0 && j < width - 1)
				{
					int num9 = Mathf.FloorToInt(Mathf.Sqrt((num5 - i) * (num5 - i) + (num4 - j) * (num4 - j)));
					if (num9 <= num8)
					{
						num6 = i * width + j;
						Color32 color2 = fogOfWarColorBuffer[num6];
						color2.a = (byte)Mathf.Lerp((int)b, (int)color2.a, (float)num9 / (float)num8);
						fogOfWarColorBuffer[num6] = color2;
						fogOfWarTexture.SetPixel(j, i, color2);
						AddFowOfWarClearanceSlot(j, i, color2.a);
					}
				}
			}
		}
		fogOfWarTexture.Apply();
	}

	public void ResetFogOfWarAlpha(Vector3 worldPosition, float radius)
	{
		if (fogOfWarTexture == null)
		{
			return;
		}
		float num = (worldPosition.x - _fogOfWarCenter.x) / _fogOfWarSize.x + 0.5f;
		if (num < 0f || num > 1f)
		{
			return;
		}
		float num2 = (worldPosition.z - _fogOfWarCenter.z) / _fogOfWarSize.z + 0.5f;
		if (num2 < 0f || num2 > 1f)
		{
			return;
		}
		int width = fogOfWarTexture.width;
		int num3 = fogOfWarTexture.height;
		int num4 = (int)(num * (float)width);
		int num5 = (int)(num2 * (float)num3);
		int num6 = num5 * width + num4;
		float num7 = radius / _fogOfWarSize.z;
		int num8 = Mathf.FloorToInt((float)num3 * num7);
		for (int i = num5 - num8; i <= num5 + num8; i++)
		{
			if (i > 0 && i < num3 - 1)
			{
				for (int j = num4 - num8; j <= num4 + num8; j++)
				{
					if (j > 0 && j < width - 1)
					{
						int num9 = Mathf.FloorToInt(Mathf.Sqrt((num5 - i) * (num5 - i) + (num4 - j) * (num4 - j)));
						if (num9 <= num8)
						{
							num6 = i * width + j;
							Color32 color = fogOfWarColorBuffer[num6];
							color.a = byte.MaxValue;
							fogOfWarColorBuffer[num6] = color;
							fogOfWarTexture.SetPixel(j, i, color);
						}
					}
				}
			}
			fogOfWarTexture.Apply();
		}
	}

	public void ResetFogOfWar()
	{
		if (!(fogOfWarTexture == null))
		{
			int num = fogOfWarTexture.height;
			int width = fogOfWarTexture.width;
			int num2 = num * width;
			if (fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length != num2)
			{
				fogOfWarColorBuffer = new Color32[num2];
			}
			Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			for (int i = 0; i < num2; i++)
			{
				fogOfWarColorBuffer[i] = color;
			}
			fogOfWarTexture.SetPixels32(fogOfWarColorBuffer);
			fogOfWarTexture.Apply();
			isDirty = true;
		}
	}

	private void AddFowOfWarClearanceSlot(int x, int y, byte alphaSet)
	{
		int count = fowClearanceList.Count;
		FogOfWarClearance fogOfWarClearance = null;
		for (int i = 0; i < count; i++)
		{
			FogOfWarClearance fogOfWarClearance2 = fowClearanceList[i];
			if (fogOfWarClearance2.x == x && fogOfWarClearance2.y == y)
			{
				fogOfWarClearance = fogOfWarClearance2;
				break;
			}
			if (!fogOfWarClearance2.enabled)
			{
				fogOfWarClearance = fogOfWarClearance2;
			}
		}
		if (fogOfWarClearance == null)
		{
			fogOfWarClearance = new FogOfWarClearance();
			fowClearanceList.Add(fogOfWarClearance);
		}
		fogOfWarClearance.x = x;
		fogOfWarClearance.y = y;
		fogOfWarClearance.clearTime = Time.time;
		fogOfWarClearance.alphaSet = alphaSet;
		fogOfWarClearance.enabled = true;
	}
}
