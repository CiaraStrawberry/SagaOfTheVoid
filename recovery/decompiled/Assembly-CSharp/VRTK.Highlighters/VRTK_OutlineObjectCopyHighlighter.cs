using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Highlighters;

[AddComponentMenu("VRTK/Scripts/Interactions/Highlighters/VRTK_OutlineObjectCopyHighlighter")]
public class VRTK_OutlineObjectCopyHighlighter : VRTK_BaseHighlighter
{
	[Tooltip("The thickness of the outline effect")]
	public float thickness = 1f;

	[Tooltip("The GameObjects to use as the model to outline. If one isn't provided then the first GameObject with a valid Renderer in the current GameObject hierarchy will be used.")]
	public GameObject[] customOutlineModels;

	[Tooltip("A path to a GameObject to find at runtime, if the GameObject doesn't exist at edit time.")]
	public string[] customOutlineModelPaths;

	[Tooltip("If the mesh has multiple sub-meshes to highlight then this should be checked, otherwise only the first mesh will be highlighted.")]
	public bool enableSubmeshHighlight;

	public Material stencilOutline;

	protected GameObject[] highlightModels;

	protected string[] copyComponents = new string[2] { "UnityEngine.MeshFilter", "UnityEngine.MeshRenderer" };

	public List<GameObject> listofcopiedmodels = new List<GameObject>();

	public List<GameObject> listoforiginalmodels = new List<GameObject>();

	public override void Initialise(Color? color = null, Dictionary<string, object> options = null)
	{
		usesClonedObject = true;
		if (stencilOutline == null)
		{
			stencilOutline = UnityEngine.Object.Instantiate((Material)Resources.Load("OutlineBasic"));
		}
		SetOptions(options);
		ResetHighlighter();
	}

	public override void ResetHighlighter()
	{
		DeleteExistingHighlightModels();
		ResetHighlighterWithCustomModelPaths();
		ResetHighlighterWithCustomModels();
		ResetHighlightersWithCurrentGameObject();
	}

	public override void Highlight(Color? color, float duration = 0f)
	{
		Debug.Log("highlighted try");
		if (highlightModels == null || highlightModels.Length <= 0)
		{
			return;
		}
		Debug.Log("highlighted!");
		stencilOutline.SetFloat("_Thickness", thickness);
		stencilOutline.SetColor("_OutlineColor", color.Value);
		for (int i = 0; i < highlightModels.Length; i++)
		{
			if ((bool)highlightModels[i])
			{
				highlightModels[i].SetActive(value: true);
			}
		}
	}

	public override void Unhighlight(Color? color = null, float duration = 0f)
	{
		if (highlightModels == null)
		{
			return;
		}
		for (int i = 0; i < highlightModels.Length; i++)
		{
			if ((bool)highlightModels[i])
			{
				highlightModels[i].SetActive(value: false);
			}
		}
	}

	protected virtual void OnEnable()
	{
		if (customOutlineModels == null)
		{
			customOutlineModels = new GameObject[0];
		}
		if (customOutlineModelPaths == null)
		{
			customOutlineModelPaths = new string[0];
		}
	}

	protected virtual void OnDestroy()
	{
		if (highlightModels != null)
		{
			for (int i = 0; i < highlightModels.Length; i++)
			{
				if ((bool)highlightModels[i])
				{
					UnityEngine.Object.Destroy(highlightModels[i]);
				}
			}
		}
		UnityEngine.Object.Destroy(stencilOutline);
	}

	protected virtual void ResetHighlighterWithCustomModels()
	{
		if (customOutlineModels != null && customOutlineModels.Length > 0)
		{
			highlightModels = new GameObject[customOutlineModels.Length];
			for (int i = 0; i < customOutlineModels.Length; i++)
			{
				highlightModels[i] = CreateHighlightModel(customOutlineModels[i], string.Empty);
			}
		}
	}

	protected virtual void ResetHighlighterWithCustomModelPaths()
	{
		if (customOutlineModelPaths != null && customOutlineModelPaths.Length > 0)
		{
			highlightModels = new GameObject[customOutlineModels.Length];
			for (int i = 0; i < customOutlineModelPaths.Length; i++)
			{
				highlightModels[i] = CreateHighlightModel(null, customOutlineModelPaths[i]);
			}
		}
	}

