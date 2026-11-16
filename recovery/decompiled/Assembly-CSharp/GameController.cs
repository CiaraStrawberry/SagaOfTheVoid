using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public GameObject VrObject;

	private aGameState _CurrentState;

	public aGameState DefaultState;

	public GameObject WorldContainerObject;

	[SerializeField]
	private Vector3 SpawnPoint1local;

	[SerializeField]
	private Vector3 SpawnPoint2local;

	[SerializeField]
	private Vector3 SpawnPoint3local;

	[SerializeField]
	private Vector3 SpawnPoint4local;

	[SerializeField]
	private Vector3 SpawnPoint5local;

	[SerializeField]
	private Vector3 SpawnPoint6local;

	[SerializeField]
	private Vector3 SpawnPoint1localfreeforall;

	[SerializeField]
	private Vector3 SpawnPoint2localfreeforall;

	[SerializeField]
	private Vector3 SpawnPoint3localfreeforall;

	[SerializeField]
	private Vector3 SpawnPoint4localfreeforall;

	[SerializeField]
	private Vector3 SpawnPoint5localfreeforall;

	[SerializeField]
	private Vector3 SpawnPoint6localfreeforall;

	[HideInInspector]
	public Vector3 SpawnPoint1;

	[HideInInspector]
	public Vector3 SpawnPoint2;

	[HideInInspector]
	public Vector3 SpawnPoint3;

	[HideInInspector]
	public Vector3 SpawnPoint4;

	[HideInInspector]
	public Vector3 SpawnPoint5;

	[HideInInspector]
	public Vector3 SpawnPoint6;

	public List<aGameState> CachedStates;

	public void TransitionTo(aGameState go)
	{
		if (go != null)
		{
			go.OnTransitionTo();
		}
		else if (_CurrentState != null)
		{
			_CurrentState.OnTransitionFrom();
			_CurrentState.OnDeactivate();
			_CurrentState = null;
		}
		if (_CurrentState != null)
		{
			_CurrentState.OnTransitionFrom();
		}
		go.OnActive();
		_CurrentState = go;
	}

	public void convertspawnpoints(CrossLevelVariableHolder.gamemode input)
	{
		if (input == CrossLevelVariableHolder.gamemode.TeamDeathMatch)
		{
			GameObject gameObject = GameObject.Find("Objects");
			SpawnPoint1 = gameObject.transform.TransformPoint(SpawnPoint1local);
			SpawnPoint2 = gameObject.transform.TransformPoint(SpawnPoint2local);
			SpawnPoint3 = gameObject.transform.TransformPoint(SpawnPoint3local);
			SpawnPoint4 = gameObject.transform.TransformPoint(SpawnPoint4local);
			SpawnPoint5 = gameObject.transform.TransformPoint(SpawnPoint5local);
			SpawnPoint6 = gameObject.transform.TransformPoint(SpawnPoint6local);
		}
		if (input == CrossLevelVariableHolder.gamemode.FreeForAll)
		{
			GameObject gameObject2 = GameObject.Find("Objects");
			SpawnPoint1 = gameObject2.transform.TransformPoint(SpawnPoint1localfreeforall);
			SpawnPoint2 = gameObject2.transform.TransformPoint(SpawnPoint2localfreeforall);
			SpawnPoint3 = gameObject2.transform.TransformPoint(SpawnPoint3localfreeforall);
			SpawnPoint4 = gameObject2.transform.TransformPoint(SpawnPoint4localfreeforall);
			SpawnPoint5 = gameObject2.transform.TransformPoint(SpawnPoint5localfreeforall);
			SpawnPoint6 = gameObject2.transform.TransformPoint(SpawnPoint6localfreeforall);
		}
		if (input == CrossLevelVariableHolder.gamemode.CapitalShip)
		{
			GameObject gameObject3 = GameObject.Find("Objects");
			SpawnPoint1 = gameObject3.transform.TransformPoint(SpawnPoint1local);
			SpawnPoint2 = gameObject3.transform.TransformPoint(SpawnPoint2local);
			SpawnPoint3 = gameObject3.transform.TransformPoint(SpawnPoint3local);
			SpawnPoint4 = gameObject3.transform.TransformPoint(SpawnPoint4local);
			SpawnPoint5 = gameObject3.transform.TransformPoint(SpawnPoint5local);
			SpawnPoint6 = gameObject3.transform.TransformPoint(SpawnPoint6local);
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SOV\\"))
		{
			Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SOV\\");
		}
		if (DefaultState != null)
		{
			_CurrentState = DefaultState;
			_CurrentState.transform.parent = transform;
			_CurrentState.OnTransitionTo();
			_CurrentState.OnActive();
		}
	}

	private void Update()
	{
	}
}
