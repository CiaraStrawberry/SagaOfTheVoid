using UnityEngine;

namespace Apex.LoadBalancing;

[AddComponentMenu("")]
public class LoadBalancerComponentOld : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	public LoadBalancerConfig[] _configurations;

	[SerializeField]
	public int _mashallerMaxMillisecondPerFrame = 4;

	private void Awake()
	{
		Debug.LogWarning("This scene contains deprecated components, please run Tools -> Apex -> Update Scene.");
	}
}
