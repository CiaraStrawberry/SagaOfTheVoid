using UnityEngine;

namespace VRTK;

[AddComponentMenu("VRTK/Scripts/Presence/VRTK_HeadsetCollision")]
public class VRTK_HeadsetCollision : MonoBehaviour
{
	[Tooltip("The radius of the auto generated sphere collider for detecting collisions on the headset.")]
	public float colliderRadius = 0.1f;

	[Tooltip("A specified VRTK_PolicyList to use to determine whether any objects will be acted upon by the Headset Collision.")]
	public VRTK_PolicyList targetListPolicy;

	[HideInInspector]
	public bool headsetColliding;

	[HideInInspector]
	public Collider collidingWith;

	protected Transform headset;

	protected VRTK_HeadsetCollider headsetColliderScript;

	protected GameObject headsetColliderContainer;

	protected bool generateCollider;

	protected bool generateRigidbody;

	public event HeadsetCollisionEventHandler HeadsetCollisionDetect;

	public event HeadsetCollisionEventHandler HeadsetCollisionEnded;

	public virtual void OnHeadsetCollisionDetect(HeadsetCollisionEventArgs e)
	{
		if (HeadsetCollisionDetect != null)
		{
			HeadsetCollisionDetect(this, e);
		}
	}

	public virtual void OnHeadsetCollisionEnded(HeadsetCollisionEventArgs e)
	{
		if (HeadsetCollisionEnded != null)
		{
			HeadsetCollisionEnded(this, e);
		}
	}

	public virtual bool IsColliding()
	{
		return headsetColliding;
	}

	public virtual GameObject GetHeadsetColliderContainer()
	{
		return headsetColliderContainer;
	}

	protected virtual void Awake()
	{
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void OnEnable()
	{
		VRTK_ObjectCache.registeredHeadsetCollider = this;
		headset = VRTK_DeviceFinder.HeadsetTransform();
		if (headset != null)
		{
			headsetColliding = false;
			SetupHeadset();
			VRTK_PlayerObject.SetPlayerObject(headsetColliderContainer.gameObject, VRTK_PlayerObject.ObjectTypes.Headset);
		}
	}

	protected virtual void OnDisable()
	{
		if (headset != null && headsetColliderScript != null)
		{
			headsetColliderScript.EndCollision(collidingWith);
			VRTK_ObjectCache.registeredHeadsetCollider = null;
			TearDownHeadset();
		}
	}

	protected virtual void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void Update()
	{
		if (headsetColliderContainer != null && headsetColliderContainer.transform.parent != headset)
		{
			headsetColliderContainer.transform.SetParent(headset);
			headsetColliderContainer.transform.localPosition = Vector3.zero;
			headsetColliderContainer.transform.localRotation = headset.localRotation;
		}
	}

	protected virtual void CreateHeadsetColliderContainer()
	{
		if (headsetColliderContainer == null)
		{
			headsetColliderContainer = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, "HeadsetColliderContainer"));
			headsetColliderContainer.transform.position = Vector3.zero;
			headsetColliderContainer.transform.localRotation = headset.localRotation;
			headsetColliderContainer.transform.localScale = Vector3.one;
			headsetColliderContainer.layer = LayerMask.NameToLayer("Ignore Raycast");
		}
	}

	protected virtual void SetupHeadset()
	{
		Rigidbody rigidbody = headset.GetComponentInChildren<Rigidbody>();
		if (rigidbody == null)
		{
			CreateHeadsetColliderContainer();
			rigidbody = headsetColliderContainer.AddComponent<Rigidbody>();
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			generateRigidbody = true;
		}
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
		Collider collider = headset.GetComponentInChildren<Collider>();
		if (collider == null)
		{
			CreateHeadsetColliderContainer();
			SphereCollider sphereCollider = headsetColliderContainer.gameObject.AddComponent<SphereCollider>();
			sphereCollider.radius = colliderRadius;
			collider = sphereCollider;
			generateCollider = true;
		}
		collider.isTrigger = true;
		if (headsetColliderScript == null)
		{
			GameObject gameObject = ((!headsetColliderContainer) ? headset.gameObject : headsetColliderContainer);
			headsetColliderScript = gameObject.AddComponent<VRTK_HeadsetCollider>();
			headsetColliderScript.SetParent(base.gameObject);
			headsetColliderScript.SetIgnoreTarget(targetListPolicy);
		}
	}

	protected virtual void TearDownHeadset()
	{
		if (generateCollider)
		{
			Object.Destroy(headset.gameObject.GetComponent<BoxCollider>());
		}
		if (generateRigidbody)
		{
			Object.Destroy(headset.gameObject.GetComponent<Rigidbody>());
		}
		if (headsetColliderScript != null)
		{
			Object.Destroy(headsetColliderScript);
		}
		if (headsetColliderContainer != null)
		{
			Object.Destroy(headsetColliderContainer);
		}
	}
}
