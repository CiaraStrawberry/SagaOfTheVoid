using UnityEngine;

namespace Apex.Common;

public abstract class AttributedComponent : ExtendedMonoBehaviour, IHaveAttributes
{
	[SerializeField]
	[AttributeProperty("Attributes", "The custom attributes of this entity.")]
	private int _attributeMask;

	public AttributeMask attributes
	{
		get
		{
			return _attributeMask;
		}
		set
		{
			if (_attributeMask != value)
			{
				int attributeMask = _attributeMask;
				_attributeMask = value;
				OnAttributesChanged(attributeMask);
			}
		}
	}

	protected virtual void OnAttributesChanged(AttributeMask previous)
	{
	}
}
