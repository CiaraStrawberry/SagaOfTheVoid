using UnityEngine;

namespace VRTK.SecondaryControllerGrabActions;

[AddComponentMenu("VRTK/Scripts/Interactions/Secondary Controller Grab Actions/VRTK_SwapControllerGrabAction")]
public class VRTK_SwapControllerGrabAction : VRTK_BaseGrabAction
{
	protected virtual void Awake()
	{
		isActionable = false;
		isSwappable = true;
	}
}
