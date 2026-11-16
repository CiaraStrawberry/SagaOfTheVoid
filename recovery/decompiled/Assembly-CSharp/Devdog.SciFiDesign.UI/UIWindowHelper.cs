using Devdog.General.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Devdog.SciFiDesign.UI;

public class UIWindowHelper : MonoBehaviour
{
	public UnityEvent onParentWindowShow;

	public UnityEvent onParentWindowHide;

	protected void Start()
	{
		UIWindow componentInParent = GetComponentInParent<UIWindow>();
	}
}
