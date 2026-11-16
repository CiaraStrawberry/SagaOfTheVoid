using System;

namespace Devdog.General;

public interface ILocalIdentifier : IEquatable<ILocalIdentifier>
{
	string ID { get; }

	new string ToString();
}
