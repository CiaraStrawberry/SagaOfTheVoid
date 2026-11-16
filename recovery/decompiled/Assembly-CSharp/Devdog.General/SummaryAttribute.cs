using System;

namespace Devdog.General;

[AttributeUsage(AttributeTargets.All)]
public class SummaryAttribute : Attribute
{
	public string summary { get; private set; }

	public SummaryAttribute(string summary)
	{
		this.summary = summary;
	}
}
