using UnityEngine;

namespace Devdog.General;

[DisallowMultipleComponent]
public class Player2D : Player
{
	protected override void SetTriggerHandler()
	{
		GameObject gameObject = new GameObject("_TriggerHandler2D");
		gameObject.transform.SetParent(transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.gameObject.layer = 2;
		PlayerTriggerHandler2D playerTriggerHandler2D = gameObject.AddComponent<PlayerTriggerHandler2D>();
		playerTriggerHandler2D.player = this;
		playerTriggerHandler2D.selector = triggerSelector;
		triggerHandler = playerTriggerHandler2D;
	}
}
