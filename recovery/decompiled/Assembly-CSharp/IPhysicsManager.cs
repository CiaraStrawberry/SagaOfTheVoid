using System;
using TrueSync;
using UnityEngine;

public interface IPhysicsManager : IPhysicsManagerBase
{
	TSVector Gravity { get; set; }

	bool SpeculativeContacts { get; set; }

	FP LockedTimeStep { get; set; }

	GameObject GetGameObject(IBody rigidBody);

	int GetBodyLayer(IBody rigidBody);

	bool IsCollisionEnabled(IBody rigidBody1, IBody rigidBody2);

	void AddBody(ICollider iCollider);

	void OnRemoveBody(Action<IBody> OnRemoveBody);
}
