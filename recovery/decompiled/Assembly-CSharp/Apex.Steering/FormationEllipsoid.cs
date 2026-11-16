using System;
using Apex.Units;
using UnityEngine;

namespace Apex.Steering;

public class FormationEllipsoid : IFormation
{
	private float _formationRadius = 3f;

	public FormationEllipsoid(float formationRadius)
	{
		_formationRadius = formationRadius;
	}

	public Vector3 GetFormationPosition(int count, int formationIndex, IUnitFacade unit)
	{
		float num = (float)count / ((float)Math.PI * 2f) * _formationRadius;
		float num2 = (float)Math.PI * 2f / (float)count;
		float f = num2 * (float)formationIndex;
		float x = Mathf.Cos(f);
		float z = Mathf.Sin(f);
		return new Vector3(x, 0f, z) * num;
	}
}
