using TrueSync;
using UnityEngine;

public class SimpleControl : TrueSyncBehaviour
{
	private const byte INPUT_KEY_HORIZONTAL = 0;

	private const byte INPUT_KEY_VERTICAL = 1;

	private const byte INPUT_KEY_CREATE = 2;

	public bool createdRuntime;

	public GameObject prefab;

	[AddTracking]
	private bool lastCreateState;

	public override void OnSyncedStart()
	{
		if (owner != null && owner.Id == 1)
		{
			GetComponent<Renderer>().material.color = Color.black;
		}
		if (!createdRuntime)
		{
			tsRigidBody.position = new TSVector(-3 + (owner.Id - 1) * 4, 1, 16);
		}
	}

	public override void OnSyncedInput()
	{
		if (!createdRuntime)
		{
			float axis = Input.GetAxis("Horizontal");
			float axis2 = Input.GetAxis("Vertical");
			bool key = Input.GetKey(KeyCode.Space);
			TrueSyncInput.SetInt(0, (int)(axis * 100f));
			TrueSyncInput.SetInt(1, (int)(axis2 * 100f));
			TrueSyncInput.SetBool(2, key);
		}
	}

	public override void OnSyncedUpdate()
	{
		FP fP = (FP)TrueSyncInput.GetInt(0) / (FP)100;
		FP fP2 = (FP)TrueSyncInput.GetInt(1) / (FP)100;
		bool flag = TrueSyncInput.GetBool(2);
		if (!lastCreateState && flag && !createdRuntime)
		{
			SimpleControl component = TrueSyncManager.SyncedInstantiate(prefab, tsTransform.position, tsTransform.rotation).GetComponent<SimpleControl>();
			component.createdRuntime = true;
			component.owner = owner;
			lastCreateState = flag;
			return;
		}
		TSVector zero = TSVector.zero;
		if (FP.Abs(fP) > FP.Zero)
		{
			zero.x = fP / 3;
		}
		if (FP.Abs(fP2) > FP.Zero)
		{
			zero.z = fP2 / 3;
		}
		tsRigidBody.AddForce(zero, ForceMode.Impulse);
		lastCreateState = flag;
	}

	public void OnSyncedCollisionEnter(TSCollision other)
	{
		if (other.gameObject.name == "Box(Clone)")
		{
			other.gameObject.GetComponent<Renderer>().material.color = Color.gray;
		}
	}

	public void OnSyncedCollisionStay(TSCollision other)
	{
		if (other.gameObject.name == "Box(Clone)")
		{
			other.gameObject.transform.localScale *= 1.01f;
		}
	}

	public void OnSyncedCollisionExit(TSCollision other)
	{
		if (other.gameObject.name == "Box(Clone)")
		{
			other.gameObject.transform.localScale = Vector3.one;
			other.gameObject.GetComponent<Renderer>().material.color = Color.blue;
		}
	}
}
