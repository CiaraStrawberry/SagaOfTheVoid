using TrueSync;
using TrueSync.Physics3D;
using UnityEngine;
using UnityEngine.UI;

public class LockPlayer : TrueSyncBehaviour
{
	public Transform graphics;

	public GameObject nickPrefab;

	private byte MOUSE_X;

	private byte MOUSE_Y = 1;

	private TSVector destination;

	public TSVector movement;

	public FP speed = 0.15;

	private SphereShape ss;

	private Text nickLabel;

	[AddTracking]
	public FP currentScale = FP.One;

	public FP growSlow = 1.125f;

	public FP growFast = 1.25f;

	private FP originalRadius;

	private Vector3 originalGraphicsScale;

	private bool onFocus;

	private void OnApplicationFocus(bool focusStatus)
	{
		onFocus = focusStatus;
	}

	public override void OnSyncedStartLocalPlayer()
	{
		Camera.main.GetComponent<TopDownCamera>().target = transform;
	}

	public override void OnSyncedStart()
	{
		nickLabel = Object.Instantiate(nickPrefab).GetComponent<Text>();
		nickLabel.transform.SetParent(GameObject.Find("Canvas").transform, worldPositionStays: false);
		nickLabel.text = owner.Name;
		ss = (SphereShape)GetComponent<TSSphereCollider>().Shape;
		tsRigidBody.position += new TSVector(owner.Id * 2, 0, 0);
		originalRadius = ss.Radius;
		originalGraphicsScale = graphics.localScale;
	}

	public override void OnSyncedInput()
	{
		if (!onFocus)
		{
			TrueSyncInput.SetInt(MOUSE_X, (int)transform.position.x);
			TrueSyncInput.SetInt(MOUSE_Y, (int)transform.position.z);
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out var hitInfo))
		{
			TrueSyncInput.SetInt(MOUSE_X, (int)hitInfo.point.x);
			TrueSyncInput.SetInt(MOUSE_Y, (int)hitInfo.point.z);
		}
	}

	public override void OnSyncedUpdate()
	{
		destination.x = TrueSyncInput.GetInt(MOUSE_X);
		destination.z = TrueSyncInput.GetInt(MOUSE_Y);
		UpdateShapes();
		Steer();
	}

	private void UpdateShapes()
	{
		ss.Radius = originalRadius * currentScale;
		graphics.localScale = originalGraphicsScale * (float)currentScale;
	}

	private void Steer()
	{
		movement = destination - tsRigidBody.position;
		if (!(movement.sqrMagnitude < FP.One))
		{
			movement.Normalize();
			movement *= TrueSyncManager.DeltaTime * speed / currentScale;
			tsRigidBody.position += movement;
		}
	}

	public void OnSyncedCollisionEnter(TSCollision other)
	{
		if (other.gameObject.tag == "Food")
		{
			currentScale *= growSlow;
			TSRigidBody rigidbody = other.rigidbody;
			rigidbody.position = new TSVector(TSRandom.Range(-35, 35), 0, TSRandom.Range(-20, 20));
		}
		else if (other.gameObject.tag == "Player")
		{
			LockPlayer component = other.gameObject.GetComponent<LockPlayer>();
			if (ss.Radius > component.ss.Radius)
			{
				currentScale *= growFast;
				component.tsRigidBody.position = new TSVector(TSRandom.Range(-35, 35), 0, TSRandom.Range(-20, 20));
				component.currentScale = FP.One;
			}
		}
	}

	private void LateUpdate()
	{
		if (nickLabel != null)
		{
			nickLabel.transform.position = Camera.main.WorldToScreenPoint(transform.position);
		}
	}
}
