using Apex.Units;
using UnityEngine;

namespace Apex.Steering;

public class FormationLine : IFormation
{
	private float _formationSpacing = 1.5f;

	public FormationLine(float formationSpacing)
	{
		_formationSpacing = formationSpacing;
	}

	public Vector3 GetFormationPosition(int count, int formationIndex, IUnitFacade unit)
	{
		float num = (float)count * 0.5f;
		return new Vector3(0f, 0f, ((float)formationIndex - num) * _formationSpacing);
	}
}
