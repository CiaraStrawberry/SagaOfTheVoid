public struct SgtRectL(long newMinX, long newMinY, long newMaxX, long newMaxY)
{
	public long minX = newMinX;

	public long minY = newMinY;

	public long maxX = newMaxX;

	public long maxY = newMaxY;

	public long SizeX => maxX - minX;

	public long SizeY => maxY - minY;

	public void ClampTo(SgtRectL other)
	{
		if (minX < other.minX)
		{
			minX = other.minX;
		}
		else if (minX > other.maxX)
		{
			minX = other.maxX;
		}
		if (minY < other.minY)
		{
			minY = other.minY;
		}
		else if (minY > other.maxY)
		{
			minY = other.maxY;
		}
		if (maxX < other.minX)
		{
			maxX = other.minX;
		}
		else if (maxX > other.maxX)
		{
			maxX = other.maxX;
		}
		if (maxY < other.minY)
		{
			maxY = other.minY;
		}
		else if (maxY > other.maxY)
		{
			maxY = other.maxY;
		}
	}

	public SgtRectL GetExpanded(long amount)
	{
		return new SgtRectL(minX - amount, minY - amount, maxX + amount, maxY + amount);
	}

	public bool Contains(long x, long y)
	{
		return x >= minX && x < maxX && y >= minY && y < maxY;
	}

	public void Clear()
	{
		minX = (minY = (maxX = (maxY = 0L)));
	}

	public void SwapX()
	{
		long num = minX;
		minX = -maxX;
		maxX = -num;
	}

	public void SwapY()
	{
		long num = minY;
		minY = -maxY;
		maxY = -num;
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(SgtRectL a, SgtRectL b)
	{
		return a.minX == b.minX && a.minY == b.minY && a.maxX == b.maxX && a.maxY == b.maxY;
	}

	public static bool operator !=(SgtRectL a, SgtRectL b)
	{
		return a.minX != b.minX || a.minY != b.minY || a.maxX != b.maxX || a.maxY != b.maxY;
	}

	public override string ToString()
	{
		return "(" + minX + ", " + minY + " : " + maxX + ", " + maxY + ")";
	}
}
