using UnityEngine;

namespace Apex.Steering;

public interface IDefineSpeed
{
	float maxAcceleration { get; }

	float maxDeceleration { get; }

	float maxAngularAcceleration { get; }

	float minimumSpeed { get; }

	float maximumSpeed { get; }

	float maximumAngularSpeed { get; }

	void SignalStop();

	void SetPreferredSpeed(float speed);

	float GetPreferredSpeed(Vector3 currentMovementDirection);

	void CloneFrom(IDefineSpeed speedComponent);
}
