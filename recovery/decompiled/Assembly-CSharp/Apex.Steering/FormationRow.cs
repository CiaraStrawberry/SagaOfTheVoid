using Apex.Units;
using UnityEngine;

namespace Apex.Steering;

public class FormationRow : IFormation
{
	private float _formationSpacing = 1.5f;

	public FormationRow(float formationSpacing)
	{
		_formationSpacing = formationSpacing;
	}

	public Vector3 GetFormationPosition(int count, int formationIndex, IUnitFacade unit)
	{
		float num = (float)(count - 1) * -0.5f * _formationSpacing;
		return new Vector3(num + (float)formationIndex * _formationSpacing, 0f, 0f);
	}
}
