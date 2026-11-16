using System;

namespace Devdog.General;

public class LocalIdentifier : ILocalIdentifier, IEquatable<ILocalIdentifier>
{
	public string ID { get; protected set; }

	public LocalIdentifier(string ID)
	{
		this.ID = ID;
	}

	public static bool operator ==(LocalIdentifier a, LocalIdentifier b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(LocalIdentifier a, LocalIdentifier b)
	{
		return !(a == b);
	}

	public bool Equals(ILocalIdentifier other)
	{
		return ID == other.ID;
	}

	protected bool Equals(LocalIdentifier other)
	{
		return object.Equals(ID, other.ID);
	}

	public override bool Equals(object obj)
	{
		if (object.ReferenceEquals(null, obj))
		{
			return false;
		}
		if (object.ReferenceEquals(this, obj))
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((LocalIdentifier)obj);
	}

	public override int GetHashCode()
	{
		return ID.GetHashCode();
	}

	public override string ToString()
	{
		return ID;
	}
}
