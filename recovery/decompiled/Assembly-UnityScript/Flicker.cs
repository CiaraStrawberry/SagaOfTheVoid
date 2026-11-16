using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class Flicker : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Start_002478 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal float _0024_002435_002479;

			internal Color _0024_002436_002480;

			internal Flicker _0024self__002481;

			public _0024(Flicker self_)
			{
				_0024self__002481 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					_0024self__002481.screenContent = _0024self__002481.transform.Find("Screen_Content").gameObject;
					goto case 4;
				case 4:
				{
					_0024self__002481.screenContent.active = true;
					float num = (_0024_002435_002479 = UnityEngine.Random.Range(_0024self__002481.minAlpha, _0024self__002481.maxAlpha));
					Color color = (_0024_002436_002480 = _0024self__002481.screenContent.GetComponent<Renderer>().material.color);
					float num2 = (_0024_002436_002480.a = _0024_002435_002479);
					Color color2 = (_0024self__002481.screenContent.GetComponent<Renderer>().material.color = _0024_002436_002480);
					result = (Yield(2, new WaitForSeconds(UnityEngine.Random.Range(_0024self__002481.minOnDelay, _0024self__002481.maxOnDelay))) ? 1 : 0);
					break;
				}
				case 2:
					_0024self__002481.screenContent.active = false;
					result = (Yield(3, new WaitForSeconds(UnityEngine.Random.Range(_0024self__002481.minOffDelay, _0024self__002481.maxOffDelay))) ? 1 : 0);
					break;
				case 3:
					result = (YieldDefault(4) ? 1 : 0);
					break;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal Flicker _0024self__002482;

		public _0024Start_002478(Flicker self_)
		{
			_0024self__002482 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002482);
		}
	}

	public float minOnDelay;

	public float maxOnDelay;

	public float minOffDelay;

	public float maxOffDelay;

	public float minAlpha;

	public float maxAlpha;

	private GameObject screenContent;

	public Flicker()
	{
		minOnDelay = 0.5f;
		maxOnDelay = 5f;
		minOffDelay = 0.01f;
		maxOffDelay = 0.02f;
		minAlpha = 0.6f;
		maxAlpha = 0.7f;
	}

	public virtual IEnumerator Start()
	{
		return new _0024Start_002478(this).GetEnumerator();
	}

	public virtual void Main()
	{
	}
}
