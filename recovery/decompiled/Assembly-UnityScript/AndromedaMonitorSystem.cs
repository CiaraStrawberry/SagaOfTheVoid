using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using Boo.Lang.Runtime;
using UnityEngine;

[Serializable]
public class AndromedaMonitorSystem : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024floatingup_002465 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal float _0024_002425_002466;

			internal Vector3 _0024_002426_002467;

			internal AndromedaMonitorSystem _0024self__002468;

			public _0024(AndromedaMonitorSystem self_)
			{
				_0024self__002468 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
				{
					float num = (_0024_002425_002466 = _0024self__002468.transform.position.y + _0024self__002468.shakeIntensity * Time.deltaTime);
					Vector3 vector = (_0024_002426_002467 = _0024self__002468.transform.position);
					float num2 = (_0024_002426_002467.y = _0024_002425_002466);
					Vector3 vector2 = (_0024self__002468.transform.position = _0024_002426_002467);
					result = (Yield(2, new WaitForSeconds(_0024self__002468.floatUpSpeed)) ? 1 : 0);
					break;
				}
				case 2:
					_0024self__002468.floatup = false;
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal AndromedaMonitorSystem _0024self__002469;

		public _0024floatingup_002465(AndromedaMonitorSystem self_)
		{
			_0024self__002469 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002469);
		}
	}

	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024floatingdown_002470 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal float _0024_002427_002471;

			internal Vector3 _0024_002428_002472;

			internal AndromedaMonitorSystem _0024self__002473;

			public _0024(AndromedaMonitorSystem self_)
			{
				_0024self__002473 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
				{
					float num = (_0024_002427_002471 = _0024self__002473.transform.position.y - _0024self__002473.shakeIntensity * Time.deltaTime);
					Vector3 vector = (_0024_002428_002472 = _0024self__002473.transform.position);
					float num2 = (_0024_002428_002472.y = _0024_002427_002471);
					Vector3 vector2 = (_0024self__002473.transform.position = _0024_002428_002472);
					result = (Yield(2, new WaitForSeconds(_0024self__002473.floatDownSpeed)) ? 1 : 0);
					break;
				}
				case 2:
					_0024self__002473.floatup = true;
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal AndromedaMonitorSystem _0024self__002474;

		public _0024floatingdown_002470(AndromedaMonitorSystem self_)
		{
			_0024self__002474 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002474);
		}
	}

	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Delay_002475 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal AndromedaMonitorSystem _0024self__002476;

			public _0024(AndromedaMonitorSystem self_)
			{
				_0024self__002476 = self_;
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
					_0024self__002476.GetComponent<AudioSource>().enabled = false;
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal AndromedaMonitorSystem _0024self__002477;

		public _0024Delay_002475(AndromedaMonitorSystem self_)
		{
			_0024self__002477 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002477);
		}
	}

	public float floatUpSpeed;

	public float floatDownSpeed;

	public float shakeIntensity;

	public GameObject hologramModel1;

	public GameObject hologramModel2;

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

	public AndromedaMonitorSystem()
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
		if (hologramModel1 == null)
		{
			Debug.LogError("You need to apply a plane model to the Monitor Static slot. The model must contain the Hologram Static Shader. Refer to the demo scene for an example if needed.");
		}
		if (hologramModel2 == null)
		{
			Debug.LogError("You need to apply a plane model to the Monitor Texture slot. The model must contain the Hologram Shader. Refer to the demo scene for an example if needed.");
		}
	}

	public virtual void Update()
	{
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
				GetComponent<AudioSource>().volume = flickerSpeed;
				StartCoroutine(Delay());
			}
			if (!(flickerSpeed >= 0.25f))
			{
				GetComponent<AudioSource>().enabled = false;
			}
		}
		float a = flickerSpeed;
		Color color = hologramModel1.GetComponent<Renderer>().material.color;
		float num = (color.a = a);
		Color color2 = (hologramModel1.GetComponent<Renderer>().material.color = color);
		float a2 = flickerSpeed;
		Color color4 = hologramModel2.GetComponent<Renderer>().material.color;
		float num2 = (color4.a = a2);
		Color color5 = (hologramModel2.GetComponent<Renderer>().material.color = color4);
		if (!(maxFlicker <= 2f))
		{
			Debug.LogError("Max flicker amount should not exceed 2");
		}
		if (!(minFlicker >= -1f))
		{
			Debug.LogError("Min flicker amount should not go below -1");
		}
		hologramModel1.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY);
	}

	public virtual IEnumerator floatingup()
	{
		return new _0024floatingup_002465(this).GetEnumerator();
	}

	public virtual IEnumerator floatingdown()
	{
		return new _0024floatingdown_002470(this).GetEnumerator();
	}

	public virtual IEnumerator Delay()
	{
		return new _0024Delay_002475(this).GetEnumerator();
	}

	public virtual void Main()
	{
	}
}
