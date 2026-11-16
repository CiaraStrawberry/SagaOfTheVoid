using System;
using UnityEngine;
using UnityEngine.Events;

namespace Devdog.SciFiDesign;

public class EventOnTrigger : MonoBehaviour
{
	[Serializable]
	public class OnTriggerEvent : UnityEvent<Collider>
	{
	}

	public OnTriggerEvent onTriggerEnter;

	public OnTriggerEvent onTriggerExit;

	protected void OnTriggerEnter(Collider col)
	{
		onTriggerEnter.Invoke(col);
	}

	protected void OnTriggerExit(Collider col)
	{
		onTriggerEnter.Invoke(col);
	}
}
