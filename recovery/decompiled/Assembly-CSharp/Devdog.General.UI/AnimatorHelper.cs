using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Devdog.General.UI;

[RequireComponent(typeof(Animator))]
public sealed class AnimatorHelper : MonoBehaviour
{
	private Animator _animator;

	private Regex _animationNameRegex;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_animationNameRegex = new Regex("(\\w*)(?:[(]*([0-9,]+\\.?[0-9,.]*)*[)]*)", RegexOptions.Singleline);
	}

	public void Play(string name)
	{
		Match match = _animationNameRegex.Match(name);
		if (match.Groups.Count == 3)
		{
			_animator.enabled = true;
			float result2;
			if (match.Groups[2].Value == string.Empty)
			{
				_animator.Play(match.Groups[1].Value);
			}
			else if (match.Groups[2].Value.Contains(","))
			{
				string[] array = match.Groups[2].Value.Split(',');
				List<float> list = new List<float>(array.Length);
				string[] array2 = array;
				foreach (string text in array2)
				{
					string text2 = text.Trim();
					if (!(text2 == string.Empty))
					{
						if (!float.TryParse(text2, out var result))
						{
							Debug.LogWarning("Parameter (" + text2 + ") passed is not a number, use (0.3), not (0.3f) or (0,3).", transform);
							_animator.enabled = false;
							return;
						}
						list.Add(result);
					}
				}
				if (gameObject.activeInHierarchy)
				{
					StartCoroutine(PlayAnimationAfter(match.Groups[1].Value, list.ToArray()));
				}
			}
			else if (!float.TryParse(match.Groups[2].Value, out result2))
			{
				Debug.LogWarning("Parameter passed is not a number, use (0.3), not (0.3f) or (0,3).", transform);
				_animator.enabled = false;
			}
			else if (gameObject.activeInHierarchy)
			{
				StartCoroutine(PlayAnimationAfter(match.Groups[1].Value, result2, 1f));
			}
		}
		else
		{
			Debug.LogWarning("InventoryAnimatorHelper: Regex failed, most the string passed is likely faulty.", transform);
		}
	}

	private IEnumerator PlayAnimationAfter(string animationName, params float[] paramsFloat)
	{
		_animator.Play(animationName);
		_animator.speed = 0f;
		if (paramsFloat[0] > 0f)
		{
			yield return new WaitForSeconds(paramsFloat[0]);
		}
		if (paramsFloat.Length > 1)
		{
			_animator.speed = paramsFloat[1];
		}
		else
		{
			_animator.speed = 1f;
		}
	}
}
