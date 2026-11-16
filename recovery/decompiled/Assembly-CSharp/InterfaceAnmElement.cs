using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InterfaceAnmElement : MonoBehaviour
{
	public GameObject gameObjectRef;

	public float timeAppear = 1f;

	public float timeDisappear = 1f;

	public bool recycling;

	public int sortID;

	public Animator animator;

	public AnimationClip[] animClips;

	public static List<InterfaceAnmElement> list = new List<InterfaceAnmElement>();

	public CSFHIAnimableState currentState = CSFHIAnimableState.disappeared;

	public bool isNested_IAM;

	public static InterfaceAnmElement Create(GameObject _gameObjectRef, int _newSortID, bool _isNested_IAM)
	{
		InterfaceAnmElement component;
		if ((bool)_gameObjectRef.GetComponent<InterfaceAnmElement>())
		{
			component = _gameObjectRef.GetComponent<InterfaceAnmElement>();
			return component.Confirm(_gameObjectRef, _newSortID, _isNested_IAM);
		}
		foreach (InterfaceAnmElement item in list)
		{
			if (item.recycling)
			{
				component = item;
				return component.Confirm(_gameObjectRef, _newSortID, _isNested_IAM);
			}
		}
		component = _gameObjectRef.AddComponent<InterfaceAnmElement>();
		return component.Confirm(_gameObjectRef, _newSortID, _isNested_IAM);
	}

	public virtual InterfaceAnmElement Confirm(GameObject _gameObjectRef, int _newSortID, bool isNested_IAM)
	{
		gameObjectRef = _gameObjectRef;
		sortID = _newSortID;
		this.isNested_IAM = isNested_IAM;
		recycling = false;
		if (!this.isNested_IAM)
		{
			animator = _gameObjectRef.GetComponent<Animator>();
		}
		if (!list.Contains(this))
		{
			list.Add(this);
		}
		return this;
	}

	public void UpdateProperties()
	{
		if ((bool)gameObjectRef.GetComponent<InterfaceAnimManager>() && (bool)gameObjectRef.transform.parent.GetComponent<InterfaceAnimManager>())
		{
			isNested_IAM = true;
			animator = null;
		}
		else
		{
			isNested_IAM = false;
			animator = gameObjectRef.GetComponent<Animator>();
		}
	}

	public virtual void Delete()
	{
		gameObjectRef = null;
		sortID = 0;
		timeAppear = 1f;
		timeDisappear = 1f;
		isNested_IAM = false;
		currentState = CSFHIAnimableState.disappeared;
		recycling = true;
	}

	public void Update()
	{
		if ((bool)gameObjectRef && !gameObjectRef.GetComponent<InterfaceAnimManager>())
		{
		}
	}
}
