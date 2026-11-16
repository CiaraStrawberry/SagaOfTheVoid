using System;
using UnityEngine;

namespace Devdog.General;

public interface IObjectRelevancy
{
	event Action OnBecameRelevant;

	event Action OnBecameIrrelevant;

	bool IsRelevant(GameObject obj);
}
