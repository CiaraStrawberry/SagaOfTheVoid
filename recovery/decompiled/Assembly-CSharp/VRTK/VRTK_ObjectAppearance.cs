using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.Highlighters;

namespace VRTK;

public class VRTK_ObjectAppearance : MonoBehaviour
{
	protected static VRTK_ObjectAppearance instance;

	protected Dictionary<GameObject, Coroutine> setOpacityCoroutines = new Dictionary<GameObject, Coroutine>();

	public static void SetOpacity(GameObject model, float alpha, float transitionDuration = 0f)
	{
		SetupInstance();
		if (instance != null)
		{
			instance.InternalSetOpacity(model, alpha, transitionDuration);
		}
	}

	public static void SetRendererVisible(GameObject model, GameObject ignoredModel = null)
	{
		SetupInstance();
		if (instance != null)
		{
			instance.InternalSetRendererVisible(model, ignoredModel);
		}
	}

	public static void SetRendererHidden(GameObject model, GameObject ignoredModel = null)
	{
		SetupInstance();
		if (instance != null)
		{
			instance.InternalSetRendererHidden(model, ignoredModel);
		}
	}

	public static void ToggleRenderer(bool state, GameObject model, GameObject ignoredModel = null)
	{
		if (state)
		{
			SetRendererVisible(model, ignoredModel);
		}
		else
		{
			SetRendererHidden(model, ignoredModel);
		}
	}

	public static void HighlightObject(GameObject model, Color? highlightColor, float fadeDuration = 0f)
	{
		SetupInstance();
		if (instance != null)
		{
			instance.InternalHighlightObject(model, highlightColor, fadeDuration);
		}
	}

	public static void UnhighlightObject(GameObject model)
	{
		SetupInstance();
		if (instance != null)
		{
			instance.InternalUnhighlightObject(model);
		}
	}

	protected virtual void OnDisable()
	{
		foreach (KeyValuePair<GameObject, Coroutine> setOpacityCoroutine in setOpacityCoroutines)
		{
			CancelSetOpacityCoroutine(setOpacityCoroutine.Key);
		}
	}

	protected static void SetupInstance()
	{
		if (instance == null && VRTK_SDKManager.instance != null)
		{
			instance = VRTK_SDKManager.instance.gameObject.AddComponent<VRTK_ObjectAppearance>();
		}
	}

	protected virtual void InternalSetOpacity(GameObject model, float alpha, float transitionDuration = 0f)
	{
		if (!model || !model.activeInHierarchy)
		{
			return;
		}
		if (transitionDuration == 0f)
		{
			ChangeRendererOpacity(model, alpha);
			return;
		}
		CancelSetOpacityCoroutine(model);
		Coroutine value = StartCoroutine(TransitionRendererOpacity(model, GetInitialAlpha(model), alpha, transitionDuration));
		if (!setOpacityCoroutines.ContainsKey(model))
		{
			setOpacityCoroutines.Add(model, value);
		}
	}

