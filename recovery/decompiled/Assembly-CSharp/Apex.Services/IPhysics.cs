using UnityEngine;

namespace Apex.Services;

public interface IPhysics
{
	Vector3 gravity { get; set; }

	float bounceThreshold { get; set; }

	int solverIterationCount { get; set; }

	bool CheckSphere(Vector3 position, float radius, int layerMask);

	bool CheckSphere(Vector3 position, float radius);

	bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layermask);

	bool CheckCapsule(Vector3 start, Vector3 end, float radius);

	void IgnoreCollision(Collider collider1, Collider collider2, bool ignore);

	void IgnoreCollision(Collider collider1, Collider collider2);

	void IgnoreLayerCollision(int layer1, int layer2, bool ignore);

	void IgnoreLayerCollision(int layer1, int layer2);

	bool GetIgnoreLayerCollision(int layer1, int layer2);

	bool Raycast(Vector3 origin, Vector3 direction, float distance);

	bool Raycast(Vector3 origin, Vector3 direction);

	bool Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask);

	bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance);

	bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo);

	bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask);

	bool Raycast(Ray ray, float distance);

	bool Raycast(Ray ray);

	bool Raycast(Ray ray, float distance, int layerMask);

	bool Raycast(Ray ray, out RaycastHit hitInfo, float distance);

	bool Raycast(Ray ray, out RaycastHit hitInfo);

	bool Raycast(Ray ray, out RaycastHit hitInfo, float distance, int layerMask);

	RaycastHit[] RaycastAll(Ray ray, float distance);

	RaycastHit[] RaycastAll(Ray ray);

	RaycastHit[] RaycastAll(Ray ray, float distance, int layerMask);

	RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float distance, int layermask);

	RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float distance);

	RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction);

	bool Linecast(Vector3 start, Vector3 end);

	bool Linecast(Vector3 start, Vector3 end, int layerMask);

	bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo);

	bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, int layerMask);

	Collider[] OverlapSphere(Vector3 position, float radius, int layerMask);

	Collider[] OverlapSphere(Vector3 position, float radius);

	bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance);

	bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction);

	bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance, int layerMask);

	bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float distance);

	bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo);

	bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask);

	bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float distance);

	bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo);

	bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask);

	bool SphereCast(Ray ray, float radius, float distance);

	bool SphereCast(Ray ray, float radius);

	bool SphereCast(Ray ray, float radius, float distance, int layerMask);

	bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float distance);

	bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo);

	bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float distance, int layerMask);

	RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance, int layermask);

	RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance);

	RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction);

	RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float distance);

	RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction);

	RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float distance, int layerMask);

	RaycastHit[] SphereCastAll(Ray ray, float radius, float distance);

	RaycastHit[] SphereCastAll(Ray ray, float radius);

	RaycastHit[] SphereCastAll(Ray ray, float radius, float distance, int layerMask);
}
