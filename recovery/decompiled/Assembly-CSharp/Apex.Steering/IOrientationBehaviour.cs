namespace Apex.Steering;

public interface IOrientationBehaviour
{
	int priority { get; }

	void GetOrientation(SteeringInput input, OrientationOutput output);
}
