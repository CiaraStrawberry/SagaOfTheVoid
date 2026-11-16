using System;

namespace Devdog.General;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class CustomSerializationAttribute : Attribute
{
}
