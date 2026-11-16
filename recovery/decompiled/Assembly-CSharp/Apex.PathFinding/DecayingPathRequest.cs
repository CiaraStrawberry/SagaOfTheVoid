using System.Diagnostics;

namespace Apex.PathFinding;

public class DecayingPathRequest : BasicPathRequest
{
	private int _timeLimit;

	private bool _hardDecay;

	private Stopwatch _w;

	public override bool hasDecayed
	{
		get
		{
			return _hardDecay || _w.ElapsedMilliseconds >= _timeLimit;
		}
		set
		{
			_hardDecay = value;
		}
	}

	public DecayingPathRequest(int timeLimitInMilliseconds)
	{
		_timeLimit = timeLimitInMilliseconds;
		_w = Stopwatch.StartNew();
	}
}
