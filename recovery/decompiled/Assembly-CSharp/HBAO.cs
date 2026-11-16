using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/HBAO")]
[RequireComponent(typeof(Camera))]
public class HBAO : MonoBehaviour
{
	public enum Preset
	{
		FastestPerformance,
		FastPerformance,
		Normal,
		HighQuality,
		HighestQuality,
		Custom
	}

	public enum Quality
	{
		Lowest,
		Low,
		Medium,
		High,
		Highest
	}

	public enum Resolution
	{
		Full,
		Half,
		Quarter
	}

	public enum Deinterleaving
	{
		Disabled,
		_2x,
		_4x
	}

	public enum DisplayMode
	{
		Normal,
		AOOnly,
		ColorBleedingOnly,
		SplitWithoutAOAndWithAO,
		SplitWithAOAndAOOnly,
		SplitWithoutAOAndAOOnly
	}

	public enum Blur
	{
		None,
		Narrow,
		Medium,
		Wide,
		ExtraWide
	}

	public enum NoiseType
	{
		Random,
		Dither
	}

	public enum PerPixelNormals
	{
		GBuffer,
		Camera,
		Reconstruct
	}

	[Serializable]
	public struct Presets
	{
		public Preset preset;

		[SerializeField]
		public static Presets defaultPresets => new Presets
		{
			preset = Preset.Normal
		};
	}

	[Serializable]
	public struct GeneralSettings
	{
		[Tooltip("The quality of the AO.")]
		[Space(6f)]
		public Quality quality;

		[Tooltip("The deinterleaving factor.")]
		public Deinterleaving deinterleaving;

		[Tooltip("The resolution at which the AO is calculated.")]
		public Resolution resolution;

		[Tooltip("The type of noise to use.")]
		[Space(10f)]
		public NoiseType noiseType;

		[Tooltip("The way the AO is displayed on screen.")]
		[Space(10f)]
		public DisplayMode displayMode;

		[SerializeField]
		public static GeneralSettings defaultSettings => new GeneralSettings
		{
			quality = Quality.Medium,
			deinterleaving = Deinterleaving.Disabled,
			resolution = Resolution.Full,
			noiseType = NoiseType.Dither,
			displayMode = DisplayMode.Normal
		};
	}

	[Serializable]
	public struct AOSettings
	{
		[Tooltip("AO radius: this is the distance outside which occluders are ignored.")]
		[Space(6f)]
		[Range(0f, 2f)]
		public float radius;

		[Tooltip("Maximum radius in pixels: this prevents the radius to grow too much with close-up object and impact on performances.")]
		[Range(32f, 256f)]
		public float maxRadiusPixels;

		[Tooltip("For low-tessellated geometry, occlusion variations tend to appear at creases and ridges, which betray the underlying tessellation. To remove these artifacts, we use an angle bias parameter which restricts the hemisphere.")]
		[Range(0f, 0.5f)]
		public float bias;

		[Tooltip("This value allows to scale up the ambient occlusion values.")]
		[Range(0f, 10f)]
		public float intensity;

		[Tooltip("This value allows to attenuate ambient occlusion depending on final color luminance.")]
		[Range(0f, 1f)]
		public float luminanceInfluence;

		[Tooltip("The max distance to display AO.")]
		public float maxDistance;

		[Tooltip("The distance before max distance at which AO start to decrease.")]
		public float distanceFalloff;

		[Tooltip("The type of per pixel normals to use.")]
		[Space(10f)]
		public PerPixelNormals perPixelNormals;

		[Tooltip("This setting allow you to set the base color if the AO, the alpha channel value is unused.")]
		[Space(10f)]
		public Color baseColor;

		[SerializeField]
		public static AOSettings defaultSettings => new AOSettings
		{
			radius = 0.8f,
			maxRadiusPixels = 128f,
			bias = 0.05f,
			intensity = 1f,
			luminanceInfluence = 0f,
			maxDistance = 150f,
			distanceFalloff = 50f,
			perPixelNormals = PerPixelNormals.GBuffer,
			baseColor = Color.black
		};
	}

	[Serializable]
	public struct ColorBleedingSettings
	{
		[Space(6f)]
		public bool enabled;

		[Tooltip("This value allows to control the saturation of the color bleeding.")]
		[Space(10f)]
		[Range(0f, 4f)]
		public float saturation;

		[Tooltip("This value allows to scale the contribution of the color bleeding samples.")]
		[Range(0f, 32f)]
		public float albedoMultiplier;

		[SerializeField]
		public static ColorBleedingSettings defaultSettings => new ColorBleedingSettings
		{
			enabled = false,
			saturation = 1f,
			albedoMultiplier = 4f
		};
	}

