namespace Apex.Services;

public interface IHandleMessage<T>
{
	void Handle(T message);
}
