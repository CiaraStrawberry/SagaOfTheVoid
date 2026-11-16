using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using Boo.Lang.Runtime;
using UnityEngine;

[Serializable]
public class AndromedaModelSystem : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024floatingup_002452 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal float _0024_002417_002453;

			internal Vector3 _0024_002418_002454;

			internal AndromedaModelSystem _0024self__002455;

			public _0024(AndromedaModelSystem self_)
			{
				_0024self__002455 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
				{
					float num = (_0024_002417_002453 = _0024self__002455.transform.position.y + _0024self__002455.shakeIntensity * Time.deltaTime);
					Vector3 vector = (_0024_002418_002454 = _0024self__002455.transform.position);
					float num2 = (_0024_002418_002454.y = _0024_002417_002453);
					Vector3 vector2 = (_0024self__002455.transform.position = _0024_002418_002454);
					result = (Yield(2, new WaitForSeconds(_0024self__002455.floatUpSpeed)) ? 1 : 0);
					break;
				}
				case 2:
					_0024self__002455.floatup = false;
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal AndromedaModelSystem _0024self__002456;

		public _0024floatingup_002452(AndromedaModelSystem self_)
		{
			_0024self__002456 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002456);
		}
	}

	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024floatingdown_002457 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal float _0024_002419_002458;

			internal Vector3 _0024_002420_002459;

			internal AndromedaModelSystem _0024self__002460;

			public _0024(AndromedaModelSystem self_)
			{
				_0024self__002460 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
				{
					float num = (_0024_002419_002458 = _0024self__002460.transform.position.y - _0024self__002460.shakeIntensity * Time.deltaTime);
					Vector3 vector = (_0024_002420_002459 = _0024self__002460.transform.position);
					float num2 = (_0024_002420_002459.y = _0024_002419_002458);
					Vector3 vector2 = (_0024self__002460.transform.position = _0024_002420_002459);
					result = (Yield(2, new WaitForSeconds(_0024self__002460.floatDownSpeed)) ? 1 : 0);
					break;
				}
				case 2:
					_0024self__002460.floatup = true;
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal AndromedaModelSystem _0024self__002461;

		public _0024floatingdown_002457(AndromedaModelSystem self_)
		{
			_0024self__002461 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002461);
		}
	}

	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Delay_002462 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal AndromedaModelSystem _0024self__002463;

			public _0024(AndromedaModelSystem self_)
			{
				_0024self__002463 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					result = (Yield(2, new WaitForSeconds(0.05f)) ? 1 : 0);
					break;
				case 2:
					_0024self__002463.GetComponent<AudioSource>().enabled = false;
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal AndromedaModelSystem _0024self__002464;

		public _0024Delay_002462(AndromedaModelSystem self_)
		{
			_0024self__002464 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002464);
		}
	}

	public float floatUpSpeed;

	public float floatDownSpeed;

	public float shakeIntensity;

	public GameObject hologramModel;

	public float rotateSpeed;

	private object floatup;

	private float flickerSpeed;

	public float minFlicker;

	public float maxFlicker;

	public float offsetX;

	public float offsetY;

	public float xSpeed;

	public float ySpeed;

	public Light hologramLight;

	public AudioClip hologramSound;

	public bool useLight;

	public bool useSound;

	public AndromedaModelSystem()
	{
		floatUpSpeed = 0.01f;
		floatDownSpeed = 0.01f;
		shakeIntensity = 0.1f;
		maxFlicker = 1f;
		xSpeed = 1f;
		ySpeed = 1f;
	}

	public virtual void Awake()
	{
		GetComponent<AudioSource>().clip = hologramSound;
		GetComponent<AudioSource>().enabled = false;
	}

	public virtual void Start()
	{
		floatup = false;
		if (useSound)
		{
			GetComponent<AudioSource>().enabled = true;
		}
		if (hologramModel == null)
		{
			Debug.LogError("You need to apply a model to the Hologram Model slot. The model must contain 1st the the Hologram Static Shader then the Hologram Solid Shader. Refer to the demo scene for an example if needed.");
		}
	}

	public virtual void Update()
	{
		transform.Rotate(Vector3.right * rotateSpeed);
		transform.Rotate(Vector3.up * rotateSpeed, Space.World);
		int num = 0;
		Quaternion rotation = transform.rotation;
		Vector3 eulerAngles = rotation.eulerAngles;
		float num2 = (eulerAngles.x = num);
		Vector3 vector = (rotation.eulerAngles = eulerAngles);
		Quaternion quaternion = (transform.rotation = rotation);
		int num3 = 0;
		Quaternion rotation2 = transform.rotation;
		Vector3 eulerAngles2 = rotation2.eulerAngles;
		float num4 = (eulerAngles2.z = num3);
		Vector3 vector3 = (rotation2.eulerAngles = eulerAngles2);
		Quaternion quaternion3 = (transform.rotation = rotation2);
		offsetX = Time.time * xSpeed;
		offsetY = Time.time * ySpeed;
		if (RuntimeServices.ToBool(floatup))
		{
			StartCoroutine(floatingup());
		}
		else if (!RuntimeServices.ToBool(floatup))
		{
			StartCoroutine(floatingdown());
		}
		flickerSpeed = UnityEngine.Random.Range(minFlicker, maxFlicker);
		if (useLight)
		{
			hologramLight.intensity = flickerSpeed;
		}
		if (useSound && GetComponent<AudioSource>().clip == hologramSound)
		{
			if (!(flickerSpeed <= 0.25f))
			{
				GetComponent<AudioSource>().enabled = true;
				GetComponent<AudioSource>().volume = flickerSpeed + 0.3f;
				StartCoroutine(Delay());
			}
			if (!(flickerSpeed >= 0.25f))
			{
				GetComponent<AudioSource>().enabled = false;
			}
		}
		float a = flickerSpeed;
		Color color = hologramModel.GetComponent<Renderer>().material.color;
		float num5 = (color.a = a);
		Color color2 = (hologramModel.GetComponent<Renderer>().material.color = color);
		if (!(maxFlicker <= 2f))
		{
			Debug.LogError("Max flicker amount should not exceed 2");
		}
		if (!(minFlicker >= -1f))
		{
			Debug.LogError("Min flicker amount should not go below -1");
		}
		hologramModel.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
	}

	public virtual IEnumerator floatingup()
	{
		return new _0024floatingup_002452(this).GetEnumerator();
	}

	public virtual IEnumerator floatingdown()
	{
		return new _0024floatingdown_002457(this).GetEnumerator();
	}

	public virtual IEnumerator Delay()
	{
		return new _0024Delay_002462(this).GetEnumerator();
	}

	public virtual void Main()
	{
	}
}
