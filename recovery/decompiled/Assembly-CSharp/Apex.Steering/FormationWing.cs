using Apex.Units;
using UnityEngine;

namespace Apex.Steering;

public class FormationWing : IFormation
{
	private float _formationSpacing = 1.5f;

	public FormationWing(float formationSpacing)
	{
		_formationSpacing = formationSpacing;
	}

	public Vector3 GetFormationPosition(int count, int formationIndex, IUnitFacade unit)
	{
		float z = Mathf.Abs((float)count * 0.5f - (float)formationIndex) * -0.5f * _formationSpacing;
		float x = ((float)count * 0.5f - (float)formationIndex) * (0f - _formationSpacing);
		return new Vector3(x, 0f, z);
	}
}