	protected virtual void InternalSetRendererVisible(GameObject model, GameObject ignoredModel = null)
	{
		if (model != null)
		{
			Renderer[] componentsInChildren = model.GetComponentsInChildren<Renderer>(includeInactive: true);
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.gameObject != ignoredModel && (ignoredModel == null || !renderer.transform.IsChildOf(ignoredModel.transform)))
				{
					renderer.enabled = true;
				}
			}
		}
		EmitControllerEvents(model, state: true);
	}

	protected virtual void InternalSetRendererHidden(GameObject model, GameObject ignoredModel = null)
	{
		if (model != null)
		{
			Renderer[] componentsInChildren = model.GetComponentsInChildren<Renderer>(includeInactive: true);
			foreach (Renderer renderer in componentsInChildren)
			{
				if (renderer.gameObject != ignoredModel && (ignoredModel == null || !renderer.transform.IsChildOf(ignoredModel.transform)))
				{
					renderer.enabled = false;
				}
			}
		}
		EmitControllerEvents(model, state: false);
	}

	protected virtual void InternalHighlightObject(GameObject model, Color? highlightColor, float fadeDuration = 0f)
	{
		VRTK_BaseHighlighter componentInChildren = model.GetComponentInChildren<VRTK_BaseHighlighter>();
		if (model.activeInHierarchy && componentInChildren != null)
		{
			componentInChildren.Highlight(highlightColor ?? new Color?(Color.white), fadeDuration);
		}
	}

	protected virtual void InternalUnhighlightObject(GameObject model)
	{
		VRTK_BaseHighlighter componentInChildren = model.GetComponentInChildren<VRTK_BaseHighlighter>();
		if (model.activeInHierarchy && componentInChildren != null)
		{
			componentInChildren.Unhighlight();
		}
	}

	protected virtual void EmitControllerEvents(GameObject model, bool state)
	{
		GameObject gameObject = null;
		if (VRTK_DeviceFinder.GetModelAliasControllerHand(model) == SDK_BaseController.ControllerHand.Left)
		{
			gameObject = VRTK_DeviceFinder.GetControllerLeftHand();
		}
		else if (VRTK_DeviceFinder.GetModelAliasControllerHand(model) == SDK_BaseController.ControllerHand.Right)
		{
			gameObject = VRTK_DeviceFinder.GetControllerRightHand();
		}
		if (!(gameObject != null) || !gameObject.activeInHierarchy)
		{
			return;
		}
		VRTK_ControllerEvents component = gameObject.GetComponent<VRTK_ControllerEvents>();
		if (component != null)
		{
			if (state)
			{
				component.OnControllerVisible(component.SetControllerEvent());
			}
			else
			{
				component.OnControllerHidden(component.SetControllerEvent());
			}
		}
	}

	protected virtual void ChangeRendererOpacity(GameObject model, float alpha)
	{
		if (!(model != null))
		{
			return;
		}
		alpha = Mathf.Clamp(alpha, 0f, 1f);
		Renderer[] componentsInChildren = model.GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (Renderer renderer in componentsInChildren)
		{
			if (alpha < 1f)
			{
				renderer.material.SetInt("_SrcBlend", 1);
				renderer.material.SetInt("_DstBlend", 10);
				renderer.material.SetInt("_ZWrite", 0);
				renderer.material.DisableKeyword("_ALPHATEST_ON");
				renderer.material.DisableKeyword("_ALPHABLEND_ON");
				renderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				renderer.material.renderQueue = 3000;
			}
			else
			{
				renderer.material.SetInt("_SrcBlend", 1);
				renderer.material.SetInt("_DstBlend", 0);
				renderer.material.SetInt("_ZWrite", 1);
				renderer.material.DisableKeyword("_ALPHATEST_ON");
				renderer.material.DisableKeyword("_ALPHABLEND_ON");
				renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				renderer.material.renderQueue = -1;
			}
			if (renderer.material.HasProperty("_Color"))
			{
				renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
			}
		}
	}

	protected virtual float GetInitialAlpha(GameObject model)
	{
		Renderer componentInChildren = model.GetComponentInChildren<Renderer>(includeInactive: true);
		if (componentInChildren.material.HasProperty("_Color"))
		{
			return componentInChildren.material.color.a;
		}
		return 0f;
	}

	protected virtual IEnumerator TransitionRendererOpacity(GameObject model, float initialAlpha, float targetAlpha, float transitionDuration)
	{
		float elapsedTime = 0f;
		while (elapsedTime < transitionDuration)
		{
			float newAlpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsedTime / transitionDuration);
			ChangeRendererOpacity(model, newAlpha);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		ChangeRendererOpacity(model, targetAlpha);
	}

	protected virtual void CancelSetOpacityCoroutine(GameObject model)
	{
		if (setOpacityCoroutines.ContainsKey(model) && setOpacityCoroutines[model] != null)
		{
			StopCoroutine(setOpacityCoroutines[model]);
		}
	}
}
