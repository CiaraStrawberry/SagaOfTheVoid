namespace Apex.Steering.HeightNavigation;

public interface IUnitHeightProvider
{
	float GetHeightDelta(SteeringInput input);
}
