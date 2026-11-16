using UnityEngine;

namespace Apex.Steering.Components;

public static class ExtensionMethods
{
	public static Vector3 DirToXYZ(this Vector3 from, Vector3 to)
	{
		return new Vector3(to.x - from.x, to.y - from.y, to.z - from.z);
	}
}
