using UnityEngine;

namespace Apex.Services;

public sealed class PhysicsWrapper : IPhysics
{
	public Vector3 gravity
	{
		get
		{
			return Physics.gravity;
		}
		set
		{
			Physics.gravity = value;
		}
	}

	public float bounceThreshold
	{
		get
		{
			return Physics.bounceThreshold;
		}
		set
		{
			Physics.bounceThreshold = value;
		}
	}

	public int solverIterationCount
	{
		get
		{
			return Physics.defaultSolverIterations;
		}
		set
		{
			Physics.defaultSolverIterations = value;
		}
	}

	public bool CheckSphere(Vector3 position, float radius, int layerMask)
	{
		return Physics.CheckSphere(position, radius, layerMask);
	}

	public bool CheckSphere(Vector3 position, float radius)
	{
		return Physics.CheckSphere(position, radius);
	}

	public bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layermask)
	{
		return Physics.CheckCapsule(start, end, radius, layermask);
	}

	public bool CheckCapsule(Vector3 start, Vector3 end, float radius)
	{
		return Physics.CheckCapsule(start, end, radius);
	}

	public void IgnoreCollision(Collider collider1, Collider collider2, bool ignore)
	{
		Physics.IgnoreCollision(collider1, collider2, ignore);
	}

	public void IgnoreCollision(Collider collider1, Collider collider2)
	{
		Physics.IgnoreCollision(collider1, collider2);
	}

	public void IgnoreLayerCollision(int layer1, int layer2, bool ignore)
	{
		Physics.IgnoreLayerCollision(layer1, layer2, ignore);
	}

	public void IgnoreLayerCollision(int layer1, int layer2)
	{
		Physics.IgnoreLayerCollision(layer1, layer2);
	}

	public bool GetIgnoreLayerCollision(int layer1, int layer2)
	{
		return Physics.GetIgnoreLayerCollision(layer1, layer2);
	}

	public bool Raycast(Vector3 origin, Vector3 direction, float distance)
	{
		return Physics.Raycast(origin, direction, distance);
	}

	public bool Raycast(Vector3 origin, Vector3 direction)
	{
		return Physics.Raycast(origin, direction);
	}

	public bool Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask)
	{
		return Physics.Raycast(origin, direction, distance, layerMask);
	}

	public bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance)
	{
		return Physics.Raycast(origin, direction, out hitInfo, distance);
	}

	public bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo)
	{
		return Physics.Raycast(origin, direction, out hitInfo);
	}

	public bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask)
	{
		return Physics.Raycast(origin, direction, out hitInfo, distance, layerMask);
	}

	public bool Raycast(Ray ray, float distance)
	{
		return Physics.Raycast(ray, distance);
	}

	public bool Raycast(Ray ray)
	{
		return Physics.Raycast(ray);
	}

	public bool Raycast(Ray ray, float distance, int layerMask)
	{
		return Physics.Raycast(ray, distance, layerMask);
	}

	public bool Raycast(Ray ray, out RaycastHit hitInfo, float distance)
	{
		return Physics.Raycast(ray, out hitInfo, distance);
	}

	public bool Raycast(Ray ray, out RaycastHit hitInfo)
	{
		return Physics.Raycast(ray, out hitInfo);
	}

	public bool Raycast(Ray ray, out RaycastHit hitInfo, float distance, int layerMask)
	{
		return Physics.Raycast(ray, out hitInfo, distance, layerMask);
	}

	public RaycastHit[] RaycastAll(Ray ray, float distance)
	{
		return Physics.RaycastAll(ray, distance);
	}

	public RaycastHit[] RaycastAll(Ray ray)
	{
		return Physics.RaycastAll(ray);
	}

	public RaycastHit[] RaycastAll(Ray ray, float distance, int layerMask)
	{
		return Physics.RaycastAll(ray, distance, layerMask);
	}

	public RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float distance, int layermask)
	{
		return Physics.RaycastAll(origin, direction, distance, layermask);
	}

	public RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float distance)
	{
		return Physics.RaycastAll(origin, direction, distance);
	}

	public RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction)
	{
		return Physics.RaycastAll(origin, direction);
	}

	public bool Linecast(Vector3 start, Vector3 end)
	{
		return Physics.Linecast(start, end);
	}

	public bool Linecast(Vector3 start, Vector3 end, int layerMask)
	{
		return Physics.Linecast(start, end, layerMask);
	}

	public bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo)
	{
		return Physics.Linecast(start, end, out hitInfo);
	}

	public bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, int layerMask)
	{
		return Physics.Linecast(start, end, out hitInfo, layerMask);
	}

	public Collider[] OverlapSphere(Vector3 position, float radius, int layerMask)
	{
		return Physics.OverlapSphere(position, radius, layerMask);
	}

	public Collider[] OverlapSphere(Vector3 position, float radius)
	{
		return Physics.OverlapSphere(position, radius);
	}

	public bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance)
	{
		return Physics.CapsuleCast(point1, point2, radius, direction, distance);
	}

	public bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction)
	{
		return Physics.CapsuleCast(point1, point2, radius, direction);
	}

	public bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance, int layerMask)
	{
		return Physics.CapsuleCast(point1, point2, radius, direction, distance, layerMask);
	}

	public bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float distance)
	{
		return Physics.CapsuleCast(point1, point2, radius, direction, out hitInfo, distance);
	}

	public bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo)
	{
		return Physics.CapsuleCast(point1, point2, radius, direction, out hitInfo);
	}

	public bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask)
	{
		return Physics.CapsuleCast(point1, point2, radius, direction, out hitInfo, distance, layerMask);
	}

	public bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float distance)
	{
		return Physics.SphereCast(origin, radius, direction, out hitInfo, distance);
	}

	public bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo)
	{
		return Physics.SphereCast(origin, radius, direction, out hitInfo);
	}

	public bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float distance, int layerMask)
	{
		return Physics.SphereCast(origin, radius, direction, out hitInfo, distance, layerMask);
	}

	public bool SphereCast(Ray ray, float radius, float distance)
	{
		return Physics.SphereCast(ray, radius, distance);
	}

	public bool SphereCast(Ray ray, float radius)
	{
		return Physics.SphereCast(ray, radius);
	}

	public bool SphereCast(Ray ray, float radius, float distance, int layerMask)
	{
		return Physics.SphereCast(ray, radius, distance, layerMask);
	}

	public bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float distance)
	{
		return Physics.SphereCast(ray, radius, out hitInfo, distance);
	}

	public bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo)
	{
		return Physics.SphereCast(ray, radius, out hitInfo);
	}

	public bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float distance, int layerMask)
	{
		return Physics.SphereCast(ray, radius, out hitInfo, distance, layerMask);
	}

	public RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance, int layermask)
	{
		return Physics.CapsuleCastAll(point1, point2, radius, direction, distance, layermask);
	}

	public RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float distance)
	{
		return Physics.CapsuleCastAll(point1, point2, radius, direction, distance);
	}

	public RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction)
	{
		return Physics.CapsuleCastAll(point1, point2, radius, direction);
	}

	public RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float distance)
	{
		return Physics.SphereCastAll(origin, radius, direction, distance);
	}

	public RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction)
	{
		return Physics.SphereCastAll(origin, radius, direction);
	}

	public RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float distance, int layerMask)
	{
		return Physics.SphereCastAll(origin, radius, direction, distance, layerMask);
	}

	public RaycastHit[] SphereCastAll(Ray ray, float radius, float distance)
	{
		return Physics.SphereCastAll(ray, radius, distance);
	}

	public RaycastHit[] SphereCastAll(Ray ray, float radius)
	{
		return Physics.SphereCastAll(ray, radius);
	}

	public RaycastHit[] SphereCastAll(Ray ray, float radius, float distance, int layerMask)
	{
		return Physics.SphereCastAll(ray, radius, distance, layerMask);
	}
}
