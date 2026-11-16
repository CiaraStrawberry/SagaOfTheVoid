using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Apex.DataStructures;
using Apex.Utilities;

namespace Apex.PathFinding;

public sealed class PathService : IPathService, IDisposable
{
	private const int StartQueueSize = 10;

	private Stopwatch _stopwatch;

	private PriorityQueueFifo<IPathRequest> _queue;

	private IPathingEngine _engine;

	private bool _threadPoolSupported = true;

	private bool _processingActive;

	private AutoResetEvent _waitHandle;

	private IThreadFactory _threadFactory;

	private Thread _dedicatedThread;

	public Action OnAsyncFailed { get; set; }

	public bool runAsync { get; set; }

	public PathService(IPathingEngine engine, IThreadFactory threadFactory)
		: this(engine, threadFactory, useThreadPoolForAsync: true)
	{
	}

	public PathService(IPathingEngine engine, IThreadFactory threadFactory, bool useThreadPoolForAsync)
	{
		Ensure.ArgumentNotNull(engine, "engine");
		Ensure.ArgumentNotNull(threadFactory, "threadFactory");
		_stopwatch = new Stopwatch();
		_queue = new PriorityQueueFifo<IPathRequest>(10, QueueType.Max);
		_engine = engine;
		_threadPoolSupported = useThreadPoolForAsync;
		_threadFactory = threadFactory;
	}

	public void QueueRequest(IPathRequest request)
	{
		QueueRequest(request, 0);
	}

	public void QueueRequest(IPathRequest request, int priority)
	{
		Ensure.ArgumentNotNull(request, "request");
		lock (_queue)
		{
			_queue.Enqueue(request, priority);
			if (runAsync && !_processingActive)
			{
				StartAsyncProcessing();
			}
		}
	}

	public IEnumerator ProcessRequests(int maxMillisecondsPerFrame)
	{
		if (runAsync)
		{
			throw new InvalidOperationException("Cannot process as coroutine when set to async operation.");
		}
		while (!runAsync)
		{
			IPathRequest next = GetNext();
			if (next == null)
			{
				if (_stopwatch.IsRunning)
				{
					_stopwatch.Reset();
				}
				yield return null;
				continue;
			}
			bool run = true;
			IEnumerator subIter = _engine.ProcessRequestCoroutine(next);
			while (run)
			{
				_stopwatch.Start();
				run = subIter.MoveNext();
				if (_stopwatch.ElapsedMilliseconds > maxMillisecondsPerFrame)
				{
					_stopwatch.Reset();
					yield return null;
				}
			}
		}
	}

	public void ProcessRequests()
	{
		for (IPathRequest next = GetNext(); next != null; next = GetNext())
		{
			_engine.ProcessRequest(next);
		}
	}

	private void ProcessRequestsAsync(object ignored)
	{
		try
		{
			ProcessRequests();
		}
		catch (OutOfMemoryException)
		{
			throw;
		}
		catch
		{
			_processingActive = false;
		}
	}

	private void ProcessRequestsAsyncDedicated(object ignored)
	{
		try
		{
			while (runAsync)
			{
				ProcessRequests();
				_waitHandle.WaitOne();
			}
		}
		catch (OutOfMemoryException)
		{
			throw;
		}
		catch
		{
			_processingActive = false;
			_dedicatedThread = null;
		}
	}

	private void StartAsyncProcessing()
	{
		_processingActive = true;
		if (_threadPoolSupported)
		{
			try
			{
				_threadPoolSupported = ThreadPool.QueueUserWorkItem(ProcessRequestsAsync);
			}
			catch (NotSupportedException)
			{
				_threadPoolSupported = false;
			}
		}
		if (_threadPoolSupported)
		{
			return;
		}
		if (_dedicatedThread != null)
		{
			_waitHandle.Set();
			return;
		}
		try
		{
			if (_waitHandle == null)
			{
				_waitHandle = new AutoResetEvent(initialState: true);
			}
			Thread thread = _threadFactory.CreateThread("Pathing", ProcessRequestsAsyncDedicated);
			thread.IsBackground = true;
			_dedicatedThread = thread;
			thread.Start();
		}
		catch
		{
			runAsync = false;
			if (_waitHandle != null)
			{
				_waitHandle.Close();
			}
			_processingActive = false;
			_dedicatedThread = null;
			_waitHandle = null;
			OnAsyncFailed?.Invoke();
		}
	}

	private IPathRequest GetNext()
	{
		lock (_queue)
		{
			while (_queue.hasNext)
			{
				IPathRequest pathRequest = _queue.Dequeue();
				if (!pathRequest.hasDecayed)
				{
					return pathRequest;
				}
				pathRequest.Complete(new PathResult(PathingStatus.Decayed, null, 0, pathRequest));
			}
			_processingActive = false;
		}
		return null;
	}

	public void Dispose()
	{
		lock (_queue)
		{
			_queue.Clear();
			runAsync = false;
		}
		if (_waitHandle != null)
		{
			_waitHandle.Set();
			_waitHandle.Close();
		}
		_dedicatedThread = null;
		_waitHandle = null;
	}
}
