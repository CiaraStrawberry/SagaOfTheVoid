namespace Apex.Steering;

public interface ISteeringBehaviour
{
	int priority { get; }

	void GetSteering(SteeringInput input, SteeringOutput output);

	void Stop();
}