	protected virtual void ResetHighlightersWithCurrentGameObject()
	{
		if (highlightModels != null && highlightModels.Length != 0)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>();
		int num = 0;
		foreach (Transform item in base.transform)
		{
			if (item.GetComponent<MeshRenderer>() != null)
			{
				list.Add(item.gameObject);
				num++;
			}
		}
		highlightModels = new GameObject[num];
		int num2 = 0;
		foreach (GameObject item2 in list)
		{
			highlightModels[num2] = CreateHighlightModel(item2, string.Empty);
			num2++;
		}
	}

	protected virtual void SetOptions(Dictionary<string, object> options = null)
	{
		float option = GetOption<float>(options, "thickness");
		if (option > 0f)
		{
			thickness = option;
		}
		GameObject[] option2 = GetOption<GameObject[]>(options, "customOutlineModels");
		if (option2 != null)
		{
			customOutlineModels = option2;
		}
		string[] option3 = GetOption<string[]>(options, "customOutlineModelPaths");
		if (option3 != null)
		{
			customOutlineModelPaths = option3;
		}
	}

	protected virtual void DeleteExistingHighlightModels()
	{
		VRTK_PlayerObject[] componentsInChildren = GetComponentsInChildren<VRTK_PlayerObject>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].objectType == VRTK_PlayerObject.ObjectTypes.Highlighter)
			{
				UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
			}
		}
	}

	protected virtual GameObject CreateHighlightModel(GameObject givenOutlineModel, string givenOutlineModelPath)
	{
		if (givenOutlineModel != null)
		{
			givenOutlineModel = ((!givenOutlineModel.GetComponent<Renderer>()) ? givenOutlineModel.GetComponentInChildren<Renderer>().gameObject : givenOutlineModel);
		}
		else if (givenOutlineModelPath != string.Empty)
		{
			Transform transform = base.transform.Find(givenOutlineModelPath);
			givenOutlineModel = ((!transform) ? null : transform.gameObject);
		}
		GameObject gameObject = givenOutlineModel;
		if (gameObject == null)
		{
			gameObject = ((!GetComponent<Renderer>()) ? GetComponentInChildren<Renderer>().gameObject : base.gameObject);
		}
		if (gameObject == null)
		{
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_OutlineObjectCopyHighlighter", "Renderer", "the same or child", " to add the highlighter to"));
			return null;
		}
		GameObject gameObject2 = new GameObject(name + "_HighlightModel");
		gameObject2.transform.SetParent(gameObject.transform.parent, worldPositionStays: false);
		gameObject2.transform.localPosition = gameObject.transform.localPosition;
		gameObject2.transform.localRotation = gameObject.transform.localRotation;
		gameObject2.transform.localScale = gameObject.transform.localScale;
		gameObject2.transform.SetParent(base.transform);
		if (!listofcopiedmodels.Contains(gameObject2))
		{
			listofcopiedmodels.Add(gameObject2);
		}
		if (!listoforiginalmodels.Contains(gameObject))
		{
			listoforiginalmodels.Add(gameObject);
		}
		Component[] components = gameObject.GetComponents<Component>();
		foreach (Component component in components)
		{
			if (Array.IndexOf(copyComponents, component.GetType().ToString()) >= 0)
			{
				VRTK_SharedMethods.CloneComponent(component, gameObject2);
			}
		}
		MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
		MeshFilter component3 = gameObject2.GetComponent<MeshFilter>();
		if ((bool)component3)
		{
			List<CombineInstance> list = new List<CombineInstance>();
			for (int j = 0; j < component2.mesh.subMeshCount; j++)
			{
				list.Add(new CombineInstance
				{
					mesh = component2.mesh,
					subMeshIndex = j,
					transform = component2.transform.localToWorldMatrix
				});
			}
			component3.mesh = new Mesh();
			component3.mesh.CombineMeshes(list.ToArray(), mergeSubMeshes: true, useMatrices: false);
			gameObject2.GetComponent<Renderer>().material = stencilOutline;
		}
		gameObject2.SetActive(value: false);
		VRTK_PlayerObject.SetPlayerObject(gameObject2, VRTK_PlayerObject.ObjectTypes.Highlighter);
		return gameObject2;
	}
}
