using UnityEngine;

namespace Apex.Steering;

public interface IMoveUnits
{
	void Move(Vector3 velocity, float deltaTime);

	void Rotate(Vector3 targetOrientation, float angularSpeed, float deltaTime);

	void Stop();
}