	[Serializable]
	public struct BlurSettings
	{
		[Tooltip("The type of blur to use.")]
		[Space(6f)]
		public Blur amount;

		[Tooltip("This parameter controls the depth-dependent weight of the bilateral filter, to avoid bleeding across edges. A zero sharpness is a pure Gaussian blur. Increasing the blur sharpness removes bleeding by using lower weights for samples with large depth delta from the current pixel.")]
		[Space(10f)]
		[Range(0f, 16f)]
		public float sharpness;

		[Tooltip("Is the blur downsampled.")]
		public bool downsample;

		[SerializeField]
		public static BlurSettings defaultSettings => new BlurSettings
		{
			amount = Blur.Medium,
			sharpness = 8f,
			downsample = false
		};
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class SettingsGroup : Attribute
	{
	}

	private static class MersenneTwister
	{
		public static float[] Numbers = new float[32]
		{
			0.463937f, 0.340042f, 0.223035f, 0.468465f, 0.322224f, 0.979269f, 0.031798f, 0.973392f, 0.778313f, 0.456168f,
			0.258593f, 0.330083f, 0.387332f, 0.380117f, 0.179842f, 0.910755f, 0.511623f, 0.092933f, 0.180794f, 0.620153f,
			0.101348f, 0.556342f, 0.642479f, 0.442008f, 0.215115f, 0.475218f, 0.157357f, 0.568868f, 0.501241f, 0.629229f,
			0.699218f, 0.707733f
		};
	}

	private static class Pass
	{
		public const int AO_LowestQuality = 0;

		public const int AO_LowQuality = 1;

		public const int AO_MediumQuality = 2;

		public const int AO_HighQuality = 3;

		public const int AO_HighestQuality = 4;

		public const int AO_Deinterleaved_LowestQuality = 5;

		public const int AO_Deinterleaved_LowQuality = 6;

		public const int AO_Deinterleaved_MediumQuality = 7;

		public const int AO_Deinterleaved_HighQuality = 8;

		public const int AO_Deinterleaved_HighestQuality = 9;

		public const int Depth_Deinterleaving_2x2 = 10;

		public const int Depth_Deinterleaving_4x4 = 11;

		public const int Normals_Deinterleaving_2x2 = 12;

		public const int Normals_Deinterleaving_4x4 = 13;

		public const int Atlas = 14;

		public const int Reinterleaving_2x2 = 15;

		public const int Reinterleaving_4x4 = 16;

		public const int Blur_X_Narrow = 17;

		public const int Blur_X_Medium = 18;

		public const int Blur_X_Wide = 19;

		public const int Blur_X_ExtraWide = 20;

		public const int Blur_Y_Narrow = 21;

		public const int Blur_Y_Medium = 22;

		public const int Blur_Y_Wide = 23;

		public const int Blur_Y_ExtraWide = 24;

		public const int Composite = 25;

		public const int Debug_AO_Only = 26;

		public const int Debug_ColorBleeding_Only = 27;

		public const int Debug_Split_WithoutAO_WithAO = 28;

		public const int Debug_Split_WithAO_AOOnly = 29;

		public const int Debug_Split_WithoutAO_AOOnly = 30;
	}

	private class RenderTarget
	{
		public bool orthographic;

		public bool hdr;

		public int width;

		public int height;

		public int fullWidth;

		public int fullHeight;

		public int layerWidth;

		public int layerHeight;

		public int downsamplingFactor;

		public int deinterleavingFactor;

		public int blurDownsamplingFactor;
	}

	public Texture2D noiseTex;

	public Mesh quadMesh;

	public Shader hbaoShader;

	[SerializeField]
	[SettingsGroup]
	private Presets m_Presets = Presets.defaultPresets;

	[SerializeField]
	[SettingsGroup]
	private GeneralSettings m_GeneralSettings = GeneralSettings.defaultSettings;

	[SerializeField]
	[SettingsGroup]
	private AOSettings m_AOSettings = AOSettings.defaultSettings;

	[SerializeField]
	[SettingsGroup]
	private ColorBleedingSettings m_ColorBleedingSettings = ColorBleedingSettings.defaultSettings;

	[SerializeField]
	[SettingsGroup]
	private BlurSettings m_BlurSettings = BlurSettings.defaultSettings;

	private Quality _quality;

	private NoiseType _noiseType;

	private RenderTarget _renderTarget;

	private const int NUM_MRTS = 4;

	private RenderTexture[] _mrtTexDepth = new RenderTexture[16];

	private RenderTexture[] _mrtTexNrm = new RenderTexture[16];

