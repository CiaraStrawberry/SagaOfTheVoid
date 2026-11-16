namespace TrueSync.Physics3D;

public class UsageBitfield
{
	private bool _usedVertexA;

	private bool _usedVertexB;

	private bool _usedVertexC;

	private bool _usedVertexD;

	public bool UsedVertexA
	{
		get
		{
			return _usedVertexA;
		}
		set
		{
			_usedVertexA = value;
		}
	}

	public bool UsedVertexB
	{
		get
		{
			return _usedVertexB;
		}
		set
		{
			_usedVertexB = value;
		}
	}

	public bool UsedVertexC
	{
		get
		{
			return _usedVertexC;
		}
		set
		{
			_usedVertexC = value;
		}
	}

	public bool UsedVertexD
	{
		get
		{
			return _usedVertexD;
		}
		set
		{
			_usedVertexD = value;
		}
	}

	public void Reset()
	{
		_usedVertexA = (_usedVertexB = (_usedVertexC = (_usedVertexD = false)));
	}
}
