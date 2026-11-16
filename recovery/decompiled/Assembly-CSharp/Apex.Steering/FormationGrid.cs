using Apex.Units;
using UnityEngine;

namespace Apex.Steering;

public class FormationGrid : IFormation
{
	private float _formationSpacing = 1.5f;

	public FormationGrid(float formationSpacing)
	{
		_formationSpacing = formationSpacing;
	}

	public Vector3 GetFormationPosition(int count, int formationIndex, IUnitFacade unit)
	{
		int num = Mathf.CeilToInt(Mathf.Sqrt(count));
		int num2 = Mathf.FloorToInt((float)count / (float)num);
		float num3 = (float)(formationIndex % num) - (float)num * 0.5f;
		float num4 = Mathf.FloorToInt((float)(formationIndex / num) - (float)num2 * 0.5f);
		return new Vector3(num3 * _formationSpacing, 0f, (0f - num4) * _formationSpacing);
	}
}
