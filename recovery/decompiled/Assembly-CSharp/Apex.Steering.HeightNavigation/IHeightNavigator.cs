namespace Apex.Steering.HeightNavigation;

public interface IHeightNavigator
{
	float gravity { get; }

	HeightOutput GetHeightOutput(SteeringInput input, float effectiveMaxSpeed);

	void CloneFrom(IHeightNavigator other);
}
