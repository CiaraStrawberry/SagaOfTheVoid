using System;
using UnityEngine;

[Serializable]
public class SceneField
{
	[SerializeField]
	private UnityEngine.Object m_SceneAsset;

	[SerializeField]
	private string m_SceneName = string.Empty;

	public string SceneName => m_SceneName;

	public static implicit operator string(SceneField sceneField)
	{
		return sceneField.SceneName;
	}
}