	private RenderTexture[] _mrtTexAO = new RenderTexture[16];

	private RenderBuffer[][] _mrtRB = new RenderBuffer[4][]
	{
		new RenderBuffer[4],
		new RenderBuffer[4],
		new RenderBuffer[4],
		new RenderBuffer[4]
	};

	private RenderBuffer[][] _mrtRBNrm = new RenderBuffer[4][]
	{
		new RenderBuffer[4],
		new RenderBuffer[4],
		new RenderBuffer[4],
		new RenderBuffer[4]
	};

	private Vector4[] _jitter = new Vector4[16];

	private string[] _hbaoShaderKeywords = new string[3];

	private Material _hbaoMaterial;

	private Camera _hbaoCamera;

	private int[] _numSampleDirections = new int[5] { 3, 4, 6, 8, 8 };

	public Presets presets
	{
		get
		{
			return m_Presets;
		}
		set
		{
			m_Presets = value;
		}
	}

	public GeneralSettings generalSettings
	{
		get
		{
			return m_GeneralSettings;
		}
		set
		{
			m_GeneralSettings = value;
		}
	}

	public AOSettings aoSettings
	{
		get
		{
			return m_AOSettings;
		}
		set
		{
			m_AOSettings = value;
		}
	}

	public ColorBleedingSettings colorBleedingSettings
	{
		get
		{
			return m_ColorBleedingSettings;
		}
		set
		{
			m_ColorBleedingSettings = value;
		}
	}

	public BlurSettings blurSettings
	{
		get
		{
			return m_BlurSettings;
		}
		set
		{
			m_BlurSettings = value;
		}
	}

	private void OnEnable()
	{
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			Debug.LogWarning("HBAO shader is not supported on this platform.");
			enabled = false;
		}
		else if (hbaoShader != null && !hbaoShader.isSupported)
		{
			Debug.LogWarning("HBAO shader is not supported on this platform.");
			enabled = false;
		}
		else if (!(hbaoShader == null))
		{
			CreateMaterial();
			_hbaoCamera.depthTextureMode |= DepthTextureMode.Depth;
			if (aoSettings.perPixelNormals == PerPixelNormals.Camera)
			{
				_hbaoCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
			}
		}
	}

	private void OnDisable()
	{
		if (_hbaoMaterial != null)
		{
			UnityEngine.Object.DestroyImmediate(_hbaoMaterial);
		}
		if (noiseTex != null)
		{
			UnityEngine.Object.DestroyImmediate(noiseTex);
		}
		if (quadMesh != null)
		{
			UnityEngine.Object.DestroyImmediate(quadMesh);
		}
	}

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (hbaoShader == null || _hbaoCamera == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		_hbaoCamera.depthTextureMode |= DepthTextureMode.Depth;
		if (aoSettings.perPixelNormals == PerPixelNormals.Camera)
		{
			_hbaoCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
		}
		CheckParameters();
		UpdateShaderProperties();
		UpdateShaderKeywords();
		if (generalSettings.deinterleaving == Deinterleaving._2x)
		{
			RenderHBAODeinterleaved2x(source, destination);
		}
		else if (generalSettings.deinterleaving == Deinterleaving._4x)
		{
			RenderHBAODeinterleaved4x(source, destination);
		}
		else
		{
			RenderHBAO(source, destination);
		}
	}

