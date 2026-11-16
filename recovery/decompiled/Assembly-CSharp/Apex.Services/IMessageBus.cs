using System;

namespace Apex.Services;

public interface IMessageBus
{
	void Subscribe<T>(IHandleMessage<T> subscriber);

	void Unsubscribe<T>(IHandleMessage<T> subscriber);

	void Post<T>(T message);

	void PostBalanced<T>(T message, int maxMillisecondUsedPerFrame);

	void PostBalanced<T>(T message, int maxMillisecondUsedPerFrame, Action callback);
}
