using System;

namespace Devdog.General.ThirdParty.FullSerializer.Internal;

public struct fsOption<T>(T value)
{
	private bool _hasValue = true;

	private T _value = value;

	public static fsOption<T> Empty;

	public bool HasValue => _hasValue;

	public bool IsEmpty => !_hasValue;

	public T Value
	{
		get
		{
			if (IsEmpty)
			{
				throw new InvalidOperationException("fsOption is empty");
			}
			return _value;
		}
	}
}
public static class fsOption
{
	public static fsOption<T> Just<T>(T value)
	{
		return new fsOption<T>(value);
	}
}
