using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General;

[CreateAssetMenu(menuName = "Devdog/Range trigger selector")]
public class RangeBestTriggerSelector : BestTriggerSelectorBase
{
	public float forwardEffect = 0.2f;

	public override TriggerBase GetBestTrigger(Player player, List<TriggerBase> triggersInRange)
	{
		float num = -999f;
		TriggerBase result = null;
		foreach (TriggerBase item in triggersInRange)
		{
			if (!(item == null) && item.enabled && item.gameObject.activeSelf)
			{
				Vector3 vector = item.transform.position - player.transform.position;
				float num2 = Vector3.Magnitude(vector);
				float num3 = Mathf.Clamp01(Vector3.Dot(player.transform.forward, vector / num2));
				num3 *= forwardEffect;
				float f = (ManagerBase<GeneralSettingsManager>.instance.settings.triggerUseDistance - num2) * num3;
				f = Mathf.Abs(f);
				if (f > num)
				{
					result = item;
					num = f;
				}
			}
		}
		return result;
	}
}
