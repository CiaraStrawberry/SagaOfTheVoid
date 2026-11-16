using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General;

[CreateAssetMenu(menuName = "Devdog/Raycast trigger selector")]
public class RaycastBestTriggerSelector : BestTriggerSelectorBase
{
	public LayerMask layerMask = -1;

	public float raycastDistance = 10f;

	public override TriggerBase GetBestTrigger(Player player, List<TriggerBase> triggersInRange)
	{
		Camera main = Camera.main;
		if (main == null)
		{
			return null;
		}
		Debug.DrawRay(main.transform.position, main.transform.forward * raycastDistance, Color.red, 0.2f);
		if (Physics.Raycast(main.transform.position, main.transform.forward, out var hitInfo, raycastDistance))
		{
			TriggerBase component = hitInfo.transform.GetComponent<TriggerBase>();
			if (component != null && component.enabled)
			{
				return component;
			}
		}
		return null;
	}
}
