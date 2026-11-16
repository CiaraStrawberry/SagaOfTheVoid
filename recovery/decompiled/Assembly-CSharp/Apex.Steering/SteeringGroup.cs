using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apex.Steering;

public class SteeringGroup : ISteeringBehaviour
{
	private readonly SteeringOutput _memberOutput;

	private readonly List<ISteeringBehaviour> _steeringComponents;

	public int priority { get; private set; }

	public SteeringGroup(int priority)
	{
		this.priority = priority;
		_memberOutput = new SteeringOutput();
		_steeringComponents = new List<ISteeringBehaviour>();
	}

	public void Add(ISteeringBehaviour member)
	{
		if (member.priority != priority)
		{
			throw new ArgumentException("Invalid priority");
		}
		_steeringComponents.Add(member);
	}

	public void Remove(ISteeringBehaviour member)
	{
		_steeringComponents.Remove(member);
	}

	public void GetSteering(SteeringInput input, SteeringOutput output)
	{
		for (int i = 0; i < _steeringComponents.Count; i++)
		{
			ISteeringBehaviour steeringBehaviour = _steeringComponents[i];
			_memberOutput.Clear();
			steeringBehaviour.GetSteering(input, _memberOutput);
			output.overrideHeightNavigation |= _memberOutput.overrideHeightNavigation;
			if (_memberOutput.hasOutput)
			{
				output.desiredAcceleration += _memberOutput.desiredAcceleration;
				output.verticalForce += _memberOutput.verticalForce;
				output.maxAllowedSpeed = Mathf.Max(output.maxAllowedSpeed, _memberOutput.maxAllowedSpeed);
				output.pause |= _memberOutput.pause;
			}
		}
	}

	public void Stop()
	{
		for (int i = 0; i < _steeringComponents.Count; i++)
		{
			_steeringComponents[i].Stop();
		}
	}
}
