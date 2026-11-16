using System;
using System.Collections;
using Devdog.General;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.SciFiDesign;

public class SciFiSceneLoaderHelper : MonoBehaviour
{
	public string format = "{0}%";

	[Required]
	public Text text;

	public int roundTo;

	public float waitDestroyTimeAfterLoad = 0.2f;

	protected virtual void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public virtual void LoadScene(string name)
	{
		Reset();
		StartCoroutine(_LoadScene(name));
	}

	public virtual void LoadScene(int id)
	{
		Reset();
		StartCoroutine(_LoadScene(id));
	}

	private IEnumerator _LoadScene(string name)
	{
		yield return new WaitForSeconds(0.2f);
		Repaint(SceneUtility.LoadSceneAsync(name));
	}

	private IEnumerator _LoadScene(int id)
	{
		yield return new WaitForSeconds(0.2f);
		Repaint(SceneUtility.LoadSceneAsync(id));
	}

	protected virtual void Repaint(AsyncOperation async)
	{
		StartCoroutine(_Repaint(async));
	}

	protected void Reset()
	{
		text.text = string.Format(format, 0f);
	}

	private IEnumerator _Repaint(AsyncOperation async)
	{
		while (!async.isDone)
		{
			text.text = string.Format(format, Math.Round(async.progress * 100f, roundTo));
			yield return null;
		}
	}
}
