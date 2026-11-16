using Apex.Units;
using UnityEngine;

namespace Apex.Steering;

public interface IFormation
{
	Vector3 GetFormationPosition(int count, int formationIndex, IUnitFacade unit);
}
