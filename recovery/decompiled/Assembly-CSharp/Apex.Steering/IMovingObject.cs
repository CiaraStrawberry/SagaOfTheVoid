using UnityEngine;

namespace Apex.Steering;

public interface IMovingObject
{
	Vector3 velocity { get; }

	Vector3 actualVelocity { get; }

	bool isGrounded { get; }

	void Stop();

	void Wait(float? seconds);

	void Resume();
}
