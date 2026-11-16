using UnityEngine;

namespace TrueSync;

public abstract class TrueSyncBehaviour : MonoBehaviour, ITrueSyncBehaviourGamePlay, ITrueSyncBehaviourCallbacks, ITrueSyncBehaviour
{
	[HideInInspector]
	public int numberOfPlayers;

	public int ownerIndex = -1;

	[HideInInspector]
	public TSPlayerInfo owner;

	[HideInInspector]
	public TSPlayerInfo localOwner;

	private TSTransform _tsTransform;

	private TSTransform2D _tsTransform2D;

	private TSRigidBody _tsRigidBody;

	private TSRigidBody2D _tsRigidBody2D;

	private TSCollider _tsCollider;

	private TSCollider2D _tsCollider2D;

	public TSTransform tsTransform
	{
		get
		{
			if (_tsTransform == null)
			{
				_tsTransform = GetComponent<TSTransform>();
			}
			return _tsTransform;
		}
	}

	public TSTransform2D tsTransform2D
	{
		get
		{
			if (_tsTransform2D == null)
			{
				_tsTransform2D = GetComponent<TSTransform2D>();
			}
			return _tsTransform2D;
		}
	}

	public TSRigidBody tsRigidBody
	{
		get
		{
			if (_tsRigidBody == null)
			{
				_tsRigidBody = GetComponent<TSRigidBody>();
			}
			return _tsRigidBody;
		}
	}

	public TSRigidBody2D tsRigidBody2D
	{
		get
		{
			if (_tsRigidBody2D == null)
			{
				_tsRigidBody2D = GetComponent<TSRigidBody2D>();
			}
			return _tsRigidBody2D;
		}
	}

	public TSCollider tsCollider
	{
		get
		{
			if (_tsCollider == null)
			{
				_tsCollider = GetComponent<TSCollider>();
			}
			return _tsCollider;
		}
	}

	public TSCollider2D tsCollider2D
	{
		get
		{
			if (_tsCollider2D == null)
			{
				_tsCollider2D = GetComponent<TSCollider2D>();
			}
			return _tsCollider2D;
		}
	}

	public void SetGameInfo(TSPlayerInfo localOwner, int numberOfPlayers)
	{
	}

	public virtual void OnSyncedStart()
	{
	}

	public virtual void OnSyncedStartLocalPlayer()
	{
	}

	public virtual void OnGamePaused()
	{
	}

	public virtual void OnGameUnPaused()
	{
	}

	public virtual void OnGameEnded()
	{
	}

	public virtual void OnPreSyncedUpdate()
	{
	}

	public virtual void OnSyncedUpdate()
	{
	}

	public virtual void OnSyncedInput()
	{
	}

	public virtual void OnPlayerDisconnection(int playerId)
	{
	}
}
