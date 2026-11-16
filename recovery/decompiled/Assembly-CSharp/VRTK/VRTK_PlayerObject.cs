using UnityEngine;

namespace VRTK;

public sealed class VRTK_PlayerObject : MonoBehaviour
{
	public enum ObjectTypes
	{
		Null,
		CameraRig,
		Headset,
		Controller,
		Pointer,
		Highlighter,
		Collider
	}

	public ObjectTypes objectType;

	public static void SetPlayerObject(GameObject obj, ObjectTypes objType)
	{
		VRTK_PlayerObject vRTK_PlayerObject = obj.GetComponent<VRTK_PlayerObject>();
		if (vRTK_PlayerObject == null)
		{
			vRTK_PlayerObject = obj.AddComponent<VRTK_PlayerObject>();
		}
		vRTK_PlayerObject.objectType = objType;
	}

	public static bool IsPlayerObject(GameObject obj, ObjectTypes ofType = ObjectTypes.Null)
	{
		VRTK_PlayerObject[] componentsInParent = obj.GetComponentsInParent<VRTK_PlayerObject>(includeInactive: true);
		foreach (VRTK_PlayerObject vRTK_PlayerObject in componentsInParent)
		{
			if (ofType == ObjectTypes.Null || ofType == vRTK_PlayerObject.objectType)
			{
				return true;
			}
		}
		return false;
	}
}
