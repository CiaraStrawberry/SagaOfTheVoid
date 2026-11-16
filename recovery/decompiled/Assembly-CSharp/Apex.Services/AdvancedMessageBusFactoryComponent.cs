using UnityEngine;

namespace Apex.Services;

[AddComponentMenu("Apex/Game World/Advanced MessageBus", 1027)]
[ApexComponent("Game World")]
public class AdvancedMessageBusFactoryComponent : MonoBehaviour, IMessageBusFactory
{
	public IMessageBus CreateMessageBus()
	{
		return new AdvancedMessageBus();
	}
}
