using System.Collections;
using UnityEngine;

namespace Apex.Common;

public abstract class AttributeSensitiveTrigger : MonoBehaviour
{
	[Tooltip("Whether the entity's attributes must be an exact match to trigger this trigger. If false the trigger will trigger if the entity has at least one of the attributes assigned to Triggered By")]
	public bool strictMatch;

	[Tooltip("Whether to invert the Triggered By mask, i.e. make it so that the trigger triggers for all except Triggered By.")]
	public bool invert;

	[SerializeField]
	[AttributeProperty("Triggered By", "The mask if attributes that trigger this trigger.")]
	private int _triggerdBy;

	private int _entitiesInTrigger;

	public AttributeMask triggeredBy
	{
		get
		{
			return _triggerdBy;
		}
		set
		{
			_triggerdBy = value;
		}
	}

	protected abstract IEnumerator OnTriggerEntered(Collider other, int entitiesInTrigger);

	protected abstract IEnumerator OnTriggerExited(Collider other, int entitiesInTrigger);

	private IEnumerator OnTriggerEnter(Collider other)
	{
		if (!IsAttributeMatch(other))
		{
			return null;
		}
		return OnTriggerEntered(other, ++_entitiesInTrigger);
	}

	private IEnumerator OnTriggerExit(Collider other)
	{
		if (!IsAttributeMatch(other))
		{
			return null;
		}
		return OnTriggerExited(other, --_entitiesInTrigger);
	}

	private bool IsAttributeMatch(Collider other)
	{
		AttributedComponent component = other.GetComponent<AttributedComponent>();
		if (component == null)
		{
			return false;
		}
		bool flag = false;
		flag = ((!strictMatch) ? (((int)component.attributes & _triggerdBy) > 0) : (((int)component.attributes & _triggerdBy) == _triggerdBy));
		if (invert)
		{
			return !flag;
		}
		return flag;
	}
}