	private void RenderHBAO(RenderTexture source, RenderTexture destination)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(_renderTarget.fullWidth / _renderTarget.downsamplingFactor, _renderTarget.fullHeight / _renderTarget.downsamplingFactor);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = temporary;
		GL.Clear(clearDepth: false, clearColor: true, Color.white);
		RenderTexture.active = active;
		Graphics.Blit(source, temporary, _hbaoMaterial, GetAoPass());
		if (blurSettings.amount != Blur.None)
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary(_renderTarget.fullWidth / _renderTarget.downsamplingFactor / _renderTarget.blurDownsamplingFactor, _renderTarget.fullHeight / _renderTarget.downsamplingFactor / _renderTarget.blurDownsamplingFactor);
			Graphics.Blit(temporary, temporary2, _hbaoMaterial, GetBlurXPass());
			temporary.DiscardContents();
			Graphics.Blit(temporary2, temporary, _hbaoMaterial, GetBlurYPass());
			RenderTexture.ReleaseTemporary(temporary2);
		}
		_hbaoMaterial.SetTexture("_HBAOTex", temporary);
		Graphics.Blit(source, destination, _hbaoMaterial, GetFinalPass());
		RenderTexture.ReleaseTemporary(temporary);
	}

	private void RenderHBAODeinterleaved2x(RenderTexture source, RenderTexture destination)
	{
		RenderTexture active = RenderTexture.active;
		for (int i = 0; i < 4; i++)
		{
			_mrtTexDepth[i] = RenderTexture.GetTemporary(_renderTarget.layerWidth, _renderTarget.layerHeight, 0, RenderTextureFormat.RFloat);
			_mrtTexNrm[i] = RenderTexture.GetTemporary(_renderTarget.layerWidth, _renderTarget.layerHeight, 0, RenderTextureFormat.ARGB2101010);
			_mrtTexAO[i] = RenderTexture.GetTemporary(_renderTarget.layerWidth, _renderTarget.layerHeight);
			ref RenderBuffer reference = ref _mrtRB[0][i];
			reference = _mrtTexDepth[i].colorBuffer;
			ref RenderBuffer reference2 = ref _mrtRBNrm[0][i];
			reference2 = _mrtTexNrm[i].colorBuffer;
			RenderTexture.active = _mrtTexAO[i];
			GL.Clear(clearDepth: false, clearColor: true, Color.white);
		}
		_hbaoMaterial.SetVector("_Deinterleaving_Offset00", new Vector2(0f, 0f));
		_hbaoMaterial.SetVector("_Deinterleaving_Offset10", new Vector2(1f, 0f));
		_hbaoMaterial.SetVector("_Deinterleaving_Offset01", new Vector2(0f, 1f));
		_hbaoMaterial.SetVector("_Deinterleaving_Offset11", new Vector2(1f, 1f));
		Graphics.SetRenderTarget(_mrtRB[0], _mrtTexDepth[0].depthBuffer);
		_hbaoMaterial.SetPass(10);
		Graphics.DrawMeshNow(quadMesh, Matrix4x4.identity);
		Graphics.SetRenderTarget(_mrtRBNrm[0], _mrtTexNrm[0].depthBuffer);
		_hbaoMaterial.SetPass(12);
		Graphics.DrawMeshNow(quadMesh, Matrix4x4.identity);
		RenderTexture.active = active;
		for (int j = 0; j < 4; j++)
		{
			_hbaoMaterial.SetTexture("_DepthTex", _mrtTexDepth[j]);
			_hbaoMaterial.SetTexture("_NormalsTex", _mrtTexNrm[j]);
			_hbaoMaterial.SetVector("_Jitter", _jitter[j]);
			Graphics.Blit(source, _mrtTexAO[j], _hbaoMaterial, GetAoDeinterleavedPass());
			_mrtTexDepth[j].DiscardContents();
			_mrtTexNrm[j].DiscardContents();
		}
		RenderTexture temporary = RenderTexture.GetTemporary(_renderTarget.fullWidth, _renderTarget.fullHeight);
		for (int k = 0; k < 4; k++)
		{
			_hbaoMaterial.SetVector("_LayerOffset", new Vector2((k & 1) * _renderTarget.layerWidth, (k >> 1) * _renderTarget.layerHeight));
			Graphics.Blit(_mrtTexAO[k], temporary, _hbaoMaterial, 14);
			RenderTexture.ReleaseTemporary(_mrtTexAO[k]);
			RenderTexture.ReleaseTemporary(_mrtTexNrm[k]);
			RenderTexture.ReleaseTemporary(_mrtTexDepth[k]);
		}
		RenderTexture temporary2 = RenderTexture.GetTemporary(_renderTarget.fullWidth, _renderTarget.fullHeight);
		Graphics.Blit(temporary, temporary2, _hbaoMaterial, 15);
		temporary.DiscardContents();
		if (blurSettings.amount != Blur.None)
		{
			if (blurSettings.downsample)
			{
				RenderTexture temporary3 = RenderTexture.GetTemporary(_renderTarget.fullWidth / _renderTarget.blurDownsamplingFactor, _renderTarget.fullHeight / _renderTarget.blurDownsamplingFactor);
				Graphics.Blit(temporary2, temporary3, _hbaoMaterial, GetBlurXPass());
				temporary2.DiscardContents();
				Graphics.Blit(temporary3, temporary2, _hbaoMaterial, GetBlurYPass());
				RenderTexture.ReleaseTemporary(temporary3);
			}
			else
			{
				Graphics.Blit(temporary2, temporary, _hbaoMaterial, GetBlurXPass());
				temporary2.DiscardContents();
				Graphics.Blit(temporary, temporary2, _hbaoMaterial, GetBlurYPass());
			}
		}
		RenderTexture.ReleaseTemporary(temporary);
		_hbaoMaterial.SetTexture("_HBAOTex", temporary2);
		Graphics.Blit(source, destination, _hbaoMaterial, GetFinalPass());
		RenderTexture.ReleaseTemporary(temporary2);
	}

	private void RenderHBAODeinterleaved4x(RenderTexture source, RenderTexture destination)
	{
		RenderTexture active = RenderTexture.active;
		for (int i = 0; i < 16; i++)
		{
			_mrtTexDepth[i] = RenderTexture.GetTemporary(_renderTarget.layerWidth, _renderTarget.layerHeight, 0, RenderTextureFormat.RFloat);
			_mrtTexNrm[i] = RenderTexture.GetTemporary(_renderTarget.layerWidth, _renderTarget.layerHeight, 0, RenderTextureFormat.ARGB2101010);
			_mrtTexAO[i] = RenderTexture.GetTemporary(_renderTarget.layerWidth, _renderTarget.layerHeight);
			RenderTexture.active = _mrtTexAO[i];
			GL.Clear(clearDepth: false, clearColor: true, Color.white);
		}
		for (int j = 0; j < 4; j++)
		{
			for (int k = 0; k < 4; k++)
			{
				ref RenderBuffer reference = ref _mrtRB[j][k];
				reference = _mrtTexDepth[k + 4 * j].colorBuffer;
				ref RenderBuffer reference2 = ref _mrtRBNrm[j][k];
				reference2 = _mrtTexNrm[k + 4 * j].colorBuffer;
			}
		}
		for (int l = 0; l < 4; l++)
		{
			int num = (l & 1) << 1;
			int num2 = l >> 1 << 1;
			_hbaoMaterial.SetVector("_Deinterleaving_Offset00", new Vector2(num, num2));
			_hbaoMaterial.SetVector("_Deinterleaving_Offset10", new Vector2(num + 1, num2));
			_hbaoMaterial.SetVector("_Deinterleaving_Offset01", new Vector2(num, num2 + 1));
			_hbaoMaterial.SetVector("_Deinterleaving_Offset11", new Vector2(num + 1, num2 + 1));
			Graphics.SetRenderTarget(_mrtRB[l], _mrtTexDepth[4 * l].depthBuffer);
			_hbaoMaterial.SetPass(11);
			Graphics.DrawMeshNow(quadMesh, Matrix4x4.identity);
			Graphics.SetRenderTarget(_mrtRBNrm[l], _mrtTexNrm[4 * l].depthBuffer);
			_hbaoMaterial.SetPass(13);
			Graphics.DrawMeshNow(quadMesh, Matrix4x4.identity);
		}
		RenderTexture.active = active;
		for (int m = 0; m < 16; m++)
		{
			_hbaoMaterial.SetTexture("_DepthTex", _mrtTexDepth[m]);
			_hbaoMaterial.SetTexture("_NormalsTex", _mrtTexNrm[m]);
			_hbaoMaterial.SetVector("_Jitter", _jitter[m]);
			Graphics.Blit(source, _mrtTexAO[m], _hbaoMaterial, GetAoDeinterleavedPass());
			_mrtTexDepth[m].DiscardContents();
			_mrtTexNrm[m].DiscardContents();
		}
		RenderTexture temporary = RenderTexture.GetTemporary(_renderTarget.fullWidth, _renderTarget.fullHeight);
		for (int n = 0; n < 16; n++)
		{
			_hbaoMaterial.SetVector("_LayerOffset", new Vector2(((n & 1) + ((n & 7) >> 2 << 1)) * _renderTarget.layerWidth, (((n & 3) >> 1) + (n >> 3 << 1)) * _renderTarget.layerHeight));
			Graphics.Blit(_mrtTexAO[n], temporary, _hbaoMaterial, 14);
			RenderTexture.ReleaseTemporary(_mrtTexAO[n]);
			RenderTexture.ReleaseTemporary(_mrtTexNrm[n]);
			RenderTexture.ReleaseTemporary(_mrtTexDepth[n]);
		}
		RenderTexture temporary2 = RenderTexture.GetTemporary(_renderTarget.fullWidth, _renderTarget.fullHeight);
		Graphics.Blit(temporary, temporary2, _hbaoMaterial, 16);
		temporary.DiscardContents();
		if (blurSettings.amount != Blur.None)
		{
			if (blurSettings.downsample)
			{
				RenderTexture temporary3 = RenderTexture.GetTemporary(_renderTarget.fullWidth / _renderTarget.blurDownsamplingFactor, _renderTarget.fullHeight / _renderTarget.blurDownsamplingFactor);
				Graphics.Blit(temporary2, temporary3, _hbaoMaterial, GetBlurXPass());
				temporary2.DiscardContents();
				Graphics.Blit(temporary3, temporary2, _hbaoMaterial, GetBlurYPass());
				RenderTexture.ReleaseTemporary(temporary3);
			}
			else
			{
				Graphics.Blit(temporary2, temporary, _hbaoMaterial, GetBlurXPass());
				temporary2.DiscardContents();
				Graphics.Blit(temporary, temporary2, _hbaoMaterial, GetBlurYPass());
			}
		}
		RenderTexture.ReleaseTemporary(temporary);
		_hbaoMaterial.SetTexture("_HBAOTex", temporary2);
		Graphics.Blit(source, destination, _hbaoMaterial, GetFinalPass());
		RenderTexture.ReleaseTemporary(temporary2);
	}

	private void CreateMaterial()
	{
		if (_hbaoMaterial == null)
		{
			_hbaoMaterial = new Material(hbaoShader);
			_hbaoMaterial.hideFlags = HideFlags.HideAndDontSave;
			_hbaoCamera = GetComponent<Camera>();
		}
		if (quadMesh != null)
		{
			UnityEngine.Object.DestroyImmediate(quadMesh);
		}
		quadMesh = new Mesh();
		quadMesh.vertices = new Vector3[4]
		{
			new Vector3(-0.5f, -0.5f, 0f),
			new Vector3(0.5f, 0.5f, 0f),
			new Vector3(0.5f, -0.5f, 0f),
			new Vector3(-0.5f, 0.5f, 0f)
		};
		quadMesh.uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f)
		};
		quadMesh.triangles = new int[6] { 0, 1, 2, 1, 0, 3 };
		_renderTarget = new RenderTarget();
	}

	private void UpdateShaderProperties()
	{
		_renderTarget.orthographic = _hbaoCamera.orthographic;
		_renderTarget.hdr = _hbaoCamera.hdr;
		_renderTarget.width = _hbaoCamera.pixelWidth;
		_renderTarget.height = _hbaoCamera.pixelHeight;
		_renderTarget.downsamplingFactor = ((generalSettings.resolution == Resolution.Full) ? 1 : ((generalSettings.resolution != Resolution.Half) ? 4 : 2));
		_renderTarget.deinterleavingFactor = GetDeinterleavingFactor();
		_renderTarget.blurDownsamplingFactor = ((!blurSettings.downsample) ? 1 : 2);
		float num = Mathf.Tan(0.5f * _hbaoCamera.fieldOfView * ((float)Math.PI / 180f));
		float num2 = 1f / (1f / num * ((float)_renderTarget.height / (float)_renderTarget.width));
		float num3 = 1f / (1f / num);
		_hbaoMaterial.SetVector("_UVToView", new Vector4(2f * num2, -2f * num3, -1f * num2, 1f * num3));
		_hbaoMaterial.SetMatrix("_WorldToCameraMatrix", _hbaoCamera.worldToCameraMatrix);
		if (generalSettings.deinterleaving != Deinterleaving.Disabled)
		{
			_renderTarget.fullWidth = _renderTarget.width + ((_renderTarget.width % _renderTarget.deinterleavingFactor != 0) ? (_renderTarget.deinterleavingFactor - _renderTarget.width % _renderTarget.deinterleavingFactor) : 0);
			_renderTarget.fullHeight = _renderTarget.height + ((_renderTarget.height % _renderTarget.deinterleavingFactor != 0) ? (_renderTarget.deinterleavingFactor - _renderTarget.height % _renderTarget.deinterleavingFactor) : 0);
			_renderTarget.layerWidth = _renderTarget.fullWidth / _renderTarget.deinterleavingFactor;
			_renderTarget.layerHeight = _renderTarget.fullHeight / _renderTarget.deinterleavingFactor;
			_hbaoMaterial.SetVector("_FullRes_TexelSize", new Vector4(1f / (float)_renderTarget.fullWidth, 1f / (float)_renderTarget.fullHeight, _renderTarget.fullWidth, _renderTarget.fullHeight));
			_hbaoMaterial.SetVector("_LayerRes_TexelSize", new Vector4(1f / (float)_renderTarget.layerWidth, 1f / (float)_renderTarget.layerHeight, _renderTarget.layerWidth, _renderTarget.layerHeight));
			_hbaoMaterial.SetVector("_TargetScale", new Vector4((float)_renderTarget.fullWidth / (float)_renderTarget.width, (float)_renderTarget.fullHeight / (float)_renderTarget.height, 1f / ((float)_renderTarget.fullWidth / (float)_renderTarget.width), 1f / ((float)_renderTarget.fullHeight / (float)_renderTarget.height)));
		}
		else
		{
			_renderTarget.fullWidth = _renderTarget.width;
			_renderTarget.fullHeight = _renderTarget.height;
			if (generalSettings.resolution == Resolution.Half && aoSettings.perPixelNormals == PerPixelNormals.Reconstruct)
			{
				_hbaoMaterial.SetVector("_TargetScale", new Vector4(((float)_renderTarget.width + 0.5f) / (float)_renderTarget.width, ((float)_renderTarget.height + 0.5f) / (float)_renderTarget.height, 1f, 1f));
			}
			else
			{
				_hbaoMaterial.SetVector("_TargetScale", new Vector4(1f, 1f, 1f, 1f));
			}
		}
		if (noiseTex == null || _quality != generalSettings.quality || _noiseType != generalSettings.noiseType)
		{
			if (noiseTex != null)
			{
				UnityEngine.Object.DestroyImmediate(noiseTex);
			}
			float num4 = ((generalSettings.noiseType != NoiseType.Dither) ? 64 : 4);
			CreateRandomTexture((int)num4);
		}
		_quality = generalSettings.quality;
		_noiseType = generalSettings.noiseType;
		_hbaoMaterial.SetTexture("_NoiseTex", noiseTex);
		_hbaoMaterial.SetFloat("_NoiseTexSize", (_noiseType != NoiseType.Dither) ? 64 : 4);
		_hbaoMaterial.SetFloat("_Radius", aoSettings.radius * 0.5f * ((float)_renderTarget.height / (num * 2f)) / (float)_renderTarget.deinterleavingFactor);
		_hbaoMaterial.SetFloat("_MaxRadiusPixels", aoSettings.maxRadiusPixels / (float)_renderTarget.deinterleavingFactor);
		_hbaoMaterial.SetFloat("_NegInvRadius2", -1f / (aoSettings.radius * aoSettings.radius));
		_hbaoMaterial.SetFloat("_AngleBias", aoSettings.bias);
		_hbaoMaterial.SetFloat("_AOmultiplier", 2f * (1f / (1f - aoSettings.bias)));
		_hbaoMaterial.SetFloat("_Intensity", aoSettings.intensity);
		_hbaoMaterial.SetFloat("_LuminanceInfluence", aoSettings.luminanceInfluence);
		_hbaoMaterial.SetFloat("_MaxDistance", aoSettings.maxDistance);
		_hbaoMaterial.SetFloat("_DistanceFalloff", aoSettings.distanceFalloff);
		_hbaoMaterial.SetColor("_BaseColor", aoSettings.baseColor);
		_hbaoMaterial.SetFloat("_ColorBleedSaturation", colorBleedingSettings.saturation);
		_hbaoMaterial.SetFloat("_AlbedoMultiplier", colorBleedingSettings.albedoMultiplier);
		_hbaoMaterial.SetFloat("_BlurSharpness", blurSettings.sharpness);
	}

	private void UpdateShaderKeywords()
	{
		_hbaoShaderKeywords[0] = ((!colorBleedingSettings.enabled) ? "__" : "COLOR_BLEEDING_ON");
		if (_renderTarget.orthographic)
		{
			_hbaoShaderKeywords[1] = "ORTHOGRAPHIC_PROJECTION_ON";
		}
		else
		{
			_hbaoShaderKeywords[1] = ((!IsDeferredShading()) ? "__" : "DEFERRED_SHADING_ON");
		}
		ref string reference = ref _hbaoShaderKeywords[2];
		string text;
		if (aoSettings.perPixelNormals == PerPixelNormals.Camera)
		{
			text = "NORMALS_CAMERA";
		}
		else
		{
			text = ((aoSettings.perPixelNormals != PerPixelNormals.Reconstruct) ? "__" : "NORMALS_RECONSTRUCT");
		}
		reference = text;
		_hbaoMaterial.shaderKeywords = _hbaoShaderKeywords;
	}

	private void CheckParameters()
	{
		if (!IsDeferredShading() && aoSettings.perPixelNormals == PerPixelNormals.GBuffer)
		{
			m_AOSettings.perPixelNormals = PerPixelNormals.Camera;
		}
		if (generalSettings.deinterleaving != Deinterleaving.Disabled && SystemInfo.supportedRenderTargetCount < 4)
		{
			m_GeneralSettings.deinterleaving = Deinterleaving.Disabled;
		}
	}

	private bool IsDeferredShading()
	{
		return _hbaoCamera.actualRenderingPath == RenderingPath.DeferredShading;
	}

	private int GetDeinterleavingFactor()
	{
		return generalSettings.deinterleaving switch
		{
			Deinterleaving._2x => 2, 
			Deinterleaving._4x => 4, 
			_ => 1, 
		};
	}

	private int GetAoPass()
	{
		return generalSettings.quality switch
		{
			Quality.Lowest => 0, 
			Quality.Low => 1, 
			Quality.Medium => 2, 
			Quality.High => 3, 
			Quality.Highest => 4, 
			_ => 2, 
		};
	}

	private int GetAoDeinterleavedPass()
	{
		return generalSettings.quality switch
		{
			Quality.Lowest => 5, 
			Quality.Low => 6, 
			Quality.Medium => 7, 
			Quality.High => 8, 
			Quality.Highest => 9, 
			_ => 7, 
		};
	}

	private int GetBlurXPass()
	{
		return blurSettings.amount switch
		{
			Blur.Narrow => 17, 
			Blur.Medium => 18, 
			Blur.Wide => 19, 
			Blur.ExtraWide => 20, 
			_ => 18, 
		};
	}

	private int GetBlurYPass()
	{
		return blurSettings.amount switch
		{
			Blur.Narrow => 21, 
			Blur.Medium => 22, 
			Blur.Wide => 23, 
			Blur.ExtraWide => 24, 
			_ => 22, 
		};
	}

	private int GetFinalPass()
	{
		return generalSettings.displayMode switch
		{
			DisplayMode.Normal => 25, 
			DisplayMode.AOOnly => 26, 
			DisplayMode.ColorBleedingOnly => 27, 
			DisplayMode.SplitWithoutAOAndWithAO => 28, 
			DisplayMode.SplitWithAOAndAOOnly => 29, 
			DisplayMode.SplitWithoutAOAndAOOnly => 30, 
			_ => 25, 
		};
	}

	private void CreateRandomTexture(int size)
	{
		noiseTex = new Texture2D(size, size, TextureFormat.RGB24, mipmap: false, linear: true);
		noiseTex.filterMode = FilterMode.Point;
		noiseTex.wrapMode = TextureWrapMode.Repeat;
		int num = 0;
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float num2 = ((generalSettings.noiseType != NoiseType.Dither) ? UnityEngine.Random.Range(0f, 1f) : MersenneTwister.Numbers[num++]);
				float b = ((generalSettings.noiseType != NoiseType.Dither) ? UnityEngine.Random.Range(0f, 1f) : MersenneTwister.Numbers[num++]);
				float f = (float)Math.PI * 2f * num2 / (float)_numSampleDirections[GetAoPass()];
				Color color = new Color(Mathf.Cos(f), Mathf.Sin(f), b);
				noiseTex.SetPixel(i, j, color);
			}
		}
		noiseTex.Apply();
		int k = 0;
		int num3 = 0;
		for (; k < _jitter.Length; k++)
		{
			float num4 = MersenneTwister.Numbers[num3++];
			float z = MersenneTwister.Numbers[num3++];
			float f2 = (float)Math.PI * 2f * num4 / (float)_numSampleDirections[GetAoPass()];
			ref Vector4 reference = ref _jitter[k];
			reference = new Vector4(Mathf.Cos(f2), Mathf.Sin(f2), z, 0f);
		}
	}

	public void ApplyPreset(Preset preset)
	{
		if (preset == Preset.Custom)
		{
			m_Presets.preset = preset;
			return;
		}
		DisplayMode displayMode = generalSettings.displayMode;
		m_GeneralSettings = GeneralSettings.defaultSettings;
		m_AOSettings = AOSettings.defaultSettings;
		m_ColorBleedingSettings = ColorBleedingSettings.defaultSettings;
		m_BlurSettings = BlurSettings.defaultSettings;
		m_GeneralSettings.displayMode = displayMode;
		switch (preset)
		{
		case Preset.FastestPerformance:
			m_GeneralSettings.quality = Quality.Lowest;
			m_AOSettings.radius = 0.5f;
			m_AOSettings.maxRadiusPixels = 64f;
			m_BlurSettings.amount = Blur.ExtraWide;
			m_BlurSettings.downsample = true;
			break;
		case Preset.FastPerformance:
			m_GeneralSettings.quality = Quality.Low;
			m_AOSettings.radius = 0.5f;
			m_AOSettings.maxRadiusPixels = 64f;
			m_BlurSettings.amount = Blur.Wide;
			m_BlurSettings.downsample = true;
			break;
		case Preset.HighQuality:
			m_GeneralSettings.quality = Quality.High;
			m_AOSettings.radius = 1f;
			break;
		case Preset.HighestQuality:
			m_GeneralSettings.quality = Quality.Highest;
			m_AOSettings.radius = 1.2f;
			m_AOSettings.maxRadiusPixels = 256f;
			m_BlurSettings.amount = Blur.Narrow;
			break;
		}
		m_Presets.preset = preset;
	}
}
