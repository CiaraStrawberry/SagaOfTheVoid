using UnityEngine;

namespace Apex.Common;

[AddComponentMenu("Apex/Game World/Attribute Manipulating Trigger", 1028)]
[ApexComponent("Triggers")]
public class AttributeManipulatingTrigger : MonoBehaviour
{
	public enum Trigger
	{
		OnEnter = 1,
		OnExit,
		Both
	}

	[Tooltip("When the trigger behaviour should happen, on entry, exit or both.")]
	public Trigger updateOn = Trigger.Both;

	[SerializeField]
	[AttributeProperty("Applies", "The attribute mask applied when this trigger is triggered.")]
	private int _applies;

	[SerializeField]
	[AttributeProperty("Removes", "The attribute mask removed when this trigger is triggered.")]
	private int _removes;

	public AttributeMask applies
	{
		get
		{
			return _applies;
		}
		set
		{
			_applies = value;
		}
	}

	public AttributeMask removes
	{
		get
		{
			return _removes;
		}
		set
		{
			_removes = value;
		}
	}

	private static void Apply(Collider other, int apply, int remove)
	{
		AttributedComponent component = other.GetComponent<AttributedComponent>();
		if (!(component == null))
		{
			component.attributes = (int)component.attributes | apply;
			component.attributes = (int)component.attributes & ~remove;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((updateOn & Trigger.OnEnter) > (Trigger)0)
		{
			Apply(other, _applies, _removes);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (updateOn == Trigger.OnExit)
		{
			Apply(other, _applies, _removes);
		}
		else if ((updateOn & Trigger.OnExit) > (Trigger)0)
		{
			Apply(other, _removes, _applies);
		}
	}
}
