using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.SciFiDesign.UI;

public class TimerUIHelper : MonoBehaviour
{
	public void EnableMaskAfterSeconds(float waitTime)
	{
		StartCoroutine(_SetEnabledAfterSeconds(GetComponent<Mask>(), state: true, waitTime));
	}

	public void DisableMaskAfterSeconds(float waitTime)
	{
		StartCoroutine(_SetEnabledAfterSeconds(GetComponent<Mask>(), state: false, waitTime));
	}

	private IEnumerator _SetEnabledAfterSeconds(MonoBehaviour comp, bool state, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		comp.enabled = state;
	}

	public void SetActiveAfter(float waitTime)
	{
		if (waitTime > 0f)
		{
			StartCoroutine(_SetActive(waitTime, set: true));
		}
		else
		{
			_SetActive(set: true);
		}
	}

	public void SetInActiveAfter(float waitTime)
	{
		if (waitTime > 0f)
		{
			StartCoroutine(_SetActive(waitTime, set: false));
		}
		else
		{
			_SetActive(set: false);
		}
	}

	private IEnumerator _SetActive(float waitTime, bool set)
	{
		yield return new WaitForSeconds(waitTime);
		_SetActive(set);
	}

	private void _SetActive(bool set)
	{
		gameObject.SetActive(set);
	}
}
