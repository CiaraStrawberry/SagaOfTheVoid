using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using Boo.Lang.Runtime;
using UnityEngine;

[Serializable]
public class Andromeda2DSystem : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024floatingup_002439 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal float _0024_00245_002440;

			internal Vector3 _0024_00246_002441;

			internal Andromeda2DSystem _0024self__002442;

			public _0024(Andromeda2DSystem self_)
			{
				_0024self__002442 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
				{
					float num = (_0024_00245_002440 = _0024self__002442.transform.position.y + _0024self__002442.shakeIntensity * Time.deltaTime);
					Vector3 vector = (_0024_00246_002441 = _0024self__002442.transform.position);
					float num2 = (_0024_00246_002441.y = _0024_00245_002440);
					Vector3 vector2 = (_0024self__002442.transform.position = _0024_00246_002441);
					result = (Yield(2, new WaitForSeconds(_0024self__002442.floatUpSpeed)) ? 1 : 0);
					break;
				}
				case 2:
					_0024self__002442.floatup = false;
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal Andromeda2DSystem _0024self__002443;

		public _0024floatingup_002439(Andromeda2DSystem self_)
		{
			_0024self__002443 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002443);
		}
	}

	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024floatingdown_002444 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal float _0024_00247_002445;

			internal Vector3 _0024_00248_002446;

			internal Andromeda2DSystem _0024self__002447;

			public _0024(Andromeda2DSystem self_)
			{
				_0024self__002447 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
				{
					float num = (_0024_00247_002445 = _0024self__002447.transform.position.y - _0024self__002447.shakeIntensity * Time.deltaTime);
					Vector3 vector = (_0024_00248_002446 = _0024self__002447.transform.position);
					float num2 = (_0024_00248_002446.y = _0024_00247_002445);
					Vector3 vector2 = (_0024self__002447.transform.position = _0024_00248_002446);
					result = (Yield(2, new WaitForSeconds(_0024self__002447.floatDownSpeed)) ? 1 : 0);
					break;
				}
				case 2:
					_0024self__002447.floatup = true;
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal Andromeda2DSystem _0024self__002448;

		public _0024floatingdown_002444(Andromeda2DSystem self_)
		{
			_0024self__002448 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002448);
		}
	}

	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Delay_002449 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal Andromeda2DSystem _0024self__002450;

			public _0024(Andromeda2DSystem self_)
			{
				_0024self__002450 = self_;
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
					_0024self__002450.GetComponent<AudioSource>().enabled = false;
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal Andromeda2DSystem _0024self__002451;

		public _0024Delay_002449(Andromeda2DSystem self_)
		{
			_0024self__002451 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002451);
		}
	}

	public float floatUpSpeed;

	public float floatDownSpeed;

	public float shakeIntensity;

	public GameObject hologramPlane1;

	public GameObject hologramPlane2;

	public bool doesRotate;

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

	public Andromeda2DSystem()
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
		if (hologramPlane1 == null)
		{
			Debug.LogError("You need to apply a plane model to the Hologram Plane 1 slot. The model must contain the Hologram Shader. Refer to the demo scene for an example if needed.");
		}
		if (hologramPlane2 == null)
		{
			Debug.LogError("You need to apply a plane model to the Hologram Plane 2 slot. The model must contain the Hologram Shader. Refer to the demo scene for an example if needed.");
		}
	}

	public virtual void Update()
	{
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
		Color color = hologramPlane1.GetComponent<Renderer>().material.color;
		float num = (color.a = a);
		Color color2 = (hologramPlane1.GetComponent<Renderer>().material.color = color);
		float a2 = flickerSpeed;
		Color color4 = hologramPlane2.GetComponent<Renderer>().material.color;
		float num2 = (color4.a = a2);
		Color color5 = (hologramPlane2.GetComponent<Renderer>().material.color = color4);
		if (!(maxFlicker <= 2f))
		{
			Debug.LogError("Max flicker amount should not exceed 2");
		}
		if (!(minFlicker >= -1f))
		{
			Debug.LogError("Min flicker amount should not go below -1");
		}
	}

	public virtual IEnumerator floatingup()
	{
		return new _0024floatingup_002439(this).GetEnumerator();
	}

	public virtual IEnumerator floatingdown()
	{
		return new _0024floatingdown_002444(this).GetEnumerator();
	}

	public virtual IEnumerator Delay()
	{
		return new _0024Delay_002449(this).GetEnumerator();
	}

	public virtual void Main()
	{
	}
}
