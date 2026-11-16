using System;

namespace Devdog.General.ThirdParty.FullSerializer;

public abstract class fsConverter : fsBaseConverter
{
	public abstract bool CanProcess(Type type);
}
