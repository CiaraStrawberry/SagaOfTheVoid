using System;

namespace Devdog.General.ThirdParty.FullSerializer;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class fsIgnoreAttribute : Attribute
{
}
